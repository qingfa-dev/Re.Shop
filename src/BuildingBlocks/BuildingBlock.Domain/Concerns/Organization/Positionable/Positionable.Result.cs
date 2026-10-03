namespace BuildingBlocks.Domain.Concerns.Organization.Positionable;

public static class PositionableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Positionable.Entity.Required",
            message: "Positionable entity is required.");

        public static Error PositionNegative => Error.UnprocessableEntity(
            code: "Positionable.Position.Negative",
            message: "Position must not be negative.");

        #endregion
    }
}
