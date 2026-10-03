namespace BuildingBlocks.Domain.Concerns.Foundation.Identifiable;

public static class IdentifiableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Identifiable.Entity.Required",
            message: "Identifiable entity is required.");

        public static Error IdRequired => Error.UnprocessableEntity(
            code: "Identifiable.Id.Required",
            message: "Identifier must be specified.");

        #endregion
    }
}
