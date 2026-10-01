using SharedKernel.Results;
using Shouldly;

namespace SharedKernel.UnitTests.Results;

public class ResultConstantSpec
{
    [Fact]
    public void Constraints_ExposeErrorMetadataAndStatusRangesInNestedGroups()
    {
        ResultConstant.Constraint.Errors.MaxCount.ShouldBe(10);
        ResultConstant.Constraint.Metadata.MaxEntries.ShouldBe(50);
        ResultConstant.Constraint.Status.Min.ShouldBe(100);
        ResultConstant.Constraint.Status.Max.ShouldBe(599);
        ResultConstant.Constraint.Status.Success.Min.ShouldBe(200);
        ResultConstant.Constraint.Status.Success.Max.ShouldBe(299);
        ResultConstant.Constraint.Status.Failure.Min.ShouldBe(400);
        ResultConstant.Constraint.Status.Failure.Max.ShouldBe(599);
    }

    [Fact]
    public void StatusCodeFactories_ExposeNamedSuccessAndFailureCodes()
    {
        ResultConstant.StatusCode.Success.Ok.ShouldBe(200);
        ResultConstant.StatusCode.Success.Created.ShouldBe(201);
        ResultConstant.StatusCode.Success.Accepted.ShouldBe(202);
        ResultConstant.StatusCode.Success.NoContent.ShouldBe(204);
        ResultConstant.Default.Status.ShouldBe(500);
    }

    [Fact]
    public void FailureCodes_UseLowercaseObjectCategoryReasonFormat()
    {
        ResultConstant.Failure.Metadata.EmptyKey.Code.ShouldBe("result.metadata.empty_key");
        ResultConstant.Failure.Metadata.NullValue.Code.ShouldBe("result.metadata.null_value");
        ResultConstant.Failure.Value.ExceedsMaxMetadataEntries.Code.ShouldBe("result.metadata.exceeds_max_entries");
        ResultConstant.Failure.Value.Required.Code.ShouldBe("result.value.required");
        ResultConstant.Failure.Value.NotAllowed.Code.ShouldBe("result.value.not_allowed");
        ResultConstant.Failure.Status.InvalidStatusCode.Code.ShouldBe("result.status.invalid_status_code");
        ResultConstant.Failure.Status.InvalidSuccessStatus.Code.ShouldBe("result.status.invalid_success_status");
        ResultConstant.Failure.Status.InvalidFailureStatus.Code.ShouldBe("result.status.invalid_failure_status");
        ResultConstant.Failure.Errors.Empty.Code.ShouldBe("result.errors.empty");
        ResultConstant.Failure.Errors.ExceedsMaxRange.Code.ShouldBe("result.errors.exceeds_max_range");
        ResultConstant.Failure.Result.MixedStatusCodes.Code.ShouldBe("result.errors.mixed_status_codes");
        ResultConstant.Failure.Result.FailureWithoutErrors.Code.ShouldBe("result.failure.without_errors");
    }

    [Fact]
    public void LegacyValueResultFailureCodes_UseSameResultCodeFormat()
    {
        ResultConstant.Failure.ValueResult.MetadataEmptyKey.Code.ShouldBe("result.metadata.empty_key");
        ResultConstant.Failure.ValueResult.MetadataNullValue.Code.ShouldBe("result.metadata.null_value");
        ResultConstant.Failure.ValueResult.EmptyErrors.Code.ShouldBe("result.errors.empty");
        ResultConstant.Failure.ValueResult.ExceedsMaxErrors.Code.ShouldBe("result.errors.exceeds_max_range");
        ResultConstant.Failure.ValueResult.MixedStatusCodes.Code.ShouldBe("result.errors.mixed_status_codes");
        ResultConstant.Failure.ValueResult.FailureWithoutErrors.Code.ShouldBe("result.failure.without_errors");
    }
}
