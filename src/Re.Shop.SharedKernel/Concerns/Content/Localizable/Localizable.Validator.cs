namespace SharedKernel.Concerns.Content.Localizable;

public static class LocalizableValidator
{
    /// <summary>
    /// Validates the localized value that
    /// <see cref="LocalizableExtensions.SetLocalizedValue{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateLocalizedValue<TValue>(
        TValue auditable,
        string? property,
        string? locale,
        string? value)
        where TValue : ILocalizable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                LocalizableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(property),
                error: LocalizableResult.Failure.PropertyRequired)

            .Ensure(
                predicate: _ =>
                    property!.Length <=
                    LocalizableConstant.Constraints.MaxPropertyLength,
                error: LocalizableResult.Failure.PropertyTooLong)

            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(locale),
                error: LocalizableResult.Failure.LocaleRequired)

            .Ensure(
                predicate: _ =>
                    locale!.Length <=
                    LocalizableConstant.Constraints.MaxLocaleLength,
                error: LocalizableResult.Failure.LocaleTooLong);
    }
}
