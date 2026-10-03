using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.UnitTest.Metadata;

/// <summary>Tests for the <see cref="IMetadata"/> interface hierarchy.</summary>
/// <remarks>
/// Verifies that <see cref="IMetadataDictionary"/> extends <see cref="IReadOnlyDictionary{TKey,TValue}"/>,
/// <see cref="IMetadata"/> extends <see cref="IMetadata{TDictionary}"/>, and
/// <see cref="MetadataDictionary"/> implements <see cref="IMetadataDictionary"/>.
/// </remarks>
[Trait("Category", "Unit")]
public class MetadataInterfaceSpec
{
    #region Interface hierarchy

    [Fact]
    public void Metadata_Dictionary_Should_Extend_ReadOnly_Dictionary()
    {
        // Arrange
        // Act
        // Assert
        typeof(IReadOnlyDictionary<string, object>).IsAssignableFrom(typeof(IMetadataDictionary)).ShouldBeTrue();
    }

    [Fact]
    public void Metadata_Should_Extend_Metadata_Generic()
    {
        // Arrange
        // Act
        // Assert
        typeof(IMetadata<MetadataDictionary>).IsAssignableFrom(typeof(IMetadata)).ShouldBeTrue();
    }

    [Fact]
    public void Concrete_Metadata_Dictionary_Should_Implement_Contract()
    {
        // Arrange
        // Act
        // Assert
        typeof(IMetadataDictionary).IsAssignableFrom(typeof(MetadataDictionary)).ShouldBeTrue();
    }

    #endregion
}