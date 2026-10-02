using BuildingBlocks.Core.ValueObjects.Common;

namespace Domain.UnitTests.ValueObjects;

public class TimeRangeSpec
{
    [Fact]
    public void Create_OrderedTimes_ShouldSucceed()
    {
        var result = TimeRange.Create(
            new TimeOnly(9, 0),
            new TimeOnly(10, 0));

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Create_EndBeforeStart_ShouldFailWithInvalidRange()
    {
        var result = TimeRange.Create(
            new TimeOnly(10, 0),
            new TimeOnly(9, 0));

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("TimeRange.Range.Invalid");
    }
}
