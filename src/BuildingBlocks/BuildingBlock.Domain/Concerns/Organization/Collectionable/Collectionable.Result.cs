namespace BuildingBlocks.Domain.Concerns.Organization.Collectionable;

public static class CollectionableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Collectionable.Entity.Required",
            message: "Collectionable entity is required.");

        public static Error CollectionDuplicate => Error.UnprocessableEntity(
            code: "Collectionable.Collection.Duplicate",
            message: "Collection is already assigned.");

        public static Error CollectionNotFound => Error.UnprocessableEntity(
            code: "Collectionable.Collection.NotFound",
            message: "Collection is not assigned.");

        #endregion
    }
}
