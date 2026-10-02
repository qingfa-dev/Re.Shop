namespace SharedKernel.Concerns.Organization.Collectionable;

public static class CollectionableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "collectionable.entity.required",
            message: "Collectionable entity is required.");

        public static Error CollectionDuplicate => Error.Validation(
            code: "collectionable.collection.duplicate",
            message: "Collection is already present.");

        public static Error CollectionNotFound => Error.Validation(
            code: "collectionable.collection.not_found",
            message: "Collection is not present.");

        #endregion
    }
}
