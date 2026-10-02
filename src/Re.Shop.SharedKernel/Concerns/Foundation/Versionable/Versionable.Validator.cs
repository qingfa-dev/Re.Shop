namespace SharedKernel.Concerns.Foundation.Versionable;

public static class VersionableValidator
{
    /// <summary>
    /// Validates that the entity's version can be incremented,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateBump<TValue>(
        TValue auditable)
        where TValue : IVersionable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                VersionableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    entity.Version >= VersionableConstant.Constraints.MinVersion,
                error: VersionableResult.Failure.VersionNegative)

            .Ensure(
                predicate: entity =>
                    entity.Version < VersionableConstant.Constraints.MaxVersion,
                error: VersionableResult.Failure.VersionOverflow);
    }
}
