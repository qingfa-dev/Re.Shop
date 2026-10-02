namespace SharedKernel.Concerns.Content.Taggable;

public static class TaggableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "taggable.entity.required",
            message: "Taggable entity is required.");

        public static Error TagRequired => Error.Validation(
            code: "taggable.tag.required",
            message: "Tag must be specified.");

        public static Error TagTooLong => Error.Validation(
            code: "taggable.tag.too_long",
            message:
                $"Tag cannot exceed {TaggableConstant.Constraints.MaxTagLength} characters.");

        public static Error TagDuplicate => Error.Validation(
            code: "taggable.tag.duplicate",
            message: "Tag is already present.");

        public static Error TagNotFound => Error.Validation(
            code: "taggable.tag.not_found",
            message: "Tag is not present.");

        #endregion
    }
}
