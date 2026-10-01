# Re.Shop.Domain

Aggregates, entities and domain events — the model the shop is built on. _layer · kernel_

## Summary

This is where the business rules live: aggregates and their invariants, entity
lifecycles, and domain events raised when something meaningful happens. It may
reach inward to `SharedKernel` and nowhere else, so nothing in this folder can
know that a database, an HTTP request, or an ORM exists. That isolation is the
point of the layer, and it is enforced by a compile-time reference guard as well
as an architecture rule.

## Contains

| Path | Role | Notes |
|---|---|---|
| `DomainMarker.cs` | Assembly anchor | Located by `Re.Shop.ArchitectureTests`; no behaviour |
| `Events/` | Domain events | _(empty — no README)_ |
| `Re.Shop.Domain.csproj` | Project file | References `Re.Shop.SharedKernel` only |

## Boundaries

In scope:
- Aggregates, entities, value objects, and the invariants they enforce.
- Domain events raised by the model.
- Business rules that would remain true with a different persistence engine.

Out of scope:
- Orchestration and use-case flow → `../Re.Shop.Application/`.
- Persistence and ORM types → `../Re.Shop.Infrastructure/`.
- Transport DTOs → `../Re.Shop.Contracts/`.
- Anything referencing `Microsoft.EntityFrameworkCore` or `Npgsql` — asserted by
  `Only_Infrastructure_and_Api_may_reference_entity_framework`.

## Assumptions

- `Re.Shop.SharedKernel` is referenced and available; this is the only
  permitted dependency (spec §5.3 rule 1).
- Persistence is supplied from outside; the domain never opens a connection.
- `GenerateDocumentationFile` is `true`, so every public and protected member
  needs an XML summary.

## Invariants

- Depends only on `Re.Shop.SharedKernel` — spec §5.3 rule 1.
- No type here may mention `DbContext`, `HttpRequest`, or a repository.
- Invariants are enforced by the aggregate, never by a caller or a validator.

## Contracts

- `DomainMarker` — the anchor the architecture suite resolves.
- Domain events raised by aggregates — no event type exists yet.

## Conventions

- One aggregate per file; do not merge an aggregate with its child entities.

## Related

- Parent: [`../README.md`](../README.md)
- Rules: [`tests/Re.Shop.ArchitectureTests/`](../../tests/Re.Shop.ArchitectureTests/README.md)
- Naming: [`../../guide/dotted-file-naming-guide.md`](../../guide/dotted-file-naming-guide.md)

## Notes

- Known gap: `Events/` is an empty placeholder; only the marker exists.
