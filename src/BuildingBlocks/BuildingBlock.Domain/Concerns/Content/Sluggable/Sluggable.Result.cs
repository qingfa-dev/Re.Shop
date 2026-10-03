namespace BuildingBlocks.Domain.Concerns.Content.Sluggable;

public static class SluggableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Sluggable.Entity.Required",
            message: "Sluggable entity is required.");

        public static Error SlugRequired => Error.UnprocessableEntity(
            code: "Sluggable.Slug.Required",
            message: "Slug must be specified.");

        public static Error SlugInvalid => Error.UnprocessableEntity(
            code: "Sluggable.Slug.Invalid",
            message:
                "Slug must contain only lowercase letters, digits and single hyphens.");

        public static Error SlugTooLong => Error.UnprocessableEntity(
            code: "Sluggable.Slug.TooLong",
            message:
                $"Slug cannot exceed {SluggableConstant.Constraints.MaxSlugLength} characters.");

        #endregion
    }
}
