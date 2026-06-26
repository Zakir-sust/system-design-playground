# Interview angle — 02 · Redis Cache-Aside

## Questions this prepares you for

- **"Design a distributed cache" / "How do you reduce DB read load?"**
  → Put Redis in front for read-heavy data; cache-aside; TTL for freshness; measure hit ratio. Scale Redis
  with replicas/cluster and partition keys via consistent hashing (projects 19/20).

- **"Cache-aside vs write-through vs write-behind?"**
  → Cache-aside: app fills cache on miss, simplest, tolerates staleness. Write-through: write both, always
  warm, slower writes. Write-behind: async flush, fastest writes, risk of loss. Default to cache-aside.

- **"How do you keep a cache consistent with the DB?"**
  → You don't get perfect consistency cheaply. Invalidate on write + TTL bound the staleness. Strong
  consistency would mean write-through + careful ordering, at a cost. State the trade-off explicitly.

- **"What is a cache stampede / thundering herd, and how do you prevent it?"**
  → A hot key expires and many requests miss simultaneously, all hitting the DB. Prevent with a rebuild lock
  (single-flight), staggered TTLs, or pre-warming.

- **"What do you cache, and what do you NOT cache?"**
  → Cache read-heavy, slow-to-compute, tolerant-of-staleness data. Don't cache highly volatile or
  must-be-exact data (e.g. a bank balance) without care.

## Trade-offs to discuss
- Caching trades **freshness for speed and DB load** — name the staleness window (your TTL).
- Hot keys can overload a single Redis node; mitigate with replicas or key-splitting.
- A cache is an optimization, not a source of truth — the system must still be correct if the cache is empty.
