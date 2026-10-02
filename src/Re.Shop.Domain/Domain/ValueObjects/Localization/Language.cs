using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Localization;

/// <summary>A trimmed, validated language code using two or three lowercase letters.</summary>
/// <param name="Code">The language code created by <see cref="Create"/>.</param>
public readonly partial record struct Language(string? Code)
{
    /// <summary>Creates a language code after trimming and validating its syntax.</summary>
    /// <param name="code">The candidate language code; surrounding whitespace is removed.</param>
    /// <returns>The normalized code, or a validation error.</returns>
    public static Result<Language, Error> Create(string? code)
    {
        var candidate = new Language(code?.Trim());

        return LanguageValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="Language"/> creation.</summary>
public static class LanguageResult
{
    /// <summary>Contains language-code validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the language code is missing.</summary>
        public static Error CodeRequired => Error.Validation(
            code: "Language.Code.Required",
            message: "Language must be specified.");

        /// <summary>Error returned when the language code exceeds the configured maximum length.</summary>
        public static Error CodeTooLong => Error.Validation(
            code: "Language.Code.TooLong",
            message:
                $"Language cannot exceed {LanguageConstant.Constraints.MaxLength} characters.");

        /// <summary>Error returned when the code is not two or three lowercase letters.</summary>
        public static Error CodeInvalid => Error.Validation(
            code: "Language.Code.Invalid",
            message:
                "Language must be a lowercase ISO-639 subtag such as 'en' or 'fra'.");

        #endregion
    }
}

/// <summary>Defines language-code length and syntax constraints.</summary>
public static class LanguageConstant
{
    /// <summary>Language-code length limits.</summary>
    public static class Constraints
    {
        /// <summary>Maximum accepted language-code length.</summary>
        public const int MaxLength = 8;
    }

    /// <summary>Regular expressions used to validate language codes.</summary>
    public static class Patterns
    {
        /// <summary>Accepts two or three lowercase ASCII letters.</summary>
        public const string Code = @"^[a-z]{2,3}$";
    }
}

/// <summary>Validates language codes without modifying them.</summary>
public static class LanguageValidator
{
    /// <summary>
    /// Validates a language subtag without mutating it.
    /// </summary>
    /// <summary>Returns success when a code is present and matches the configured syntax and length.</summary>
    /// <param name="language">The language code to validate.</param>
    /// <returns>The unchanged value on success, or its validation errors.</returns>
    public static Result<Language, Error> Validate(Language language)
    {
        return Result<Language, Error>.Success(language)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(language.Code),
                error: LanguageResult.Errors.CodeRequired)

            .Ensure(
                predicate: _ =>
                    language.Code!.Length <=
                    LanguageConstant.Constraints.MaxLength,
                error: LanguageResult.Errors.CodeTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        language.Code!,
                        LanguageConstant.Patterns.Code),
                error: LanguageResult.Errors.CodeInvalid);
    }
}
