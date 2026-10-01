# storefront

Vue 3 single-page application for the customer-facing shop. _entrypoint · consumer_

## Summary

The storefront is a Vite + Vue 3 SPA and a pnpm workspace package named
`re-shop-storefront`. It is the only non-.NET application in the repository and
is started by the Aspire AppHost through `AddViteApp` rather than `dotnet run`.
Its package versions come from the `catalog:` in the root `pnpm-workspace.yaml`,
so this folder deliberately declares no versions of its own.

## Contains

| Path | Role | Notes |
|---|---|---|
| `src/` | Application code | Vue components, views, stores; _(no README)_ |
| `e2e/` | Playwright journeys | Driven by `test:e2e`; _(no README)_ |
| `public/` | Static assets | Served verbatim; _(no README)_ |
| `package.json` | Package manifest | Name `re-shop-storefront`; every dependency is `catalog:` |
| `vite.config.ts` | Build config | Vite |
| `vitest.config.ts` | Unit test config | `pnpm test:unit` |
| `playwright.config.ts` | E2E config | `pnpm test:e2e` |
| `tsconfig*.json` | TypeScript config | `tsconfig.app.json` extends `../../tsconfig.base.json` |
| `eslint.config.ts`, `.oxlintrc.json`, `.oxfmtrc.json` | Lint/format | `pnpm lint` runs oxlint then eslint |
| `pnpm-workspace.yaml` | Nested workspace | See Notes — contains inert `overrides` |
| `README.md` | This file | Replaces the original Vite template boilerplate |

## Boundaries

In scope:
- Rendering, routing, and client-side state for the shop.
- Consuming the API's HTTP contract.

Out of scope:
- Server-side logic and persistence → [`../api/README.md`](../api/README.md).
- Admin tooling → [`../admin/README.md`](../admin/README.md).
- .NET project configuration — this folder has no `.csproj`.

## Assumptions

- Installed and run through the root workspace: `pnpm install` from the
  repository root, not inside this folder.
- Node comes from `fnm` in this environment; a system Node may fail to resolve
  shared libraries.
- `catalog:` entries are resolved against the **root** `pnpm-workspace.yaml`.
- `tsconfig.app.json` extends `../../tsconfig.base.json`, so shared compiler
  options live there rather than here.
- Vitest is selected for unit tests and Playwright for E2E, despite spec §11.5
  originally saying neither would be chosen.

## Conventions

- No version literal appears in `package.json` — every value is `catalog:`.
- Test files use the suffix `.Spec`-style naming from the frontend tooling;
  directories are split into `src/` (unit) and `e2e/` (journeys).

## Reading Order

1. `src/main.ts` — application bootstrap.
2. `src/router/index.ts` — the route table.
3. `src/views/` — the pages the router resolves to.
4. `src/stores/` — shared client state.
5. `e2e/` — Playwright journeys over the four above.

## Related

- Parent: [`../README.md`](../README.md)
- Host wiring: [`../../aspire/Re.AppHost/README.md`](../../aspire/Re.AppHost/README.md)
- Frontend section: [`../../README.md#frontend`](../../README.md#frontend)
- Commands: `pnpm dev`, `pnpm build`, `pnpm test:unit`, `pnpm test:e2e`, `pnpm type-check`, `pnpm lint`

## Notes

- `TEMP:` the nested `pnpm-workspace.yaml` in this folder carries `overrides`
  for a Vue `rc` and `typescript-native-bridge` that are **inert** — pnpm does
  not apply them from a nested workspace file. They were left untouched rather
  than silently promoted to the root.
- Known gap: `test:e2e` exists but the suite is minimal; `src/` holds little
  beyond the scaffold's starting components.
