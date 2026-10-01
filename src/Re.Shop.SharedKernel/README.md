# Re.Shop.SharedKernel

Cross-cutting primitives shared by every other layer. _layer · shared_

## Summary

The innermost layer of the architecture: value objects, result and error types,
and anything else that has no opinion about the domain. It is referenced by all
four sibling projects and is the only project allowed to reference nothing at
all. Because every layer depends on it, anything added here is effectively
permanent API for the whole system.

## Contains

| Path | Role | Notes |
|---|---|---|
| `SharedKernelMarker.cs` | Assembly anchor | Located by `Re.Shop.ArchitectureTests`; no behaviour |
| `Structures/Eithers/` | Result types | _(empty — no README)_ |
| `Structures/Errors/` | Error primitives | _(empty — no README)_ |
| `Re.Shop.SharedKernel.csproj` | Project file | No `ProjectReference` entries |

## Boundaries

In scope:
- Primitives that are genuinely domain-agnostic: result types, error shapes,
  strongly-typed IDs, base value-object helpers.

Out of scope:
- Anything that knows what a product, order, or customer is → `../Re.Shop.Domain/`.
- Persistence concerns → `../Re.Shop.Infrastructure/`.
- Cross-layer types used only by one consumer → keep them with that consumer.

## Assumptions

- References no other `Re.Shop.*` project; this is asserted by
  `SharedKernel_depends_on_nothing` in [`tests/Re.Shop.ArchitectureTests/`](../../tests/Re.Shop.ArchitectureTests/README.md).
- `GenerateDocumentationFile` is `true` here, so every public and protected
  member needs an XML summary or the build fails on CS1591.

## Invariants

- References no other project of any kind — asserted by
  `SharedKernel_depends_on_nothing`.
- Nothing here may mention the domain, an ORM, or a transport.

## Contracts

- `SharedKernelMarker` — the anchor the architecture suite resolves.
- `Structures/Eithers/` — result types (placeholder, currently empty).
- `Structures/Errors/` — error primitives (placeholder, currently empty).

## Conventions

- Nothing may be added to this layer "for convenience" — it is on everyone's
  compile path.

## Related

- Parent: [`../README.md`](../README.md)
- Siblings: [`../Re.Shop.Domain/`](../Re.Shop.Domain/README.md),
  [`../Re.Shop.Application/`](../Re.Shop.Application/README.md),
  [`../Re.Shop.Contracts/`](../Re.Shop.Contracts/README.md),
  [`../Re.Shop.Infrastructure/`](../Re.Shop.Infrastructure/README.md)
- Rules: [`tests/Re.Shop.ArchitectureTests/`](../../tests/Re.Shop.ArchitectureTests/README.md)

## Notes

- Known gap: `Structures/Eithers/` and `Structures/Errors/` are empty
  placeholders; only the marker exists.
