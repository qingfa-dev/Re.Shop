using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Domain.Events;

public abstract class Event : IEvent
{
    protected Event(
        string eventType,
        int eventVersion,
        Guid? eventId = null,
        DateTimeOffset? occurredOnUtc = null,
        MetadataDictionary? metadata = null)
    {
        EventId = eventId ?? Guid.NewGuid();
        OccurredOnUtc = (occurredOnUtc ?? DateTimeOffset.UtcNow).ToUniversalTime();
        EventType = eventType;
        EventVersion = eventVersion;
        Metadata = new MetadataDictionary(
            metadata ?? MetadataDictionary.Empty);

        EventValidator.EnsureValid(this);
    }

    public Guid EventId { get; }
    public DateTimeOffset OccurredOnUtc { get; }
    public string EventType { get; }
    public int EventVersion { get; }
    public MetadataDictionary Metadata { get; }
}