namespace SharedKernel.Concerns.Foundation.Identifiable;

public static class IdentifiableValidator
{
    /// <summary>
    /// Validates that the entity carries a non-default identifier.
    /// </summary>
    public static Result<TValue, Error> ValidateIdentification<TValue, TKey>(
        TValue auditable)
        where TValue : IIdentifiable<TKey>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                IdentifiableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    !EqualityComparer<TKey>.Default.Equals(
                        entity.Id,
                        default!),
                error: IdentifiableResult.Failure.IdRequired);
    }
}
