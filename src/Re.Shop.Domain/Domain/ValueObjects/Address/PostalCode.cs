using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Address;

/// <summary>A validated postal or ZIP code.</summary>
/// <param name="Value">The postal code; <see cref="Create"/> trims and uppercases it.</param>
public readonly partial record struct PostalCode(string? Value)
{
    /// <summary>Creates a normalized postal code with at most ten allowed characters.</summary>
    /// <param name="value">The candidate postal code.</param>
    /// <returns>The normalized code, or a validation error.</returns>
    public static Result<PostalCode, Error> Create(string? value)
    {
        var candidate = new PostalCode(value?.Trim().ToUpperInvariant());

        return PostalCodeValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="PostalCode"/> creation.</summary>
public static class PostalCodeResult
{
    /// <summary>Contains postal-code validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the postal code is missing.</summary>
        public static Error ValueRequired => Error.Validation(
            code: "PostalCode.Value.Required",
            message: "Postal code must be specified.");

        /// <summary>Error returned when the postal code exceeds ten characters.</summary>
        public static Error ValueTooLong => Error.Validation(
            code: "PostalCode.Value.TooLong",
            message:
                $"Postal code cannot exceed {PostalCodeConstant.Constraints.MaxLength} characters.");

        /// <summary>Error returned when the postal code contains unsupported characters.</summary>
        public static Error ValueInvalid => Error.Validation(
            code: "PostalCode.Value.Invalid",
            message:
                "Postal code may contain only letters, digits, spaces and hyphens.");

        #endregion
    }
}

/// <summary>Defines postal-code length and character constraints.</summary>
public static class PostalCodeConstant
{
    /// <summary>Postal-code length limits.</summary>
    public static class Constraints
    {
        /// <summary>Maximum accepted number of characters.</summary>
        public const int MaxLength = 10;
    }

    /// <summary>Regular expressions used to validate postal codes.</summary>
    public static class Patterns
    {
        /// <summary>Accepts an alphanumeric first character followed by up to nine alphanumeric, space, or hyphen characters.</summary>
        public const string Code = @"^[A-Za-z0-9][A-Za-z0-9 -]{0,9}$";
    }
}

/// <summary>Validates postal-code values without modifying them.</summary>
public static class PostalCodeValidator
{
    /// <summary>
    /// Validates a postal code without mutating it.
    /// </summary>
    /// <summary>Returns success when a postal code is present and matches the configured format.</summary>
    /// <param name="postalCode">The postal code to validate.</param>
    /// <returns>The unchanged value on success, or its validation errors.</returns>
    public static Result<PostalCode, Error> Validate(PostalCode postalCode)
    {
        return Result<PostalCode, Error>.Success(postalCode)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(postalCode.Value),
                error: PostalCodeResult.Errors.ValueRequired)

            .Ensure(
                predicate: _ =>
                    postalCode.Value!.Length <=
                    PostalCodeConstant.Constraints.MaxLength,
                error: PostalCodeResult.Errors.ValueTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        postalCode.Value!,
                        PostalCodeConstant.Patterns.Code),
                error: PostalCodeResult.Errors.ValueInvalid);
    }
}
