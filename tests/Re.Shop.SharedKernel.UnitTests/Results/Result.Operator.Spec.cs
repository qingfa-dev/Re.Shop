using Shouldly;
namespace SharedKernel.UnitTests.Results;

public class ResultOperatorSpec
{
    #region Implicit Operators

    [Theory]
    [InlineData("Code1", "Desc1")]
    [InlineData("Test.Code", "Description")]
    public void ImplicitOperator_SingleError_ShouldCreateFailureResult(string code, string desc)
    {
        Error error = Error.BadRequest(code, desc);

        Result<Unit, Error> result = error;

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldBe(new[] { error });
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public void ImplicitOperator_ErrorArray_ShouldCreateFailureResult(int errorCount)
    {
        var errors = Enumerable.Range(0, errorCount)
            .Select(i => Error.BadRequest($"Code{i}", $"Desc{i}"))
            .ToArray();

        Result<Unit, Error> result = errors;

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(errorCount);
    }

    [Theory]
    [InlineData("Code1", "Desc1")]
    [InlineData("Code2", "Desc2")]
    public void ImplicitOperator_ListErrors_ShouldCreateFailureResult(string code, string desc)
    {
        var errors = new List<Error> { Error.BadRequest(code, desc) };

        Result<Unit, Error> result = errors;

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData("Test.Code", "Not found")]
    [InlineData("Other.Code", "Other desc")]
    public void ImplicitOperator_SingleError_ShouldPreserveErrorDetails(string code, string desc)
    {
        var error = Error.NotFound(code, desc);

        Result<Unit, Error> result = error;

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

        Result<Unit, Error> result = errors;

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
    }

    #endregion
}