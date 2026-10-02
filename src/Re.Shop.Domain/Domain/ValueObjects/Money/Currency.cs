using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Money;

/// <summary>A currency code normalized to uppercase.</summary>
/// <param name="Code">The three-letter uppercase code created by <see cref="Create"/>.</param>
public readonly partial record struct Currency(string? Code)
{
    /// <summary>Creates a currency after trimming, uppercasing, and validating its code.</summary>
    /// <param name="code">The candidate currency code.</param>
    /// <returns>The normalized currency, or a validation error.</returns>
    public static Result<Currency, Error> Create(string? code)
    {
        var candidate = new Currency(code?.Trim().ToUpperInvariant());

        return CurrencyValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="Currency"/> creation.</summary>
public static class CurrencyResult
{
    /// <summary>Contains currency validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the currency code is missing.</summary>
        public static Error CodeRequired => Error.Validation(
            code: "Currency.Code.Required",
            message: "Currency code must be specified.");

        /// <summary>Error returned when the code is not three uppercase letters.</summary>
        public static Error CodeInvalid => Error.Validation(
            code: "Currency.Code.Invalid",
            message: "Currency code must be a 3-letter ISO-4217 code.");

        #endregion
    }
}

/// <summary>Defines syntax constraints for currency codes.</summary>
public static class CurrencyConstant
{
    /// <summary>Regular expressions used to validate currency codes.</summary>
    public static class Patterns
    {
        /// <summary>Accepts exactly three uppercase ASCII letters.</summary>
        public const string Code = @"^[A-Z]{3}$";
    }
}

/// <summary>Validates currency codes without modifying the supplied value.</summary>
public static class CurrencyValidator
{
    /// <summary>
    /// Validates a currency without mutating it.
    /// </summary>
    /// <summary>Returns success when the code contains exactly three uppercase letters.</summary>
    /// <param name="currency">The currency to validate.</param>
    /// <returns>The unchanged currency on success, or its validation errors.</returns>
    public static Result<Currency, Error> Validate(Currency currency)
    {
        return Result<Currency, Error>.Success(currency)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(currency.Code),
                error: CurrencyResult.Errors.CodeRequired)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        currency.Code!,
                        CurrencyConstant.Patterns.Code),
                error: CurrencyResult.Errors.CodeInvalid);
    }
}
