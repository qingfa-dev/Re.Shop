using SharedKernel.Concerns.Content.Sluggable;

namespace SharedKernel.UnitTests.Concerns.Content.Sluggable;

public class SluggableExtensionSpec
{
    private sealed class TestSluggable : ISluggable
    {
        public string Slug { get; set; } = string.Empty;
    }

    private static Result<TestSluggable, Error> Success(
        TestSluggable entity)
    {
        return Result<TestSluggable, Error>.Success(entity);
    }

    [Fact]
    public void SetSlug_ValidSlug_ShouldSetSlug()
    {
        var entity = new TestSluggable();

        var result = Success(entity).SetSlug("hello-world");

        result.IsSuccess.ShouldBeTrue();
        entity.Slug.ShouldBe("hello-world");
    }

    [Fact]
    public void SetSlug_NullSlug_ShouldFailWithoutMutation()
    {
        var entity = new TestSluggable { Slug = "keep-me" };

        var result = Success(entity).SetSlug(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.slug.required");
        entity.Slug.ShouldBe("keep-me");
    }

    [Fact]
    public void SetSlug_UppercaseSlug_ShouldFailWithoutMutation()
    {
        var entity = new TestSluggable { Slug = "keep-me" };

        var result = Success(entity).SetSlug("Hello-World");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.slug.invalid");
        entity.Slug.ShouldBe("keep-me");
    }

    [Fact]
    public void SetSlug_TooLongSlug_ShouldFailWithoutMutation()
    {
        var entity = new TestSluggable { Slug = "keep-me" };
        var slug = new string(
            's',
            SluggableConstant.Constraints.MaxSlugLength + 1);

        var result = Success(entity).SetSlug(slug);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.slug.too_long");
        entity.Slug.ShouldBe("keep-me");
    }

    [Fact]
    public void SetSlugFromText_PlainText_ShouldSlugify()
    {
        var entity = new TestSluggable();

        var result = Success(entity)
            .SetSlugFromText("Hello, World!");

        result.IsSuccess.ShouldBeTrue();
        entity.Slug.ShouldBe("hello-world");
    }

    [Fact]
    public void SetSlugFromText_Diacritics_ShouldFoldToAscii()
    {
        var entity = new TestSluggable();

        var result = Success(entity)
            .SetSlugFromText("Crème Brûlée");

        result.IsSuccess.ShouldBeTrue();
        entity.Slug.ShouldBe("creme-brulee");
    }

    [Fact]
    public void SetSlugFromText_SymbolsOnly_ShouldFailSlugRequired()
    {
        var entity = new TestSluggable { Slug = "keep-me" };

        var result = Success(entity).SetSlugFromText("!!!");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("sluggable.slug.required");
        entity.Slug.ShouldBe("keep-me");
    }
}
