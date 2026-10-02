namespace SharedKernel.Concerns.Organization.Collectionable;

public interface ICollectionable<TCollection>
{
    ICollection<TCollection> Collections { get; }
}
