namespace BuildingBlock.Kernel.Metadata;

/// <summary>
/// Shared base for every type that carries a metadata bag.
/// Each instance gets its own fresh dictionary unless one is supplied.
/// </summary>
public abstract record MetadataBase : IMetadata
{
    public MetadataDictionary Metadata { get; init; } = new MetadataDictionary();
}