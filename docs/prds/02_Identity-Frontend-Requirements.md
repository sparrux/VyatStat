# 02. Identity Server Frontend Requirements

## Description

Angular SPA (`Identity.Web`) for the Identity service: registration, OAuth login, personal account, and users administration dashboard.

**Auth server URL (dev):** `https://localhost:7019`

**OAuth client:** `angular-client` (public, PKCE S256)

**Scopes:** `openid profile offline_access`

## Functional Requirements

### Implemented

- User registration via `POST /register`, redirect to login on success
- Login via HTML form POST to `/connect/authorize` (Authorization Code + PKCE)
- OAuth callback at `/callback`: exchange `code` for tokens, store in `localStorage`, redirect to `/account`
- Automatic access token refresh:
  - Proactive refresh ~50 s before JWT expiry
  - Refresh on tab visibility when token is near expiry or missing
  - HTTP interceptor retries API calls once after `401` with refreshed token
  - Redirect to `/login` when refresh fails
- Client-side logout (clears tokens from `localStorage`)
- Protected routes with `authGuard` and permission-based `readUsersGuard`
- Account page: profile, role, permissions summary, change password dialog
- Users dashboard: paginated table, permission editing dialog, block/unblock actions
- App shell header on `/account` and `/dashboard` (avatar, navigation, logout)

### Not implemented (UI placeholders)

- Change email
- Upload profile photo
- Display `createdAt`
- User details / “More info” page

## Non-Functional Requirements

- Standalone Angular components, signals for local state
- Angular CDK Dialog for modals
- Tokens in `localStorage`: `access_token`, `refresh_token`, `code_verifier`
- Page size for users list: 10 (backend allows up to 30 per request)

## Routes

| Path | Component | Guards | Description |
|------|-----------|--------|-------------|
| `/` | — | — | Redirects to `/account` |
| `/login` | LoginPageComponent | — | Sign in |
| `/register` | RegisterPageComponent | — | Create account |
| `/callback` | CallbackComponent | — | OAuth redirect handler |
| `/account` | AccountPageComponent | `authGuard` | Personal account |
| `/dashboard` | DashboardPageComponent | `authGuard`, `readUsersGuard` | Users administration |
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

### Account Page

Design: `docs/design/account/account-page.png`

- Loads `GET /me` and `GET /users/{id}/permissions` for current user
- Sections: About (login; email/createdAt placeholders), Role, Opportunities (derived from permissions)
- **Change password** dialog → `PUT /me/password`
- Link to Users dashboard when `readUsers` permission is present

### Users Dashboard Page

Design: `docs/design/dashboard/dashaboard-page.png`

- Requires `readUsers` permission (`readUsersGuard`)
- Paginated users table via `GET /users?skip=&take=`
- Columns: user info, role, status (Active / Blocked), actions
- **Change access** (if `updateUserPermissions`): opens permissions dialog → `POST /users/{id}/permissions`
- **Block / Unblock** (if `lockOutUsers`): confirmation, then `PUT /users/{id}/lock?lockout=`

## Components and dialogs

| Component | Purpose |
|-----------|---------|
| `AppShellHeaderComponent` | Header with avatar, account/dashboard links, logout |
| `CallbackComponent` | Exchanges authorization code for tokens |
| `UsersTableComponent` | Paginated users table with action buttons |
| `ChangePasswordDialogComponent` | Current + new password; min length 6 |
| `UserPermissionsDialogComponent` | Toggle `readUsers`, `updateUserPermissions`, `lockOutUsers` (with dependency rules in UI) |
| `MessageDialogComponent` | Generic success/error messages |
| `DialogShellComponent` | Shared dialog layout |

## Services

### `AuthService`

| Method | Backend |
|--------|---------|
| `register` | `POST /register` |
| `login` | Form POST `/connect/authorize` |
| `exchangeCodeForToken` | `POST /connect/token` (authorization_code) |
| `refreshAccessTokenSilently` | `POST /connect/token` (refresh_token) |
| `getProfile` | `GET /me` |
| `getUserPermissions` | `GET /users/{id}/permissions` |
| `updatePassword` | `PUT /me/password` |
| `logout` | Clears local tokens only |
| `isAuthenticated` | Checks `access_token` or `refresh_token` in storage |
| `onAppBootstrap` | Starts proactive refresh and visibility listener |

### `UsersService`

| Method | Backend |
|--------|---------|
| `getUsers` | `GET /users` |
| `updateUserPermissions` | `POST /users/{id}/permissions` |
| `setUserLockOut` | `PUT /users/{id}/lock` |

### `DialogService`

Opens change-password, user-permissions, and message dialogs.

## Guards and interceptors

- **`authGuard`:** allows route if `isAuthenticated()`, otherwise `/login`
- **`readUsersGuard`:** loads current user permissions; allows `/dashboard` only if `readUsers`, otherwise `/account` or `/login`
- **`authInterceptor`:** on `401` from Identity API (except OAuth and register), refreshes token and retries once; on failure calls `invalidateSessionAndRedirectToLogin()`
