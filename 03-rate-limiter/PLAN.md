# Plan — 03 · Rate Limiter

## Goal
Build a working rate limiter, first in-memory then distributed via Redis, and understand the algorithms.

## Scope
**In:** fixed window → sliding window → token bucket; per-IP/key; Redis-backed; 429 + Retry-After.
**Out:** quota billing, per-endpoint tiers (mention only), API-key management.

## Milestones

- [ ] **M1 — Fixed window, in-memory.**
  Middleware: key = client IP. Count requests per 1-minute bucket in a `ConcurrentDictionary`. Over the
  limit → `429`. Simple, and intentionally flawed (see M3).

- [ ] **M2 — See the boundary problem.**
  Fixed windows allow a burst at the edge: limit 100/min, a client can send 100 at 0:59 and 100 at 1:00 =
  200 in 2 seconds. Reproduce it. This motivates the sliding window.

- [ ] **M3 — See the distributed problem, then fix it with Redis.**
  Run **two** API replicas behind the project-01 gateway. The in-memory counter is now per-replica, so the
  real limit is 2×. Move the counter to **Redis** (`INCR` + `EXPIRE`, or a single atomic Lua script) so all
  replicas share one count.

- [ ] **M4 — Better algorithms.**
  Implement **sliding window** (sorted set of timestamps, trim old ones) and/or **token bucket** (tokens
  refill at a rate; each request spends one). Compare behavior under bursts.

## Initial → Improve

**Initial:** Redis-backed fixed-window limiter returning 429 across replicas. ✅ core lesson.

**Improvements:**
- [ ] **Atomicity** — replace `INCR`+`EXPIRE` (two round-trips, racy) with one **Lua script** so check-and-increment is atomic.
- [ ] **Token bucket** for smooth bursting; expose `X-RateLimit-Remaining` / `Retry-After` headers.
- [ ] **Keys & tiers** — limit by API key, with different limits per tier (free vs paid).
- [ ] **Compare to built-in** — try .NET's `Microsoft.AspNetCore.RateLimiting` middleware; note it's per-instance unless you back it with shared state. Discuss when to use built-in vs custom-distributed.
- [ ] **Where to enforce** — move the limiter to the gateway (project 01) so services don't each re-implement it.

## Definition of Done
- [ ] Burst over the limit → `429` + `Retry-After`.
- [ ] Limit is correct with 2 replicas running (Redis-shared counter).
- [ ] You can explain the fixed-window boundary bug and how sliding window / token bucket address it.
- [ ] You can explain why the increment must be atomic.
