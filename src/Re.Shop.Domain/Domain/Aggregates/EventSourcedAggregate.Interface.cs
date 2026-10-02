using Domain.Events.Sourcing;

namespace Domain.Aggregates;

/// <summary>Exposes the event history and pending events of an event-sourced aggregate.</summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
/// <remarks>
/// The aggregate applies business events to derive state; persisted stream identity,
/// positions, and concurrency metadata belong to Infrastructure.
/// </remarks>
public interface IEventSourcedAggregate<TId> : IAggregateRoot<TId>
{
    /// <summary>Gets the count of successfully applied events in this aggregate instance.</summary>
    long Version { get; }

    /// <summary>Gets sourced events raised locally and not yet cleared.</summary>
    IReadOnlyCollection<IEventSourcedEvent> UncommittedEvents { get; }

    /// <summary>Applies ordered historical events to a fresh aggregate.</summary>
    /// <param name="history">The ordered event history to replay.</param>
    /// <remarks>Replayed events do not enter the uncommitted event collection.</remarks>
    void LoadFromHistory(IEnumerable<IEventSourcedEvent> history);

    /// <summary>Clears pending events without changing aggregate state or version.</summary>
    void ClearUncommittedEvents();
}
