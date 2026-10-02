using SharedKernel.Concerns.Organization.Positionable;

namespace SharedKernel.UnitTests.Concerns.Organization.Positionable;

public class PositionableExtensionSpec
{
    private sealed class TestPositionable : IPositionable
    {
        public int Position { get; set; }
    }

    private static Result<TestPositionable, Error> Success(
        TestPositionable entity)
    {
        return Result<TestPositionable, Error>.Success(entity);
    }

    [Fact]
    public void SetPosition_PositivePosition_ShouldSetPosition()
    {
        var entity = new TestPositionable();

        var result = Success(entity).SetPosition(5);

        result.IsSuccess.ShouldBeTrue();
        entity.Position.ShouldBe(5);
    }

    [Fact]
    public void SetPosition_ZeroPosition_ShouldSetPosition()
    {
        var entity = new TestPositionable { Position = 7 };

        var result = Success(entity).SetPosition(0);

        result.IsSuccess.ShouldBeTrue();
        entity.Position.ShouldBe(0);
    }

    [Fact]
    public void SetPosition_NegativePosition_ShouldFailWithoutMutation()
    {
        var entity = new TestPositionable { Position = 3 };

        var result = Success(entity).SetPosition(-1);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("positionable.position.negative");
        entity.Position.ShouldBe(3);
    }
}
