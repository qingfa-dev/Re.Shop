namespace SharedKernel.Concerns.Foundation.Identifiable;

public static class IdentifiableExtensions
{
    /// <summary>
    /// Fails when the entity's identifier is missing (default value).
    /// Validation only; the identifier is get-only.
    /// </summary>
    public static Result<TValue, Error> EnsureIdentified<TValue, TKey>(
        this Result<TValue, Error> result)
        where TValue : IIdentifiable<TKey>
    {
        return result.Ensure(entity =>
            IdentifiableValidator.ValidateIdentification<TValue, TKey>(entity));
    }
}
