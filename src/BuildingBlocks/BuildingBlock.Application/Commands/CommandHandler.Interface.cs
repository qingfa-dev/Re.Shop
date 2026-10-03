using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Application.Commands;

public interface ICommandHandler<TCommand, TPayload>
    : Mediator.ICommandHandler<TCommand, Result<TPayload>>
    where TCommand : ICommand<TPayload>;
