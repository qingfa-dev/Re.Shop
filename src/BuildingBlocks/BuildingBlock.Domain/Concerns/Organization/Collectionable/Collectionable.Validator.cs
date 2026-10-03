namespace BuildingBlocks.Domain.Concerns.Organization.Collectionable;

public static class CollectionableValidator
{
    /// <summary>
    /// Validates the collection that
    /// <see cref="CollectionableExtensions.AddCollection{TValue, TCollection}"/>
    /// is about to add, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateAddCollection<
        TValue,
        TCollection>(
        TValue auditable,
        TCollection collection)
        where TValue : ICollectionable<TCollection>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                CollectionableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => !entity.Collections.Contains(collection),
                errorValue: CollectionableResult.Failure.CollectionDuplicate);
    }

    /// <summary>
    /// Validates that the collection that
    /// <see cref="CollectionableExtensions.RemoveCollection{TValue, TCollection}"/>
    /// targets is assigned, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateRemoveCollection<
        TValue,
        TCollection>(
        TValue auditable,
        TCollection collection)
        where TValue : ICollectionable<TCollection>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                CollectionableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => entity.Collections.Contains(collection),
                errorValue: CollectionableResult.Failure.CollectionNotFound);
    }
}
