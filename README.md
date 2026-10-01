# Re.Shop

**An online shop, built as a monorepo.**

> **Status:** early scaffold. Features listed below are planned, not yet implemented.

## Features (WIP)

## Tech stack

- **API:** ASP.NET Core Web API (`app/api`)
- **Admin:** Blazor Web App, interactive server (`app/admin`)
- **Storefront:** Vue 3 + TypeScript + Vite (`app/storefront`)
- **Database:** PostgreSQL with Entity Framework Core
- **Orchestration:** .NET Aspire AppHost (`aspire/Re.AppHost`)
- **.NET:** clean-architecture libraries under `src/`, tests under `tests/`

## Getting started

### Prerequisites

- [.NET SDK 10.0.401](https://dotnet.microsoft.com/download) — pinned in `global.json`
- Node.js `^22.18.0` or `>=24.12.0`, and `pnpm` 12
- [Docker](https://docs.docker.com/get-docker/) — required only to run the `postgres` container via Aspire

### Initial setup

```bash
export PATH=/usr/share/dotnet:$PATH   # if /usr/share/dotnet is not on PATH

dotnet restore --force-evaluate   # first run only: rewrites the lock files
dotnet build
dotnet test

pnpm install                      # from the repository root
```

### Run everything with Aspire

```bash
dotnet run --project aspire/Re.AppHost
```

This starts `postgres`, `api`, `admin` and `storefront`, wires the connection
string into the API, and launches the storefront's Vite dev server through
**pnpm** (`AddViteApp(...).WithPnpm()`).

Run a single piece instead:

```bash
dotnet run --project app/api
dotnet run --project app/admin
pnpm --filter re-shop-storefront dev
```

## Build and test

```bash
dotnet build   # 0 warnings is the bar: TreatWarningsAsErrors is on repo-wide
dotnet test
```

## Frontend

```bash
pnpm install     # from the repository root
pnpm dev         # run every workspace package's dev server
pnpm build       # type-check + production build
pnpm lint        # lint every workspace package
pnpm type-check  # type-check only
```

The storefront lives in `app/storefront`. Its dependency versions are pinned
centrally in the `catalog:` block of `pnpm-workspace.yaml`, and
`app/storefront/package.json` references them with the `catalog:` protocol —
the JavaScript counterpart to `Directory.Packages.props`.

Shared TypeScript settings live in `tsconfig.base.json` at the repository root;
`app/storefront/tsconfig.app.json` extends it.

## Repository layout

```
Re.Shop/
├── app/
│   ├── api/           # ASP.NET Core Web API
│   ├── admin/         # Blazor admin host
│   └── storefront/    # Vue 3 SPA (pnpm workspace package)
├── aspire/
│   ├── Re.AppHost/        # Aspire orchestration — composes api, admin, storefront, postgres
│   └── Re.ServiceDefaults/ # shared service defaults for the hosts
├── src/               # clean-architecture libraries
│   ├── Re.Shop.SharedKernel/
│   ├── Re.Shop.Domain/
│   ├── Re.Shop.Contracts/
│   ├── Re.Shop.Application/
│   └── Re.Shop.Infrastructure/
├── tests/             # xUnit test projects
├── Directory.Build.props          # shared MSBuild settings for all projects
├── Directory.Build.targets        # shared validation targets
├── Directory.Packages.props       # central package versions (.NET)
├── pnpm-workspace.yaml            # workspace + catalog: (JavaScript)
├── tsconfig.base.json             # shared TypeScript settings
└── global.json                    # pinned .NET SDK
```

## Contributing

Issues and pull requests are welcome.

## License

MIT
