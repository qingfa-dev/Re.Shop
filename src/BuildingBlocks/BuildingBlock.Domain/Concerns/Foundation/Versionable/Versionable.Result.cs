namespace BuildingBlocks.Domain.Concerns.Foundation.Versionable;

public static class VersionableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Versionable.Entity.Required",
            message: "Versionable entity is required.");

        public static Error VersionNegative => Error.UnprocessableEntity(
            code: "Versionable.Version.Negative",
            message: "Version must not be negative.");

        public static Error VersionOverflow => Error.UnprocessableEntity(
            code: "Versionable.Version.Overflow",
            message: "Version is already at its maximum value.");

        #endregion
    }
}
