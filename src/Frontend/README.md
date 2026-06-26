# Vyatka Frontend

Single Angular workspace for all VyatkaTracker SPAs and shared libraries.

## Projects

| Project | Path | Description |
|---------|------|-------------|
| `identity-app` | `identity-app/` | Identity / IdP UI |
| `tracker-app` | `tracker-app/` | Tracker UI |
| `auth` | `libs/auth/` | `@vyatka-tracker/auth` |
| `ui` | `vyatka-tracker-ui/` | `@vyatka-tracker/ui` |

## Setup

```bash
cd src/Frontend
npm install
```

## Build everything

```bash
npm run build
```

Builds both applications. Shared libraries are compiled from source via TypeScript path mapping — no manual build order.

To also produce publishable library packages:

```bash
npm run build:all
```

## Run locally

From `src/Frontend` (single Angular CLI and `node_modules`):

```bash
npm run start:identity   # port 4200
npm run start:tracker      # port 4201
```

## Aspire AppHost

`AppHost` runs both SPAs from this directory via `npm run start:identity` and `npm run start:tracker`.
Aspire performs `npm install` here automatically on first run.

## Add a new app

1. Generate or copy an app under `src/Frontend/<app-name>/`
2. Register it in `angular.json`
3. Extend `tsconfig.base.json` paths if needed
4. Add scripts to root `package.json`
