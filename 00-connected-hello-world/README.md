# 00 · Connected Hello World

A minimal but **complete** connected system: a .NET 8 minimal API that persists notes to
**PostgreSQL**, with structured logging to **Seq** and a **pgAdmin** database GUI — all wired
together and run with a single `docker compose up`.

This is the baseline every backend service starts from: an API that talks to a database, both in
containers, finding each other over a Docker network. The Compose skeleton here is the template
reused across later projects.

> **Building block:** Containerization & service wiring (Compose networking, healthchecks, config, observability)
> **Tier:** Foundation spike · **Time:** ~3–4 hours

---

## Architecture

Everything runs inside one Docker Compose network. The **host** reaches services through
**published ports**; **containers** reach each other by **service name** (Docker DNS).

```
                          host (your machine / browser)
        localhost:8080        localhost:5341        localhost:5050
              │                     │                     │
 ─────────────┼─────────────────────┼─────────────────────┼──────────  docker network
              ▼                     ▼                     ▼
        ┌───────────┐         ┌───────────┐         ┌───────────┐
        │    api    │         │    seq    │         │  pgadmin  │
        │  :8080    │         │   :80     │         │   :80     │
        └─────┬─────┘         └───────────┘         └─────┬─────┘
              │  mydb:5432            ▲                    │ mydb:5432
              │                       │ seq:80 (logs)      │
              ▼                       └────────┬───────────┘
        ┌───────────┐                          │
        │   mydb    │◄─────────────────────────┘
        │  postgres │
        │  :5432    │
        └───────────┘
     volumes: pgdata · seqdata · pgadmindata  (data survives restarts)
```

**Key idea:** inside the network the API connects to Postgres as `mydb:5432` and ships logs to
`seq:80` — never `localhost`. `localhost` only works from *your* machine, via the published ports.

---

## Tech stack

- **.NET 8** minimal API (`SimpleNote`)
- **Entity Framework Core** + **Npgsql** (PostgreSQL provider)
- **PostgreSQL 16**
- **Serilog** → console + **Seq** (structured logging)
- **pgAdmin 4** (database GUI)
- **Docker Compose** (orchestration, healthchecks, networking)

---

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Compose v2)
- That's it — the .NET SDK is only needed if you want to run the API outside Docker.

---

## Quick start

```bash
cd SimpleNote            # the folder with docker-compose.yml
docker compose up --build -d
```

Compose builds the API image, starts Postgres, waits for it to be **healthy**, then starts the API
(which applies EF migrations on startup), plus Seq and pgAdmin. First run takes a bit longer while
Postgres initializes.

Then:

```bash
# create a note
curl -X POST http://localhost:8080/notes \
     -H "Content-Type: application/json" \
     -d '"my first note"'

# read notes back
curl http://localhost:8080/notes

# app + DB health
curl http://localhost:8080/health        # -> Healthy
```

Tear down (keep data): `docker compose down`
Tear down (wipe data/volumes): `docker compose down -v`

---

## Services & ports

| Service   | Image             | Open from host          | Internal address | Purpose                     |
|-----------|-------------------|-------------------------|------------------|-----------------------------|
| `api`     | built from source | http://localhost:8080   | `api:8080`       | the .NET minimal API        |
| `mydb`    | `postgres:16`     | `localhost:5432`        | `mydb:5432`      | PostgreSQL database         |
| `seq`     | `datalust/seq`    | http://localhost:5341   | `seq:80`         | structured log viewer       |
| `pgadmin` | `dpage/pgadmin4`  | http://localhost:5050   | `pgadmin:80`     | database GUI                |

---

## API endpoints

| Method | Path      | Description                          |
|--------|-----------|--------------------------------------|
| `GET`  | `/`       | returns `Hello World!`               |
| `GET`  | `/notes`  | lists all notes                      |
| `POST` | `/notes`  | creates a note (JSON string body)    |
| `GET`  | `/health` | health check (app + DB) → `Healthy`  |

> Swagger is scaffolded but currently commented out in `Program.cs`. Uncomment the `UseSwagger` /
> `UseSwaggerUI` block to enable the UI at `/swagger`.

---

## Observability — Seq

Logs are structured via Serilog and shipped to Seq. Open **http://localhost:5341** and you'll see
each request as a queryable event (e.g. `POST /notes` carries a `Note` property you can filter on).
The API also logs to the console, viewable with:

```bash
docker compose logs -f api
```

---

## Database GUI — pgAdmin

Open **http://localhost:5050** and log in (`admin@admin.com` / `admin` by default). Then register
the server:

- **Host:** `mydb`  ← the service name, **not** `localhost`
- **Port:** `5432`
- **Username / Password:** `postgres` / `postgres`
- **Database:** `firstdb`

Browse to `firstdb → Schemas → public → Tables → Notes`, or query with:

```sql
SELECT * FROM "Notes";
```

---

## Configuration

Defaults live in `appsettings.json`; Compose overrides them per environment via environment
variables (double underscore `__` maps to nested config keys). Values are parameterized in
`docker-compose.yml` with `${VAR:-default}`, so the stack **runs out of the box with zero setup**.

| Setting                              | Default                       | Override (env var)                     |
|--------------------------------------|-------------------------------|----------------------------------------|
| DB connection                        | `Server=mydb;…;Port=5432`     | `ConnectionStrings__DefaultConnection` |
| Seq server URL (where the API sends) | `http://seq:80`               | `Seq__ServerUrl`                       |
| Postgres user / password / db        | `postgres` / `postgres` / `firstdb` | `POSTGRES_USER` etc.             |
| pgAdmin login                        | `admin@admin.com` / `admin`   | `PGADMIN_EMAIL` / `PGADMIN_PASSWORD`   |

To override locally, copy `.env.example` → `.env` and edit. `.env` is gitignored (real values stay
off the repo); `.env.example` is committed as the template.

---

## How it works (the interesting bits)

- **Service discovery:** Compose puts every service on a shared network and registers each service
  name as a DNS hostname — that's why the API uses `mydb`, not `localhost`.
- **Startup ordering:** the API `depends_on` Postgres with `condition: service_healthy`, and
  Postgres has a `pg_isready` **healthcheck**, so the API only starts once the DB actually accepts
  connections.
- **Migrations on boot:** on startup the API calls `db.Database.Migrate()`, so a fresh database gets
  the `Notes` table automatically.
- **Persistence:** named volumes (`pgdata`, `seqdata`, `pgadmindata`) keep data across
  `docker compose down` / `up`.

---

## Project structure

```
00-connected-hello-world/
├── README.md                  ← you are here
├── CONCEPT.md                 ← what containers/Compose are
├── PLAN.md                    ← milestones + initial→improve checklist
├── INTERVIEW.md               ← how to talk about it
└── SimpleNote/
    ├── docker-compose.yml     ← the whole stack
    └── SimpleNote/
        ├── Program.cs         ← endpoints, EF, Serilog, health checks
        ├── AppDbContext.cs
        ├── appsettings.json   ← default config
        ├── Dockerfile         ← multi-stage build
        └── .dockerignore
```

---

## Learn more

1. [`CONCEPT.md`](CONCEPT.md) — what containers/Compose are and how the wiring works.
2. [`PLAN.md`](PLAN.md) — milestones and the initial→improve checklist.
3. [`INTERVIEW.md`](INTERVIEW.md) — how to talk about this in an interview.
