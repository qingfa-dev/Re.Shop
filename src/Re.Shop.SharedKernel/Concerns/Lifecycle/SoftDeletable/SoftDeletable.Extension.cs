namespace SharedKernel.Concerns.Lifecycle.SoftDeletable;

public static class SoftDeletableExtensions
{
    /// <summary>
    /// Marks a soft-deletable entity as deleted.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> MarkDeleted<TValue>(
        this Result<TValue, Error> result,
        DateTimeOffset nowUtc,
        string? actor = null,
        bool requireActor = false)
        where TValue : ISoftDeletable
    {
        return result
            .Bind(entity =>
                SoftDeletableValidator.ValidateDeletion(
                    entity,
                    nowUtc,
                    actor,
                    requireActor))

            .Tap(entity =>
            {
                entity.IsDeleted = true;
                entity.DeletedAtUtc = nowUtc.ToUniversalTime();
                entity.DeletedBy = actor;
            });
    }

    /// <summary>
    /// Restores a soft-deletable entity and clears its deletion audit metadata.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> Restore<TValue>(
        this Result<TValue, Error> result,
        string? actor = null,
        bool requireActor = false)
        where TValue : ISoftDeletable
    {
        return result
            .Bind(entity =>
                SoftDeletableValidator.ValidateRestoration(
                    entity,
                    actor,
                    requireActor))

            .Tap(entity =>
            {
                entity.IsDeleted = false;
                entity.DeletedAtUtc = null;
                entity.DeletedBy = null;
            });
    }
}
