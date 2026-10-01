# Re.Shop.Application

Use-case orchestration: handlers, services and validators. _layer · orchestrator_

## Summary

This layer coordinates the domain to fulfil a use case — it owns the application
services and handlers that the API exposes, plus the validators that guard their
inputs. It may reach inward to `Domain` and `SharedKernel`, and must never reach
`Infrastructure`, `Api`, or `Contracts`. That keeps use cases testable against
the domain alone, with persistence and transport supplied by the composition
root.

## Contains

| Path | Role | Notes |
|---|---|---|
| `ApplicationMarker.cs` | Assembly anchor | Located by `Re.Shop.ArchitectureTests`; no behaviour |
| `Re.Shop.Application.csproj` | Project file | References `Re.Shop.SharedKernel` only in this folder |

## Boundaries

In scope:
- Application services, command/query handlers, and use-case orchestration.
- Input validators for those use cases.
- Transactions and unit-of-work coordination.

Out of scope:
- Business invariants that would hold without the application → `../Re.Shop.Domain/`.
- EF Core, `DbContext`, repositories, and any `Microsoft.EntityFrameworkCore*`
  reference → `../Re.Shop.Infrastructure/`.
- HTTP endpoints and DTO mapping → `../../app/api/`.
- Public DTOs → `../Re.Shop.Contracts/` — rule 2 forbids this layer from
  depending on them.

## Assumptions

- Persistence is reached through abstractions declared inward of this layer;
  implementations are wired by the composition root.
- Domain events raised here are collected, not dispatched, by this layer.
- `GenerateDocumentationFile` is `true`, so every public and protected member
  needs an XML summary.

## Contracts

- Application services and command/query handlers consumed by
  [`../../app/api/`](../../app/api/README.md) — none exist yet.
- Input validators, one per command or query, living beside the command they guard.
- `ApplicationMarker` — the anchor the architecture suite resolves.

## Conventions

- A handler does one thing; orchestration belongs here, business rules do not.
- Validators live with the command they guard, not with the endpoint.

## Flows

### Use case

1. `app/api` maps an incoming request onto a command or query.
2. A validator guards the input before any orchestration runs.
3. A handler coordinates the domain and records the events it raises.
4. The composition root supplies persistence — this layer never resolves it.

## Related

- Parent: [`../README.md`](../README.md)
- Test: [`../../tests/Re.Shop.Application.UnitTests/README.md`](../../tests/Re.Shop.Application.UnitTests/README.md)
- Rules: [`tests/Re.Shop.ArchitectureTests/`](../../tests/Re.Shop.ArchitectureTests/README.md)

## Notes

- Known gap: the project currently contains only the assembly marker; no
  handlers or validators exist yet.
- `Application_depends_inward_only` is asserted in
  [`tests/Re.Shop.ArchitectureTests/`](../../tests/Re.Shop.ArchitectureTests/README.md).
