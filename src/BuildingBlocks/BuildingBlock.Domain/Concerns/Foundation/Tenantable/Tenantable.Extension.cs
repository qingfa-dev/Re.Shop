namespace BuildingBlocks.Domain.Concerns.Foundation.Tenantable;

public static class TenantableExtensions
{
    /// <summary>
    /// Fails when the entity's tenant identifier is missing (default value).
    /// </summary>
    public static Result<TValue> EnsureTenanted<TValue, TTenantKey>(
        this Result<TValue> result)
        where TValue : ITenantable<TTenantKey>
    {
        return result.Bind(entity =>
            TenantableValidator.ValidateTenancy<TValue, TTenantKey>(entity));
    }
}
