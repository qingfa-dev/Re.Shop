namespace BuildingBlock.Domain.Aggregates.Repositories;

public interface IAggregateRepository<TAggregate, in TId>
    where TAggregate : class, IAggregateRoot
{
    ValueTask<TAggregate?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default);

    ValueTask AddAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    ValueTask RemoveAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);
}
