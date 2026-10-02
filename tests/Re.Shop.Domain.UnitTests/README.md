# Re.Shop.Domain.UnitTests

Unit specs for aggregates, entities and domain invariants. _test · internal_

## Summary

This project tests the domain model in complete isolation: aggregate invariants,
entity lifecycle rules, and the domain events the model raises. It references
`Re.Shop.Domain` and `Re.Shop.SharedKernel` — exactly the set spec §5.3 rule 1
allows the domain itself — so a test here cannot accidentally pull in EF Core,
HTTP, or the API. Tests construct aggregates directly and assert on state, with
no container, no database, and no mocking framework.

## Contains

| Path | Role | Notes |
|---|---|---|
| `Re.Shop.Domain.UnitTests.csproj` | Project file | References `Re.Shop.Domain` + `Re.Shop.SharedKernel` |
| `GlobalUsings.cs` | Test imports | Imports Shouldly |
| `Contracts/` | Contract specs | Entity, aggregate, and Mediator event contracts |
| `ValueObjects/` | Factory specs | Valid/invalid inputs and normalization for all sixteen value objects |
| `Domain/` | Entity and aggregate specs | Identity, state-based events, event replay, and versioning |

## Boundaries

In scope:
- Aggregate and entity invariants.
- Value-object construction and equality rules.
- Domain events raised by the model.

Out of scope:
- Orchestration, handlers, validators → [`../Re.Shop.Application.UnitTests/`](../Re.Shop.Application.UnitTests/README.md).
- Primitives in the shared kernel → [`../Re.Shop.SharedKernel.UnitTests/`](../Re.Shop.SharedKernel.UnitTests/README.md).
- Persistence round-trips → [`../Re.Shop.IntegrationTests/`](../Re.Shop.IntegrationTests/README.md).
- Dependency rules → [`../Re.Shop.ArchitectureTests/`](../Re.Shop.ArchitectureTests/README.md).

## Assumptions

- Located under `tests/`, so `IsTestProject=true` and `IsPackable=false` apply
  automatically from the root `Directory.Build.props`.
- A global `using Xunit;` is declared in the project file; `GlobalUsings.cs`
  imports Shouldly.

## Invariants

- References `Re.Shop.Domain` and `Re.Shop.SharedKernel` and nothing else —
  the exact set spec §5.3 rule 1 permits the domain itself.
- EF Core, HTTP, and the API therefore stay unreachable from this project.

## Conventions

- Test files use the suffix `.Spec.cs`.
- Arrange by constructing the aggregate; do not reach for a repository or a test
  double.
- Test names describe the invariant, not the method being called.

## Related

- Parent: [`../README.md`](../README.md)
- Code under test: [`src/Re.Shop.Domain/`](../../src/Re.Shop.Domain/README.md)
- Rules: [`../Re.Shop.ArchitectureTests/README.md`](../Re.Shop.ArchitectureTests/README.md)

## Notes

- Factory behavior, identity, aggregate event collection, and event-sourcing
  lifecycle behavior are covered by focused unit specs.
