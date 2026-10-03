using BuildingBlocks.Domain.Concerns.Lifecycle;
using BuildingBlocks.Domain.Concerns.Lifecycle.Activatable;
namespace BuildingBlocks.Domain.Tests.Concerns.Lifecycle.Activatable;

public class ActivatableExtensionTest
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private static Result<TestActivatable> Success(TestActivatable entity)
    {
        return Result<TestActivatable>.Success(entity);
    }

    private sealed class TestActivatable : IActivatable
    {
        public bool IsActive { get; set; }
        public DateTimeOffset? ActivatedAtUtc { get; set; }
        public string? ActivatedBy { get; set; }
    }

    private static TestActivatable ActiveEntity(
        string? activatedBy = "bob",
        DateTimeOffset? activatedAtUtc = null)
    {
        return new TestActivatable
        {
            IsActive = true,
            ActivatedAtUtc = activatedAtUtc ?? NowUtc,
            ActivatedBy = activatedBy
        };
    }

    #region Activate

    [Fact]
    public void Activate_InactiveEntity_ShouldSetActivationFields()
    {
        var entity = new TestActivatable();

        var result = Success(entity).Activate(NowUtc, "alice");

        result.IsSuccess.ShouldBeTrue();
        entity.IsActive.ShouldBeTrue();
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
        entity.ActivatedBy.ShouldBe("alice");
    }

    [Fact]
    public void Activate_OffsetTimestamp_ShouldStoreUtcInstant()
    {
        var entity = new TestActivatable();
        var offset = new DateTimeOffset(2024, 5, 1, 20, 0, 0, TimeSpan.FromHours(8));

        var result = Success(entity).Activate(offset, "alice");

        result.IsSuccess.ShouldBeTrue();
        entity.ActivatedAtUtc!.Value.Offset.ShouldBe(TimeSpan.Zero);
        entity.ActivatedAtUtc.ShouldBe(offset.ToUniversalTime());
    }

    [Fact]
    public void Activate_DefaultNowUtc_ShouldFailActivatedAtRequiredWithoutMutation()
    {
        var entity = new TestActivatable();

        var result = Success(entity).Activate(default, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.ActivatedAt.Required");
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void Activate_AlreadyActive_ShouldFailWithoutMutation()
    {
        var entity = ActiveEntity();

        var result = Success(entity).Activate(NowUtc, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.AlreadyActive");
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
        entity.ActivatedBy.ShouldBe("bob");
    }

    [Fact]
    public void Activate_ActorTooLong_ShouldFailActivatedByTooLongWithoutMutation()
    {
        var entity = new TestActivatable();
        var actor = new string('a', LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = Success(entity).Activate(NowUtc, actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.ActivatedBy.TooLong");
        entity.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Activate_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestActivatable();

        var result = Success(entity).Activate(NowUtc, " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.Actor.Invalid");
        entity.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Activate_EmptyActor_ShouldFailActorInvalid()
    {
        var entity = new TestActivatable();

        var result = Success(entity).Activate(NowUtc, string.Empty);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.Actor.Invalid");
    }

    #endregion

    #region Deactivate

    [Fact]
    public void Deactivate_ActiveEntity_ShouldClearActivationFields()
    {
        var entity = ActiveEntity();

        var result = Success(entity).Deactivate("carol");

        result.IsSuccess.ShouldBeTrue();
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
        entity.ActivatedBy.ShouldBeNull();
    }

    [Fact]
    public void Deactivate_NotActive_ShouldFailNotActiveWithoutMutation()
    {
        var entity = new TestActivatable();

        var result = Success(entity).Deactivate("carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.NotActive");
        entity.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Deactivate_RequireActorWithoutActor_ShouldFailActivatedByRequiredWithoutMutation()
    {
        var entity = ActiveEntity();

        var result = Success(entity).Deactivate(
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.ActivatedBy.Required");
        entity.IsActive.ShouldBeTrue();
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
    }

    #endregion

    #region Validators

    [Fact]
    public void ValidateActivation_NullEntity_ShouldFailEntityRequired()
    {
        var result = ActivatableValidator.ValidateActivation<TestActivatable>(
            null!,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.Entity.Required");
    }

    [Fact]
    public void ValidateDeactivation_NullEntity_ShouldFailEntityRequired()
    {
        var result = ActivatableValidator.ValidateDeactivation<TestActivatable>(
            null!,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Activatable.Entity.Required");
    }

    #endregion
}
