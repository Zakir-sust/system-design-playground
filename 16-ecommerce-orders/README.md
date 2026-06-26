# 16 · E-commerce Orders — DDD + Clean Architecture + CQRS 🖥️ (Angular UI)

**Building block:** Domain-Driven Design, Clean Architecture, CQRS (the architecture-patterns home).
**Tier:** Real app + capstone · **Time:** 2–3 days · **Interview relevance:** senior modeling; pairs with "Design e-commerce/Amazon."

> 📄 **STUB** — expand using projects 00–09 as the template. See [`../CONCEPTS.md`](../CONCEPTS.md) Part B for the patterns. **Has an Angular UI.**

## What you'll build
An ordering/checkout domain built the way a senior engineer structures complex business logic: a rich
**domain model** (aggregates, value objects, domain events), **Clean Architecture** layering, and **CQRS** to
split writes from reads. This is where "what is DDD and what else is there" becomes concrete.

## Builds on
00 (API+DB), 05 (outbox for integration events), 04 (events). Feeds into 22 (saga) and 23 (event sourcing).

## Initial → Improve
**Initial (M1):** plain layered **CRUD** for orders/products/cart — deliberately "anemic," so you feel the pain DDD solves.
**Improvements:**
- Refactor to **DDD**: `Order` aggregate guarding invariants, `Money`/`Address` value objects, `OrderPlaced` domain event.
- **Clean Architecture**: Domain ← Application (use cases) ← Infrastructure (EF/broker) ← API; dependencies point inward.
- **CQRS**: commands change state; a separate **read model** serves queries (e.g. order history) — possibly a different table/store.
- **Integration events** via the **outbox** (project 05): `OrderPlaced` → inventory/notification.
- Inventory **reservation** on checkout (sets up booking/concurrency, project 17).

## Key questions this answers
- What is DDD and when is it worth it (vs CRUD)? (aggregates, value objects, ubiquitous language)
- What does Clean Architecture buy you? (testable domain, swappable infra)
- What problem does CQRS solve, and what does it cost? (read/write asymmetry, eventual consistency)
