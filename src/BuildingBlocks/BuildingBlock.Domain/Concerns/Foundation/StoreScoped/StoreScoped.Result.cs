namespace BuildingBlocks.Domain.Concerns.Foundation.StoreScoped;

public static class StoreScopedResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "StoreScoped.Entity.Required",
            message: "Store-scoped entity is required.");

        public static Error StoreIdRequired => Error.UnprocessableEntity(
            code: "StoreScoped.StoreId.Required",
            message: "Store identifier must be specified.");

        #endregion
    }
}
