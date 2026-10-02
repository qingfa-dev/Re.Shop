using SharedKernel.Concerns.Organization.Collectionable;

namespace SharedKernel.UnitTests.Concerns.Organization.Collectionable;

public class CollectionableValidatorSpec
{
    private sealed class TestCollectionable : ICollectionable<string>
    {
        public ICollection<string> Collections { get; } = new List<string>();
    }

    [Fact]
    public void ValidateAddCollection_ValidInput_ShouldSucceed()
    {
        var entity = new TestCollectionable();

        var result = CollectionableValidator.ValidateAddCollection(
            entity,
            "summer-2026");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Collections.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateAddCollection_NullEntity_ShouldFailEntityRequired()
    {
        var result = CollectionableValidator.ValidateAddCollection(
            (TestCollectionable)null!,
            "summer-2026");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("collectionable.entity.required");
    }

    [Fact]
    public void ValidateAddCollection_DuplicateCollection_ShouldFailCollectionDuplicate()
    {
        var entity = new TestCollectionable();
        entity.Collections.Add("summer-2026");

        var result = CollectionableValidator.ValidateAddCollection(
            entity,
            "summer-2026");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("collectionable.collection.duplicate");
        entity.Collections.ShouldHaveSingleItem().ShouldBe("summer-2026");
    }

    [Fact]
    public void ValidateRemoveCollection_ValidInput_ShouldSucceed()
    {
        var entity = new TestCollectionable();
        entity.Collections.Add("summer-2026");

        var result = CollectionableValidator.ValidateRemoveCollection(
            entity,
            "summer-2026");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Collections.ShouldHaveSingleItem().ShouldBe("summer-2026");
    }

    [Fact]
    public void ValidateRemoveCollection_NullEntity_ShouldFailEntityRequired()
    {
        var result = CollectionableValidator.ValidateRemoveCollection(
            (TestCollectionable)null!,
            "summer-2026");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("collectionable.entity.required");
    }

    [Fact]
    public void ValidateRemoveCollection_MissingCollection_ShouldFailCollectionNotFound()
    {
        var entity = new TestCollectionable();

        var result = CollectionableValidator.ValidateRemoveCollection(
            entity,
            "winter-2026");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("collectionable.collection.not_found");
        entity.Collections.ShouldBeEmpty();
    }
}
