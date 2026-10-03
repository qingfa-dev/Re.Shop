namespace BuildingBlocks.Domain.Concerns.Content.Metafieldable;

public interface IMetafieldable
{
    ICollection<Metafield> Metafields { get; }
}
