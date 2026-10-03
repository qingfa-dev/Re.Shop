namespace BuildingBlocks.Domain.Concerns.Lifecycle.Activatable;

public static class ActivatableExtensions
{
    /// <summary>
    /// Activates an activatable entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> Activate<TValue>(
        this Result<TValue> result,
        DateTimeOffset nowUtc,
        string? actor = null,
        bool requireActor = false)
        where TValue : IActivatable
    {
        return result
            .Bind(entity =>
                ActivatableValidator.ValidateActivation(
                    entity,
                    nowUtc,
                    actor,
                    requireActor))

            .Tap(entity =>
            {
                entity.IsActive = true;
                entity.ActivatedAtUtc = nowUtc.ToUniversalTime();
                entity.ActivatedBy = actor;
            });
    }

    /// <summary>
    /// Deactivates an activatable entity and clears its activation audit metadata.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> Deactivate<TValue>(
        this Result<TValue> result,
        string? actor = null,
        bool requireActor = false)
        where TValue : IActivatable
    {
        return result
            .Bind(entity =>
                ActivatableValidator.ValidateDeactivation(
                    entity,
                    actor,
                    requireActor))

            .Tap(entity =>
            {
                entity.IsActive = false;
                entity.ActivatedAtUtc = null;
                entity.ActivatedBy = null;
            });
    }
}
