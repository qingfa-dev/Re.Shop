namespace BuildingBlocks.Domain.Concerns.Organization.Collectionable;

public static class CollectionableExtensions
{
    /// <summary>
    /// Adds a collection to the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> AddCollection<TValue, TCollection>(
        this Result<TValue> result,
        TCollection collection)
        where TValue : ICollectionable<TCollection>
    {
        return result
            .Bind(entity =>
                CollectionableValidator.ValidateAddCollection(
                    entity,
                    collection))

            .Tap(entity =>
            {
                entity.Collections.Add(collection);
            });
    }

    /// <summary>
    /// Removes a collection from the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> RemoveCollection<TValue, TCollection>(
        this Result<TValue> result,
        TCollection collection)
        where TValue : ICollectionable<TCollection>
    {
        return result
            .Bind(entity =>
                CollectionableValidator.ValidateRemoveCollection(
                    entity,
                    collection))

            .Tap(entity =>
            {
                entity.Collections.Remove(collection);
            });
    }
}
