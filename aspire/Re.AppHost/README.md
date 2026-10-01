# Re.AppHost

The distributed application host — one process that starts the whole system. _entrypoint · orchestrator_

## Summary

This is the Aspire AppHost: a small executable whose only job is to declare
resources and their relationships, then hand them to the Aspire runtime. It adds
a PostgreSQL container, the API, the admin host, and the storefront as a Vite
app, and expresses the ordering and configuration between them. It never
contains business logic; everything it knows about the other projects is
compiled out of the AppHost assembly and launched as separate processes.

## Contains

| Path | Role | Notes |
|---|---|---|
| `AppHost.cs` | Composition | `AddPostgres`, `AddProject` × 2, `AddViteApp` |
| `Re.AppHost.csproj` | Project file | `Aspire.AppHost.Sdk/13.6.0`, `OutputType=Exe` |
| `aspire.config.json` | Aspire config | |
| `appsettings*.json` | Host configuration | Logging for the AppHost itself |
| `Properties/` | Launch settings | _(no README)_ |

## Boundaries

In scope:
- Declaring resources (databases, projects, frontend apps).
- Their connections, environment variables, and startup ordering.

Out of scope:
- Anything executable inside the API or admin → `../../app/api/`, `../../app/admin/`.
- Shared host concerns (telemetry, resilience) → [`../Re.ServiceDefaults/README.md`](../Re.ServiceDefaults/README.md).
- Business or persistence logic → `../../src/`.
- Referencing a `src/` library — spec §5.3 rule 7 forbids it.

## Assumptions

- References only `../../app/api/` and `../../app/admin/`. The Aspire SDK marks
  both `IsAspireProjectResource=true` and forces `ReferenceOutputAssembly=false`,
  `SkipGetTargetFrameworkProperties` and `ExcludeAssets=all`, so neither appears
  in this project's `deps.json`.
- `AspireUseCliBundle` must stay `true`, or the build fails with `ASPIRE010`.
- `Aspire.Hosting.PostgreSQL` and `Aspire.Hosting.JavaScript` are the only
  hosting packages; versions come from the root `Directory.Packages.props`.
- The storefront is launched with `AddViteApp(...).WithPnpm()` because the
  frontend is hoisted into the root pnpm workspace and has no lockfile of its own.
- A `UserSecretsId` is declared for local secrets; no secret is committed.

## Conventions

- Resource names are lowercase and stable (`postgres`, `api`, `admin`,
  `storefront`) — they become service names other resources resolve by.
- Configuration crosses process boundaries through `WithReference` and
  `WithEnvironment`, never through a shared file.

## Flows

### Start-up sequence

1. `DistributedApplication.CreateBuilder` reads `aspire.config.json`.
2. `AddPostgres(name: "postgres")` declares the database resource.
3. `AddProject<Projects.Re_Shop_Api>` takes a `WithReference` to it under the
   connection name `Shop`, and `WaitFor(postgres)` gates the API on its health.
4. `AddProject<Projects.Re_Shop_Admin>` receives `Api__BaseUrl = http://api`.
5. `AddViteApp("storefront", …).WithPnpm()` resolves the frontend directory
   relative to `AppHostDirectory` and runs its `dev` script.
6. `builder.Run()` hands the whole graph to the Aspire runtime.

## Related

- Parent: [`../README.md`](../README.md)
- Sibling: [`../Re.ServiceDefaults/README.md`](../Re.ServiceDefaults/README.md)
- Hosts: [`../../app/api/README.md`](../../app/api/README.md), [`../../app/admin/README.md`](../../app/admin/README.md), [`../../app/storefront/README.md`](../../app/storefront/README.md)
- Run: [`../../README.md#run-everything-with-aspire`](../../README.md#run-everything-with-aspire)
