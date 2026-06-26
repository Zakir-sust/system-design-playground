# Plan — 09 · Background Jobs & Fan-out Pub/Sub

## Goal
Run scheduled/queued background jobs and fan a single event out to multiple subscribers.

## Scope
**In:** Hangfire/Quartz recurring + fire-and-forget jobs, retries, dashboard; MassTransit `Publish` fan-out to N consumers.
**Out:** full notification product (project 12), distributed scheduler at scale (mention Kafka/partitioning).

## Milestones

- [ ] **M1 — Fire-and-forget + recurring jobs.**
  Add Hangfire (uses Postgres for job storage) or Quartz.NET. Create a recurring job (e.g. "delete expired
  rows every 5 min") and a fire-and-forget job enqueued from an endpoint. Open the **dashboard** and watch jobs run.

- [ ] **M2 — Retries & failure.**
  Make a job throw; observe automatic retries/backoff and the failed-job view. Tune retry count.

- [ ] **M3 — Pub/sub fan-out.**
  Publish `UserSignedUp` (project 04 broker). Add **three** independent consumers — `WelcomeEmail`,
  `Analytics`, `Provisioning` — each receives its own copy. Contrast with `Send` (one queue, one handler).

- [ ] **M4 — Safe on multiple instances.**
  Run two app replicas. Ensure the **recurring** job fires once, not once-per-instance (Hangfire uses a
  distributed lock / single scheduler; understand how). Fan-out consumers, by contrast, *should* scale out.

## Initial → Improve

**Initial:** one recurring job + one event fanned out to multiple consumers. ✅ core lesson.

**Improvements:**
- [ ] **Idempotent jobs** — make jobs safe to run twice (ties to project 05).
- [ ] **Outbox cleanup job** — schedule the archival of old outbox rows from project 05.
- [ ] **Delayed messages** — schedule a message for the future (MassTransit scheduling) vs a cron job — when each?
- [ ] **Backpressure** — what happens if jobs are enqueued faster than they run? Concurrency limits.
- [ ] **Kafka contrast (read-only)** — note how a partitioned log (Kafka) gives ordered, replayable fan-out vs RabbitMQ's model.

## Definition of Done
- [ ] Recurring job runs on schedule; failures retry; dashboard shows history.
- [ ] One published event reaches 3 independent subscribers.
- [ ] With 2 instances, the recurring job runs once (not twice); you can explain why.
