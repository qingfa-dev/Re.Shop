using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Concerns.Lifecycle.SoftDeletable;

public static class SoftDeletableValidator
{
    /// <summary>
    /// Validates the deletion metadata that
    /// <see cref="SoftDeletableExtensions.MarkDeleted{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateDeletion<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireActor = false)
        where TValue : ISoftDeletable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                SoftDeletableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                errorValue: SoftDeletableResult.Failure.DeletedAtRequired)

            .Ensure(
                predicate: entity => !entity.IsDeleted,
                errorValue: SoftDeletableResult.Failure.AlreadyDeleted)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                errorValue: SoftDeletableResult.Failure.DeletedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                errorValue: SoftDeletableResult.Failure.DeletedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                errorValue: SoftDeletableResult.Failure.ActorInvalid);
    }

    /// <summary>
    /// Validates the restoration metadata that
    /// <see cref="SoftDeletableExtensions.Restore{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateRestoration<TValue>(
        TValue auditable,
        string? actor,
        bool requireActor = false)
        where TValue : ISoftDeletable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                SoftDeletableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => entity.IsDeleted,
                errorValue: SoftDeletableResult.Failure.NotDeleted)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                errorValue: SoftDeletableResult.Failure.DeletedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                errorValue: SoftDeletableResult.Failure.DeletedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                errorValue: SoftDeletableResult.Failure.ActorInvalid);
    }
}
