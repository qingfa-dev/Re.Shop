using BuildingBlocks.Core.ValueObjects.Measurement;

namespace Domain.UnitTests.ValueObjects;

public class UnitSpec
{
    [Fact]
    public void Create_ValidCode_ShouldTrimAndLowercaseCode()
    {
        var result = Unit.Create(" KG ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Code.ShouldBe("kg");
    }

    [Fact]
    public void Create_MissingCode_ShouldFailWithCodeRequired()
    {
        var result = Unit.Create(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Unit.Code.Required");
    }
}
