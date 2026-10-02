# src

Clean Architecture library layers, with dependencies pointing strictly inward. _collection · kernel_

## Summary

This folder holds the five class libraries that make up Re.Shop's core, plus the
directory-level MSBuild intent that applies to all of them. Dependencies point
inward only: `SharedKernel` is referenced by everyone and references nothing,
while `Infrastructure` reaches the furthest out. Nothing here is executable —
`src/` is library code. The Domain now contains validated value objects,
identity and aggregate building blocks, and Mediator-backed event contracts;
the other layers remain scaffolded around their architecture markers.

## Contains

| Path | Role | Notes |
|---|---|---|
| [`Re.Shop.SharedKernel/`](Re.Shop.SharedKernel/README.md) | Cross-cutting primitives | Referenced by every layer; references nothing |
| [`Re.Shop.Domain/`](Re.Shop.Domain/README.md) | Domain model | Aggregates, entities, domain events |
| [`Re.Shop.Application/`](Re.Shop.Application/README.md) | Application services | Handlers, orchestration, validators |
| [`Re.Shop.Contracts/`](Re.Shop.Contracts/README.md) | Public DTOs | Request/response shapes; references nothing |
| [`Re.Shop.Infrastructure/`](Re.Shop.Infrastructure/README.md) | Persistence adapters | EF Core + Npgsql |
| `Directory.Build.props` | Directory intent | Re-imports the root props; bans `OutputType=Exe` |

## Boundaries

In scope:
- Pure library code that composes into the API and the admin host.
- The inward-only dependency direction enforced by spec §5.3.

Out of scope:
- Executable entry points — `OutputType=Exe` is forbidden here → see `../app/`.
- Composition and DI wiring → `../app/api/`, `../app/admin/`.
- Host bootstrap, telemetry, resilience → `../aspire/`.
- Test code → `../tests/`.

## Assumptions

- The repository-root `Directory.Build.props` is imported first; this folder's
  props only layer directory intent on top of it.
- `GenerateDocumentationFile` stays `true`, so with `TreatWarningsAsErrors` every
  public and protected member needs an XML summary — CS1591 is an error here.
- Each project exposes a `*Marker` type that
  [`tests/Re.Shop.ArchitectureTests`](../tests/Re.Shop.ArchitectureTests/README.md)
  uses to locate the assembly; markers carry no behaviour.

## Invariants

- Dependencies point inward only: `SharedKernel` references nothing, `Domain`
  reaches `SharedKernel`, `Application` reaches `Domain` + `SharedKernel`, and
  `Infrastructure` reaches `Domain` + `SharedKernel`.
- Nothing under this folder is executable — `OutputType=Exe` is forbidden.
- Every public and protected member carries an XML summary; CS1591 is an error.
- Each project keeps exactly one `*Marker` type with no behaviour.

## Conventions

- A dependency may only point inward; see [spec §5.3](../docs/superpowers/specs/2026-10-01-central-config-clean-architecture-scaffold-design.md).
- Never add `OutputType=Exe` to a project under `src/`.
- Folder and file names follow [`guide/dotted-file-naming-guide.md`](../guide/dotted-file-naming-guide.md).

## Related

- Parent: [`../README.md`](../README.md)
- Children: [`Re.Shop.SharedKernel/`](Re.Shop.SharedKernel/README.md),
  [`Re.Shop.Domain/`](Re.Shop.Domain/README.md),
  [`Re.Shop.Application/`](Re.Shop.Application/README.md),
  [`Re.Shop.Contracts/`](Re.Shop.Contracts/README.md),
  [`Re.Shop.Infrastructure/`](Re.Shop.Infrastructure/README.md)
- Rules: [`tests/Re.Shop.ArchitectureTests/README.md`](../tests/Re.Shop.ArchitectureTests/README.md)
- Layout: [`../README.md#repository-layout`](../README.md#repository-layout)

## Notes

- Known gap: Application workflows and Infrastructure persistence adapters are
  not implemented yet.
- Domain contains real business contracts and value objects; each layer's
  `*Marker.cs` remains only an assembly anchor.
