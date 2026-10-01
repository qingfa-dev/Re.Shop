using Shouldly;
namespace SharedKernel.UnitTests.Results;

public class ResultFailureSpec
{
    [Fact]
    public void Failure_EmptyErrors_ShouldMatchConstants()
    {
        ResultResult.EmptyErrors.Code.ShouldBe(ResultConstant.Failure.Errors.Empty.Code);
        ResultResult.EmptyErrors.Message.ShouldBe(ResultConstant.Failure.Errors.Empty.Message);
        ResultResult.EmptyErrors.Status.ShouldBe(500);
    }

    [Fact]
    public void Failure_ExceedsMaxErrors_ShouldMatchConstants()
    {
        ResultResult.ExceedsMaxErrors.Code.ShouldBe(ResultConstant.Failure.Errors.ExceedsMaxRange.Code);
        ResultResult.ExceedsMaxErrors.Message.ShouldBe(
            string.Format(
                ResultConstant.Failure.Errors.ExceedsMaxRange.Message,
                ResultConstant.Constraint.Errors.MaxCount));
        ResultResult.ExceedsMaxErrors.Status.ShouldBe(500);
    }

    [Fact]
    public void Failure_MixedStatusCodes_ShouldMatchConstants()
    {
        ResultResult.MixedStatusCodes.Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
        ResultResult.MixedStatusCodes.Message.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Message);
        ResultResult.MixedStatusCodes.Status.ShouldBe(500);
    }

    [Fact]
    public void Failure_FailureWithoutErrors_ShouldMatchConstants()
    {
        ResultResult.FailureWithoutErrors.Code.ShouldBe(ResultConstant.Failure.Result.FailureWithoutErrors.Code);
        ResultResult.FailureWithoutErrors.Message.ShouldBe(ResultConstant.Failure.Result.FailureWithoutErrors.Message);
        ResultResult.FailureWithoutErrors.Status.ShouldBe(500);
    }
}