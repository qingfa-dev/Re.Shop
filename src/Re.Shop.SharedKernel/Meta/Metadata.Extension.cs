namespace SharedKernel.Meta;

public static class MetadataExtension
{
    // Get:
    public static object? GetMetadata<T>(this T? source, string? key)
    where T : IMetadata
    {
        return source is null || source.Metadata is null || string.IsNullOrWhiteSpace(key)
            ? null
            : source.Metadata.TryGetValue(key, out var value) ? value : null;
    }

    // Set:
    public static T? SetMetadata<T>(this T? source, string? key, object? value)
          where T : IMetadata
    {
        if (source is null || source.Metadata is null || string.IsNullOrWhiteSpace(key) || value is null)
        {
            return default;
        }

        source.Metadata[key] = value;
        return source;
    }

    // Merge:
    public static TDestination? MergeMetadata<TSource, TDestination>(
      this TDestination? destination,
      TSource? source)
      where TSource : IMetadata
      where TDestination : IMetadata
    {
        if (destination is null || destination.Metadata is null || source is null || source.Metadata is null)
        {
            return default;
        }

        foreach (var entry in source.Metadata)
        {
            destination.Metadata.TryAdd(entry.Key, entry.Value);
        }

        return destination;
    }
}
