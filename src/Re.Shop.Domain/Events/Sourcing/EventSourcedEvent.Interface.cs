using Domain.Events.Domain;

namespace Domain.Events.Sourcing;

/// <summary>Marks a domain event that an event-sourced aggregate can apply during replay.</summary>
/// <remarks>
/// The event contains immutable business facts by convention. Stream identity,
/// positions, and persistence timestamps belong to an Infrastructure-owned envelope.
/// </remarks>
public interface IEventSourcedEvent : IDomainEvent
{
}
