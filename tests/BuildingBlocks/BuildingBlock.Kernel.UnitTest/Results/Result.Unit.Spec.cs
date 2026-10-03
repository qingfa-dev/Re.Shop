using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

[Trait("Category", "Unit")]
public class ResultUnitSpec
{
    [Fact]
    public void Unit_Value_Should_Be_Default()
    {
        // Arrange
        // Act
        var actual = Unit.Value;

        // Assert
        actual.Equals(default(Unit)).ShouldBeTrue();
    }

    [Fact]
    public void Unit_Equals_Other_Unit_Should_Always_Be_True()
    {
        // Arrange
        // Act
        var actual = new Unit().Equals(new Unit());

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void Unit_Equals_Object_Should_Range_By_Input()
    {
        // Arrange
        var unit = new Unit();

        // Act
        // Assert
        unit.Equals((object)new Unit()).ShouldBeTrue();
        unit.Equals((object?)null).ShouldBeFalse();
        unit.Equals("not-a-unit").ShouldBeFalse();
    }

    [Fact]
    public void Unit_GetHashCode_Should_Be_Zero()
    {
        // Arrange
        // Act
        var actual = new Unit().GetHashCode();

        // Assert
        actual.ShouldBe(0);
    }

    [Fact]
    public void Unit_ToString_Should_Render_Parentheses()
    {
        // Arrange
        // Act
        var actual = Unit.Value.ToString();

        // Assert
        actual.ShouldBe("()");
    }

    [Fact]
    public void Unit_Equality_Operators_Should_Report_Equal()
    {
        // Arrange
        // Act
        // Assert
        (Unit.Value == default(Unit)).ShouldBeTrue();
        (Unit.Value != default(Unit)).ShouldBeFalse();
    }
}
