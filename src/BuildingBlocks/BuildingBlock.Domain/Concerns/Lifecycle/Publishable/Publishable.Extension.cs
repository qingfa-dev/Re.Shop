namespace BuildingBlocks.Domain.Concerns.Lifecycle.Publishable;

public static class PublishableExtensions
{
    /// <summary>
    /// Publishes a publishable entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> Publish<TValue>(
        this Result<TValue> result,
        DateTimeOffset nowUtc,
        string? actor = null,
        bool requireActor = false)
        where TValue : IPublishable
    {
        return result
            .Bind(entity =>
                PublishableValidator.ValidatePublication(
                    entity,
                    nowUtc,
                    actor,
                    requireActor))

            .Tap(entity =>
            {
                entity.IsPublished = true;
                entity.PublishedAtUtc = nowUtc.ToUniversalTime();
                entity.PublishedBy = actor;
            });
    }

    /// <summary>
    /// Unpublishes a publishable entity and clears its publication audit metadata.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> Unpublish<TValue>(
        this Result<TValue> result,
        string? actor = null,
        bool requireActor = false)
        where TValue : IPublishable
    {
        return result
            .Bind(entity =>
                PublishableValidator.ValidateUnpublication(
                    entity,
                    actor,
                    requireActor))

            .Tap(entity =>
            {
                entity.IsPublished = false;
                entity.PublishedAtUtc = null;
                entity.PublishedBy = null;
            });
    }
}
