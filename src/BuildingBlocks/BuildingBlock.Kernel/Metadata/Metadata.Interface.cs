namespace BuildingBlock.Kernel.Metadata;

#region IMetadata<TDictionary>

/// <summary>A type that carries a metadata dictionary of type <typeparamref name="TDictionary"/>.</summary>
/// <typeparam name="TDictionary">The type of metadata dictionary; must implement <see cref="IMetadataDictionary"/>.</typeparam>
/// <remarks>
/// Implementations provide a typed metadata bag accessible via the <see cref="Metadata"/> property.
/// The dictionary contract is case-insensitive per <see cref="StringComparer.OrdinalIgnoreCase"/>.
/// </remarks>
public interface IMetadata<TDictionary> where TDictionary : IMetadataDictionary
{
    /// <summary>Gets the metadata dictionary for this instance.</summary>
    /// <value>The typed metadata dictionary.</value>
    TDictionary Metadata { get; }
}

#endregion

#region IMetadata

/// <summary>A type that carries a <see cref="MetadataDictionary"/> metadata bag.</summary>
/// <remarks>
/// Convenience shorthand for <see cref="IMetadataDictionary"/>-typed metadata.
/// All kernel types that carry metadata implement this interface directly.
/// </remarks>
public interface IMetadata : IMetadata<MetadataDictionary>
{
}

#endregion