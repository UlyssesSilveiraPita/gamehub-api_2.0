# 🏛️ Architecture — GameHub API 2.0

## Purpose

This document describes the current architecture of the GameHub API 2.0 project, the architectural principles adopted during development, and the planned evolution of the solution.

The objective is to keep the project organized, maintainable, scalable, and easy to evolve while applying modern .NET development practices.

---

# Current Architecture

GameHub API 2.0 currently follows a layered architecture based on ASP.NET Core.

```text
GameHub.API
│
├── Controllers
├── Entities
├── DTOs
├── Services
├── Validations
├── Mappings
├── Data
├── Configurations
├── Middleware
├── Extensions
├── Common
│   ├── Result Pattern
│   ├── Errors
│   └── Responses
│
└── Infrastructure
```

Business logic is isolated from controllers, validation responsibilities are centralized, and common behaviors are shared through reusable infrastructure components.

---

# Architectural Principles

The project follows the following principles:

- Separation of Concerns
- Single Responsibility Principle (SRP)
- Dependency Injection
- Low Coupling
- High Cohesion
- Explicit Business Rules
- Incremental Evolution

Rather than performing a complete rewrite, the architecture evolves gradually while keeping the application functional throughout the process.

---

# Request Flow

```text
HTTP Request
      │
      ▼
Controller
      │
      ▼
Validator
      │
      ▼
Application Service
      │
      ▼
Entity / Business Rules
      │
      ▼
Result<T>
      │
      ▼
Controller Extensions
      │
      ▼
HTTP Response
```

Unexpected exceptions are handled globally by the Exception Middleware.

---

# Result Pattern

The application uses a custom Result Pattern to represent business outcomes without relying on exceptions.

Benefits:

- Explicit success/failure flow
- Predictable business logic
- Easier unit testing
- Reduced exception usage
- Cleaner controllers

Core classes:

- Result
- Result<T>
- Error

---

# Validation Infrastructure

Input validation is centralized through a custom validation infrastructure.

Components:

- IValidator<T>
- ValidationErrors
- Feature-specific validators

Responsibilities:

- Validate incoming requests
- Prevent invalid data from reaching business logic
- Keep controllers lightweight
- Improve code reuse

---

# Global Exception Middleware

Unexpected exceptions are handled by a centralized middleware.

Responsibilities:

- Capture unhandled exceptions
- Return standardized error responses
- Prevent internal exception details from leaking
- Keep controllers focused on business flow

Business validation errors are intentionally handled through the Result Pattern instead of exceptions.

---

# Current User Abstraction

The authenticated user is accessed through the ICurrentUser abstraction.

Implementation:

- CurrentUserService
- IHttpContextAccessor

Benefits:

- Removes direct Claim access from controllers
- Improves testability
- Reduces authentication coupling
- Centralizes user identity retrieval

---

# DTO Mapping

Entity-to-DTO mapping is centralized using Extension Methods.

Current mapping modules include:

- PurchaseMappings
- PaymentMappings

Future modules:

- PlayerMappings
- GameMappings
- AchievementMappings
- SaveGameMappings

This approach keeps controllers free from repetitive mapping logic.

---

# Observability

The backend includes built-in observability components.

Current implementation:

- Structured Logging
- Request Logging
- Payment Logging
- Health Checks
- Database Health Checks

These components simplify diagnostics and production monitoring while preserving application performance.

---

# Dependency Injection

All application services are registered through the ASP.NET Core dependency injection container.

Examples include:

- Business services
- Validators
- CurrentUserService
- Infrastructure services

Dependency Injection improves modularity and facilitates testing.

---

# Automated Testing

The project currently includes automated unit tests covering the most important infrastructure components and business rules.

Current status:

- ✅ 40 automated unit tests
- ✅ Result Pattern
- ✅ Validation Infrastructure
- ✅ Purchase Services
- ✅ Payment Services
- ✅ Query Extensions
- ✅ DTO Mappings

Test coverage continues to grow as new modules are implemented.

---

# Future Evolution

The current architecture was intentionally designed to support incremental growth without disruptive restructuring.

Future improvements include:

- CI/CD Pipeline
- Blazor Frontend
- Production Deployment
- Background Processing
- Event-Driven Integrations

---

# Architecture Status

| Component | Status |
|-----------|--------|
| Layered Architecture | ✅ |
| Dependency Injection | ✅ |
| Result Pattern | ✅ |
| Validation Infrastructure | ✅ |
| Global Exception Middleware | ✅ |
| CurrentUser Abstraction | ✅ |
| DTO Mapping | ✅ |
| Observability | ✅ |
| Docker Support | ✅ |
| Automated Unit Tests | ✅ (40 Tests) |
| CI/CD | ⏳ Planned |
| Blazor Frontend | ⏳ Planned |
| Production Release | ⏳ Planned |