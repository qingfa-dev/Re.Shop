namespace BuildingBlocks.Domain.Concerns.Foundation.Identifiable;

public static class IdentifiableValidator
{
    /// <summary>
    /// Validates that the entity carries a non-default identifier.
    /// </summary>
    public static Result<TValue> ValidateIdentification<TValue, TKey>(
        TValue auditable)
        where TValue : IIdentifiable<TKey>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                IdentifiableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    !EqualityComparer<TKey>.Default.Equals(
                        entity.Id,
                        default!),
                errorValue: IdentifiableResult.Failure.IdRequired);
    }
}
