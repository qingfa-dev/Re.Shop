using SharedKernel.Concerns.Foundation.Referenceable;

namespace SharedKernel.UnitTests.Concerns.Foundation.Referenceable;

public class ReferenceableExtensionSpec
{
    private sealed class TestReferenceable : IReferenceable
    {
        public string Reference { get; set; } = string.Empty;
    }

    private static Result<TestReferenceable, Error> Success(
        TestReferenceable entity)
    {
        return Result<TestReferenceable, Error>.Success(entity);
    }

    [Fact]
    public void SetReference_ValidReference_ShouldSetReference()
    {
        var entity = new TestReferenceable();

        var result = Success(entity).SetReference("ORD-2024-0001");

        result.IsSuccess.ShouldBeTrue();
        entity.Reference.ShouldBe("ORD-2024-0001");
    }

    [Fact]
    public void SetReference_WhitespaceReference_ShouldFailWithoutMutation()
    {
        var entity = new TestReferenceable { Reference = "keep-me" };

        var result = Success(entity).SetReference("   ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("referenceable.reference.required");
        entity.Reference.ShouldBe("keep-me");
    }

    [Fact]
    public void SetReference_TooLongReference_ShouldFailWithoutMutation()
    {
        var entity = new TestReferenceable { Reference = "keep-me" };
        var reference = new string(
            'r',
            ReferenceableConstant.Constraints.MaxReferenceLength + 1);

        var result = Success(entity).SetReference(reference);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("referenceable.reference.too_long");
        entity.Reference.ShouldBe("keep-me");
    }
}
