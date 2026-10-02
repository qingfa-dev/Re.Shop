namespace BuildingBlock.Kernel.Metadata;

public interface IMetadata<TDictionary> where TDictionary : IMetadataDictionary
{
    TDictionary Metadata { get; }
}

public interface IMetadata : IMetadata<MetadataDictionary>
{
}