using SharedKernel.Concerns.Organization.Hierarchical;

namespace SharedKernel.UnitTests.Concerns.Organization.Hierarchical;

public class HierarchicalValidatorSpec
{
    private sealed class TestHierarchical : IHierarchical<Guid>
    {
        public Guid? ParentId { get; set; }
    }

    [Fact]
    public void ValidateParent_ValidInput_ShouldSucceed()
    {
        var existingParentId = Guid.NewGuid();
        var entity = new TestHierarchical { ParentId = existingParentId };

        var result = HierarchicalValidator.ValidateParent<TestHierarchical, Guid>(
            entity,
            Guid.NewGuid());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.ParentId.ShouldBe(existingParentId);
    }

    [Fact]
    public void ValidateParent_NullEntity_ShouldFailEntityRequired()
    {
        var result = HierarchicalValidator.ValidateParent<TestHierarchical, Guid>(
            (TestHierarchical)null!,
            Guid.NewGuid());

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("hierarchical.entity.required");
    }
}
