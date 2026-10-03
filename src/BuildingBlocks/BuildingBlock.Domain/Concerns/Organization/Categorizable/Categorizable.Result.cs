namespace BuildingBlocks.Domain.Concerns.Organization.Categorizable;

public static class CategorizableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Categorizable.Entity.Required",
            message: "Categorizable entity is required.");

        public static Error CategoryDuplicate => Error.UnprocessableEntity(
            code: "Categorizable.Category.Duplicate",
            message: "Category is already assigned.");

        public static Error CategoryNotFound => Error.UnprocessableEntity(
            code: "Categorizable.Category.NotFound",
            message: "Category is not assigned.");

        #endregion
    }
}
