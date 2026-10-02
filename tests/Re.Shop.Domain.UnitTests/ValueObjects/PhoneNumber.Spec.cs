using BuildingBlocks.Core.ValueObjects.Identity;

namespace Domain.UnitTests.ValueObjects;

public class PhoneNumberSpec
{
    [Fact]
    public void Create_ValidInput_ShouldTrimValue()
    {
        var result = PhoneNumber.Create(" +1 (555) 123-4567 ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("+1 (555) 123-4567");
    }

    [Fact]
    public void Create_MissingValue_ShouldFailWithValueRequired()
    {
        var result = PhoneNumber.Create(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("PhoneNumber.Value.Required");
    }
}
