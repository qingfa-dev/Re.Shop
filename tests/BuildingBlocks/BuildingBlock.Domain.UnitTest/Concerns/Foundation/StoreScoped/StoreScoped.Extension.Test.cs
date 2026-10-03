using BuildingBlocks.Domain.Concerns.Foundation.StoreScoped;
namespace BuildingBlocks.Domain.Tests.Concerns.Foundation.StoreScoped;

public class StoreScopedExtensionTest
{
    private sealed class TestStoreScoped : IStoreScoped<Guid>
    {
        public Guid StoreId { get; set; }
    }

    private static Result<TestStoreScoped> Success(TestStoreScoped entity)
    {
        return Result<TestStoreScoped>.Success(entity);
    }

    [Fact]
    public void EnsureStoreScoped_WithStoreId_ShouldSucceed()
    {
        var entity = new TestStoreScoped { StoreId = Guid.NewGuid() };

        var result = Success(entity).EnsureStoreScoped<TestStoreScoped, Guid>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
    }

    [Fact]
    public void EnsureStoreScoped_DefaultStoreId_ShouldFailStoreIdRequired()
    {
        var entity = new TestStoreScoped();

        var result = Success(entity).EnsureStoreScoped<TestStoreScoped, Guid>();

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("StoreScoped.StoreId.Required");
    }

    [Fact]
    public void ValidateStoreScope_NullEntity_ShouldFailEntityRequired()
    {
        var result = StoreScopedValidator.ValidateStoreScope<
            TestStoreScoped,
            Guid>(null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("StoreScoped.Entity.Required");
    }
}
