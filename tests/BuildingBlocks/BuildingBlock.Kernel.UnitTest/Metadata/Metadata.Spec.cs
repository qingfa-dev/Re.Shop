using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.UnitTest.Metadata;

[Trait("Category", "Unit")]
public class MetadataBaseSpec
{
    private sealed record Thing : MetadataBase;

    [Fact]
    public void MetadataBase_Should_Implement_IMetadata()
    {
        // Arrange
        // Act
        // Assert
        typeof(IMetadata).IsAssignableFrom(typeof(MetadataBase)).ShouldBeTrue();
        typeof(MetadataBase).IsAbstract.ShouldBeTrue();
    }

    [Fact]
    public void MetadataBase_When_Derived_Should_Start_With_Fresh_Empty_Bag()
    {
        // Arrange
        // Act
        var first = new Thing();
        var second = new Thing();

        // Assert
        first.Metadata.ShouldNotBeNull();
        first.Metadata.ShouldBeEmpty();
        first.Metadata.ShouldNotBeSameAs(second.Metadata);
    }

    [Fact]
    public void MetadataBase_When_Metadata_Supplied_Should_Use_It()
    {
        // Arrange
        var metadata = MetadataDictionary.Create().With("trace.id", "abc");

        // Act
        var thing = new Thing { Metadata = metadata };

        // Assert
        thing.Metadata.ShouldBeSameAs(metadata);
        thing.GetCorrelationId().ShouldBeNull();
    }

    [Fact]
    public void MetadataBase_Extensions_Should_Work_On_Derived()
    {
        // Arrange
        var thing = new Thing();

        // Act
        thing.SetValue("CorrelationId", Guid.Parse("11111111-2222-3333-4444-555555555555"));

        // Assert
        thing.Contains("correlationid").ShouldBeTrue();
        thing.GetCorrelationId().ShouldBe(Guid.Parse("11111111-2222-3333-4444-555555555555"));
    }
}
