using SharedKernel.Concerns.Content.Taggable;

namespace SharedKernel.UnitTests.Concerns.Content.Taggable;

public class TaggableExtensionSpec
{
    private sealed class TestTaggable : ITaggable
    {
        public ICollection<string> Tags { get; } = new List<string>();
    }

    private static Result<TestTaggable, Error> Success(
        TestTaggable entity)
    {
        return Result<TestTaggable, Error>.Success(entity);
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
        result.Errors![0].Code.ShouldBe("taggable.tag.duplicate");
        entity.Tags.ShouldHaveSingleItem();
    }

    [Fact]
    public void AddTag_NullTag_ShouldFailWithoutMutation()
    {
        var entity = new TestTaggable();

        var result = Success(entity).AddTag(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("taggable.tag.required");
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
        result.Errors![0].Code.ShouldBe("taggable.tag.too_long");
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
        result.Errors![0].Code.ShouldBe("taggable.tag.not_found");
        entity.Tags.ShouldHaveSingleItem().ShouldBe("sale");
    }
}
