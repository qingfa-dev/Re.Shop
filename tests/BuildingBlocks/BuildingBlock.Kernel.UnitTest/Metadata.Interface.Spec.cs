using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class MetadataInterfaceSpec
{
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
}
