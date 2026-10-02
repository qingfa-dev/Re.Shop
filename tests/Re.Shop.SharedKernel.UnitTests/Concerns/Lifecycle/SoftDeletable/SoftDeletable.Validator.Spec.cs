using SharedKernel.Concerns.Lifecycle;
using SharedKernel.Concerns.Lifecycle.SoftDeletable;

namespace SharedKernel.UnitTests.Concerns.Lifecycle.SoftDeletable;

public class SoftDeletableValidatorSpec
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private sealed class TestSoftDeletable : ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
    }

    [Fact]
    public void ValidateDeletion_ValidInput_ShouldSucceed()
    {
        var entity = new TestSoftDeletable();

        var result = SoftDeletableValidator.ValidateDeletion(
            entity,
            NowUtc,
            "alice");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
        entity.DeletedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateDeletion_NullEntity_ShouldFailEntityRequired()
    {
        var result = SoftDeletableValidator.ValidateDeletion(
            (TestSoftDeletable)null!,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.entity.required");
    }

    [Fact]
    public void ValidateDeletion_DefaultNowUtc_ShouldFailDeletedAtRequired()
    {
        var entity = new TestSoftDeletable();

        var result = SoftDeletableValidator.ValidateDeletion(
            entity,
            default,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_at.required");
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
        entity.DeletedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateDeletion_AlreadyDeleted_ShouldFailAlreadyDeleted()
    {
        var entity = new TestSoftDeletable
        {
            IsDeleted = true,
            DeletedAtUtc = NowUtc,
            DeletedBy = "bob"
        };

        var result = SoftDeletableValidator.ValidateDeletion(
            entity,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.state.already_deleted");
        entity.IsDeleted.ShouldBeTrue();
        entity.DeletedAtUtc.ShouldBe(NowUtc);
        entity.DeletedBy.ShouldBe("bob");
    }

    [Fact]
    public void ValidateDeletion_ActorTooLong_ShouldFailDeletedByTooLong()
    {
        var entity = new TestSoftDeletable();
        var actor = new string(
            'a',
            LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = SoftDeletableValidator.ValidateDeletion(
            entity,
            NowUtc,
            actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_by.too_long");
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateDeletion_MissingActor_ShouldFailDeletedByRequired()
    {
        var entity = new TestSoftDeletable();

        var result = SoftDeletableValidator.ValidateDeletion(
            entity,
            NowUtc,
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_by.required");
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateDeletion_WhitespaceActor_ShouldFailDeletedByRequired()
    {
        var entity = new TestSoftDeletable();

        var result = SoftDeletableValidator.ValidateDeletion(
            entity,
            NowUtc,
            actor: "   ",
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_by.required");
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateDeletion_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestSoftDeletable();

        var result = SoftDeletableValidator.ValidateDeletion(
            entity,
            NowUtc,
            " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.actor.invalid");
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateRestoration_ValidInput_ShouldSucceed()
    {
        var entity = new TestSoftDeletable
        {
            IsDeleted = true,
            DeletedAtUtc = NowUtc,
            DeletedBy = "bob"
        };

        var result = SoftDeletableValidator.ValidateRestoration(
            entity,
            "carol");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.IsDeleted.ShouldBeTrue();
        entity.DeletedAtUtc.ShouldBe(NowUtc);
        entity.DeletedBy.ShouldBe("bob");
    }

    [Fact]
    public void ValidateRestoration_NullEntity_ShouldFailEntityRequired()
    {
        var result = SoftDeletableValidator.ValidateRestoration(
            (TestSoftDeletable)null!,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.entity.required");
    }

    [Fact]
    public void ValidateRestoration_NotDeleted_ShouldFailNotDeleted()
    {
        var entity = new TestSoftDeletable();

        var result = SoftDeletableValidator.ValidateRestoration(
            entity,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.state.not_deleted");
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateRestoration_ActorTooLong_ShouldFailDeletedByTooLong()
    {
        var entity = new TestSoftDeletable
        {
            IsDeleted = true,
            DeletedAtUtc = NowUtc,
            DeletedBy = "bob"
        };
        var actor = new string(
            'a',
            LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = SoftDeletableValidator.ValidateRestoration(
            entity,
            actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_by.too_long");
        entity.IsDeleted.ShouldBeTrue();
        entity.DeletedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateRestoration_MissingActor_ShouldFailDeletedByRequired()
    {
        var entity = new TestSoftDeletable
        {
            IsDeleted = true,
            DeletedAtUtc = NowUtc,
            DeletedBy = "bob"
        };

        var result = SoftDeletableValidator.ValidateRestoration(
            entity,
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_by.required");
        entity.IsDeleted.ShouldBeTrue();
        entity.DeletedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateRestoration_WhitespaceActor_ShouldFailDeletedByRequired()
    {
        var entity = new TestSoftDeletable
        {
            IsDeleted = true,
            DeletedAtUtc = NowUtc,
            DeletedBy = "bob"
        };

        var result = SoftDeletableValidator.ValidateRestoration(
            entity,
            actor: "   ",
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_by.required");
        entity.IsDeleted.ShouldBeTrue();
        entity.DeletedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateRestoration_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestSoftDeletable
        {
            IsDeleted = true,
            DeletedAtUtc = NowUtc,
            DeletedBy = "bob"
        };

        var result = SoftDeletableValidator.ValidateRestoration(
            entity,
            " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.actor.invalid");
        entity.IsDeleted.ShouldBeTrue();
        entity.DeletedAtUtc.ShouldBe(NowUtc);
    }
}
