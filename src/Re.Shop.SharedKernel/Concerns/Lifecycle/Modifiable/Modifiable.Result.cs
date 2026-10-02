namespace SharedKernel.Concerns.Lifecycle.Modifiable;

public static class ModifiableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "modifiable.entity.required",
            message: "Modifiable entity is required.");

        public static Error ModifiedAtRequired => Error.Validation(
            code: "modifiable.modified_at.required",
            message: "Modified date must be specified.");

        public static Error NotInitialized => Error.Validation(
            code: "modifiable.state.not_initialized",
            message:
                "Entity has no creation audit metadata; initialize it before marking it modified.");

        public static Error ModifiedAtBeforeCreatedAt => Error.Validation(
            code: "modifiable.modified_at.invalid_range",
            message: "Modified date cannot be earlier than created date.");

        public static Error ModifiedByTooLong => Error.Validation(
            code: "modifiable.modified_by.too_long",
            message:
                $"Modified by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error ModifiedByRequired => Error.Validation(
            code: "modifiable.modified_by.required",
            message: "Modified by must be specified.");

        public static Error ActorInvalid => Error.Validation(
            code: "modifiable.actor.invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
