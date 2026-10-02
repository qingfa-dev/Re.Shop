using SharedKernel.Concerns.Lifecycle;
using SharedKernel.Concerns.Lifecycle.Publishable;

namespace SharedKernel.UnitTests.Concerns.Lifecycle.Publishable;

public class PublishableValidatorSpec
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private sealed class TestPublishable : IPublishable
    {
        public bool IsPublished { get; set; }
        public DateTimeOffset? PublishedAtUtc { get; set; }
        public string? PublishedBy { get; set; }
    }

    [Fact]
    public void ValidatePublication_ValidInput_ShouldSucceed()
    {
        var entity = new TestPublishable();

        var result = PublishableValidator.ValidatePublication(
            entity,
            NowUtc,
            "alice");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
        entity.PublishedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidatePublication_NullEntity_ShouldFailEntityRequired()
    {
        var result = PublishableValidator.ValidatePublication(
            (TestPublishable)null!,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.entity.required");
    }

    [Fact]
    public void ValidatePublication_DefaultNowUtc_ShouldFailPublishedAtRequired()
    {
        var entity = new TestPublishable();

        var result = PublishableValidator.ValidatePublication(
            entity,
            default,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_at.required");
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
        entity.PublishedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidatePublication_AlreadyPublished_ShouldFailAlreadyPublished()
    {
        var entity = new TestPublishable
        {
            IsPublished = true,
            PublishedAtUtc = NowUtc,
            PublishedBy = "bob"
        };

        var result = PublishableValidator.ValidatePublication(
            entity,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.state.already_published");
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
        entity.PublishedBy.ShouldBe("bob");
    }

    [Fact]
    public void ValidatePublication_ActorTooLong_ShouldFailPublishedByTooLong()
    {
        var entity = new TestPublishable();
        var actor = new string(
            'a',
            LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = PublishableValidator.ValidatePublication(
            entity,
            NowUtc,
            actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_by.too_long");
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidatePublication_MissingActor_ShouldFailPublishedByRequired()
    {
        var entity = new TestPublishable();

        var result = PublishableValidator.ValidatePublication(
            entity,
            NowUtc,
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_by.required");
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidatePublication_WhitespaceActor_ShouldFailPublishedByRequired()
    {
        var entity = new TestPublishable();

        var result = PublishableValidator.ValidatePublication(
            entity,
            NowUtc,
            actor: "   ",
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_by.required");
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidatePublication_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestPublishable();

        var result = PublishableValidator.ValidatePublication(
            entity,
            NowUtc,
            " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.actor.invalid");
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateUnpublication_ValidInput_ShouldSucceed()
    {
        var entity = new TestPublishable
        {
            IsPublished = true,
            PublishedAtUtc = NowUtc,
            PublishedBy = "bob"
        };

        var result = PublishableValidator.ValidateUnpublication(
            entity,
            "carol");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
        entity.PublishedBy.ShouldBe("bob");
    }

    [Fact]
    public void ValidateUnpublication_NullEntity_ShouldFailEntityRequired()
    {
        var result = PublishableValidator.ValidateUnpublication(
            (TestPublishable)null!,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.entity.required");
    }

    [Fact]
    public void ValidateUnpublication_NotPublished_ShouldFailNotPublished()
    {
        var entity = new TestPublishable();

        var result = PublishableValidator.ValidateUnpublication(
            entity,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.state.not_published");
        entity.IsPublished.ShouldBeFalse();
        entity.PublishedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateUnpublication_ActorTooLong_ShouldFailPublishedByTooLong()
    {
        var entity = new TestPublishable
        {
            IsPublished = true,
            PublishedAtUtc = NowUtc,
            PublishedBy = "bob"
        };
        var actor = new string(
            'a',
            LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = PublishableValidator.ValidateUnpublication(
            entity,
            actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_by.too_long");
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateUnpublication_MissingActor_ShouldFailPublishedByRequired()
    {
        var entity = new TestPublishable
        {
            IsPublished = true,
            PublishedAtUtc = NowUtc,
            PublishedBy = "bob"
        };

        var result = PublishableValidator.ValidateUnpublication(
            entity,
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_by.required");
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateUnpublication_WhitespaceActor_ShouldFailPublishedByRequired()
    {
        var entity = new TestPublishable
        {
            IsPublished = true,
            PublishedAtUtc = NowUtc,
            PublishedBy = "bob"
        };

        var result = PublishableValidator.ValidateUnpublication(
            entity,
            actor: "   ",
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.published_by.required");
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateUnpublication_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestPublishable
        {
            IsPublished = true,
            PublishedAtUtc = NowUtc,
            PublishedBy = "bob"
        };

        var result = PublishableValidator.ValidateUnpublication(
            entity,
            " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("publishable.actor.invalid");
        entity.IsPublished.ShouldBeTrue();
        entity.PublishedAtUtc.ShouldBe(NowUtc);
    }
}
