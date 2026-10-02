using BuildingBlocks.Core.ValueObjects.Address;

namespace Domain.UnitTests.ValueObjects;

public class PostalCodeSpec
{
    [Fact]
    public void Create_ValidInput_ShouldTrimAndUppercaseValue()
    {
        var result = PostalCode.Create(" a1b 2c3 ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("A1B 2C3");
    }

    [Fact]
    public void Create_MissingValue_ShouldFailWithValueRequired()
    {
        var result = PostalCode.Create(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("PostalCode.Value.Required");
    }
}
