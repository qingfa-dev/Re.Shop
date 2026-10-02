using System.Text.RegularExpressions;

namespace BuildingBlocks.Core.ValueObjects.Address;

/// <summary>A postal address with a required first line, city, postal code, and country code.</summary>
/// <param name="Line1">The primary street address line; surrounding whitespace is removed by <see cref="Create"/>.</param>
/// <param name="Line2">An optional secondary address line.</param>
/// <param name="City">The city or locality.</param>
/// <param name="Region">The optional region or subdivision.</param>
/// <param name="PostalCode">The postal code value.</param>
/// <param name="CountryCode">The two-letter country code, normalized to uppercase by <see cref="Create"/>.</param>
public readonly partial record struct Address(
    string? Line1,
    string? Line2,
    string? City,
    string? Region,
    PostalCode PostalCode,
    string? CountryCode)
{
    /// <summary>Creates an address after validating its required fields and country/postal codes.</summary>
    /// <param name="line1">The required primary street address line.</param>
    /// <param name="line2">The optional secondary address line.</param>
    /// <param name="city">The required city or locality.</param>
    /// <param name="region">The optional region or subdivision.</param>
    /// <param name="postalCode">The postal code to validate.</param>
    /// <param name="countryCode">The two-letter country code; trimmed and uppercased.</param>
    /// <returns>The normalized address, or the validation errors.</returns>
    public static Result<Address, Error> Create(
        string? line1,
        string? line2,
        string? city,
        string? region,
        PostalCode postalCode,
        string? countryCode)
    {
        var candidate = new Address(
            line1?.Trim(),
            line2?.Trim(),
            city?.Trim(),
            region?.Trim(),
            postalCode,
            countryCode?.Trim().ToUpperInvariant());

        return AddressValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="Address"/> creation.</summary>
public static class AddressResult
{
    /// <summary>Contains address validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the primary address line is missing.</summary>
        public static Error Line1Required => Error.Validation(
            code: "Address.Line1.Required",
            message: "Address line 1 must be specified.");

        /// <summary>Error returned when the primary address line exceeds its limit.</summary>
        public static Error Line1TooLong => Error.Validation(
            code: "Address.Line1.TooLong",
            message:
                $"Address line 1 cannot exceed {AddressConstant.Constraints.MaxLineLength} characters.");

        /// <summary>Error returned when the secondary address line exceeds its limit.</summary>
        public static Error Line2TooLong => Error.Validation(
            code: "Address.Line2.TooLong",
            message:
                $"Address line 2 cannot exceed {AddressConstant.Constraints.MaxLineLength} characters.");

        /// <summary>Error returned when the city is missing.</summary>
        public static Error CityRequired => Error.Validation(
            code: "Address.City.Required",
            message: "City must be specified.");

        /// <summary>Error returned when the city exceeds its limit.</summary>
        public static Error CityTooLong => Error.Validation(
            code: "Address.City.TooLong",
            message:
                $"City cannot exceed {AddressConstant.Constraints.MaxCityLength} characters.");

        /// <summary>Error returned when the region exceeds its limit.</summary>
        public static Error RegionTooLong => Error.Validation(
            code: "Address.Region.TooLong",
            message:
                $"Region cannot exceed {AddressConstant.Constraints.MaxRegionLength} characters.");

        /// <summary>Error returned when the postal code is invalid.</summary>
        public static Error PostalCodeInvalid => Error.Validation(
            code: "Address.PostalCode.Invalid",
            message: "Postal code is invalid.");

        /// <summary>Error returned when the country code is not two uppercase letters.</summary>
        public static Error CountryCodeInvalid => Error.Validation(
            code: "Address.CountryCode.Invalid",
            message: "Country code must be a 2-letter ISO-3166 code.");

        #endregion
    }
}

/// <summary>Defines address field limits and accepted country-code syntax.</summary>
public static class AddressConstant
{
    /// <summary>Maximum accepted address field lengths.</summary>
    public static class Constraints
    {
        /// <summary>Maximum length of either street address line.</summary>
        public const int MaxLineLength = 256;
        /// <summary>Maximum length of a city name.</summary>
        public const int MaxCityLength = 128;
        /// <summary>Maximum length of a region name.</summary>
        public const int MaxRegionLength = 128;
    }

    /// <summary>Regular expressions used to validate address codes.</summary>
    public static class Patterns
    {
        /// <summary>Accepts exactly two uppercase ASCII letters.</summary>
        public const string CountryCode = @"^[A-Z]{2}$";
    }
}

/// <summary>Validates address fields without modifying the supplied value.</summary>
public static class AddressValidator
{
    /// <summary>
    /// Validates an address without mutating it.
    /// </summary>
    /// <summary>Returns success when every address field satisfies its constraints.</summary>
    /// <param name="address">The address to validate.</param>
    /// <returns>The unchanged address on success, or its validation errors.</returns>
    public static Result<Address, Error> Validate(Address address)
    {
        return Result<Address, Error>.Success(address)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(address.Line1),
                error: AddressResult.Errors.Line1Required)

            .Ensure(
                predicate: _ =>
                    address.Line1!.Length <=
                    AddressConstant.Constraints.MaxLineLength,
                error: AddressResult.Errors.Line1TooLong)

            .Ensure(
                predicate: _ =>
                    address.Line2 is null ||
                    address.Line2.Length <=
                    AddressConstant.Constraints.MaxLineLength,
                error: AddressResult.Errors.Line2TooLong)

            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(address.City),
                error: AddressResult.Errors.CityRequired)

            .Ensure(
                predicate: _ =>
                    address.City!.Length <=
                    AddressConstant.Constraints.MaxCityLength,
                error: AddressResult.Errors.CityTooLong)

            .Ensure(
                predicate: _ =>
                    address.Region is null ||
                    address.Region.Length <=
                    AddressConstant.Constraints.MaxRegionLength,
                error: AddressResult.Errors.RegionTooLong)

            .Ensure(
                predicate: _ =>
                    !string.IsNullOrWhiteSpace(address.PostalCode.Value) &&
                    Regex.IsMatch(
                        address.PostalCode.Value!,
                        PostalCodeConstant.Patterns.Code),
                error: AddressResult.Errors.PostalCodeInvalid)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        address.CountryCode ?? string.Empty,
                        AddressConstant.Patterns.CountryCode),
                error: AddressResult.Errors.CountryCodeInvalid);
    }
}
