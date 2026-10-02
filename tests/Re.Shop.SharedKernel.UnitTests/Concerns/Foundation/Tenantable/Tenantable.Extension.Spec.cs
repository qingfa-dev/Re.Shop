using SharedKernel.Concerns.Foundation.Tenantable;

namespace SharedKernel.UnitTests.Concerns.Foundation.Tenantable;

public class TenantableExtensionSpec
{
    private sealed class TestTenantable : ITenantable<Guid>
    {
        public Guid TenantId { get; set; }
    }

    private static Result<TestTenantable, Error> Success(TestTenantable entity)
    {
        return Result<TestTenantable, Error>.Success(entity);
    }

    [Fact]
    public void EnsureTenanted_WithTenantId_ShouldSucceed()
    {
        var entity = new TestTenantable { TenantId = Guid.NewGuid() };

        var result = Success(entity).EnsureTenanted<TestTenantable, Guid>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
    }

    [Fact]
    public void EnsureTenanted_DefaultTenantId_ShouldFailTenantIdRequired()
    {
        var entity = new TestTenantable();

        var result = Success(entity).EnsureTenanted<TestTenantable, Guid>();

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("tenantable.tenant_id.required");
    }
}
