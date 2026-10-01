# Re.Shop.Infrastructure

Persistence adapters: EF Core, Npgsql and repository implementations. _layer · adapter_

## Summary

This layer implements the persistence concerns the application declares — the
`DbContext`, entity configurations, and the repository adapters that satisfy
inward-facing interfaces. It sits outermost among the `src/` libraries, which is
why it may depend on `Domain` but never on `Api` or `Admin`. It is also one of
only two projects permitted to reference `Microsoft.EntityFrameworkCore*` or
`Npgsql*`.

## Contains

| Path | Role | Notes |
|---|---|---|
| `InfrastructureMarker.cs` | Assembly anchor | Located by `Re.Shop.ArchitectureTests`; no behaviour |
| `Re.Shop.Infrastructure.csproj` | Project file | References `Re.Shop.SharedKernel` in this folder |

## Boundaries

In scope:
- `DbContext`, entity configurations, migrations.
- Repository and gateway implementations of application-declared interfaces.
- Npgsql and any other persistence driver.

Out of scope:
- Use-case orchestration → `../Re.Shop.Application/`.
- Domain invariants → `../Re.Shop.Domain/`.
- Endpoint mapping → `../../app/api/`.
- Health checks, telemetry, resilience → `../../aspire/Re.ServiceDefaults/`.

## Assumptions

- Entity Framework and Npgsql are versioned centrally in
  `Directory.Packages.props`; this project declares no versions of its own.
- Connection strings are supplied by the composition root — see
  [`../../app/api/README.md`](../../app/api/README.md).
- Of the five `src/` layers, this one and `Re.Shop.Api` may reference EF Core;
  the other three may not, and that is asserted by
  `Only_Infrastructure_and_Api_may_reference_entity_framework`.

## Contracts

- Repository and gateway implementations of interfaces declared inward of this
  layer — none exist yet.
- The `DbContext` that [`../../app/api/`](../../app/api/README.md) will register
  — not created yet.
- `InfrastructureMarker` — the anchor the architecture suite resolves.

## Conventions

- Adapters implement interfaces declared inward of this layer; the dependency
  points from here toward the abstraction, never the reverse.

## Related

- Parent: [`../README.md`](../README.md)
- Test: [`../../tests/Re.Shop.IntegrationTests/README.md`](../../tests/Re.Shop.IntegrationTests/README.md)
- Rules: [`tests/Re.Shop.ArchitectureTests/`](../../tests/Re.Shop.ArchitectureTests/README.md)

## Notes

- Known gap: the project currently contains only the assembly marker — no
  `DbContext` or repository exists yet.
