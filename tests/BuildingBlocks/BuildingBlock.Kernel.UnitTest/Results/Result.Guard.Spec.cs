using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

[Trait("Category", "Unit")]
public class ResultGuardSpec
{
    [Theory]
    [InlineData(99, true, "result.status.invalid_status_code")]
    [InlineData(404, true, "result.status.invalid_success_status")]
    [InlineData(200, false, "result.status.invalid_failure_status")]
    public void ValidateStatusCode_Should_Return_An_Error_For_Invalid_Status(
        int statusCode,
        bool isSuccess,
        string expectedCode)
    {
        var error = ResultGuard.ValidateStatusCode(statusCode, isSuccess);

        error.ShouldNotBeNull();
        error.Code.ShouldBe(expectedCode);
    }

    [Fact]
    public void ValidateStatusCode_Should_Accept_Status_Within_Result_Range()
    {
        ResultGuard.ValidateStatusCode(201, isSuccess: true).ShouldBeNull();
        ResultGuard.ValidateStatusCode(404, isSuccess: false).ShouldBeNull();
    }

    [Fact]
    public void ValidateErrors_Should_Require_Errors_Only_For_Failures_And_Enforce_Limit()
    {
        ResultGuard.ValidateErrors(new List<Error>(), isSuccess: false)!
            .Code.ShouldBe(ResultConstant.Failure.Errors.Empty.Code);
        ResultGuard.ValidateErrors(new List<Error>(), isSuccess: true).ShouldBeNull();

        var tooManyErrors = Enumerable.Repeat<Error>(ResultStub.NotFoundError(),
            ResultConstant.Constraint.Errors.MaxCount + 1).ToList();
        ResultGuard.ValidateErrors(tooManyErrors, isSuccess: false)!
            .Code.ShouldBe(ResultConstant.Failure.Errors.ExceedsMaxCount.Code);
    }

    [Fact]
    public void ValidateValue_Should_Enforce_Success_And_Failure_Value_Rules()
    {
        ResultGuard.ValidateValue<string>(isSuccess: true, value: null!)!
            .Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
        ResultGuard.ValidateValue(isSuccess: false, value: "not allowed")!
            .Code.ShouldBe(ResultConstant.Failure.Value.NotAllowed.Code);
        ResultGuard.ValidateValue<string>(isSuccess: false, value: null!).ShouldBeNull();
    }

    [Fact]
    public void ValidateResultConsistency_Should_Reject_Empty_Or_Mixed_Status_Failures()
    {
        ResultGuard.ValidateResultConsistency(isSuccess: false, new List<Error>())!
            .Code.ShouldBe(ResultConstant.Failure.Result.FailureWithoutErrors.Code);

        ResultGuard.ValidateResultConsistency(
            isSuccess: false,
            new List<Error> { ResultStub.NotFoundError(), ResultStub.ConflictError() })!
            .Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
    }

    [Fact]
    public void ValidateResultConsistency_Should_Accept_Consistent_Failures_And_Success()
    {
        ResultGuard.ValidateResultConsistency(
            isSuccess: false,
            new List<Error> { ResultStub.NotFoundError(), ResultStub.NotFoundError() }).ShouldBeNull();
        ResultGuard.ValidateResultConsistency(isSuccess: true, new List<Error>()).ShouldBeNull();
    }
}
