# Application CQRS Conventions

This guide applies to new Application-layer work. Keep Domain invariants in
`BuildingBlock.Domain`; keep persistence and host lifecycle decisions outside
the reusable Application contracts.

## Commands and queries

Use `BuildingBlock.Application.Commands.ICommand<TPayload>` for state-changing
requests and `ICommandHandler<TCommand,TPayload>` for their handlers. Use
`BuildingBlock.Application.Queries.IQuery<TPayload>` and
`IQueryHandler<TQuery,TPayload>` for read-only requests. These interfaces wrap
Mediator 3.0.2's command/query contracts; they do not create a separate bus.

The generic parameter names the successful payload. Every Application command
and query is sent as `Result<TPayload>` and every handler returns
`ValueTask<Result<TPayload>>`; there is no raw-response variant. Use Kernel
`Unit` for a command with no meaningful payload and return
`Result<Unit>`.

## Mediator host registration

Install `Mediator.SourceGenerator` in the outermost executable application or
worker project, not in the reusable `BuildingBlock.Application` library.
Install `Mediator.Abstractions` in libraries that declare messages or
handlers. In the host, include the assembly containing concrete handlers and
register pipeline behavior types explicitly:

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IValidator<CreateOrderCommand>, CreateOrderValidator>();

builder.Services.AddMediator(options =>
{
    options.Assemblies = [typeof(CreateOrderHandler)];
    options.PipelineBehaviors =
    [
        typeof(ValidationPipelineBehavior<,>),
        typeof(CreateOrderPipelineBehavior)
    ];
});
```

Implement concrete behaviors with
`IApplicationPipelineBehavior<TMessage,TResponse>` where `TResponse` is a
Kernel Result. For example, a behavior for `CreateOrderCommand` returning
`Result<OrderId>` implements
`IApplicationPipelineBehavior<CreateOrderCommand,Result<OrderId>>`. The
response generic follows Mediator's open-generic registration convention;
`Result<T>` implements the Kernel typed failure-factory contract so behavior
implementations retain the correct Result type.

The opt-in `ValidationPipelineBehavior<,>` runs all registered FluentValidation
validators sequentially and aggregates failures. Each failure is returned as a
Kernel `Error.UnprocessableEntity` (HTTP 422) with code
`Application.Validation.Failed`; messages include the property path when
available, redact rendered attempted values as `[redacted]`, and are capped
by Kernel's message-length limit. Any validation failure short-circuits before handler execution.
Cancellation and unexpected validator exceptions propagate. Register validators
and the behavior explicitly in the host; the reusable Application library
does not scan assemblies or add behaviors automatically.

Mediator invokes each behavior with the message, its
`MessageHandlerDelegate<TMessage,TResponse>`, and the cancellation token.
Behaviors may call the next delegate, short-circuit with a typed failed Result,
or propagate an error according to their documented responsibility. Pipeline
behavior order and registration belong to the application host; the building
block adds no automatic logging, authorization, or transaction behavior.

## Post-commit domain events

Use `IPostCommitDomainEventDispatcher` only with events captured from
aggregates before persistence. The host owns the transaction and follows this
order:

1. Snapshot pending domain events in aggregate recording order.
2. Commit business state.
3. If the commit succeeds, clear pending events from the aggregates.
4. Dispatch the captured snapshot sequentially through the post-commit seam.

If persistence fails, keep pending events and do not dispatch them. A dispatch
failure occurs after commit: surface it to the caller, do not report the
transaction as rolled back, and do not assume the in-memory snapshot will be
retried. The Phase 2 seam has no durable retry or delivery guarantee. Add an
outbox/inbox only in the later durable messaging increment.

Integration-event publication is not part of this local domain-event path.
Do not publish integration events directly from aggregate mutations.
