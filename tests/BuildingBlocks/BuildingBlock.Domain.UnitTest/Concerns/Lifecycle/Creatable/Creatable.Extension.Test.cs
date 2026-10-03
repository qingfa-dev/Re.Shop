using BuildingBlocks.Domain.Concerns.Lifecycle;
using BuildingBlocks.Domain.Concerns.Lifecycle.Creatable;
namespace BuildingBlocks.Domain.Tests.Concerns.Lifecycle.Creatable;

public class CreatableExtensionTest
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private static Result<ICreatable> Success(ICreatable entity)
    {
        return Result<ICreatable>.Success(entity);
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
        result.Errors![0].Code.ShouldBe("Creatable.CreatedAt.Required");
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
        result.Errors![0].Code.ShouldBe("Creatable.CreatedBy.Required");
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
        result.Errors![0].Code.ShouldBe("Creatable.CreatedAt.AlreadyInitialized");
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
        result.Errors![0].Code.ShouldBe("Creatable.CreatedBy.TooLong");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
    }

    [Fact]
    public void InitializeAudit_UntrimmedActor_ShouldFailActorInvalid()
    {
        var entity = new TestCreatable();

        var result = Success(entity).InitializeAudit(NowUtc, " alice ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Creatable.Actor.Invalid");
        entity.CreatedAtUtc.ShouldBe(default(DateTimeOffset));
    }

    [Fact]
    public void InitializeAudit_EmptyActor_ShouldFailActorInvalid()
    {
        var entity = new TestCreatable();

        var result = Success(entity).InitializeAudit(NowUtc, string.Empty);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Creatable.Actor.Invalid");
    }

    [Fact]
    public void InitializeAudit_InputFailure_ShouldPropagateErrors()
    {
        var input = Result<ICreatable>.Fail(
            Error.UnprocessableEntity("Test.Fail", "boom"));

        var result = input.InitializeAudit(NowUtc, "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Test.Fail");
    }

    #endregion

    #region ValidateCreation

    [Fact]
    public void ValidateCreation_NullEntity_ShouldFailEntityRequired()
    {
        var result = CreatableValidator.ValidateCreation<ICreatable>(
            null!,
            NowUtc,
            "alice");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Creatable.Entity.Required");
    }

    #endregion
}
