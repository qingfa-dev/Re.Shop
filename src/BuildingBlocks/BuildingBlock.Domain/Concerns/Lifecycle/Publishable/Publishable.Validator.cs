using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Concerns.Lifecycle.Publishable;

public static class PublishableValidator
{
    /// <summary>
    /// Validates the publication metadata that
    /// <see cref="PublishableExtensions.Publish{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidatePublication<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireActor = false)
        where TValue : IPublishable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                PublishableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                errorValue: PublishableResult.Failure.PublishedAtRequired)

            .Ensure(
                predicate: entity => !entity.IsPublished,
                errorValue: PublishableResult.Failure.AlreadyPublished)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                errorValue: PublishableResult.Failure.PublishedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                errorValue: PublishableResult.Failure.PublishedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                errorValue: PublishableResult.Failure.ActorInvalid);
    }

    /// <summary>
    /// Validates the unpublication metadata that
    /// <see cref="PublishableExtensions.Unpublish{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateUnpublication<TValue>(
        TValue auditable,
        string? actor,
        bool requireActor = false)
        where TValue : IPublishable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                PublishableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => entity.IsPublished,
                errorValue: PublishableResult.Failure.NotPublished)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                errorValue: PublishableResult.Failure.PublishedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                errorValue: PublishableResult.Failure.PublishedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                errorValue: PublishableResult.Failure.ActorInvalid);
    }
}
