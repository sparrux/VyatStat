# 02. Identity Server Frontend Requirements

## Description

Angular SPA (`Identity.Web`) for the Identity service: registration, OAuth login, personal account, and users administration dashboard.

**Project root:** `src/Frontend/identity-app`

**Auth server URL (dev):** `https://localhost:7019` — configured in `src/environments/environment.ts`

**OAuth client:** `angular-client` (public, PKCE S256)

**Scopes:** `openid profile offline_access`

## Project structure

```
src/
├── environments/
│   └── environment.ts          # authServerUrl, clientId
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
- Login via HTML form POST to `/connect/authorize` (Authorization Code + PKCE)
- OAuth callback at `/callback`: exchange `code` for tokens, store in `localStorage`, redirect to `/account`
  - Redirects to `/login` when `code` is missing or token exchange fails
- Automatic access token refresh:
  - Proactive refresh ~50 s before JWT expiry
  - Refresh on tab visibility when token is near expiry or missing
  - HTTP interceptor retries API calls once after `401` with refreshed token
  - Redirect to `/login` when refresh fails
- Client-side logout (clears tokens from `localStorage`)
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
- Page size for users list: 10 (backend allows up to 30 per request)
- Domain types in `models/`; runtime config in `environment.ts`

## Routes

All page components are lazy-loaded. Guards and interceptors are eager.

| Path | Component | Guards | Description |
|------|-----------|--------|-------------|
| `/` | — | — | Redirects to `/account` |
| `/login` | `LoginPageComponent` | — | Sign in |
| `/register` | `RegisterPageComponent` | — | Create account |
| `/callback` | `CallbackPageComponent` | — | OAuth redirect handler |
| `/account` | `AccountPageComponent` | `authGuard` | Personal account |
| `/dashboard` | `DashboardPageComponent` | `authGuard`, `readUsersGuard` | Users administration |
| `/**` | — | — | Redirects to `/account` |

## Pages

### Login Page

Design: `docs/design/login/login-page.png`

- Form: login, password
- Submits credentials through OAuth authorize endpoint (form POST with PKCE)
- Link to registration page
- Validation and error messages for empty fields / failed login

### Register Page

Design: `docs/design/register/register-page.png`

- Form: login, password, repeat password
- Client-side check that passwords match
- Calls `POST /register`, navigates to `/login` on success

### OAuth Callback Page

- Reads `code` from query params
- Exchanges code via `AuthService.exchangeCodeForToken`, persists tokens, redirects to `/account`
- Redirects to `/login` when `code` is absent or exchange fails

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
| `CallbackPageComponent` | OAuth callback: code exchange and redirect |
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

Reads `authServerUrl` and `clientId` from `environment`.

| Method | Backend / behavior |
|--------|---------------------|
| `register` | `POST /register` |
| `login` | Form POST `/connect/authorize` |
| `exchangeCodeForToken` | `POST /connect/token` (authorization_code) |
| `refreshAccessTokenSilently` | `POST /connect/token` (refresh_token) |
| `getProfile` | `GET /me` |
| `getUserPermissions` | `GET /users/{id}/permissions` |
| `updatePassword` | `PUT /me/password` |
| `logout` | Clears local tokens only |
| `isAuthenticated` | Checks `access_token` or `refresh_token` in storage |
| `ensureAccessTokenIfNeeded` | Refreshes access token when missing but refresh token exists |
| `getAuthServerUrl` | Returns configured auth server base URL |
| `applyOAuthTokens` | Persists tokens and schedules proactive refresh |
| `invalidateSessionAndRedirectToLogin` | Clears tokens and navigates to `/login` |
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

- **`authGuard`:** allows route if `isAuthenticated()`, otherwise returns `UrlTree` to `/login`
- **`readUsersGuard`:** loads current user permissions; allows `/dashboard` only if `readUsers`, otherwise redirects to `/account` or `/login`
- **`authInterceptor`:** on `401` from Identity API (except OAuth and register), refreshes token and retries once; on failure calls `invalidateSessionAndRedirectToLogin()`
