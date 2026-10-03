using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Concerns.Lifecycle.Activatable;

public static class ActivatableValidator
{
    /// <summary>
    /// Validates the activation metadata that
    /// <see cref="ActivatableExtensions.Activate{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateActivation<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireActor = false)
        where TValue : IActivatable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                ActivatableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                errorValue: ActivatableResult.Failure.ActivatedAtRequired)

            .Ensure(
                predicate: entity => !entity.IsActive,
                errorValue: ActivatableResult.Failure.AlreadyActive)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                errorValue: ActivatableResult.Failure.ActivatedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                errorValue: ActivatableResult.Failure.ActivatedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                errorValue: ActivatableResult.Failure.ActorInvalid);
    }

    /// <summary>
    /// Validates the deactivation metadata that
    /// <see cref="ActivatableExtensions.Deactivate{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateDeactivation<TValue>(
        TValue auditable,
        string? actor,
        bool requireActor = false)
        where TValue : IActivatable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                ActivatableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => entity.IsActive,
                errorValue: ActivatableResult.Failure.NotActive)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                errorValue: ActivatableResult.Failure.ActivatedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                errorValue: ActivatableResult.Failure.ActivatedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                errorValue: ActivatableResult.Failure.ActorInvalid);
    }
}
