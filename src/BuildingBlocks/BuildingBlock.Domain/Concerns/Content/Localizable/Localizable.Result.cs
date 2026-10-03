namespace BuildingBlocks.Domain.Concerns.Content.Localizable;

public static class LocalizableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Localizable.Entity.Required",
            message: "Localizable entity is required.");

        public static Error PropertyRequired => Error.UnprocessableEntity(
            code: "Localizable.Property.Required",
            message: "Localized property must be specified.");

        public static Error PropertyTooLong => Error.UnprocessableEntity(
            code: "Localizable.Property.TooLong",
            message:
                $"Localized property cannot exceed {LocalizableConstant.Constraints.MaxPropertyLength} characters.");

        public static Error LocaleRequired => Error.UnprocessableEntity(
            code: "Localizable.Locale.Required",
            message: "Locale must be specified.");

        public static Error LocaleTooLong => Error.UnprocessableEntity(
            code: "Localizable.Locale.TooLong",
            message:
                $"Locale cannot exceed {LocalizableConstant.Constraints.MaxLocaleLength} characters.");

        #endregion
    }
}
