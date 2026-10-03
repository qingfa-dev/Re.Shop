# Persistence and Messaging Conventions

These conventions define provider-neutral boundaries for the BuildingBlocks.
They add contracts only; applications must register and compose concrete
persistence and transport adapters explicitly.

## Aggregate writes and read specifications

Use `IAggregateRepository<TAggregate,TId>` only to load, add, and remove
aggregate roots. A loaded tracked aggregate is changed through its domain
behavior and persisted by committing the Unit of Work; the repository has no
generic `Update` method.

Keep query specifications and `IReadRepository<TEntity>` separate from
aggregate repositories. A specification can describe criteria, navigation
includes, ordered key selectors, and (for projected specifications) a selector.
The read repository returns materialized results and does not expose
`IQueryable` to Application code. Keep pagination in request/result contracts,
not in reusable specifications. Provider-only flags and raw queries belong in
adapters, not shared specifications.

## Transaction ownership and domain events

The application host owns transaction composition, cancellation, and
registration of repository, Unit of Work, outbox, and dispatcher
implementations. When a command can emit integration messages, follow this
order:

1. Capture pending domain-event snapshots from the changed aggregates.
2. Map domain events to zero or more integration events outside aggregate
   methods.
3. Persist business changes and write corresponding outbox records in the
   same transaction.
4. Commit through `IUnitOfWork`.
5. Clear aggregate pending events only after commit succeeds.
6. Dispatch the captured domain-event snapshot through the local
   post-commit dispatcher.

If commit fails, keep pending events and do not dispatch them. A failure in
local post-commit dispatch occurs after persistence has committed; it is not a
transaction rollback. The existing post-commit dispatcher is best-effort and
in-memory, not durable delivery.

## Outbox and inbox obligations

`IOutboxWriter.AddAsync` records an integration event in the caller's
transaction. The contract alone does not guarantee persistence or eventual
delivery. Future relay implementations should assume at-least-once delivery
and make consumers idempotent; do not claim exactly-once delivery.

`IInboxStore.TryRegisterAsync` uses a nonblank consumer name and the event's
`Guid` identity to deduplicate processing. The inbox registration and that
consumer's effects must commit or roll back in the same future transaction.
`true` means a new registration; `false` means the event was already
registered.

These interfaces do not provide a database schema, transaction manager,
background relay, broker integration, retries, or dead-letter processing.
The host selects and composes those implementations.

## Event sourcing is a separate future phase

Aggregate pending events are transient dispatch state, not an event store.
This phase does not introduce append/replay contracts or event-sourced
aggregates. A future optional event-sourcing design must independently address
expected stream revisions, replay, event versioning/upcasting,
projection/checkpoint ownership, and pending-event retention when append
fails.

No persistence or messaging provider is automatically registered or scanned
by these abstractions.
