using SharedKernel.Concerns.Lifecycle;
using SharedKernel.Concerns.Lifecycle.Creatable;
using SharedKernel.Concerns.Lifecycle.Modifiable;

namespace SharedKernel.UnitTests.Concerns.Lifecycle.Modifiable;

public class ModifiableValidatorSpec
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private sealed class TestModifiable : ICreatable, IModifiable
    {
        public DateTimeOffset CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedAtUtc { get; set; }
        public string? ModifiedBy { get; set; }
    }

    [Fact]
    public void ValidateModification_ValidInput_ShouldSucceed()
    {
        var entity = new TestModifiable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "alice"
        };

        var result = ModifiableValidator.ValidateModification(
            entity,
            NowUtc.AddHours(1),
            "carol");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.ModifiedAtUtc.ShouldBeNull();
        entity.ModifiedBy.ShouldBeNull();
        entity.CreatedAtUtc.ShouldBe(NowUtc);
        entity.CreatedBy.ShouldBe("alice");
    }

    [Fact]
    public void ValidateModification_NullEntity_ShouldFailEntityRequired()
    {
        var result = ModifiableValidator.ValidateModification(
            (TestModifiable)null!,
            NowUtc,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("modifiable.entity.required");
    }

    [Fact]
    public void ValidateModification_DefaultNowUtc_ShouldFailModifiedAtRequired()
    {
        var entity = new TestModifiable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "alice"
        };

        var result = ModifiableValidator.ValidateModification(
            entity,
            default,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("modifiable.modified_at.required");
        entity.ModifiedAtUtc.ShouldBeNull();
        entity.ModifiedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateModification_NotInitialized_ShouldFailNotInitialized()
    {
        var entity = new TestModifiable();

        var result = ModifiableValidator.ValidateModification(
            entity,
            NowUtc,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("modifiable.state.not_initialized");
        entity.ModifiedAtUtc.ShouldBeNull();
        entity.ModifiedBy.ShouldBeNull();
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
    }

    [Fact]
    public void ValidateModification_BeforeCreatedAt_ShouldFailModifiedAtBeforeCreatedAt()
    {
        var entity = new TestModifiable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "alice"
        };

        var result = ModifiableValidator.ValidateModification(
            entity,
            NowUtc.AddMinutes(-1),
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("modifiable.modified_at.invalid_range");
        entity.ModifiedAtUtc.ShouldBeNull();
        entity.ModifiedBy.ShouldBeNull();
        entity.CreatedAtUtc.ShouldBe(NowUtc);
    }

    [Fact]
    public void ValidateModification_ActorTooLong_ShouldFailModifiedByTooLong()
    {
        var entity = new TestModifiable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "alice"
        };
        var actor = new string(
            'a',
            LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = ModifiableValidator.ValidateModification(
            entity,
            NowUtc.AddHours(1),
            actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("modifiable.modified_by.too_long");
        entity.ModifiedAtUtc.ShouldBeNull();
        entity.ModifiedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateModification_MissingActor_ShouldFailModifiedByRequired()
    {
        var entity = new TestModifiable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "alice"
        };

        var result = ModifiableValidator.ValidateModification(
            entity,
            NowUtc.AddHours(1),
            actor: null,
            requireModifiedBy: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("modifiable.modified_by.required");
        entity.ModifiedAtUtc.ShouldBeNull();
        entity.ModifiedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateModification_WhitespaceActor_ShouldFailModifiedByRequired()
    {
        var entity = new TestModifiable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "alice"
        };

        var result = ModifiableValidator.ValidateModification(
            entity,
            NowUtc.AddHours(1),
            actor: "   ",
            requireModifiedBy: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("modifiable.modified_by.required");
        entity.ModifiedAtUtc.ShouldBeNull();
        entity.ModifiedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateModification_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestModifiable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "alice"
        };

        var result = ModifiableValidator.ValidateModification(
            entity,
            NowUtc.AddHours(1),
            " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("modifiable.actor.invalid");
        entity.ModifiedAtUtc.ShouldBeNull();
        entity.ModifiedBy.ShouldBeNull();
    }
}
