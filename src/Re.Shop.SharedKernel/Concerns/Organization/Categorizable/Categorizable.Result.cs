namespace SharedKernel.Concerns.Organization.Categorizable;

public static class CategorizableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "categorizable.entity.required",
            message: "Categorizable entity is required.");

        public static Error CategoryDuplicate => Error.Validation(
            code: "categorizable.category.duplicate",
            message: "Category is already present.");

        public static Error CategoryNotFound => Error.Validation(
            code: "categorizable.category.not_found",
            message: "Category is not present.");

        #endregion
    }
}
