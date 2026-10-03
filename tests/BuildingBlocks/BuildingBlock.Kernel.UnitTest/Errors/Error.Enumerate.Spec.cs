using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.UnitTest.Errors;

[Trait("Category", "Unit")]
public class ErrorSeveritySpec
{
    [Theory]
    [InlineData(ErrorSeverity.Info, 0)]
    [InlineData(ErrorSeverity.Warning, 1)]
    [InlineData(ErrorSeverity.Error, 2)]
    [InlineData(ErrorSeverity.Critical, 3)]
    public void ErrorSeverity_Should_Have_Stable_Wire_Values(ErrorSeverity severity, int expected)
    {
        // Arrange
        // Act
        var actual = (int)severity;

        // Assert
        actual.ShouldBe(expected);
    }
}
