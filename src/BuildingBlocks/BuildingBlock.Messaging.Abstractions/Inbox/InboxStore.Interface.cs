namespace BuildingBlock.Messaging.Abstractions.Inbox;

/// <summary>Atomically registers integration events for consumer deduplication.</summary>
/// <remarks>
/// The consumer name must be nonblank. Registration and the consumer's effects
/// must be committed or rolled back in the same transaction. A successful
/// registration does not by itself provide exactly-once processing.
/// </remarks>
public interface IInboxStore
{
    /// <returns>
    /// <see langword="true"/> when this event was newly registered;
    /// <see langword="false"/> when the event was already registered.
    /// </returns>
    ValueTask<bool> TryRegisterAsync(
        string consumerName,
        Guid eventId,
        CancellationToken cancellationToken = default);
}
