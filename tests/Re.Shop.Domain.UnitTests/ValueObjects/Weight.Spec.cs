using BuildingBlocks.Core.ValueObjects.Measurement;

namespace Domain.UnitTests.ValueObjects;

public class WeightSpec
{
    [Fact]
    public void Create_NonNegativeValue_ShouldSucceed()
    {
        var unit = Unit.Create("kg").Value;
        var result = Weight.Create(3m, unit);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(3m);
    }

    [Fact]
    public void Create_NegativeValue_ShouldFailWithValueNegative()
    {
        var unit = Unit.Create("kg").Value;
        var result = Weight.Create(-1m, unit);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Weight.Value.Negative");
    }
}
