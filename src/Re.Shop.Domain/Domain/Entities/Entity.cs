namespace Domain.Entities;

/// <summary>Base class for entities whose equality is defined by identity.</summary>
/// <typeparam name="TId">The type of the entity's stable identifier.</typeparam>
/// <remarks>
/// Equality requires the same runtime type and identifier. This follows the
/// entity pattern described in Eric Evans, <c>Domain-Driven Design</c> (2003).
/// </remarks>
public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
{
    /// <summary>Initializes an entity with a valid, non-default identifier.</summary>
    /// <param name="id">The entity's stable identifier.</param>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="id"/> is its type's default value.</exception>
    protected Entity(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (EqualityComparer<TId>.Default.Equals(id, default!))
        {
            throw new ArgumentException("Entity identifier cannot be the default value.", nameof(id));
        }

        Id = id;
    }

    /// <inheritdoc />
    public TId Id { get; }

    /// <inheritdoc />
    public bool Equals(Entity<TId>? other)
    {
        return other is not null &&
            GetType() == other.GetType() &&
            EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}
