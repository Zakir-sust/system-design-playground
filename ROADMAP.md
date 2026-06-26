# System Design Roadmap — 31 Projects (.NET + Docker)

A self-paced curriculum that takes you from "I mostly fix bugs" to "I can design and reason about
real distributed systems." Each project teaches **one building block** (or part of one), maps to a
**real system-design interview question**, and runs **locally in Docker** first — so you can *see how
everything connects* — then becomes **upgradable to the cloud**.

> You write all the code yourself. These docs are your **plan, primer, and interview prep** — not the
> implementation. That's the point: the wiring (Redis connection, MassTransit consumer, EF Core context)
> is where the learning lives.

---

## How to use this repo

Each project folder is a self-contained lesson. Work them **in order** (lower number first) — later
projects reuse blocks you built earlier.

**Per-project workflow (mimics how a senior engineer approaches a ticket):**

1. **`CONCEPT.md`** — understand the building block first: *what is it, why does it exist, where does it fit?*
2. **`PLAN.md`** — read the senior-style mini-plan; extend it with your own notes before touching code.
3. **Build M1 (the initial version)** — the simplest thing that works.
4. **Verify in Docker** — `docker compose up`, hit the endpoints, watch the logs. Every `PLAN.md` ends with a runnable *Definition of Done*.
5. **Iterate the improvements** — each `PLAN.md` lists *initial → improve* steps. This is the "now make it better" loop.
6. **`INTERVIEW.md`** — review the trade-offs out loud as if in an interview. If you can explain it, you've learned it.

**Two root references you'll reuse constantly:**
- [`CONCEPTS.md`](CONCEPTS.md) — glossary of every building block + the architecture patterns (DDD, Clean Architecture, CQRS, Event Sourcing), each with a "where do I apply this" note.

---

## Suggested cadence (so ~30 stays realistic)

| Tier | What | Time each | Notes |
|---|---|---|---|
| Spikes (Phase 0–1) | Wire up one concept | 2–4 hours | Short, do in a break |
| Real apps (Phase 2) | Combine blocks into a product | 1–2 days | The presentable ones |
| Scale (Phase 3) | Take an app to scale | 0.5–1 day | Often extends a Phase-2 app |
| Capstones (Phase 4) | Broad, multi-block | 3–5 days | Portfolio pieces |
| Cloud (Phase 5) | Ship to Azure | 1–2 days | Free tier |

**You can stop after any phase** and still have learned something coherent. Don't treat all 31 as equal weight.

**Legend:** 📘 DEEP = full guide set (README · PLAN · CONCEPT · INTERVIEW) · 📄 STUB = one-page README (expand when you reach it) · 🖥️ = gets an Angular UI · ★ = introduces testing.

---

## Phase 0 — Foundations: containers & "how it all connects"
*Goal: kill the "I can't visualize how services connect" weakness before anything else.*

| # | Project | Building block | Interview question |
|---|---|---|---|
| 00 📘 | [connected-hello-world](00-connected-hello-world/) | Containerization, compose networking, healthchecks | (foundation) |
| 01 📘 | [gateway-two-services](01-gateway-two-services/) | Reverse proxy / API gateway, service-to-service calls | "How are requests routed? What's an API gateway?" |

## Phase 1 — Building-block spikes (2–4 h each)
*Goal: learn each block in isolation — what it is, where it goes, how to connect it.*

| # | Project | Building block | Interview question |
|---|---|---|---|
| 02 📘★ | [redis-cache-aside](02-redis-cache-aside/) | Caching (cache-aside, TTL) **+ first tests** | "Design a distributed cache" |
| 03 📘 | [rate-limiter](03-rate-limiter/) | Rate limiting (token bucket, sliding window) | **"Design a rate limiter"** |
| 04 📘 | [message-broker](04-message-broker/) | Message queue (RabbitMQ + MassTransit) | "Message queue / pub-sub" |
| 05 📘★ | [outbox-idempotency](05-outbox-idempotency/) | Reliable messaging, idempotency **+ integration tests** | "Idempotency / exactly-once" |
| 06 📘 | [snowflake-ids](06-snowflake-ids/) | Unique ID generation | **"Design a unique ID generator"** |
| 07 📘 | [blob-storage](07-blob-storage/) | Object storage (MinIO / S3-compatible) | "Design S3-like storage" |
| 08 📘 | [full-text-search](08-full-text-search/) | Search index (Elasticsearch) | "Search system" |
| 09 📘 | [jobs-and-pubsub](09-jobs-and-pubsub/) | Background jobs + fan-out pub/sub | "Distributed scheduler" |

## Phase 2 — Real-life apps (combine blocks; each has initial → improve)
*Goal: build presentable products that stitch the blocks together.*

| # | Project | Building block | Interview question |
|---|---|---|---|
| 10 📄 | [url-shortener](10-url-shortener/) ⭐ | Base62, KV store, cache, analytics | **"Design a URL shortener"** |
| 11 📄 | [pastebin](11-pastebin/) | Blob storage, expiration, access control | **"Design Pastebin"** |
| 12 📄 | [notification-service](12-notification-service/) | Queues, fan-out, provider abstraction | **"Design a notification system"** |
| 13 📄 | [image-service](13-image-service/) | Blob storage + async processing + CDN | **"Design Instagram"** (storage) |
| 14 📄🖥️ | [chat-app](14-chat-app/) | WebSockets/SignalR, presence, Redis backplane | **"Design a chat system"** |
| 15 📄 | [news-feed](15-news-feed/) | Fan-out on write vs read, feed cache | **"Design a news feed / Twitter"** |
| 16 📄🖥️ | [ecommerce-orders](16-ecommerce-orders/) | **DDD + Clean Architecture + CQRS** | "Design e-commerce / Amazon" |
| 17 📄 | [booking-system](17-booking-system/) | Concurrency, locking, idempotency | **"Design hotel reservation / Ticketmaster"** |
| 18 📄🖥️ | [autocomplete](18-autocomplete/) | Trie/prefix index, top-k ranking | **"Design search autocomplete"** |

## Phase 3 — Scale & distributed concerns
*Goal: take an app from Phase 2 and make it scale.*

| # | Project | Building block | Interview question |
|---|---|---|---|
| 19 📄 | [read-replicas](19-read-replicas/) | Replication, read/write split | "Scale database reads" |
| 20 📄 | [sharding](20-sharding/) | Partitioning, consistent hashing | **"Sharding / key-value store"** |
| 21 📄 | [load-balancing](21-load-balancing/) | N stateless replicas behind a LB | **"Load balancing / scale stateless"** |
| 22 📄 | [saga-orchestration](22-saga-orchestration/) | Distributed transactions (MassTransit Saga) | "Microservice consistency" |
| 23 📄 | [event-sourcing](23-event-sourcing/) | Event store, projections, snapshots | "Event sourcing" |
| 24 📄 | [proximity-service](24-proximity-service/) | Geohashing, spatial index | **"Design Yelp / Uber proximity"** |

## Phase 4 — Broad capstones (portfolio pieces)
*Goal: large, presentable systems that combine many blocks.*

| # | Project | Building block | Interview question |
|---|---|---|---|
| 25 📄🖥️ | [video-streaming](25-video-streaming/) | Transcoding pipeline, HLS, CDN | **"Design YouTube/Netflix"** |
| 26 📄🖥️ | [collaborative-editor](26-collaborative-editor/) | CRDT/OT, real-time conflict resolution | **"Design Google Docs"** |
| 27 📄 | [payment-ledger](27-payment-ledger/) | Idempotency, double-entry ledger, state machine | **"Design a payment system"** |
| 28 📄 | [observability-stack](28-observability-stack/) | OpenTelemetry, Prometheus, Grafana tracing | "Monitoring / metrics system" |

## Phase 5 — Cloud (Azure-first, free)
*Goal: map every local building block onto a managed cloud service.*

| # | Project | Building block | Interview question |
|---|---|---|---|
| 29 📄 | [deploy-to-azure](29-deploy-to-azure/) | Container Apps, managed Postgres/Redis/Blob, CI/CD | "How would you deploy & operate this?" |
| 30 📄 | [cloud-scale-iac](30-cloud-scale-iac/) *(stretch)* | IaC (Bicep/Terraform), autoscale, CDN, AWS compare | "Multi-cloud / reproducible infra" |

---

## Interview-question coverage map

Every commonly-reported system-design question, and where you practice it:

- **URL shortener** → 10 · **Rate limiter** → 03 · **Distributed cache** → 02 · **Pastebin** → 11
- **Unique ID generator** → 06 · **Notification system** → 12 · **Pub/sub & message queue** → 04, 09
- **Chat system** → 14 · **News feed / Twitter** → 15 · **Search autocomplete** → 18 · **Search system** → 08
- **YouTube / video** → 25 · **Dropbox / file storage** → 07, 13 · **Instagram** → 13, 15
- **Uber / Yelp proximity** → 24 · **Hotel / ticket booking** → 17 · **Google Docs** → 26
- **Payment system** → 27 · **S3 object storage** → 07 · **Kafka-like queue** → 04, 09
- **Sharding / partitioning** → 20 · **Replication** → 19 · **Load balancing** → 21
- **Distributed transactions / saga** → 22 · **Event sourcing** → 23 · **Monitoring/observability** → 28

---

## Progress tracker

Phase 0: ☐ 00 ☐ 01
Phase 1: ☐ 02 ☐ 03 ☐ 04 ☐ 05 ☐ 06 ☐ 07 ☐ 08 ☐ 09
Phase 2: ☐ 10 ☐ 11 ☐ 12 ☐ 13 ☐ 14 ☐ 15 ☐ 16 ☐ 17 ☐ 18
Phase 3: ☐ 19 ☐ 20 ☐ 21 ☐ 22 ☐ 23 ☐ 24
Phase 4: ☐ 25 ☐ 26 ☐ 27 ☐ 28
Phase 5: ☐ 29 ☐ 30

---

## Prerequisites (already verified on your machine)

- **.NET 9** (`dotnet --version` → 9.0.x) · **Docker** (28.x) · **Node** (22.x, for Angular capstones)
- Recommended VS Code / Rider extensions: C# Dev Kit, Docker, REST Client (or use Swagger UI).
- A folder `00-connected-hello-world` is where you start. Read its `CONCEPT.md` first.
