using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Measurement;

/// <summary>A normalized, validated unit code.</summary>
/// <param name="Code">The lowercase code created by <see cref="Create"/>.</param>
public readonly partial record struct Unit(string? Code)
{
    /// <summary>Creates a unit code after trimming, lowercasing, and validating it.</summary>
    /// <param name="code">The candidate unit code.</param>
    /// <returns>The normalized unit, or a validation error.</returns>
    public static Result<Unit, Error> Create(string? code)
    {
        var candidate = new Unit(code?.Trim().ToLowerInvariant());

        return UnitValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="Unit"/> creation.</summary>
public static class UnitResult
{
    /// <summary>Contains unit-code validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the unit code is missing.</summary>
        public static Error CodeRequired => Error.Validation(
            code: "Unit.Code.Required",
            message: "Unit code must be specified.");

        /// <summary>Error returned when the unit code exceeds four characters.</summary>
        public static Error CodeTooLong => Error.Validation(
            code: "Unit.Code.TooLong",
            message:
                $"Unit code cannot exceed {UnitConstant.Constraints.MaxLength} characters.");

        /// <summary>Error returned when the unit code contains invalid characters.</summary>
        public static Error CodeInvalid => Error.Validation(
            code: "Unit.Code.Invalid",
            message: "Unit code must be 1-4 lowercase letters.");

        #endregion
    }
}

/// <summary>Defines unit-code length and syntax constraints.</summary>
public static class UnitConstant
{
    /// <summary>Unit-code length limits.</summary>
    public static class Constraints
    {
        /// <summary>Maximum accepted unit-code length.</summary>
        public const int MaxLength = 4;
    }

    /// <summary>Regular expressions used to validate unit codes.</summary>
    public static class Patterns
    {
        /// <summary>Accepts one to four lowercase ASCII letters.</summary>
        public const string Code = @"^[a-z]{1,4}$";
    }
}

/// <summary>Validates unit codes without modifying them.</summary>
public static class UnitValidator
{
    /// <summary>
    /// Validates a unit code without mutating it.
    /// </summary>
    /// <summary>Returns success when the code contains one to four lowercase letters.</summary>
    /// <param name="unit">The unit to validate.</param>
    /// <returns>The unchanged unit on success, or its validation errors.</returns>
    public static Result<Unit, Error> Validate(Unit unit)
    {
        return Result<Unit, Error>.Success(unit)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(unit.Code),
                error: UnitResult.Errors.CodeRequired)

            .Ensure(
                predicate: _ =>
                    unit.Code!.Length <= UnitConstant.Constraints.MaxLength,
                error: UnitResult.Errors.CodeTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(unit.Code!, UnitConstant.Patterns.Code),
                error: UnitResult.Errors.CodeInvalid);
    }
}
