namespace BuildingBlocks.Domain.Concerns.Lifecycle.Creatable;

public static class CreatableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Creatable.Entity.Required",
            message: "Creatable entity is required.");

        public static Error CreatedAtRequired => Error.UnprocessableEntity(
            code: "Creatable.CreatedAt.Required",
            message: "Created date must be specified.");

        public static Error AlreadyInitialized => Error.UnprocessableEntity(
            code: "Creatable.CreatedAt.AlreadyInitialized",
            message:
                "Creation audit metadata is already set; it cannot be initialized twice.");

        public static Error CreatedByTooLong => Error.UnprocessableEntity(
            code: "Creatable.CreatedBy.TooLong",
            message:
                $"Created by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error CreatedByRequired => Error.UnprocessableEntity(
            code: "Creatable.CreatedBy.Required",
            message: "Created by is required.");

        public static Error ActorInvalid => Error.UnprocessableEntity(
            code: "Creatable.Actor.Invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
