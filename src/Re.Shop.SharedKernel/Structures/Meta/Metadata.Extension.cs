using System.Diagnostics.CodeAnalysis;

namespace SharedKernel.Structures.Meta;

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
        if (source is null)
        {
            return default;
        }

        MetadataGuard.ValidateKey(key);
        MetadataGuard.ValidateValue(value);

        source.Metadata ??= new Dictionary<string, object?>();
        source.Metadata[key] = value;
        return source;
    }

    // Fluent:
    [return: NotNullIfNotNull(nameof(source))]
    public static T? WithMetadata<T>(this T? source, string? key, object? value)
        where T : IMetadata
    {
        return source.SetMetadata(key, value);
    }

    [return: NotNullIfNotNull(nameof(source))]
    public static T? WithAdditionalMetadata<T>(
        this T? source,
        Dictionary<string, object?>? additionalMetadata = null)
        where T : IMetadata
    {
        if (source is null || additionalMetadata is null || additionalMetadata.Count == 0)
        {
            return source;
        }

        foreach (var entry in additionalMetadata)
        {
            source = source.SetMetadata(entry.Key, entry.Value);
        }

        return source;
    }

    public static T? WithType<T>(this T? source, string? type)
        where T : IMetadata
    {
        return source.WithMetadata(MetadataConstant.MetadataKey.Type, type);
    }

    public static T? WithTarget<T>(this T? source, string? target)
        where T : IMetadata
    {
        return source.WithMetadata(MetadataConstant.MetadataKey.Target, target);
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
