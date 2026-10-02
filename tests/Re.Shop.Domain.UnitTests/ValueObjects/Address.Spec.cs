using BuildingBlocks.Core.ValueObjects.Address;

namespace Domain.UnitTests.ValueObjects;

public class AddressSpec
{
    [Fact]
    public void Create_ValidInput_ShouldTrimFieldsAndNormalizeCountryCode()
    {
        var postalCode = PostalCode.Create("M5V 2T6").Value;

        var result = Address.Create(
            " 12 Main St ",
            null,
            " Toronto ",
            " ON ",
            postalCode,
            "ca");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Line1.ShouldBe("12 Main St");
        result.Value.City.ShouldBe("Toronto");
        result.Value.Region.ShouldBe("ON");
        result.Value.CountryCode.ShouldBe("CA");
    }

    [Fact]
    public void Create_MissingLine1_ShouldFailWithLine1Required()
    {
        var postalCode = PostalCode.Create("M5V 2T6").Value;

        var result = Address.Create(
            null,
            null,
            "Toronto",
            null,
            postalCode,
            "CA");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Address.Line1.Required");
    }

    [Fact]
    public void RecordEquality_ShouldCompareAllAddressComponents()
    {
        var postalCode = PostalCode.Create("M5V 2T6").Value;
        var address = Address.Create("12 Main St", null, "Toronto", "ON", postalCode, "CA").Value;
        var equivalent = Address.Create("12 Main St", null, "Toronto", "ON", postalCode, "CA").Value;
        var changed = Address.Create("12 Main St", null, "Toronto", "ON", postalCode, "US").Value;

        address.ShouldBe(equivalent);
        address.ShouldNotBe(changed);
    }
}
