namespace SharedKernel.Concerns.Foundation.StoreScoped;

public static class StoreScopedExtensions
{
    /// <summary>
    /// Fails when the entity's store identifier is missing (default value).
    /// </summary>
    public static Result<TValue, Error> EnsureStoreScoped<TValue, TStoreKey>(
        this Result<TValue, Error> result)
        where TValue : IStoreScoped<TStoreKey>
    {
        return result.Ensure(entity =>
            StoreScopedValidator.ValidateStoreScope<TValue, TStoreKey>(entity));
    }
}
