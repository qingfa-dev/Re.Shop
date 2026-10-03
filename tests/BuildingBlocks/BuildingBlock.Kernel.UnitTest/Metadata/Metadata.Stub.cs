using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.UnitTest.Metadata;

// Create: Minimal IMetadata double for extension-method specs.
public sealed class MetadataStub : IMetadata
{
    public MetadataDictionary Metadata { get; init; } = MetadataDictionary.Create();
}

// Guard: Null container reaches the mutation branches that guard on `is IDictionary`.
public sealed class NullMetadataStub : IMetadata
{
    public MetadataDictionary Metadata => null!;
}

// Create: Opaque type with no converter — forces TypeConverter-failure paths.
public sealed class StubOpaque
{
    public override string? ToString() => null;
}
