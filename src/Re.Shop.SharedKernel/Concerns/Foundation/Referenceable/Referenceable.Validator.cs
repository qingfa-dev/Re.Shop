namespace SharedKernel.Concerns.Foundation.Referenceable;

public static class ReferenceableValidator
{
    /// <summary>
    /// Validates the reference that <see cref="ReferenceableExtensions.SetReference{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateReference<TValue>(
        TValue auditable,
        string? reference)
        where TValue : IReferenceable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                ReferenceableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(reference),
                error: ReferenceableResult.Failure.ReferenceRequired)

            .Ensure(
                predicate: _ =>
                    reference!.Length <=
                    ReferenceableConstant.Constraints.MaxReferenceLength,
                error: ReferenceableResult.Failure.ReferenceTooLong);
    }
}
