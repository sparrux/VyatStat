# 01. Identity Server Backend Requirements

## Description

Single point of access for company services. Manages user accounts, OAuth 2.0 / OpenID Connect tokens, and permission-based authorization for the Identity API and client applications.

**Stack:** ASP.NET Core, ASP.NET Identity, OpenIddict, Entity Framework Core, PostgreSQL.

**Dev URL:** `https://localhost:7019` (see `launchSettings.json`). Runtime `Idp:Authority` and client URLs are injected by AppHost (or environment variables in production), not hardcoded in base `appsettings.json`.

## Functional Requirements

### Implemented

- IdP cookie session (`Vyatka.IdP.Session`) for SSO across OAuth clients
- Sign in with login and password via `POST /account/login` (sets IdP cookie), then OAuth Authorization Code flow via `GET /connect/authorize`
- OAuth 2.0 / OpenID Connect token issuance (Access Token, Refresh Token, ID Token)
- Authorization Code flow with PKCE (public SPA clients)
- Refresh Token flow
- Client Credentials flow (machine-to-machine clients)
- Multiple OAuth SPA clients with per-client API audience (`vyatka-identity-api`, `vyatka-tracker-api`)
- User registration (anonymous)
- View own profile and permissions
- Change own password (invalidates existing tokens via security stamp)
- List users with pagination (requires `read_users` permission)
- View a single user by id (requires `read_users` permission)
- View user permissions (self or `read_users` permission)
- Update user permissions (requires `update_user_permissions` permission; partial updates via nullable fields)
- Block / unblock users (requires `lock_out_users` permission; cannot lock own account)
- Role and permission model stored as ASP.NET Identity claims
- Token invalidation when permissions, password, or lockout state change (security stamp middleware)
- Database seeding: default admin user and OAuth clients on startup
- OpenAPI + Scalar API reference in Development

### Not implemented

- User deletion
- Server-side logout / token revocation endpoint
- Email management
- User avatar / profile photo
- `createdAt` in user responses

## Authorization Model

Permissions are stored as Identity claims and enforced via ASP.NET authorization policies.

| Policy | Claim type | Claim value | Description |
|--------|------------|-------------|-------------|
| `admin` | `id.user.role` | `id.user.role.admin` | Administrator role (included in tokens; not a separate API policy on all endpoints) |
| `read_users` | `id.user.permission` | `id.user.permission.read_users` | View users list and user details |
| `update_user_permissions` | `id.user.permission` | `id.user.permission.update_user_permissions` | Change other users' permissions |
| `lock_out_users` | `id.user.permission` | `id.user.permission.lock_out_users` | Block / unblock other users |

**Default seeded user (Development):** login `primary`, password `asd1234`, administrator with all permissions.

## Configuration

### OAuth clients (`Clients` section)

| Config key | Client ID | Audience | Redirect URI |
|------------|-----------|----------|--------------|
| `identity-app` | `identity-app` | `vyatka-identity-api` | `{identity-app.Url}/callback` |
| `tracker-app` | `tracker-app` | `vyatka-tracker-api` | `{tracker-app.Url}/callback` |

Client `Url`, `Idp:Authority`, and `Idp:LoginPageUrl` are supplied at runtime (Aspire AppHost or deployment env vars). Base `appsettings.json` keeps empty URLs.

### IdP options (`Idp` section)

| Key | Description |
|-----|-------------|
| `Authority` | Public base URL of Identity Server (validates authorize `returnUrl`) |
| `LoginPageUrl` | Absolute URL of identity-app login page (e.g. `{identity-app.Url}/login`) |

## Non-Functional Requirements

- **Database:** PostgreSQL (`ConnectionStrings:IdentityDb`)
- **Password policy:** minimum length 6; digit, uppercase, and special character not required
- **Access token lifetime:** 5 minutes
- **Refresh token lifetime:** 30 days
- **CORS:** origins from all configured `Clients:*:Url` with `AllowCredentials`
- **OAuth clients:** public SPA clients with PKCE S256; no client secret
- **Authenticated API requests:** Bearer access token (OpenIddict validation)
- **Stale token handling:** middleware compares `id.user.security_stamp` claim with current user stamp; returns `401` with `X-Token-Stale: 1` on mismatch or lockout

## Endpoints

All JSON endpoints return FluentResults-based HTTP responses unless noted otherwise.

### Account (`AccountController`)

#### Check IdP session

```http
GET /account/session
```

**Public.** Validates IdP application cookie (not Bearer token).

Returns `200` when cookie session is valid and user is not locked out; `401` otherwise.

Used by identity-app login page to continue external OAuth flows when SSO cookie is present.

#### Login (IdP cookie)

```http
POST /account/login
Content-Type: application/json
```

**Public.**

**Request:**

```json
{
  "login": "your-login",
  "password": "your-password"
}
```

Optional query: `returnUrl` — must be a valid authorize URL on `Idp:Authority` (`/connect/authorize`).

On success, sets `Vyatka.IdP.Session` cookie. If `returnUrl` is valid, redirects to it; otherwise returns `200`.

#### Logout (IdP cookie)

```http
POST /account/logout
GET /account/logout
```

**Public.** Clears IdP cookie. Optional `returnUrl` redirect when it matches a registered client origin.

### OAuth 2.0 / OpenID Connect (OpenIddict)

Implemented in `AuthorizationController`. Content type for token endpoint: `application/x-www-form-urlencoded`.

#### Authorize

```http
GET /connect/authorize
```

**Public.** Browser redirect endpoint.

If IdP cookie is present → issues authorization code and redirects to `redirect_uri`.
If no cookie → redirects to `Idp:LoginPageUrl?returnUrl={this authorize URL}`.

Supported `response_type`: `code` only.

Typical parameters for the identity web client:

| Parameter | Value |
|-----------|-------|
| `client_id` | `identity-app` |
| `response_type` | `code` |
| `scope` | `openid profile offline_access` |
| `redirect_uri` | `{identity-app.Url}/callback` |
| `state` | CSRF token (validated by SPA on callback) |
| `code_challenge` | PKCE challenge (S256) |
| `code_challenge_method` | `S256` |

For tracker-app use `client_id=tracker-app` and `redirect_uri={tracker-app.Url}/callback`; token exchange uses `aud=vyatka-tracker-api`.

On success, redirects to `redirect_uri` with `?code=...&state=...`. On failure, returns OAuth error or redirects to login.

> **Note:** Password is not accepted on `/connect/authorize`. Credentials are submitted only via `POST /account/login`.

#### Token

```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded
Accept: application/json
```

**Public.**

**Authorization Code exchange** (`grant_type=authorization_code`):

| Parameter | Description |
|-----------|-------------|
| `client_id` | `identity-app` or `tracker-app` |
| `code` | Authorization code from callback |
| `redirect_uri` | Same as authorize request |
| `code_verifier` | PKCE verifier |
| `aud` | `vyatka-identity-api` (identity-app) or `vyatka-tracker-api` (tracker-app) |

**Refresh Token** (`grant_type=refresh_token`):

| Parameter | Description |
|-----------|-------------|
| `client_id` | Same client that issued the refresh token |
| `refresh_token` | Valid refresh token |
| `scope` | `openid profile offline_access` |
| `aud` | Target API audience |

Locked-out users cannot refresh tokens.

**Client Credentials** (`grant_type=client_credentials`):

| Parameter | Description |
|-----------|-------------|
| `client_id` | Registered client id |
| `client_secret` | Client secret (confidential clients) |

Returns JSON with `access_token`, optional `refresh_token`, `expires_in`, etc.

After authorization the SPA lands on `/callback` and exchanges the code for tokens.

### Users API (`UsersController`)

Base path: `/`. All endpoints except `/register` require `Authorization: Bearer {access_token}`.

#### Registration

```http
POST /register
Content-Type: application/json
```

**Authorization:** not required.

**Request:**

```json
{
  "login": "your-login",
  "password": "your-password"
}
```

**Response:** `UserResponse` (new user without claims, not locked out).

#### Get own account

```http
GET /me
```

**Response:** `UserResponse`

#### Change own password

```http
PUT /me/password
Content-Type: application/json
```

**Request:**

```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```

Updates security stamp; existing access tokens become invalid.

#### Get users (paginated)

```http
GET /users?take={int}&skip={int}
```

**Authorization:** `read_users` policy.

Query defaults and limits: `take` clamped to 1–30 (default behavior if invalid: min 1, max 30), `skip` ≥ 0.

**Response:** `UsersResponse`

```json
{
  "users": [ /* UserResponse[] */ ],
  "total": 0
}
```

#### Get user by id

```http
GET /users/{userId}
```

**Authorization:** `read_users` policy.

**Response:** `UserResponse`

#### Get user permissions

```http
GET /users/{userId}/permissions
```

**Authorization:** required. Allowed if requesting own permissions or caller has `read_users`.

**Response:** `UserClaimsResponse`

```json
{
  "isAdmin": false,
  "readUsers": false,
  "updateUserPermissions": false,
  "lockOutUsers": false
}
```

#### Update user permissions

```http
POST /users/{userId}/permissions
Content-Type: application/json
```

**Authorization:** `update_user_permissions` policy.

Only fields that should change need to be sent; omitted or `null` fields are left unchanged.

**Request:**

```json
{
  "readUsers": true,
  "updateUserPermissions": false,
  "lockOutUsers": null
}
```

**Response:** updated `UserClaimsResponse`.

Changing permissions updates the user's security stamp.

#### Block / unblock user

```http
PUT /users/{userId}/lock?lockout={bool}
```

**Authorization:** `lock_out_users` policy.

`lockout=true` enables lockout and sets lockout end far in the future. `lockout=false` clears lockout.

Cannot change lockout for the authenticated user's own account (`400 Bad Request`).

Updates security stamp on success.

### Shared response models

**`UserResponse`**

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "userName": "string | null",
  "email": "string | null",
  "claims": { /* UserClaimsResponse | null */ },
  "isLockedOut": false
}
```

**`UserClaimsResponse`**

```json
{
  "isAdmin": false,
  "readUsers": false,
  "updateUserPermissions": false,
  "lockOutUsers": false
}
```
