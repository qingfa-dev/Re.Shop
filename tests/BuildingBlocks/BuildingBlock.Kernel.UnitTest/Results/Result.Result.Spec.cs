using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class ResultErrorSpec
{
    [Fact]
    public void Invalid_Status_Error_Factories_Should_Identify_Status_Kind()
    {
        ResultError.Status.InvalidStatusCode(99).Code
            .ShouldBe(ResultConstant.Failure.Status.InvalidStatusCode.Code);
        ResultError.Status.InvalidSuccessStatus(404).Code
            .ShouldBe(ResultConstant.Failure.Status.InvalidSuccessStatus.Code);
        ResultError.Status.InvalidFailureStatus(200).Code
            .ShouldBe(ResultConstant.Failure.Status.InvalidFailureStatus.Code);
    }

    [Fact]
    public void Result_And_Error_Validation_Factories_Should_Expose_Their_Codes()
    {
        ResultError.Result.MixedStatusCodes().Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
        ResultError.Result.FailureWithoutErrors().Code.ShouldBe(ResultConstant.Failure.Result.FailureWithoutErrors.Code);
        ResultError.Errors.Empty().Code.ShouldBe(ResultConstant.Failure.Errors.Empty.Code);
        ResultError.Errors.ExceedsMaxCount().Code.ShouldBe(ResultConstant.Failure.Errors.ExceedsMaxCount.Code);
    }

    [Fact]
    public void Metadata_And_Value_Error_Factories_Should_Format_Arguments()
    {
        ResultError.Metadata.NullValue("trace.id").Message.ShouldContain("trace.id");
        ResultError.Metadata.EmptyKey().Code.ShouldBe(ResultConstant.Failure.Metadata.EmptyKey.Code);
        ResultError.Metadata.ExceedsMaxEntries().Code.ShouldBe(ResultConstant.Failure.Metadata.ExceedsMaxEntries.Code);
        ResultError.Value.Required().Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
        ResultError.Value.NotAllowed().Code.ShouldBe(ResultConstant.Failure.Value.NotAllowed.Code);
    }
}
