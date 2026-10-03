namespace BuildingBlocks.Domain.Concerns.Organization.Collectionable;

public interface ICollectionable<TCollection>
{
    ICollection<TCollection> Collections { get; }
}
