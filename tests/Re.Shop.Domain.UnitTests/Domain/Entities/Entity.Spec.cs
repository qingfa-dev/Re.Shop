using Domain.Entities;

namespace Domain.UnitTests.Domain.Entities;

public class EntitySpec
{
    [Fact]
    public void Constructor_ShouldExposeTypedId()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);

        entity.Id.ShouldBe(id);
    }

    [Fact]
    public void Constructor_NullId_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new StringEntity(null!));
    }

    [Fact]
    public void Constructor_DefaultId_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() => new TestEntity(Guid.Empty));
    }

    [Fact]
    public void Equality_SameRuntimeTypeAndId_ShouldIgnoreOtherState()
    {
        var id = Guid.NewGuid();
        var first = new TestEntity(id) { State = "first" };
        var second = new TestEntity(id) { State = "second" };

        first.ShouldBe(second);
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentRuntimeTypeOrId_ShouldBeFalse()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);

        entity.Equals(new TestEntity(Guid.NewGuid())).ShouldBeFalse();
        entity.Equals(new OtherTestEntity(id)).ShouldBeFalse();
        entity.Equals(null).ShouldBeFalse();
        entity.Equals(new object()).ShouldBeFalse();
    }

    private sealed class TestEntity(Guid id) : Entity<Guid>(id)
    {
        public string State { get; set; } = string.Empty;
    }

    private sealed class OtherTestEntity(Guid id) : Entity<Guid>(id)
    {
    }

    private sealed class StringEntity(string id) : Entity<string>(id)
    {
    }
}
