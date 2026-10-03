namespace BuildingBlock.Kernel.Metadata;

#region IMetadataDictionary

/// <summary>Read-only dictionary contract for metadata bags.</summary>
/// <remarks>
/// Keys are case-insensitive per <see cref="StringComparer.OrdinalIgnoreCase"/>.
/// Implementations must guarantee that <c>ContainsKey("X")</c> matches <c>ContainsKey("x")</c>.
/// </remarks>
public interface IMetadataDictionary : IReadOnlyDictionary<string, object>
{
}

#endregion