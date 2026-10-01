# Re.Shop.SharedKernel.UnitTests

Unit specs for the cross-cutting primitives layer. _test · internal_

## Summary

This project tests `src/Re.Shop.SharedKernel/` — value objects, result and error
types, and anything else that must behave correctly in isolation. It references
`Re.Shop.SharedKernel` and nothing else, which makes an accidental dependency on
a higher layer a compile error rather than a review finding. It is deliberately
small and dependency-free so the innermost layer stays verifiable without a host,
a database, or a test framework beyond xUnit and Shouldly.

## Contains

| Path | Role | Notes |
|---|---|---|
| `Re.Shop.SharedKernel.UnitTests.csproj` | Project file | References `Re.Shop.SharedKernel` only |

## Boundaries

In scope:
- Behaviour of `Re.Shop.SharedKernel` types in isolation.
- Edge cases for primitives shared by every other layer.

Out of scope:
- Domain behaviour → [`../Re.Shop.Domain.UnitTests/`](../Re.Shop.Domain.UnitTests/README.md).
- Application or handler behaviour → [`../Re.Shop.Application.UnitTests/`](../Re.Shop.Application.UnitTests/README.md).
- Anything needing the API, a database, or a container → [`../Re.Shop.IntegrationTests/`](../Re.Shop.IntegrationTests/README.md).
- Dependency rules → [`../Re.Shop.ArchitectureTests/`](../Re.Shop.ArchitectureTests/README.md).

## Assumptions

- Located under `tests/`, so `IsTestProject=true` and `IsPackable=false` apply
  automatically from the root `Directory.Build.props`.
- `Microsoft.NET.Test.Sdk`, xunit v3, and Shouldly are referenced here with no
  `Version` attribute — versions come from the root `Directory.Packages.props`.
- A global `using Xunit;` is declared in the project file, so test files need no
  `using Xunit;` directive.

## Invariants

- References `Re.Shop.SharedKernel` and no other project; a stray reference is
  a compile error here, not a review finding.
- The reference set mirrors spec §5.3 rule 1, so this project stays as
  dependency free as the layer it tests.

## Conventions

- Test files use the suffix `.Spec.cs`.
- Tests construct the subject directly; no `WebApplicationFactory`, no mocking
  of inward dependencies.

## Related

- Parent: [`../README.md`](../README.md)
- Code under test: [`src/Re.Shop.SharedKernel/`](../../src/Re.Shop.SharedKernel/README.md)
- Rules: [`../Re.Shop.ArchitectureTests/README.md`](../Re.Shop.ArchitectureTests/README.md)

## Notes

- Known gap: contains no test files yet — `src/Re.Shop.SharedKernel/` currently
  holds only its assembly marker, so there is nothing to assert.
