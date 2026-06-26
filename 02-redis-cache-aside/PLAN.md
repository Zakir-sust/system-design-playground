# Plan — 02 · Redis Cache-Aside

## Goal
Add Redis to the project-00 API and serve a read endpoint via cache-aside, then cover it with tests.

## Scope
**In:** Redis container, StackExchange.Redis connection, cache-aside on one GET, invalidation, first tests.
**Out:** distributed locking, multi-level caches, cache as primary store.

## Milestones

- [ ] **M1 — Connect to Redis.**
  Add a `redis` service to Compose. Add `StackExchange.Redis`; register `IConnectionMultiplexer` as a
  **singleton** (it's expensive; one per app). Prove the connection with a `PING` on startup.

- [ ] **M2 — Cache-aside on `GET /notes/{id}`.**
  Read key `note:{id}` from Redis → **hit**: deserialize & return → **miss**: read Postgres, serialize to
  Redis with a TTL (e.g. 60s), return. Log HIT/MISS so you can *see* it working.

- [ ] **M3 — Invalidate on write.**
  On `PUT`/`DELETE /notes/{id}`, delete `note:{id}` from Redis. Observe the next GET re-populates it.

- [ ] **M4 — Tests (see `TESTING.md`).**
  One **unit test** of the cache-aside decision logic; one **integration test** that runs against a real
  Redis container via Testcontainers.

## Initial → Improve

**Initial:** cache-aside + invalidation on one endpoint, with HIT/MISS logging. ✅ core lesson.

**Improvements:**
- [ ] **Measure it** — log or expose cache **hit ratio**; compare endpoint latency with cache on vs off.
- [ ] **Stampede protection** — when many requests miss the same hot key at once they all hit the DB. Add a short lock / single-flight so only one rebuilds it.
- [ ] **Negative caching** — cache "not found" briefly to stop repeated misses for a bad id (carefully — short TTL).
- [ ] **Eviction policy** — set Redis `maxmemory` + `maxmemory-policy` (e.g. `allkeys-lru`); understand what gets evicted under pressure.
- [ ] **Key design** — version your keys (`note:v1:{id}`) so a schema change can invalidate en masse.

## Definition of Done
- [ ] Second GET of the same id is served from Redis (HIT logged); DB not queried.
- [ ] Update invalidates; next GET is a MISS then HIT.
- [ ] `dotnet test` runs and passes a unit test + a Testcontainers integration test.
- [ ] You can state when cache-aside returns **stale** data and why that's an accepted trade-off.
