using System.Collections.ObjectModel;
using Domain.Entities;
using Domain.Events.Domain;

namespace Domain.Aggregates;

/// <summary>Base class for state-based aggregates that collect domain events.</summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
/// <remarks>
/// Events are kept locally in raise order. Dispatch and persistence are outside
/// the Domain boundary, consistent with the aggregate pattern in Eric Evans,
/// <c>Domain-Driven Design</c> (2003).
/// </remarks>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly ReadOnlyCollection<IDomainEvent> _domainEventsView;

    /// <summary>Initializes the aggregate root with its stable identifier.</summary>
    /// <param name="id">The aggregate root's identifier.</param>
    protected AggregateRoot(TId id)
        : base(id)
    {
        _domainEventsView = _domainEvents.AsReadOnly();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEventsView;

    /// <summary>Adds a domain event raised by a successful aggregate operation.</summary>
    /// <param name="domainEvent">The event to append to the pending collection.</param>
    /// <exception cref="ArgumentNullException"><paramref name="domainEvent"/> is null.</exception>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <inheritdoc />
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
