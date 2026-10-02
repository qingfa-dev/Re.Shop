using SharedKernel.Concerns.Lifecycle;
using SharedKernel.Concerns.Lifecycle.Creatable;

namespace SharedKernel.UnitTests.Concerns.Lifecycle.Creatable;

public class CreatableValidatorSpec
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private sealed class TestCreatable : ICreatable
    {
        public DateTimeOffset CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
    }

    [Fact]
    public void ValidateCreation_ValidInput_ShouldSucceed()
    {
        var entity = new TestCreatable();

        var result = CreatableValidator.ValidateCreation(
            entity,
            NowUtc,
            "alice");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
        entity.CreatedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateCreation_NullEntity_ShouldFailEntityRequired()
    {
        var result = CreatableValidator.ValidateCreation(
            (TestCreatable)null!,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.entity.required");
    }

    [Fact]
    public void ValidateCreation_DefaultNowUtc_ShouldFailCreatedAtRequired()
    {
        var entity = new TestCreatable();

        var result = CreatableValidator.ValidateCreation(
            entity,
            default,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_at.required");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
        entity.CreatedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateCreation_AlreadyInitialized_ShouldFailAlreadyInitialized()
    {
        var entity = new TestCreatable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "bob"
        };

        var result = CreatableValidator.ValidateCreation(
            entity,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_at.already_initialized");
        entity.CreatedAtUtc.ShouldBe(NowUtc);
        entity.CreatedBy.ShouldBe("bob");
    }

    [Fact]
    public void ValidateCreation_ActorTooLong_ShouldFailCreatedByTooLong()
    {
        var entity = new TestCreatable();
        var actor = new string(
            'a',
            LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = CreatableValidator.ValidateCreation(
            entity,
            NowUtc,
            actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_by.too_long");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
        entity.CreatedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateCreation_MissingActor_ShouldFailCreatedByRequired()
    {
        var entity = new TestCreatable();

        var result = CreatableValidator.ValidateCreation(
            entity,
            NowUtc,
            actor: null,
            requireCreatedBy: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_by.required");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
        entity.CreatedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateCreation_WhitespaceActor_ShouldFailCreatedByRequired()
    {
        var entity = new TestCreatable();

        var result = CreatableValidator.ValidateCreation(
            entity,
            NowUtc,
            actor: "   ",
            requireCreatedBy: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_by.required");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
        entity.CreatedBy.ShouldBeNull();
    }

    [Fact]
    public void ValidateCreation_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestCreatable();

        var result = CreatableValidator.ValidateCreation(
            entity,
            NowUtc,
            " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.actor.invalid");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
        entity.CreatedBy.ShouldBeNull();
    }
}
