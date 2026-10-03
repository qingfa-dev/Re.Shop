namespace BuildingBlocks.Domain.Concerns.Organization.Positionable;

public static class PositionableValidator
{
    /// <summary>
    /// Validates the position that <see cref="PositionableExtensions.SetPosition{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidatePosition<TValue>(
        TValue auditable,
        int position)
        where TValue : IPositionable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                PositionableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ =>
                    position >= PositionableConstant.Constraints.MinPosition,
                errorValue: PositionableResult.Failure.PositionNegative);
    }
}
