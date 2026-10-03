using BuildingBlock.Domain.Events;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Domain.Events.Domain;

public abstract class DomainEvent(
    string eventType,
    int eventVersion,
    Guid? eventId = null,
    DateTimeOffset? occurredOnUtc = null,
    MetadataDictionary? metadata = null)
    : Event(eventType, eventVersion, eventId, occurredOnUtc, metadata), IDomainEvent;