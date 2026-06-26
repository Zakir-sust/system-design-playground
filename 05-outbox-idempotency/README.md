# 05 · Outbox & Idempotency

**Building block:** Reliable messaging — the transactional outbox pattern + idempotent consumers. **★ Second testing project.**
**Real scenario:** "We saved the order but the 'order placed' email never sent" — the dual-write bug, fixed properly.
**Time:** ~4 hours · **Tier:** Building-block spike

## What you'll build
Take project 04's publish flow and make it **reliable**. First reproduce the **dual-write bug** (DB commit
succeeds, broker publish fails → lost event). Then fix it with the **outbox pattern**: write the event into
an outbox table *in the same DB transaction* as the state change, and a relay publishes it afterward. Make
the consumer **idempotent** so at-least-once redelivery is safe. Then write integration tests proving it.

## Why this matters
This is the difference between a demo and a system you'd trust with money. The outbox + idempotency combo is
the backbone of reliable event-driven systems and shows up everywhere from here on (10, 16, 22, 27).

## What you'll be able to answer afterwards
- "How do you reliably publish an event when you also write to the database?" (the dual-write problem)
- "What is the outbox pattern and why does it work?"
- "What does idempotent mean and how do you make a consumer idempotent?"

## Definition of Done (full version in `PLAN.md`)
- You've **demonstrated** the dual-write bug (kill the broker between commit and publish → event lost).
- With the outbox, the event is **never lost** even if the broker is briefly down.
- Delivering the same message twice has the **same effect as once**.
- Integration tests (Postgres + RabbitMQ via Testcontainers) prove both.

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`TESTING.md`](TESTING.md) 4. [`INTERVIEW.md`](INTERVIEW.md)
