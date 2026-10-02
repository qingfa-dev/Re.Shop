namespace SharedKernel.Concerns.Lifecycle.Activatable;

public static class ActivatableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "activatable.entity.required",
            message: "Activatable entity is required.");

        public static Error ActivatedAtRequired => Error.Validation(
            code: "activatable.activated_at.required",
            message: "Activated date must be specified.");

        public static Error AlreadyActive => Error.Validation(
            code: "activatable.state.already_active",
            message: "Entity is already active.");

        public static Error NotActive => Error.Validation(
            code: "activatable.state.not_active",
            message: "Entity is not active, so it cannot be deactivated.");

        public static Error ActivatedByTooLong => Error.Validation(
            code: "activatable.activated_by.too_long",
            message:
                $"Activated by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error ActivatedByRequired => Error.Validation(
            code: "activatable.activated_by.required",
            message: "Activated by must be specified.");

        public static Error ActorInvalid => Error.Validation(
            code: "activatable.actor.invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
