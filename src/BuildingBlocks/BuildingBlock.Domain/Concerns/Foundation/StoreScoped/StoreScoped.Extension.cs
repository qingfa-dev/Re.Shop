namespace BuildingBlocks.Domain.Concerns.Foundation.StoreScoped;

public static class StoreScopedExtensions
{
    /// <summary>
    /// Fails when the entity's store identifier is missing (default value).
    /// </summary>
    public static Result<TValue> EnsureStoreScoped<TValue, TStoreKey>(
        this Result<TValue> result)
        where TValue : IStoreScoped<TStoreKey>
    {
        return result.Bind(entity =>
            StoreScopedValidator.ValidateStoreScope<TValue, TStoreKey>(entity));
    }
}
