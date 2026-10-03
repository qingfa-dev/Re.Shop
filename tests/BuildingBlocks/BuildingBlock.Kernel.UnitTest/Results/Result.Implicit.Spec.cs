using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for implicit conversion operators on <see cref="Result"/>.</summary>
[Trait("Category", "Unit")]
public class ResultImplicitSpec
{
    #region Non-generic Failure

    /// <summary>Error should implicitly convert to non-generic failure.</summary>
    [Fact]
    public void Error_Should_Implicitly_Convert_To_NonGeneric_Failure()
    {
        // Arrange
        var error = ResultStub.NotFoundError();

        // Act
        Result result = error;

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(new[] { error });
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    /// <summary>Error array should implicitly convert to non-generic failure.</summary>
    [Fact]
    public void ErrorArray_Should_Implicitly_Convert_To_NonGeneric_Failure()
    {
        // Arrange
        var errors = new[] { ResultStub.NotFoundError() };

        // Act
        Result result = errors;

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(errors);
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    #endregion

    #region Error Collection

    /// <summary>Error collections should use Fail factory for non-generic results.</summary>
    [Fact]
    public void Error_Collection_Should_Use_Fail_Factory_For_NonGeneric_Results()
    {
        // Arrange
        List<Error> errors = new List<Error> { ResultStub.NotFoundError() };

        // Act
        Result nonGeneric = Result.Fail(errors);

        // Assert
        nonGeneric.IsFailure.ShouldBeTrue();
        nonGeneric.Errors.ShouldBe(errors);
    }

    #endregion
}
