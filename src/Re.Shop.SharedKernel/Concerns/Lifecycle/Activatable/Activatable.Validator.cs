using System.Text.RegularExpressions;

namespace SharedKernel.Concerns.Lifecycle.Activatable;

public static class ActivatableValidator
{
    /// <summary>
    /// Validates the activation metadata that
    /// <see cref="ActivatableExtensions.Activate{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateActivation<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireActor = false)
        where TValue : IActivatable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                ActivatableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                error: ActivatableResult.Failure.ActivatedAtRequired)

            .Ensure(
                predicate: entity => !entity.IsActive,
                error: ActivatableResult.Failure.AlreadyActive)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                error: ActivatableResult.Failure.ActivatedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                error: ActivatableResult.Failure.ActivatedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                error: ActivatableResult.Failure.ActorInvalid);
    }

    /// <summary>
    /// Validates the deactivation metadata that
    /// <see cref="ActivatableExtensions.Deactivate{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateDeactivation<TValue>(
        TValue auditable,
        string? actor,
        bool requireActor = false)
        where TValue : IActivatable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                ActivatableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => entity.IsActive,
                error: ActivatableResult.Failure.NotActive)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                error: ActivatableResult.Failure.ActivatedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                error: ActivatableResult.Failure.ActivatedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                error: ActivatableResult.Failure.ActorInvalid);
    }
}
