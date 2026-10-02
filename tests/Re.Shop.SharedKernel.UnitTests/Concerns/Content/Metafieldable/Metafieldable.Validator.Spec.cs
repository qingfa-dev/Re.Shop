using SharedKernel.Concerns.Content.Metafieldable;

namespace SharedKernel.UnitTests.Concerns.Content.Metafieldable;

public class MetafieldableValidatorSpec
{
    private sealed class TestMetafieldable : IMetafieldable
    {
        public ICollection<Metafield> Metafields { get; } =
            new List<Metafield>();
    }

    [Fact]
    public void ValidateMetafield_ValidInput_ShouldSucceed()
    {
        var entity = new TestMetafieldable();

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            "specs",
            "weight");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafield_NullEntity_ShouldFailEntityRequired()
    {
        var result = MetafieldableValidator.ValidateMetafield(
            (TestMetafieldable)null!,
            "specs",
            "weight");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.entity.required");
    }

    [Fact]
    public void ValidateMetafield_NullNamespace_ShouldFailNamespaceRequired()
    {
        var entity = new TestMetafieldable();

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            null,
            "weight");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.namespace.required");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafield_WhitespaceNamespace_ShouldFailNamespaceRequired()
    {
        var entity = new TestMetafieldable();

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            "   ",
            "weight");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.namespace.required");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafield_TooLongNamespace_ShouldFailNamespaceTooLong()
    {
        var entity = new TestMetafieldable();
        var @namespace = new string(
            'n',
            MetafieldableConstant.Constraints.MaxNamespaceLength + 1);

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            @namespace,
            "weight");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.namespace.too_long");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafield_InvalidNamespace_ShouldFailNamespaceInvalid()
    {
        var entity = new TestMetafieldable();

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            "Bad NS",
            "weight");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.namespace.invalid");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafield_NullKey_ShouldFailKeyRequired()
    {
        var entity = new TestMetafieldable();

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            "specs",
            null);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.key.required");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafield_WhitespaceKey_ShouldFailKeyRequired()
    {
        var entity = new TestMetafieldable();

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            "specs",
            "   ");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.key.required");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafield_TooLongKey_ShouldFailKeyTooLong()
    {
        var entity = new TestMetafieldable();
        var key = new string(
            'k',
            MetafieldableConstant.Constraints.MaxKeyLength + 1);

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            "specs",
            key);

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.key.too_long");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafield_InvalidKey_ShouldFailKeyInvalid()
    {
        var entity = new TestMetafieldable();

        var result = MetafieldableValidator.ValidateMetafield(
            entity,
            "specs",
            "Bad-Key");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.key.invalid");
        entity.Metafields.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateMetafieldRemoval_ValidInput_ShouldSucceed()
    {
        var entity = new TestMetafieldable();
        entity.Metafields.Add(new Metafield
        {
            Namespace = "specs",
            Key = "weight",
        });

        var result = MetafieldableValidator.ValidateMetafieldRemoval(
            entity,
            "specs",
            "weight");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Metafields.ShouldHaveSingleItem();
    }

    [Fact]
    public void ValidateMetafieldRemoval_NullEntity_ShouldFailEntityRequired()
    {
        var result = MetafieldableValidator.ValidateMetafieldRemoval(
            (TestMetafieldable)null!,
            "specs",
            "weight");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.entity.required");
    }

    [Fact]
    public void ValidateMetafieldRemoval_MissingMetafield_ShouldFailMetafieldNotFound()
    {
        var entity = new TestMetafieldable();
        entity.Metafields.Add(new Metafield
        {
            Namespace = "specs",
            Key = "weight",
        });

        var result = MetafieldableValidator.ValidateMetafieldRemoval(
            entity,
            "specs",
            "depth");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("metafieldable.metafield.not_found");
        entity.Metafields.ShouldHaveSingleItem();
    }
}
