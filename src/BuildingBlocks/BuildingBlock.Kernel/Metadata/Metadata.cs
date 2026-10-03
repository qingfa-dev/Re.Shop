namespace BuildingBlock.Kernel.Metadata;

/// <summary>Shared base for every type that carries a metadata bag.</summary>
/// <remarks>
/// Each instance gets its own fresh dictionary unless one is supplied via the <see cref="Metadata"/> setter.
/// The default dictionary is mutable; derived types may override initialization to supply a pre-populated bag.
/// </remarks>
public abstract record MetadataBase : IMetadata
{
    #region Metadata

    /// <summary>Gets the metadata dictionary for this instance.</summary>
    /// <value>A fresh <see cref="MetadataDictionary"/> by default; mutable unless replaced.</value>
    /// <remarks>
    /// Use <see cref="MetadataDictionary.Create()"/> when you need a mutable copy,
    /// or assign a custom dictionary at initialization time.
    /// The <see cref="MetadataDictionary.Empty"/> singleton is read-only by convention — do not cast and mutate it.
    /// </remarks>
    public MetadataDictionary Metadata { get; init; } = new MetadataDictionary();

    #endregion
}