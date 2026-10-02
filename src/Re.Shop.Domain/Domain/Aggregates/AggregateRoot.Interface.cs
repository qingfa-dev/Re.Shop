using Domain.Events.Domain;
using Domain.Entities;

namespace Domain.Aggregates;

/// <summary>Marks an entity that owns a consistency boundary and its domain events.</summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
/// <remarks>
/// The aggregate root is the entry point for changes within its boundary, following
/// Eric Evans, <c>Domain-Driven Design</c> (2003).
/// </remarks>
public interface IAggregateRoot<TId> : IEntity<TId>
{
    /// <summary>Gets domain events raised but not yet cleared by the caller.</summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>Clears the pending domain events without dispatching them.</summary>
    void ClearDomainEvents();
}
