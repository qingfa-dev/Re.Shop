namespace Domain.Events.Domain;

/// <summary>Handles an in-process domain event.</summary>
/// <typeparam name="TEvent">The domain-event type handled by this handler.</typeparam>
public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>
     where TEvent : IDomainEvent
{
}
