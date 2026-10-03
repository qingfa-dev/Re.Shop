namespace BuildingBlocks.Domain.Concerns.Content.Metafieldable;

public static class MetafieldableExtensions
{
    /// <summary>
    /// Sets the metafield for a (namespace, key) pair, adding it when new.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> SetMetafield<TValue>(
        this Result<TValue> result,
        string? @namespace,
        string? key,
        MetafieldType type,
        string? value)
        where TValue : IMetafieldable
    {
        return result
            .Bind(entity =>
                MetafieldableValidator.ValidateMetafield(
                    entity,
                    @namespace,
                    key))

            .Tap(entity =>
            {
                var existing = entity.Metafields.FirstOrDefault(metafield =>
                    metafield.Namespace == @namespace &&
                    metafield.Key == key);

                if (existing is null)
                {
                    entity.Metafields.Add(new Metafield
                    {
                        Namespace = @namespace!,
                        Key = key!,
                        Type = type,
                        Value = value
                    });
                }
                else
                {
                    existing.Type = type;
                    existing.Value = value;
                }
            });
    }

    /// <summary>
    /// Removes the metafield for a (namespace, key) pair.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> RemoveMetafield<TValue>(
        this Result<TValue> result,
        string? @namespace,
        string? key)
        where TValue : IMetafieldable
    {
        return result
            .Bind(entity =>
                MetafieldableValidator.ValidateMetafieldRemoval(
                    entity,
                    @namespace,
                    key))

            .Tap(entity =>
            {
                var existing = entity.Metafields.First(metafield =>
                    metafield.Namespace == @namespace &&
                    metafield.Key == key);

                entity.Metafields.Remove(existing);
            });
    }
}
