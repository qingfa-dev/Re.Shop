namespace BuildingBlocks.Domain.Concerns.Foundation.Tenantable;

public interface ITenantable<TTenantKey>
{
    TTenantKey TenantId { get; set; }
}
