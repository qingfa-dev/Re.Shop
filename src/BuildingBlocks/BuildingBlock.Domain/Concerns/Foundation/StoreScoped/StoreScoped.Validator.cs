namespace BuildingBlocks.Domain.Concerns.Foundation.StoreScoped;

public static class StoreScopedValidator
{
    /// <summary>
    /// Validates that the entity carries a non-default store identifier.
    /// </summary>
    public static Result<TValue> ValidateStoreScope<TValue, TStoreKey>(
        TValue auditable)
        where TValue : IStoreScoped<TStoreKey>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                StoreScopedResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    !EqualityComparer<TStoreKey>.Default.Equals(
                        entity.StoreId,
                        default!),
                errorValue: StoreScopedResult.Failure.StoreIdRequired);
    }
}
