namespace SharedKernel.UnitTests.Structures.Results;

public class ResultFlowControlValueSpec
{
    #region Map

    [Fact]
    public void Map_WhenSuccessful_TransformsValue()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Map(x => x * 2);

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(10);
    }

    [Fact]
    public void Map_WhenFailed_PropagatesErrors()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<int, Error>.Failure(error);

        var actual = result.Map(x => x * 2);

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(error);
    }

    [Fact]
    public void Map_PreservesMetadata()
    {
        var metadata = new Dictionary<string, object?>
        {
            ["key"] = "value"
        };

        var result = Result<int, Error>
            .Ok(5)
            .WithAdditionalMetadata(metadata);

        var actual = result.Map(x => x * 2);

        actual.Metadata!.ShouldContainKey("key");
    }

    [Fact]
    public void Map_WhenMapperIsNull_ReturnsFailure()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Map<int>(null!);

        actual.IsFailure.ShouldBeTrue();
    }

    #endregion

    #region Bind

    [Fact]
    public void Bind_WhenSuccessful_ReturnsBoundResult()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Bind(
            x => Result<string, Error>.Ok(x.ToString()));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("5");
    }

    [Fact]
    public void Bind_WhenBinderReturnsFailure_PropagatesBinderFailure()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<int, Error>.Ok(5);

        var actual = result.Bind<int>(
            _ => Result<int, Error>.Failure(error));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(error);
    }

    [Fact]
    public void Bind_WhenOriginalResultFails_PropagatesErrors()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<int, Error>.Failure(error);

        var actual = result.Bind(
            x => Result<string, Error>.Ok(x.ToString()));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(error);
    }

    [Fact]
    public void Bind_WhenFailed_PreservesMetadata()
    {
        var metadata = new Dictionary<string, object?>
        {
            ["key"] = "value"
        };

        var result = Result<int, Error>
            .Failure(Error.BadRequest("Code", "Desc"))
            .WithAdditionalMetadata(metadata);

        var actual = result.Bind(
            x => Result<string, Error>.Ok(x.ToString()));

        actual.Metadata!.ShouldContainKey("key");
    }

    [Fact]
    public void Bind_WhenBinderIsNull_ReturnsFailure()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Bind<int>(null!);

        actual.IsFailure.ShouldBeTrue();
    }

    #endregion

    #region Match

    [Theory]
    [InlineData(5, "5")]
    [InlineData(10, "10")]
    public void Match_WhenSuccessful_InvokesSuccessHandler(
        int input,
        string expected)
    {
        var result = Result<int, Error>.Ok(input);

        var actual = result.Match(
            onSuccess: x => x.ToString(),
            onFailure: _ => "failure");

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Match_WhenFailed_InvokesFailureHandler()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.Match(
            onSuccess: x => x,
            onFailure: errors => errors.Count);

        actual.ShouldBe(1);
    }

    [Fact]
    public void Match_WhenSuccessHandlerIsNull_ReturnsDefault()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Match<string>(
            onSuccess: null!,
            onFailure: _ => "failure");

        actual.ShouldBeNull();
    }

    [Fact]
    public void Match_WhenFailureHandlerIsNull_ReturnsDefault()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Match<string>(
            onSuccess: x => x.ToString(),
            onFailure: null!);

        actual.ShouldBeNull();
    }

    #endregion

    #region Tap

    [Fact]
    public void Tap_WhenSuccessful_ExecutesActionWithValue()
    {
        var result = Result<int, Error>.Ok(5);
        var captured = 0;

        var actual = result.Tap(x => captured = x);

        captured.ShouldBe(5);
        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void Tap_WhenFailed_DoesNotExecuteAction()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var executed = false;

        var actual = result.Tap(_ => executed = true);

        executed.ShouldBeFalse();
        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void Tap_WhenActionIsNull_ReturnsSameResult()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Tap((Action<int>?)null);

        actual.ShouldBeSameAs(result);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void Tap_WhenChained_ExecutesEveryAction(int chainCount)
    {
        var result = Result<int, Error>.Ok(5);
        var executionCount = 0;

        var actual = result;

        for (var i = 0; i < chainCount; i++)
        {
            actual = actual.Tap(_ => executionCount++);
        }

        executionCount.ShouldBe(chainCount);
        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region TapErrors

    [Fact]
    public void TapErrors_WhenSuccessful_DoesNotExecuteAction()
    {
        var result = Result<int, Error>.Ok(5);
        var executed = false;

        var actual = result.TapErrors(_ => executed = true);

        executed.ShouldBeFalse();
        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void TapErrors_WhenFailed_PassesErrorsToAction()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<int, Error>.Failure(error);
        List<Error>? captured = null;

        var actual = result.TapErrors(errors => captured = errors);

        captured.ShouldHaveSingleItem().ShouldBe(error);

        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void TapErrors_WhenMultipleErrors_PassesAllErrorsInOrder()
    {
        var first = Error.BadRequest("Code1", "Desc1");
        var second = Error.BadRequest("Code2", "Desc2");
        var result = Result<int, Error>.Failure(first, second);

        List<Error>? captured = null;

        result.TapErrors(errors => captured = errors);

        captured.ShouldBe([first, second]);
    }

    [Fact]
    public void TapErrors_WhenActionIsNull_ReturnsSameResult()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.TapErrors(null!);

        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region Recover

    [Fact]
    public void Recover_WhenFailed_ReturnsRecoveryValue()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.Recover(_ => 42);

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(42);
    }

    [Fact]
    public void Recover_WhenFailed_ReturnsRecoveryResult()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.Recover(
            _ => Result<int, Error>.Ok(7));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(7);
    }

    [Fact]
    public void Recover_WhenSuccessful_DoesNotInvokeRecovery()
    {
        var result = Result<int, Error>.Ok(5);
        var invoked = false;

        var actual = result.Recover(_ =>
        {
            invoked = true;
            return 99;
        });

        invoked.ShouldBeFalse();
        actual.Value.ShouldBe(5);
    }

    [Fact]
    public void Recover_WhenFailed_PassesErrorsToRecovery()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<int, Error>.Failure(error);
        IReadOnlyList<Error>? captured = null;

        result.Recover(errors =>
        {
            captured = errors;
            return 0;
        });

        captured.ShouldHaveSingleItem().ShouldBe(error);
    }

    [Fact]
    public void Recover_WhenRecoveryIsNull_ReturnsSameResult()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.Recover(
            (Func<IReadOnlyList<Error>, int>)null!);

        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void Recover_WhenRecoveryFails_PropagatesRecoveryFailure()
    {
        var recoveryError = Error.NotFound("Rec.Code", "Rec");
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.Recover(
            _ => Result<int, Error>.Failure(recoveryError));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(recoveryError);
    }

    #endregion

    #region GetValueOrDefault

    [Fact]
    public void GetValueOrDefault_WhenSuccessful_ReturnsValue()
    {
        var result = Result<int, Error>.Ok(5);

        result.GetValueOrDefault().ShouldBe(5);
    }

    [Fact]
    public void GetValueOrDefault_WhenFailed_ReturnsDefault()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        result.GetValueOrDefault().ShouldBe(0);
    }

    [Fact]
    public void GetValueOrDefault_WhenFailedWithCustomDefault_ReturnsCustomDefault()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        result.GetValueOrDefault(42).ShouldBe(42);
    }

    #endregion

    #region GetValueOrThrow

    [Fact]
    public void GetValueOrThrow_WhenSuccessful_ReturnsValue()
    {
        var result = Result<int, Error>.Ok(5);

        result.GetValueOrThrow().ShouldBe(5);
    }

    [Fact]
    public void GetValueOrThrow_WhenFailed_ThrowsInvalidOperationException()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Test.Code", "Description"));

        Action act = () => result.GetValueOrThrow();

        Should.Throw<InvalidOperationException>(act)
            .Message.ShouldBe("Errors(1): [Test.Code: Description]");
    }

    #endregion

    #region ThrowIfFailure

    [Fact]
    public void ThrowIfFailure_WhenSuccessful_ReturnsSameResult()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.ThrowIfFailure();

        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void ThrowIfFailure_WhenFailed_ThrowsInvalidOperationException()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Test.Code", "Description"));

        Action act = () => result.ThrowIfFailure();

        Should.Throw<InvalidOperationException>(act)
            .Message.ShouldBe("Errors(1): [Test.Code: Description]");
    }

    #endregion

    #region MapError

    [Fact]
    public void MapError_WhenSuccessful_PreservesValue()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.MapError(
            error => Error.NotFound(error.Code, error.Message));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(5);
    }

    [Fact]
    public void MapError_WhenFailed_TransformsErrors()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.MapError(
            error => Error.NotFound(error.Code, error.Message));

        actual.IsFailure.ShouldBeTrue();

        var mappedError = actual.Errors.ShouldHaveSingleItem();

        mappedError.Code.ShouldBe("Code");
        mappedError.Status.ShouldBe(404);
    }

    [Fact]
    public void MapError_WhenFailed_PreservesMetadata()
    {
        var metadata = new Dictionary<string, object?>
        {
            ["key"] = "value"
        };

        var result = Result<int, Error>
            .Failure(Error.BadRequest("Code", "Desc"))
            .WithAdditionalMetadata(metadata);

        var actual = result.MapError(
            error => Error.NotFound(error.Code, error.Message));

        actual.Metadata!.ShouldContainKey("key");
    }

    [Fact]
    public void MapError_WhenMapperIsNull_ReturnsFailure()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.MapError<Error>(null!);

        actual.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void MapError_WhenMapperIsNullAndResultFailed_PreservesFailure()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.MapError<Error>(null!);

        actual.IsFailure.ShouldBeTrue();
        actual.Status.ShouldBe(400);
    }

    #endregion

    #region FailIf

    [Fact]
    public void FailIf_WhenPredicateIsTrue_ReturnsFailure()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<int, Error>.Ok(5);

        var actual = result.FailIf(x => x > 3, error);

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(error);
    }

    [Fact]
    public void FailIf_WhenPredicateIsFalse_PreservesSuccess()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.FailIf(
            x => x > 10,
            Error.BadRequest("Code", "Desc"));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(5);
    }

    [Fact]
    public void FailIf_WhenAlreadyFailed_PreservesOriginalFailure()
    {
        var originalError = Error.BadRequest("Orig", "Orig");
        var result = Result<int, Error>.Failure(originalError);

        var actual = result.FailIf(
            _ => true,
            Error.BadRequest("New", "New"));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(originalError);
    }

    [Fact]
    public void FailIf_WhenPredicateIsNull_ReturnsSameResult()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.FailIf(
            (Func<int, bool>?)null,
            Error.BadRequest("Code", "Desc"));

        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void FailIf_WhenFactoryPredicateIsTrue_ReturnsFactoryError()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.FailIf(
            x => x > 3,
            x => Error.BadRequest(
                "TooHigh",
                $"Value {x} is too high"));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().Code.ShouldBe("TooHigh");
    }

    [Fact]
    public void FailIf_WhenFactoryIsNull_ReturnsSameResult()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.FailIf(
            _ => true,
            (Func<int, Error>)null!);

        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region Ensure

    [Fact]
    public void Ensure_WhenPredicateIsTrue_PreservesSuccess()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Ensure(
            x => x > 0,
            Error.BadRequest("Invalid", "Must be positive"));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(5);
    }

    [Fact]
    public void Ensure_WhenPredicateIsFalse_ReturnsFailure()
    {
        var error = Error.BadRequest("Invalid", "Must be positive");
        var result = Result<int, Error>.Ok(-1);

        var actual = result.Ensure(x => x > 0, error);

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(error);
    }

    [Fact]
    public void Ensure_WhenAlreadyFailed_PreservesOriginalFailure()
    {
        var originalError = Error.BadRequest("Orig", "Orig");
        var result = Result<int, Error>.Failure(originalError);

        var actual = result.Ensure(
            _ => true,
            Error.BadRequest("New", "New"));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(originalError);
    }

    [Fact]
    public void Ensure_WhenValidatorSucceeds_PreservesValue()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Ensure(x =>
            x > 0
                ? Result<int, Error>.Ok(x)
                : Result<int, Error>.Failure(
                    Error.BadRequest("Invalid", "Must be positive")));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(5);
    }

    [Fact]
    public void Ensure_WhenValidatorFails_ReturnsValidatorFailure()
    {
        var result = Result<int, Error>.Ok(-1);

        var actual = result.Ensure(x =>
            x > 0
                ? Result<int, Error>.Ok(x)
                : Result<int, Error>.Failure(
                    Error.BadRequest("Invalid", "Must be positive")));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().Code.ShouldBe("Invalid");
    }

    [Fact]
    public void Ensure_WhenAlreadyFailed_DoesNotInvokeValidator()
    {
        var originalError = Error.BadRequest("Orig", "Orig");
        var result = Result<int, Error>.Failure(originalError);
        var invoked = false;

        var actual = result.Ensure(x =>
        {
            invoked = true;

            return Result<int, Error>.Ok(x);
        });

        invoked.ShouldBeFalse();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(originalError);
    }

    [Fact]
    public void Ensure_WhenPredicateIsNull_ReturnsSameResult()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Ensure(
            (Func<int, bool>?)null,
            Error.BadRequest("Code", "Desc"));

        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void Ensure_WhenValidatorIsNull_ReturnsSameResult()
    {
        var result = Result<int, Error>.Ok(5);

        var actual = result.Ensure((Func<int, Result<int, Error>>?)null);

        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region Switch

    [Fact]
    public void Switch_WhenSuccessful_ExecutesOnlySuccessHandler()
    {
        var result = Result<int, Error>.Ok(5);
        var captured = 0;
        var failureExecuted = false;

        result.Switch(
            value => captured = value,
            _ => failureExecuted = true);

        captured.ShouldBe(5);
        failureExecuted.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenFailed_ExecutesOnlyFailureHandler()
    {
        var result = Result<int, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var successExecuted = false;
        var capturedErrorCount = 0;

        result.Switch(
            _ => successExecuted = true,
            errors => capturedErrorCount = errors.Count);

        successExecuted.ShouldBeFalse();
        capturedErrorCount.ShouldBe(1);
    }

    [Fact]
    public void Switch_WhenSuccessHandlerIsNull_DoesNotThrow()
    {
        var result = Result<int, Error>.Ok(5);

        Action act = () => result.Switch(null!, _ => { });

        act.ShouldNotThrow();
    }

    [Fact]
    public void Switch_WhenFailureHandlerIsNull_DoesNotThrow()
    {
        var result = Result<int, Error>.Ok(5);

        Action act = () => result.Switch(_ => { }, null!);

        act.ShouldNotThrow();
    }

    #endregion
}
