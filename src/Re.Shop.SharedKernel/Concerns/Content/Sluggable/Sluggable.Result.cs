namespace SharedKernel.Concerns.Content.Sluggable;

public static class SluggableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "sluggable.entity.required",
            message: "Sluggable entity is required.");

        public static Error SlugRequired => Error.Validation(
            code: "sluggable.slug.required",
            message: "Slug must be specified.");

        public static Error SlugTooLong => Error.Validation(
            code: "sluggable.slug.too_long",
            message:
                $"Slug cannot exceed {SluggableConstant.Constraints.MaxSlugLength} characters.");

        public static Error SlugInvalid => Error.Validation(
            code: "sluggable.slug.invalid",
            message:
                "Slug must contain only lowercase letters, digits and single hyphens.");

        #endregion
    }
}
