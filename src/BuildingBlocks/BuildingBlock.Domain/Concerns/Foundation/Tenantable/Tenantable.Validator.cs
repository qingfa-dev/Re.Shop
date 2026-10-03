namespace BuildingBlocks.Domain.Concerns.Foundation.Tenantable;

public static class TenantableValidator
{
    /// <summary>
    /// Validates that the entity carries a non-default tenant identifier.
    /// </summary>
    public static Result<TValue> ValidateTenancy<TValue, TTenantKey>(
        TValue auditable)
        where TValue : ITenantable<TTenantKey>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                TenantableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    !EqualityComparer<TTenantKey>.Default.Equals(
                        entity.TenantId,
                        default!),
                errorValue: TenantableResult.Failure.TenantIdRequired);
    }
}
