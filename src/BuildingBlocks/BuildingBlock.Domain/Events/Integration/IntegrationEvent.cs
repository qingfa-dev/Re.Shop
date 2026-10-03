using BuildingBlock.Domain.Events;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Domain.Events.Integration;

public abstract class IntegrationEvent(
    string eventType,
    int eventVersion,
    Guid? eventId = null,
    DateTimeOffset? occurredOnUtc = null,
    MetadataDictionary? metadata = null)
    : Event(eventType, eventVersion, eventId, occurredOnUtc, metadata), IIntegrationEvent;
