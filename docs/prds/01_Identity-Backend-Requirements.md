# 01. Identity Server Backend Requirements

## Description

This is the single point of access to all company services. Manage and administer user sessions, tokens, and system users

## Functional Requirements

- Login to System using login and password
- User logout
- OAuth 2.0, Single Sign-On
- No relogin required between services
- Create, delete, block users with administrator rights
- Manage user access rights
- View system user information
- Manage user roles in the system
- Manage tokens and sessions: : Access Token, Refresh Token, ID Token
- OpenID Connect, Refresh Token Flow, Client Credentials Flow, Authorization Code Flow

## Non-Functional Requirements


## Endpoints

### OAuth 2.0

Default endpoints from library OpenIddict in .NET.
After authorization you will back to `/callback` endpoint with tokens.

### Account Controller

#### Registration

```http request
POST /register HTTP/2
Content-Type: application/json

{
    "login": "your-login",
    "password": "your-password"
}
```

#### Get your own account info

Authorization Required

```http request
GET /me HTTP/2
```

#### Get user permissions

Authorization Required.

You need advanced access to know about other users' access rights (except yourself).

```http request
GET /users/:guid/permissions HTTP/2
```

#### Change user permissions

Authorization Required.

You need advanced access to change users permissions.

If you do not want to change the current access rights settings, leave field with the corresponding access right and null value.

```http request
POST /users/:guid/permissions HTTP/2
Content-Type: application/json

{
    "readUsers": bool,
    "updateUserPermissions": bool
}
```