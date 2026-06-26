# Plan — 04 · Message Broker

## Goal
Publish an event from an API and process it in a separate worker via RabbitMQ + MassTransit; understand
queues, retries, and dead-lettering.

## Scope
**In:** RabbitMQ, MassTransit publish/consume, retries, DLQ, consumer concurrency.
**Out:** outbox (project 05), sagas (project 22), Kafka-style log semantics (project mentioned in 09).

## Milestones

- [ ] **M1 — Run RabbitMQ.**
  Add a `rabbitmq:management` service to Compose (management UI on :15672 — log in and look around; *seeing*
  the queues is the point). 

- [ ] **M2 — Publish an event.**
  In the API, configure MassTransit with the RabbitMQ transport. On `POST /orders`, **publish** an
  `OrderPlaced` message and return `202 Accepted` immediately.

- [ ] **M3 — Consume it in a worker.**
  A separate worker service (or a hosted service) with a `Consumer<OrderPlaced>` that logs/handles it.
  Watch the message flow in the RabbitMQ UI.

- [ ] **M4 — Failure handling.**
  Make the consumer throw for a "poison" message. Configure **retry** (e.g. 3 attempts with backoff); after
  retries are exhausted, MassTransit moves it to an `_error` (dead-letter) queue. Observe both.

## Initial → Improve

**Initial:** publish `OrderPlaced`, consumer handles it, message survives a consumer restart. ✅ core lesson.

**Improvements:**
- [ ] **Retry + DLQ** — configurable retry policy; inspect the error queue; redeliver manually.
- [ ] **Consumer concurrency / prefetch** — raise throughput; understand ordering implications.
- [ ] **Publish vs Send** — `Publish` (pub/sub, many subscribers) vs `Send` (to one queue). Add a second consumer to `OrderPlaced` and watch both receive it.
- [ ] **Idempotent consumer** — handle the same message twice safely (sets up project 05).
- [ ] **Message versioning** — add a field to the event; think about old vs new consumers.

## Definition of Done
- [ ] `POST /orders` returns fast; worker processes `OrderPlaced` asynchronously.
- [ ] Message persists in the queue across a consumer restart.
- [ ] A poison message ends up in the dead-letter/error queue after retries.
- [ ] You can explain queue (one consumer) vs pub/sub (fan-out) and at-least-once delivery.
