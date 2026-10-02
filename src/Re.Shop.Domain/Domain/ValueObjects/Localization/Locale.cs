using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Localization;

/// <summary>A trimmed, validated locale code.</summary>
/// <param name="Code">The locale code created by <see cref="Create"/>.</param>
public readonly partial record struct Locale(string? Code)
{
    /// <summary>Creates a locale code after trimming and validating its configured syntax.</summary>
    /// <param name="code">The candidate locale code; surrounding whitespace is removed.</param>
    /// <returns>The normalized code, or a validation error.</returns>
    public static Result<Locale, Error> Create(string? code)
    {
        var candidate = new Locale(code?.Trim());

        return LocaleValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="Locale"/> creation.</summary>
public static class LocaleResult
{
    /// <summary>Contains locale-code validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the locale code is missing.</summary>
        public static Error CodeRequired => Error.Validation(
            code: "Locale.Code.Required",
            message: "Locale must be specified.");

        /// <summary>Error returned when the locale code exceeds thirty-five characters.</summary>
        public static Error CodeTooLong => Error.Validation(
            code: "Locale.Code.TooLong",
            message:
                $"Locale cannot exceed {LocaleConstant.Constraints.MaxLength} characters.");

        /// <summary>Error returned when the code does not match the configured locale syntax.</summary>
        public static Error CodeInvalid => Error.Validation(
            code: "Locale.Code.Invalid",
            message:
                "Locale must be a BCP-47 tag such as 'en', 'en-US' or 'zh-Hans-CN'.");

        #endregion
    }
}

/// <summary>Defines locale-code length and syntax constraints.</summary>
public static class LocaleConstant
{
    /// <summary>Locale-code length limits.</summary>
    public static class Constraints
    {
        /// <summary>Maximum accepted locale-code length.</summary>
        public const int MaxLength = 35;
    }

    /// <summary>Regular expressions used to validate locale codes.</summary>
    public static class Patterns
    {
        /// <summary>Accepts the configured language and optional subtags syntax.</summary>
        public const string Code =
            @"^[a-z]{2,3}(?:-[A-Za-z]{4})?(?:-(?:[A-Z]{2}|[0-9]{3}))?$";
    }
}

/// <summary>Validates locale codes without modifying them.</summary>
public static class LocaleValidator
{
    /// <summary>
    /// Validates a locale without mutating it.
    /// </summary>
    /// <summary>Returns success when a code is present and matches the configured syntax and length.</summary>
    /// <param name="locale">The locale code to validate.</param>
    /// <returns>The unchanged value on success, or its validation errors.</returns>
    public static Result<Locale, Error> Validate(Locale locale)
    {
        return Result<Locale, Error>.Success(locale)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(locale.Code),
                error: LocaleResult.Errors.CodeRequired)

            .Ensure(
                predicate: _ =>
                    locale.Code!.Length <=
                    LocaleConstant.Constraints.MaxLength,
                error: LocaleResult.Errors.CodeTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(locale.Code!, LocaleConstant.Patterns.Code),
                error: LocaleResult.Errors.CodeInvalid);
    }
}
