using BuildingBlocks.Core.ValueObjects.Money;

namespace Domain.UnitTests.ValueObjects;

public class MoneySpec
{
    [Fact]
    public void Create_ValidCurrency_ShouldSucceed()
    {
        var currency = Currency.Create("USD").Value;

        var result = Money.Create(12.5m, currency);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(12.5m);
        result.Value.Currency.ShouldBe(currency);
    }

    [Fact]
    public void Create_InvalidCurrency_ShouldFailWithCurrencyInvalid()
    {
        var result = Money.Create(12.5m, default);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Money.Currency.Invalid");
    }
}
