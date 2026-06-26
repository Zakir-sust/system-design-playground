# Interview angle — 09 · Background Jobs & Fan-out Pub/Sub

## Questions this prepares you for

- **"How do you handle work that shouldn't block the request?"**
  → Background jobs (fire-and-forget/queued) and scheduled jobs; reply fast, do the work async, with retries and visibility.

- **"How do you run a scheduled task across many instances exactly once?"**
  → A distributed lock / leader election so only one instance triggers it (Hangfire/Quartz via shared
  storage). Contrast with queue consumers, which you intentionally scale out.

- **"Pub/sub vs work queue?"**
  → Pub/sub: every subscriber gets a copy (fan-out reactions). Work queue: competing consumers, each message
  done once (load sharing).

- **"How do you make background work reliable?"**
  → Retries + backoff, dead-letter for poison jobs, idempotent handlers, monitoring/dashboard, and don't lose
  the trigger (outbox if the job must follow a DB change).

- **"RabbitMQ vs Kafka for this?"**
  → RabbitMQ: flexible routing, per-message ack, good for task/fan-out. Kafka: partitioned, ordered,
  replayable log — better for event streaming/high-throughput pipelines and re-processing history.

## Trade-offs
- More async = more eventual consistency and harder tracing (project 28).
- Scheduling guarantees ("at least once on time") vs cost of distributed coordination.
- Hangfire's simplicity vs Quartz's scheduling power vs a broker's delayed-message feature — pick per need.
