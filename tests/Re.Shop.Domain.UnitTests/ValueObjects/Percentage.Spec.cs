using BuildingBlocks.Core.ValueObjects.Common;

namespace Domain.UnitTests.ValueObjects;

public class PercentageSpec
{
    [Fact]
    public void Create_ValueWithinRange_ShouldSucceed()
    {
        var result = Percentage.Create(42.5m);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(42.5m);
    }

    [Fact]
    public void Create_ValueAboveRange_ShouldFailWithOutOfRange()
    {
        var result = Percentage.Create(101m);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Percentage.Value.OutOfRange");
    }
}
