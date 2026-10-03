namespace BuildingBlocks.Domain.Concerns.Foundation.Referenceable;

public static class ReferenceableValidator
{
    /// <summary>
    /// Validates the reference that <see cref="ReferenceableExtensions.SetReference{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateReference<TValue>(
        TValue auditable,
        string? reference)
        where TValue : IReferenceable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                ReferenceableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(reference),
                errorValue: ReferenceableResult.Failure.ReferenceRequired)

            .Ensure(
                predicate: _ =>
                    reference!.Length <=
                    ReferenceableConstant.Constraints.MaxReferenceLength,
                errorValue: ReferenceableResult.Failure.ReferenceTooLong);
    }
}
