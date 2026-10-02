using BuildingBlocks.Core.ValueObjects.Identity;

namespace Domain.UnitTests.ValueObjects;

public class EmailAddressSpec
{
    [Fact]
    public void Create_ValidInput_ShouldTrimAndLowercaseValue()
    {
        var result = EmailAddress.Create(" Ada@example.com ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("ada@example.com");
    }

    [Fact]
    public void Create_MissingValue_ShouldFailWithValueRequired()
    {
        var result = EmailAddress.Create(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("EmailAddress.Value.Required");
    }
}
