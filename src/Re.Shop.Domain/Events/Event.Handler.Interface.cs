using Mediator;

namespace Domain.Events;

/// <summary>Handles an event routed through Mediator.</summary>
/// <typeparam name="TEvent">The event type handled by this handler.</typeparam>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
     where TEvent : IEvent
{
}
