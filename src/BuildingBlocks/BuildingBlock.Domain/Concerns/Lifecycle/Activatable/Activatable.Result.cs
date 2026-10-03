namespace BuildingBlocks.Domain.Concerns.Lifecycle.Activatable;

public static class ActivatableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Activatable.Entity.Required",
            message: "Activatable entity is required.");

        public static Error ActivatedAtRequired => Error.UnprocessableEntity(
            code: "Activatable.ActivatedAt.Required",
            message: "Activated date must be specified.");

        public static Error AlreadyActive => Error.UnprocessableEntity(
            code: "Activatable.AlreadyActive",
            message: "Entity is already active.");

        public static Error NotActive => Error.UnprocessableEntity(
            code: "Activatable.NotActive",
            message: "Entity is not active, so it cannot be deactivated.");

        public static Error ActivatedByTooLong => Error.UnprocessableEntity(
            code: "Activatable.ActivatedBy.TooLong",
            message:
                $"Activated by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error ActivatedByRequired => Error.UnprocessableEntity(
            code: "Activatable.ActivatedBy.Required",
            message: "Activated by is required.");

        public static Error ActorInvalid => Error.UnprocessableEntity(
            code: "Activatable.Actor.Invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
