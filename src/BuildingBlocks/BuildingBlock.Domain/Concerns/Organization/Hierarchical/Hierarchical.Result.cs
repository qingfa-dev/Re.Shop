namespace BuildingBlocks.Domain.Concerns.Organization.Hierarchical;

public static class HierarchicalResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Hierarchical.Entity.Required",
            message: "Hierarchical entity is required.");

        #endregion
    }
}
