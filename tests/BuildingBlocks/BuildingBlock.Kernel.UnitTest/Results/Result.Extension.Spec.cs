using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class ResultExtensionSpec
{
    [Fact]
    public void NonGeneric_Metadata_And_Status_Extensions_Should_Return_Copies()
    {
        var original = Result.Ok()
            .WithMetadata("source", "original");

        var copy = original
            .WithMetadata("source", "updated")
            .WithStatus(ResultConstant.StatusCode.Accepted);

        copy.ShouldNotBeSameAs(original);
        copy.Metadata["source"].ShouldBe("updated");
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
        original.Metadata["source"].ShouldBe("original");
        original.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    [Fact]
    public void Generic_Metadata_And_Status_Extensions_Should_Return_Copies()
    {
        var original = Result<int>.Ok(5)
            .WithMetadata("source", "original");

        var copy = original
            .WithMetadata("source", "updated")
            .WithStatus(ResultConstant.StatusCode.Accepted);

        copy.ShouldNotBeSameAs(original);
        copy.Metadata["source"].ShouldBe("updated");
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
        copy.Value.ShouldBe(5);
        original.Metadata["source"].ShouldBe("original");
        original.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }
}
