namespace BuildingBlocks.Core.ValueObjects.Common;

/// <summary>An inclusive range between two times on the same day.</summary>
/// <param name="Start">The first included time.</param>
/// <param name="End">The last included time.</param>
public readonly partial record struct TimeRange(TimeOnly Start, TimeOnly End)
{
    /// <summary>Creates a time range whose end is not before its start.</summary>
    /// <param name="start">The first included time.</param>
    /// <param name="end">The last included time.</param>
    /// <returns>The range, or a validation error when the times are reversed.</returns>
    public static Result<TimeRange, Error> Create(
        TimeOnly start,
        TimeOnly end)
    {
        var candidate = new TimeRange(start, end);

        return TimeRangeValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="TimeRange"/> creation.</summary>
public static class TimeRangeResult
{
    /// <summary>Contains time-range validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the end time precedes the start time.</summary>
        public static Error RangeInvalid => Error.Validation(
            code: "TimeRange.Range.Invalid",
            message: "Start time must not be after end time.");

        #endregion
    }
}

/// <summary>Validates time ranges and provides inclusive range calculations.</summary>
public static class TimeRangeValidator
{
    /// <summary>
    /// Validates a time range without mutating it.
    /// </summary>
    /// <summary>Returns success when the start time is not after the end time.</summary>
    /// <param name="range">The range to validate.</param>
    /// <returns>The unchanged range on success, or its validation error.</returns>
    public static Result<TimeRange, Error> Validate(TimeRange range)
    {
        return Result<TimeRange, Error>.Success(range)
            .Ensure(
                predicate: _ => range.Start <= range.End,
                error: TimeRangeResult.Errors.RangeInvalid);
    }
}

/// <summary>Provides elapsed-time, containment, and overlap calculations for time ranges.</summary>
public static class TimeRangeExtensions
{
    /// <summary>Returns the elapsed time from the start to the end of the range.</summary>
    /// <param name="range">The time range.</param>
    /// <returns>The difference between its end and start times.</returns>
    public static TimeSpan Duration(this TimeRange range)
    {
        return range.End.ToTimeSpan() - range.Start.ToTimeSpan();
    }

    /// <summary>
    /// Whether the given time falls inside the range (inclusive).
    /// </summary>
    /// <summary>Determines whether a time is inside the range, including either endpoint.</summary>
    /// <param name="range">The inclusive time range.</param>
    /// <param name="time">The time to test.</param>
    /// <returns><see langword="true"/> when the time is included.</returns>
    public static bool Includes(this TimeRange range, TimeOnly time)
    {
        return time >= range.Start && time <= range.End;
    }

    /// <summary>
    /// Whether two ranges share at least one moment.
    /// </summary>
    /// <summary>Determines whether two inclusive ranges share at least one moment.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when the ranges overlap.</returns>
    public static bool Overlaps(
        this TimeRange left,
        TimeRange right)
    {
        return left.Start <= right.End && right.Start <= left.End;
    }
}
