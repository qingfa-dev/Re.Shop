using SharedKernel.Meta;
using Shouldly;

namespace SharedKernel.UnitTests.Meta;

public class MetadataInterfaceTest
{
    private sealed class MetadataHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata { get; set; } = [];
    }

    private sealed class NullMetadataHolder : IMetadata
    {
        public Dictionary<string, object?>? Metadata { get; set; }
    }

    [Fact]
    public void Metadata_Initialized_ExposesDictionary()
    {
        var holder = new MetadataHolder();

        holder.Metadata.ShouldNotBeNull();
        holder.Metadata.ShouldBeAssignableTo<IDictionary<string, object>>();
    }

    [Fact]
    public void Metadata_KeyValueSet_SupportsReadAndWrite()
    {
        var holder = new MetadataHolder();

        holder.Metadata.ShouldNotBeNull();
        holder.Metadata["key"] = "value";

        holder.Metadata.ShouldContainKey("key");
        holder.Metadata["key"].ShouldBe("value");
    }

    [Fact]
    public void MetadataHolder_Created_ImplementsIMetadata()
    {
        var holder = new MetadataHolder();

        holder.ShouldBeAssignableTo<IMetadata>();
    }

    [Fact]
    public void Metadata_NullStorage_ReturnsNull()
    {
        IMetadata holder = new NullMetadataHolder();

        holder.Metadata.ShouldBeNull();
    }
}