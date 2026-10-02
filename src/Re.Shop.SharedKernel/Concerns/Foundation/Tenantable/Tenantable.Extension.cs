namespace SharedKernel.Concerns.Foundation.Tenantable;

public static class TenantableExtensions
{
    /// <summary>
    /// Fails when the entity's tenant identifier is missing (default value).
    /// </summary>
    public static Result<TValue, Error> EnsureTenanted<TValue, TTenantKey>(
        this Result<TValue, Error> result)
        where TValue : ITenantable<TTenantKey>
    {
        return result.Ensure(entity =>
            TenantableValidator.ValidateTenancy<TValue, TTenantKey>(entity));
    }
}
