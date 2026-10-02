using System.Diagnostics.CodeAnalysis;

namespace SharedKernel.Structures.Meta;

public static class MetadataExtension
{
    // ───────────────────────────── GET ─────────────────────────────

    public static object? GetMetadata<T>(this T? source, string? key)
        where T : IMetadata
    {
        return source is null
            || source.Metadata is null
            || string.IsNullOrWhiteSpace(key)
            ? null
            : source.Metadata.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>Strongly-typed getter.</summary>
    public static TValue? GetMetadata<T, TValue>(this T? source, string? key)
        where T : IMetadata
        => source.GetMetadata(key) is TValue typed ? typed : default;

    /// <summary>Non-throwing lookup with out parameter.</summary>
    public static bool TryGetMetadata<T>(this T? source, string? key, out object? value)
        where T : IMetadata
    {
        value = null;
        return source is not null
            && source.Metadata is not null
            && !string.IsNullOrWhiteSpace(key)
            && source.Metadata.TryGetValue(key, out value);
    }

    public static bool HasMetadata<T>(this T? source, string? key)
        where T : IMetadata
        => source is not null
        && source.Metadata is not null
        && !string.IsNullOrWhiteSpace(key)
        && source.Metadata.ContainsKey(key);

    /// <summary>Convenience string accessor (the "value" form).</summary>
    public static string? GetMetadataValue<T>(this T? source, string? key)
        where T : IMetadata
        => source.GetMetadata(key)?.ToString();

    // ───────────────────────────── SET ─────────────────────────────

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
        source.Metadata[key!] = value;
        return source;
    }

    public static T? RemoveMetadata<T>(this T? source, string? key)
        where T : IMetadata
    {
        if (source?.Metadata is null || string.IsNullOrWhiteSpace(key))
        {
            return source;
        }

        source.Metadata.Remove(key);
        return source;
    }

    public static T? ClearMetadata<T>(this T? source)
        where T : IMetadata
    {
        source?.Metadata?.Clear();
        return source;
    }

    // ─────────────────────────── FLUENT ────────────────────────────

    [return: NotNullIfNotNull(nameof(source))]
    public static T? WithMetadata<T>(this T? source, string? key, object? value)
        where T : IMetadata
        => source.SetMetadata(key, value);

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
        => source.WithMetadata(MetadataConstant.MetadataKey.Type, type);

    public static T? WithTarget<T>(this T? source, string? target)
        where T : IMetadata
        => source.WithMetadata(MetadataConstant.MetadataKey.Target, target);

    // ─────────────────────────── MERGE ─────────────────────────────

    public static TDestination? MergeMetadata<TSource, TDestination>(
        this TDestination? destination,
        TSource? source,
        bool overwrite = false)
        where TSource : IMetadata
        where TDestination : IMetadata
    {
        if (destination is null || source?.Metadata is null)
        {
            return destination; 
        }

        destination.Metadata ??= new Dictionary<string, object?>();

        foreach (var entry in source.Metadata)
        {
            if (overwrite)
            {
                destination.Metadata[entry.Key] = entry.Value;
            }
            else
            {
                destination.Metadata.TryAdd(entry.Key, entry.Value);
            }
        }

        return destination;
    }
}
