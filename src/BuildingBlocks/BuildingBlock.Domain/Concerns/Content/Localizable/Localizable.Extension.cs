namespace BuildingBlocks.Domain.Concerns.Content.Localizable;

public static class LocalizableExtensions
{
    /// <summary>
    /// Sets the value for a (property, locale) pair, adding the entry when new.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> SetLocalizedValue<TValue>(
        this Result<TValue> result,
        string? property,
        string? locale,
        string? value)
        where TValue : ILocalizable
    {
        return result
            .Bind(entity =>
                LocalizableValidator.ValidateLocalizedValue(
                    entity,
                    property,
                    locale,
                    value))

            .Tap(entity =>
            {
                var existing = entity.LocalizedValues.FirstOrDefault(entry =>
                    entry.Property == property &&
                    entry.Locale == locale);

                if (existing is null)
                {
                    entity.LocalizedValues.Add(new LocalizedValue
                    {
                        Property = property!,
                        Locale = locale!,
                        Value = value
                    });
                }
                else
                {
                    existing.Value = value;
                }
            });
    }
}
