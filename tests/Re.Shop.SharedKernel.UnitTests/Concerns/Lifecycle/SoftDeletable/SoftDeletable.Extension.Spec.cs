using SharedKernel.Concerns.Lifecycle;
using SharedKernel.Concerns.Lifecycle.SoftDeletable;

namespace SharedKernel.UnitTests.Concerns.Lifecycle.SoftDeletable;

public class SoftDeletableExtensionSpec
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private static Result<TestSoftDeletable, Error> Success(TestSoftDeletable entity)
    {
        return Result<TestSoftDeletable, Error>.Success(entity);
    }

    private sealed class TestSoftDeletable : ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
    }

    private static TestSoftDeletable DeletedEntity(
        string? deletedBy = "bob",
        DateTimeOffset? deletedAtUtc = null)
    {
        return new TestSoftDeletable
        {
            IsDeleted = true,
            DeletedAtUtc = deletedAtUtc ?? NowUtc,
            DeletedBy = deletedBy
        };
    }

    #region MarkDeleted

    [Fact]
    public void MarkDeleted_ActiveEntity_ShouldSetDeletionFields()
    {
        var entity = new TestSoftDeletable();

        var result = Success(entity).MarkDeleted(NowUtc, "alice");

        result.IsSuccess.ShouldBeTrue();
        entity.IsDeleted.ShouldBeTrue();
        entity.DeletedAtUtc.ShouldBe(NowUtc);
        entity.DeletedBy.ShouldBe("alice");
    }

    [Fact]
    public void MarkDeleted_OffsetTimestamp_ShouldStoreUtcInstant()
    {
        var entity = new TestSoftDeletable();
        var offset = new DateTimeOffset(2024, 5, 1, 20, 0, 0, TimeSpan.FromHours(8));

        var result = Success(entity).MarkDeleted(offset, "alice");

        result.IsSuccess.ShouldBeTrue();
        entity.DeletedAtUtc!.Value.Offset.ShouldBe(TimeSpan.Zero);
        entity.DeletedAtUtc.ShouldBe(offset.ToUniversalTime());
    }

    [Fact]
    public void MarkDeleted_DefaultNowUtc_ShouldFailDeletedAtRequiredWithoutMutation()
    {
        var entity = new TestSoftDeletable();

        var result = Success(entity).MarkDeleted(default, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_at.required");
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void MarkDeleted_AlreadyDeleted_ShouldFailWithoutMutation()
    {
        var entity = DeletedEntity();

        var result = Success(entity).MarkDeleted(NowUtc, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.state.already_deleted");
        entity.DeletedAtUtc.ShouldBe(NowUtc);
        entity.DeletedBy.ShouldBe("bob");
    }

    [Fact]
    public void MarkDeleted_ActorTooLong_ShouldFailDeletedByTooLongWithoutMutation()
    {
        var entity = new TestSoftDeletable();
        var actor = new string('a', LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = Success(entity).MarkDeleted(NowUtc, actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_by.too_long");
        entity.IsDeleted.ShouldBeFalse();
    }

    [Fact]
    public void MarkDeleted_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestSoftDeletable();

        var result = Success(entity).MarkDeleted(NowUtc, " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.actor.invalid");
        entity.IsDeleted.ShouldBeFalse();
    }

    [Fact]
    public void MarkDeleted_EmptyActor_ShouldFailActorInvalid()
    {
        var entity = new TestSoftDeletable();

        var result = Success(entity).MarkDeleted(NowUtc, string.Empty);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.actor.invalid");
    }

    #endregion

    #region Restore

    [Fact]
    public void Restore_DeletedEntity_ShouldClearDeletionFields()
    {
        var entity = DeletedEntity();

        var result = Success(entity).Restore("carol");

        result.IsSuccess.ShouldBeTrue();
        entity.IsDeleted.ShouldBeFalse();
        entity.DeletedAtUtc.ShouldBeNull();
        entity.DeletedBy.ShouldBeNull();
    }

    [Fact]
    public void Restore_NotDeleted_ShouldFailNotDeletedWithoutMutation()
    {
        var entity = new TestSoftDeletable();

        var result = Success(entity).Restore("carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.state.not_deleted");
        entity.IsDeleted.ShouldBeFalse();
    }

    [Fact]
    public void Restore_RequireActorWithoutActor_ShouldFailDeletedByRequiredWithoutMutation()
    {
        var entity = DeletedEntity();

        var result = Success(entity).Restore(
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("soft_deletable.deleted_by.required");
        entity.IsDeleted.ShouldBeTrue();
        entity.DeletedAtUtc.ShouldBe(NowUtc);
    }

    #endregion
}
