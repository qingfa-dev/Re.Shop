namespace SharedKernel.Concerns.Lifecycle.SoftDeletable;

public static class SoftDeletableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "soft_deletable.entity.required",
            message: "Soft-deletable entity is required.");

        public static Error DeletedAtRequired => Error.Validation(
            code: "soft_deletable.deleted_at.required",
            message: "Deleted date must be specified.");

        public static Error AlreadyDeleted => Error.Validation(
            code: "soft_deletable.state.already_deleted",
            message: "Entity is already deleted.");

        public static Error NotDeleted => Error.Validation(
            code: "soft_deletable.state.not_deleted",
            message: "Entity is not deleted, so it cannot be restored.");

        public static Error DeletedByTooLong => Error.Validation(
            code: "soft_deletable.deleted_by.too_long",
            message:
                $"Deleted by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error DeletedByRequired => Error.Validation(
            code: "soft_deletable.deleted_by.required",
            message: "Deleted by must be specified.");

        public static Error ActorInvalid => Error.Validation(
            code: "soft_deletable.actor.invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
