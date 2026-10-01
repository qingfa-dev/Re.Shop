# Re.Shop.IntegrationTests

Boots the real API composition root and drives it over HTTP. _test · internal_

## Summary

This suite exercises the application across a process boundary rather than in
isolation: it constructs `app/api` through `WebApplicationFactory<Program>` and
sends real requests through the in-memory `TestServer` pipeline. That means
dependency injection, middleware, and configuration are all exercised for real,
while still requiring no Docker and no database connection. It is the layer
where "the wiring works" is proven, as distinct from "the logic is correct".

## Contains

| Path | Role | Notes |
|---|---|---|
| `ApiHost.Spec.cs` | Host boot spec | Asserts the composition root serves requests without a 500 |
| `Re.Shop.IntegrationTests.csproj` | Project file | References `app/api`; brings `Microsoft.AspNetCore.Mvc.Testing` |

## Boundaries

In scope:
- Booting `app/api` with its real DI container and middleware pipeline.
- In-process HTTP round-trips through `TestServer`.
- Asserting that services resolve from the composition root.

Out of scope:
- Pure logic and validators → `../Re.Shop.SharedKernel.UnitTests/` and siblings.
- Dependency direction → `../Re.Shop.ArchitectureTests/`.
- A real PostgreSQL server — `Testcontainers.PostgreSql` and `Respawn` are
  deliberately deferred in spec §9.4, so no container is started here.
- Browser journeys → see `app/storefront/e2e/`.

## Assumptions

- `app/api/Program.cs` declares `public partial class Program { }`, which is
  what lets `WebApplicationFactory<Program>` reach the entry point. Removing
  that line breaks every test in this folder.
- No database provider is registered, and none is needed: `AddDbContext` builds
  its options lazily and never dials a connection (spec §3).
- `Microsoft.AspNetCore.Mvc.Testing` is versioned centrally in
  `Directory.Packages.props` §17 alongside `TestHost`, `Testcontainers` and
  `Respawn` — the versions are pinned even though the last two are unused.
- `Microsoft.NET.Test.Sdk`, xunit v3, and Shouldly are referenced with no
  `Version` attribute.

## Conventions

- Test files use the suffix `.Spec.cs`.
- One `WebApplicationFactory` per test, disposed with `await using`.
- Assertions use Shouldly; FluentAssertions is not available in this repo.

## Flows

### Boot the composition root

1. `WebApplicationFactory<Program>` reaches the entry point through
   `public partial class Program { }`.
2. The factory builds the real `WebApplication` and its in-memory `TestServer`.
3. A request travels the full middleware pipeline without leaving the process.
4. The assertion is that the response is not a 500 — no endpoint is mapped yet.

## Related

- Parent: [`../README.md`](../README.md)
- Code under test: [`../../app/api/README.md`](../../app/api/README.md)
- Rules: [`../Re.Shop.ArchitectureTests/README.md`](../Re.Shop.ArchitectureTests/README.md)
- Run: `make test api` from the repository root

## Notes

- `TEMP:` the current assertion is that requests do not return 500, because
  `app/api` exposes no endpoints yet. Tighten it to concrete routes as they land.
- Known gap: no database-backed integration test exists; that needs
  `Testcontainers.PostgreSql` to be un-deferred and a `DbContext` to exist.
