using BuildingBlocks.Core.ValueObjects.Identity;

namespace Domain.UnitTests.ValueObjects;

public class UrlSpec
{
    [Fact]
    public void Create_AbsoluteHttpUrl_ShouldTrimValue()
    {
        var result = Url.Create(" https://example.com ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("https://example.com");
    }

    [Fact]
    public void Create_MissingValue_ShouldFailWithValueRequired()
    {
        var result = Url.Create(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Url.Value.Required");
    }
}
