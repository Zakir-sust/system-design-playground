# Plan — 00 · Connected Hello World

## Goal
Run a .NET 9 API + PostgreSQL together in Docker Compose, persisting one entity, and be able to explain
the full request→API→DB path.

## Scope
**In:** one minimal API, one entity + EF Core, Postgres container, Compose networking, healthchecks, basic logging.
**Out (later projects):** caching, queues, auth, multiple services, cloud.

## Milestones

> Do these in order. Don't jump ahead — the point is to feel each layer connect.

- [ ] **M1 — API runs on the host (no Docker yet).**
  `dotnet new webapi`, add one in-memory endpoint, confirm Swagger at `https://localhost:xxxx/swagger`.
  *You're proving the app works before adding infrastructure.*

- [ ] **M2 — Add Postgres + EF Core, still running the API on the host.**
  Run only Postgres in Docker (`docker run` or a one-service compose). Add `Npgsql.EntityFrameworkCore.PostgreSQL`,
  a `DbContext`, a `Note` entity, a migration. Connect using `localhost:5432`. Persist + read a row.
  *This is the "redis/db connection" wiring you wanted to do by hand — do it slowly and understand each piece.*

- [ ] **M3 — Containerize the API and put both in one Compose file.**
  Write a `Dockerfile` (multi-stage) for the API. Now the API talks to Postgres using the **service name**
  (`Host=db`), not `localhost`. Understand *why* (Compose DNS).

- [ ] **M4 — Make startup robust.**
  Add a healthcheck to Postgres and `depends_on: condition: service_healthy` so the API waits. Apply EF
  migrations on startup (and understand the race if you don't wait).

## Initial → Improve

**Initial version (stop here for the core lesson):**
- [ ] API + Postgres in Compose, one entity, data persists across restarts.

**Improvements (each is a small, optional upgrade):**
- [ ] **Structured logging** — add Serilog, ship logs to a **Seq** container; view them at `http://localhost:5341`. (This is your first taste of observability — project 28 expands it.)
- [ ] **`.env` file** for connection strings / passwords; reference with `${VAR}` in Compose. Add `.env` to `.gitignore`.
- [ ] **pgAdmin or adminer** container so you can browse the DB in a GUI.
- [ ] **Healthcheck endpoint** on the API (`/health`) using `AspNetCore.HealthChecks`.
- [ ] **`.dockerignore`** + smaller multi-stage image; compare image size before/after.

## Definition of Done
- [ ] `docker compose up -d` brings up `api` + `db` (+ optional `seq`, `pgadmin`).
- [ ] Swagger at `http://localhost:8080/swagger` lists the endpoints.
- [ ] `POST /notes` → 201; `GET /notes` returns the note.
- [ ] `docker compose down && up` keeps the data (named volume on Postgres).
- [ ] You can sketch the topology from memory: browser → api container :8080 → db container :5432, same Compose network.
