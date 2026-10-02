using BuildingBlocks.Core.ValueObjects.Measurement;

namespace Domain.UnitTests.ValueObjects;

public class DimensionsSpec
{
    [Fact]
    public void Create_NonNegativeMeasurements_ShouldSucceed()
    {
        var unit = Unit.Create("cm").Value;
        var result = Dimensions.Create(1m, 2m, 3m, unit);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Length.ShouldBe(1m);
        result.Value.Width.ShouldBe(2m);
        result.Value.Height.ShouldBe(3m);
    }

    [Fact]
    public void Create_NegativeLength_ShouldFailWithLengthNegative()
    {
        var unit = Unit.Create("cm").Value;
        var result = Dimensions.Create(-1m, 2m, 3m, unit);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Dimensions.Length.Negative");
    }

    [Fact]
    public void RecordEquality_ShouldCompareAllMeasurementComponents()
    {
        var unit = Unit.Create("cm").Value;
        var dimensions = Dimensions.Create(1m, 2m, 3m, unit).Value;
        var equivalent = Dimensions.Create(1m, 2m, 3m, unit).Value;
        var changed = Dimensions.Create(1m, 2m, 4m, unit).Value;

        dimensions.ShouldBe(equivalent);
        dimensions.ShouldNotBe(changed);
    }
}
