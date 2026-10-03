namespace BuildingBlocks.Domain.Concerns.Lifecycle.Publishable;

public static class PublishableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Publishable.Entity.Required",
            message: "Publishable entity is required.");

        public static Error PublishedAtRequired => Error.UnprocessableEntity(
            code: "Publishable.PublishedAt.Required",
            message: "Published date must be specified.");

        public static Error AlreadyPublished => Error.UnprocessableEntity(
            code: "Publishable.AlreadyPublished",
            message: "Entity is already published.");

        public static Error NotPublished => Error.UnprocessableEntity(
            code: "Publishable.NotPublished",
            message: "Entity is not published, so it cannot be unpublished.");

        public static Error PublishedByTooLong => Error.UnprocessableEntity(
            code: "Publishable.PublishedBy.TooLong",
            message:
                $"Published by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error PublishedByRequired => Error.UnprocessableEntity(
            code: "Publishable.PublishedBy.Required",
            message: "Published by is required.");

        public static Error ActorInvalid => Error.UnprocessableEntity(
            code: "Publishable.Actor.Invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
