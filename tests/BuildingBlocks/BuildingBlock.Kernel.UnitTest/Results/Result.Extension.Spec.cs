using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for <see cref="ResultExtension"/> metadata and status extensions.</summary>
[Trait("Category", "Unit")]
public class ResultExtensionSpec
{
    #region Metadata and Status

    /// <summary>Non-generic metadata and status extensions should return copies.</summary>
    [Theory]
    [InlineData("original", "updated")]
    [InlineData("first", "second")]
    public void NonGeneric_Metadata_And_Status_Extensions_Should_Return_Copies(
        string originalValue,
        string updatedValue)
    {
        // Arrange
        var original = Result.Ok()
            .WithMetadata("source", originalValue);

        // Act
        var copy = original
            .WithMetadata("source", updatedValue)
            .WithStatus(ResultConstant.StatusCode.Accepted);

        // Assert
        copy.ShouldNotBeSameAs(original);
        copy.Metadata["source"].ShouldBe(updatedValue);
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
        original.Metadata["source"].ShouldBe(originalValue);
        original.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    /// <summary>Generic metadata and status extensions should return copies.</summary>
    [Theory]
    [InlineData(5)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Generic_Metadata_And_Status_Extensions_Should_Return_Copies(int value)
    {
        // Arrange
        var original = Result<int>.Ok(value)
            .WithMetadata("source", "original");

        // Act
        var copy = original
            .WithMetadata("source", "updated")
            .WithStatus(ResultConstant.StatusCode.Accepted);

        // Assert
        copy.ShouldNotBeSameAs(original);
        copy.Metadata["source"].ShouldBe("updated");
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
        copy.Value.ShouldBe(value);
        original.Metadata["source"].ShouldBe("original");
        original.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    #endregion
}