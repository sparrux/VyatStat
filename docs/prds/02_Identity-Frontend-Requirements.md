# 02. Identity Server Frontend Requirements

## Description

Angular SPA (`Identity.Web`) for the Identity service: registration, OAuth login, personal account, and users administration dashboard. Acts as the IdP login UI for all OAuth clients (including tracker-app).

**Project root:** `src/Frontend/identity-app`

**Auth server URL (dev):** `https://localhost:7019` — configured in `src/environments/environment.ts`

**OAuth client:** `identity-app` (public, PKCE S256)

**API audience:** `vyatka-identity-api`

**Scopes:** `openid profile offline_access`

## Project structure

```
src/
├── environments/
│   └── environment.ts          # authServerUrl, clientId, apiAudience
└── app/
    ├── app.component.ts        # Root shell (header + outlet + footer)
    ├── app.config.ts
    ├── app.routes.ts           # Lazy-loaded page routes
    ├── components/             # Reusable UI (header, footer, dialogs, table)
    ├── pages/                  # Route-level page components
    ├── services/               # *.service.ts
    ├── models/                 # Shared TypeScript interfaces
    ├── guards/
    ├── interceptors/
    └── utils/                  # jwt-exp, display.utils
```

**Naming conventions:** kebab-case files; `*.component.ts` for components, `*.service.ts` for services; standalone components with `inject()` for DI.

## Functional Requirements

### Implemented

- User registration via `POST /register`, redirect to login on success
- Login via `POST /account/login` (IdP cookie) followed by `GET /connect/authorize` (Authorization Code + PKCE + `state`)
- External OAuth support: when opened with `?returnUrl=` (authorize URL from another client), checks IdP cookie via `GET /account/session` before showing login form
- OAuth callback at `/callback`: validates `state`, exchanges `code` for tokens, stores in `localStorage`, redirect to `/account`
  - Restarts authorization flow when `code` or `state` is missing/invalid or provider returns `error`
- Automatic access token refresh:
  - Proactive refresh ~50 s before JWT expiry
  - Refresh on tab visibility when token is near expiry or missing
  - HTTP interceptor retries API calls once after `401` with refreshed token
  - Restarts OAuth flow when refresh fails
- Logout: clears local tokens and IdP cookie via `POST /account/logout`
- Protected routes with `authGuard` and permission-based `readUsersGuard`
- Account page: profile, role, permissions summary, change password dialog
- Users dashboard: paginated table, permission editing dialog, block/unblock actions with confirm dialog
- App shell header on `/account` and `/dashboard` (avatar, navigation, logout)
- App footer on all pages (brand, placeholder nav links, social links)

### Not implemented (UI placeholders)

- Change email
- Upload profile photo
- Display `createdAt`
- User details / “More info” page

## Non-Functional Requirements

- Standalone Angular components, signals for local state
- Lazy-loaded routes via `loadComponent`
- Angular CDK Dialog for modals
- Shared design tokens and styles from `@vyatka-tracker/ui`
- Tokens in `localStorage`: `access_token`, `refresh_token`, `code_verifier`
- OAuth `state` in `sessionStorage` (one-time use, CSRF protection on callback)
- Page size for users list: 10 (backend allows up to 30 per request)
- Domain types in `models/`; runtime config in `environment.ts`

## Routes

All page components are lazy-loaded. Guards and interceptors are eager.

| Path | Component | Guards | Description |
|------|-----------|--------|-------------|
| `/` | — | — | Redirects to `/account` |
| `/login` | `LoginPageComponent` | — | Sign in (IdP UI; also serves external OAuth clients via `returnUrl`) |
| `/register` | `RegisterPageComponent` | — | Create account |
| `/callback` | `CallbackPageComponent` | — | OAuth redirect handler |
| `/account` | `AccountPageComponent` | `authGuard` | Personal account |
| `/dashboard` | `DashboardPageComponent` | `authGuard`, `readUsersGuard` | Users administration |
| `/**` | — | — | Redirects to `/account` |

## Pages

### Login Page

Design: `docs/design/login/login-page.png`

- Form: login, password
- Submits via `POST /account/login`, then redirects to OAuth authorize URL (own flow or external `returnUrl`)
- When `?returnUrl=` is present: probes IdP cookie with `GET /account/session`; if valid, redirects to `returnUrl` (SSO for tracker-app and other clients)
- Does not redirect to `/account` based on local tokens when `returnUrl` is present (local tokens ≠ IdP cookie)
- Link to registration page
- Validation and error messages for empty fields / failed login

### Register Page

Design: `docs/design/register/register-page.png`

- Form: login, password, repeat password
- Client-side check that passwords match
- Calls `POST /register`, navigates to `/login` on success

### OAuth Callback Page

- Reads `code` and `state` from query params
- Validates and consumes `state` before token exchange
- Exchanges code via `AuthService.exchangeCodeForToken`, persists tokens, redirects to `/account`
- Restarts authorization flow when `error`, missing `code`, or invalid `state`

### Account Page

Design: `docs/design/account/account-page.png`

- Loads `GET /me` and `GET /users/{id}/permissions` for current user
- Sections: About (login, email; createdAt placeholder), Role, Opportunities (derived from permissions)
- **Change password** dialog → `PUT /me/password`
- Link to Users dashboard when `readUsers` permission is present

### Users Dashboard Page

Design: `docs/design/dashboard/dashaboard-page.png`

- Requires `readUsers` permission (`readUsersGuard`)
- Paginated users table via `GET /users?skip=&take=`
- Columns: user info, role, status (Active / Blocked), actions
- **Change access** (if `updateUserPermissions`): opens permissions dialog → `POST /users/{id}/permissions`
- **Block / Unblock** (if `lockOutUsers`): confirm dialog, then `PUT /users/{id}/lock?lockout=`

## Components and dialogs

| Component | Purpose |
|-----------|---------|
| `AppComponent` | Root shell: header, router outlet, footer |
| `AppShellHeaderComponent` | Header with avatar, account/dashboard links, logout (visible on `/account`, `/dashboard`) |
| `AppFooterComponent` | Site footer with brand, nav placeholders, social links |
| `CallbackPageComponent` | OAuth callback: state validation, code exchange and redirect |
| `UsersTableComponent` | Paginated users table with action buttons |
| `ChangePasswordDialogComponent` | Current + new password; min length 6 |
| `UserPermissionsDialogComponent` | Toggle `readUsers`, `updateUserPermissions`, `lockOutUsers` (with dependency rules in UI) |
| `ConfirmDialogComponent` | Yes/no confirmation (block/unblock user) |
| `MessageDialogComponent` | Generic success/error messages |
| `DialogShellComponent` | Shared dialog layout |

## Models

| File | Types |
|------|-------|
| `auth.model.ts` | `UserProfile`, `UserClaims`, `OAuthTokenResponse`, `UpdatePasswordRequest` |
| `user.model.ts` | `DashboardUser`, `UsersListResponse`, `UpdateUserPermissionsRequest` |
| `dialog.model.ts` | `MessageDialogData`, `ConfirmDialogData`, `UserPermissionsDialogData`, `UserPermissionsDialogResult` |

## Utils

| File | Purpose |
|------|---------|
| `jwt-exp.ts` | Parse JWT `exp` claim for proactive refresh scheduling |
| `display.utils.ts` | `displayInitials`, `displayOrNull` for profile display |

## Services

### `AuthService` (`auth.service.ts`)

Reads `authServerUrl`, `clientId`, and `apiAudience` from `environment`.

| Method | Backend / behavior |
|--------|---------------------|
| `register` | `POST /register` |
| `login` | `POST /account/login` (cookie), then redirect to authorize URL |
| `hasIdpCookieSession` | `GET /account/session` |
| `isValidAuthorizeReturnUrl` | Client-side check that `returnUrl` targets `/connect/authorize` on auth server |
| `startAuthorizationFlow` | `GET /connect/authorize` with PKCE + `state` |
| `buildAuthorizeReturnUrl` | Builds authorize URL; stores `code_verifier` and `state` |
| `validateAndConsumeOAuthState` | One-time `state` validation on callback |
| `exchangeCodeForToken` | `POST /connect/token` (authorization_code) |
| `refreshAccessTokenSilently` | `POST /connect/token` (refresh_token) |
| `getProfile` | `GET /me` |
| `getUserPermissions` | `GET /users/{id}/permissions` |
| `updatePassword` | `PUT /me/password` |
| `logout` | Clears local tokens; `POST /account/logout` (IdP cookie) |
| `isAuthenticated` | Checks `access_token` or `refresh_token` in storage |
| `ensureAccessTokenIfNeeded` | Refreshes access token when missing but refresh token exists |
| `getAuthServerUrl` | Returns configured auth server base URL |
| `applyOAuthTokens` | Persists tokens and schedules proactive refresh |
| `invalidateSessionAndRedirectToLogin` | Clears tokens and restarts OAuth flow |
| `onAppBootstrap` | Starts proactive refresh and visibility listener |

### `UsersService` (`users.service.ts`)

Uses `AuthService.ensureAccessTokenIfNeeded()` before authenticated requests.

| Method | Backend |
|--------|---------|
| `getUsers` | `GET /users` |
| `updateUserPermissions` | `POST /users/{id}/permissions` |
| `setUserLockOut` | `PUT /users/{id}/lock` |

### `DialogService` (`dialog.service.ts`)

| Method | Opens |
|--------|-------|
| `open` | Generic CDK dialog wrapper |
| `openMessage` | Success/error message dialog |
| `openConfirm` | Yes/no confirm dialog; returns `Promise<boolean>` |
| `openUserPermissions` | User permissions editor |
| `openChangePassword` | Change password form |

## Guards and interceptors

- **`authGuard`:** allows route if `isAuthenticated()`, otherwise starts OAuth authorization flow
- **`readUsersGuard`:** loads current user permissions; allows `/dashboard` only if `readUsers`, otherwise redirects to `/account` or starts OAuth flow
- **`authInterceptor`:** on `401` from Identity API (except OAuth, register, account login/logout/session), refreshes token and retries once; on failure restarts OAuth flow
