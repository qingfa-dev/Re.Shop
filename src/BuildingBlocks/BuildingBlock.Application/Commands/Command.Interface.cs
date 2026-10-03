namespace BuildingBlock.Application.Commands;

public interface ICommand<TPayload> : Mediator.ICommand<Result<TPayload>>;
