using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Identity;

/// <summary>An email address normalized by trimming and lowercasing its value.</summary>
/// <param name="Value">The normalized address created by <see cref="Create"/>.</param>
public readonly partial record struct EmailAddress(string? Value)
{
    /// <summary>Creates an email address using the Domain's configured syntax and length checks.</summary>
    /// <param name="value">The candidate address; surrounding whitespace is removed and text is lowercased.</param>
    /// <returns>The normalized address, or a validation error.</returns>
    public static Result<EmailAddress, Error> Create(string? value)
    {
        var candidate = new EmailAddress(value?.Trim().ToLowerInvariant());

        return EmailAddressValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="EmailAddress"/> creation.</summary>
public static class EmailAddressResult
{
    /// <summary>Contains email-address validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the address is missing.</summary>
        public static Error ValueRequired => Error.Validation(
            code: "EmailAddress.Value.Required",
            message: "Email address must be specified.");

        /// <summary>Error returned when the address exceeds its maximum length.</summary>
        public static Error ValueTooLong => Error.Validation(
            code: "EmailAddress.Value.TooLong",
            message:
                $"Email address cannot exceed {EmailAddressConstant.Constraints.MaxLength} characters.");

        /// <summary>Error returned when the address does not match the configured syntax.</summary>
        public static Error ValueInvalid => Error.Validation(
            code: "EmailAddress.Value.Invalid",
            message: "Email address format is invalid.");

        #endregion
    }
}

/// <summary>Defines email-address length and syntax constraints.</summary>
public static class EmailAddressConstant
{
    /// <summary>Email-address length limits.</summary>
    public static class Constraints
    {
        /// <summary>Maximum accepted address length.</summary>
        public const int MaxLength = 254;
    }

    /// <summary>Regular expressions used to validate email addresses.</summary>
    public static class Patterns
    {
        /// <summary>Accepts a non-whitespace local and domain part separated by an at sign and a dotted alphabetic suffix.</summary>
        public const string Address = @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$";
    }
}

/// <summary>Validates email addresses without modifying them.</summary>
public static class EmailAddressValidator
{
    /// <summary>
    /// Validates an email address without mutating it.
    /// </summary>
    /// <summary>Returns success when the address is present, within the length limit, and matches the configured pattern.</summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>The unchanged address on success, or its validation errors.</returns>
    public static Result<EmailAddress, Error> Validate(EmailAddress email)
    {
        return Result<EmailAddress, Error>.Success(email)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(email.Value),
                error: EmailAddressResult.Errors.ValueRequired)

            .Ensure(
                predicate: _ =>
                    email.Value!.Length <=
                    EmailAddressConstant.Constraints.MaxLength,
                error: EmailAddressResult.Errors.ValueTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        email.Value!,
                        EmailAddressConstant.Patterns.Address),
                error: EmailAddressResult.Errors.ValueInvalid);
    }
}
