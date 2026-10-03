using BuildingBlock.Persistence.Abstractions.Specifications;

namespace BuildingBlock.Persistence.Abstractions.Repositories;

public interface IReadRepository<TEntity>
{
    ValueTask<IReadOnlyList<TEntity>> ListAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<TProjection>> ListAsync<TProjection>(
        ISpecification<TEntity, TProjection> specification,
        CancellationToken cancellationToken = default);

    ValueTask<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    ValueTask<long> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);
}
