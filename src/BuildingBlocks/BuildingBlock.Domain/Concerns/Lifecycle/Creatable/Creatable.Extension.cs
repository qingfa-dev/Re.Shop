namespace BuildingBlocks.Domain.Concerns.Lifecycle.Creatable;

public static class CreatableExtensions
{
    /// <summary>
    /// Initializes creation audit metadata for a newly created entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> InitializeAudit<TValue>(
        this Result<TValue> result,
        DateTimeOffset nowUtc,
        string? actor = null,
        bool requireCreatedBy = false)
        where TValue : ICreatable
    {
        return result
            .Bind(entity =>
                CreatableValidator.ValidateCreation(
                    entity,
                    nowUtc,
                    actor,
                    requireCreatedBy))

            .Tap(entity =>
            {
                entity.CreatedAtUtc = nowUtc.ToUniversalTime();
                entity.CreatedBy = actor;
            });
    }
}
