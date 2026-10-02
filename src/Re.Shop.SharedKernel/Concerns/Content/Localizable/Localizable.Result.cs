namespace SharedKernel.Concerns.Content.Localizable;

public static class LocalizableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "localizable.entity.required",
            message: "Localizable entity is required.");

        public static Error PropertyRequired => Error.Validation(
            code: "localizable.property.required",
            message: "Localized property must be specified.");

        public static Error PropertyTooLong => Error.Validation(
            code: "localizable.property.too_long",
            message:
                $"Localized property cannot exceed {LocalizableConstant.Constraints.MaxPropertyLength} characters.");

        public static Error LocaleRequired => Error.Validation(
            code: "localizable.locale.required",
            message: "Locale must be specified.");

        public static Error LocaleTooLong => Error.Validation(
            code: "localizable.locale.too_long",
            message:
                $"Locale cannot exceed {LocalizableConstant.Constraints.MaxLocaleLength} characters.");

        #endregion
    }
}
