# Interview angle — 04 · Message Broker

## Questions this prepares you for

- **"When do you use a message queue vs a direct HTTP call?"**
  → Async/queue for slow, optional, retryable, or fan-out work, and to decouple availability and smooth load
  spikes. Sync HTTP when the caller needs the result *now* to proceed.

- **"Queue vs pub/sub?"**
  → Queue = one consumer per message (load sharing). Pub/sub = every subscriber gets a copy (fan-out, e.g. one
  `OrderPlaced` → email + analytics + inventory).

- **"What delivery guarantees does your system have?"**
  → At-least-once is the practical default → consumers must be idempotent. Exactly-once is hard end-to-end;
  approximate with at-least-once + dedup (project 05).

- **"A consumer keeps failing on one message — what happens?"**
  → Retry with backoff for transient errors; after N attempts, dead-letter it so it doesn't block the queue;
  alert and inspect.

- **"How do queues help you scale?"**
  → They buffer bursts and let you add **competing consumers** to drain faster; the producer is unaffected by
  consumer speed.

## Trade-offs
- Async decouples but introduces **eventual consistency** and harder debugging (you can't follow one stack
  trace) → observability matters (project 28).
- More moving parts (the broker is now critical infra to run and monitor).
- Ordering is not guaranteed with concurrent consumers — design for it or use partitioning (project 09 / Kafka note).
