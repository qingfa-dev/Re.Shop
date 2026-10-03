namespace BuildingBlocks.Domain.Concerns.Lifecycle.Modifiable;

public static class ModifiableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Modifiable.Entity.Required",
            message: "Modifiable entity is required.");

        public static Error NotInitialized => Error.UnprocessableEntity(
            code: "Modifiable.NotInitialized",
            message:
                "Entity has no creation audit metadata; initialize it before marking it modified.");

        public static Error ModifiedAtRequired => Error.UnprocessableEntity(
            code: "Modifiable.ModifiedAt.Required",
            message: "Modified date must be specified.");

        public static Error ModifiedAtBeforeCreatedAt => Error.UnprocessableEntity(
            code: "Modifiable.ModifiedAt.InvalidRange",
            message: "Modified date cannot be earlier than created date.");

        public static Error ModifiedByTooLong => Error.UnprocessableEntity(
            code: "Modifiable.ModifiedBy.TooLong",
            message:
                $"Modified by cannot exceed {LifecycleConstant.Constraints.MaxActorLength} characters.");

        public static Error ModifiedByRequired => Error.UnprocessableEntity(
            code: "Modifiable.ModifiedBy.Required",
            message: "Modified by is required when the entity has been modified.");

        public static Error ActorInvalid => Error.UnprocessableEntity(
            code: "Modifiable.Actor.Invalid",
            message:
                "Actor must not be empty or contain leading/trailing whitespace.");

        #endregion
    }
}
