# 🎮 Domain Model — GameHub API 2.0

## Purpose

This document describes the business domain of GameHub API 2.0, including its core entities, relationships, responsibilities, and business rules.

It serves as the primary reference for:

- Domain modeling
- Entity Framework Core mappings
- Business services
- API endpoints
- Automated tests
- Future Blazor integration

---

# Domain Overview

GameHub is a digital platform that allows users to manage game-related information, digital purchases, achievements, save games, and rankings.

The current domain is divided into three major areas:

- Identity
- Gaming
- Commerce

---

# Domain Diagram

```text
ApplicationUser
│
├── Players
│     ├── SaveGames
│     ├── PlayerAchievements
│     └── LeaderboardEntries
│
└── Purchases
      │
      ├── PurchaseItems
      │
      └── Payments

Game
│
├── GameProducts
├── Achievements
└── LeaderboardEntries
```

---

# Identity Domain

## ApplicationUser

Represents an authenticated user within the platform.

Responsibilities

- Authentication
- Authorization
- Ownership of business resources
- Purchase history

Relationships

- One user can own multiple Players.
- One user can create multiple Purchases.

---

# Gaming Domain

## Game

Represents a published game.

Responsibilities

- Store game metadata
- Organize digital products
- Associate achievements
- Maintain leaderboard entries

Relationships

- One Game has many GameProducts.
- One Game has many Achievements.
- One Game has many LeaderboardEntries.

---

## Player

Represents a player's profile for a specific game.

Responsibilities

- Store player progression
- Connect save games
- Unlock achievements
- Participate in leaderboards

Relationships

- Belongs to one ApplicationUser.
- Belongs to one Game.
- Owns many SaveGames.
- Owns many PlayerAchievements.

---

## SaveGame

Represents a saved game state.

Responsibilities

- Persist player progress
- Support game continuation
- Preserve game history

Each SaveGame belongs to one Player.

---

## Achievement

Represents an unlockable in-game objective.

Responsibilities

- Define achievement metadata
- Track completion requirements

Each Achievement belongs to one Game.

---

## PlayerAchievement

Represents the association between a Player and an Achievement.

Responsibilities

- Register unlocked achievements
- Store unlock date

---

## LeaderboardEntry

Represents a player's ranking.

Responsibilities

- Store scores
- Calculate ranking positions
- Display competitive results

---

# Commerce Domain

## GameProduct

Represents a digital product available for purchase.

Examples

- Base Game
- DLC
- Expansion
- Cosmetic Pack
- Bundle

Responsibilities

- Product catalog
- Pricing
- Availability

Each GameProduct belongs to one Game.

---

## Purchase

Represents a purchase created by a user.

Responsibilities

- Store purchased products
- Calculate total amount
- Manage purchase lifecycle

States

- Pending
- Paid
- Cancelled

Relationships

- Belongs to one ApplicationUser.
- Contains one or more PurchaseItems.
- Owns one or more Payments.

---

## PurchaseItem

Represents an individual product inside a purchase.

Responsibilities

- Product reference
- Quantity
- Unit price
- Total price

---

## Payment

Represents a payment attempt for a purchase.

Responsibilities

- Store payment information
- Track payment status
- Ensure idempotency

States

- Pending
- Paid
- Cancelled

---

# Business Rules

## Ownership

Users can only access resources they own unless they have administrative privileges.

---

## Payments

Only pending purchases may receive new payment attempts.

Duplicate active payments are not allowed.

---

## Products

Inactive products cannot be purchased.

---

## Authentication

All commerce operations require authenticated users.

Administrative operations require the Admin role.

---

# Current Domain Status

| Domain | Status |
|---------|--------|
| Identity | ✅ |
| Players | ✅ |
| Achievements | ✅ |
| Save Games | ✅ |
| Leaderboards | ✅ |
| Commerce | ✅ |
| Payments | ✅ |
| Authorization | ✅ |
| Automated Tests | ✅ (40 Tests) |
| Documentation | ✅ |
| Docker Support | ✅ |
| Health Checks | ✅ |

---

# Domain Maturity

The current business domain is considered stable.

Core commerce workflows, authentication, authorization, validation, and persistence have been implemented and covered by automated tests.

Future development will primarily focus on expanding platform capabilities rather than redesigning the existing domain model.

Examples include:

- Wishlist
- Friends System
- Reviews
- Shopping Cart
- Inventory
- Notifications
- Digital Library

The existing domain model provides the foundation for these future modules without requiring structural redesign.

---

# Future Domain Evolution

Future versions may introduce:

- Wishlist
- Friends System
- Reviews
- Shopping Cart
- Coupons
- Inventory
- Notifications
- Digital Library
- Multiplayer Statistics
- Game Sessions

These features are intentionally outside the scope of version 2.0.