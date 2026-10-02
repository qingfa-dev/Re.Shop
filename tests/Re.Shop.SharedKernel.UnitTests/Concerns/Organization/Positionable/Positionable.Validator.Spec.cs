using SharedKernel.Concerns.Organization.Positionable;

namespace SharedKernel.UnitTests.Concerns.Organization.Positionable;

public class PositionableValidatorSpec
{
    private sealed class TestPositionable : IPositionable
    {
        public int Position { get; set; }
    }

    [Fact]
    public void ValidatePosition_ValidInput_ShouldSucceed()
    {
        var entity = new TestPositionable { Position = 3 };

        var result = PositionableValidator.ValidatePosition(entity, 5);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Position.ShouldBe(3);
    }

    [Fact]
    public void ValidatePosition_NullEntity_ShouldFailEntityRequired()
    {
        var result = PositionableValidator.ValidatePosition(
            (TestPositionable)null!,
            1);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("positionable.entity.required");
    }

    [Fact]
    public void ValidatePosition_NegativePosition_ShouldFailPositionNegative()
    {
        var entity = new TestPositionable { Position = 3 };

        var result = PositionableValidator.ValidatePosition(
            entity,
            PositionableConstant.Constraints.MinPosition - 1);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("positionable.position.negative");
        entity.Position.ShouldBe(3);
    }
}
