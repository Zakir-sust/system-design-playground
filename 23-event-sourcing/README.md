# 23 · Event Sourcing + CQRS

**Building block:** Event store, projections, snapshots (state as a log of events).
**Tier:** Scale/distributed · **Time:** 1–2 days · **Interview question:** "Event sourcing / audit-heavy systems."

> 📄 **STUB** — expand using projects 00–09 as the template. See [`../CONCEPTS.md`](../CONCEPTS.md) Part B.

## What you'll build
A bank-account / ledger domain that stores **events** (`Deposited`, `Withdrawn`) instead of current state.
Current balance is derived by replaying events; read models (**projections**) serve queries. Pairs naturally
with CQRS (from project 16).

## Builds on
16 (CQRS, aggregates), 05 (events/idempotency). Feeds into 27 (payment ledger).

## Initial → Improve
**Initial (M1):** classic CRUD `Account` with a mutable `Balance` column — the baseline you'll replace.
**Improvements:**
- **Append events** to an event store; rebuild the aggregate by replaying them.
- Build a **projection** (read model) for fast balance/history queries (CQRS read side).
- **Snapshots** so you don't replay millions of events every load.
- **Temporal queries** ("balance as of last Tuesday") — the superpower event sourcing gives you.
- Event **versioning/upcasting** as the schema evolves.

## Key questions this answers
- What is event sourcing and what does it buy you? (audit trail, time-travel, debuggability)
- How do you query current state? (projections)
- What does it cost? (projections, snapshots, event versioning, complexity)
- When is it overkill vs essential?
