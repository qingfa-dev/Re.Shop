using BuildingBlock.Domain.Events.Domain;

namespace BuildingBlock.Application.Events;

/// <summary>
/// Dispatches a captured ordered snapshot of domain events after the
/// persistence host has successfully committed and cleared its aggregates.
/// </summary>
/// <remarks>
/// Implementations reject a null list, treat an empty list as a no-op,
/// dispatch events sequentially in list order, pass cancellation through to
/// the domain dispatcher, and propagate the first failure. This contract
/// does not own persistence, clear aggregate events, retry failures, or
/// provide durable delivery.
/// </remarks>
public interface IPostCommitDomainEventDispatcher
{
    /// <summary>
    /// Dispatches the supplied events after a successful commit.
    /// </summary>
    /// <param name="domainEvents">
    /// The ordered event snapshot captured before the commit. Implementations
    /// must reject a null list.
    /// </param>
    /// <param name="cancellationToken">The token passed to each domain dispatch.</param>
    /// <returns>A task that completes when dispatch completes.</returns>
    Task DispatchAsync(
        IReadOnlyList<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}
