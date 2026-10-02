namespace BuildingBlock.Kernel.UnitTest;

public class BuildingBlockMarkerSpec
{
    [Fact]
    public void BuildingBlockMarker_ShouldBe_A_Static_StaticClass()
    {
        // Arrange
        var type = typeof(BuildingBlockMarker);

        // Act
        // (no action needed)

        // Assert
        type.IsAbstract.ShouldBeTrue();
        type.IsSealed.ShouldBeTrue();
    }
}