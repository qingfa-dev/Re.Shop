using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Application.Behaviors;

public interface IApplicationPipelineBehavior<TMessage, TResponse>
    : Mediator.IPipelineBehavior<TMessage, TResponse>
    where TMessage : Mediator.IMessage
    where TResponse : IResult<Error>, IResultFailure<TResponse, Error>;
