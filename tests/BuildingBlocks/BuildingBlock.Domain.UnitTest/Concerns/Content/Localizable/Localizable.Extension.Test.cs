using BuildingBlock.Kernel.Results;

using BuildingBlocks.Domain.Concerns.Content.Localizable;

namespace BuildingBlock.Domain.UnitTest.Concerns.Content.Localizable;

public class LocalizableExtensionTest
{
    private sealed class TestLocalizable : ILocalizable
    {
        public ICollection<LocalizedValue> LocalizedValues { get; } =
            new List<LocalizedValue>();
    }

    private static Result<TestLocalizable> Success(
        TestLocalizable entity)
    {
        return Result<TestLocalizable>.Success(entity);
    }

    [Fact]
    public void SetLocalizedValue_NewEntry_ShouldAddEntry()
    {
        var entity = new TestLocalizable();

        var result = Success(entity)
            .SetLocalizedValue("title", "en", "Hello");

        result.IsSuccess.ShouldBeTrue();
        entity.LocalizedValues.ShouldHaveSingleItem();
        entity.LocalizedValues.Single().Property.ShouldBe("title");
        entity.LocalizedValues.Single().Locale.ShouldBe("en");
        entity.LocalizedValues.Single().Value.ShouldBe("Hello");
    }

    [Fact]
    public void SetLocalizedValue_ExistingEntry_ShouldUpdateValue()
    {
        var entity = new TestLocalizable();
        Success(entity).SetLocalizedValue("title", "en", "Old");

        var result = Success(entity)
            .SetLocalizedValue("title", "en", "New");

        result.IsSuccess.ShouldBeTrue();
        entity.LocalizedValues.ShouldHaveSingleItem();
        entity.LocalizedValues.Single().Value.ShouldBe("New");
    }

    [Fact]
    public void SetLocalizedValue_NullProperty_ShouldFailWithoutMutation()
    {
        var entity = new TestLocalizable();

        var result = Success(entity)
            .SetLocalizedValue(null, "en", "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Localizable.Property.Required");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void SetLocalizedValue_TooLongProperty_ShouldFailWithoutMutation()
    {
        var entity = new TestLocalizable();
        var property = new string(
            'p',
            LocalizableConstant.Constraints.MaxPropertyLength + 1);

        var result = Success(entity)
            .SetLocalizedValue(property, "en", "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Localizable.Property.TooLong");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void SetLocalizedValue_NullLocale_ShouldFailWithoutMutation()
    {
        var entity = new TestLocalizable();

        var result = Success(entity)
            .SetLocalizedValue("title", null, "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Localizable.Locale.Required");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void SetLocalizedValue_TooLongLocale_ShouldFailWithoutMutation()
    {
        var entity = new TestLocalizable();
        var locale = new string(
            'l',
            LocalizableConstant.Constraints.MaxLocaleLength + 1);

        var result = Success(entity)
            .SetLocalizedValue("title", locale, "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Localizable.Locale.TooLong");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateLocalizedValue_NullEntity_ShouldFailEntityRequired()
    {
        var result = LocalizableValidator.ValidateLocalizedValue(
            (TestLocalizable)null!,
            "title",
            "en",
            "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Localizable.Entity.Required");
    }
}
