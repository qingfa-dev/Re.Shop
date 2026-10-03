using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Concerns.Lifecycle.Creatable;

public static class CreatableValidator
{
    /// <summary>
    /// Validates the audit metadata that <see cref="CreatableExtensions.InitializeAudit{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateCreation<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireCreatedBy = false)
        where TValue : ICreatable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                CreatableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                errorValue: CreatableResult.Failure.CreatedAtRequired)

            .Ensure(
                predicate: entity => entity.CreatedAtUtc == default,
                errorValue: CreatableResult.Failure.AlreadyInitialized)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                errorValue: CreatableResult.Failure.CreatedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireCreatedBy ||
                    !string.IsNullOrWhiteSpace(actor),
                errorValue: CreatableResult.Failure.CreatedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                errorValue: CreatableResult.Failure.ActorInvalid);
    }
}
