using SharedKernel.Concerns.Lifecycle.Creatable;

namespace SharedKernel.Concerns.Lifecycle.Modifiable;

public static class ModifiableExtensions
{
    /// <summary>
    /// Updates modification audit metadata for a modified entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> MarkModified<TValue>(
        this Result<TValue, Error> result,
        DateTimeOffset nowUtc,
        string? actor = null,
        bool requireModifiedBy = false)
        where TValue : ICreatable, IModifiable
    {
        return result
            .Bind(entity =>
                ModifiableValidator.ValidateModification(
                    entity,
                    nowUtc,
                    actor,
                    requireModifiedBy))

            .Tap(entity =>
            {
                entity.ModifiedAtUtc = nowUtc.ToUniversalTime();
                entity.ModifiedBy = actor;
            });
    }
}
