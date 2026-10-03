using System.Text.RegularExpressions;

using BuildingBlocks.Domain.Concerns.Lifecycle.Creatable;

namespace BuildingBlocks.Domain.Concerns.Lifecycle.Modifiable;

public static class ModifiableValidator
{
    /// <summary>
    /// Validates the audit metadata that <see cref="ModifiableExtensions.MarkModified{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateModification<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireModifiedBy = false)
        where TValue : ICreatable, IModifiable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                ModifiableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                errorValue: ModifiableResult.Failure.ModifiedAtRequired)

            .Ensure(
                predicate: entity => entity.CreatedAtUtc != default,
                errorValue: ModifiableResult.Failure.NotInitialized)

            .Ensure(
                predicate: entity => nowUtc >= entity.CreatedAtUtc,
                errorValue: ModifiableResult.Failure.ModifiedAtBeforeCreatedAt)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                errorValue: ModifiableResult.Failure.ModifiedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireModifiedBy ||
                    !string.IsNullOrWhiteSpace(actor),
                errorValue: ModifiableResult.Failure.ModifiedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                errorValue: ModifiableResult.Failure.ActorInvalid);
    }
}
