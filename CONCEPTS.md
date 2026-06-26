# Concepts — Building Blocks & Architecture Patterns

Your reference glossary. Before each project, read the relevant entry here, then the project's own
`CONCEPT.md` (which goes deeper and shows the .NET wiring). Each entry answers the three questions you
asked: **what is it, where do I apply it, how do I connect it.**

---

## Part A — System design building blocks

### 1. Load balancer
**What:** A component that spreads incoming requests across multiple identical instances of a service.
**Where:** In front of any service you run more than one copy of. It's how you scale *stateless* services horizontally.
**How (local):** nginx or YARP container in `docker-compose`, with N replicas of your API behind it.
**Key ideas:** round-robin vs least-connections, health checks (stop sending traffic to dead instances), sticky sessions (and why you usually want to *avoid* needing them by keeping services stateless).
**Practiced in:** 01 (intro), **21** (the real lesson).

### 2. Caching (Redis)
**What:** A fast in-memory store (Redis) holding a copy of data that's expensive to recompute or fetch.
**Where:** In front of read-heavy database queries, for session state, rate-limit counters, leaderboards.
**How:** `StackExchange.Redis` client → Redis container. Pattern: **cache-aside** (look in cache; on miss, read DB and populate cache with a TTL).
**Key ideas:** TTL/expiry, eviction (LRU/LFU), invalidation (the hard part), cache stampede, write-through vs write-behind.
**Practiced in:** **02** (cache-aside), 03 (counters), 10/15/18 (real use), 19 (with replicas).

### 3. CDN (Content Delivery Network)
**What:** Geographically distributed edge caches for static assets (images, video, JS).
**Where:** Serving files/media to users far from your origin.
**How (local):** simulate with a caching reverse proxy; in cloud it's Azure Front Door / CloudFront.
**Key ideas:** edge caching, cache invalidation, origin pull.
**Practiced in:** 13, 25 (concept), 30 (real, cloud).

### 4. Database replication
**What:** Keeping copies (replicas) of your database in sync with a primary.
**Where:** When reads vastly outnumber writes — send reads to replicas, writes to the primary.
**How:** Postgres primary + replica containers; route read queries to the replica connection string.
**Key ideas:** read/write split, **replication lag** (replicas are slightly behind → stale reads), failover.
**Practiced in:** **19**.

### 5. Sharding / partitioning
**What:** Splitting one large dataset across multiple databases ("shards"), each holding a slice.
**Where:** When a single DB can't hold the data or handle the write load.
**How:** choose a **shard key**, route each row to a shard (often via **consistent hashing**).
**Key ideas:** shard key choice, hot shards, resharding pain, cross-shard queries are expensive.
**Practiced in:** **20**.

### 6. Consistent hashing
**What:** A hashing scheme that minimizes how much data moves when you add/remove a node.
**Where:** Distributed caches and sharded databases — deciding which node owns a key.
**Key ideas:** hash ring, virtual nodes, why plain `hash(key) % N` is bad (everything reshuffles when N changes).
**Practiced in:** 20 (sharding), referenced in 02.

### 7. Message queue & pub/sub (RabbitMQ, MassTransit)
**What:** A broker that lets services communicate **asynchronously**. *Queue* = one consumer processes each message; *pub/sub* = many subscribers each get a copy.
**Where:** Decouple slow/optional work from the request path (send email, generate thumbnail, update analytics). Smooth out write spikes.
**How:** RabbitMQ container + **MassTransit** (the .NET abstraction) to publish and consume.
**Key ideas:** producer/consumer, exchanges/topics, acknowledgements, retries, **dead-letter queue**, consumer concurrency.
**Practiced in:** **04** (basics), 09 (fan-out), 12/13 (real use), 22 (saga).

### 8. Outbox pattern & idempotency
**What:** **Outbox** = save your DB change and the "event to publish" in the *same transaction*, then a relay publishes the event — avoiding the "saved to DB but broker call failed" dual-write bug. **Idempotency** = processing the same message twice has the same effect as once.
**Where:** Any time a state change must reliably produce an event.
**Key ideas:** dual-write problem, outbox table + relay, idempotency keys, dedup on the consumer.
**Practiced in:** **05**, reused in 10, 16, 22, 27.

### 9. Rate limiting
**What:** Capping how many requests a client can make in a time window.
**Where:** Public APIs, login endpoints, anything abuse-prone.
**How:** counters in Redis (shared across instances); return HTTP **429**.
**Key ideas:** fixed window, sliding window, **token bucket**, leaky bucket; atomic increments (Lua) to avoid races.
**Practiced in:** **03**, reused in 10.

### 10. Object / blob storage
**What:** Storage for large unstructured files (images, video, backups) addressed by key — not a database.
**Where:** User uploads, media, anything you shouldn't put in a relational DB.
**How:** **MinIO** (S3-compatible) container; in cloud it's Azure Blob / AWS S3.
**Key ideas:** buckets, keys, **presigned URLs** (let clients upload/download directly), lifecycle/expiry, store *metadata* in your DB and the *bytes* in blob storage.
**Practiced in:** **07**, reused in 11, 13, 25.

### 11. Search index (Elasticsearch / OpenSearch)
**What:** A specialized store built for fast text search and ranking, using an **inverted index**.
**Where:** "Search products/docs/messages", autocomplete, filtering/faceting.
**How:** Elasticsearch container; index documents, query with relevance scoring; keep it in sync with your DB via events.
**Key ideas:** inverted index, analyzers/tokenization, relevance scoring, eventual consistency with the source DB.
**Practiced in:** **08**, reused in 18.

### 12. WebSockets / real-time (SignalR)
**What:** A persistent two-way connection so the server can *push* to clients (vs clients polling).
**Where:** Chat, live notifications, collaborative editing, dashboards, presence.
**How:** **SignalR** (ASP.NET's real-time library); to scale across instances use a **Redis backplane**.
**Key ideas:** connection state, presence/heartbeats, message ordering, fan-out, backplane for multi-instance.
**Practiced in:** **14**, reused in 26.

### 13. Background jobs & scheduling
**What:** Work that runs outside the request — immediately (queued) or on a schedule (cron).
**Where:** Emails, cleanup, periodic aggregation, retries.
**How:** Hangfire or Quartz.NET; or a queue + worker consumer.
**Key ideas:** recurring vs fire-and-forget, retries/backoff, idempotency, visibility/monitoring.
**Practiced in:** **09**, reused in 12, 27.

### 14. CAP theorem & consistency
**What:** Under a network **P**artition you must choose **C**onsistency or **A**vailability. More practically: how fresh must reads be?
**Where:** Every distributed-data decision (replicas, caches, sharding).
**Key ideas:** strong vs **eventual consistency**, read-your-writes, why caches and replicas trade freshness for speed.
**Referenced in:** 02, 15, 19, 23 (and discussed in most `INTERVIEW.md`).

### 15. Geospatial indexing
**What:** Indexing locations so "find things near me" is fast.
**Where:** Ride-sharing, food delivery, "nearby" features.
**How:** **geohashing** or H3 to bucket coordinates; Redis geo commands or a spatial index.
**Key ideas:** geohash precision, nearest-neighbor, live location updates.
**Practiced in:** **24**.

### 16. Observability (logs, metrics, traces)
**What:** Seeing what your system is doing: structured **logs**, **metrics** (numbers over time), **traces** (one request across many services).
**Where:** Everywhere — but it pays off most once you have multiple services. This is the direct fix for "I can't visualize how it's connected."
**How:** Serilog → Seq (logs, from project 00); OpenTelemetry → Prometheus + Grafana + a tracer (project 28).
**Key ideas:** correlation IDs, distributed tracing, the three pillars, alerting.
**Practiced in:** 00 (logs), **28** (full stack).

---

## Part B — Architecture patterns (the "DDD and what else" question)

You wrote "ddt" — you meant **DDD, Domain-Driven Design**. Here's that and its usual companions. You don't
need these for small spikes; they earn their keep on **complex business domains** (project 16 onward).

### Domain-Driven Design (DDD)
**What:** Model your code around the **business domain**, not around the database. Core tactical pieces:
- **Entity** — has identity over time (an `Order` with an Id).
- **Value object** — defined by its values, no identity (`Money`, `Address`); immutable.
- **Aggregate** — a cluster of entities/value objects with one **root** that guards invariants; you load/save the whole aggregate.
- **Domain event** — "something happened in the domain" (`OrderPlaced`).
- **Repository** — collection-like interface to load/save aggregates.
- **Bounded context** — an explicit boundary where a model and its **ubiquitous language** (shared vocabulary with the business) apply. "Customer" can mean different things in Sales vs Billing contexts.
**When to use:** complex, rules-heavy domains. **When NOT to:** simple CRUD — DDD is overkill there.
**Practiced in:** **16** (orders), 22 (saga across contexts).

### Clean Architecture
**What:** Organize code in concentric layers — **Domain** (entities, rules) → **Application** (use cases) →
**Infrastructure** (DB, broker, external APIs) → **Presentation** (API/UI). Dependencies point **inward**;
the domain knows nothing about the database.
**Why:** business logic stays testable and framework-independent; you can swap infrastructure.
**Practiced in:** **16**.

### CQRS (Command Query Responsibility Segregation)
**What:** Separate the **write** model (commands that change state) from the **read** model (queries).
They can use different shapes, even different databases.
**When:** reads and writes have very different scale/shape; pairs well with event-driven systems.
**Trade-off:** more moving parts; read model is often **eventually consistent** with writes.
**Practiced in:** **16**, 23.

### Event Sourcing
**What:** Instead of storing current state, store the **sequence of events** that produced it. Rebuild
state by replaying events; build read models (**projections**) from the event stream.
**Why:** perfect audit trail, time-travel/debugging, natural fit with CQRS.
**Trade-off:** querying current state needs projections; schema/versioning of events is real work.
**Practiced in:** **23**, concepts reused in 27.

### Saga (distributed transactions)
**What:** A long-running business transaction spanning multiple services, coordinated by events with
**compensating actions** instead of a single ACID transaction (which you can't have across services).
**Where:** "order → payment → inventory → shipping" where any step can fail.
**Key ideas:** orchestration vs choreography, compensation (undo), timeouts, idempotency.
**Practiced in:** **22** (MassTransit state-machine saga).

---

## "Where do I apply X?" cheat-sheet

| Symptom / need | Reach for | Project |
|---|---|---|
| Reads are slow / hammering the DB | Caching (Redis) | 02, 19 |
| Too many requests / abuse | Rate limiting | 03 |
| Slow work blocking the response | Message queue + worker | 04, 09, 12 |
| "Saved but event didn't publish" bugs | Outbox + idempotency | 05 |
| Need IDs unique across servers | Snowflake / unique ID gen | 06 |
| Storing images / files / video | Object storage (MinIO/S3) | 07, 13, 25 |
| "Search" / suggestions | Search index (Elasticsearch) | 08, 18 |
| Live updates / chat / presence | WebSockets (SignalR) | 14, 26 |
| One DB can't handle the writes | Sharding | 20 |
| One DB can't handle the reads | Replication + cache | 19 |
| One server can't handle the traffic | Load balancer + replicas | 21 |
| Multi-step process across services | Saga | 22 |
| Need full audit trail / time-travel | Event sourcing | 23 |
| "Find things near me" | Geospatial index | 24 |
| Complex business rules | DDD + Clean Architecture | 16 |
| "Why is this request slow?" across services | Observability / tracing | 28 |
