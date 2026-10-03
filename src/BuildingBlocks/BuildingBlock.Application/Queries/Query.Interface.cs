using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Application.Queries;

public interface IQuery<TPayload> : Mediator.IQuery<Result<TPayload>>;
