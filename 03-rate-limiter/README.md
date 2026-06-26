# 03 · Rate Limiter

**Building block:** Rate limiting (fixed/sliding window, token bucket) with Redis counters.
**Real scenario:** Protect an API or login endpoint from abuse and runaway clients.
**Time:** ~3–4 hours · **Tier:** Building-block spike

## What you'll build
Middleware that caps how many requests a client (by IP or API key) may make per time window, returning
**HTTP 429 Too Many Requests** when exceeded. You'll start with a naive in-memory counter, see why it
breaks across multiple instances, then move the state to **Redis** so it works cluster-wide.

## Why this matters
Every public API needs this. It's also one of the most common *standalone* interview questions because it
forces you to reason about algorithms, distributed state, and race conditions.

## What you'll be able to answer afterwards
- **"Design a rate limiter."**
- "Token bucket vs leaky bucket vs sliding window — when would you pick each?"
- "Why does an in-memory counter fail behind a load balancer, and how does Redis fix it?"
- "Where in the stack should rate limiting live?"

## Definition of Done (full version in `PLAN.md`)
- Exceeding the limit returns `429` with a `Retry-After` header.
- The limit holds **across two API replicas** (because the counter lives in Redis, not memory).
- You can explain why your Redis increment is **atomic** and why that matters.

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`INTERVIEW.md`](INTERVIEW.md)
