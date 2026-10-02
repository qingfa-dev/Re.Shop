using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Money;

/// <summary>An amount denominated in a currency.</summary>
/// <param name="Amount">The monetary amount.</param>
/// <param name="Currency">The currency in which the amount is denominated.</param>
public readonly partial record struct Money(decimal Amount, Currency Currency)
{
    /// <summary>Creates a money value when its currency code is valid.</summary>
    /// <param name="amount">The amount to retain.</param>
    /// <param name="currency">The currency to validate.</param>
    /// <returns>The money value, or a currency validation error.</returns>
    public static Result<Money, Error> Create(
        decimal amount,
        Currency currency)
    {
        var candidate = new Money(amount, currency);

        return MoneyValidator.Validate(candidate);
    }
}

/// <summary>Groups validation and arithmetic errors for <see cref="Money"/>.</summary>
public static class MoneyResult
{
    /// <summary>Contains money validation and operation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the currency code is invalid.</summary>
        public static Error CurrencyInvalid => Error.Validation(
            code: "Money.Currency.Invalid",
            message: "Money currency must be a valid ISO-4217 code.");

        /// <summary>Error returned when arithmetic operands use different currencies.</summary>
        public static Error CurrencyMismatch => Error.Validation(
            code: "Money.Currency.Mismatch",
            message: "Currencies must match for this operation.");

        #endregion
    }
}

/// <summary>Validates money values without changing their amount or currency.</summary>
public static class MoneyValidator
{
    /// <summary>
    /// Validates a money value without mutating it.
    /// </summary>
    /// <summary>Returns success when the currency code is valid.</summary>
    /// <param name="money">The money value to validate.</param>
    /// <returns>The unchanged value on success, or its validation error.</returns>
    public static Result<Money, Error> Validate(Money money)
    {
        return Result<Money, Error>.Success(money)
            .Ensure(
                predicate: _ =>
                    !string.IsNullOrWhiteSpace(money.Currency.Code) &&
                    Regex.IsMatch(
                        money.Currency.Code!,
                        CurrencyConstant.Patterns.Code),
                error: MoneyResult.Errors.CurrencyInvalid);
    }
}

/// <summary>Provides currency-aware arithmetic for <see cref="Money"/> values.</summary>
public static class MoneyExtensions
{
    /// <summary>
    /// Adds two money values of the same currency.
    /// </summary>
    /// <summary>Adds two money values with the same currency.</summary>
    /// <param name="left">The left operand and result currency.</param>
    /// <param name="right">The amount to add.</param>
    /// <returns>The sum, or a currency-mismatch error.</returns>
    public static Result<Money, Error> Add(
        this Money left,
        Money right)
    {
        return Result<Money, Error>.Success(left)
            .Ensure(
                predicate: _ => left.Currency.Code == right.Currency.Code,
                error: MoneyResult.Errors.CurrencyMismatch)

            .Map(money =>
                new Money(money.Amount + right.Amount, money.Currency));
    }

    /// <summary>
    /// Subtracts two money values of the same currency.
    /// </summary>
    /// <summary>Subtracts the right operand from the left when currencies match.</summary>
    /// <param name="left">The minuend and result currency.</param>
    /// <param name="right">The amount to subtract.</param>
    /// <returns>The difference, or a currency-mismatch error.</returns>
    public static Result<Money, Error> Subtract(
        this Money left,
        Money right)
    {
        return Result<Money, Error>.Success(left)
            .Ensure(
                predicate: _ => left.Currency.Code == right.Currency.Code,
                error: MoneyResult.Errors.CurrencyMismatch)

            .Map(money =>
                new Money(money.Amount - right.Amount, money.Currency));
    }

    /// <summary>
    /// Scales the money value; the currency is preserved.
    /// </summary>
    /// <summary>Scales an amount while retaining its currency.</summary>
    /// <param name="money">The amount to scale.</param>
    /// <param name="factor">The multiplicative factor.</param>
    /// <returns>The scaled money value.</returns>
    public static Money Multiply(this Money money, decimal factor)
    {
        return new Money(money.Amount * factor, money.Currency);
    }
}
