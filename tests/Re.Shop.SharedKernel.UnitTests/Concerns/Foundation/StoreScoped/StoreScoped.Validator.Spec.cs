using SharedKernel.Concerns.Foundation.StoreScoped;

namespace SharedKernel.UnitTests.Concerns.Foundation.StoreScoped;

public class StoreScopedValidatorSpec
{
    private sealed class TestStoreScoped : IStoreScoped<Guid>
    {
        public Guid StoreId { get; set; }
    }

    [Fact]
    public void ValidateStoreScope_ValidInput_ShouldSucceed()
    {
        var storeId = Guid.NewGuid();
        var entity = new TestStoreScoped { StoreId = storeId };

        var result = StoreScopedValidator.ValidateStoreScope<
            TestStoreScoped,
            Guid>(entity);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.StoreId.ShouldBe(storeId);
    }

    [Fact]
    public void ValidateStoreScope_NullEntity_ShouldFailEntityRequired()
    {
        var result = StoreScopedValidator.ValidateStoreScope<
            TestStoreScoped,
            Guid>(null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("store_scoped.entity.required");
    }

    [Fact]
    public void ValidateStoreScope_DefaultStoreId_ShouldFailStoreIdRequired()
    {
        var entity = new TestStoreScoped();

        var result = StoreScopedValidator.ValidateStoreScope<
            TestStoreScoped,
            Guid>(entity);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("store_scoped.store_id.required");
        entity.StoreId.ShouldBe(Guid.Empty);
    }
}
