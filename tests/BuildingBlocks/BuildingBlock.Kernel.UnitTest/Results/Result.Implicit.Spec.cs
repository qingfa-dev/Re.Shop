using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

[Trait("Category", "Unit")]
public class ResultImplicitSpec
{
    [Fact]
    public void TValue_Should_Implicitly_Convert_To_Generic_Success()
    {
        Result<string> result = "value";

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("value");
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    [Fact]
    public void Error_Should_Implicitly_Convert_To_Generic_Failure()
    {
        var error = ResultStub.NotFoundError();

        Result<string> result = error;

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(new[] { error });
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    [Fact]
    public void ErrorArray_Should_Implicitly_Convert_To_Generic_Failure()
    {
        var errors = new[] { ResultStub.NotFoundError() };

        Result<string> result = errors;

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(errors);
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    [Fact]
    public void Error_Should_Implicitly_Convert_To_NonGeneric_Failure()
    {
        var error = ResultStub.NotFoundError();

        Result result = error;

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(new[] { error });
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    [Fact]
    public void ErrorArray_Should_Implicitly_Convert_To_NonGeneric_Failure()
    {
        var errors = new[] { ResultStub.NotFoundError() };

        Result result = errors;

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(errors);
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    [Fact]
    public void Direct_Returns_Should_Convert_Values_And_Errors_To_Generic_Results()
    {
        var success = ReturnValue();
        var failure = ReturnError();

        success.IsSuccess.ShouldBeTrue();
        success.Value.ShouldBe(42);
        failure.IsFailure.ShouldBeTrue();
        failure.Errors.ShouldContain(error => error.Code == "test.not_found");

        static Result<int> ReturnValue() => 42;
        static Result<int> ReturnError() => ResultStub.NotFoundError();
    }

    [Fact]
    public void Error_Collection_Should_Use_Fail_Factory_For_Generic_And_NonGeneric_Results()
    {
        List<Error> errors = new List<Error> { ResultStub.NotFoundError() };

        Result<int> generic = Result<int>.Fail(errors);
        Result nonGeneric = Result.Fail(errors);

        generic.IsFailure.ShouldBeTrue();
        generic.Errors.ShouldBe(errors);
        nonGeneric.IsFailure.ShouldBeTrue();
        nonGeneric.Errors.ShouldBe(errors);
    }

}
