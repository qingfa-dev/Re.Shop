namespace SharedKernel.UnitTests.Structures.Results;

public class ResultFlowControlUnitSpec
{
    #region Match

    [Theory]
    [InlineData("success")]
    [InlineData("other")]
    public void Match_WhenSuccessful_InvokesSuccessHandler(string expected)
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Match(
            onSuccess: () => expected,
            onFailure: _ => "failure");

        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Match_WhenFailed_InvokesFailureHandler(int errorCount)
    {
        var errors = Enumerable.Range(0, errorCount)
            .Select(i => Error.BadRequest($"Code{i}", $"Desc{i}"))
            .ToArray();

        var result = Result<Unit, Error>.Failure(errors);

        var actual = result.Match(
            onSuccess: () => 0,
            onFailure: failures => failures.Count);

        actual.ShouldBe(errorCount);
    }

    [Fact]
    public void Match_WhenSuccessHandlerIsNull_ReturnsDefault()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Match<string>(
            onSuccess: null!,
            onFailure: _ => "failure");

        actual.ShouldBeNull();
    }

    [Fact]
    public void Match_WhenFailureHandlerIsNull_ReturnsDefault()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Match<string>(
            onSuccess: () => "success",
            onFailure: null!);

        actual.ShouldBeNull();
    }

    #endregion

    #region Tap

    [Fact]
    public void Tap_WhenSuccessful_ExecutesActionAndReturnsSameResult()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        var executed = false;

        var actual = result.Tap(() => executed = true);

        executed.ShouldBeTrue();
        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void Tap_WhenFailed_DoesNotExecuteAction()
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var executed = false;

        var actual = result.Tap(() => executed = true);

        executed.ShouldBeFalse();
        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void Tap_WhenActionIsNull_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Tap((Action?)null);

        actual.ShouldBeSameAs(result);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    public void Tap_WhenChained_ExecutesEachAction(int chainCount)
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        var executionCount = 0;

        var actual = result;

        for (var i = 0; i < chainCount; i++)
        {
            actual = actual.Tap(() => executionCount++);
        }

        executionCount.ShouldBe(chainCount);
        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region TapErrors

    [Fact]
    public void TapErrors_WhenSuccessful_DoesNotExecuteAction()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        var executed = false;

        var actual = result.TapErrors(_ => executed = true);

        executed.ShouldBeFalse();
        actual.ShouldBeSameAs(result);
    }

    [Theory]
    [InlineData("Code1", "Desc1")]
    [InlineData("Test.Code", "Description")]
    public void TapErrors_WhenFailed_PassesAllErrorsToAction(
        string code,
        string description)
    {
        var error = Error.BadRequest(code, description);
        var result = Result<Unit, Error>.Failure(error);
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
        var result = Result<Unit, Error>.Failure(first, second);

        List<Error>? captured = null;

        result.TapErrors(errors => captured = errors);

        captured.ShouldBe([first, second]);
    }

    [Fact]
    public void TapErrors_WhenActionIsNull_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.TapErrors(null!);

        actual.ShouldBeSameAs(result);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    public void TapErrors_WhenChained_ExecutesEachAction(int chainCount)
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var executionCount = 0;
        var actual = result;

        for (var i = 0; i < chainCount; i++)
        {
            actual = actual.TapErrors(_ => executionCount++);
        }

        executionCount.ShouldBe(chainCount);
        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region ThrowIfFailure

    [Fact]
    public void ThrowIfFailure_WhenSuccessful_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.ThrowIfFailure();

        actual.ShouldBeSameAs(result);
    }

    [Theory]
    [InlineData("Code1", "Desc1")]
    [InlineData("Test.Code", "Description")]
    public void ThrowIfFailure_WhenFailed_ThrowsInvalidOperationException(
        string code,
        string description)
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest(code, description));

        Action act = () => result.ThrowIfFailure();

        Should.Throw<InvalidOperationException>(act)
            .Message.ShouldBe($"Errors(1): [{code}: {description}]");
    }

    [Fact]
    public void ThrowIfFailure_WhenUnexpectedError_ThrowsWithErrorDetails()
    {
        var result = Result<Unit, Error>.Failure(
            Error.Unexpected("Code", "Desc"));

        Action act = () => result.ThrowIfFailure();

        Should.Throw<InvalidOperationException>(act)
            .Message.ShouldBe("Errors(1): [Code: Desc]");
    }

    #endregion

    #region MapError

    [Fact]
    public void MapError_WhenSuccessful_ReturnsSuccessfulResult()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.MapError(
            error => Error.NotFound(error.Code, error.Message));

        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MapError_WhenFailed_TransformsErrors()
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.MapError(
            error => Error.NotFound(error.Code, error.Message));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem();

        var mappedError = actual.Errors.Single();
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

        var result = Result<Unit, Error>
            .Failure(Error.BadRequest("Code", "Desc"))
            .WithAdditionalMetadata(metadata);

        var actual = result.MapError(
            error => Error.NotFound(error.Code, error.Message));

        actual.Metadata!.ShouldContainKey("key");
    }

    [Fact]
    public void MapError_WhenMapperIsNull_OnSuccess_ReturnsSuccess()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.MapError<Error>(null!);

        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MapError_WhenMapperIsNull_OnFailure_PreservesFailure()
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.MapError<Error>(null!);

        actual.IsFailure.ShouldBeTrue();
        actual.Status.ShouldBe(400);
    }

    #endregion

    #region Bind

    [Theory]
    [InlineData(201)]
    [InlineData(202)]
    public void Bind_WhenSuccessful_InvokesNextOperation(int expectedStatus)
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        var invoked = false;

        var actual = result.Bind(() =>
        {
            invoked = true;

            return expectedStatus switch
            {
                201 => Result<Unit, Error>.Created(Unit.Value),
                202 => Result<Unit, Error>.Accepted(Unit.Value),
                _ => throw new ArgumentOutOfRangeException()
            };
        });

        invoked.ShouldBeTrue();
        actual.IsSuccess.ShouldBeTrue();
        actual.Status.ShouldBe(expectedStatus);
    }

    [Fact]
    public void Bind_WhenFailed_DoesNotInvokeNextOperation()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<Unit, Error>.Failure(error);
        var invoked = false;

        var actual = result.Bind(() =>
        {
            invoked = true;
            return Result<Unit, Error>.Ok(Unit.Value);
        });

        invoked.ShouldBeFalse();
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

        var result = Result<Unit, Error>
            .Failure(Error.BadRequest("Code", "Desc"))
            .WithAdditionalMetadata(metadata);

        var actual = result.Bind(Result<Unit, Error>.Ok);

        actual.Metadata!.ShouldContainKey("key");
    }

    [Fact]
    public void Bind_WhenNextOperationIsNull_ReturnsFailure()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Bind((Func<Result<Unit, Error>>?)null);

        actual.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Bind_WhenChained_ExecutesSequentially()
    {
        var actual = Result<Unit, Error>.Ok(Unit.Value)
            .Bind(Result<Unit, Error>.Ok)
            .Bind(Result<Unit, Error>.Accepted);

        actual.IsSuccess.ShouldBeTrue();
        actual.Status.ShouldBe(202);
    }

    #endregion

    #region Recover

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Recover_WhenFailed_ReturnsRecoveryResult(bool shouldRecover)
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.Recover(_ =>
            shouldRecover
                ? Result<Unit, Error>.Ok(Unit.Value)
                : Result<Unit, Error>.Failure(
                    Error.NotFound("Rec.Code", "Rec")));

        actual.IsSuccess.ShouldBe(shouldRecover);
    }

    [Fact]
    public void Recover_WhenSuccessful_DoesNotInvokeRecovery()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        var invoked = false;

        var actual = result.Recover(_ =>
        {
            invoked = true;
            return Result<Unit, Error>.Failure(
                Error.BadRequest("Rec", "Rec"));
        });

        invoked.ShouldBeFalse();
        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Recover_WhenRecoveryFails_PropagatesRecoveryFailure()
    {
        var recoveryError = Error.NotFound("Rec.Code", "Rec");

        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.Recover(
            _ => Result<Unit, Error>.Failure(recoveryError));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(recoveryError);
    }

    [Fact]
    public void Recover_WhenFailed_PassesErrorsToRecovery()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<Unit, Error>.Failure(error);

        IReadOnlyList<Error>? captured = null;

        result.Recover(errors =>
        {
            captured = errors;
            return Result<Unit, Error>.Ok(Unit.Value);
        });

        captured.ShouldHaveSingleItem().ShouldBe(error);
    }

    [Fact]
    public void Recover_WhenRecoveryIsNull_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.Recover((Func<IReadOnlyList<Error>, Result<Unit, Error>>?)null);

        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region FailIf

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void FailIf_WhenSuccessful_ReturnsExpectedResult(
        bool predicate,
        bool shouldFail)
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.FailIf(() => predicate, error);

        actual.IsFailure.ShouldBe(shouldFail);

        if (shouldFail)
        {
            actual.Errors.ShouldHaveSingleItem().ShouldBe(error);
        }
    }

    [Fact]
    public void FailIf_WhenAlreadyFailed_PreservesOriginalFailure()
    {
        var originalError = Error.BadRequest("Orig", "Orig");
        var result = Result<Unit, Error>.Failure(originalError);

        var actual = result.FailIf(
            () => true,
            Error.BadRequest("New", "New"));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(originalError);
    }

    [Fact]
    public void FailIf_WhenPredicateIsNull_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.FailIf(
            (Func<bool>?)null,
            Error.BadRequest("Code", "Desc"));

        actual.ShouldBeSameAs(result);
    }

    [Theory]
    [InlineData("TooHigh")]
    [InlineData("TooLow")]
    public void FailIf_WhenFactoryPredicateIsTrue_ReturnsFactoryError(
        string errorCode)
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.FailIf(
            () => true,
            () => Error.BadRequest(
                errorCode,
                "Value is out of range"));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().Code.ShouldBe(errorCode);
    }

    [Fact]
    public void FailIf_WhenFactoryIsNull_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.FailIf(
            () => true,
            (Func<Error>)null!);

        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region Ensure

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void Ensure_WhenSuccessful_ReturnsExpectedResult(
        bool predicate,
        bool shouldSucceed)
    {
        var error = Error.BadRequest("Invalid", "Invalid state");
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Ensure(() => predicate, error);

        actual.IsSuccess.ShouldBe(shouldSucceed);

        if (!shouldSucceed)
        {
            actual.Errors.ShouldHaveSingleItem().ShouldBe(error);
        }
    }

    [Fact]
    public void Ensure_WhenAlreadyFailed_PreservesOriginalFailure()
    {
        var originalError = Error.BadRequest("Orig", "Orig");
        var result = Result<Unit, Error>.Failure(originalError);

        var actual = result.Ensure(
            () => true,
            Error.BadRequest("New", "New"));

        actual.IsFailure.ShouldBeTrue();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(originalError);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Ensure_WhenValidatorReturnsResult_ReturnsValidatorResult(
        bool validatorSuccess)
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Ensure(() =>
            validatorSuccess
                ? Result<Unit, Error>.Ok(Unit.Value)
                : Result<Unit, Error>.Failure(
                    Error.BadRequest("Invalid", "Invalid state")));

        actual.IsSuccess.ShouldBe(validatorSuccess);
    }

    [Fact]
    public void Ensure_WhenAlreadyFailed_DoesNotRunValidator()
    {
        var originalError = Error.BadRequest("Orig", "Orig");
        var invoked = false;

        var result = Result<Unit, Error>.Failure(originalError);

        var actual = result.Ensure(() =>
        {
            invoked = true;
            return Result<Unit, Error>.Failure(
                Error.BadRequest("New", "New"));
        });

        invoked.ShouldBeFalse();
        actual.Errors.ShouldHaveSingleItem().ShouldBe(originalError);
    }

    [Fact]
    public void Ensure_WhenPredicateIsNull_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Ensure(
            (Func<bool>?)null,
            Error.BadRequest("Code", "Desc"));

        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void Ensure_WhenValidatorIsNull_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        var actual = result.Ensure((Func<Result<Unit, Error>>?)null);

        actual.ShouldBeSameAs(result);
    }

    #endregion

    #region Switch

    [Fact]
    public void Switch_WhenSuccessful_ExecutesOnlySuccessHandler()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        var successExecuted = false;
        var failureExecuted = false;

        result.Switch(
            () => successExecuted = true,
            _ => failureExecuted = true);

        successExecuted.ShouldBeTrue();
        failureExecuted.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenFailed_ExecutesOnlyFailureHandler()
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var successExecuted = false;
        var capturedErrorCount = 0;

        result.Switch(
            () => successExecuted = true,
            errors => capturedErrorCount = errors.Count);

        successExecuted.ShouldBeFalse();
        capturedErrorCount.ShouldBe(1);
    }

    [Fact]
    public void Switch_WhenSuccessHandlerIsNull_DoesNotThrow()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        Action act = () => result.Switch(null!, _ => { });

        act.ShouldNotThrow();
    }

    [Fact]
    public void Switch_WhenFailureHandlerIsNull_DoesNotThrow()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        Action act = () => result.Switch(() => { }, null!);

        act.ShouldNotThrow();
    }

    #endregion

    #region TapError

    [Fact]
    public void TapError_WhenSuccessful_DoesNotExecuteAction()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        var executed = false;

        var actual = result.TapError(_ => executed = true);

        executed.ShouldBeFalse();
        actual.ShouldBeSameAs(result);
    }

    [Theory]
    [InlineData("Code1", "Desc1")]
    [InlineData("Test.Code", "Description")]
    public void TapError_WhenFailed_PassesErrorToAction(
        string code,
        string description)
    {
        var error = Error.BadRequest(code, description);
        var result = Result<Unit, Error>.Failure(error);
        Error? captured = null;

        var actual = result.TapError(capturedError => captured = capturedError);

        captured.ShouldBe(error);
        actual.ShouldBeSameAs(result);
    }

    [Fact]
    public void TapError_WhenActionIsNull_ReturnsSameResult()
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var actual = result.TapError(null!);

        actual.ShouldBeSameAs(result);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    public void TapError_WhenChained_ExecutesEachAction(int chainCount)
    {
        var result = Result<Unit, Error>.Failure(
            Error.BadRequest("Code", "Desc"));

        var executionCount = 0;
        var actual = result;

        for (var i = 0; i < chainCount; i++)
        {
            actual = actual.TapError(_ => executionCount++);
        }

        executionCount.ShouldBe(chainCount);
        actual.ShouldBeSameAs(result);
    }

    #endregion
}
