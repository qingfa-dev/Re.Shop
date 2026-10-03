namespace BuildingBlocks.Domain.Concerns.Content.Localizable;

public static class LocalizableValidator
{
    /// <summary>
    /// Validates the localized value that
    /// <see cref="LocalizableExtensions.SetLocalizedValue{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateLocalizedValue<TValue>(
        TValue auditable,
        string? property,
        string? locale,
        string? value)
        where TValue : ILocalizable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                LocalizableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(property),
                errorValue: LocalizableResult.Failure.PropertyRequired)

            .Ensure(
                predicate: _ =>
                    property!.Length <=
                    LocalizableConstant.Constraints.MaxPropertyLength,
                errorValue: LocalizableResult.Failure.PropertyTooLong)

            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(locale),
                errorValue: LocalizableResult.Failure.LocaleRequired)

            .Ensure(
                predicate: _ =>
                    locale!.Length <=
                    LocalizableConstant.Constraints.MaxLocaleLength,
                errorValue: LocalizableResult.Failure.LocaleTooLong);
    }
}
