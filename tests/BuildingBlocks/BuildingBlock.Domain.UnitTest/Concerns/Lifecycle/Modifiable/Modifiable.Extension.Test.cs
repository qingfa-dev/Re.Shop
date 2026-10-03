using BuildingBlocks.Domain.Concerns.Lifecycle;
using BuildingBlocks.Domain.Concerns.Lifecycle.Creatable;
using BuildingBlocks.Domain.Concerns.Lifecycle.Modifiable;
namespace BuildingBlocks.Domain.Tests.Concerns.Lifecycle.Modifiable;

public class ModifiableExtensionTest
{
    private static readonly DateTimeOffset NowUtc =
        new(2024, 5, 1, 12, 0, 0, TimeSpan.Zero);

    private static Result<TestModifiable> Success(TestModifiable entity)
    {
        return Result<TestModifiable>.Success(entity);
    }

    private sealed class TestModifiable : ICreatable, IModifiable
    {
        public DateTimeOffset CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedAtUtc { get; set; }
        public string? ModifiedBy { get; set; }
    }

    private static TestModifiable InitializedEntity(
        string? createdBy = "alice",
        DateTimeOffset? createdAtUtc = null)
    {
        return new TestModifiable
        {
            CreatedAtUtc = createdAtUtc ?? NowUtc,
            CreatedBy = createdBy
        };
    }

    #region MarkModified

    [Fact]
    public void MarkModified_InitializedEntity_ShouldSetModifiedFields()
    {
        var entity = InitializedEntity();

        var result = Success(entity).MarkModified(NowUtc.AddHours(1), "carol");

        result.IsSuccess.ShouldBeTrue();
        entity.ModifiedAtUtc.ShouldBe(NowUtc.AddHours(1));
        entity.ModifiedBy.ShouldBe("carol");
        entity.CreatedAtUtc.ShouldBe(NowUtc);
        entity.CreatedBy.ShouldBe("alice");
    }

    [Fact]
    public void MarkModified_OffsetTimestamp_ShouldStoreUtcInstant()
    {
        var entity = InitializedEntity();
        var offset = new DateTimeOffset(2024, 5, 1, 20, 0, 0, TimeSpan.FromHours(8));

        var result = Success(entity).MarkModified(offset, "carol");

        result.IsSuccess.ShouldBeTrue();
        entity.ModifiedAtUtc!.Value.Offset.ShouldBe(TimeSpan.Zero);
        entity.ModifiedAtUtc.ShouldBe(offset.ToUniversalTime());
    }

    [Fact]
    public void MarkModified_NullActor_ShouldClearModifiedBy()
    {
        var entity = InitializedEntity();
        entity.ModifiedBy = "stale";

        var result = Success(entity).MarkModified(NowUtc.AddHours(1));

        result.IsSuccess.ShouldBeTrue();
        entity.ModifiedBy.ShouldBeNull();
    }

    [Fact]
    public void MarkModified_DefaultNowUtc_ShouldFailModifiedAtRequiredWithoutMutation()
    {
        var entity = InitializedEntity();

        var result = Success(entity).MarkModified(default, "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Modifiable.ModifiedAt.Required");
        entity.ModifiedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void MarkModified_UninitializedEntity_ShouldFailNotInitializedWithoutMutation()
    {
        var entity = new TestModifiable();

        var result = Success(entity).MarkModified(NowUtc, "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Modifiable.NotInitialized");
        entity.ModifiedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void MarkModified_BeforeCreatedAt_ShouldFailModifiedAtBeforeCreatedAt()
    {
        var entity = InitializedEntity();

        var result = Success(entity).MarkModified(NowUtc.AddMinutes(-1), "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Modifiable.ModifiedAt.InvalidRange");
        entity.ModifiedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void MarkModified_RequireModifiedByWithoutActor_ShouldFailWithoutMutation()
    {
        var entity = InitializedEntity();

        var result = Success(entity).MarkModified(
            NowUtc.AddHours(1),
            actor: null,
            requireModifiedBy: true);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Modifiable.ModifiedBy.Required");
        entity.ModifiedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void MarkModified_ActorTooLong_ShouldFailModifiedByTooLongWithoutMutation()
    {
        var entity = InitializedEntity();
        var actor = new string('a', LifecycleConstant.Constraints.MaxActorLength + 1);

        var result = Success(entity).MarkModified(NowUtc.AddHours(1), actor);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Modifiable.ModifiedBy.TooLong");
        entity.ModifiedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void MarkModified_InputFailure_ShouldPropagateErrors()
    {
        var input = Result<TestModifiable>.Fail(
            Error.UnprocessableEntity("Test.Fail", "boom"));

        var result = input.MarkModified(NowUtc, "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Test.Fail");
    }

    #endregion

    #region ValidateModification

    [Fact]
    public void ValidateModification_NullEntity_ShouldFailEntityRequired()
    {
        var result = ModifiableValidator.ValidateModification<TestModifiable>(
            null!,
            NowUtc,
            "carol");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Modifiable.Entity.Required");
    }

    #endregion
}
