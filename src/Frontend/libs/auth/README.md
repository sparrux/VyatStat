# VyatkaTracker Auth

Shared OAuth authentication library for Identity and Tracker frontends.

Built and consumed from the **Vyatka frontend workspace** (`src/Frontend`).

## Contents

- `AuthService` — PKCE OAuth flow, token refresh, profile API
- `authGuard`, `authServerInterceptor`, `resourceApiInterceptor`
- `handleOAuthCallback` — shared OAuth callback handler
- Auth models and `provideAuthConfig`

## Usage

From `src/Frontend`:

```bash
npm install
npm run build
```

Apps import via path mapping (`@vyatka-tracker/auth` → `libs/auth/src/public-api.ts`).

To publish the library package separately:

```bash
npm run build:libs
```
