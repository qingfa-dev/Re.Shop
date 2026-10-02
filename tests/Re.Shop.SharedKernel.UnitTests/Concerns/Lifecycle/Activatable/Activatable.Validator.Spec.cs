using SharedKernel.Concerns.Lifecycle;
using SharedKernel.Concerns.Lifecycle.Activatable;

namespace SharedKernel.UnitTests.Concerns.Lifecycle.Activatable;

public class ActivatableValidatorSpec
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private sealed class TestActivatable : IActivatable
    {
        public bool IsActive { get; set; }
        public DateTimeOffset? ActivatedAtUtc { get; set; }
        public string? ActivatedBy { get; set; }
    }

    [Fact]
    public void ValidateActivation_ValidInput_ShouldSucceed()
    {
        var entity = new TestActivatable();

        var result = ActivatableValidator.ValidateActivation(
            entity,
            NowUtc,
            "alice");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
        entity.ActivatedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateActivation_NullEntity_ShouldFailEntityRequired()
    {
        var result = ActivatableValidator.ValidateActivation(
            (TestActivatable)null!,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.entity.required");
    }

    [Fact]
    public void ValidateActivation_DefaultNowUtc_ShouldFailActivatedAtRequired()
    {
        var entity = new TestActivatable();

        var result = ActivatableValidator.ValidateActivation(
            entity,
            default,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.activated_at.required");
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
        entity.ActivatedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateActivation_AlreadyActive_ShouldFailAlreadyActive()
    {
        var entity = new TestActivatable
        {
            IsActive = true,
            ActivatedAtUtc = NowUtc,
            ActivatedBy = "bob"
        };

        var result = ActivatableValidator.ValidateActivation(
            entity,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.state.already_active");
        entity.IsActive.ShouldBeTrue();
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
        entity.ActivatedBy.ShouldBe("bob");
    }

    [Fact]
    public void ValidateActivation_ActorTooLong_ShouldFailActivatedByTooLong()
    {
        var entity = new TestActivatable();
        var actor = new string(
            'a',
            LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = ActivatableValidator.ValidateActivation(
            entity,
            NowUtc,
            actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.activated_by.too_long");
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateActivation_MissingActor_ShouldFailActivatedByRequired()
    {
        var entity = new TestActivatable();

        var result = ActivatableValidator.ValidateActivation(
            entity,
            NowUtc,
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.activated_by.required");
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateActivation_WhitespaceActor_ShouldFailActivatedByRequired()
    {
        var entity = new TestActivatable();

        var result = ActivatableValidator.ValidateActivation(
            entity,
            NowUtc,
            actor: "   ",
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.activated_by.required");
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateActivation_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestActivatable();

        var result = ActivatableValidator.ValidateActivation(
            entity,
            NowUtc,
            " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.actor.invalid");
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateDeactivation_ValidInput_ShouldSucceed()
    {
        var entity = new TestActivatable
        {
            IsActive = true,
            ActivatedAtUtc = NowUtc,
            ActivatedBy = "bob"
        };

        var result = ActivatableValidator.ValidateDeactivation(
            entity,
            "carol");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.IsActive.ShouldBeTrue();
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
        entity.ActivatedBy.ShouldBe("bob");
    }

    [Fact]
    public void ValidateDeactivation_NullEntity_ShouldFailEntityRequired()
    {
        var result = ActivatableValidator.ValidateDeactivation(
            (TestActivatable)null!,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.entity.required");
    }

    [Fact]
    public void ValidateDeactivation_NotActive_ShouldFailNotActive()
    {
        var entity = new TestActivatable();

        var result = ActivatableValidator.ValidateDeactivation(
            entity,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.state.not_active");
        entity.IsActive.ShouldBeFalse();
        entity.ActivatedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void ValidateDeactivation_ActorTooLong_ShouldFailActivatedByTooLong()
    {
        var entity = new TestActivatable
        {
            IsActive = true,
            ActivatedAtUtc = NowUtc,
            ActivatedBy = "bob"
        };
        var actor = new string(
            'a',
            LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = ActivatableValidator.ValidateDeactivation(
            entity,
            actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.activated_by.too_long");
        entity.IsActive.ShouldBeTrue();
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateDeactivation_MissingActor_ShouldFailActivatedByRequired()
    {
        var entity = new TestActivatable
        {
            IsActive = true,
            ActivatedAtUtc = NowUtc,
            ActivatedBy = "bob"
        };

        var result = ActivatableValidator.ValidateDeactivation(
            entity,
            actor: null,
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.activated_by.required");
        entity.IsActive.ShouldBeTrue();
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateDeactivation_WhitespaceActor_ShouldFailActivatedByRequired()
    {
        var entity = new TestActivatable
        {
            IsActive = true,
            ActivatedAtUtc = NowUtc,
            ActivatedBy = "bob"
        };

        var result = ActivatableValidator.ValidateDeactivation(
            entity,
            actor: "   ",
            requireActor: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.activated_by.required");
        entity.IsActive.ShouldBeTrue();
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateDeactivation_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestActivatable
        {
            IsActive = true,
            ActivatedAtUtc = NowUtc,
            ActivatedBy = "bob"
        };

        var result = ActivatableValidator.ValidateDeactivation(
            entity,
            " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("activatable.actor.invalid");
        entity.IsActive.ShouldBeTrue();
        entity.ActivatedAtUtc.ShouldBe(NowUtc);
    }
}
