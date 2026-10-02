namespace SharedKernel.Concerns.Foundation.Tenantable;

public static class TenantableValidator
{
    /// <summary>
    /// Validates that the entity carries a non-default tenant identifier.
    /// </summary>
    public static Result<TValue, Error> ValidateTenancy<TValue, TTenantKey>(
        TValue auditable)
        where TValue : ITenantable<TTenantKey>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                TenantableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    !EqualityComparer<TTenantKey>.Default.Equals(
                        entity.TenantId,
                        default!),
                error: TenantableResult.Failure.TenantIdRequired);
    }
}
