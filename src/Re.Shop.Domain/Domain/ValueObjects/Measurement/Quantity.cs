using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Measurement;

/// <summary>A non-negative quantity expressed in a unit.</summary>
/// <param name="Value">The quantity amount.</param>
/// <param name="Unit">The unit of the amount.</param>
public readonly partial record struct Quantity(decimal Value, Unit Unit)
{
    /// <summary>Creates a quantity when its amount is non-negative and its unit is valid.</summary>
    /// <param name="value">The quantity amount.</param>
    /// <param name="unit">The unit of the amount.</param>
    /// <returns>The quantity, or validation errors.</returns>
    public static Result<Quantity, Error> Create(
        decimal value,
        Unit unit)
    {
        var candidate = new Quantity(value, unit);

        return QuantityValidator.Validate(candidate);
    }
}

/// <summary>Groups validation and arithmetic errors for <see cref="Quantity"/>.</summary>
public static class QuantityResult
{
    /// <summary>Contains quantity validation and operation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the amount is negative.</summary>
        public static Error ValueNegative => Error.Validation(
            code: "Quantity.Value.Negative",
            message: "Quantity must not be negative.");

        /// <summary>Error returned when the unit code is invalid.</summary>
        public static Error UnitInvalid => Error.Validation(
            code: "Quantity.Unit.Invalid",
            message: "Quantity unit must be a valid unit code.");

        /// <summary>Error returned when arithmetic operands use different units.</summary>
        public static Error UnitMismatch => Error.Validation(
            code: "Quantity.Unit.Mismatch",
            message: "Units must match for this operation.");

        #endregion
    }
}

/// <summary>Defines the minimum accepted quantity amount.</summary>
public static class QuantityConstant
{
    /// <summary>Quantity amount limits.</summary>
    public static class Constraints
    {
        /// <summary>The smallest accepted quantity amount.</summary>
        public const decimal MinValue = 0m;
    }
}

/// <summary>Validates quantities without modifying them.</summary>
public static class QuantityValidator
{
    /// <summary>
    /// Validates a quantity without mutating it.
    /// </summary>
    /// <summary>Returns success when the amount is non-negative and its unit code is valid.</summary>
    /// <param name="quantity">The quantity to validate.</param>
    /// <returns>The unchanged value on success, or its validation errors.</returns>
    public static Result<Quantity, Error> Validate(Quantity quantity)
    {
        return Result<Quantity, Error>.Success(quantity)
            .Ensure(
                predicate: _ =>
                    quantity.Value >= QuantityConstant.Constraints.MinValue,
                error: QuantityResult.Errors.ValueNegative)

            .Ensure(
                predicate: _ =>
                    !string.IsNullOrWhiteSpace(quantity.Unit.Code) &&
                    Regex.IsMatch(
                        quantity.Unit.Code!,
                        UnitConstant.Patterns.Code),
                error: QuantityResult.Errors.UnitInvalid);
    }
}

/// <summary>Provides unit-aware arithmetic for quantities.</summary>
public static class QuantityExtensions
{
    /// <summary>
    /// Adds two quantities of the same unit.
    /// </summary>
    /// <summary>Adds two quantities expressed in the same unit.</summary>
    /// <param name="left">The left operand and result unit.</param>
    /// <param name="right">The quantity to add.</param>
    /// <returns>The sum, or a unit-mismatch error.</returns>
    public static Result<Quantity, Error> Add(
        this Quantity left,
        Quantity right)
    {
        return Result<Quantity, Error>.Success(left)
            .Ensure(
                predicate: _ => left.Unit.Code == right.Unit.Code,
                error: QuantityResult.Errors.UnitMismatch)

            .Map(quantity =>
                new Quantity(quantity.Value + right.Value, quantity.Unit));
    }
}
