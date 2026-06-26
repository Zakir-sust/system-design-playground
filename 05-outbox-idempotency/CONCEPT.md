# Concept — Outbox pattern & idempotency

## The dual-write problem
You want to do two things on checkout: (1) save the order to the DB, (2) publish `OrderPlaced` to the broker.
These are **two different systems** with no shared transaction. Any ordering can fail:
- Publish then save → broker has an event for an order that doesn't exist (if the save fails).
- Save then publish → order exists but the event is lost (if the publish fails / broker is down).

You cannot make two systems commit atomically without distributed-transaction machinery you don't want.

## The outbox pattern (the fix)
Write the event into an **outbox table in the same database transaction** as the business change:
```
BEGIN
  INSERT order ...
  INSERT outbox_message (OrderPlaced payload, sent = false)
COMMIT        -- both succeed or both fail: one atomic local transaction
```
A separate **relay** then reads unsent outbox rows, publishes them to the broker, and marks them sent:
```
loop:
  rows = select * from outbox where sent = false
  for r in rows: broker.publish(r); mark r sent
```
Now the event is durably recorded the instant the order is. If the relay crashes mid-publish, the row stays
`sent = false` and is retried. Nothing is lost — at the cost of possible **duplicate** delivery.

## Idempotency (the necessary partner)
Because the relay (and the broker) are **at-least-once**, a consumer may receive the same message twice.
**Idempotent** = handling it twice has the same effect as once. Achieve it by:
- Giving each message a stable **id**.
- The consumer records processed ids in a dedup/**inbox** table and skips ones it's seen.
- Or design the operation to be naturally idempotent (e.g. `SET status = 'paid'` rather than `balance += x`).

**Idempotency key** (API-level cousin): a client sends a unique key with a request; the server ensures that
key is processed once even if the client retries. (You'll use this in 10 and 27.)

## .NET wiring (you write it)
- An `OutboxMessages` table + EF Core entity; insert within the same `DbContext` `SaveChanges` as the order.
- A `BackgroundService` relay that polls and publishes via MassTransit.
- A dedup table for the consumer (`ProcessedMessages`).
- Then try MassTransit's built-in **EF Core Outbox** (`AddEntityFrameworkOutbox`) and compare.

## Pitfalls
- Relay with no idempotent consumer → duplicates double-apply (e.g. charge twice). Both halves are required.
- Polling too aggressively (DB load) vs too slowly (latency) — tune the interval / use change notifications.
- Forgetting to clean up old outbox rows (table grows forever).
- Putting the publish inside the transaction by calling the broker directly — that's the dual-write again.

## Further reading
- "Transactional outbox" pattern (microservices.io).
- MassTransit Transactional Outbox docs.
