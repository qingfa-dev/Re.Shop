namespace SharedKernel.Concerns.Organization.Hierarchical;

public static class HierarchicalResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "hierarchical.entity.required",
            message: "Hierarchical entity is required.");

        #endregion
    }
}
