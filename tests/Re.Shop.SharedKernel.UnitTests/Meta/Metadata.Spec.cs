using SharedKernel.Meta;
using Shouldly;

namespace SharedKernel.UnitTests.Meta;

public class MetadataInterfaceTest
{
    private sealed class MetadataHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata { get; } = [];
    }

    private sealed class NullMetadataHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata => null;
    }

    [Fact]
    public void Metadata_ShouldExposeDictionary()
    {
        var holder = new MetadataHolder();

        holder.Metadata.ShouldNotBeNull();
        holder.Metadata.ShouldBeAssignableTo<IDictionary<string, object>>();
    }

    [Fact]
    public void Metadata_ShouldSupportReadAndWrite()
    {
        var holder = new MetadataHolder();

        holder.Metadata.ShouldNotBeNull();
        holder.Metadata["key"] = "value";

        holder.Metadata.ShouldContainKey("key");
        holder.Metadata["key"].ShouldBe("value");
    }

    [Fact]
    public void MetadataHolder_ShouldImplementIMetadata()
    {
        var holder = new MetadataHolder();

        holder.ShouldBeAssignableTo<IMetadata>();
    }

    [Fact]
    public void Metadata_ShouldAllowNull()
    {
        IMetadata holder = new NullMetadataHolder();

        holder.Metadata.ShouldBeNull();
    }
}