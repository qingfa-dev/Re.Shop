using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Persistence.Abstractions.UnitOfWork;

public interface IUnitOfWork
{
    ValueTask<Result<Unit>> CommitAsync(
        CancellationToken cancellationToken = default);
}
