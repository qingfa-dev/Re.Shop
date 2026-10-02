using SharedKernel.Concerns.Content.Metafieldable;

namespace SharedKernel.UnitTests.Concerns.Content.Metafieldable;

public class MetafieldableExtensionSpec
{
    private sealed class TestMetafieldable : IMetafieldable
    {
        public ICollection<Metafield> Metafields { get; } =
            new List<Metafield>();
    }

    private static Result<TestMetafieldable, Error> Success(
        TestMetafieldable entity)
    {
        return Result<TestMetafieldable, Error>.Success(entity);
    }

    [Fact]
    public void SetMetafield_NewKey_ShouldAddMetafield()
    {
        var entity = new TestMetafieldable();

        var result = Success(entity)
            .SetMetafield("specs", "weight", MetafieldType.Number, "1.5");

        result.IsSuccess.ShouldBeTrue();
        var metafield = entity.Metafields.ShouldHaveSingleItem();
        metafield.Namespace.ShouldBe("specs");
        metafield.Key.ShouldBe("weight");
        metafield.Type.ShouldBe(MetafieldType.Number);
        metafield.Value.ShouldBe("1.5");
    }

    [Fact]
    public void SetMetafield_ExistingKey_ShouldUpdateTypeAndValue()
    {
        var entity = new TestMetafieldable();
        Success(entity)
            .SetMetafield("specs", "weight", MetafieldType.ShortText, "old");

        var result = Success(entity)
            .SetMetafield("specs", "weight", MetafieldType.Number, "2.0");

        result.IsSuccess.ShouldBeTrue();
        var metafield = entity.Metafields.ShouldHaveSingleItem();
        metafield.Type.ShouldBe(MetafieldType.Number);
        metafield.Value.ShouldBe("2.0");
    }

    [Fact]
    public void SetMetafield_InvalidNamespace_ShouldFailWithoutMutation()
    {
        var entity = new TestMetafieldable();

        var result = Success(entity)
            .SetMetafield("Bad NS", "weight", MetafieldType.Number, "1.5");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.namespace.invalid");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void SetMetafield_TooLongNamespace_ShouldFailWithoutMutation()
    {
        var entity = new TestMetafieldable();
        var @namespace = new string(
            'n',
            MetafieldableConstant.Constraints.MaxNamespaceLength + 1);

        var result = Success(entity)
            .SetMetafield(@namespace, "weight", MetafieldType.Number, "1.5");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.namespace.too_long");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void SetMetafield_NullKey_ShouldFailWithoutMutation()
    {
        var entity = new TestMetafieldable();

        var result = Success(entity)
            .SetMetafield("specs", null, MetafieldType.Number, "1.5");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.key.required");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void SetMetafield_InvalidKey_ShouldFailWithoutMutation()
    {
        var entity = new TestMetafieldable();

        var result = Success(entity)
            .SetMetafield("specs", "Bad-Key", MetafieldType.Number, "1.5");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.key.invalid");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveMetafield_ExistingKey_ShouldRemoveMetafield()
    {
        var entity = new TestMetafieldable();
        Success(entity)
            .SetMetafield("specs", "weight", MetafieldType.Number, "1.5");

        var result = Success(entity).RemoveMetafield("specs", "weight");

        result.IsSuccess.ShouldBeTrue();
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveMetafield_MissingKey_ShouldFailWithoutMutation()
    {
        var entity = new TestMetafieldable();
        Success(entity)
            .SetMetafield("specs", "weight", MetafieldType.Number, "1.5");

        var result = Success(entity).RemoveMetafield("specs", "depth");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.metafield.not_found");
        entity.Metafields.ShouldHaveSingleItem();
    }
}
