using SharedKernel.Concerns.Foundation.Identifiable;

namespace SharedKernel.UnitTests.Concerns.Foundation.Identifiable;

public class IdentifiableValidatorSpec
{
    private sealed class TestIdentifiable : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
    }

    [Fact]
    public void ValidateIdentification_ValidInput_ShouldSucceed()
    {
        var id = Guid.NewGuid();
        var entity = new TestIdentifiable { Id = id };

        var result = IdentifiableValidator.ValidateIdentification<
            TestIdentifiable,
            Guid>(entity);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Id.ShouldBe(id);
    }

    [Fact]
    public void ValidateIdentification_NullEntity_ShouldFailEntityRequired()
    {
        var result = IdentifiableValidator.ValidateIdentification<
            TestIdentifiable,
            Guid>(null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("identifiable.entity.required");
    }

    [Fact]
    public void ValidateIdentification_DefaultId_ShouldFailIdRequired()
    {
        var entity = new TestIdentifiable();

        var result = IdentifiableValidator.ValidateIdentification<
            TestIdentifiable,
            Guid>(entity);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("identifiable.id.required");
        entity.Id.ShouldBe(Guid.Empty);
    }
}
