# Interview angle — 05 · Outbox & Idempotency

These concepts separate "I've used a queue" from "I've built reliable systems." They come up in payments,
ordering, and any "design X" where an action must reliably trigger downstream effects.

## Questions this prepares you for

- **"You write to the DB and publish an event. What can go wrong?"**
  → The dual-write problem: the two systems can't commit atomically, so you can lose or orphan events. Fix
  with the transactional **outbox** (event written in the same DB transaction; a relay publishes it).

- **"What does idempotent mean? How do you make a consumer idempotent?"**
  → Same input applied twice = same result. Use a message id + a dedup/inbox table, or naturally idempotent
  operations (set-state instead of increment). Required because delivery is at-least-once.

- **"What's an idempotency key in an API?"**
  → A client-supplied unique key so retries of the same request don't double-charge/double-create. (Stripe does this.)

- **"How do you get exactly-once?"**
  → You usually don't, end-to-end. You get at-least-once delivery + idempotent processing, which is
  *effectively* once. Say this clearly — claiming true exactly-once is a red flag.

## Trade-offs
- Outbox adds a table, a relay, and polling cost; the payoff is no lost events.
- At-least-once + idempotency is simpler and more robust than chasing real exactly-once.
- The relay's polling interval trades latency against DB load.

## Strong signal in an interview
Volunteering "I'd use an outbox so the publish can't be lost, and make the consumer idempotent because
delivery is at-least-once" — unprompted — signals real distributed-systems maturity.
