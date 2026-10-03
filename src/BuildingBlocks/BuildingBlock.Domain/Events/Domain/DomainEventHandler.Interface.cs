using Mediator;

namespace BuildingBlock.Domain.Events.Domain;

// Handler:
public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
 where TEvent : IDomainEvent;