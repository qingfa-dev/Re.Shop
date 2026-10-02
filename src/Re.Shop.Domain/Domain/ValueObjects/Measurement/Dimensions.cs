using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Measurement;

/// <summary>Three non-negative measurements expressed in a single unit.</summary>
/// <param name="Length">The length measurement.</param>
/// <param name="Width">The width measurement.</param>
/// <param name="Height">The height measurement.</param>
/// <param name="Unit">The unit shared by all three measurements.</param>
public readonly partial record struct Dimensions(
    decimal Length,
    decimal Width,
    decimal Height,
    Unit Unit)
{
    /// <summary>Creates dimensions when all measurements are non-negative and the unit is valid.</summary>
    /// <param name="length">The length measurement.</param>
    /// <param name="width">The width measurement.</param>
    /// <param name="height">The height measurement.</param>
    /// <param name="unit">The unit shared by all measurements.</param>
    /// <returns>The dimensions, or validation errors.</returns>
    public static Result<Dimensions, Error> Create(
        decimal length,
        decimal width,
        decimal height,
        Unit unit)
    {
        var candidate = new Dimensions(length, width, height, unit);

        return DimensionsValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="Dimensions"/> creation.</summary>
public static class DimensionsResult
{
    /// <summary>Contains dimension validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when length is negative.</summary>
        public static Error LengthNegative => Error.Validation(
            code: "Dimensions.Length.Negative",
            message: "Length must not be negative.");

        /// <summary>Error returned when width is negative.</summary>
        public static Error WidthNegative => Error.Validation(
            code: "Dimensions.Width.Negative",
            message: "Width must not be negative.");

        /// <summary>Error returned when height is negative.</summary>
        public static Error HeightNegative => Error.Validation(
            code: "Dimensions.Height.Negative",
            message: "Height must not be negative.");

        /// <summary>Error returned when the unit code is invalid.</summary>
        public static Error UnitInvalid => Error.Validation(
            code: "Dimensions.Unit.Invalid",
            message: "Dimensions unit must be a valid unit code.");

        #endregion
    }
}

/// <summary>Defines the minimum accepted dimension measurement.</summary>
public static class DimensionsConstant
{
    /// <summary>Dimension measurement limits.</summary>
    public static class Constraints
    {
        /// <summary>The smallest accepted value for each dimension.</summary>
        public const decimal MinValue = 0m;
    }
}

/// <summary>Validates dimensions without modifying them.</summary>
public static class DimensionsValidator
{
    /// <summary>
    /// Validates dimensions without mutating them.
    /// </summary>
    /// <summary>Returns success when all measurements are non-negative and the unit code is valid.</summary>
    /// <param name="dimensions">The dimensions to validate.</param>
    /// <returns>The unchanged value on success, or its validation errors.</returns>
    public static Result<Dimensions, Error> Validate(Dimensions dimensions)
    {
        return Result<Dimensions, Error>.Success(dimensions)
            .Ensure(
                predicate: _ =>
                    dimensions.Length >=
                    DimensionsConstant.Constraints.MinValue,
                error: DimensionsResult.Errors.LengthNegative)

            .Ensure(
                predicate: _ =>
                    dimensions.Width >=
                    DimensionsConstant.Constraints.MinValue,
                error: DimensionsResult.Errors.WidthNegative)

            .Ensure(
                predicate: _ =>
                    dimensions.Height >=
                    DimensionsConstant.Constraints.MinValue,
                error: DimensionsResult.Errors.HeightNegative)

            .Ensure(
                predicate: _ =>
                    !string.IsNullOrWhiteSpace(dimensions.Unit.Code) &&
                    Regex.IsMatch(
                        dimensions.Unit.Code!,
                        UnitConstant.Patterns.Code),
                error: DimensionsResult.Errors.UnitInvalid);
    }
}
