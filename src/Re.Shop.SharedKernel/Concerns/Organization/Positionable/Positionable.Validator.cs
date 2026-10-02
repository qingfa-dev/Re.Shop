namespace SharedKernel.Concerns.Organization.Positionable;

public static class PositionableValidator
{
    /// <summary>
    /// Validates the position that <see cref="PositionableExtensions.SetPosition{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidatePosition<TValue>(
        TValue auditable,
        int position)
        where TValue : IPositionable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                PositionableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ =>
                    position >= PositionableConstant.Constraints.MinPosition,
                error: PositionableResult.Failure.PositionNegative);
    }
}
