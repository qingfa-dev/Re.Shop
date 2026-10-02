# Re.Shop.Application.UnitTests

Unit specs for handlers, application services and validators. _test · internal_

## Summary

This project tests use-case orchestration and input validation in isolation:
handlers are driven with hand-built commands and queries, and validators are
constructed directly rather than resolved from a container. It references
`Re.Shop.Application`, `Re.Shop.Domain`, and `Re.Shop.SharedKernel` — the exact
inward set spec §5.3 rule 2 permits — so a test here cannot compile against
`Infrastructure`, `Api`, or `Contracts`. That keeps application behaviour
verifiable without a database or a running host.

## Contains

| Path | Role | Notes |
|---|---|---|
| `Re.Shop.Application.UnitTests.csproj` | Project file | References `Application` + `Domain` + `SharedKernel` |

## Boundaries

In scope:
- Handler and application-service behaviour.
- Input validator rules for commands and queries.
- Orchestration decisions, including failure paths.

Out of scope:
- Domain invariants that hold without the application → [`../Re.Shop.Domain.UnitTests/`](../Re.Shop.Domain.UnitTests/README.md).
- Whether the whole graph resolves in a real host → [`../Re.Shop.IntegrationTests/`](../Re.Shop.IntegrationTests/README.md).
- Persistence and EF Core → `src/Re.Shop.Infrastructure/`.
- Public DTO shapes → `src/Re.Shop.Contracts/` (rule 2 forbids this layer from
  depending on them, so tests here must not either).

## Assumptions

- Located under `tests/`, so `IsTestProject=true` and `IsPackable=false` apply
  automatically from the root `Directory.Build.props`.
- A global `using Xunit;` is declared in the project file.

## Invariants

- References `Re.Shop.Application`, `Re.Shop.Domain`, and `Re.Shop.SharedKernel`
  and nothing else — the test-side mirror of spec §5.3 rule 2.
- `Re.Shop.Contracts` and `Re.Shop.Infrastructure` must never appear in the
  project file; adding one weakens the guardrail and should be treated as a
  deliberate architecture decision, not a convenience.

## Conventions

- Test files use the suffix `.Spec.cs`.
- Validators are instantiated directly; resolving them from DI belongs to the
  integration suite.
- Arrange with explicit test data; no builder or fixture until one earns itself.

## Related

- Parent: [`../README.md`](../README.md)
- Code under test: [`src/Re.Shop.Application/`](../../src/Re.Shop.Application/README.md)
- Rules: [`../Re.Shop.ArchitectureTests/README.md`](../Re.Shop.ArchitectureTests/README.md)

## Notes

- Known gap: contains no test files yet — Application use-case handlers have
  not been implemented; Domain model behavior is covered by Domain unit specs.
