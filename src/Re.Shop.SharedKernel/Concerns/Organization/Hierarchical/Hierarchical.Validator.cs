namespace SharedKernel.Concerns.Organization.Hierarchical;

public static class HierarchicalValidator
{
    /// <summary>
    /// Validates the parent assignment that <c>SetParent</c>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateParent<TValue, TKey>(
        TValue auditable,
        TKey? parentId)
        where TValue : IHierarchical<TKey>
        where TKey : struct
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                HierarchicalResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable);
    }
}
