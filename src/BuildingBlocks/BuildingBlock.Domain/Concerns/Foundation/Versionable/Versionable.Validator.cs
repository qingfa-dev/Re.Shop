namespace BuildingBlocks.Domain.Concerns.Foundation.Versionable;

public static class VersionableValidator
{
    /// <summary>
    /// Validates that the entity's version can be incremented,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateBump<TValue>(
        TValue auditable)
        where TValue : IVersionable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                VersionableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    entity.Version >= VersionableConstant.Constraints.MinVersion,
                errorValue: VersionableResult.Failure.VersionNegative)

            .Ensure(
                predicate: entity =>
                    entity.Version < VersionableConstant.Constraints.MaxVersion,
                errorValue: VersionableResult.Failure.VersionOverflow);
    }
}
