namespace BuildingBlocks.Domain.Concerns.Foundation.Identifiable;

public interface IIdentifiable<TKey>
{
    TKey Id { get; }
}
