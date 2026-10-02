using System.Globalization;
using Mediator;
using SharedKernel.Structures.Meta;

namespace Domain.Events;

/// <summary>Reads and writes optional event context using SharedKernel metadata keys.</summary>
/// <remarks>
/// These helpers do not generate identifiers or timestamps. Metadata is supplementary
/// context and does not replace business fields in the event.
/// </remarks>
public static class EventExtensions
{
    // ───────────────────────────── SET ─────────────────────────────

    /// <summary>Stores a non-empty event identifier in metadata.</summary>
    /// <typeparam name="TEvent">The concrete event type.</typeparam>
    /// <param name="event">The event to update.</param>
    /// <param name="eventId">The identifier to store; <see cref="Guid.Empty"/> leaves metadata unchanged.</param>
    /// <returns>The same event instance.</returns>
    public static TEvent WithEventId<TEvent>(this TEvent @event, Guid eventId)
        where TEvent : IEvent
    {
        if (eventId == Guid.Empty)
        {
            return @event;
        }

        return @event.WithMetadata(
            MetadataConstant.MetadataKey.EventId,
            eventId.ToString("D", CultureInfo.InvariantCulture));
    }

    /// <summary>Stores a non-default occurrence timestamp using round-trip format.</summary>
    /// <typeparam name="TEvent">The concrete event type.</typeparam>
    /// <param name="event">The event to update.</param>
    /// <param name="occurredOnUtc">The caller-supplied occurrence time.</param>
    /// <returns>The same event instance.</returns>
    public static TEvent WithOccurredOnUtc<TEvent>(this TEvent @event, DateTimeOffset occurredOnUtc)
        where TEvent : IEvent
    {
        if (occurredOnUtc == default)
        {
            return @event;
        }

        return @event.WithMetadata(
            MetadataConstant.MetadataKey.Timestamp,
            occurredOnUtc.ToString("o", CultureInfo.InvariantCulture));
    }

    /// <summary>Stores a non-blank correlation identifier.</summary>
    /// <typeparam name="TEvent">The concrete event type.</typeparam>
    /// <param name="event">The event to update.</param>
    /// <param name="correlationId">The correlation identifier; blank values leave metadata unchanged.</param>
    /// <returns>The same event instance.</returns>
    public static TEvent WithCorrelationId<TEvent>(this TEvent @event, string? correlationId)
        where TEvent : IEvent
    {
        return string.IsNullOrWhiteSpace(correlationId)
            ? @event
            : @event.WithMetadata(
                MetadataConstant.MetadataKey.CorrelationId,
                correlationId);
    }

    /// <summary>Stores a non-blank tenant identifier.</summary>
    /// <typeparam name="TEvent">The concrete event type.</typeparam>
    /// <param name="event">The event to update.</param>
    /// <param name="tenantId">The tenant identifier; blank values leave metadata unchanged.</param>
    /// <returns>The same event instance.</returns>
    public static TEvent WithTenantId<TEvent>(this TEvent @event, string? tenantId)
        where TEvent : IEvent
    {
        return string.IsNullOrWhiteSpace(tenantId)
            ? @event
            : @event.WithMetadata(
                MetadataConstant.MetadataKey.TenantId,
                tenantId);
    }

    // ───────────────────────────── GET ─────────────────────────────

    /// <summary>Gets the event identifier, or <see cref="Guid.Empty"/> when unavailable or invalid.</summary>
    /// <param name="event">The event whose metadata is read.</param>
    /// <returns>The parsed identifier or <see cref="Guid.Empty"/>.</returns>
    public static Guid GetEventId(this IEvent @event)
        => @event.TryGetEventId(out var id) ? id : Guid.Empty;

    /// <summary>Gets the occurrence timestamp, or <see cref="DateTimeOffset.MinValue"/> when unavailable or invalid.</summary>
    /// <param name="event">The event whose metadata is read.</param>
    /// <returns>The parsed timestamp or <see cref="DateTimeOffset.MinValue"/>.</returns>
    public static DateTimeOffset GetOccurredOnUtc(this IEvent @event)
        => @event.TryGetOccurredOnUtc(out var ts) ? ts : DateTimeOffset.MinValue;

    /// <summary>Gets the correlation identifier, or an empty string when absent.</summary>
    /// <param name="event">The event whose metadata is read.</param>
    /// <returns>The correlation identifier or an empty string.</returns>
    public static string GetCorrelationId(this IEvent @event)
        => @event.GetMetadataValue(MetadataConstant.MetadataKey.CorrelationId) ?? string.Empty;

    /// <summary>Gets the tenant identifier, or an empty string when absent.</summary>
    /// <param name="event">The event whose metadata is read.</param>
    /// <returns>The tenant identifier or an empty string.</returns>
    public static string GetTenantId(this IEvent @event)
        => @event.GetMetadataValue(MetadataConstant.MetadataKey.TenantId) ?? string.Empty;

    // ─────────────────────────── TRY-GET ───────────────────────────

    /// <summary>Attempts to parse the event identifier from metadata.</summary>
    /// <param name="event">The event whose metadata is read.</param>
    /// <param name="eventId">Receives the parsed identifier when successful.</param>
    /// <returns><see langword="true"/> when metadata contains a valid identifier.</returns>
    public static bool TryGetEventId(this IEvent @event, out Guid eventId)
        => Guid.TryParse(
            @event.GetMetadataValue(MetadataConstant.MetadataKey.EventId),
            out eventId);

    /// <summary>Attempts to parse the occurrence timestamp from metadata.</summary>
    /// <param name="event">The event whose metadata is read.</param>
    /// <param name="occurredOnUtc">Receives the parsed timestamp when successful.</param>
    /// <returns><see langword="true"/> when metadata contains a valid timestamp.</returns>
    public static bool TryGetOccurredOnUtc(this IEvent @event, out DateTimeOffset occurredOnUtc)
        => DateTimeOffset.TryParse(
            @event.GetMetadataValue(MetadataConstant.MetadataKey.Timestamp),
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out occurredOnUtc);

    /// <summary>Attempts to read a non-empty correlation identifier from metadata.</summary>
    /// <param name="event">The event whose metadata is read.</param>
    /// <param name="correlationId">Receives the identifier or an empty string.</param>
    /// <returns><see langword="true"/> when a non-empty identifier is present.</returns>
    public static bool TryGetCorrelationId(this IEvent @event, out string correlationId)
    {
        correlationId = @event.GetCorrelationId();
        return correlationId.Length > 0;
    }

    /// <summary>Attempts to read a non-empty tenant identifier from metadata.</summary>
    /// <param name="event">The event whose metadata is read.</param>
    /// <param name="tenantId">Receives the identifier or an empty string.</param>
    /// <returns><see langword="true"/> when a non-empty identifier is present.</returns>
    public static bool TryGetTenantId(this IEvent @event, out string tenantId)
    {
        tenantId = @event.GetTenantId();
        return tenantId.Length > 0;
    }
}
