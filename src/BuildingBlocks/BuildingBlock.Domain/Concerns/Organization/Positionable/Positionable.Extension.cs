namespace BuildingBlocks.Domain.Concerns.Organization.Positionable;

public static class PositionableExtensions
{
    /// <summary>
    /// Sets the entity's position.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> SetPosition<TValue>(
        this Result<TValue> result,
        int position)
        where TValue : IPositionable
    {
        return result
            .Bind(entity =>
                PositionableValidator.ValidatePosition(entity, position))
            .Tap(entity =>
            {
                entity.Position = position;
            });
    }
}
