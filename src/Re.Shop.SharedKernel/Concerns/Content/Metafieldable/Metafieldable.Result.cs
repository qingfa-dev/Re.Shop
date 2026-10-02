namespace SharedKernel.Concerns.Content.Metafieldable;

public static class MetafieldableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "metafieldable.entity.required",
            message: "Metafieldable entity is required.");

        public static Error NamespaceRequired => Error.Validation(
            code: "metafieldable.namespace.required",
            message: "Metafield namespace must be specified.");

        public static Error NamespaceTooLong => Error.Validation(
            code: "metafieldable.namespace.too_long",
            message:
                $"Namespace cannot exceed {MetafieldableConstant.Constraints.MaxNamespaceLength} characters.");

        public static Error NamespaceInvalid => Error.Validation(
            code: "metafieldable.namespace.invalid",
            message:
                "Namespace must be snake_case (lowercase letters, digits, underscores).");

        public static Error KeyRequired => Error.Validation(
            code: "metafieldable.key.required",
            message: "Metafield key must be specified.");

        public static Error KeyTooLong => Error.Validation(
            code: "metafieldable.key.too_long",
            message:
                $"Key cannot exceed {MetafieldableConstant.Constraints.MaxKeyLength} characters.");

        public static Error KeyInvalid => Error.Validation(
            code: "metafieldable.key.invalid",
            message:
                "Key must be snake_case (lowercase letters, digits, underscores).");

        public static Error MetafieldNotFound => Error.Validation(
            code: "metafieldable.metafield.not_found",
            message: "Metafield is not present.");

        #endregion
    }
}
