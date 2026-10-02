using SharedKernel.Concerns.Foundation.Versionable;

namespace SharedKernel.UnitTests.Concerns.Foundation.Versionable;

public class VersionableValidatorSpec
{
    private sealed class TestVersionable : IVersionable
    {
        public long Version { get; set; }
    }

    [Fact]
    public void ValidateBump_ValidInput_ShouldSucceed()
    {
        var entity = new TestVersionable
        {
            Version = VersionableConstant.Constraints.MinVersion
        };

        var result = VersionableValidator.ValidateBump(entity);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Version.ShouldBe(VersionableConstant.Constraints.MinVersion);
    }

    [Fact]
    public void ValidateBump_NullEntity_ShouldFailEntityRequired()
    {
        var result = VersionableValidator.ValidateBump(
            (TestVersionable)null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("versionable.entity.required");
    }

    [Fact]
    public void ValidateBump_NegativeVersion_ShouldFailVersionNegative()
    {
        var entity = new TestVersionable
        {
            Version = VersionableConstant.Constraints.MinVersion - 1
        };

        var result = VersionableValidator.ValidateBump(entity);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("versionable.version.negative");
        entity.Version.ShouldBe(VersionableConstant.Constraints.MinVersion - 1);
    }

    [Fact]
    public void ValidateBump_MaxVersion_ShouldFailVersionOverflow()
    {
        var entity = new TestVersionable
        {
            Version = VersionableConstant.Constraints.MaxVersion
        };

        var result = VersionableValidator.ValidateBump(entity);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("versionable.version.overflow");
        entity.Version.ShouldBe(VersionableConstant.Constraints.MaxVersion);
    }
}
