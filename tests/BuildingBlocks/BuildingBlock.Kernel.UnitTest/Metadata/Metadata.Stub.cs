using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.UnitTest.Metadata;

/// <summary>Stubs used across metadata extension-method specs.</summary>
/// <remarks>
/// <list type="bullet">
/// <item><see cref="MetadataStub"/> — minimal <see cref="IMetadata"/> double with a mutable dictionary.</item>
/// <item><see cref="NullMetadataStub"/> — null <see cref="IMetadata.Metadata"/> to exercise mutation guards.</item>
/// <item><see cref="StubOpaque"/> — opaque type with no converter, forcing TypeConverter-failure paths.</item>
/// </list>
/// </remarks>
#region Stubs

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

#endregion