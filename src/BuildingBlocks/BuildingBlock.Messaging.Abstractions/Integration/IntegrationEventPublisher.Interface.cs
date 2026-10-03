using BuildingBlock.Domain.Events.Integration;

namespace BuildingBlock.Messaging.Abstractions.Integration;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default);
}
