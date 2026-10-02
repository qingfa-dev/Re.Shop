using SharedKernel.Concerns.Organization.Categorizable;

namespace SharedKernel.UnitTests.Concerns.Organization.Categorizable;

public class CategorizableValidatorSpec
{
    private sealed class TestCategorizable : ICategorizable<string>
    {
        public ICollection<string> Categories { get; } = new List<string>();
    }

    [Fact]
    public void ValidateAddCategory_ValidInput_ShouldSucceed()
    {
        var entity = new TestCategorizable();

        var result = CategorizableValidator.ValidateAddCategory(
            entity,
            "shoes");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Categories.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateAddCategory_NullEntity_ShouldFailEntityRequired()
    {
        var result = CategorizableValidator.ValidateAddCategory(
            (TestCategorizable)null!,
            "shoes");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("categorizable.entity.required");
    }

    [Fact]
    public void ValidateAddCategory_DuplicateCategory_ShouldFailCategoryDuplicate()
    {
        var entity = new TestCategorizable();
        entity.Categories.Add("shoes");

        var result = CategorizableValidator.ValidateAddCategory(
            entity,
            "shoes");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("categorizable.category.duplicate");
        entity.Categories.ShouldHaveSingleItem().ShouldBe("shoes");
    }

    [Fact]
    public void ValidateRemoveCategory_ValidInput_ShouldSucceed()
    {
        var entity = new TestCategorizable();
        entity.Categories.Add("shoes");

        var result = CategorizableValidator.ValidateRemoveCategory(
            entity,
            "shoes");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Categories.ShouldHaveSingleItem().ShouldBe("shoes");
    }

    [Fact]
    public void ValidateRemoveCategory_NullEntity_ShouldFailEntityRequired()
    {
        var result = CategorizableValidator.ValidateRemoveCategory(
            (TestCategorizable)null!,
            "shoes");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("categorizable.entity.required");
    }

    [Fact]
    public void ValidateRemoveCategory_MissingCategory_ShouldFailCategoryNotFound()
    {
        var entity = new TestCategorizable();

        var result = CategorizableValidator.ValidateRemoveCategory(
            entity,
            "boots");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("categorizable.category.not_found");
        entity.Categories.ShouldBeEmpty();
    }
}
