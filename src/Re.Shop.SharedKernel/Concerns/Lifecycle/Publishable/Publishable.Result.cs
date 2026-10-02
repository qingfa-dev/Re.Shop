namespace SharedKernel.Concerns.Lifecycle.Publishable;

public static class PublishableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "publishable.entity.required",
            message: "Publishable entity is required.");

        public static Error PublishedAtRequired => Error.Validation(
            code: "publishable.published_at.required",
            message: "Published date must be specified.");

        public static Error AlreadyPublished => Error.Validation(
            code: "publishable.state.already_published",
            message: "Entity is already published.");

        public static Error NotPublished => Error.Validation(
            code: "publishable.state.not_published",
            message: "Entity is not published, so it cannot be unpublished.");

        public static Error PublishedByTooLong => Error.Validation(
            code: "publishable.published_by.too_long",
            message:
                $"Published by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error PublishedByRequired => Error.Validation(
            code: "publishable.published_by.required",
            message: "Published by must be specified.");

        public static Error ActorInvalid => Error.Validation(
            code: "publishable.actor.invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
