namespace BuildingBlocks.Domain.Concerns.Foundation.Identifiable;

public static class IdentifiableExtensions
{
    /// <summary>
    /// Fails when the entity's identifier is missing (default value).
    /// Validation only; the identifier is get-only.
    /// </summary>
    public static Result<TValue> EnsureIdentified<TValue, TKey>(
        this Result<TValue> result)
        where TValue : IIdentifiable<TKey>
    {
        return result.Bind(entity =>
            IdentifiableValidator.ValidateIdentification<TValue, TKey>(entity));
    }
}
