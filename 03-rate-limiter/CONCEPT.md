# Concept — Rate limiting

## What & why
Rate limiting caps how often a client can call you in a time window. It protects against abuse, brute-force
login, scrapers, buggy clients, and accidental self-DoS, and it enforces fair use across tenants.

## Where it lives
At the edge (gateway/load balancer), in API middleware, or both. Closer to the edge = cheaper (you reject
before doing work). Per-service middleware = simplest to start. You'll do middleware here, then discuss
moving it to the gateway (project 01).

## The algorithms

**Fixed window** — count requests per fixed bucket (e.g. each clock-minute). Simple, but allows a 2× burst
across the boundary (100 at 0:59 + 100 at 1:00).

**Sliding window log** — store each request's timestamp (a Redis sorted set); on each call, drop timestamps
older than the window and count what's left. Accurate, more memory.

**Sliding window counter** — weighted blend of current + previous fixed window. Good accuracy, cheap.

**Token bucket** — a bucket holds N tokens, refilled at a steady rate; each request spends one; empty =
reject. Allows controlled bursts, smooth long-run rate. Very common (used by AWS, Stripe).

**Leaky bucket** — requests queue and drain at a fixed rate; smooths output, adds latency.

> Interview-ready default: **token bucket** for general APIs (burst-friendly), **sliding window** when you
> need precise "no more than N per window."

## Why Redis (the distributed part)
Behind a load balancer you have many API instances. An in-memory counter is **per instance**, so the real
limit becomes `instances × limit`. Put the counter in **Redis** (shared by all instances) and the limit is
global. This is the key lesson of M3.

## Atomicity (the race condition)
`GET count; if ok then INCR` is racy — two requests can both read "99" and both proceed. Use an **atomic**
operation: `INCR` returns the new value in one step, paired with `EXPIRE` to reset the window. Even better,
a single **Lua script** does check-limit-and-increment atomically in one round-trip (no window where two
requests slip through).

## .NET wiring (you write it)
- Custom middleware that builds the client key (IP / API key), runs the algorithm against Redis
  (`StackExchange.Redis`), and short-circuits with `429` + `Retry-After` when over.
- Or `Microsoft.AspNetCore.RateLimiting` (built-in) — convenient, but per-instance unless backed by shared
  state. Know both.

## Pitfalls
- In-memory counters behind a load balancer (the classic bug M3 demonstrates).
- Non-atomic check-then-increment (lets requests slip through under load).
- Keying by IP only (NAT/proxies share IPs; prefer API key/user where possible, set IP via `X-Forwarded-For` correctly).
- No `Retry-After` header (clients can't back off intelligently).

## Further reading
- "Token bucket" and "sliding window" explanations in any system-design primer.
- Redis `INCR`/`EXPIRE` and scripting (`EVAL`).
