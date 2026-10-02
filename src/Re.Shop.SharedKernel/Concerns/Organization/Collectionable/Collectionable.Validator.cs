namespace SharedKernel.Concerns.Organization.Collectionable;

public static class CollectionableValidator
{
    /// <summary>
    /// Validates the collection that
    /// <see cref="CollectionableExtensions.AddCollection{TValue, TCollection}"/>
    /// is about to add, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateAddCollection<
        TValue,
        TCollection>(
        TValue auditable,
        TCollection collection)
        where TValue : ICollectionable<TCollection>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                CollectionableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => !entity.Collections.Contains(collection),
                error: CollectionableResult.Failure.CollectionDuplicate);
    }

    /// <summary>
    /// Validates that the collection that
    /// <see cref="CollectionableExtensions.RemoveCollection{TValue, TCollection}"/>
    /// targets is assigned, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateRemoveCollection<
        TValue,
        TCollection>(
        TValue auditable,
        TCollection collection)
        where TValue : ICollectionable<TCollection>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                CollectionableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => entity.Collections.Contains(collection),
                error: CollectionableResult.Failure.CollectionNotFound);
    }
}
