using BuildingBlocks.Core.ValueObjects.Localization;

namespace Domain.UnitTests.ValueObjects;

public class LanguageSpec
{
    [Fact]
    public void Create_ValidInput_ShouldTrimAndNormalizeCode()
    {
        var result = Language.Create(" en ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Code.ShouldBe("en");
    }

    [Fact]
    public void Create_MissingCode_ShouldFailWithCodeRequired()
    {
        var result = Language.Create(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Language.Code.Required");
    }
}
