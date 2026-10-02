namespace SharedKernel.Concerns.Foundation.StoreScoped;

public static class StoreScopedResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "store_scoped.entity.required",
            message: "Store-scoped entity is required.");

        public static Error StoreIdRequired => Error.Validation(
            code: "store_scoped.store_id.required",
            message: "Store identifier must be specified.");

        #endregion
    }
}
