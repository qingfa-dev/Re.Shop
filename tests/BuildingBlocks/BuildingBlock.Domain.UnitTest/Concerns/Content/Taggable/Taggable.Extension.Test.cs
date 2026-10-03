using BuildingBlocks.Domain.Concerns.Content.Taggable;
namespace BuildingBlocks.Domain.Tests.Concerns.Content.Taggable;

public class TaggableExtensionTest
{
    private sealed class TestTaggable : ITaggable
    {
        public ICollection<string> Tags { get; } = new List<string>();
    }

    private static Result<TestTaggable> Success(
        TestTaggable entity)
    {
        return Result<TestTaggable>.Success(entity);
    }

    [Fact]
    public void AddTag_NewTag_ShouldAddTag()
    {
        var entity = new TestTaggable();

        var result = Success(entity).AddTag("sale");

        result.IsSuccess.ShouldBeTrue();
        entity.Tags.ShouldHaveSingleItem().ShouldBe("sale");
    }

    [Fact]
    public void AddTag_DuplicateTag_ShouldFailWithoutMutation()
    {
        var entity = new TestTaggable();
        Success(entity).AddTag("sale");

        var result = Success(entity).AddTag("sale");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Taggable.Tag.Duplicate");
        entity.Tags.ShouldHaveSingleItem();
    }

    [Fact]
    public void AddTag_NullTag_ShouldFailWithoutMutation()
    {
        var entity = new TestTaggable();

        var result = Success(entity).AddTag(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Taggable.Tag.Required");
        entity.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void AddTag_TooLongTag_ShouldFailWithoutMutation()
    {
        var entity = new TestTaggable();
        var tag = new string(
            't',
            TaggableConstant.Constraints.MaxTagLength + 1);

        var result = Success(entity).AddTag(tag);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Taggable.Tag.TooLong");
        entity.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveTag_ExistingTag_ShouldRemoveTag()
    {
        var entity = new TestTaggable();
        Success(entity).AddTag("sale");

        var result = Success(entity).RemoveTag("sale");

        result.IsSuccess.ShouldBeTrue();
        entity.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveTag_MissingTag_ShouldFailWithoutMutation()
    {
        var entity = new TestTaggable();
        Success(entity).AddTag("sale");

        var result = Success(entity).RemoveTag("clearance");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Taggable.Tag.NotFound");
        entity.Tags.ShouldHaveSingleItem().ShouldBe("sale");
    }

    [Fact]
    public void ValidateAddTag_NullEntity_ShouldFailEntityRequired()
    {
        var result = TaggableValidator.ValidateAddTag(
            (TestTaggable)null!,
            "sale");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Taggable.Entity.Required");
    }
}
