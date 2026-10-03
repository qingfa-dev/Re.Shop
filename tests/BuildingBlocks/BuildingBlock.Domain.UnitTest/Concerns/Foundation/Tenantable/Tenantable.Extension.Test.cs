using BuildingBlocks.Domain.Concerns.Foundation.Tenantable;
namespace BuildingBlocks.Domain.Tests.Concerns.Foundation.Tenantable;

public class TenantableExtensionTest
{
    private sealed class TestTenantable : ITenantable<Guid>
    {
        public Guid TenantId { get; set; }
    }

    private static Result<TestTenantable> Success(TestTenantable entity)
    {
        return Result<TestTenantable>.Success(entity);
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
        result.Errors![0].Code.ShouldBe("Tenantable.TenantId.Required");
    }

    [Fact]
    public void ValidateTenancy_NullEntity_ShouldFailEntityRequired()
    {
        var result = TenantableValidator.ValidateTenancy<TestTenantable, Guid>(
            null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Tenantable.Entity.Required");
    }
}
