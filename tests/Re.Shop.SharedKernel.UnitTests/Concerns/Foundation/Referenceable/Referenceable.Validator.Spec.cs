using SharedKernel.Concerns.Foundation.Referenceable;

namespace SharedKernel.UnitTests.Concerns.Foundation.Referenceable;

public class ReferenceableValidatorSpec
{
    private sealed class TestReferenceable : IReferenceable
    {
        public string Reference { get; set; } = string.Empty;
    }

    [Fact]
    public void ValidateReference_ValidInput_ShouldSucceed()
    {
        var entity = new TestReferenceable { Reference = "keep-me" };

        var result = ReferenceableValidator.ValidateReference(
            entity,
            "ORD-2024-0001");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Reference.ShouldBe("keep-me");
    }

    [Fact]
    public void ValidateReference_NullEntity_ShouldFailEntityRequired()
    {
        var result = ReferenceableValidator.ValidateReference(
            (TestReferenceable)null!,
            "ORD-1");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("referenceable.entity.required");
    }

    [Fact]
    public void ValidateReference_NullReference_ShouldFailReferenceRequired()
    {
        var entity = new TestReferenceable { Reference = "keep-me" };

        var result = ReferenceableValidator.ValidateReference(
            entity,
            null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("referenceable.reference.required");
        entity.Reference.ShouldBe("keep-me");
    }

    [Fact]
    public void ValidateReference_WhitespaceReference_ShouldFailReferenceRequired()
    {
        var entity = new TestReferenceable { Reference = "keep-me" };

        var result = ReferenceableValidator.ValidateReference(
            entity,
            "   ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("referenceable.reference.required");
        entity.Reference.ShouldBe("keep-me");
    }

    [Fact]
    public void ValidateReference_TooLongReference_ShouldFailReferenceTooLong()
    {
        var entity = new TestReferenceable { Reference = "keep-me" };
        var reference = new string(
            'r',
            ReferenceableConstant.Constraints.MaxReferenceLength + 1);

        var result = ReferenceableValidator.ValidateReference(
            entity,
            reference);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("referenceable.reference.too_long");
        entity.Reference.ShouldBe("keep-me");
    }
}
