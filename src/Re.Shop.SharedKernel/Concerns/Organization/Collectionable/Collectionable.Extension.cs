namespace SharedKernel.Concerns.Organization.Collectionable;

public static class CollectionableExtensions
{
    /// <summary>
    /// Adds a collection to the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> AddCollection<TValue, TCollection>(
        this Result<TValue, Error> result,
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
    public static Result<TValue, Error> RemoveCollection<TValue, TCollection>(
        this Result<TValue, Error> result,
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
