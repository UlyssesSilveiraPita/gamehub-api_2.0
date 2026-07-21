# 📘 Architecture Decision Log (ADR)

## Purpose

This document records the most important architectural and technical decisions made during the development of GameHub API 2.0.

Each Architecture Decision Record (ADR) contains:

- Context
- Decision
- Rationale
- Consequences

The purpose of this document is to document the project's evolution, facilitate maintenance, and preserve the reasoning behind architectural decisions.

---

# ADR-001

## Title

Evolution from GameHub API 1.0 to GameHub API 2.0

### Status

Accepted

### Date

July 2026

### Context

GameHub API 1.0 already provided a functional backend including JWT authentication, player management, achievements, save games and leaderboards.

The second phase of the project required a more robust architecture, automated testing, commercial features, observability and deployment infrastructure.

### Decision

Instead of starting from scratch, the existing project would evolve incrementally into GameHub API 2.0.

### Rationale

Real-world software evolves continuously. Building upon an existing codebase better reflects professional software development.

### Consequences

The application will continue to evolve incrementally while maintaining backward compatibility whenever possible.

---

# ADR-002

## Title

Adoption of the GameHub Domain

### Status

Accepted

### Date

July 2026

### Context

The original educational project was based on courses, students and enrollments.

### Decision

Adapt the business domain to represent a digital gaming platform.

### Rationale

A game-oriented domain aligns with the author's professional goals and produces a stronger portfolio.

### Consequences

Future features will follow the GameHub domain language rather than the original educational terminology.

---

# ADR-003

## Title

Blazor as the Official Frontend Technology

### Status

Accepted

### Date

July 2026

### Context

The project initially consisted only of a Web API.

### Decision

Use Blazor as the official frontend technology.

### Rationale

Blazor allows the entire application to remain within the .NET ecosystem while sharing knowledge, tooling and architectural concepts.

### Consequences

The project will evolve into a complete full-stack .NET application.

---

# ADR-004

## Title

Incremental Clean Architecture Migration

### Status

Accepted

### Date

July 2026

### Context

Migrating directly to a full Clean Architecture would introduce unnecessary risk and complexity.

### Decision

Adopt an incremental migration strategy.

### Rationale

Small, validated improvements reduce risk and preserve application stability.

### Consequences

The solution will progressively evolve into separate Application, Domain and Infrastructure projects.

---

# ADR-005

## Title

Centralized Current User Abstraction

### Status

Accepted

### Date

July 2026

### Context

Controllers were directly reading JWT claims through HttpContext.

### Decision

Introduce the ICurrentUser abstraction implemented by CurrentUserService.

### Rationale

Centralizes authentication concerns and improves testability.

### Consequences

Controllers no longer depend directly on authentication implementation details.

---

# ADR-006

## Title

Adoption of the Result Pattern

### Status

Accepted

### Date

July 2026

### Context

Business services were using exceptions for expected validation failures.

### Decision

Introduce Result, Result<T> and Error classes.

### Rationale

Business failures should be represented explicitly instead of relying on exceptions.

### Consequences

Services return Result<T>, controllers interpret business outcomes, and exceptions remain reserved for unexpected failures.

---

# ADR-007

## Title

Custom Validation Infrastructure

### Status

Accepted

### Date

July 2026

### Context

Validation logic was duplicated across controllers.

### Decision

Create a reusable validation infrastructure based on IValidator<T>.

### Rationale

Separate input validation from business logic while improving code reuse.

### Consequences

Controllers remain lightweight and validation becomes reusable across the application.

---

# ADR-008

## Title

Standardized API Responses

### Status

Accepted

### Date

July 2026

### Context

Controllers returned inconsistent HTTP error responses.

### Decision

Create standardized response models and reusable Controller Extensions.

### Rationale

Provide a consistent API contract and simplify controller implementations.

### Consequences

All controllers follow the same response format.

---

# ADR-009

## Title

Centralized Entity Mapping

### Status

Accepted

### Date

July 2026

### Context

Entity-to-DTO mapping was duplicated across multiple endpoints.

### Decision

Move mappings into dedicated Extension Methods.

### Rationale

Reduce duplication and improve maintainability.

### Consequences

Controllers remain focused on request handling rather than object transformation.

---

# ADR-010

## Title

Test-Driven Backend Evolution

### Status

Accepted

### Date

July 2026

### Context

The backend architecture became increasingly complex as new features were introduced.

### Decision

Introduce automated unit testing before continuing with infrastructure evolution.

### Rationale

Automated tests provide confidence for future refactoring and architectural improvements.

### Consequences

The project currently includes 34 automated unit tests, with additional integration tests planned.

---

# ADR-011

## Title

Documentation as a First-Class Artifact

### Status

Accepted

### Date

July 2026

### Context

As the project grew, architectural knowledge became increasingly difficult to preserve.

### Decision

Maintain dedicated technical documentation throughout development.

### Rationale

Good documentation improves maintainability, onboarding and long-term project quality.

### Consequences

Architecture, domain model, permissions, backlog and ADRs evolve together with the source code.

---

# Future ADRs

The following architectural decisions are expected during future development:

- Structured Logging Strategy
- Observability and Health Checks
- Docker Containerization
- CI/CD Pipeline
- Clean Architecture Separation
- Production Deployment Strategy
- API Versioning
- Caching Strategy
- Background Processing
- Event-Driven Integrations

---

# Current ADR Status

| ADR | Status |
|------|--------|
| ADR-001 | ✅ Accepted |
| ADR-002 | ✅ Accepted |
| ADR-003 | ✅ Accepted |
| ADR-004 | ✅ Accepted |
| ADR-005 | ✅ Accepted |
| ADR-006 | ✅ Accepted |
| ADR-007 | ✅ Accepted |
| ADR-008 | ✅ Accepted |
| ADR-009 | ✅ Accepted |
| ADR-010 | ✅ Accepted |
| ADR-011 | ✅ Accepted |