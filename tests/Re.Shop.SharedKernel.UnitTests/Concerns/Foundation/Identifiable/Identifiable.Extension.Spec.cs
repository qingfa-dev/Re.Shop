using SharedKernel.Concerns.Foundation.Identifiable;

namespace SharedKernel.UnitTests.Concerns.Foundation.Identifiable;

public class IdentifiableExtensionSpec
{
    private sealed class TestIdentifiable : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
    }

    private sealed class TestStringIdentifiable : IIdentifiable<string>
    {
        public string Id { get; set; } = string.Empty;
    }

    private static Result<TestIdentifiable, Error> Success(TestIdentifiable entity)
    {
        return Result<TestIdentifiable, Error>.Success(entity);
    }

    [Fact]
    public void EnsureIdentified_WithId_ShouldSucceed()
    {
        var entity = new TestIdentifiable { Id = Guid.NewGuid() };

        var result = Success(entity).EnsureIdentified<TestIdentifiable, Guid>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
    }

    [Fact]
    public void EnsureIdentified_DefaultId_ShouldFailIdRequired()
    {
        var entity = new TestIdentifiable();

        var result = Success(entity).EnsureIdentified<TestIdentifiable, Guid>();

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("identifiable.id.required");
    }

    [Fact]
    public void EnsureIdentified_NullStringId_ShouldFailIdRequired()
    {
        var entity = new TestStringIdentifiable { Id = null! };

        var result = Result<TestStringIdentifiable, Error>.Success(entity)
            .EnsureIdentified<TestStringIdentifiable, string>();

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("identifiable.id.required");
    }
}
