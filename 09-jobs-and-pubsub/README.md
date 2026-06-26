# 09 · Background Jobs & Fan-out Pub/Sub

**Building block:** Background processing (scheduled + queued jobs) and pub/sub fan-out.
**Real scenario:** Send a daily digest, clean up expired data, and fan one event out to several independent handlers.
**Time:** ~3–4 hours · **Tier:** Building-block spike

## What you'll build
Two related capabilities: (1) **scheduled/recurring jobs** with Hangfire or Quartz.NET (cron-like work,
retries, a dashboard), and (2) **pub/sub fan-out** where one published event (`UserSignedUp`) is consumed by
several independent subscribers (welcome email, analytics, provisioning) — reusing the broker from project 04.

## Why this matters
Real systems do a lot *outside* the request: periodic aggregation, cleanup, retries, and reacting to events
in multiple places. This combines "do it later/on a schedule" with "tell everyone who cares." It's the engine
behind the notification system (12) and many maintenance tasks (e.g. outbox cleanup from 05).

## What you'll be able to answer afterwards
- "How do you run scheduled/background work reliably?"
- "Pub/sub fan-out vs a work queue — when each?"
- "How do you make a recurring job safe to run on multiple instances?"

## Definition of Done (full version in `PLAN.md`)
- A recurring job runs on a schedule and is visible in a dashboard; a failed job retries.
- Publishing one `UserSignedUp` event triggers **multiple** independent subscribers.
- The recurring job is safe when two app instances are running (no double-execution).

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`INTERVIEW.md`](INTERVIEW.md)
