# Concept — Caching with Redis

## What is Redis?
An in-memory key-value store. Because it lives in RAM (not disk like Postgres), reads/writes are
sub-millisecond. It's used for caching, session state, rate-limit counters (project 03), leaderboards,
pub/sub, and more. Here we use the most common job: **a cache in front of the database.**

## Where caching fits
Your database is the source of truth but is comparatively slow and easy to overload. If the same data is
read far more than it's written, keep a copy in Redis and serve reads from there. This cuts latency and
offloads the DB — the #1 scaling move.

## Cache-aside (a.k.a. lazy loading) — the pattern you'll implement

```
read(id):
    v = redis.get("note:"+id)
    if v exists:  return v                  # HIT
    v = db.get(id)                          # MISS
    redis.set("note:"+id, v, ttl=60s)       # populate for next time
    return v

write(id, ...):
    db.update(id, ...)
    redis.del("note:"+id)                    # invalidate; next read repopulates
```

The app manages the cache. Redis never talks to the DB. This is the most common caching pattern and the
default you should reach for.

## Other strategies (know them for the interview)
- **Write-through** — write to cache *and* DB on every write. Cache always warm, writes slower.
- **Write-behind** — write to cache, flush to DB asynchronously. Fast writes, risk of data loss.
- **Cache-aside** (above) — simplest, cache only what's read, tolerates stale within TTL.

## The two hard parts
1. **Invalidation** — "There are only two hard things in CS: cache invalidation and naming things." Stale
   data lives until TTL expires or you explicitly delete the key. You accept some staleness for speed.
2. **Stampede / thundering herd** — a hot key expires and 1,000 concurrent requests all miss and hit the DB
   at once. Fix with a short lock so one rebuilds while others wait, or slightly randomized TTLs.

## Eviction
Redis has finite memory. With `maxmemory` set, a `maxmemory-policy` like `allkeys-lru` evicts least-recently-used
keys when full. TTL handles freshness; eviction handles capacity.

## .NET wiring (you write it)
- Package: `StackExchange.Redis`.
- Register `IConnectionMultiplexer` as a **singleton** (creating it is expensive; it multiplexes one
  connection). Get an `IDatabase` from it per operation (cheap).
- Serialize values to JSON for storage. Pick clear keys: `note:{id}`.
- (Alternative: `Microsoft.Extensions.Caching.StackExchangeRedis` gives you `IDistributedCache` — a simpler
  abstraction. Try the raw client first to understand what's happening, then compare.)

## Pitfalls
- Creating a new `ConnectionMultiplexer` per request (huge mistake — make it a singleton).
- Caching with no TTL (data never refreshes; memory fills).
- Forgetting to invalidate on write (silent stale reads).
- Caching `null`/not-found without care (either skip or use a very short TTL).

## Further reading
- StackExchange.Redis "Basics" + connection multiplexing.
- AWS/Azure cache-aside pattern docs.
