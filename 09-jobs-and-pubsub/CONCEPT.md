# Concept — Background jobs & pub/sub fan-out

## Two distinct needs (don't conflate them)

**1. Background jobs** — work that runs *outside* the HTTP request:
- **Fire-and-forget** — enqueue now, run soon (resize image, send one email).
- **Recurring/scheduled** — cron-like (nightly digest, cleanup every 5 min).
- **Delayed** — run once at a future time (reminder in 24h).
Tools: **Hangfire** (simple, Postgres-backed, great dashboard) or **Quartz.NET** (richer scheduling).

**2. Pub/sub fan-out** — one event, many independent reactions. `UserSignedUp` → welcome email + analytics +
provisioning, each a separate consumer that can fail/scale independently. Tool: the broker from project 04
(`Publish`, not `Send`).

## Queue (work) vs pub/sub (fan-out)
- **Work queue** — N competing consumers share the load; each message handled **once**. Use for "do this unit of work."
- **Pub/sub** — each subscriber gets **its own copy**. Use for "this happened; whoever cares, react."

## The multi-instance gotcha
When you run several app replicas:
- **Recurring jobs** must fire **once**, not once per instance → the scheduler needs a **distributed lock** /
  leader so only one instance triggers it. Hangfire/Quartz handle this with shared storage; know that it's happening.
- **Fan-out consumers** are the opposite — you *want* them to scale across instances (competing consumers per
  subscription) to drain faster.

This single point — "scheduled = run once cluster-wide; queue consumers = scale out" — is a common interview
trap and a real production bug source.

## Reliability
- **Retries with backoff** for transient failures; cap attempts; dead-letter/failed view for the rest.
- **Idempotency** — a job/consumer may run twice (crash + retry), so make effects idempotent (project 05).
- **Visibility** — a dashboard/metrics so you can see what ran, failed, and is pending.

## .NET wiring (you write it)
- Hangfire: `Hangfire.AspNetCore` + `Hangfire.PostgreSql`; `RecurringJob.AddOrUpdate(...)`,
  `BackgroundJob.Enqueue(...)`; map the dashboard.
- Fan-out: MassTransit `IPublishEndpoint.Publish(new UserSignedUp(...))` + multiple `IConsumer<UserSignedUp>`.

## Pitfalls
- Recurring job firing once per instance (forgot the distributed lock/shared store).
- Long work on the request thread instead of a background job.
- Non-idempotent jobs double-applying on retry.
- Cron drift / timezone bugs in schedules.

## Further reading
- Hangfire docs (recurring jobs, dashboard, distributed locks).
- MassTransit `Publish` vs `Send`.
