using SharedKernel.Concerns.Lifecycle;
using SharedKernel.Concerns.Lifecycle.Creatable;

namespace SharedKernel.UnitTests.Concerns.Lifecycle.Creatable;

public class CreatableExtensionSpec
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private static Result<ICreatable, Error> Success(ICreatable entity)
    {
        return Result<ICreatable, Error>.Success(entity);
    }

    private sealed class TestCreatable : ICreatable
    {
        public DateTimeOffset CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
    }

    #region InitializeAudit

    [Fact]
    public void InitializeAudit_NewEntity_ShouldSetCreationFields()
    {
        var entity = new TestCreatable();

        var result = Success(entity).InitializeAudit(NowUtc, "alice");

        result.IsSuccess.ShouldBeTrue();
        entity.CreatedAtUtc.ShouldBe(NowUtc);
        entity.CreatedBy.ShouldBe("alice");
    }

    [Fact]
    public void InitializeAudit_OffsetTimestamp_ShouldStoreUtcInstant()
    {
        var entity = new TestCreatable();
        var offset = new DateTimeOffset(2024, 5, 1, 20, 0, 0, TimeSpan.FromHours(8));

        var result = Success(entity).InitializeAudit(offset, "alice");

        result.IsSuccess.ShouldBeTrue();
        entity.CreatedAtUtc.Offset.ShouldBe(TimeSpan.Zero);
        entity.CreatedAtUtc.ShouldBe(offset.ToUniversalTime());
    }

    [Fact]
    public void InitializeAudit_DefaultNowUtc_ShouldFailWithoutMutation()
    {
        var entity = new TestCreatable();

        var result = Success(entity).InitializeAudit(default, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_at.required");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
        entity.CreatedBy.ShouldBeNull();
    }

    [Fact]
    public void InitializeAudit_RequireCreatedByWithoutActor_ShouldFailWithoutMutation()
    {
        var entity = new TestCreatable();

        var result = Success(entity).InitializeAudit(
            NowUtc,
            actor: null,
            requireCreatedBy: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_by.required");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
    }

    [Fact]
    public void InitializeAudit_AlreadyInitialized_ShouldFailAndKeepOriginalValues()
    {
        var entity = new TestCreatable
        {
            CreatedAtUtc = NowUtc,
            CreatedBy = "bob"
        };

        var result = Success(entity).InitializeAudit(NowUtc, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_at.already_initialized");
        entity.CreatedAtUtc.ShouldBe(NowUtc);
        entity.CreatedBy.ShouldBe("bob");
    }

    [Fact]
    public void InitializeAudit_ActorTooLong_ShouldFailCreatedByTooLong()
    {
        var entity = new TestCreatable();
        var actor = new string('a', LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = Success(entity).InitializeAudit(NowUtc, actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.created_by.too_long");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
    }

    [Fact]
    public void InitializeAudit_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestCreatable();

        var result = Success(entity).InitializeAudit(NowUtc, " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.actor.invalid");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
    }

    [Fact]
    public void InitializeAudit_EmptyActor_ShouldFailActorInvalid()
    {
        var entity = new TestCreatable();

        var result = Success(entity).InitializeAudit(NowUtc, string.Empty);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("creatable.actor.invalid");
    }

    [Fact]
    public void InitializeAudit_InputFailure_ShouldPropagateErrors()
    {
        var input = Result<ICreatable, Error>.Failure(
            Error.Validation("Test.Fail", "boom"));

        var result = input.InitializeAudit(NowUtc, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Test.Fail");
    }

    #endregion
}
