using SharedKernel.Concerns.Content.Localizable;

namespace SharedKernel.UnitTests.Concerns.Content.Localizable;

public class LocalizableExtensionSpec
{
    private sealed class TestLocalizable : ILocalizable
    {
        public ICollection<LocalizedValue> LocalizedValues { get; } =
            new List<LocalizedValue>();
    }

    private static Result<TestLocalizable, Error> Success(
        TestLocalizable entity)
    {
        return Result<TestLocalizable, Error>.Success(entity);
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
        result.Errors![0].Code.ShouldBe("localizable.property.required");
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
        result.Errors![0].Code.ShouldBe("localizable.property.too_long");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void SetLocalizedValue_NullLocale_ShouldFailWithoutMutation()
    {
        var entity = new TestLocalizable();

        var result = Success(entity)
            .SetLocalizedValue("title", null, "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("localizable.locale.required");
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
        result.Errors![0].Code.ShouldBe("localizable.locale.too_long");
        entity.LocalizedValues.ShouldBeEmpty();
    }
}
