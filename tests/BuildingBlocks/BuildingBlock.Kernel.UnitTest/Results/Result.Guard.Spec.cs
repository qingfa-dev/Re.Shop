using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for <see cref="ResultGuard"/> validation methods.</summary>
[Trait("Category", "Unit")]
public class ResultGuardSpec
{
    #region ValidateStatusCode

    /// <summary>ValidateStatusCode should reject out-of-range and wrong-kind statuses and accept valid ones.</summary>
    [Theory]
    [InlineData(99, true, "result.status.invalid_status_code")]
    [InlineData(600, false, "result.status.invalid_status_code")]
    [InlineData(100, true, "result.status.invalid_success_status")]
    [InlineData(199, true, "result.status.invalid_success_status")]
    [InlineData(300, true, "result.status.invalid_success_status")]
    [InlineData(599, true, "result.status.invalid_success_status")]
    [InlineData(404, true, "result.status.invalid_success_status")]
    [InlineData(200, false, "result.status.invalid_failure_status")]
    [InlineData(399, false, "result.status.invalid_failure_status")]
    [InlineData(200, true, null)]
    [InlineData(201, true, null)]
    [InlineData(299, true, null)]
    [InlineData(400, false, null)]
    [InlineData(404, false, null)]
    [InlineData(599, false, null)]
    public void ValidateStatusCode_Should_Enforce_Range_And_Result_State_Rules(
        int statusCode,
        bool isSuccess,
        string? expectedCode)
    {
        // Act
        var error = ResultGuard.ValidateStatusCode(statusCode, isSuccess);

        // Assert
        if (expectedCode is null)
        {
            error.ShouldBeNull();
        }
        else
        {
            error.ShouldNotBeNull();
            error.Code.ShouldBe(expectedCode);
        }
    }

    #endregion

    #region ValidateErrors

    /// <summary>ValidateErrors should require errors only for failures and enforce the count limit.</summary>
    [Theory]
    [InlineData(0, false, ResultConstant.Failure.Errors.Empty.Code)]
    [InlineData(0, true, null)]
    [InlineData(ResultConstant.Constraint.Errors.MaxCount, false, null)]
    [InlineData(ResultConstant.Constraint.Errors.MaxCount + 1, false, ResultConstant.Failure.Errors.ExceedsMaxCount.Code)]
    public void ValidateErrors_Should_Require_Errors_Only_For_Failures_And_Enforce_Limit(
        int errorCount,
        bool isSuccess,
        string? expectedCode)
    {
        // Arrange
        var errors = Enumerable.Repeat(ResultStub.NotFoundError(), errorCount).ToList();

        // Act
        var error = ResultGuard.ValidateErrors(errors, isSuccess);

        // Assert
        if (expectedCode is null)
        {
            error.ShouldBeNull();
        }
        else
        {
            error.ShouldNotBeNull();
            error.Code.ShouldBe(expectedCode);
        }
    }

    #endregion

    #region ValidateValue

    /// <summary>ValidateValue should enforce success and failure value rules.</summary>
    [Theory]
    [InlineData(true, null, ResultConstant.Failure.Value.Required.Code)]
    [InlineData(false, "not allowed", ResultConstant.Failure.Value.NotAllowed.Code)]
    [InlineData(false, null, null)]
    [InlineData(true, "value", null)]
    public void ValidateValue_Should_Enforce_Success_And_Failure_Value_Rules(
        bool isSuccess,
        string? value,
        string? expectedCode)
    {
        // Act
        var error = ResultGuard.ValidateValue<string>(isSuccess, value!);

        // Assert
        if (expectedCode is null)
        {
            error.ShouldBeNull();
        }
        else
        {
            error.ShouldNotBeNull();
            error.Code.ShouldBe(expectedCode);
        }
    }

    #endregion

    #region ValidateResultConsistency

    /// <summary>ValidateResultConsistency should reject empty or mixed failures and accept consistent failures and success.</summary>
    [Theory]
    [MemberData(nameof(ValidateResultConsistencyCases))]
    public void ValidateResultConsistency_Should_Enforce_Failure_And_Success_Rules(
        bool isSuccess,
        List<Error> errors,
        string? expectedCode)
    {
        // Act
        var error = ResultGuard.ValidateResultConsistency(isSuccess, errors);

        // Assert
        if (expectedCode is null)
        {
            error.ShouldBeNull();
        }
        else
        {
            error.ShouldNotBeNull();
            error.Code.ShouldBe(expectedCode);
        }
    }

    public static TheoryData<bool, List<Error>, string?> ValidateResultConsistencyCases()
    {
        var data = new TheoryData<bool, List<Error>, string?>();
        data.Add(false, new List<Error>(), ResultConstant.Failure.Result.FailureWithoutErrors.Code);
        data.Add(
            false,
            new List<Error> { ResultStub.NotFoundError(), ResultStub.ConflictError() },
            ResultConstant.Failure.Result.MixedStatusCodes.Code);
        data.Add(false, new List<Error> { ResultStub.NotFoundError(), ResultStub.NotFoundError() }, null);
        data.Add(true, new List<Error>(), null);
        return data;
    }

    #endregion
}
