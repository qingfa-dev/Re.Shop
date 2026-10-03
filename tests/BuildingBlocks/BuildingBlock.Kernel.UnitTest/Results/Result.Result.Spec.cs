using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for <see cref="ResultError"/> factory methods.</summary>
[Trait("Category", "Unit")]
public class ResultErrorSpec
{
    #region Status

    /// <summary>Status error factories should identify their status kind.</summary>
    [Theory]
    [MemberData(nameof(StatusFactoryCases))]
    public void Invalid_Status_Error_Factories_Should_Identify_Status_Kind(Error error, string expectedCode)
    {
        // Arrange
        // Act
        // Assert
        error.Code.ShouldBe(expectedCode);
    }

    public static TheoryData<Error, string> StatusFactoryCases()
    {
        var data = new TheoryData<Error, string>();
        data.Add(ResultError.Status.InvalidStatusCode(99), ResultConstant.Failure.Status.InvalidStatusCode.Code);
        data.Add(ResultError.Status.InvalidSuccessStatus(404), ResultConstant.Failure.Status.InvalidSuccessStatus.Code);
        data.Add(ResultError.Status.InvalidFailureStatus(200), ResultConstant.Failure.Status.InvalidFailureStatus.Code);
        return data;
    }

    #endregion

    #region Result and Errors

    /// <summary>Result and error validation factories should expose their codes.</summary>
    [Theory]
    [MemberData(nameof(ResultAndErrorsFactoryCases))]
    public void Result_And_Error_Validation_Factories_Should_Expose_Their_Codes(Error error, string expectedCode)
    {
        // Arrange
        // Act
        // Assert
        error.Code.ShouldBe(expectedCode);
    }

    public static TheoryData<Error, string> ResultAndErrorsFactoryCases()
    {
        var data = new TheoryData<Error, string>();
        data.Add(ResultError.Result.MixedStatusCodes(), ResultConstant.Failure.Result.MixedStatusCodes.Code);
        data.Add(ResultError.Result.FailureWithoutErrors(), ResultConstant.Failure.Result.FailureWithoutErrors.Code);
        data.Add(ResultError.Errors.Empty(), ResultConstant.Failure.Errors.Empty.Code);
        data.Add(ResultError.Errors.ExceedsMaxCount(), ResultConstant.Failure.Errors.ExceedsMaxCount.Code);
        return data;
    }

    #endregion

    #region Metadata and Value

    /// <summary>Metadata and value error factories should format arguments.</summary>
    [Theory]
    [MemberData(nameof(MetadataAndValueFactoryCases))]
    public void Metadata_And_Value_Error_Factories_Should_Format_Arguments(
        Error error,
        string expectedCode,
        string? expectedMessageFragment)
    {
        // Arrange
        // Act
        // Assert
        error.Code.ShouldBe(expectedCode);

        if (expectedMessageFragment is not null)
        {
            error.Message.ShouldContain(expectedMessageFragment);
        }
    }

    public static TheoryData<Error, string, string?> MetadataAndValueFactoryCases()
    {
        var data = new TheoryData<Error, string, string?>();
        data.Add(
            ResultError.Metadata.NullValue("trace.id"),
            ResultConstant.Failure.Metadata.NullValue.Code,
            "trace.id");
        data.Add(ResultError.Metadata.EmptyKey(), ResultConstant.Failure.Metadata.EmptyKey.Code, null);
        data.Add(ResultError.Metadata.ExceedsMaxEntries(), ResultConstant.Failure.Metadata.ExceedsMaxEntries.Code, null);
        data.Add(ResultError.Value.Required(), ResultConstant.Failure.Value.Required.Code, null);
        data.Add(ResultError.Value.NotAllowed(), ResultConstant.Failure.Value.NotAllowed.Code, null);
        return data;
    }

    #endregion
}
