using System.Globalization;

using SharedKernel.Structures.Meta;

namespace Domain.Events.Domain;

/// <summary>Reads and writes optional aggregate context in domain-event metadata.</summary>
/// <remarks>Aggregate metadata is supplementary; event payload remains the source of business facts.</remarks>
public static class DomainEventExtensions
{
    // ── SET ────────────────────────────────────────────────
    /// <summary>Stores the aggregate identifier using invariant GUID formatting.</summary>
    /// <typeparam name="TEvent">The concrete domain-event type.</typeparam>
    /// <param name="event">The event to update.</param>
    /// <param name="aggregateId">The identifier to store.</param>
    /// <returns>The same event instance.</returns>
    public static TEvent WithAggregateId<TEvent>(this TEvent @event, Guid aggregateId)
        where TEvent : IDomainEvent
        => @event.WithMetadata(
            MetadataConstant.MetadataKey.AggregateId,
            aggregateId.ToString("D", CultureInfo.InvariantCulture));

    /// <summary>Stores the supplied aggregate type name.</summary>
    /// <typeparam name="TEvent">The concrete domain-event type.</typeparam>
    /// <param name="event">The event to update.</param>
    /// <param name="aggregateType">The aggregate type name.</param>
    /// <returns>The same event instance.</returns>
    public static TEvent WithAggregateType<TEvent>(this TEvent @event, string aggregateType)
        where TEvent : IDomainEvent
        => @event.WithMetadata(MetadataConstant.MetadataKey.AggregateType, aggregateType);

    /// <summary>Stores the name of the specified aggregate type.</summary>
    /// <typeparam name="TEvent">The concrete domain-event type.</typeparam>
    /// <typeparam name="TAggregate">The aggregate type whose name is stored.</typeparam>
    /// <param name="event">The event to update.</param>
    /// <returns>The same event instance.</returns>
    public static TEvent WithAggregateType<TEvent, TAggregate>(this TEvent @event)
        where TEvent : IDomainEvent
        => @event.WithMetadata(MetadataConstant.MetadataKey.AggregateType, typeof(TAggregate).Name);

    // ── GET ────────────────────────────────────────────────
    /// <summary>Gets the aggregate identifier, or <see cref="Guid.Empty"/> when unavailable or invalid.</summary>
    /// <param name="event">The domain event whose metadata is read.</param>
    /// <returns>The parsed identifier or <see cref="Guid.Empty"/>.</returns>
    public static Guid GetAggregateId(this IDomainEvent @event)
        => @event.TryGetAggregateId(out var id) ? id : Guid.Empty;

    /// <summary>Gets the aggregate type name, or an empty string when absent.</summary>
    /// <param name="event">The domain event whose metadata is read.</param>
    /// <returns>The stored aggregate type name or an empty string.</returns>
    public static string GetAggregateType(this IDomainEvent @event)
        => @event.GetMetadataValue(MetadataConstant.MetadataKey.AggregateType) ?? string.Empty;

    /// <summary>Attempts to parse the aggregate identifier from metadata.</summary>
    /// <param name="event">The domain event whose metadata is read.</param>
    /// <param name="aggregateId">Receives the parsed identifier when successful.</param>
    /// <returns><see langword="true"/> when metadata contains a valid identifier.</returns>
    public static bool TryGetAggregateId(this IDomainEvent @event, out Guid aggregateId)
        => Guid.TryParse(@event.GetMetadataValue(MetadataConstant.MetadataKey.AggregateId), out aggregateId);
}
