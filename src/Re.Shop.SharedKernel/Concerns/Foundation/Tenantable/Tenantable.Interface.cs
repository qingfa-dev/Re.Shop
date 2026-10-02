namespace SharedKernel.Concerns.Foundation.Tenantable;

public interface ITenantable<TTenantKey>
{
    TTenantKey TenantId { get; set; }
}
