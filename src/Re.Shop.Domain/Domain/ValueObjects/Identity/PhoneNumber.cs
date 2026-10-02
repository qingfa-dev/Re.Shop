using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Identity;

/// <summary>A phone number retained after surrounding whitespace is removed.</summary>
/// <param name="Value">The trimmed phone-number text created by <see cref="Create"/>.</param>
public readonly partial record struct PhoneNumber(string? Value)
{
    /// <summary>Creates a phone number within the configured length and character constraints.</summary>
    /// <param name="value">The candidate phone number; surrounding whitespace is removed.</param>
    /// <returns>The normalized number, or a validation error.</returns>
    public static Result<PhoneNumber, Error> Create(string? value)
    {
        var candidate = new PhoneNumber(value?.Trim());

        return PhoneNumberValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="PhoneNumber"/> creation.</summary>
public static class PhoneNumberResult
{
    /// <summary>Contains phone-number validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the phone number is missing.</summary>
        public static Error ValueRequired => Error.Validation(
            code: "PhoneNumber.Value.Required",
            message: "Phone number must be specified.");

        /// <summary>Error returned when the phone number exceeds its maximum length.</summary>
        public static Error ValueTooLong => Error.Validation(
            code: "PhoneNumber.Value.TooLong",
            message:
                $"Phone number cannot exceed {PhoneNumberConstant.Constraints.MaxLength} characters.");

        /// <summary>Error returned when the phone number contains unsupported characters.</summary>
        public static Error ValueInvalid => Error.Validation(
            code: "PhoneNumber.Value.Invalid",
            message: "Phone number format is invalid.");

        #endregion
    }
}

/// <summary>Defines phone-number length and syntax constraints.</summary>
public static class PhoneNumberConstant
{
    /// <summary>Phone-number length limits.</summary>
    public static class Constraints
    {
        /// <summary>Maximum accepted phone-number length.</summary>
        public const int MaxLength = 20;
    }

    /// <summary>Regular expressions used to validate phone numbers.</summary>
    public static class Patterns
    {
        /// <summary>Accepts an optional plus sign followed by digits, spaces, parentheses, or hyphens.</summary>
        public const string Number = @"^\+?[0-9 ()\-]{5,20}$";
    }
}

/// <summary>Validates phone numbers without modifying them.</summary>
public static class PhoneNumberValidator
{
    /// <summary>
    /// Validates a phone number without mutating it.
    /// </summary>
    /// <summary>Returns success when a number is present and matches the configured constraints.</summary>
    /// <param name="phone">The phone number to validate.</param>
    /// <returns>The unchanged value on success, or its validation errors.</returns>
    public static Result<PhoneNumber, Error> Validate(PhoneNumber phone)
    {
        return Result<PhoneNumber, Error>.Success(phone)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(phone.Value),
                error: PhoneNumberResult.Errors.ValueRequired)

            .Ensure(
                predicate: _ =>
                    phone.Value!.Length <=
                    PhoneNumberConstant.Constraints.MaxLength,
                error: PhoneNumberResult.Errors.ValueTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        phone.Value!,
                        PhoneNumberConstant.Patterns.Number),
                error: PhoneNumberResult.Errors.ValueInvalid);
    }
}
