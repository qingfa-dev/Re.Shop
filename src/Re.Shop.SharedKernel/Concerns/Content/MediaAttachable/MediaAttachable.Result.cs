namespace SharedKernel.Concerns.Content.MediaAttachable;

public static class MediaAttachableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "media_attachable.entity.required",
            message: "Media-attachable entity is required.");

        public static Error MediaDuplicate => Error.Validation(
            code: "media_attachable.media.duplicate",
            message: "Media is already present.");

        public static Error MediaNotFound => Error.Validation(
            code: "media_attachable.media.not_found",
            message: "Media is not present.");

        #endregion
    }
}
