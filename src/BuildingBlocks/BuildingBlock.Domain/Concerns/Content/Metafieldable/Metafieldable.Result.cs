namespace BuildingBlocks.Domain.Concerns.Content.Metafieldable;

public static class MetafieldableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Metafieldable.Entity.Required",
            message: "Metafieldable entity is required.");

        public static Error NamespaceRequired => Error.UnprocessableEntity(
            code: "Metafieldable.Namespace.Required",
            message: "Metafield namespace must be specified.");

        public static Error NamespaceTooLong => Error.UnprocessableEntity(
            code: "Metafieldable.Namespace.TooLong",
            message:
                $"Namespace cannot exceed {MetafieldableConstant.Constraints.MaxNamespaceLength} characters.");

        public static Error NamespaceInvalid => Error.UnprocessableEntity(
            code: "Metafieldable.Namespace.Invalid",
            message:
                "Namespace must be snake_case (lowercase letters, digits, underscores).");

        public static Error KeyRequired => Error.UnprocessableEntity(
            code: "Metafieldable.Key.Required",
            message: "Metafield key must be specified.");

        public static Error KeyTooLong => Error.UnprocessableEntity(
            code: "Metafieldable.Key.TooLong",
            message:
                $"Key cannot exceed {MetafieldableConstant.Constraints.MaxKeyLength} characters.");

        public static Error KeyInvalid => Error.UnprocessableEntity(
            code: "Metafieldable.Key.Invalid",
            message:
                "Key must be snake_case (lowercase letters, digits, underscores).");

        public static Error MetafieldNotFound => Error.UnprocessableEntity(
            code: "Metafieldable.Metafield.NotFound",
            message: "Metafield is not present.");

        #endregion
    }
}
