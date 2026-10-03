namespace BuildingBlocks.Domain.Concerns.Content.MediaAttachable;

public static class MediaAttachableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "MediaAttachable.Entity.Required",
            message: "Media-attachable entity is required.");

        public static Error MediaDuplicate => Error.UnprocessableEntity(
            code: "MediaAttachable.Media.Duplicate",
            message: "Media is already attached.");

        public static Error MediaNotFound => Error.UnprocessableEntity(
            code: "MediaAttachable.Media.NotFound",
            message: "Media is not attached.");

        #endregion
    }
}
