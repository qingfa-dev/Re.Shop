using BuildingBlock.Domain.Events.Domain;

namespace BuildingBlock.Domain.Aggregates;

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
