using System.Text.RegularExpressions;

namespace SharedKernel.Concerns.Lifecycle.SoftDeletable;

public static class SoftDeletableValidator
{
    /// <summary>
    /// Validates the deletion metadata that
    /// <see cref="SoftDeletableExtensions.MarkDeleted{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateDeletion<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireActor = false)
        where TValue : ISoftDeletable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                SoftDeletableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                error: SoftDeletableResult.Failure.DeletedAtRequired)

            .Ensure(
                predicate: entity => !entity.IsDeleted,
                error: SoftDeletableResult.Failure.AlreadyDeleted)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                error: SoftDeletableResult.Failure.DeletedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                error: SoftDeletableResult.Failure.DeletedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                error: SoftDeletableResult.Failure.ActorInvalid);
    }

    /// <summary>
    /// Validates the restoration metadata that
    /// <see cref="SoftDeletableExtensions.Restore{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateRestoration<TValue>(
        TValue auditable,
        string? actor,
        bool requireActor = false)
        where TValue : ISoftDeletable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                SoftDeletableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => entity.IsDeleted,
                error: SoftDeletableResult.Failure.NotDeleted)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                error: SoftDeletableResult.Failure.DeletedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                error: SoftDeletableResult.Failure.DeletedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                error: SoftDeletableResult.Failure.ActorInvalid);
    }
}
