# Re.Shop.Domain

The business model for Re.Shop: validated values, identity, aggregate
consistency, and domain events. _layer · kernel_

## Summary

The Domain owns business facts and invariants, not application workflows or
storage. It depends on `Re.Shop.SharedKernel` and `Mediator.Abstractions` for
result/error and notification contracts. It does not reference Infrastructure,
Contracts, a database, an ORM, a serializer, an event store, or a broker SDK.

## Contains

| Path | Role | Notes |
|---|---|---|
| `Domain/ValueObjects/` | Immutable value types | Sixteen readonly record structs with `Create` factories returning `Result<T, Error>` |
| `Domain/Entities/` | Entity identity | `IEntity<TId>` and `Entity<TId>` |
| `Domain/Aggregates/` | Aggregate contracts and bases | State-based and event-sourced aggregates remain separate |
| `Events/` | Mediator-backed domain event contracts | Business events and optional metadata helpers; no transport events |
| `GlobalUsing.cs` | SharedKernel imports | Imports the shared `Error` and `Result` namespaces |
| `Re.Shop.Domain.csproj` | Project file | References SharedKernel and Mediator.Abstractions |

## Value objects

The existing record structs provide immutable structural equality; no common
`ValueObject` base is needed. Factories validate input and return shared
`Result<T, Error>` values while preserving the existing error codes and
normalization:

- Address: `Address`, `PostalCode`
- Common: `DateRange`, `Percentage`, `TimeRange`
- Identity: `EmailAddress`, `PhoneNumber`, `Url`
- Localization: `Language`, `Locale`
- Measurement: `Dimensions`, `Quantity`, `Unit`, `Weight`
- Money: `Currency`, `Money`

## Aggregate and event model

`IEntity<TId>` refines SharedKernel's `IIdentifiable<TId>`. `Entity<TId>`
rejects null/default IDs and defines equality by exact runtime type and ID.

`AggregateRoot<TId>` is the state-based option. Derived aggregates change
their state and then raise `IDomainEvent`s; the root retains them in order
until `ClearDomainEvents()` is called. It does not publish or persist them.

`EventSourcedAggregateRoot<TId>` is a separate option for models that rebuild
state from `IEventSourcedEvent`s. `LoadFromHistory()` can be called once on a
fresh aggregate; replay applies events in order, advances `Version` for each
successful application, and does not add replayed events to the pending list.
Locally raised events are applied, queued, then advance the version.
`DomainEvents` and `UncommittedEvents` expose the same pending event instances;
either clear method empties that list without rewinding state or version. If
event application throws, the exception propagates; partial state is not
rolled back, so discard that aggregate instance.

Domain events implement Mediator notification contracts but remain business
facts. Optional event ID, occurrence-time, correlation, tenant, and aggregate
metadata helpers reuse SharedKernel keys; they do not generate IDs/timestamps
or make metadata authoritative over typed event fields.

## Boundaries

In scope:
- Validated immutable values, entity identity, aggregate invariants, and domain events.
- Local collection and replay semantics for aggregate events.
- Shared result/error contracts and Mediator abstractions.

Out of scope:
- Commands, queries, handlers, dispatch timing, and orchestration → `../Re.Shop.Application/`.
- Stream IDs, event-store envelopes, serialization, snapshots, concurrency,
  persistence, and event publication → `../Re.Shop.Infrastructure/`.
- Cross-process integration events and transport DTOs → `../Re.Shop.Contracts/`.
- HTTP request types, `DbContext`, EF Core, and database drivers.

## References

- Eric Evans, *Domain-Driven Design: Tackling Complexity in the Heart of Software*, Addison-Wesley, 2003 — entities, value objects, and aggregates.
- Vaughn Vernon, *Implementing Domain-Driven Design*, Addison-Wesley, 2013 — aggregate boundaries and event-sourced models.
- Martin Fowler, [Domain Event](https://martinfowler.com/eaaDev/DomainEvent.html), [CQRS](https://martinfowler.com/bliki/CQRS.html), and [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html).
- [Approved design](../../docs/superpowers/specs/2026-10-02-reusable-domain-building-blocks-design.md)

## Invariants

- Project references remain inward; Domain has no project reference to Infrastructure.
- Expected business-rule failures use `Result<T, Error>`, not exceptions.
- Aggregates neither publish events nor claim persistence.
- Persistence-time stream position and concurrency are Infrastructure responsibilities.
- Public and protected APIs are documented in XML; documentation generation is enabled.
