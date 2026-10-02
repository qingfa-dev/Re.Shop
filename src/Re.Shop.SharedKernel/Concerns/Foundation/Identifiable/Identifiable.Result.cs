namespace SharedKernel.Concerns.Foundation.Identifiable;

public static class IdentifiableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "identifiable.entity.required",
            message: "Identifiable entity is required.");

        public static Error IdRequired => Error.Validation(
            code: "identifiable.id.required",
            message: "Identifier must be specified.");

        #endregion
    }
}
