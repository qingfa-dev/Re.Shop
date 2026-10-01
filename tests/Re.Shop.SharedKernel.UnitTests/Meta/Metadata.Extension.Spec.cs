using SharedKernel.Meta;
using Shouldly;

namespace SharedKernel.UnitTests.Meta;

public class MetadataExtensionsTest
{
    private sealed class SourceHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata { get; } = new();
    }

    private sealed class DestHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata { get; } = new();
    }

    private sealed class NullMetadataHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata => null;
    }

    #region GetMetadata

    [Fact]
    public void GetMetadata_ExistingKey_ShouldReturnValue()
    {
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"] = "tenant-1";

        var result = source.GetMetadata("TenantId");

        result.ShouldBe("tenant-1");
    }

    [Fact]
    public void GetMetadata_NonExistingKey_ShouldReturnNull()
    {
        var source = new SourceHolder();

        var result = source.GetMetadata("missing");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetMetadata_NullSource_ShouldReturnNull()
    {
        SourceHolder? source = null;

        var result = source.GetMetadata("key");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetMetadata_NullKey_ShouldReturnNull()
    {
        var source = new SourceHolder();

        var result = source.GetMetadata(null);

        result.ShouldBeNull();
    }

    [Fact]
    public void GetMetadata_NullMetadataStorage_ShouldReturnNull()
    {
        var source = new NullMetadataHolder();

        var result = source.GetMetadata("key");

        result.ShouldBeNull();
    }

    [Fact]
    public void GetMetadata_DifferentCasing_ShouldReturnNull()
    {
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"] = "tenant-1";

        var result = source.GetMetadata("tenantid");

        result.ShouldBeNull();
    }

    #endregion

    #region SetMetadata

    [Fact]
    public void SetMetadata_ShouldSetValueAndReturnSource()
    {
        var source = new SourceHolder();

        var result = source.SetMetadata("TenantId", "tenant-1");

        source.Metadata.ShouldNotBeNull();
        source.Metadata["TenantId"].ShouldBe("tenant-1");
        result.ShouldBeSameAs(source);
    }

    [Fact]
    public void SetMetadata_NullSource_ShouldReturnNull()
    {
        SourceHolder? source = null;

        var result = source.SetMetadata("key", "value");

        result.ShouldBeNull();
    }

    [Fact]
    public void SetMetadata_NullKey_ShouldReturnNull()
    {
        var source = new SourceHolder();

        var result = source.SetMetadata(null, "value");

        result.ShouldBeNull();
    }

    [Fact]
    public void SetMetadata_NullValue_ShouldReturnNull()
    {
        var source = new SourceHolder();

        var result = source.SetMetadata("key", null);

        result.ShouldBeNull();
    }

    [Fact]
    public void SetMetadata_NullMetadataStorage_ShouldReturnNull()
    {
        var source = new NullMetadataHolder();

        var result = source.SetMetadata("key", "value");

        result.ShouldBeNull();
    }

    #endregion

    #region MergeMetadata

    [Fact]
    public void MergeMetadata_ShouldCopyAllEntries()
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
    public void MergeMetadata_NullDest_ShouldReturnNull()
    {
        DestHolder? dest = null;
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["key"] = "value";

        var result = dest.MergeMetadata(source);

        result.ShouldBeNull();
    }

    [Fact]
    public void MergeMetadata_NullSource_ShouldReturnNull()
    {
        var dest = new DestHolder();
        SourceHolder? source = null;

        var result = dest.MergeMetadata(source);

        result.ShouldBeNull();
    }

    [Fact]
    public void MergeMetadata_ShouldNotOverwriteExistingKeys()
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
    public void MergeMetadata_NullDestinationStorage_ShouldReturnNull()
    {
        var dest = new NullMetadataHolder();
        var source = new SourceHolder();

        source.Metadata.ShouldNotBeNull();
        source.Metadata["key"] = "value";

        var result = dest.MergeMetadata(source);

        result.ShouldBeNull();
    }

    [Fact]
    public void MergeMetadata_NullSourceStorage_ShouldReturnNull()
    {
        var dest = new DestHolder();
        var source = new NullMetadataHolder();

        var result = dest.MergeMetadata(source);

        result.ShouldBeNull();
    }

    #endregion
}