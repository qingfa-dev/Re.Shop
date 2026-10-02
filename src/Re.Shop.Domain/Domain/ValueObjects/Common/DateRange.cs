namespace BuildingBlocks.Core.ValueObjects.Common;

/// <summary>An inclusive range of calendar dates.</summary>
/// <param name="Start">The first included date.</param>
/// <param name="End">The last included date.</param>
public readonly partial record struct DateRange(DateOnly Start, DateOnly End)
{
    /// <summary>Creates a date range whose end is not before its start.</summary>
    /// <param name="start">The first included date.</param>
    /// <param name="end">The last included date.</param>
    /// <returns>The range, or a validation error when the dates are reversed.</returns>
    public static Result<DateRange, Error> Create(
        DateOnly start,
        DateOnly end)
    {
        var candidate = new DateRange(start, end);

        return DateRangeValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="DateRange"/> creation.</summary>
public static class DateRangeResult
{
    /// <summary>Contains date-range validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the end date precedes the start date.</summary>
        public static Error RangeInvalid => Error.Validation(
            code: "DateRange.Range.Invalid",
            message: "Start date must not be after end date.");

        #endregion
    }
}

/// <summary>Validates date ranges without modifying them.</summary>
public static class DateRangeValidator
{
    /// <summary>
    /// Validates a date range without mutating it.
    /// </summary>
    /// <summary>Returns success when the start date is not after the end date.</summary>
    /// <param name="range">The range to validate.</param>
    /// <returns>The unchanged range on success, or its validation error.</returns>
    public static Result<DateRange, Error> Validate(DateRange range)
    {
        return Result<DateRange, Error>.Success(range)
            .Ensure(
                predicate: _ => range.Start <= range.End,
                error: DateRangeResult.Errors.RangeInvalid);
    }
}

/// <summary>Provides inclusive date-range calculations.</summary>
public static class DateRangeExtensions
{
    /// <summary>
    /// Total days covered by the range, counting both ends.
    /// </summary>
    /// <summary>Returns the number of calendar days covered, including both endpoints.</summary>
    /// <param name="range">The inclusive date range.</param>
    /// <returns>The inclusive day count.</returns>
    public static int Duration(this DateRange range)
    {
        return range.End.DayNumber - range.Start.DayNumber + 1;
    }

    /// <summary>
    /// Whether the given date falls inside the range (inclusive).
    /// </summary>
    /// <summary>Determines whether a date is inside the range, including either endpoint.</summary>
    /// <param name="range">The inclusive date range.</param>
    /// <param name="date">The date to test.</param>
    /// <returns><see langword="true"/> when the date is included.</returns>
    public static bool Includes(this DateRange range, DateOnly date)
    {
        return date >= range.Start && date <= range.End;
    }

    /// <summary>
    /// Whether two ranges share at least one day.
    /// </summary>
    /// <summary>Determines whether two inclusive ranges share at least one date.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when the ranges overlap.</returns>
    public static bool Overlaps(
        this DateRange left,
        DateRange right)
    {
        return left.Start <= right.End && right.Start <= left.End;
    }
}
