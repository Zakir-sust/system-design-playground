# 22 · Saga / Distributed Transactions

**Building block:** Distributed transactions via sagas (orchestration), compensation, eventual consistency.
**Tier:** Scale/distributed · **Time:** 1–2 days · **Interview question:** "Microservice consistency / distributed transactions."

> 📄 **STUB** — expand using projects 00–09 as the template. Builds directly on the e-commerce domain (16).

## What you'll build
A multi-step business process across services — **order → payment → inventory → shipping** — where you can't
use one ACID transaction. You coordinate it with a **saga** (MassTransit state machine) and **compensate**
(undo) when a step fails.

## Builds on
16 (order domain), 04 (broker), 05 (outbox + idempotency — essential here).

## Initial → Improve
**Initial (M1):** a happy-path **event chain**: each service reacts to the previous event. No failure handling yet.
**Improvements:**
- Introduce a **MassTransit state-machine saga** to orchestrate and track state.
- Add **compensating actions**: payment fails → release reserved inventory; shipping fails → refund.
- **Timeouts** for steps that never respond; retries; idempotent handlers (project 05).
- Orchestration vs **choreography** — implement one, discuss the other.

## Key questions this answers
- Why can't you use a single transaction across services? (no distributed ACID; CAP)
- What is a saga, and how does compensation replace rollback?
- Orchestration vs choreography — trade-offs?
- How do timeouts, retries, and idempotency keep a saga correct?
