namespace BuildingBlocks.Domain.Concerns.Content.Taggable;

public static class TaggableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Taggable.Entity.Required",
            message: "Taggable entity is required.");

        public static Error TagRequired => Error.UnprocessableEntity(
            code: "Taggable.Tag.Required",
            message: "Tag must be specified.");

        public static Error TagTooLong => Error.UnprocessableEntity(
            code: "Taggable.Tag.TooLong",
            message:
                $"Tag cannot exceed {TaggableConstant.Constraints.MaxTagLength} characters.");

        public static Error TagDuplicate => Error.UnprocessableEntity(
            code: "Taggable.Tag.Duplicate",
            message: "Tag is already present.");

        public static Error TagNotFound => Error.UnprocessableEntity(
            code: "Taggable.Tag.NotFound",
            message: "Tag is not present.");

        #endregion
    }
}
