# Re.Shop.Api

The ASP.NET Core composition root — the only process that touches the database. _entrypoint · api_

## Summary

This host wires the application together: it registers the application services,
the infrastructure adapters, and the HTTP surface that exposes them. It is the
single-homed entry point for all persistence, which is what allows the admin and
storefront to stay thin presentation layers. Everything that is not this process
reaches the system through its HTTP contract.

## Contains

| Path | Role | Notes |
|---|---|---|
| `Program.cs` | Composition root | Bare `CreateBuilder`/`Build`/`Run`; `public partial class Program` at the end |
| `ApiMarker.cs` | Assembly anchor | Located by `Re.Shop.ArchitectureTests`; no behaviour |
| `appsettings.json` | Configuration | Currently only `Logging` and `AllowedHosts` |
| `appsettings.Development.json` | Local overrides | Not for production values |
| `Properties/` | Launch settings | `dotnet run` profiles; _(no README)_ |
| `Re.Shop.Api.csproj` | Project file | Web SDK; references `Application`, `Infrastructure`, `Contracts`, `SharedKernel`, `Re.ServiceDefaults` |

## Boundaries

In scope:
- DI registration and the middleware pipeline.
- Endpoint definitions and request/response mapping.
- Connection strings and other host-level configuration.

Out of scope:
- Business rules → [`../../src/Re.Shop.Application/README.md`](../../src/Re.Shop.Application/README.md).
- Domain invariants → [`../../src/Re.Shop.Domain/README.md`](../../src/Re.Shop.Domain/README.md).
- DbContext and repositories → [`../../src/Re.Shop.Infrastructure/README.md`](../../src/Re.Shop.Infrastructure/README.md).
- Health checks, telemetry, resilience → [`../../aspire/Re.ServiceDefaults/README.md`](../../aspire/Re.ServiceDefaults/README.md).
- Admin UI → [`../admin/README.md`](../admin/README.md).

## Assumptions

- `public partial class Program { }` at the end of `Program.cs` is required —
  [`../../tests/Re.Shop.IntegrationTests`](../../tests/Re.Shop.IntegrationTests/README.md)
  reaches the entry point through it. Deleting that line breaks every test there.
- Of the whole repository, only this project and `src/Re.Shop.Infrastructure/`
  may reference `Microsoft.EntityFrameworkCore*` or `Npgsql*`.
- No database provider is registered yet, and none is needed for the current
  integration tests: `AddDbContext` builds its options lazily and never dials.
- `GenerateDocumentationFile` is `false` for this host, so XML summaries are
  not enforced here.

## Contracts

- `public partial class Program { }` at the end of `Program.cs` — the entry point
  [`../../tests/Re.Shop.IntegrationTests/`](../../tests/Re.Shop.IntegrationTests/README.md)
  boots through.
- The HTTP surface every other host consumes; **no endpoint is mapped yet**, so
  there is currently nothing to call.
- The connection name `Shop`, handed down from [`../../aspire/Re.AppHost/`](../../aspire/Re.AppHost/README.md).

## Conventions

- Composition lives here, not in `../../src/`; libraries must not self-register.
- Configuration keys are read once in `Program.cs`, not scattered through
  services.

## Flows

### Boot

1. `Program.cs` creates a `WebApplication` from the builder and calls `Run`.
2. In tests, `WebApplicationFactory<Program>` rebuilds the same entry point in memory.
3. No `DbContext` is registered, so no connection is opened during boot.

## Related

- Parent: [`../README.md`](../README.md)
- Tests: [`../../tests/Re.Shop.IntegrationTests/README.md`](../../tests/Re.Shop.IntegrationTests/README.md)
- Rules: [`../../tests/Re.Shop.ArchitectureTests/README.md`](../../tests/Re.Shop.ArchitectureTests/README.md)
- Run: `make run api` or start everything through Aspire

## Notes

- Known gap: `appsettings.json` carries no `ConnectionStrings` entry, and
  `Program.cs` maps no endpoints — `GET /` returns 404 by design for now.
