using BuildingBlocks.Core.ValueObjects.Money;

namespace Domain.UnitTests.ValueObjects;

public class CurrencySpec
{
    [Fact]
    public void Create_ValidCode_ShouldTrimAndUppercaseCode()
    {
        var result = Currency.Create(" usd ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Code.ShouldBe("USD");
    }

    [Fact]
    public void Create_MissingCode_ShouldFailWithCodeRequired()
    {
        var result = Currency.Create(null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Currency.Code.Required");
    }
}
