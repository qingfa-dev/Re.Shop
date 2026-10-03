using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class ResultFlowSpec
{
    [Fact]
    public void NonGeneric_Map_And_Match_Should_Run_Success_Operations()
    {
        var result = Result.Ok()
            .Map(() => 6);

        result.Value.ShouldBe(6);
        result.Match(value => value, _ => 0).ShouldBe(6);
    }

    [Fact]
    public void NonGeneric_Map_Should_Propagate_Failure_Without_Invoking_Mapper()
    {
        var failure = Result.Fail(ResultStub.NotFoundError());
        var mapperCalled = false;

        var mapped = failure.Map(() =>
        {
            mapperCalled = true;
            return 1;
        });

        mapperCalled.ShouldBeFalse();
        mapped.IsFailure.ShouldBeTrue();
        mapped.Errors.ShouldBe(failure.Errors);
        mapped.StatusCode.ShouldBe(failure.StatusCode);
    }

    [Fact]
    public void Generic_Map_Match_And_Value_Access_Should_Chain_On_Success()
    {
        var result = Result<int>.Ok(2)
            .Map(value => value * 3)
            .Ensure(value => value == 6, _ => ResultStub.ConflictError());

        result.Value.ShouldBe(6);
        result.Match(value => value, _ => 0).ShouldBe(6);
        result.ValueOrThrow().ShouldBe(6);
        result.ValueOr(-1).ShouldBe(6);
    }

    [Fact]
    public void Bind_Should_Run_Both_Generic_And_NonGeneric_Binders()
    {
        var source = Result<int>.Ok(2);

        var generic = source.Bind(value => Result<string>.Ok((value * 3).ToString()));
        var nonGeneric = source.Bind(value =>
            value > 0 ? Result.Ok() : Result.Fail(ResultStub.ConflictError()));

        generic.Value.ShouldBe("6");
        nonGeneric.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Match_Should_Use_Failure_Callback_For_Failed_Result()
    {
        var result = Result<int>.Fail(ResultStub.NotFoundError());

        var message = result.Match(_ => "success", errors => errors[0].Code);

        message.ShouldBe("test.not_found");
    }

    [Fact]
    public void Match_Should_Allow_Only_The_Inactive_Callback_To_Be_Missing()
    {
        var result = Result<int>.Ok(1)
            .Match<int>(onSuccess: null, onFailure: _ => 9);

        result.ShouldBe(default);
    }

    [Fact]
    public void Map_Should_Return_Flow_Failure_When_The_Active_Mapper_Is_Missing()
    {
        var result = Result<int>.Ok(1).Map<int, int>(mapper: null);

        result.IsFailure.ShouldBeTrue();
        result.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
    }

    [Fact]
    public void Bind_And_Ensure_Should_Return_Flow_Failures_When_Active_Callbacks_Are_Missing()
    {
        var missingBinder = Result<int>.Ok(1)
            .Bind<int, string>(binder: null);
        var missingPredicate = Result<int>.Ok(1)
            .Ensure(predicate: null, error: ResultStub.NotFoundError);

        missingBinder.IsFailure.ShouldBeTrue();
        missingBinder.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingPredicate.IsFailure.ShouldBeTrue();
        missingPredicate.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
    }

    [Fact]
    public void Ensure_Should_Accept_A_Direct_Error_Or_Zero_Argument_Error_Factory()
    {
        var directError = Result<int>.Ok(0)
            .Ensure(_ => false, errorValue: ResultStub.ConflictError());
        var factoryError = Result<int>.Ok(0)
            .Ensure(_ => false, error: ResultStub.NotFoundError);

        directError.Errors[0].Code.ShouldBe("test.conflict");
        factoryError.Errors[0].Code.ShouldBe("test.not_found");
    }

    [Fact]
    public void Ensure_Should_Return_Flow_Error_When_The_Error_Factory_Returns_Null()
    {
        var result = Result<int>.Ok(0)
            .Ensure(_ => false, error: (Func<IError>)(() => null!));
        var missingError = Result<int>.Ok(0)
            .Ensure(_ => false, errorValue: null);

        result.IsFailure.ShouldBeTrue();
        result.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ErrorMissing.Code);
        missingError.IsFailure.ShouldBeTrue();
        missingError.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
    }

    [Fact]
    public void Recover_Should_Replace_Failures_And_Return_Flow_Error_When_Handler_Is_Missing()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        var recovered = failure.Recover(errors => errors.Count);
        var withoutHandler = failure.Recover<int>((Func<IReadOnlyList<BuildingBlock.Kernel.Errors.IError>, int>?)null);

        recovered.IsSuccess.ShouldBeTrue();
        recovered.Value.ShouldBe(1);
        withoutHandler.IsFailure.ShouldBeTrue();
        withoutHandler.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);

        var recoveredResult = failure.Recover(errors => Result<int>.Ok(errors.Count + 1));
        recoveredResult.Value.ShouldBe(2);
        Result.Fail(ResultStub.NotFoundError())
            .Recover(_ => Result.Ok())
            .IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Switch_And_Tap_Should_Run_Only_The_Selected_Handler_And_Allow_Omissions()
    {
        var successValue = 0;
        var errorCount = 0;
        var success = Result<int>.Ok(3);
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        success.Tap(value => successValue = value)
            .TapError(_ => errorCount++);
        failure.Switch(
            onSuccess: _ => successValue = -1,
            onFailure: errors => errorCount += errors.Count);
        success.Switch<int>(onSuccess: null, onFailure: null);

        successValue.ShouldBe(3);
        errorCount.ShouldBe(1);
    }

    [Fact]
    public async Task Async_Flow_Methods_Should_Await_Selected_Callbacks_And_Propagate_Results()
    {
        var source = Result<int>.Ok(2);
        var mapped = await source.MapAsync((value, _) => Task.FromResult(value * 3));
        var bound = await mapped.BindAsync((value, _) =>
            Task.FromResult(Result<string>.Ok(value.ToString())));
        var ensured = await bound.EnsureAsync(
            (value, _) => Task.FromResult(value == "6"),
            ResultStub.ConflictError());
        var ensuredWithValueFactory = await Result<int>.Ok(0).EnsureAsync(
            (_, _) => Task.FromResult(false),
            _ => ResultStub.ConflictError());
        var ensuredWithAsyncFactory = await Result<int>.Ok(0).EnsureAsync(
            (_, _) => Task.FromResult(false),
            (_, _) => Task.FromResult<IError>(ResultStub.NotFoundError()));
        var switchedValue = 0;
        await ensured.SwitchAsync(
            (value, _) =>
            {
                switchedValue = value.Length;
                return Task.CompletedTask;
            },
            (_, _) => Task.CompletedTask);
        await ensured.TapAsync((value, _) =>
        {
            switchedValue += value.Length;
            return Task.CompletedTask;
        });
        await ensured.TapErrorAsync((_, _) => Task.CompletedTask);

        var match = await ensured.MatchAsync(
            (value, _) => Task.FromResult(value),
            (_, _) => Task.FromResult("failure"));

        bound.Value.ShouldBe("6");
        ensured.IsSuccess.ShouldBeTrue();
        ensuredWithValueFactory.Errors[0].Code.ShouldBe("test.conflict");
        ensuredWithAsyncFactory.Errors[0].Code.ShouldBe("test.not_found");
        switchedValue.ShouldBe(2);
        match.ShouldBe("6");
    }

    [Fact]
    public async Task Async_Flow_Should_Support_NonGeneric_Map_Bind_And_Recovery()
    {
        var mapped = await Result.Ok().MapAsync((_) => Task.FromResult(2));
        var bound = await mapped.BindAsync((value, _) =>
            Task.FromResult(value == 2 ? Result.Ok() : Result.Fail(ResultStub.ConflictError())));
        var recovered = await Result.Fail(ResultStub.NotFoundError())
            .RecoverAsync((errors, _) => Task.FromResult(Result.Ok()));
        var recoveredValue = await Result<int>.Fail(ResultStub.NotFoundError())
            .RecoverAsync((errors, _) => Task.FromResult(errors.Count));

        mapped.Value.ShouldBe(2);
        bound.IsSuccess.ShouldBeTrue();
        recovered.IsSuccess.ShouldBeTrue();
        recoveredValue.Value.ShouldBe(1);
    }

    [Fact]
    public async Task Async_Switch_And_TapError_Should_Invoke_Failure_Callbacks()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());
        var switchedCount = 0;
        var tappedCount = 0;

        var switched = await failure.SwitchAsync(
            (_, _) => Task.CompletedTask,
            (errors, _) =>
            {
                switchedCount = errors.Count;
                return Task.CompletedTask;
            });
        var tapped = await failure.TapErrorAsync((errors, _) =>
        {
            tappedCount = errors.Count;
            return Task.CompletedTask;
        });

        switched.ShouldBeSameAs(failure);
        tapped.ShouldBeSameAs(failure);
        switchedCount.ShouldBe(1);
        tappedCount.ShouldBe(1);
    }

    [Fact]
    public async Task Async_Flow_Should_Convert_Missing_Active_Callbacks_Into_Result_Errors()
    {
        var missingMap = await Result.Ok().MapAsync<int>(mapper: null);
        var missingRecover = await Result<int>.Fail(ResultStub.NotFoundError())
            .RecoverAsync<int>((Func<IReadOnlyList<IError>, CancellationToken, Task<int>>?)null);
        var missingPredicate = await Result<int>.Ok(1)
            .EnsureAsync((Func<int, CancellationToken, Task<bool>>?)null, ResultStub.NotFoundError());
        var nullMapTask = await Result<int>.Ok(1)
            .MapAsync<int, int>((_, _) => null!);
        var nullMatchTask = await Result<int>.Ok(1)
            .MatchAsync<int, string>((_, _) => null!, null);

        missingMap.IsFailure.ShouldBeTrue();
        missingMap.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingRecover.IsFailure.ShouldBeTrue();
        missingRecover.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingPredicate.IsFailure.ShouldBeTrue();
        missingPredicate.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        nullMapTask.IsFailure.ShouldBeTrue();
        nullMapTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        nullMatchTask.ShouldBeNull();
    }

    [Fact]
    public async Task Async_Side_Effect_Should_Return_Flow_Error_When_Callback_Returns_Null_Task()
    {
        var tapped = await Result<int>.Ok(1)
            .TapAsync((_, _) => null!);
        var switched = await Result<int>.Ok(1)
            .SwitchAsync((_, _) => null!);

        tapped.IsFailure.ShouldBeTrue();
        tapped.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        switched.IsFailure.ShouldBeTrue();
        switched.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public void NonGeneric_Match_Should_Choose_Callback_By_State()
    {
        var success = Result.Ok();
        var failure = Result.Fail(ResultStub.NotFoundError());

        success.Match(() => "ok", _ => "failed").ShouldBe("ok");
        failure.Match(() => "ok", errors => errors[0].Code).ShouldBe("test.not_found");
    }

    [Fact]
    public void NonGeneric_Match_Should_Allow_Omitted_Callbacks_And_Still_Reject_Null_Result()
    {
        var result = Result.Ok();

        Action nullResult = () => ((Result)null!).Match(() => "ok", _ => "failed");

        Should.Throw<ArgumentNullException>(nullResult).ParamName.ShouldBe("result");
        result.Match<string>(null, _ => "failed").ShouldBeNull();
        result.Match(() => "ok", null).ShouldBe("ok");
    }

    [Fact]
    public void Generic_Map_When_Failure_Should_Propagate_Without_Invoking_Mapper()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());
        var mapperCalled = false;

        var mapped = failure.Map(value =>
        {
            mapperCalled = true;
            return value * 2;
        });

        mapperCalled.ShouldBeFalse();
        mapped.IsFailure.ShouldBeTrue();
        mapped.StatusCode.ShouldBe(failure.StatusCode);
    }

    [Fact]
    public void Bind_When_Failure_Should_Propagate_Without_Invoking_Binder()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());
        var genericCalled = false;
        var nonGenericCalled = false;

        var generic = failure.Bind(value =>
        {
            genericCalled = true;
            return Result<string>.Ok(value.ToString());
        });
        var nonGeneric = failure.Bind(value =>
        {
            nonGenericCalled = true;
            return Result.Ok();
        });

        genericCalled.ShouldBeFalse();
        nonGenericCalled.ShouldBeFalse();
        generic.IsFailure.ShouldBeTrue();
        nonGeneric.IsFailure.ShouldBeTrue();
        generic.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    [Fact]
    public void Ensure_When_Failure_Should_Return_Original_Without_Invoking()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());
        var predicateCalled = false;
        var violationCalled = false;

        var result = failure.Ensure(
            value =>
            {
                predicateCalled = true;
                return false;
            },
            value =>
            {
                violationCalled = true;
                return ResultStub.ConflictError();
            });

        predicateCalled.ShouldBeFalse();
        violationCalled.ShouldBeFalse();
        result.ShouldBeSameAs(failure);
    }

    [Fact]
    public void Ensure_When_Predicate_Violated_Should_Fail_With_Violation_Error()
    {
        var success = Result<int>.Ok(1);

        var result = success.Ensure(value => value > 10, _ => ResultStub.ConflictError());

        result.IsFailure.ShouldBeTrue();
        result.Errors[0].Code.ShouldBe("test.conflict");
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Conflict);
    }

    [Fact]
    public void Ensure_When_OnViolation_Returns_Null_Should_Return_Flow_Error()
    {
        var success = Result<int>.Ok(1);

        var act = () => success.Ensure(value => false, _ => null!);

        var result = act();
        result.IsFailure.ShouldBeTrue();
        result.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ErrorMissing.Code);
    }

    [Fact]
    public void ValueOrThrow_When_Failure_Should_Throw_With_Error_Text()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        var ex = Should.Throw<InvalidOperationException>(() => _ = failure.ValueOrThrow());
        var second = Should.Throw<InvalidOperationException>(() => _ = failure.ValueOrThrow());

        ex.Message.ShouldContain("test.not_found");
        second.Message.ShouldContain("test.not_found");
    }

    [Fact]
    public void ValueOr_When_Failure_Should_Return_Fallback()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        failure.ValueOr(-1).ShouldBe(-1);
    }

    [Fact]
    public void NonGeneric_Match_With_Failure_And_Null_Callback_Should_Return_Default()
    {
        var result = Result.Fail(ResultStub.NotFoundError()).Match(() => "ok", null);

        result.ShouldBeNull();
    }

    [Fact]
    public void Generic_Match_With_Failure_And_Null_Callback_Should_Return_Default_And_Reject_Null_Result()
    {
        var defaulted = Result<int>.Fail(ResultStub.NotFoundError()).Match(_ => 1, null);

        defaulted.ShouldBe(0);
        Action nullResult = () => ((Result<int>)null!).Match(_ => 1, null);

        Should.Throw<ArgumentNullException>(nullResult).ParamName.ShouldBe("result");
    }

    [Fact]
    public void NonGeneric_Map_With_Null_Mapper_Should_Return_Flow_Error()
    {
        var result = Result.Ok().Map<int>(mapper: null);

        result.IsFailure.ShouldBeTrue();
        result.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
    }

    [Fact]
    public void Bind_With_Null_Binder_Or_Null_Result_Should_Return_Flow_Errors()
    {
        var missingBinder = Result<int>.Ok(1).Bind((Func<int, Result>?)null);
        var missingResult = Result<int>.Ok(1).Bind((Func<int, Result>)(_ => null!));
        var missingGenericResult = Result<int>.Ok(1).Bind((Func<int, Result<string>>)(_ => null!));

        missingBinder.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        missingGenericResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public void Ensure_With_Null_Factory_Func_Should_Return_Flow_Error()
    {
        var result = Result<int>.Ok(0).Ensure(_ => false, error: (Func<IError>?)null);

        result.IsFailure.ShouldBeTrue();
        result.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
    }

    [Fact]
    public void NonGeneric_Recover_Should_Pass_Through_Success_And_Handle_Missing_Handlers()
    {
        var success = Result.Ok();
        var recovered = success.Recover(_ => Result.Fail(ResultStub.ConflictError()));
        var missingHandler = Result.Fail(ResultStub.NotFoundError())
            .Recover((Func<IReadOnlyList<IError>, Result>?)null);
        var missingResult = Result.Fail(ResultStub.NotFoundError())
            .Recover(_ => null!);

        recovered.ShouldBeSameAs(success);
        missingHandler.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public void Recover_Value_Should_Pass_Through_Success()
    {
        var success = Result<int>.Ok(5);

        var recovered = success.Recover(errors => 99);

        recovered.ShouldBeSameAs(success);
        recovered.Value.ShouldBe(5);
    }

    [Fact]
    public void Recover_Result_Func_Should_Pass_Through_Success_And_Handle_Missing_Handlers()
    {
        var success = Result<int>.Ok(5);
        var recovered = success.Recover(_ => Result<int>.Ok(9));
        var missingHandler = Result<int>.Fail(ResultStub.NotFoundError())
            .Recover((Func<IReadOnlyList<IError>, Result<int>>?)null);
        var missingResult = Result<int>.Fail(ResultStub.NotFoundError())
            .Recover(_ => null!);

        recovered.ShouldBeSameAs(success);
        missingHandler.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task RecoverAsync_NonGeneric_Should_Handle_Success_Missing_Handler_Null_Task_And_Null_Result()
    {
        var success = Result.Ok();
        var recovered = await success.RecoverAsync((_, _) => Task.FromResult(Result.Fail(ResultStub.ConflictError())));
        var missingHandler = await Result.Fail(ResultStub.NotFoundError())
            .RecoverAsync((Func<IReadOnlyList<IError>, CancellationToken, Task<Result>>?)null);
        var missingTask = await Result.Fail(ResultStub.NotFoundError())
            .RecoverAsync((_, _) => null!);
        var missingResult = await Result.Fail(ResultStub.NotFoundError())
            .RecoverAsync((_, _) => Task.FromResult((Result)null!));

        recovered.ShouldBeSameAs(success);
        missingHandler.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        missingResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task RecoverAsync_Value_Should_Pass_Through_Success_And_Handle_Null_Task()
    {
        var success = Result<int>.Ok(5);
        var recovered = await success.RecoverAsync((_, _) => Task.FromResult(9));
        var missingTask = await Result<int>.Fail(ResultStub.NotFoundError())
            .RecoverAsync((Func<IReadOnlyList<IError>, CancellationToken, Task<int>>)((_, _) => null!));

        recovered.ShouldBeSameAs(success);
        missingTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task RecoverAsync_Result_Func_Should_Handle_All_Paths()
    {
        var success = Result<int>.Ok(5);
        var recovered = await success.RecoverAsync((_, _) => Task.FromResult(Result<int>.Ok(9)));
        var missingHandler = await Result<int>.Fail(ResultStub.NotFoundError())
            .RecoverAsync((Func<IReadOnlyList<IError>, CancellationToken, Task<Result<int>>>?)null);
        var missingTask = await Result<int>.Fail(ResultStub.NotFoundError())
            .RecoverAsync((Func<IReadOnlyList<IError>, CancellationToken, Task<Result<int>>>)((_, _) => null!));
        var missingResult = await Result<int>.Fail(ResultStub.NotFoundError())
            .RecoverAsync((_, _) => Task.FromResult((Result<int>)null!));
        var recoveredFromFailure = await Result<int>.Fail(ResultStub.NotFoundError())
            .RecoverAsync((_, _) => Task.FromResult(Result<int>.Ok(9)));

        recovered.ShouldBeSameAs(success);
        missingHandler.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        missingResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        recoveredFromFailure.IsSuccess.ShouldBeTrue();
        recoveredFromFailure.Value.ShouldBe(9);
    }

    [Fact]
    public void NonGeneric_Switch_Should_Run_Handlers_For_Both_States()
    {
        var successRan = false;
        var failureRan = false;

        Result.Ok().Switch(() => successRan = true, _ => failureRan = true);
        Result.Ok().Switch(null, null);
        Result.Fail(ResultStub.NotFoundError()).Switch(() => successRan = false, _ => failureRan = true);
        Result.Fail(ResultStub.NotFoundError()).Switch(null, null);

        successRan.ShouldBeTrue();
        failureRan.ShouldBeTrue();
    }

    [Fact]
    public void Generic_Switch_With_Success_And_OnSuccess_Should_Invoke_Handler()
    {
        var got = 0;

        Result<int>.Ok(2).Switch<int>(value => got = value, null);

        got.ShouldBe(2);
    }

    [Fact]
    public void NonGeneric_Tap_Should_Run_Handler_Only_On_Success()
    {
        var count = 0;

        Result.Ok().Tap(() => count++);
        Result.Ok().Tap(null);
        Result.Fail(ResultStub.NotFoundError()).Tap(() => count++);

        count.ShouldBe(1);
    }

    [Fact]
    public void Generic_Tap_On_Failure_Should_Not_Invoke_Handler()
    {
        var count = 0;

        Result<int>.Fail(ResultStub.NotFoundError()).Tap<int>(_ => count++);
        Result<int>.Ok(1).Tap<int>(_ => count++);

        count.ShouldBe(1);
    }

    [Fact]
    public void NonGeneric_TapError_Should_Run_Handler_Only_On_Failure()
    {
        var count = 0;

        Result.Fail(ResultStub.NotFoundError()).TapError(_ => count++);
        Result.Fail(ResultStub.NotFoundError()).TapError(null);
        Result.Ok().TapError(_ => count++);

        count.ShouldBe(1);
    }

    [Fact]
    public void Generic_TapError_With_Null_Handler_Should_No_Op()
    {
        var count = 0;

        Result<int>.Fail(ResultStub.NotFoundError()).TapError<int>(null);
        Result<int>.Ok(1).TapError<int>(null);
        Result<int>.Fail(ResultStub.NotFoundError()).TapError<int>(_ => count++);

        count.ShouldBe(1);
    }

    [Fact]
    public void Value_Access_With_Null_Result_Should_Throw_Param()
    {
        Action valueOrThrow = () => _ = ((Result<int>)null!).ValueOrThrow();
        Action valueOr = () => ((Result<int>)null!).ValueOr(-1);

        Should.Throw<ArgumentNullException>(valueOrThrow).ParamName.ShouldBe("result");
        Should.Throw<ArgumentNullException>(valueOr).ParamName.ShouldBe("result");
    }

    [Fact]
    public async Task NonGeneric_MatchAsync_Should_Handle_All_Callback_Combinations()
    {
        var successWithValue = await Result.Ok().MatchAsync<string>(
            (_) => Task.FromResult("s"),
            null);
        var successWithNullCallbacks = await Result.Ok().MatchAsync<string>(null, null);
        var successWithNullTask = await Result.Ok().MatchAsync<string>((_) => null!, null);
        var failureWithValue = await Result.Fail(ResultStub.NotFoundError()).MatchAsync<string>(
            (_) => Task.FromResult("s"),
            (_, _) => Task.FromResult("f"));
        var failureWithNullCallbacks = await Result.Fail(ResultStub.NotFoundError())
            .MatchAsync<string>(null, null);

        successWithValue.ShouldBe("s");
        successWithNullCallbacks.ShouldBeNull();
        successWithNullTask.ShouldBeNull();
        failureWithValue.ShouldBe("f");
        failureWithNullCallbacks.ShouldBeNull();
    }

    [Fact]
    public async Task Generic_MatchAsync_Should_Handle_Missing_Callbacks_And_Failure()
    {
        var successWithNullCallback = await Result<int>.Ok(1).MatchAsync<int, string>(null, null);
        var failureWithValue = await Result<int>.Fail(ResultStub.NotFoundError())
            .MatchAsync<int, string>(
                (value, _) => Task.FromResult(value.ToString()),
                (_, _) => Task.FromResult("f"));
        var failureWithNullCallbacks = await Result<int>.Fail(ResultStub.NotFoundError())
            .MatchAsync<int, string>(null, null);

        successWithNullCallback.ShouldBeNull();
        failureWithValue.ShouldBe("f");
        failureWithNullCallbacks.ShouldBeNull();
    }

    [Fact]
    public async Task NonGeneric_MapAsync_Should_Handle_Failure_And_Null_Task()
    {
        var failure = await Result.Fail(ResultStub.NotFoundError()).MapAsync((_) => Task.FromResult(1));
        var nullTask = await Result.Ok().MapAsync<int>((_) => null!);

        failure.IsFailure.ShouldBeTrue();
        failure.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        nullTask.IsFailure.ShouldBeTrue();
        nullTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task Generic_MapAsync_Should_Handle_Failure()
    {
        var failure = await Result<int>.Fail(ResultStub.NotFoundError())
            .MapAsync<int, int>((value, _) => Task.FromResult(value));

        failure.IsFailure.ShouldBeTrue();
        failure.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        failure.TryGetValue(out _).ShouldBeFalse();
    }

    [Fact]
    public async Task NonGeneric_BindAsync_Should_Handle_Failure_Missing_Binder_Null_Task_And_Null_Result()
    {
        var binderRan = false;
        var failure = await Result<int>.Fail(ResultStub.NotFoundError())
            .BindAsync((_, _) =>
            {
                binderRan = true;
                return Task.FromResult(Result.Ok());
            });
        var missingBinder = await Result<int>.Ok(1)
            .BindAsync((Func<int, CancellationToken, Task<Result>>?)null);
        var missingTask = await Result<int>.Ok(1)
            .BindAsync((Func<int, CancellationToken, Task<Result>>)((_, _) => null!));
        var missingResult = await Result<int>.Ok(1)
            .BindAsync((_, _) => Task.FromResult((Result)null!));

        binderRan.ShouldBeFalse();
        failure.IsFailure.ShouldBeTrue();
        missingBinder.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        missingResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task Generic_BindAsync_Should_Handle_Failure_Missing_Binder_Null_Task_And_Null_Result()
    {
        var binderRan = false;
        var failure = await Result<int>.Fail(ResultStub.NotFoundError())
            .BindAsync((_, _) =>
            {
                binderRan = true;
                return Task.FromResult(Result<string>.Ok("x"));
            });
        var missingBinder = await Result<int>.Ok(1)
            .BindAsync((Func<int, CancellationToken, Task<Result<string>>>?)null);
        var missingTask = await Result<int>.Ok(1)
            .BindAsync((Func<int, CancellationToken, Task<Result<string>>>)((_, _) => null!));
        var missingResult = await Result<int>.Ok(1)
            .BindAsync((_, _) => Task.FromResult((Result<string>)null!));

        binderRan.ShouldBeFalse();
        failure.IsFailure.ShouldBeTrue();
        missingBinder.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        missingTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        missingResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task EnsureAsync_Should_Handle_Failure_Predicate_Null_Task_And_Missing_ErrorValue()
    {
        var predicateRan = false;
        var failure = await Result<int>.Fail(ResultStub.NotFoundError())
            .EnsureAsync(
                (_, _) =>
                {
                    predicateRan = true;
                    return Task.FromResult(true);
                },
                ResultStub.NotFoundError());
        var nullPredicateTask = await Result<int>.Ok(1)
            .EnsureAsync((_, _) => null!, ResultStub.NotFoundError());
        var missingErrorValue = await Result<int>.Ok(1)
            .EnsureAsync((_, _) => Task.FromResult(false), errorValue: null);

        predicateRan.ShouldBeFalse();
        failure.IsFailure.ShouldBeTrue();
        nullPredicateTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        missingErrorValue.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
    }

    [Fact]
    public async Task EnsureAsync_With_ErrorValue_Should_Apply_Error_On_Violation()
    {
        var violation = await Result<int>.Ok(1)
            .EnsureAsync((_, _) => Task.FromResult(false), ResultStub.NotFoundError());

        violation.IsFailure.ShouldBeTrue();
        violation.Errors[0].Code.ShouldBe("test.not_found");
    }

    [Fact]
    public async Task EnsureAsync_With_Factory_Func_Overloads_Should_Handle_Missing_Values()
    {
        var missingFunc = await Result<int>.Ok(1)
            .EnsureAsync((_, _) => Task.FromResult(false), error: (Func<IError>?)null);
        var nullErrorResult = await Result<int>.Ok(1)
            .EnsureAsync((_, _) => Task.FromResult(false), error: (Func<IError>)(() => null!));
        var missingSyncViolation = await Result<int>.Ok(1)
            .EnsureAsync((_, _) => Task.FromResult(false), onViolation: (Func<int, IError>?)null);
        var nullAsyncTask = await Result<int>.Ok(1)
            .EnsureAsync((_, _) => Task.FromResult(false), onViolation: (_, _) => null!);
        var nullAsyncResult = await Result<int>.Ok(1)
            .EnsureAsync((_, _) => Task.FromResult(false), onViolation: (_, _) => Task.FromResult<IError>(null!));

        missingFunc.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        nullErrorResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ErrorMissing.Code);
        missingSyncViolation.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
        nullAsyncTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        nullAsyncResult.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ErrorMissing.Code);
    }

    [Fact]
    public async Task NonGeneric_SwitchAsync_Should_Handle_All_Handler_Combinations()
    {
        var successRan = false;
        var failureRan = false;

        await Result.Ok().SwitchAsync(
            (_) =>
            {
                successRan = true;
                return Task.CompletedTask;
            },
            null);
        await Result.Ok().SwitchAsync(null, null);
        var successNullTask = await Result.Ok().SwitchAsync((_) => null!, null);
        await Result.Fail(ResultStub.NotFoundError()).SwitchAsync(
            null,
            (_, _) =>
            {
                failureRan = true;
                return Task.CompletedTask;
            });
        await Result.Fail(ResultStub.NotFoundError()).SwitchAsync(null, null);
        var failureNullTask = await Result.Fail(ResultStub.NotFoundError())
            .SwitchAsync(null, (_, _) => null!);

        successRan.ShouldBeTrue();
        failureRan.ShouldBeTrue();
        successNullTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
        failureNullTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task Generic_SwitchAsync_Should_Handle_Missing_Handlers_And_Null_Task()
    {
        await Result<int>.Ok(1).SwitchAsync<int>(null, null);
        await Result<int>.Fail(ResultStub.NotFoundError()).SwitchAsync<int>(null, null);
        var nullTask = await Result<int>.Fail(ResultStub.NotFoundError())
            .SwitchAsync<int>(null, (_, _) => null!);

        nullTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task NonGeneric_TapAsync_Should_Handle_Success_Failure_And_Null_Task()
    {
        var count = 0;

        await Result.Ok().TapAsync(
            (_) =>
            {
                count++;
                return Task.CompletedTask;
            });
        await Result.Ok().TapAsync(null);
        var nullTask = await Result.Ok().TapAsync((_) => null!);
        await Result.Fail(ResultStub.NotFoundError()).TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        count.ShouldBe(1);
        nullTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task Generic_TapAsync_With_Missing_Handler_Should_No_Op()
    {
        var count = 0;

        await Result<int>.Ok(1).TapAsync<int>(null);
        await Result<int>.Fail(ResultStub.NotFoundError()).TapAsync<int>((_, _) =>
        {
            count++;
            return Task.CompletedTask;
        });

        count.ShouldBe(0);
    }

    [Fact]
    public void Generic_Match_With_Success_And_Null_OnSuccess_Should_Return_Default()
    {
        var result = Result<int>.Ok(1).Match<int, int>(null, _ => 9);

        result.ShouldBe(0);
    }

    [Fact]
    public async Task Generic_MapAsync_With_Null_Mapper_Should_Return_Flow_Error()
    {
        var result = await Result<int>.Ok(1).MapAsync<int, int>(mapper: null);

        result.IsFailure.ShouldBeTrue();
        result.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.CallbackMissing.Code);
    }

    [Fact]
    public async Task Generic_TapErrorAsync_With_Null_Task_Should_Return_Flow_Error()
    {
        var result = await Result<int>.Fail(ResultStub.NotFoundError())
            .TapErrorAsync<int>((_, _) => null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task NonGeneric_TapErrorAsync_Should_Handle_Failure_Success_And_Null_Task()
    {
        var count = 0;

        await Result.Fail(ResultStub.NotFoundError()).TapErrorAsync((_, _) =>
        {
            count++;
            return Task.CompletedTask;
        });
        await Result.Fail(ResultStub.NotFoundError()).TapErrorAsync(null);
        var nullTask = await Result.Fail(ResultStub.NotFoundError()).TapErrorAsync((_, _) => null!);
        await Result.Ok().TapErrorAsync((_, _) =>
        {
            count++;
            return Task.CompletedTask;
        });

        count.ShouldBe(1);
        nullTask.Errors[0].Code.ShouldBe(ResultConstant.Failure.Flow.ResultMissing.Code);
    }

    [Fact]
    public async Task Generic_TapErrorAsync_With_Null_Handler_Should_No_Op()
    {
        var count = 0;

        await Result<int>.Fail(ResultStub.NotFoundError()).TapErrorAsync<int>(null);
        await Result<int>.Ok(1).TapErrorAsync<int>((_, _) =>
        {
            count++;
            return Task.CompletedTask;
        });

        count.ShouldBe(0);
    }
}
