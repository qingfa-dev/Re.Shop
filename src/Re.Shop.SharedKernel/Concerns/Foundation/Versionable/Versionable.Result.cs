namespace SharedKernel.Concerns.Foundation.Versionable;

public static class VersionableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "versionable.entity.required",
            message: "Versionable entity is required.");

        public static Error VersionNegative => Error.Validation(
            code: "versionable.version.negative",
            message: "Version must not be negative.");

        public static Error VersionOverflow => Error.Validation(
            code: "versionable.version.overflow",
            message: "Version is already at its maximum value.");

        #endregion
    }
}
