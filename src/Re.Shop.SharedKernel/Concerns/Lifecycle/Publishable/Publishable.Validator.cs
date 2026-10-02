using System.Text.RegularExpressions;

namespace SharedKernel.Concerns.Lifecycle.Publishable;

public static class PublishableValidator
{
    /// <summary>
    /// Validates the publication metadata that
    /// <see cref="PublishableExtensions.Publish{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidatePublication<TValue>(
        TValue auditable,
        DateTimeOffset nowUtc,
        string? actor,
        bool requireActor = false)
        where TValue : IPublishable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                PublishableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => nowUtc != default,
                error: PublishableResult.Failure.PublishedAtRequired)

            .Ensure(
                predicate: entity => !entity.IsPublished,
                error: PublishableResult.Failure.AlreadyPublished)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                error: PublishableResult.Failure.PublishedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                error: PublishableResult.Failure.PublishedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                error: PublishableResult.Failure.ActorInvalid);
    }

    /// <summary>
    /// Validates the unpublication metadata that
    /// <see cref="PublishableExtensions.Unpublish{TValue}"/> is about to apply,
    /// without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateUnpublication<TValue>(
        TValue auditable,
        string? actor,
        bool requireActor = false)
        where TValue : IPublishable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                PublishableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => entity.IsPublished,
                error: PublishableResult.Failure.NotPublished)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    actor.Length <=
                    LifecycleConstant.Constraints.MaxActorLength,
                error: PublishableResult.Failure.PublishedByTooLong)

            .Ensure(
                predicate: _ =>
                    !requireActor ||
                    !string.IsNullOrWhiteSpace(actor),
                error: PublishableResult.Failure.PublishedByRequired)

            .Ensure(
                predicate: _ =>
                    actor is null ||
                    Regex.IsMatch(actor, LifecycleConstant.Patterns.Actor),
                error: PublishableResult.Failure.ActorInvalid);
    }
}
