using BuildingBlocks.Domain.Concerns.Organization.Collectionable;
namespace BuildingBlocks.Domain.Tests.Concerns.Organization.Collectionable;

public class CollectionableExtensionTest
{
    private sealed class TestCollectionable : ICollectionable<string>
    {
        public ICollection<string> Collections { get; } = new List<string>();
    }

    private static Result<TestCollectionable> Success(
        TestCollectionable entity)
    {
        return Result<TestCollectionable>.Success(entity);
    }

    [Fact]
    public void AddCollection_NewCollection_ShouldAddCollection()
    {
        var entity = new TestCollectionable();

        var result = Success(entity).AddCollection("summer-2026");

        result.IsSuccess.ShouldBeTrue();
        entity.Collections.ShouldHaveSingleItem().ShouldBe("summer-2026");
    }

    [Fact]
    public void AddCollection_DuplicateCollection_ShouldFailWithoutMutation()
    {
        var entity = new TestCollectionable();
        Success(entity).AddCollection("summer-2026");

        var result = Success(entity).AddCollection("summer-2026");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe(
            "Collectionable.Collection.Duplicate");
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
        result.Errors![0].Code.ShouldBe(
            "Collectionable.Collection.NotFound");
        entity.Collections.ShouldHaveSingleItem();
    }

    [Fact]
    public void ValidateAddCollection_NullEntity_ShouldFailEntityRequired()
    {
        var result = CollectionableValidator.ValidateAddCollection(
            (TestCollectionable)null!,
            "summer-2026");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe(
            "Collectionable.Entity.Required");
    }
}
