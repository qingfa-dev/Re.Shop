using BuildingBlocks.Domain.Concerns.Foundation.Versionable;
namespace BuildingBlocks.Domain.Tests.Concerns.Foundation.Versionable;

public class VersionableExtensionTest
{
    private sealed class TestVersionable : IVersionable
    {
        public long Version { get; set; }
    }

    private static Result<TestVersionable> Success(TestVersionable entity)
    {
        return Result<TestVersionable>.Success(entity);
    }

    [Fact]
    public void BumpVersion_ZeroVersion_ShouldIncrementToOne()
    {
        var entity = new TestVersionable();

        var result = Success(entity).BumpVersion();

        result.IsSuccess.ShouldBeTrue();
        entity.Version.ShouldBe(1);
    }

    [Fact]
    public void BumpVersion_NegativeVersion_ShouldFailWithoutMutation()
    {
        var entity = new TestVersionable { Version = -1 };

        var result = Success(entity).BumpVersion();

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Versionable.Version.Negative");
        entity.Version.ShouldBe(-1);
    }

    [Fact]
    public void BumpVersion_MaxVersion_ShouldFailOverflowWithoutMutation()
    {
        var entity = new TestVersionable { Version = long.MaxValue };

        var result = Success(entity).BumpVersion();

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Versionable.Version.Overflow");
        entity.Version.ShouldBe(long.MaxValue);
    }

    [Fact]
    public void ValidateBump_NullEntity_ShouldFailEntityRequired()
    {
        var result = VersionableValidator.ValidateBump(
            (TestVersionable)null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("Versionable.Entity.Required");
    }
}
