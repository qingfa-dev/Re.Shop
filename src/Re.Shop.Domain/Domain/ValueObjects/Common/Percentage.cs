namespace BuildingBlocks.Core.ValueObjects.Common;

/// <summary>A percentage value constrained to the inclusive range zero to one hundred.</summary>
/// <param name="Value">The percentage number, where <c>100</c> represents one hundred percent.</param>
public readonly partial record struct Percentage(decimal Value)
{
    /// <summary>Creates a percentage within the inclusive range zero to one hundred.</summary>
    /// <param name="value">The percentage number.</param>
    /// <returns>The percentage, or a range validation error.</returns>
    public static Result<Percentage, Error> Create(decimal value)
    {
        var candidate = new Percentage(value);

        return PercentageValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="Percentage"/> creation.</summary>
public static class PercentageResult
{
    /// <summary>Contains percentage validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the value is below zero or above one hundred.</summary>
        public static Error ValueOutOfRange => Error.Validation(
            code: "Percentage.Value.OutOfRange",
            message:
                $"Percentage must be between {PercentageConstant.Constraints.MinValue} and {PercentageConstant.Constraints.MaxValue}.");

        #endregion
    }
}

/// <summary>Defines the inclusive percentage limits.</summary>
public static class PercentageConstant
{
    /// <summary>Percentage validation limits.</summary>
    public static class Constraints
    {
        /// <summary>The smallest accepted percentage.</summary>
        public const decimal MinValue = 0m;
        /// <summary>The largest accepted percentage.</summary>
        public const decimal MaxValue = 100m;
    }
}

/// <summary>Validates percentages without modifying them.</summary>
public static class PercentageValidator
{
    /// <summary>
    /// Validates a percentage without mutating it.
    /// </summary>
    /// <summary>Returns success when the value is between zero and one hundred inclusive.</summary>
    /// <param name="percentage">The percentage to validate.</param>
    /// <returns>The unchanged percentage on success, or its validation error.</returns>
    public static Result<Percentage, Error> Validate(Percentage percentage)
    {
        return Result<Percentage, Error>.Success(percentage)
            .Ensure(
                predicate: _ =>
                    percentage.Value >=
                        PercentageConstant.Constraints.MinValue &&
                    percentage.Value <=
                        PercentageConstant.Constraints.MaxValue,
                error: PercentageResult.Errors.ValueOutOfRange);
    }
}

/// <summary>Provides conversions and calculations for percentages.</summary>
public static class PercentageExtensions
{
    /// <summary>
    /// Converts the percentage to a decimal fraction (50% -> 0.5m).
    /// </summary>
    /// <summary>Converts a percentage number to its decimal fraction.</summary>
    /// <param name="percentage">The percentage to convert.</param>
    /// <returns>The percentage divided by one hundred.</returns>
    public static decimal ToFraction(this Percentage percentage)
    {
        return percentage.Value / 100m;
    }

    /// <summary>
    /// Applies the percentage to an amount (10% of 200 -> 20).
    /// </summary>
    /// <summary>Calculates a percentage of an amount.</summary>
    /// <param name="percentage">The percentage to apply.</param>
    /// <param name="amount">The base amount.</param>
    /// <returns>The amount multiplied by the percentage fraction.</returns>
    public static decimal ApplyTo(
        this Percentage percentage,
        decimal amount)
    {
        return amount * percentage.ToFraction();
    }
}
