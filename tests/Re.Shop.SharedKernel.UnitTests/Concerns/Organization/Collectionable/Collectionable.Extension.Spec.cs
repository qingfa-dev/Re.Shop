using SharedKernel.Concerns.Organization.Collectionable;

namespace SharedKernel.UnitTests.Concerns.Organization.Collectionable;

public class CollectionableExtensionSpec
{
    private sealed class TestCollectionable : ICollectionable<string>
    {
        public ICollection<string> Collections { get; } = new List<string>();
    }

    private static Result<TestCollectionable, Error> Success(
        TestCollectionable entity)
    {
        return Result<TestCollectionable, Error>.Success(entity);
    }

    [Fact]
    public void AddCollection_NewCollection_ShouldAddCollection()
    {
        var entity = new TestCollectionable();

        var result = Success(entity).AddCollection("summer-2026");

        result.IsSuccess.ShouldBeTrue();
        entity.Collections.ShouldHaveSingleItem()
            .ShouldBe("summer-2026");
    }

    [Fact]
    public void AddCollection_DuplicateCollection_ShouldFailWithoutMutation()
    {
        var entity = new TestCollectionable();
        Success(entity).AddCollection("summer-2026");

        var result = Success(entity).AddCollection("summer-2026");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("collectionable.collection.duplicate");
        entity.Collections.ShouldHaveSingleItem();
    }

    [Fact]
    public void RemoveCollection_AssignedCollection_ShouldRemoveCollection()
    {
        var entity = new TestCollectionable();
        Success(entity).AddCollection("summer-2026");

        var result = Success(entity).RemoveCollection("summer-2026");

        result.IsSuccess.ShouldBeTrue();
        entity.Collections.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveCollection_MissingCollection_ShouldFailWithoutMutation()
    {
        var entity = new TestCollectionable();
        Success(entity).AddCollection("summer-2026");

        var result = Success(entity).RemoveCollection("winter-2026");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("collectionable.collection.not_found");
        entity.Collections.ShouldHaveSingleItem();
    }
}
