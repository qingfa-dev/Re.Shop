# app

The three runnable application hosts: API, admin, and storefront. _collection · entrypoint_

## Summary

Everything under `app/` is something you can start. The ASP.NET Core API is the
composition root and the only place that touches persistence; the Blazor admin
host renders against it over HTTP; the Vue storefront is the customer-facing SPA
and a workspace package rather than a .NET project. This folder's
`Directory.Build.props` relaxes the documentation requirement the rest of the
repository enforces, because executable hosts carry template-generated code.

## Contains

| Path | Role | Notes |
|---|---|---|
| [`api/`](api/README.md) | ASP.NET Core Web API | Composition root; only writer to the database |
| [`admin/`](admin/README.md) | Blazor admin host | Interactive Server rendering |
| [`storefront/`](storefront/README.md) | Vue 3 SPA | pnpm workspace package `re-shop-storefront` |
| `Directory.Build.props` | Directory intent | `GenerateDocumentationFile=false`, `IsPublishable=true` |

## Boundaries

In scope:
- Executable hosts and their entry points (`Program.cs`).
- Endpoint mapping, middleware pipeline, and DI registration.
- Frontend application code.

Out of scope:
- Domain and application logic → `../src/`.
- Aspire orchestration and shared host defaults → `../aspire/`.
- Automated tests → `../tests/`.

## Assumptions

- The root `Directory.Build.props` is imported first; this folder's props only
  relax documentation for executable projects.
- `GenerateDocumentationFile` is `false` here, so CS1591 cannot fail the build
  on undocumented template code — unlike `../src/`, where it is an error.
- `IsPackable=false` but `IsPublishable=true`: hosts are deployed, not packed.
- Spec §5.3 rule 6 applies to `../src/` only; `OutputType=Exe` is expected here
  and in `../aspire/Re.AppHost/`.

## Conventions

- Composition — `AddRazorComponents`, `AddOpenApi`, resilience handlers — is
  registered in each host's `Program.cs`, never inside `../src/`.
- Admin reaches the API over HTTP, not by referencing `Application` in-process.

## Reading Order

1. [`api/README.md`](api/README.md) — the composition root and the only database writer.
2. [`admin/README.md`](admin/README.md) — the Blazor back office, consuming the API over HTTP.
3. [`storefront/README.md`](storefront/README.md) — the customer-facing SPA.
4. [`../aspire/README.md`](../aspire/README.md) — how the three start together.

## Related

- Parent: [`../README.md`](../README.md)
- Children: [`api/README.md`](api/README.md), [`admin/README.md`](admin/README.md), [`storefront/README.md`](storefront/README.md)
- Orchestration: [`../aspire/README.md`](../aspire/README.md)
- Layout: [`../README.md#repository-layout`](../README.md#repository-layout)
