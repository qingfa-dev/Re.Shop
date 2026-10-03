using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for the <see cref="Unit"/> struct.</summary>
[Trait("Category", "Unit")]
public class ResultUnitSpec
{
    #region Value

    /// <summary>Unit.Value should be the default instance.</summary>
    [Fact]
    public void Unit_Value_Should_Be_Default()
    {
        // Arrange & Act
        var actual = Unit.Value;

        // Assert
        actual.Equals(default(Unit)).ShouldBeTrue();
    }

    #endregion

    #region Equality

    /// <summary>Two units should always be equal.</summary>
    [Fact]
    public void Unit_Equals_Other_Unit_Should_Always_Be_True()
    {
        // Arrange & Act
        var actual = new Unit().Equals(new Unit());

        // Assert
        actual.ShouldBeTrue();
    }

    /// <summary>Unit.Equals(object) should range by input type.</summary>
    [Theory]
    [MemberData(nameof(EqualsObjectCases))]
    public void Unit_Equals_Object_Should_Range_By_Input(object? other, bool expected)
    {
        // Arrange
        var unit = new Unit();

        // Act & Assert
        unit.Equals(other).ShouldBe(expected);
    }

    public static TheoryData<object?, bool> EqualsObjectCases()
    {
        var data = new TheoryData<object?, bool>();
        data.Add(new Unit(), true);
        data.Add(null, false);
        data.Add("not-a-unit", false);
        return data;
    }

    /// <summary>Unit.GetHashCode should return 0.</summary>
    [Fact]
    public void Unit_GetHashCode_Should_Be_Zero()
    {
        // Arrange & Act
        var actual = new Unit().GetHashCode();

        // Assert
        actual.ShouldBe(0);
    }

    /// <summary>Unit equality operators should report correct results.</summary>
    [Fact]
    public void Unit_Equality_Operators_Should_Report_Equal()
    {
        // Arrange & Act & Assert
        (Unit.Value == default(Unit)).ShouldBeTrue();
        (Unit.Value != default(Unit)).ShouldBeFalse();
    }

    #endregion

    #region ToString

    /// <summary>Unit.ToString should render parentheses.</summary>
    [Fact]
    public void Unit_ToString_Should_Render_Parentheses()
    {
        // Arrange & Act
        var actual = Unit.Value.ToString();

        // Assert
        actual.ShouldBe("()");
    }

    #endregion
}
