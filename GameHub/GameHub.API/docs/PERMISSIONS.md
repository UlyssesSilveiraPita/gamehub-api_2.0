# 🔐 Permissions Matrix — GameHub API 2.0

## Purpose

This document defines the authorization model adopted by GameHub API 2.0.

It specifies which resources each user role can access and documents the security rules applied throughout the application.

This document serves as a reference for:

- Authentication
- Authorization
- Role management
- Ownership validation
- API security
- Automated authorization tests

---

# User Roles

## Public

Represents any unauthenticated visitor.

Capabilities:

- Register an account
- Login
- Browse public information
- View public leaderboards
- View public games

---

## User

Represents an authenticated player.

Capabilities:

- Manage personal profile
- Manage owned players
- Manage save games
- Unlock achievements
- Purchase digital products
- Manage own payments
- Access owned resources

Users are never allowed to access resources owned by other users.

---

## Administrator

Represents a system administrator.

Capabilities:

- Full system administration
- Manage users
- Manage products
- View all purchases
- View all payments
- Perform administrative operations
- Access monitoring endpoints

Administrators bypass ownership restrictions while still respecting authentication requirements.

---

# General Security Rules

The following rules apply across the entire application:

- JWT authentication is required for protected endpoints.
- Authentication is validated before authorization.
- Resource ownership is always verified.
- Business rules never rely on client-provided identifiers alone.
- Sensitive information must never be exposed in API responses.
- Administrative operations require the **Admin** role.
- Authorization failures return appropriate HTTP status codes.

---

# Ownership Validation

Ownership is one of the most important security principles in GameHub.

Example:

```text
Authenticated User
UserId = 10

Requested Player
UserId = 10

Result
✔ Access Granted
```

---

```text
Authenticated User
UserId = 10

Requested Player
UserId = 25

Result
❌ Access Denied
```

Controllers never trust IDs received from the client without validating ownership.

---

# Permission Matrix

| Feature | Public | User | Admin |
|---------|:------:|:----:|:-----:|
| Register Account | ✅ | ✅ | ✅ |
| Login | ✅ | ✅ | ✅ |
| View Swagger | ✅ | ✅ | ✅ |
| Authenticate with JWT | ❌ | ✅ | ✅ |
| View Public Games | ✅ | ✅ | ✅ |
| View Game Details | ✅ | ✅ | ✅ |
| View Leaderboards | ✅ | ✅ | ✅ |
| View Own Profile | ❌ | ✅ | ✅ |
| Update Own Profile | ❌ | ✅ | ✅ |
| View Other User Profiles | ❌ | ❌ | ✅ |
| Manage Own Players | ❌ | ✅ | ✅ |
| Manage Own Save Games | ❌ | ✅ | ✅ |
| View Own Achievements | ❌ | ✅ | ✅ |
| Unlock Achievements | ❌ | ✅ | ✅ |
| Create Purchases | ❌ | ✅ | ✅ |
| View Own Purchases | ❌ | ✅ | ✅ |
| View Other User Purchases | ❌ | ❌ | ✅ |
| Create Payments | ❌ | ✅ | ✅ |
| View Own Payments | ❌ | ✅ | ✅ |
| View All Payments | ❌ | ❌ | ✅ |
| Manage Game Products | ❌ | ❌ | ✅ |
| Administrative Operations | ❌ | ❌ | ✅ |
| View Public Health Checks *(future)* | ✅ | ✅ | ✅ |
| View Detailed Health Checks *(future)* | ❌ | ❌ | ✅ |

---

# Authorization Strategy

The project currently combines three authorization mechanisms:

## Authentication

JWT Bearer Tokens identify the authenticated user.

---

## Role-Based Authorization

Administrative endpoints require the **Admin** role.

Example:

- Product management
- Administrative monitoring
- Payment auditing

---

## Resource Ownership

Users may only manipulate resources they own.

Ownership is validated using the authenticated user's identifier obtained from the JWT token through the `ICurrentUser` abstraction.

---

# Current Security Model

GameHub API 2.0 currently adopts a layered authorization model.

The authentication process is handled by ASP.NET Identity and JWT Bearer Authentication.

Authorization is enforced through three complementary mechanisms:

- Authentication (JWT)
- Role-based Authorization
- Resource Ownership Validation

This approach ensures that authenticated users can only access resources they own while administrators maintain elevated privileges where appropriate.

The CurrentUser abstraction centralizes user identity retrieval, reducing coupling with the HTTP layer and improving testability.

This security model is considered stable and provides a solid foundation for future platform growth.

---

# Future Improvements

The current permission model is intentionally simple and maintainable.

Future platform growth may introduce additional authorization scenarios such as:

- Policy-based Authorization
- Permission-based Authorization
- Fine-grained Administrative Roles
- Audit Logging
- Multi-Tenant Support
- External Identity Providers (Google, Microsoft, Steam)

These improvements are not required for the current project scope but can be incorporated without major architectural changes.

---

# Security Principles

GameHub API follows these security principles:

- Authentication before authorization
- Least privilege principle
- Explicit ownership validation
- Centralized user identity access
- Standardized authorization behavior
- Secure default configuration

---

# Permission System Status

| Component | Status |
|-----------|--------|
| Authentication | ✅ |
| JWT Authorization | ✅ |
| Role-based Authorization | ✅ |
| Ownership Validation | ✅ |
| CurrentUser Abstraction | ✅ |
| Protected Controllers | ✅ |
| Commerce Authorization | ✅ |
| Automated Tests | ✅ (40 Tests) |
| Documentation | ✅ |