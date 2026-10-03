using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Domain.Events;

public interface IEvent : IMetadata
{
    Guid EventId { get; }
    DateTimeOffset OccurredOnUtc { get; }
    string EventType { get; }
    int EventVersion { get; }
}