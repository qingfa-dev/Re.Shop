using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

[Trait("Category", "Unit")]
public class ResultInterfaceSpec
{
    [Fact]
    public void NonGeneric_Result_Should_Implement_IResult()
    {
        IResult<Error> result = Result.Ok();

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.HasValue.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    [Fact]
    public void Generic_Result_Should_Implement_IResult_And_IResultOfValue()
    {
        IResult<int, Error> result = Result<int>.Ok(42);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
        result.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void Generic_Failure_Should_Expose_Failed_State_Through_IResult_Contract()
    {
        IResult<int, Error> result = Result<int>.Fail(ResultStub.NotFoundError());

        result.IsFailure.ShouldBeTrue();
        result.HasValue.ShouldBeFalse();
        result.TryGetValue(out var value).ShouldBeFalse();
        value.ShouldBe(default);
        result.Errors.ShouldHaveSingleItem();
    }
}
