using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for <see cref="ResultGuard"/> validation methods.</summary>
[Trait("Category", "Unit")]
public class ResultGuardSpec
{
    #region ValidateStatusCode

    /// <summary>ValidateStatusCode should return an error for invalid status codes.</summary>
    [Theory]
    [InlineData(99, true, "result.status.invalid_status_code")]
    [InlineData(404, true, "result.status.invalid_success_status")]
    [InlineData(200, false, "result.status.invalid_failure_status")]
    public void ValidateStatusCode_Should_Return_An_Error_For_Invalid_Status(
        int statusCode,
        bool isSuccess,
        string expectedCode)
    {
        // Act
        var error = ResultGuard.ValidateStatusCode(statusCode, isSuccess);

        // Assert
        error.ShouldNotBeNull();
        error.Code.ShouldBe(expectedCode);
    }

    /// <summary>ValidateStatusCode should accept status within the result range.</summary>
    [Fact]
    public void ValidateStatusCode_Should_Accept_Status_Within_Result_Range()
    {
        // Act & Assert
        ResultGuard.ValidateStatusCode(201, isSuccess: true).ShouldBeNull();
        ResultGuard.ValidateStatusCode(404, isSuccess: false).ShouldBeNull();
    }

    #endregion

    #region ValidateErrors

    /// <summary>ValidateErrors should require errors for failures and enforce the count limit.</summary>
    [Fact]
    public void ValidateErrors_Should_Require_Errors_Only_For_Failures_And_Enforce_Limit()
    {
        // Act & Assert
        ResultGuard.ValidateErrors(new List<Error>(), isSuccess: false)!
            .Code.ShouldBe(ResultConstant.Failure.Errors.Empty.Code);
        ResultGuard.ValidateErrors(new List<Error>(), isSuccess: true).ShouldBeNull();

        var tooManyErrors = Enumerable.Repeat<Error>(ResultStub.NotFoundError(),
            ResultConstant.Constraint.Errors.MaxCount + 1).ToList();
        ResultGuard.ValidateErrors(tooManyErrors, isSuccess: false)!
            .Code.ShouldBe(ResultConstant.Failure.Errors.ExceedsMaxCount.Code);
    }

    #endregion

    #region ValidateValue

    /// <summary>ValidateValue should enforce success and failure value rules.</summary>
    [Fact]
    public void ValidateValue_Should_Enforce_Success_And_Failure_Value_Rules()
    {
        // Act & Assert
        ResultGuard.ValidateValue<string>(isSuccess: true, value: null!)!
            .Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
        ResultGuard.ValidateValue(isSuccess: false, value: "not allowed")!
            .Code.ShouldBe(ResultConstant.Failure.Value.NotAllowed.Code);
        ResultGuard.ValidateValue<string>(isSuccess: false, value: null!).ShouldBeNull();
    }

    #endregion

    #region ValidateResultConsistency

    /// <summary>ValidateResultConsistency should reject empty or mixed-status failures.</summary>
    [Fact]
    public void ValidateResultConsistency_Should_Reject_Empty_Or_Mixed_Status_Failures()
    {
        // Act & Assert
        ResultGuard.ValidateResultConsistency(isSuccess: false, new List<Error>())!
            .Code.ShouldBe(ResultConstant.Failure.Result.FailureWithoutErrors.Code);

        ResultGuard.ValidateResultConsistency(
            isSuccess: false,
            new List<Error> { ResultStub.NotFoundError(), ResultStub.ConflictError() })!
            .Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
    }

    /// <summary>ValidateResultConsistency should accept consistent failures and success.</summary>
    [Fact]
    public void ValidateResultConsistency_Should_Accept_Consistent_Failures_And_Success()
    {
        // Act & Assert
        ResultGuard.ValidateResultConsistency(
            isSuccess: false,
            new List<Error> { ResultStub.NotFoundError(), ResultStub.NotFoundError() }).ShouldBeNull();
        ResultGuard.ValidateResultConsistency(isSuccess: true, new List<Error>()).ShouldBeNull();
    }

    #endregion
}
