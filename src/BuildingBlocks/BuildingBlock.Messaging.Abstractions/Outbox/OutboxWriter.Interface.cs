using BuildingBlock.Domain.Events.Integration;

namespace BuildingBlock.Messaging.Abstractions.Outbox;

/// <summary>Records an integration event for later transport delivery.</summary>
/// <remarks>
/// Implementations must enlist the outbox record in the caller's business
/// transaction. This contract alone does not provide durable delivery.
/// </remarks>
public interface IOutboxWriter
{
    Task AddAsync(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default);
}
