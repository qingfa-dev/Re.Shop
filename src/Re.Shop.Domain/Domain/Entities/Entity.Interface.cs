using SharedKernel.Concerns.Foundation.Identifiable;

namespace Domain.Entities;

/// <summary>Refines shared identity for entities in the Domain model.</summary>
/// <typeparam name="TId">The type of the entity's stable identifier.</typeparam>
/// <remarks>
/// Entity identity is distinguished from value equality by its stable identifier.
/// This follows the entity pattern described in Eric Evans, <c>Domain-Driven Design</c> (2003).
/// </remarks>
public interface IEntity<TId> : IIdentifiable<TId>
{
}
