using BuildingBlocks.Core.ValueObjects.Measurement;

namespace Domain.UnitTests.ValueObjects;

public class QuantitySpec
{
    [Fact]
    public void Create_NonNegativeValue_ShouldSucceed()
    {
        var unit = Unit.Create("kg").Value;
        var result = Quantity.Create(4m, unit);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(4m);
    }

    [Fact]
    public void Create_NegativeValue_ShouldFailWithValueNegative()
    {
        var unit = Unit.Create("kg").Value;
        var result = Quantity.Create(-1m, unit);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Quantity.Value.Negative");
    }
}
