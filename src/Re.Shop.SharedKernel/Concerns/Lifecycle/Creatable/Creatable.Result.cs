namespace SharedKernel.Concerns.Lifecycle.Creatable;

public static class CreatableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "creatable.entity.required",
            message: "Creatable entity is required.");

        public static Error CreatedAtRequired => Error.Validation(
            code: "creatable.created_at.required",
            message: "Created date must be specified.");

        public static Error AlreadyInitialized => Error.Validation(
            code: "creatable.created_at.already_initialized",
            message:
                "Creation audit metadata is already set; it cannot be initialized twice.");

        public static Error CreatedByTooLong => Error.Validation(
            code: "creatable.created_by.too_long",
            message:
                $"Created by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error CreatedByRequired => Error.Validation(
            code: "creatable.created_by.required",
            message: "Created by must be specified.");

        public static Error ActorInvalid => Error.Validation(
            code: "creatable.actor.invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
