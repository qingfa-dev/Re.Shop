using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Application.Queries;

public interface IQueryHandler<TQuery, TPayload>
    : Mediator.IQueryHandler<TQuery, Result<TPayload>>
    where TQuery : IQuery<TPayload>;
