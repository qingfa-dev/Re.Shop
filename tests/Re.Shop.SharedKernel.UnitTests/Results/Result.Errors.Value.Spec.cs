using Shouldly;
namespace SharedKernel.UnitTests.Results;

public class ResultErrorsValueSpec
{
    #region Failure

    [Fact]
    public void Failure_ValueRequired_ShouldMatchConstants()
    {
        ResultResult.Failure.ValueRequired.Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
        ResultResult.Failure.ValueRequired.Message.ShouldBe(ResultConstant.Failure.Value.Required.Message);
        ResultResult.Failure.ValueRequired.Status.ShouldBe(422);
    }

    [Fact]
    public void Failure_ValueNotAllowed_ShouldMatchConstants()
    {
        ResultResult.Failure.ValueNotAllowed.Code.ShouldBe(ResultConstant.Failure.Value.NotAllowed.Code);
        ResultResult.Failure.ValueNotAllowed.Message.ShouldBe(ResultConstant.Failure.Value.NotAllowed.Message);
        ResultResult.Failure.ValueNotAllowed.Status.ShouldBe(422);
    }

    [Fact]
    public void Failure_ExceedsMaxMetadataEntries_ShouldMatchConstants()
    {
        ResultResult.Failure.ExceedsMaxMetadataEntries.Code
            .ShouldBe(ResultConstant.Failure.Value.ExceedsMaxMetadataEntries.Code);
        ResultResult.Failure.ExceedsMaxMetadataEntries.Message
            .ShouldBe(string.Format(
                ResultConstant.Failure.Value.ExceedsMaxMetadataEntries.Message,
                ResultConstant.Constraint.Metadata.MaxEntries));
        ResultResult.Failure.ExceedsMaxMetadataEntries.Status.ShouldBe(422);
    }

    [Fact]
    public void Failure_InvalidStatusCode_ShouldFormatStatus()
    {
        var error = ResultResult.Failure.InvalidStatusCode(999);

        error.Code.ShouldBe(ResultConstant.Failure.Status.InvalidStatusCode.Code);
        error.Message.ShouldContain("999");
        error.Status.ShouldBe(422);
    }

    [Fact]
    public void Failure_InvalidSuccessStatus_ShouldFormatStatus()
    {
        var error = ResultResult.Failure.InvalidSuccessStatus(400);

        error.Code.ShouldBe(ResultConstant.Failure.Status.InvalidSuccessStatus.Code);
        error.Message.ShouldContain("400");
    }

    [Fact]
    public void Failure_InvalidFailureStatus_ShouldFormatStatus()
    {
        var error = ResultResult.Failure.InvalidFailureStatus(200);

        error.Code.ShouldBe(ResultConstant.Failure.Status.InvalidFailureStatus.Code);
        error.Message.ShouldContain("200");
    }

    #endregion
}
