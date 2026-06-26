# Plan — 05 · Outbox & Idempotency

## Goal
Make event publishing reliable with the outbox pattern and idempotent consumers; prove it with tests.

## Scope
**In:** reproduce dual-write bug, outbox table + relay, idempotency keys, integration tests.
**Out:** full saga orchestration (project 22), Kafka exactly-once.

## Milestones

- [ ] **M1 — Reproduce the dual-write bug.**
  In project 04's `POST /orders`: save the order to Postgres (commit), then publish `OrderPlaced`. Now stop
  RabbitMQ *between* the commit and the publish (or throw there). Result: order exists, event lost. Feel the problem.

- [ ] **M2 — Add an outbox table.**
  In the **same transaction** that saves the order, insert a row into an `OutboxMessages` table (the
  serialized event). Commit both atomically. Nothing is published yet — but nothing can be lost.

- [ ] **M3 — Relay the outbox.**
  A background worker polls `OutboxMessages` for unsent rows, publishes them to RabbitMQ, marks them sent.
  Crash-safe: if it dies mid-publish, the row is still "unsent" and gets retried (→ at-least-once → why M4 matters).
  *(MassTransit ships an EF Core outbox — implement the table-poller yourself first to understand it, then compare.)*

- [ ] **M4 — Idempotent consumer.**
  Because the relay is at-least-once, the consumer may see a message twice. Give each message an id; the
  consumer records processed ids (an `InboxMessages`/dedup table) and skips duplicates.

## Initial → Improve

**Initial:** outbox guarantees the event is eventually published; consumer is idempotent. ✅ core lesson.

**Improvements:**
- [ ] **Use MassTransit's built-in EF Core Outbox** and compare with your hand-rolled version.
- [ ] **Cleanup** — archive/delete old sent outbox rows (a recurring job — ties to project 09).
- [ ] **Ordering** — discuss per-aggregate ordering of outbox messages.
- [ ] **Inbox pattern** — formalize the dedup table as an inbox; expire old keys.

## Definition of Done
- [ ] M1 bug demonstrated and understood.
- [ ] With the outbox, a broker outage during checkout does **not** lose the event (it publishes when the broker returns).
- [ ] Re-delivering a message twice doesn't double-apply its effect.
- [ ] Integration tests (Testcontainers Postgres + RabbitMQ) prove "event eventually published" and "duplicate is ignored."
