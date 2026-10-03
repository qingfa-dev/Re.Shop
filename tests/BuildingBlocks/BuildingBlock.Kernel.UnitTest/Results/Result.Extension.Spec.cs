using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for <see cref="ResultExtension"/> metadata and status extensions.</summary>
[Trait("Category", "Unit")]
public class ResultExtensionSpec
{
    #region Metadata and Status

    /// <summary>Non-generic metadata and status extensions should return copies.</summary>
    [Fact]
    public void NonGeneric_Metadata_And_Status_Extensions_Should_Return_Copies()
    {
        // Arrange
        var original = Result.Ok()
            .WithMetadata("source", "original");

        // Act
        var copy = original
            .WithMetadata("source", "updated")
            .WithStatus(ResultConstant.StatusCode.Accepted);

        // Assert
        copy.ShouldNotBeSameAs(original);
        copy.Metadata["source"].ShouldBe("updated");
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
        original.Metadata["source"].ShouldBe("original");
        original.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    /// <summary>Generic metadata and status extensions should return copies.</summary>
    [Fact]
    public void Generic_Metadata_And_Status_Extensions_Should_Return_Copies()
    {
        // Arrange
        var original = Result<int>.Ok(5)
            .WithMetadata("source", "original");

        // Act
        var copy = original
            .WithMetadata("source", "updated")
            .WithStatus(ResultConstant.StatusCode.Accepted);

        // Assert
        copy.ShouldNotBeSameAs(original);
        copy.Metadata["source"].ShouldBe("updated");
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
        copy.Value.ShouldBe(5);
        original.Metadata["source"].ShouldBe("original");
        original.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    #endregion
}