using SharedKernel.Concerns.Content.Localizable;

namespace SharedKernel.UnitTests.Concerns.Content.Localizable;

public class LocalizableValidatorSpec
{
    private sealed class TestLocalizable : ILocalizable
    {
        public ICollection<LocalizedValue> LocalizedValues { get; } =
            new List<LocalizedValue>();
    }

    [Fact]
    public void ValidateLocalizedValue_ValidInput_ShouldSucceed()
    {
        var entity = new TestLocalizable();

        var result = LocalizableValidator.ValidateLocalizedValue(
            entity,
            "title",
            "en",
            "Hello");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
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
        result.Errors![0].Code.ShouldBe("localizable.entity.required");
    }

    [Fact]
    public void ValidateLocalizedValue_NullProperty_ShouldFailPropertyRequired()
    {
        var entity = new TestLocalizable();

        var result = LocalizableValidator.ValidateLocalizedValue(
            entity,
            null,
            "en",
            "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("localizable.property.required");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateLocalizedValue_WhitespaceProperty_ShouldFailPropertyRequired()
    {
        var entity = new TestLocalizable();

        var result = LocalizableValidator.ValidateLocalizedValue(
            entity,
            "   ",
            "en",
            "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("localizable.property.required");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateLocalizedValue_TooLongProperty_ShouldFailPropertyTooLong()
    {
        var entity = new TestLocalizable();
        var property = new string(
            'p',
            LocalizableConstant.Constraints.MaxPropertyLength + 1);

        var result = LocalizableValidator.ValidateLocalizedValue(
            entity,
            property,
            "en",
            "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("localizable.property.too_long");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateLocalizedValue_NullLocale_ShouldFailLocaleRequired()
    {
        var entity = new TestLocalizable();

        var result = LocalizableValidator.ValidateLocalizedValue(
            entity,
            "title",
            null,
            "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("localizable.locale.required");
        entity.LocalizedValues.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateLocalizedValue_TooLongLocale_ShouldFailLocaleTooLong()
    {
        var entity = new TestLocalizable();
        var locale = new string(
            'l',
            LocalizableConstant.Constraints.MaxLocaleLength + 1);

        var result = LocalizableValidator.ValidateLocalizedValue(
            entity,
            "title",
            locale,
            "Hello");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("localizable.locale.too_long");
        entity.LocalizedValues.ShouldBeEmpty();
    }
}
