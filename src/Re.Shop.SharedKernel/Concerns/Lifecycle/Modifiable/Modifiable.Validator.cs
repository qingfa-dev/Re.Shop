using System.Text.RegularExpressions;

using SharedKernel.Concerns.Lifecycle.Creatable;

namespace SharedKernel.Concerns.Lifecycle.Modifiable;

public static class ModifiableValidator
{
    /// <summary>
    /// Validates the audit metadata that <see cref="ModifiableExtensions.MarkModified{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateModification<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireModifiedBy = false)
        where TValue : ICreatable, IModifiable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                ModifiableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                error: ModifiableResult.Failure.ModifiedAtRequired)

            .Ensure(
                predicate: entity => entity.CreatedAtUtc != default,
                error: ModifiableResult.Failure.NotInitialized)

            .Ensure(
                predicate: entity => nowUtc >= entity.CreatedAtUtc,
                error: ModifiableResult.Failure.ModifiedAtBeforeCreatedAt)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                error: ModifiableResult.Failure.ModifiedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireModifiedBy ||
                    !string.IsNullOrWhiteSpace(actor),
                error: ModifiableResult.Failure.ModifiedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                error: ModifiableResult.Failure.ActorInvalid);
    }
}
