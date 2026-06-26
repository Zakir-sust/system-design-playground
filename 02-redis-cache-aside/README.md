# 02 · Redis Cache-Aside

**Building block:** Caching (Redis, cache-aside pattern, TTL). **★ Also your first testing project.**
**Real scenario:** A read-heavy endpoint is hammering the database; put a cache in front of it.
**Time:** ~3–4 hours (+1–2h for the testing part) · **Tier:** Building-block spike

## What you'll build

Extend the API from project 00 with a Redis container and apply **cache-aside** to a read endpoint
(`GET /notes/{id}`): check Redis first, on a miss read Postgres and populate Redis with a TTL. Then you'll
write your **first automated tests** for it.

## Why this matters
Caching is the single most common scaling lever. "Reads are slow / the DB is overloaded" → cache. You need
to know *what* to cache, *how* to keep it fresh, and *what breaks* when you do.

## What you'll be able to answer afterwards
- "Design a distributed cache" / "How would you reduce database read load?"
- "Cache-aside vs write-through vs write-behind?"
- "How do you keep the cache fresh, and what's a cache stampede?"
- (Testing) "What's the difference between a unit test and an integration test?"

## Definition of Done (full version in `PLAN.md`)
- `GET /notes/{id}` returns from Redis on the second call (visible: log a HIT/MISS, or watch DB query logs).
- Updating a note invalidates its cache entry.
- You have **at least one passing unit test** and **one passing integration test** (real Redis via Testcontainers).

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`TESTING.md`](TESTING.md) 4. [`INTERVIEW.md`](INTERVIEW.md)
