using SharedKernel.Concerns.Content.Taggable;

namespace SharedKernel.UnitTests.Concerns.Content.Taggable;

public class TaggableValidatorSpec
{
    private sealed class TestTaggable : ITaggable
    {
        public ICollection<string> Tags { get; } = new List<string>();
    }

    [Fact]
    public void ValidateAddTag_ValidInput_ShouldSucceed()
    {
        var entity = new TestTaggable();

        var result = TaggableValidator.ValidateAddTag(
            entity,
            "sale");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateAddTag_NullEntity_ShouldFailEntityRequired()
    {
        var result = TaggableValidator.ValidateAddTag(
            (TestTaggable)null!,
            "sale");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("taggable.entity.required");
    }

    [Fact]
    public void ValidateAddTag_NullTag_ShouldFailTagRequired()
    {
        var entity = new TestTaggable();

        var result = TaggableValidator.ValidateAddTag(
            entity,
            null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("taggable.tag.required");
        entity.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateAddTag_WhitespaceTag_ShouldFailTagRequired()
    {
        var entity = new TestTaggable();

        var result = TaggableValidator.ValidateAddTag(
            entity,
            "   ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("taggable.tag.required");
        entity.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateAddTag_TooLongTag_ShouldFailTagTooLong()
    {
        var entity = new TestTaggable();
        var tag = new string(
            't',
            TaggableConstant.Constraints.MaxTagLength + 1);

        var result = TaggableValidator.ValidateAddTag(
            entity,
            tag);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("taggable.tag.too_long");
        entity.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateAddTag_DuplicateTag_ShouldFailTagDuplicate()
    {
        var entity = new TestTaggable();
        entity.Tags.Add("sale");

        var result = TaggableValidator.ValidateAddTag(
            entity,
            "sale");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("taggable.tag.duplicate");
        entity.Tags.ShouldHaveSingleItem().ShouldBe("sale");
    }

    [Fact]
    public void ValidateRemoveTag_ValidInput_ShouldSucceed()
    {
        var entity = new TestTaggable();
        entity.Tags.Add("sale");

        var result = TaggableValidator.ValidateRemoveTag(
            entity,
            "sale");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Tags.ShouldHaveSingleItem().ShouldBe("sale");
    }

    [Fact]
    public void ValidateRemoveTag_NullEntity_ShouldFailEntityRequired()
    {
        var result = TaggableValidator.ValidateRemoveTag(
            (TestTaggable)null!,
            "sale");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("taggable.entity.required");
    }

    [Fact]
    public void ValidateRemoveTag_MissingTag_ShouldFailTagNotFound()
    {
        var entity = new TestTaggable();
        entity.Tags.Add("sale");

        var result = TaggableValidator.ValidateRemoveTag(
            entity,
            "clearance");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("taggable.tag.not_found");
        entity.Tags.ShouldHaveSingleItem().ShouldBe("sale");
    }
}
