using System.Collections.ObjectModel;
using Domain.Entities;
using Domain.Events.Domain;
using Domain.Events.Sourcing;

namespace Domain.Aggregates;

/// <summary>Base class for aggregates whose state is rebuilt from sourced events.</summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
/// <remarks>
/// This is separate from the state-based <see cref="AggregateRoot{TId}"/> so
/// only <see cref="IEventSourcedEvent"/> instances can be raised. The approach
/// follows event-sourced aggregate patterns described in Vaughn Vernon,
/// <c>Implementing Domain-Driven Design</c> (2013), and Martin Fowler,
/// <c>Event Sourcing</c>.
/// </remarks>
public abstract class EventSourcedAggregateRoot<TId> :
    Entity<TId>,
    IEventSourcedAggregate<TId>
{
    private readonly List<IEventSourcedEvent> _uncommittedEvents = [];
    private readonly ReadOnlyCollection<IEventSourcedEvent> _uncommittedEventsView;
    private bool _historyLoadAttempted;

    /// <summary>Initializes the aggregate root with its stable identifier.</summary>
    /// <param name="id">The aggregate root's identifier.</param>
    protected EventSourcedAggregateRoot(TId id)
        : base(id)
    {
        _uncommittedEventsView = _uncommittedEvents.AsReadOnly();
    }

    /// <inheritdoc />
    public long Version { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<IEventSourcedEvent> UncommittedEvents =>
        _uncommittedEventsView;

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _uncommittedEventsView;

    /// <summary>Clears locally raised events without changing state or version.</summary>
    public void ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }

    /// <inheritdoc />
    public void ClearDomainEvents()
    {
        _uncommittedEvents.Clear();
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="history"/> or an event is null.</exception>
    /// <exception cref="InvalidOperationException">History has already been loaded or the aggregate is not fresh.</exception>
    /// <remarks>
    /// History is applied in order. A failure is propagated; if application has
    /// partially changed state, discard this aggregate instance rather than retry.
    /// </remarks>
    public void LoadFromHistory(IEnumerable<IEventSourcedEvent> history)
    {
        ArgumentNullException.ThrowIfNull(history);

        if (_historyLoadAttempted || Version != 0 || _uncommittedEvents.Count != 0)
        {
            throw new InvalidOperationException(
                "History can only be loaded once on a fresh aggregate.");
        }

        _historyLoadAttempted = true;

        foreach (var domainEvent in history)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);
            Apply(domainEvent);
            Version++;
        }
    }

    /// <summary>Applies a locally raised event, then records it and advances the version.</summary>
    /// <param name="domainEvent">The sourced event to apply.</param>
    /// <exception cref="ArgumentNullException"><paramref name="domainEvent"/> is null.</exception>
    /// <remarks>
    /// Application exceptions propagate. The event is not recorded and the
    /// version is not advanced if application fails, but partial state changes
    /// are not rolled back; discard the aggregate instance.
    /// </remarks>
    protected void RaiseEvent(IEventSourcedEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        Apply(domainEvent);
        _uncommittedEvents.Add(domainEvent);
        Version++;
    }

    /// <summary>Applies one sourced event to this aggregate's state.</summary>
    /// <param name="domainEvent">The event being replayed or raised.</param>
    protected abstract void Apply(IEventSourcedEvent domainEvent);
}
