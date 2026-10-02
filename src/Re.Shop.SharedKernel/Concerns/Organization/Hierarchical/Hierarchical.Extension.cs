namespace SharedKernel.Concerns.Organization.Hierarchical;

public static class HierarchicalExtensions
{
    /// <summary>
    /// Sets the entity's parent to the given id.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> SetParent<TValue, TKey>(
        this Result<TValue, Error> result,
        TKey parentId)
        where TValue : IHierarchical<TKey>
        where TKey : struct
    {
        return result.SetParent<TValue, TKey>((TKey?)parentId);
    }

    /// <summary>
    /// Sets the entity's parent; pass <c>null</c> to make it a root.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> SetParent<TValue, TKey>(
        this Result<TValue, Error> result,
        TKey? parentId)
        where TValue : IHierarchical<TKey>
        where TKey : struct
    {
        return result
            .Bind(entity =>
                HierarchicalValidator.ValidateParent<TValue, TKey>(
                    entity,
                    parentId))

            .Tap(entity =>
            {
                entity.ParentId = parentId;
            });
    }
}
