using SharedKernel.Concerns.Foundation.Versionable;

namespace SharedKernel.UnitTests.Concerns.Foundation.Versionable;

public class VersionableExtensionSpec
{
    private sealed class TestVersionable : IVersionable
    {
        public long Version { get; set; }
    }

    private static Result<TestVersionable, Error> Success(TestVersionable entity)
    {
        return Result<TestVersionable, Error>.Success(entity);
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
        result.Errors![0].Code.ShouldBe("versionable.version.negative");
        entity.Version.ShouldBe(-1);
    }

    [Fact]
    public void BumpVersion_MaxVersion_ShouldFailOverflowWithoutMutation()
    {
        var entity = new TestVersionable { Version = long.MaxValue };

        var result = Success(entity).BumpVersion();

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("versionable.version.overflow");
        entity.Version.ShouldBe(long.MaxValue);
    }
}
