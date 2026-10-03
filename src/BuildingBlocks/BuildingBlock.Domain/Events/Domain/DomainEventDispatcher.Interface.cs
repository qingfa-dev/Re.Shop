using Mediator;
namespace BuildingBlock.Domain.Events.Domain;

public interface IDomainEventDispatcher: INotificationPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
