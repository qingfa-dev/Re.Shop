# aspire

Aspire orchestration: the AppHost that starts everything, and the shared host defaults it expects. _collection · orchestrator_

## Summary

This folder holds the two projects that make local development a single command.
`Re.AppHost` is the orchestrator — it starts PostgreSQL, the API, the admin host,
and the storefront, then wires their connections and waits on their health.
`Re.ServiceDefaults` is the library the individual hosts are meant to share for
telemetry, resilience, and health checks. Only the AppHost is executable; the
defaults library is referenced, not run.

## Contains

| Path | Role | Notes |
|---|---|---|
| [`Re.AppHost/`](Re.AppHost/README.md) | Distributed app host | `OutputType=Exe`; composes postgres, api, admin, storefront |
| [`Re.ServiceDefaults/`](Re.ServiceDefaults/README.md) | Shared host library | Health checks, resilience, OpenTelemetry |
| `Directory.Build.props` | Directory intent | Re-imports root props; `GenerateDocumentationFile=false` |

## Boundaries

In scope:
- Composition of the local development environment.
- Cross-cutting host concerns shared by more than one process.

Out of scope:
- Endpoint and DI wiring that is specific to one host → `../app/`.
- Domain, application, persistence logic → `../src/`.
- Automated tests → `../tests/`.

## Assumptions

- The AppHost may reference **only** `../app/api` and `../app/admin` — never a
  `src/` library (spec §5.3 rule 7), asserted by
  `AppHost_references_only_the_two_hosts`.
- `Aspire.AppHost.Sdk` injects `ReferenceOutputAssembly=false` for every aspire
  resource (`Sdk.targets:37`), so those two hosts never appear in the AppHost's
  own `deps.json`. They are launched as separate processes instead.
- `AspireUseCliBundle` must remain `true`; setting it `false` raises `ASPIRE010`
  and fails the build.
- `GenerateDocumentationFile` is `false` here, so XML summaries are not enforced.

## Conventions

- One resource per `Add…` call, named explicitly (`name: "postgres"`).
- Cross-process wiring goes through `WithReference` / `WithEnvironment`, never
  through shared static state.

## Flows

### Local development start-up

1. `Re.AppHost` declares PostgreSQL and waits for it to become ready.
2. The API starts with a `Shop` connection reference to that container.
3. The admin host starts with `Api__BaseUrl` pointed at the API.
4. The storefront starts through `AddViteApp` under pnpm.

## Related

- Parent: [`../README.md`](../README.md)
- Children: [`Re.AppHost/README.md`](Re.AppHost/README.md), [`Re.ServiceDefaults/README.md`](Re.ServiceDefaults/README.md)
- Run: [`../README.md#run-everything-with-aspire`](../README.md#run-everything-with-aspire)

## Notes

- Known gap: `Re.ServiceDefaults` currently contains no source files — see its
  README before assuming `AddServiceDefaults` exists.
