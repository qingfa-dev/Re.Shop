# Re.Shop.Admin

Blazor Interactive Server host for back-office operations. _entrypoint · internal_

## Summary

This host renders the admin UI with Blazor components running interactively on
the server. It deliberately references only `Contracts` and `SharedKernel` —
never `Application` or `Infrastructure` — so all data access stays single-homed
in the API. That constraint is architectural, not incidental: if this project
ever needs a `DbContext`, the layering has already been broken.

## Contains

| Path | Role | Notes |
|---|---|---|
| `Program.cs` | Composition root | `AddRazorComponents` + interactive server render mode |
| `AdminMarker.cs` | Assembly anchor | Located by `Re.Shop.ArchitectureTests`; no behaviour |
| `Components/App.razor` | Root component | Shell document |
| `Components/Routes.razor` | Router | |
| `Components/Layout/` | Layout components | `MainLayout.razor` + its scoped CSS; _(no README)_ |
| `Components/_Imports.razor` | Global usings | |
| `appsettings*.json` | Configuration | `Logging`, `AllowedHosts` |
| `Re.Shop.Admin.csproj` | Project file | Web SDK; references `Contracts`, `SharedKernel`, `Re.ServiceDefaults` |

## Boundaries

In scope:
- Blazor components, pages and the application layout.
- Client-side state and presentation concerns.

Out of scope:
- Domain, application, or persistence logic — **this project may not reference
  `Re.Shop.Domain`, `Re.Shop.Application`, or `Re.Shop.Infrastructure` at all.**
- Writing to the database → [`../api/README.md`](../api/README.md).
- Customer-facing UI → [`../storefront/README.md`](../storefront/README.md).

## Assumptions

- Data arrives over HTTP from `../api/`; per spec §125 the admin must not
  invoke `Application` in-process.
- `GenerateDocumentationFile` is `false` for this host, so XML summaries are
  not enforced here.
- The permitted reference set is asserted by
  `Admin_depends_only_on_contracts_and_shared_kernel` in
  [`../../tests/Re.Shop.ArchitectureTests`](../../tests/Re.Shop.ArchitectureTests/README.md).

## Contracts

- [`../api/README.md`](../api/README.md) is the only data source; its HTTP JSON
  contract is what this host compiles against.
- References `Re.Shop.Contracts` and `Re.Shop.SharedKernel` and nothing else.

## Conventions

- Components are `@`-marked Razor files; scoped CSS sits beside its component as
  `<Name>.razor.css`.
- Business rules never appear in a component — push them to `../../src/`.

## Flows

### Render

1. `Program.cs` registers Razor components with an interactive server render mode.
2. `Components/App.razor` lays out the document; `Routes.razor` resolves the page.
3. Data fetch begins once an HTTP client to the API is registered — it is not yet.

## Related

- Parent: [`../README.md`](../README.md)
- Data source: [`../api/README.md`](../api/README.md)
- Rules: [`../../tests/Re.Shop.ArchitectureTests/README.md`](../../tests/Re.Shop.ArchitectureTests/README.md)
- Run: `make run admin` or start everything through Aspire

## Notes

- Known gap: no HTTP client to the API is registered yet, and no pages exist
  beyond the layout shell.
