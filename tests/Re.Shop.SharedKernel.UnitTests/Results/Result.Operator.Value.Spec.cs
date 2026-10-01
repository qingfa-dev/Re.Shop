using Shouldly;
namespace SharedKernel.UnitTests.Results;

public class ResultOperatorValueSpec
{
    [Theory]
    [InlineData("hello")]
    [InlineData("")]
    public void ImplicitOperator_Value_ShouldCreateSuccessResult(string value)
    {
        Result<string, Error> result = value;

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(value);
        result.Status.ShouldBe(200);
    }

    [Fact]
    public void ImplicitOperator_Error_ShouldCreateFailureResult()
    {
        Error error = Error.BadRequest("Code", "Desc");

        Result<string, Error> result = error;

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldBe(new[] { error });
    }

    [Fact]
    public void ImplicitOperator_ErrorArray_ShouldCreateFailureResult()
    {
        Error[] errors =
        [
            Error.BadRequest("Code1", "Desc1"),
            Error.BadRequest("Code2", "Desc2")
        ];

        Result<string, Error> result = errors;

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void ImplicitOperator_ListErrors_ShouldCreateFailureResult()
    {
        List<Error> errors =
        [
            Error.BadRequest("Code1", "Desc1"),
            Error.BadRequest("Code2", "Desc2")
        ];

        Result<string, Error> result = errors;

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(2);
    }

    [Theory]
    [InlineData("Test.Code", "Not found")]
    [InlineData("Other.Code", "Other desc")]
    public void ImplicitOperator_Error_ShouldPreserveErrorDetails(string code, string desc)
    {
        var error = Error.NotFound(code, desc);

        Result<string, Error> result = error;

        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(code);
        result.Errors.ShouldHaveSingleItem().Message.ShouldBe(desc);
    }

    [Fact]
    public void ImplicitOperator_ErrorArray_MixedErrors_ShouldReturnFailureWithMixedStatusCodesError()
    {
        Error[] errors =
        [
            Error.BadRequest("Code1", "Desc1"),
            Error.NotFound("Code2", "Desc2")
        ];

        Result<string, Error> result = errors;

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
    }
}