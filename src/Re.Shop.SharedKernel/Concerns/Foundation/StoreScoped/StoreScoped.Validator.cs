namespace SharedKernel.Concerns.Foundation.StoreScoped;

public static class StoreScopedValidator
{
    /// <summary>
    /// Validates that the entity carries a non-default store identifier.
    /// </summary>
    public static Result<TValue, Error> ValidateStoreScope<TValue, TStoreKey>(
        TValue auditable)
        where TValue : IStoreScoped<TStoreKey>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                StoreScopedResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    !EqualityComparer<TStoreKey>.Default.Equals(
                        entity.StoreId,
                        default!),
                error: StoreScopedResult.Failure.StoreIdRequired);
    }
}
