# System Design by Building — 31 Projects in .NET + Docker

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-cache-DC382D?logo=redis&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-MassTransit-FF6600?logo=rabbitmq&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-capstones-DD0031?logo=angular&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)

> A self-paced portfolio where I learn distributed-systems building blocks by **building 31 small, real-world projects**. Each one teaches a single concept (caching, queues, sharding, …), maps to a **real system-design interview question**, and runs locally in **Docker** — then becomes upgradable to the cloud.

**This is a learning journey, built in public.** Early projects are deliberately small 2–4 hour spikes; later ones are full applications. Depth and completeness vary by design — the [roadmap](ROADMAP.md) tracks status.

## Why this exists

I'm an app developer leveling up into system design. Rather than read about Redis or message brokers, I build with each one — learning *what it is, where it fits, and how to wire it up* — and write a small senior-style plan before each project. Doing many small projects is how I'm building the muscle gradually.

## How it's organized

- **📍 [ROADMAP.md](ROADMAP.md)** — the full curriculum: 31 projects across 6 phases, each mapped to the interview question it answers, plus a coverage map and suggested cadence.
- **📖 [CONCEPTS.md](CONCEPTS.md)** — a glossary of every building block + the architecture patterns (DDD, Clean Architecture, CQRS, Event Sourcing) with a "where do I apply this" cheat-sheet.
- Each project folder is a **self-contained lesson**. Projects **00–09** ship full guides (`README` · `PLAN` · `CONCEPT` · `INTERVIEW`); projects **10–30** are concise stubs that grow as I reach them.

## The journey (6 phases)

> Full per-project tables + interview-question coverage map are in **[ROADMAP.md](ROADMAP.md)**.

| Phase | Focus | Projects |
|---|---|---|
| **0 — Foundations** | Docker, Compose, gateway — *how it all connects* | 00–01 |
| **1 — Building blocks** | Cache, rate limiter, broker, outbox, search, storage, IDs | 02–09 |
| **2 — Real apps** | URL shortener, chat, feed, e-commerce (DDD/CQRS), booking | 10–18 |
| **3 — Scale** | Replication, sharding, load balancing, sagas, event sourcing | 19–24 |
| **4 — Capstones** | Video streaming, collaborative editor, payments, observability | 25–28 |
| **5 — Cloud** | Deploy to Azure, Infrastructure-as-Code, autoscaling | 29–30 |

## Building blocks covered

Caching (Redis) · Rate limiting · Message queues (RabbitMQ/MassTransit) · Outbox & idempotency · Object storage (S3/MinIO) · Full-text search (Elasticsearch) · Real-time (SignalR/WebSockets) · Replication · Sharding & consistent hashing · Load balancing · Sagas / distributed transactions · Event sourcing · Geospatial indexing · Observability (OpenTelemetry) · **DDD / Clean Architecture / CQRS**

## Tech stack

.NET 9 · ASP.NET Core · EF Core · PostgreSQL · Redis · RabbitMQ · MinIO · Elasticsearch · Angular (capstones) · Docker & Compose · xUnit + Testcontainers · Azure (cloud phase)

## Running a project

Every project is self-contained and runs the same way:

```bash
cd 00-connected-hello-world
# create a local .env from the template at the repo root, then fill in values:
cp ../.env.example .env
docker compose up
```

**Prerequisites:** Docker, .NET 9 SDK (Node 22 for the Angular capstones). Each project's `README.md` and `PLAN.md` cover the specifics and a runnable *Definition of Done*.

## A note on secrets

No credentials live in this repo. Copy [`.env.example`](.env.example) to a `.env` (which is git-ignored) for local values; cloud secrets use Azure Key Vault in Phase 5. Never commit a real `.env`.

---

*Built one building block at a time. Follow along in the [roadmap](ROADMAP.md).*
