# 01. Identity Server Backend Requirements

## Description

Single point of access for company services. Manages user accounts, OAuth 2.0 / OpenID Connect tokens, and permission-based authorization for the Identity API and client applications.

**Stack:** ASP.NET Core, ASP.NET Identity, OpenIddict, Entity Framework Core, PostgreSQL.

**Dev URL:** `https://localhost:7019` (see `launchSettings.json`).

## Functional Requirements

### Implemented

- Sign in with login and password via OAuth 2.0 Authorization Code flow (credentials submitted to `/connect/authorize`)
- OAuth 2.0 / OpenID Connect token issuance (Access Token, Refresh Token, ID Token)
- Authorization Code flow with PKCE (public SPA client)
- Refresh Token flow
- Client Credentials flow (machine-to-machine clients)
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
- Database seeding: default admin user and OAuth client on startup
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

## Non-Functional Requirements

- **Database:** PostgreSQL (`ConnectionStrings:vyatka-identity`)
- **Password policy:** minimum length 6; digit, uppercase, and special character not required
- **Access token lifetime:** 5 minutes
- **Refresh token lifetime:** 30 days
- **CORS:** configured from `Clients:WebClient:Url`
- **OAuth client:** public SPA client `angular-client`, redirect URI `{WebClient.Url}/callback`
- **Authenticated API requests:** Bearer access token (OpenIddict validation)
- **Stale token handling:** middleware compares `id.user.security_stamp` claim with current user stamp; returns `401` with `X-Token-Stale: 1` on mismatch or lockout

## Endpoints

All JSON endpoints return FluentResults-based HTTP responses unless noted otherwise.

### OAuth 2.0 / OpenID Connect (OpenIddict)

Implemented in `AuthorizationController`. Content type for OAuth endpoints: `application/x-www-form-urlencoded`.

#### Authorize

```http
GET|POST /connect/authorize
Content-Type: application/x-www-form-urlencoded
```

**Public.** Used by the SPA login form.

Supported `response_type`: `code` only.

Typical parameters for the web client:

| Parameter | Value |
|-----------|-------|
| `client_id` | `angular-client` |
| `response_type` | `code` |
| `scope` | `openid profile offline_access` |
| `redirect_uri` | `{WebClient.Url}/callback` |
| `code_challenge` | PKCE challenge (S256) |
| `code_challenge_method` | `S256` |
| `username` | User login |
| `password` | User password |

On success, redirects to `redirect_uri` with `?code=...`. On failure, returns OAuth error (`invalid_grant`, etc.).

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
| `client_id` | `angular-client` |
| `code` | Authorization code from callback |
| `redirect_uri` | Same as authorize request |
| `code_verifier` | PKCE verifier |
| `aud` | `vyatka-identity-api` (used by SPA) |

**Refresh Token** (`grant_type=refresh_token`):

| Parameter | Description |
|-----------|-------------|
| `client_id` | `angular-client` |
| `refresh_token` | Valid refresh token |
| `scope` | `openid profile offline_access` |
| `aud` | `vyatka-identity-api` |

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
