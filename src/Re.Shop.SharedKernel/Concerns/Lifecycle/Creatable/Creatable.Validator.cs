using System.Text.RegularExpressions;

namespace SharedKernel.Concerns.Lifecycle.Creatable;

public static class CreatableValidator
{
    /// <summary>
    /// Validates the audit metadata that <see cref="CreatableExtensions.InitializeAudit{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateCreation<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireCreatedBy = false)
        where TValue : ICreatable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                CreatableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                error: CreatableResult.Failure.CreatedAtRequired)

            .Ensure(
                predicate: entity => entity.CreatedAtUtc == default,
                error: CreatableResult.Failure.AlreadyInitialized)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                error: CreatableResult.Failure.CreatedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireCreatedBy ||
                    !string.IsNullOrWhiteSpace(actor),
                error: CreatableResult.Failure.CreatedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                error: CreatableResult.Failure.ActorInvalid);
    }
}
