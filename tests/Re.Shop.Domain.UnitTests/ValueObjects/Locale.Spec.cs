using BuildingBlocks.Core.ValueObjects.Localization;

namespace Domain.UnitTests.ValueObjects;

public class LocaleSpec
{
    [Fact]
    public void Create_ValidInput_ShouldTrimCode()
    {
        var result = Locale.Create(" en-US ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Code.ShouldBe("en-US");
    }

    [Fact]
    public void Create_MissingCode_ShouldFailWithCodeRequired()
    {
        var result = Locale.Create(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Locale.Code.Required");
    }
}
