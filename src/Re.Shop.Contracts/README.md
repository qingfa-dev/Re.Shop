# Re.Shop.Contracts

Public request and response shapes shared across process boundaries. _layer · api_

## Summary

This project holds the DTOs that cross the API boundary — the request and
response contracts an external client compiles against. It references nothing,
not even `SharedKernel`, so a consumer can depend on the contracts without
dragging in the domain model. Keeping it dependency-free is what lets the admin
Blazor host and any future client share types without inheriting the rest of the
system.

## Contains

| Path | Role | Notes |
|---|---|---|
| `ContractsMarker.cs` | Assembly anchor | Located by `Re.Shop.ArchitectureTests`; no behaviour |
| `Re.Shop.Contracts.csproj` | Project file | No `ProjectReference` entries |

## Boundaries

In scope:
- Request/response DTOs and the enums they carry.
- Anything an API consumer must compile against.

Out of scope:
- Domain entities and aggregates → `../Re.Shop.Domain/`.
- Validation logic → `../Re.Shop.Application/`.
- Internal types used only inside one process → keep them with their layer.

## Assumptions

- References no other `Re.Shop.*` project; this is asserted by
  `Contracts_depends_on_nothing` in [`tests/Re.Shop.ArchitectureTests/`](../../tests/Re.Shop.ArchitectureTests/README.md).
- The API maps between these shapes and the domain; the domain never sees them.
- `GenerateDocumentationFile` is `true`, so every public member needs an XML
  summary.

## Invariants

- References no other `Re.Shop.*` project — not even `SharedKernel`; spec §5.3
  rule 3, asserted by `Contracts_depends_on_nothing`.
- The domain never sees these types; mapping happens in `app/api`.

## Contracts

- Request and response DTOs shared by [`../../app/api/`](../../app/api/README.md)
  and [`../../app/admin/`](../../app/admin/README.md).
- `ContractsMarker` — the anchor the architecture suite resolves.

## Conventions

- Contracts are additive: never repurpose or rename a published field without a
  versioning decision.

## Related

- Parent: [`../README.md`](../README.md)
- Consumers: [`../../app/api/README.md`](../../app/api/README.md), [`../../app/admin/README.md`](../../app/admin/README.md)
- Rules: [`tests/Re.Shop.ArchitectureTests/`](../../tests/Re.Shop.ArchitectureTests/README.md)

## Notes

- Currently contains only the assembly marker.
