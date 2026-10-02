namespace SharedKernel.Concerns.Organization.Positionable;

public static class PositionableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "positionable.entity.required",
            message: "Positionable entity is required.");

        public static Error PositionNegative => Error.Validation(
            code: "positionable.position.negative",
            message: "Position must not be negative.");

        #endregion
    }
}
