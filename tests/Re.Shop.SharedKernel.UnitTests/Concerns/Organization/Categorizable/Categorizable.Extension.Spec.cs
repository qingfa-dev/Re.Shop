using SharedKernel.Concerns.Organization.Categorizable;

namespace SharedKernel.UnitTests.Concerns.Organization.Categorizable;

public class CategorizableExtensionSpec
{
    private sealed class TestCategorizable : ICategorizable<string>
    {
        public ICollection<string> Categories { get; } = new List<string>();
    }

    private static Result<TestCategorizable, Error> Success(
        TestCategorizable entity)
    {
        return Result<TestCategorizable, Error>.Success(entity);
    }

    [Fact]
    public void AddCategory_NewCategory_ShouldAddCategory()
    {
        var entity = new TestCategorizable();

        var result = Success(entity).AddCategory("shoes");

        result.IsSuccess.ShouldBeTrue();
        entity.Categories.ShouldHaveSingleItem()
            .ShouldBe("shoes");
    }

    [Fact]
    public void AddCategory_DuplicateCategory_ShouldFailWithoutMutation()
    {
        var entity = new TestCategorizable();
        Success(entity).AddCategory("shoes");

        var result = Success(entity).AddCategory("shoes");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("categorizable.category.duplicate");
        entity.Categories.ShouldHaveSingleItem();
    }

    [Fact]
    public void RemoveCategory_AssignedCategory_ShouldRemoveCategory()
    {
        var entity = new TestCategorizable();
        Success(entity).AddCategory("shoes");

        var result = Success(entity).RemoveCategory("shoes");

        result.IsSuccess.ShouldBeTrue();
        entity.Categories.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveCategory_MissingCategory_ShouldFailWithoutMutation()
    {
        var entity = new TestCategorizable();
        Success(entity).AddCategory("shoes");

        var result = Success(entity).RemoveCategory("boots");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("categorizable.category.not_found");
        entity.Categories.ShouldHaveSingleItem();
    }
}
