namespace SharedKernel.Concerns.Lifecycle.Activatable;

public static class ActivatableExtensions
{
    /// <summary>
    /// Activates an activatable entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> Activate<TValue>(
        this Result<TValue, Error> result,
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
    public static Result<TValue, Error> Deactivate<TValue>(
        this Result<TValue, Error> result,
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
