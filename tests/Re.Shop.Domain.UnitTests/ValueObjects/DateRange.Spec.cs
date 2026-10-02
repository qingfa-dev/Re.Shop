using BuildingBlocks.Core.ValueObjects.Common;

namespace Domain.UnitTests.ValueObjects;

public class DateRangeSpec
{
    [Fact]
    public void Create_OrderedDates_ShouldSucceed()
    {
        var result = DateRange.Create(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 2));

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Create_EndBeforeStart_ShouldFailWithInvalidRange()
    {
        var result = DateRange.Create(
            new DateOnly(2026, 1, 2),
            new DateOnly(2026, 1, 1));

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("DateRange.Range.Invalid");
    }
}
