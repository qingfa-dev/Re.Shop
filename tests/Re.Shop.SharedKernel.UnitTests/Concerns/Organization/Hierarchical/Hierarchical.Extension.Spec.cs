using SharedKernel.Concerns.Organization.Hierarchical;

namespace SharedKernel.UnitTests.Concerns.Organization.Hierarchical;

public class HierarchicalExtensionSpec
{
    private sealed class TestHierarchical : IHierarchical<Guid>
    {
        public Guid? ParentId { get; set; }
    }

    private static Result<TestHierarchical, Error> Success(
        TestHierarchical entity)
    {
        return Result<TestHierarchical, Error>.Success(entity);
    }

    [Fact]
    public void SetParent_WithParentId_ShouldSetParentId()
    {
        var entity = new TestHierarchical();
        var parentId = Guid.NewGuid();

        var result = Success(entity).SetParent(parentId);

        result.IsSuccess.ShouldBeTrue();
        entity.ParentId.ShouldBe(parentId);
    }

    [Fact]
    public void SetParent_Null_ShouldClearParentId()
    {
        var entity = new TestHierarchical
        {
            ParentId = Guid.NewGuid()
        };

        var result = Success(entity)
            .SetParent<TestHierarchical, Guid>(null);

        result.IsSuccess.ShouldBeTrue();
        entity.ParentId.ShouldBeNull();
    }

    [Fact]
    public void SetParent_NewParent_ShouldReplaceParentId()
    {
        var entity = new TestHierarchical
        {
            ParentId = Guid.NewGuid()
        };
        var parentId = Guid.NewGuid();

        var result = Success(entity).SetParent(parentId);

        result.IsSuccess.ShouldBeTrue();
        entity.ParentId.ShouldBe(parentId);
    }
}
