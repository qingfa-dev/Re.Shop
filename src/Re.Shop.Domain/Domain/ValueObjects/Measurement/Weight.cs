using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Measurement;

/// <summary>A non-negative weight expressed in a unit.</summary>
/// <param name="Value">The weight amount.</param>
/// <param name="Unit">The unit of the amount.</param>
public readonly partial record struct Weight(decimal Value, Unit Unit)
{
    /// <summary>Creates a weight when its amount is non-negative and its unit is valid.</summary>
    /// <param name="value">The weight amount.</param>
    /// <param name="unit">The unit of the amount.</param>
    /// <returns>The weight, or validation errors.</returns>
    public static Result<Weight, Error> Create(
        decimal value,
        Unit unit)
    {
        var candidate = new Weight(value, unit);

        return WeightValidator.Validate(candidate);
    }
}

/// <summary>Groups validation and arithmetic errors for <see cref="Weight"/>.</summary>
public static class WeightResult
{
    /// <summary>Contains weight validation and operation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the weight amount is negative.</summary>
        public static Error ValueNegative => Error.Validation(
            code: "Weight.Value.Negative",
            message: "Weight must not be negative.");

        /// <summary>Error returned when the unit code is invalid.</summary>
        public static Error UnitInvalid => Error.Validation(
            code: "Weight.Unit.Invalid",
            message: "Weight unit must be a valid unit code.");

        /// <summary>Error returned when arithmetic operands use different units.</summary>
        public static Error UnitMismatch => Error.Validation(
            code: "Weight.Unit.Mismatch",
            message: "Units must match for this operation.");

        #endregion
    }
}

/// <summary>Defines the minimum accepted weight amount.</summary>
public static class WeightConstant
{
    /// <summary>Weight amount limits.</summary>
    public static class Constraints
    {
        /// <summary>The smallest accepted weight amount.</summary>
        public const decimal MinValue = 0m;
    }
}

/// <summary>Validates weights without modifying them.</summary>
public static class WeightValidator
{
    /// <summary>
    /// Validates a weight without mutating it.
    /// </summary>
    /// <summary>Returns success when the amount is non-negative and its unit code is valid.</summary>
    /// <param name="weight">The weight to validate.</param>
    /// <returns>The unchanged value on success, or its validation errors.</returns>
    public static Result<Weight, Error> Validate(Weight weight)
    {
        return Result<Weight, Error>.Success(weight)
            .Ensure(
                predicate: _ =>
                    weight.Value >= WeightConstant.Constraints.MinValue,
                error: WeightResult.Errors.ValueNegative)

            .Ensure(
                predicate: _ =>
                    !string.IsNullOrWhiteSpace(weight.Unit.Code) &&
                    Regex.IsMatch(
                        weight.Unit.Code!,
                        UnitConstant.Patterns.Code),
                error: WeightResult.Errors.UnitInvalid);
    }
}

/// <summary>Provides unit-aware arithmetic for weights.</summary>
public static class WeightExtensions
{
    /// <summary>
    /// Adds two weights of the same unit.
    /// </summary>
    /// <summary>Adds two weights expressed in the same unit.</summary>
    /// <param name="left">The left operand and result unit.</param>
    /// <param name="right">The weight to add.</param>
    /// <returns>The sum, or a unit-mismatch error.</returns>
    public static Result<Weight, Error> Add(
        this Weight left,
        Weight right)
    {
        return Result<Weight, Error>.Success(left)
            .Ensure(
                predicate: _ => left.Unit.Code == right.Unit.Code,
                error: WeightResult.Errors.UnitMismatch)

            .Map(weight =>
                new Weight(weight.Value + right.Value, weight.Unit));
    }
}
