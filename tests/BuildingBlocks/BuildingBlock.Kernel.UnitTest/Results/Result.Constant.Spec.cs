using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class ResultConstantSpec
{
    [Fact]
    public void Constraints_Should_Expose_Status_And_Collection_Limits()
    {
        ResultConstant.Constraint.Status.Min.ShouldBe(100);
        ResultConstant.Constraint.Status.Max.ShouldBe(599);
        ResultConstant.Constraint.Status.Success.Min.ShouldBe(200);
        ResultConstant.Constraint.Status.Success.Max.ShouldBe(299);
        ResultConstant.Constraint.Status.Failure.Min.ShouldBe(400);
        ResultConstant.Constraint.Status.Failure.Max.ShouldBe(599);
        ResultConstant.Constraint.Errors.MaxCount.ShouldBe(50);
        ResultConstant.Constraint.Metadata.MaxEntries.ShouldBe(50);
    }

    [Fact]
    public void Status_Codes_Should_Match_Their_Http_Values()
    {
        ResultConstant.StatusCode.Ok.ShouldBe(200);
        ResultConstant.StatusCode.Created.ShouldBe(201);
        ResultConstant.StatusCode.Accepted.ShouldBe(202);
        ResultConstant.StatusCode.NoContent.ShouldBe(204);
        ResultConstant.StatusCode.NotFound.ShouldBe(404);
        ResultConstant.StatusCode.InternalServerError.ShouldBe(500);
    }

    [Fact]
    public void Default_Should_Expose_Internal_Server_Error_Status_And_Empty_Errors()
    {
        ResultConstant.Default.Status.ShouldBe(500);
        ResultConstant.Default.EmptyErrors.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_Constants_Should_Provide_Stable_Codes_And_Messages()
    {
        ResultConstant.Failure.Result.MixedStatusCodes.Code.ShouldBe("result.errors.mixed_status_codes");
        ResultConstant.Failure.Result.MixedStatusCodes.Message.ShouldNotBeNullOrWhiteSpace();
        ResultConstant.Failure.Value.Required.Code.ShouldBe("result.value.required");
        ResultConstant.Failure.Status.InvalidFailureStatus.Message.ShouldContain("{0}");
    }
}
