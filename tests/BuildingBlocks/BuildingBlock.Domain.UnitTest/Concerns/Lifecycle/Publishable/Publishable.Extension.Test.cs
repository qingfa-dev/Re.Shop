using BuildingBlocks.Domain.Concerns.Lifecycle;
using BuildingBlocks.Domain.Concerns.Lifecycle.Publishable;
namespace BuildingBlocks.Domain.Tests.Concerns.Lifecycle.Publishable;

public class PublishableExtensionTest
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private static Result<TestPublishable> Success(TestPublishable entity)
    {
        return Result<TestPublishable>.Success(entity);
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
        result.Errors![0].Code.ShouldBe("Publishable.PublishedAt.Required");
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void Publish_AlreadyPublished_ShouldFailWithoutMutation()
    {
        var entity = PublishedEntity();

        var result = Success(entity).Publish(NowUtc, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Publishable.AlreadyPublished");
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
        result.Errors![0].Code.ShouldBe("Publishable.PublishedBy.TooLong");
        entity.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public void Publish_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestPublishable();

        var result = Success(entity).Publish(NowUtc, " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Publishable.Actor.Invalid");
        entity.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public void Publish_EmptyActor_ShouldFailActorInvalid()
    {
        var entity = new TestPublishable();

        var result = Success(entity).Publish(NowUtc, string.Empty);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Publishable.Actor.Invalid");
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
        result.Errors![0].Code.ShouldBe("Publishable.NotPublished");
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
        result.Errors![0].Code.ShouldBe("Publishable.PublishedBy.Required");
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
    }

    #endregion

    #region Validators

    [Fact]
    public void ValidatePublication_NullEntity_ShouldFailEntityRequired()
    {
        var result = PublishableValidator.ValidatePublication<TestPublishable>(
            null!,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Publishable.Entity.Required");
    }

    [Fact]
    public void ValidateUnpublication_NullEntity_ShouldFailEntityRequired()
    {
        var result = PublishableValidator.ValidateUnpublication<TestPublishable>(
            null!,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Publishable.Entity.Required");
    }

    #endregion
}
