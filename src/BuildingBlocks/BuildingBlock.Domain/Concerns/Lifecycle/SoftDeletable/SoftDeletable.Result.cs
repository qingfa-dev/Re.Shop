namespace BuildingBlocks.Domain.Concerns.Lifecycle.SoftDeletable;

public static class SoftDeletableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "SoftDeletable.Entity.Required",
            message: "Soft-deletable entity is required.");

        public static Error DeletedAtRequired => Error.UnprocessableEntity(
            code: "SoftDeletable.DeletedAt.Required",
            message: "Deleted date must be specified.");

        public static Error AlreadyDeleted => Error.UnprocessableEntity(
            code: "SoftDeletable.AlreadyDeleted",
            message: "Entity is already deleted.");

        public static Error NotDeleted => Error.UnprocessableEntity(
            code: "SoftDeletable.NotDeleted",
            message: "Entity is not deleted, so it cannot be restored.");

        public static Error DeletedByTooLong => Error.UnprocessableEntity(
            code: "SoftDeletable.DeletedBy.TooLong",
            message:
                $"Deleted by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error DeletedByRequired => Error.UnprocessableEntity(
            code: "SoftDeletable.DeletedBy.Required",
            message: "Deleted by is required.");

        public static Error ActorInvalid => Error.UnprocessableEntity(
            code: "SoftDeletable.Actor.Invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
