using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class ResultFactorySpec
{
    [Theory]
    [InlineData(200, 200)]
    [InlineData(201, 201)]
    [InlineData(202, 202)]
    [InlineData(204, 204)]
    public void Success_Factories_Should_Use_Their_Standard_Status(int expected, int actual)
    {
        var result = actual switch
        {
            200 => Result.Ok(),
            201 => Result.Created(),
            202 => Result.Accepted(),
            204 => Result.NoContent(),
            _ => throw new ArgumentOutOfRangeException(nameof(actual)),
        };

        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(expected);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Success_With_Explicit_Status_Should_Use_Provided_Status()
    {
        var result = Result.Success(ResultConstant.StatusCode.Accepted);

        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
    }

    [Fact]
    public void Failure_Without_Explicit_Status_Should_Resolve_Status_From_Error()
    {
        var error = ResultStub.NotFoundError();

        var result = Result.Fail(error);

        result.IsFailure.ShouldBeTrue();
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        result.Errors.ShouldContain(error);
    }

    [Fact]
    public void Failure_With_Explicit_Status_Should_Use_Provided_Status()
    {
        var error = ResultStub.NotFoundError();

        var result = Result.Fail(new[] { error }, ResultConstant.StatusCode.InternalServerError);

        result.StatusCode.ShouldBe(ResultConstant.StatusCode.InternalServerError);
    }

    [Fact]
    public void Success_Factory_Should_Reject_Failure_Status_Code()
    {
        var act = () => Result.Success(ResultConstant.StatusCode.NotFound);

        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Failure_Factory_Should_Reject_Success_Status_Code()
    {
        var error = ResultStub.NotFoundError();

        var act = () => Result.Fail(new[] { error }, ResultConstant.StatusCode.Ok);

        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Failure_Factory_Should_Reject_Empty_Error_List()
    {
        var act = () => Result.Fail(Array.Empty<IError>());

        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Failure_Enumerable_Overloads_Should_Create_Failures()
    {
        var errors = new IError[] { ResultStub.NotFoundError() };

        var resolved = Result.Failure(errors);
        var explicitStatus = Result.Failure(errors, ResultConstant.StatusCode.ServiceUnavailable);

        resolved.IsFailure.ShouldBeTrue();
        resolved.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        explicitStatus.StatusCode.ShouldBe(ResultConstant.StatusCode.ServiceUnavailable);
        explicitStatus.Errors.ShouldBe(errors);
    }

    [Fact]
    public void Fail_With_Non_List_Enumerable_Should_Materialize_Errors()
    {
        var result = Result.Fail(Enumerate(ResultStub.NotFoundError()));

        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Code.ShouldBe("test.not_found");
    }

    [Fact]
    public void Failure_When_No_Error_Carries_Status_Should_Default_To_Internal_Server_Error()
    {
        var critical = Error.Custom("test.critical", "critical", severity: ErrorSeverity.Critical);
        var info = Error.Custom("test.info", "info", severity: ErrorSeverity.Info);

        var result = Result.Fail(critical, info);

        result.StatusCode.ShouldBe(ResultConstant.StatusCode.InternalServerError);
    }

    [Fact]
    public void Failure_With_Mixed_Status_Errors_Should_Reject()
    {
        var withStatus = ResultStub.NotFoundError();
        var criticalNoStatus = Error.Custom("test.critical", "critical", severity: ErrorSeverity.Critical);
        var infoNoStatus = Error.Custom("test.info", "info", severity: ErrorSeverity.Info);

        var act = () => Result.Fail(withStatus, criticalNoStatus, infoNoStatus);

        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain(ResultConstant.Failure.Result.MixedStatusCodes.Code);
    }

    [Fact]
    public void Custom_Without_Explicit_Status_Should_Resolve_Defaults()
    {
        var success = Result.Custom(isSuccess: true);

        success.IsSuccess.ShouldBeTrue();
        success.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
        success.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Custom_Failure_Without_Explicit_Status_Should_Resolve_From_Errors()
    {
        var result = Result.Custom(
            isSuccess: false,
            errors: new IError[] { ResultStub.NotFoundError() });

        result.IsFailure.ShouldBeTrue();
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    [Fact]
    public void Custom_Failure_Without_Errors_Should_Reject()
    {
        var act = () => Result.Custom(isSuccess: false);

        Should.Throw<ArgumentException>(act);
    }

    private static IEnumerable<IError> Enumerate(IError error)
    {
        yield return error;
    }
}
