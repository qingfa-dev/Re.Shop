using SharedKernel.Concerns.Foundation.Tenantable;

namespace SharedKernel.UnitTests.Concerns.Foundation.Tenantable;

public class TenantableValidatorSpec
{
    private sealed class TestTenantable : ITenantable<Guid>
    {
        public Guid TenantId { get; set; }
    }

    [Fact]
    public void ValidateTenancy_ValidInput_ShouldSucceed()
    {
        var tenantId = Guid.NewGuid();
        var entity = new TestTenantable { TenantId = tenantId };

        var result = TenantableValidator.ValidateTenancy<
            TestTenantable,
            Guid>(entity);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.TenantId.ShouldBe(tenantId);
    }

    [Fact]
    public void ValidateTenancy_NullEntity_ShouldFailEntityRequired()
    {
        var result = TenantableValidator.ValidateTenancy<TestTenantable, Guid>(
            null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("tenantable.entity.required");
    }

    [Fact]
    public void ValidateTenancy_DefaultTenantId_ShouldFailTenantIdRequired()
    {
        var entity = new TestTenantable();

        var result = TenantableValidator.ValidateTenancy<
            TestTenantable,
            Guid>(entity);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("tenantable.tenant_id.required");
        entity.TenantId.ShouldBe(Guid.Empty);
    }
}
