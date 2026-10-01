# tests

xUnit test projects, one per test kind and per tested layer. _test · internal_

## Summary

This folder holds every automated test in the repository: per-layer unit specs
for the pure `src/` layers, an integration suite that boots the real API, and
the architecture suite that enforces spec §5.3. Test projects are detected as
such by their **location under `tests/`**, not by their file names, so renaming
a project never changes its test profile. Nothing here is packable or
publishable.

## Contains

| Path | Role | Notes |
|---|---|---|
| [`Re.Shop.SharedKernel.UnitTests/`](Re.Shop.SharedKernel.UnitTests/README.md) | Unit specs | Covers `src/Re.Shop.SharedKernel/` |
| [`Re.Shop.Domain.UnitTests/`](Re.Shop.Domain.UnitTests/README.md) | Unit specs | Covers `src/Re.Shop.Domain/` |
| [`Re.Shop.Application.UnitTests/`](Re.Shop.Application.UnitTests/README.md) | Unit specs | Covers `src/Re.Shop.Application/` |
| [`Re.Shop.IntegrationTests/`](Re.Shop.IntegrationTests/README.md) | Integration specs | Boots `app/api` in-process over `WebApplicationFactory` |
| [`Re.Shop.ArchitectureTests/`](Re.Shop.ArchitectureTests/README.md) | Architecture rules | Enforces spec §5.3; see its README |
| `Directory.Build.props` | Directory intent | Re-imports the root props; `IsPackable=false` |

## Boundaries

In scope:
- Automated tests: unit, integration, and architecture.
- Test-only helpers that several test projects share.

Out of scope:
- Production code → `../src/`, `../app/`, `../aspire/`.
- End-to-end browser journeys → not created yet; see `app/storefront/e2e/`.
- Deployment and smoke checks against a running Aspire host → manual.

## Assumptions

- The root `Directory.Build.props` sets `IsTestProject=true` whenever
  `MSBuildProjectDirectory` starts with `tests/`; no name suffix is consulted.
- `Microsoft.NET.Test.Sdk` must be referenced by every project here, or
  `ReSysValidateTestSdk` fails the build.
- Assertions use **Shouldly**; FluentAssertions was removed because its 8.x line
  is commercially licensed for many uses.
- Unit test projects reference only the layer under test plus what that layer
  itself may reference, so a test cannot reach a forbidden dependency.

## Conventions

- Test files use `<Concept>.Spec.cs` — never a bare `*Tests.cs`.
- Test classes and folders declare their kind: `.UnitTests`, `.IntegrationTests`,
  `.ArchitectureTests`.
- There is no `EndToEndTests` project; the guide treats it as optional until the
  application grows.

## Reading Order

1. [`Re.Shop.ArchitectureTests/`](Re.Shop.ArchitectureTests/README.md) — the rules every other project obeys.
2. [`Re.Shop.IntegrationTests/`](Re.Shop.IntegrationTests/README.md) — proof that the composition root boots.
3. [`Re.Shop.SharedKernel.UnitTests/`](Re.Shop.SharedKernel.UnitTests/README.md),
   [`Re.Shop.Domain.UnitTests/`](Re.Shop.Domain.UnitTests/README.md),
   [`Re.Shop.Application.UnitTests/`](Re.Shop.Application.UnitTests/README.md) — layer by layer, inward out.

## Related

- Parent: [`../README.md`](../README.md)
- Children: [`Re.Shop.SharedKernel.UnitTests/`](Re.Shop.SharedKernel.UnitTests/README.md),
  [`Re.Shop.Domain.UnitTests/`](Re.Shop.Domain.UnitTests/README.md),
  [`Re.Shop.Application.UnitTests/`](Re.Shop.Application.UnitTests/README.md),
  [`Re.Shop.IntegrationTests/`](Re.Shop.IntegrationTests/README.md),
  [`Re.Shop.ArchitectureTests/`](Re.Shop.ArchitectureTests/README.md)
- Rules: [`Re.Shop.ArchitectureTests/README.md`](Re.Shop.ArchitectureTests/README.md)

## Notes

- Known gap: the three per-layer unit projects contain no tests yet, because
  `src/` has no code to test. `dotnet test` prints
  `No test is available in …` for each of them.
