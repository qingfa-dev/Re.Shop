using SharedKernel.Concerns.Content.Sluggable;

namespace SharedKernel.UnitTests.Concerns.Content.Sluggable;

public class SluggableValidatorSpec
{
    private sealed class TestSluggable : ISluggable
    {
        public string Slug { get; set; } = string.Empty;
    }

    [Fact]
    public void ValidateSlug_ValidInput_ShouldSucceed()
    {
        var entity = new TestSluggable { Slug = "keep-me" };

        var result = SluggableValidator.ValidateSlug(
            entity,
            "hello-world");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Slug.ShouldBe("keep-me");
    }

    [Fact]
    public void ValidateSlug_NullEntity_ShouldFailEntityRequired()
    {
        var result = SluggableValidator.ValidateSlug(
            (TestSluggable)null!,
            "hello-world");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.entity.required");
    }

    [Fact]
    public void ValidateSlug_NullSlug_ShouldFailSlugRequired()
    {
        var entity = new TestSluggable { Slug = "keep-me" };

        var result = SluggableValidator.ValidateSlug(
            entity,
            null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.slug.required");
        entity.Slug.ShouldBe("keep-me");
    }

    [Fact]
    public void ValidateSlug_WhitespaceSlug_ShouldFailSlugRequired()
    {
        var entity = new TestSluggable { Slug = "keep-me" };

        var result = SluggableValidator.ValidateSlug(
            entity,
            "   ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.slug.required");
        entity.Slug.ShouldBe("keep-me");
    }

    [Fact]
    public void ValidateSlug_TooLongSlug_ShouldFailSlugTooLong()
    {
        var entity = new TestSluggable { Slug = "keep-me" };
        var slug = new string(
            's',
            SluggableConstant.Constraints.MaxSlugLength + 1);

        var result = SluggableValidator.ValidateSlug(
            entity,
            slug);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.slug.too_long");
        entity.Slug.ShouldBe("keep-me");
    }

    [Fact]
    public void ValidateSlug_InvalidSlug_ShouldFailSlugInvalid()
    {
        var entity = new TestSluggable { Slug = "keep-me" };

        var result = SluggableValidator.ValidateSlug(
            entity,
            "Hello-World");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.slug.invalid");
        entity.Slug.ShouldBe("keep-me");
    }
}
