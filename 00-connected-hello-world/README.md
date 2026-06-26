# 00 · Connected Hello World

**Building block:** Containerization & "how services connect" (Docker Compose networking, healthchecks, config).
**Real scenario:** The baseline every backend service starts from — an API that talks to a database, both in containers.
**Time:** ~3–4 hours · **Tier:** Foundation spike

## Why this is project #1

You said you're weak on deployment and *visualizing how everything is connected*. This project fixes
that head-on. You'll build the smallest possible **connected** system — one API + one database — and
run it entirely in Docker Compose. The compose skeleton you write here becomes the template you copy
into **every later project**, so the time you invest now pays back 30 times.

## What you'll build

A .NET 9 minimal API with one entity (say, `Note`) persisted to **PostgreSQL**, with both the API and
the database running as Docker containers, wired together by Compose. By the end you can explain — and
draw — exactly how a request flows from your browser to the API container to the database container.

## What you'll be able to answer afterwards

- "Walk me through how your services are deployed and how they find each other."
- "What's the difference between an image and a container? Why Compose?"
- "How does config/secrets get into a container? How do you health-check a dependency?"

## Definition of Done (quick version — full one in `PLAN.md`)

`docker compose up` →
1. API reachable at `http://localhost:8080` with Swagger listing endpoints.
2. `POST /notes` inserts a row; `GET /notes` returns it (data survives because Postgres has a volume).
3. Restarting the API container doesn't lose data.
4. Logs are visible (`docker compose logs -f api`).

## Read in this order

1. [`CONCEPT.md`](CONCEPT.md) — what containers/Compose are and how the wiring works.
2. [`PLAN.md`](PLAN.md) — your milestones and the initial→improve checklist.
3. [`INTERVIEW.md`](INTERVIEW.md) — how to talk about it.
