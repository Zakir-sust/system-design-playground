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

---

## Running it

```bash
docker compose -f SimpleNote/docker-compose.yml up --build
```

| Service | URL | Notes |
|---|---|---|
| API | http://localhost:8080 | |
| Seq (logs) | http://localhost:5341 | where you watch HIT/MISS |
| pgAdmin | http://localhost:5050 | `admin@admin.com` / `admin` |
| Postgres | `localhost:5432` | `postgres` / `postgres`, db `firstdb` |
| Redis | `localhost:6379` | |

Migrations run automatically at startup. To see the cache work, request the same note twice and look
for `Cache MISS` then `Cache HIT` in Seq. To see the keys directly:

```bash
docker compose -f SimpleNote/docker-compose.yml exec redis redis-cli KEYS 'note:*'
```

## API

| Method | Route | Body | Returns |
|---|---|---|---|
| `GET` | `/notes` | — | `200` all notes (**not cached**) |
| `GET` | `/notes/{id:guid}` | — | `200` note, `404` if missing — **cache-aside** |
| `POST` | `/notes` | `{"content": "..."}` | `201` + created note, `400` if content is empty |
| `PUT` | `/notes/{id:guid}` | `{"content": "..."}` | `204`, `404` if missing, `400` if content is empty — **invalidates** |
| `DELETE` | `/notes/{id:guid}` | — | `204`, `404` if missing — **invalidates** |
| `GET` | `/health` | — | Postgres **and** Redis |

Cache key is `note:{id}`; TTL comes from `Cache:TtlSeconds` (default 60s).

## How it's put together

```
SimpleNote/
  Caching/
    ICache.cs                 Get/Set/Remove — no Redis types, so callers are fakeable
    RedisCache.cs             the only file that references StackExchange.Redis
    CacheSettings.cs          TtlSeconds, bound from the "Cache" section
  Notes/
    INoteRepository.cs        the database seam
    NoteRepository.cs         plain EF Core, no caching
    CachedNoteRepository.cs   ← the cache-aside logic lives here
    NoteEndpoints.cs          HTTP only: bind, call, map to a status code
  Program.cs                  composition root
SimpleNote.Tests/             xunit + Testcontainers
```

The caching is a **decorator**: `CachedNoteRepository` wraps `NoteRepository`, and only the decorator is
registered as `INoteRepository`. Endpoints ask for `INoteRepository` and cannot tell a cache exists. That's
deliberate — a future write path physically cannot skip invalidation, rather than relying on whoever writes
it to remember.

It's also what makes the unit test possible: `CachedNoteRepository` depends on two interfaces and nothing
else, so a fake cache plus a call-counting fake repository are enough to assert *"on a cache hit, the
database was never touched."*

### Degrading when Redis is down
A cache should never make a read *less* available than it was without one. Two things make that true here:
`AbortOnConnectFail` is off, so the app still starts and serves when Redis is unreachable; and every
`_cache` call in `CachedNoteRepository` is wrapped so a cache failure falls through to Postgres. `/health`
reports Redis as unhealthy meanwhile, so degraded is visible rather than silent.

The one exception is invalidation: if the eviction fails after a write commits, the request still succeeds
and the stale entry survives until the TTL expires. That's logged at Error.

### Two things it deliberately does not do
- **`GET /notes` (the list) is not cached.** Caching a collection means every insert and delete has to
  invalidate it — a much harder invalidation problem than single-key, and not the lesson here.
- **Invalidation is not transactional.** Redis is not part of the Postgres transaction, so if the process
  dies between the commit and the eviction, the entry stays stale until the TTL expires. The TTL *is* the
  backstop. Systems that can't tolerate that drive invalidation from the write-ahead log (CDC) or an outbox
  instead. Being able to say this out loud is the last DoD item.

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`TESTING.md`](TESTING.md) 4. [`INTERVIEW.md`](INTERVIEW.md)
