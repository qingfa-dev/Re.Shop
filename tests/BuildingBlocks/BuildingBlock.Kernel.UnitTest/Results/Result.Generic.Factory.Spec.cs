using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class ResultGenericFactorySpec
{
    [Fact]
    public void Success_Factories_Should_Use_Their_Standard_Status()
    {
        Result<int>.Ok(1).StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
        Result<int>.Created(2).StatusCode.ShouldBe(ResultConstant.StatusCode.Created);
        Result<int>.Accepted(3).StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
    }

    [Fact]
    public void Success_With_Explicit_Status_Should_Use_Provided_Status()
    {
        var result = Result<int>.Ok(1, ResultConstant.StatusCode.Accepted);

        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
        result.Value.ShouldBe(1);
    }

    [Fact]
    public void Failure_Without_Explicit_Status_Should_Resolve_Status_From_Error()
    {
        var result = Result<int>.Fail(ResultStub.NotFoundError());

        result.IsFailure.ShouldBeTrue();
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        result.TryGetValue(out _).ShouldBeFalse();
    }

    [Fact]
    public void Failure_With_Explicit_Status_Should_Use_Provided_Status()
    {
        var result = Result<int>.Fail(
            new[] { ResultStub.NotFoundError() },
            ResultConstant.StatusCode.InternalServerError);

        result.StatusCode.ShouldBe(ResultConstant.StatusCode.InternalServerError);
    }

    [Fact]
    public void Success_Factory_Should_Reject_Failure_Status_Code()
    {
        var act = () => Result<int>.Success(1, ResultConstant.StatusCode.NotFound);

        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Failure_Factory_Should_Reject_Success_Status_Code()
    {
        var act = () => Result<int>.Fail(
            new[] { ResultStub.NotFoundError() },
            ResultConstant.StatusCode.Ok);

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

        var resolved = Result<string>.Failure(errors);
        var explicitStatus = Result<string>.Failure(errors, ResultConstant.StatusCode.ServiceUnavailable);

        resolved.IsFailure.ShouldBeTrue();
        resolved.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        explicitStatus.StatusCode.ShouldBe(ResultConstant.StatusCode.ServiceUnavailable);
        explicitStatus.Errors.ShouldBe(errors);
    }

    [Fact]
    public void Generic_Fail_With_Non_List_Enumerable_Should_Materialize_Errors()
    {
        var result = Result<string>.Fail(Enumerate(ResultStub.NotFoundError()));

        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Code.ShouldBe("test.not_found");
    }

    private static IEnumerable<IError> Enumerate(IError error)
    {
        yield return error;
    }
}
