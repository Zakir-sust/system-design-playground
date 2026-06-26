# Concept — Containers, Compose & how services connect

## The mental model (read this slowly)

- **Image** = a frozen, read-only snapshot of "an app + everything it needs to run." You build it once.
- **Container** = a running instance of an image. Disposable. If it dies, you start a new one.
- **Volume** = storage that *survives* a container being deleted. Databases need this or you lose data.
- **Compose** = a YAML file describing several containers, their config, and a shared **network** so they
  can talk to each other. `docker compose up` starts them all together.

The single most important idea for you: **inside a Compose network, containers reach each other by their
service name, not `localhost`.** Your API connects to Postgres at `Host=db`, where `db` is the service
name in the compose file. `localhost` inside the API container means *the API container itself*, not your laptop.

## The topology you're building

```
        your browser / curl
                │  http://localhost:8080
                ▼
   ┌─────────────────────────┐        ┌──────────────────────────┐
   │   api  (container)       │  TCP   │   db  (container)         │
   │   ASP.NET :8080          │ ─────► │   Postgres :5432          │
   │   Host=db;Port=5432      │        │   volume: pgdata          │
   └─────────────────────────┘        └──────────────────────────┘
            └──────────── compose network "appnet" ────────────┘
```

## Reference: the Compose shape (infra — fine to use as a template)

You'll write your own, but here's the shape so the *connections* are clear. The C# wiring (DbContext,
endpoints) is **yours to write**.

```yaml
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_USER: app
      POSTGRES_PASSWORD: ${DB_PASSWORD}   # from .env
      POSTGRES_DB: notes
    volumes: [ pgdata:/var/lib/postgresql/data ]   # survives restarts
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U app"]
      interval: 5s
      retries: 5
  api:
    build: ./src/Api
    ports: [ "8080:8080" ]
    environment:
      # NOTE: Host=db (the service name), not localhost
      ConnectionStrings__Default: "Host=db;Port=5432;Database=notes;Username=app;Password=${DB_PASSWORD}"
    depends_on:
      db: { condition: service_healthy }   # wait for the DB to be ready

volumes:
  pgdata:
```

## The C# wiring (you write this — here's *what*, not the full *how*)

1. **Package:** `Npgsql.EntityFrameworkCore.PostgreSQL`.
2. **Config:** read the connection string from configuration. Note the double underscore in
   `ConnectionStrings__Default` — that's how an env var maps to the nested config key `ConnectionStrings:Default`.
   This env-over-appsettings precedence is exactly what makes the same image work locally and in the cloud.
3. **DbContext:** register it in DI pointing at that connection string; define a `Note` entity + `DbSet`.
4. **Migrations:** create one (`dotnet ef migrations add Init`). On startup, apply pending migrations —
   but only *after* the DB is reachable (that's why M4 adds the healthcheck wait).
5. **Endpoints:** minimal-API `POST /notes` and `GET /notes`.

## Common pitfalls (you will hit at least one)

- **`localhost` vs service name** — the #1 beginner trap. From the API container, the DB is `db`, not `localhost`.
- **Applying migrations before Postgres is ready** — without `depends_on: service_healthy`, the API may start first and crash. Healthcheck fixes it.
- **Forgetting the volume** — no volume = your data vanishes on `docker compose down`.
- **Secrets in plain compose** — fine for local; in the cloud (project 29) these move to Key Vault / secrets.
- **Port confusion** — `8080:8080` is `host:container`. You hit the left one from your browser.

## Further reading
- Docker Compose docs → networking & `depends_on`.
- "Configuration in ASP.NET Core" → the env-var → config-key mapping (`__`).
- `Npgsql` EF Core provider docs.
