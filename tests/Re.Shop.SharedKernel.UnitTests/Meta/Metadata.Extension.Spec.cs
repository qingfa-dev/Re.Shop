using SharedKernel.Errors;
using SharedKernel.Meta;
using Shouldly;

namespace SharedKernel.UnitTests.Meta;

public class MetadataExtensionsTest
{
    private sealed class SourceHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata { get; set; } = new();
    }

    private sealed class DestHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata { get; set; } = new();
    }

    private sealed class NullMetadataHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata { get; set; }
    }

    #region GetMetadata

    [Fact]
    public void GetMetadata_KeyExists_ReturnsValue()
    {
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"] = "tenant-1";

        var result = source.GetMetadata("TenantId");

        result.ShouldBe("tenant-1");
    }

    [Fact]
    public void GetMetadata_KeyDoesNotExist_ReturnsNull()
    {
        var source = new SourceHolder();

        var result = source.GetMetadata("missing");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetMetadata_SourceIsNull_ReturnsNull()
    {
        SourceHolder? source = null;

        var result = source.GetMetadata("key");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetMetadata_KeyIsNull_ReturnsNull()
    {
        var source = new SourceHolder();

        var result = source.GetMetadata(null);

        result.ShouldBeNull();
    }

    [Fact]
    public void GetMetadata_StorageIsNull_ReturnsNull()
    {
        var source = new NullMetadataHolder();

        var result = source.GetMetadata("key");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetMetadata_KeyCasingDiffers_ReturnsNull()
    {
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"] = "tenant-1";

        var result = source.GetMetadata("tenantid");

        result.ShouldBeNull();
    }

    #endregion

    #region Fluent metadata helpers

    [Fact]
    public void WithMetadata_ValidParameters_SetsValueAndReturnsSource()
    {
        var source = new SourceHolder();

        var result = source.WithMetadata("TenantId", "tenant-1");

        result.ShouldBeSameAs(source);
        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"].ShouldBe("tenant-1");
    }

    [Fact]
    public void WithAdditionalMetadata_ValidEntries_SetsValuesAndReturnsSource()
    {
        var source = new SourceHolder();
        var additionalMetadata = new Dictionary<string, object?>
        {
            ["TenantId"] = "tenant-1",
            ["Region"] = "us-east"
        };

        var result = source.WithAdditionalMetadata(additionalMetadata);

        result.ShouldBeSameAs(source);
        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"].ShouldBe("tenant-1");
        source.Metadata["Region"].ShouldBe("us-east");
    }

    [Fact]
    public void WithType_ValidType_UsesMetadataTypeKey()
    {
        var source = new SourceHolder();

        var result = source.WithType("problem");

        result.ShouldBeSameAs(source);
        source.Metadata.ShouldNotBeNull();
        source.Metadata[MetadataConstant.MetadataKey.Type].ShouldBe("problem");
    }

    [Fact]
    public void WithTarget_ValidTarget_UsesMetadataTargetKey()
    {
        var source = new SourceHolder();

        var result = source.WithTarget("/orders/1");

        result.ShouldBeSameAs(source);
        source.Metadata.ShouldNotBeNull();
        source.Metadata[MetadataConstant.MetadataKey.Target].ShouldBe("/orders/1");
    }

    [Fact]
    public void MetadataKey_StandardKeys_UseExpectedNames()
    {
        MetadataConstant.MetadataKey.Code.ShouldBe("code");
        MetadataConstant.MetadataKey.Message.ShouldBe("message");
        MetadataConstant.MetadataKey.Status.ShouldBe("status");
        MetadataConstant.MetadataKey.Type.ShouldBe("type");
        MetadataConstant.MetadataKey.Target.ShouldBe("target");
    }

    [Fact]
    public void ErrorFactories_DoNotAcceptMetadataDictionary()
    {
        var errorFactories = typeof(Error)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(method => method.ReturnType == typeof(Error));

        errorFactories.ShouldAllBe(method =>
            !method.GetParameters().Any(parameter =>
                parameter.ParameterType == typeof(Dictionary<string, object?>)));
    }

    [Fact]
    public void ErrorFactory_CreatesBuiltInTypeMetadata()
    {
        var error = Error.BadRequest();

        error.GetMetadata(MetadataConstant.MetadataKey.Type)
            .ShouldBe(ErrorConstant.ErrorTypes.BadRequest.Type);
    }

    #endregion

    #region SetMetadata

    [Fact]
    public void SetMetadata_ValidParameters_SetsValueAndReturnsSource()
    {
        var source = new SourceHolder();

        var result = source.SetMetadata("TenantId", "tenant-1");

        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"].ShouldBe("tenant-1");
        result.ShouldBeSameAs(source);
    }

    [Fact]
    public void SetMetadata_KeyIsWhitespace_ThrowsArgumentNullException()
    {
        var source = new SourceHolder();

        Should.Throw<ArgumentNullException>(() => source.SetMetadata(" ", "value"));
    }

    [Fact]
    public void SetMetadata_ErrorWithNullStorage_InitializesStorage()
    {
        var error = new Error("order.invalid", "Invalid order.", 400);

        var result = MetadataExtension.SetMetadata(error, "key", "value");

        result.Metadata.ShouldNotBeNull();
        result.Metadata["key"].ShouldBe("value");
    }

    [Fact]
    public void SetMetadata_SourceIsNull_ReturnsNull()
    {
        SourceHolder? source = null;

        var result = source.SetMetadata("key", "value");

        result.ShouldBeNull();
    }

    [Fact]
    public void SetMetadata_KeyIsNull_ThrowsArgumentNullException()
    {
        var source = new SourceHolder();

        Should.Throw<ArgumentNullException>(() => source.SetMetadata(null, "value"));
    }

    [Fact]
    public void SetMetadata_ValueIsNull_ThrowsArgumentNullException()
    {
        var source = new SourceHolder();

        Should.Throw<ArgumentNullException>(() => source.SetMetadata("key", null));
    }

    [Fact]
    public void SetMetadata_StorageIsNull_InitializesStorage()
    {
        var source = new NullMetadataHolder();

        var result = source.SetMetadata("key", "value");

        result.ShouldBeSameAs(source);
        source.Metadata.ShouldNotBeNull();
        source.Metadata["key"].ShouldBe("value");
    }

    #endregion

    #region MergeMetadata

    [Fact]
    public void MergeMetadata_SourceHasEntries_CopiesAllEntries()
    {
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"] = "tenant-1";
        source.Metadata["Env"] = "production";

        var dest = new DestHolder();

        dest.Metadata.ShouldNotBeNull();
        dest.Metadata["Existing"] = true;

        var result = dest.MergeMetadata(source);

        result.ShouldBeSameAs(dest);
        dest.Metadata["TenantId"].ShouldBe("tenant-1");
        dest.Metadata["Env"].ShouldBe("production");
        dest.Metadata["Existing"].ShouldBe(true);
    }

    [Fact]
    public void MergeMetadata_DestinationIsNull_ReturnsNull()
    {
        DestHolder? dest = null;
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["key"] = "value";

        var result = dest.MergeMetadata(source);

        result.ShouldBeNull();
    }

    [Fact]
    public void MergeMetadata_SourceIsNull_ReturnsNull()
    {
        var dest = new DestHolder();
        SourceHolder? source = null;

        var result = dest.MergeMetadata(source);

        result.ShouldBeNull();
    }

    [Fact]
    public void MergeMetadata_KeyExists_DoesNotOverwrite()
    {
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["key"] = "source-value";

        var dest = new DestHolder();

        dest.Metadata.ShouldNotBeNull();
        dest.Metadata["key"] = "dest-value";

        dest.MergeMetadata(source);

        dest.Metadata["key"].ShouldBe("dest-value");
    }

    [Fact]
    public void MergeMetadata_DestinationStorageIsNull_ReturnsNull()
    {
        var dest = new NullMetadataHolder();
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["key"] = "value";

        var result = dest.MergeMetadata(source);

        result.ShouldBeNull();
    }

    [Fact]
    public void MergeMetadata_SourceStorageIsNull_ReturnsNull()
    {
        var dest = new DestHolder();
        var source = new NullMetadataHolder();

        var result = dest.MergeMetadata(source);

        result.ShouldBeNull();
    }

    #endregion
}