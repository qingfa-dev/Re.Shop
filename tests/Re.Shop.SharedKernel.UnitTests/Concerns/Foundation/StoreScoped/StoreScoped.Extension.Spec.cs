using SharedKernel.Concerns.Foundation.StoreScoped;

namespace SharedKernel.UnitTests.Concerns.Foundation.StoreScoped;

public class StoreScopedExtensionSpec
{
    private sealed class TestStoreScoped : IStoreScoped<Guid>
    {
        public Guid StoreId { get; set; }
    }

    private static Result<TestStoreScoped, Error> Success(TestStoreScoped entity)
    {
        return Result<TestStoreScoped, Error>.Success(entity);
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
        result.Errors![0].Code.ShouldBe("store_scoped.store_id.required");
    }
}
