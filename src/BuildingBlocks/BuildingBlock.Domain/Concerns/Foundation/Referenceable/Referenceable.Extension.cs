namespace BuildingBlocks.Domain.Concerns.Foundation.Referenceable;

public static class ReferenceableExtensions
{
    /// <summary>
    /// Sets the entity's reference.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> SetReference<TValue>(
        this Result<TValue> result,
        string? reference)
        where TValue : IReferenceable
    {
        return result
            .Bind(entity =>
                ReferenceableValidator.ValidateReference(entity, reference))

            .Tap(entity =>
            {
                entity.Reference = reference!;
            });
    }
}
