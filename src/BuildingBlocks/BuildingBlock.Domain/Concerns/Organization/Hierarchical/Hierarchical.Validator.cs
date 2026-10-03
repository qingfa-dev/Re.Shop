namespace BuildingBlocks.Domain.Concerns.Organization.Hierarchical;

public static class HierarchicalValidator
{
    /// <summary>
    /// Validates the parent assignment that <c>SetParent</c>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateParent<TValue, TKey>(
        TValue auditable,
        TKey? parentId)
        where TValue : IHierarchical<TKey>
        where TKey : struct
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                HierarchicalResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable);
    }
}
