using SharedKernel.Concerns.Lifecycle;
using SharedKernel.Concerns.Lifecycle.Publishable;

namespace SharedKernel.UnitTests.Concerns.Lifecycle.Publishable;

public class PublishableExtensionSpec
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private static Result<TestPublishable, Error> Success(TestPublishable entity)
    {
        return Result<TestPublishable, Error>.Success(entity);
    }

    private sealed class TestPublishable : IPublishable
    {
        public bool IsPublished { get; set; }
        public DateTimeOffset? PublishedAtUtc { get; set; }
        public string? PublishedBy { get; set; }
    }

    private static TestPublishable PublishedEntity(
        string? publishedBy = "bob",
        DateTimeOffset? publishedAtUtc = null)
    {
        return new TestPublishable
        {
            IsPublished = true,
            PublishedAtUtc = publishedAtUtc ?? NowUtc,
            PublishedBy = publishedBy
        };
    }

    #region Publish

    [Fact]
    public void Publish_UnpublishedEntity_ShouldSetPublicationFields()
    {
        var entity = new TestPublishable();

        var result = Success(entity).Publish(NowUtc, "alice");

        result.IsSuccess.ShouldBeTrue();
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
        entity.PublishedBy.ShouldBe("alice");
    }

    [Fact]
    public void Publish_OffsetTimestamp_ShouldStoreUtcInstant()
    {
        var entity = new TestPublishable();
        var offset = new DateTimeOffset(2024, 5, 1, 20, 0, 0, TimeSpan.FromHours(8));

        var result = Success(entity).Publish(offset, "alice");

        result.IsSuccess.ShouldBeTrue();
        entity.PublishedAtUtc!.Value.Offset.ShouldBe(TimeSpan.Zero);
        entity.PublishedAtUtc.ShouldBe(offset.ToUniversalTime());
    }

    [Fact]
    public void Publish_DefaultNowUtc_ShouldFailPublishedAtRequiredWithoutMutation()
    {
        var entity = new TestPublishable();

        var result = Success(entity).Publish(default, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_at.required");
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void Publish_AlreadyPublished_ShouldFailWithoutMutation()
    {
        var entity = PublishedEntity();

        var result = Success(entity).Publish(NowUtc, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.state.already_published");
        entity.PublishedAtUtc.ShouldBe(NowUtc);
        entity.PublishedBy.ShouldBe("bob");
    }

    [Fact]
    public void Publish_ActorTooLong_ShouldFailPublishedByTooLongWithoutMutation()
    {
        var entity = new TestPublishable();
        var actor = new string('a', LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = Success(entity).Publish(NowUtc, actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_by.too_long");
        entity.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public void Publish_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestPublishable();

        var result = Success(entity).Publish(NowUtc, " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.actor.invalid");
        entity.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public void Publish_EmptyActor_ShouldFailActorInvalid()
    {
        var entity = new TestPublishable();

        var result = Success(entity).Publish(NowUtc, string.Empty);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.actor.invalid");
    }

    #endregion

    #region Unpublish

    [Fact]
    public void Unpublish_PublishedEntity_ShouldClearPublicationFields()
    {
        var entity = PublishedEntity();

        var result = Success(entity).Unpublish("carol");

        result.IsSuccess.ShouldBeTrue();
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
        entity.PublishedBy.ShouldBeNull();
    }

    [Fact]
    public void Unpublish_NotPublished_ShouldFailNotPublishedWithoutMutation()
    {
        var entity = new TestPublishable();

        var result = Success(entity).Unpublish("carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.state.not_published");
        entity.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public void Unpublish_RequireActorWithoutActor_ShouldFailPublishedByRequiredWithoutMutation()
    {
        var entity = PublishedEntity();

        var result = Success(entity).Unpublish(
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_by.required");
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
    }

    #endregion
}
