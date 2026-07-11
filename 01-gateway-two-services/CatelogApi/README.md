# Gateway & Two Services — Implementation

A YARP reverse-proxy gateway in front of two minimal APIs (`catelog`, `orders`), all running in Docker Compose. Clients enter through the gateway only; the services also talk to each other directly over the Compose network.

```
                        ┌─────────────────────── docker network ───────────────────────┐
                        │                                                              │
  catelog.localhost ────┤                      ┌──> catelog:8080  ──┐                  │
                :8080 ──┼──>  gateway (YARP)   │    GET /ping       │  east-west call  │
   orders.localhost ────┤     host-based       │    GET /with-orders┘──> orders:8080   │
                        │     routing          └──> orders:8080         GET /ping      │
                        │                           GET /ping                          │
                        └──────────────────────────────────────────────────────────────┘
```

## Services

| Service   | Container address | Published port | Purpose |
|-----------|-------------------|----------------|---------|
| `gateway` | `gateway:8080`    | **8080**       | YARP reverse proxy — the only intended entry point |
| `catelog` | `catelog:8080`    | 8081 (debug)   | Catalog API; also calls `orders` internally |
| `orders`  | `orders:8080`     | 8082 (debug)   | Orders API |

Ports 8081/8082 are published only for poking at the services directly while learning; in a real setup only the gateway would be exposed.

## Run it

```bash
docker compose up --build
```

Then, through the gateway (anything ending in `.localhost` resolves to `127.0.0.1` automatically):

```bash
curl http://catelog.localhost:8080/ping          # -> "Catelog API"
curl http://orders.localhost:8080/ping           # -> "Order API"
curl http://catelog.localhost:8080/with-orders   # catelog calls orders internally -> "Order API"
```

Equivalent without the `.localhost` names — proves routing is driven by the `Host` header:

```bash
curl -H "Host: orders.localhost" http://localhost:8080/ping
```

## How routing works

The gateway is configured entirely in `Gateway/appsettings.json` (`ReverseProxy` section — YARP's `LoadFromConfig`):

- **Routes** match on `Hosts` (`catelog.localhost` / `orders.localhost`) with a catch-all path `{**rest}`, and point at a cluster.
- **Clusters** hold the destination addresses, using Compose service names as DNS (`http://catelog:8080`).

This started as **path-based** routing (`/catelog/{**rest}` + a `PathRemovePrefix` transform so backends could own clean routes like `/ping`). It then moved to **host-based** routing, where the prefix problem disappears entirely: the path is forwarded untouched, and the hostname carries the "which service" information — the same model as Kubernetes Ingress or `api.company.com` vs `admin.company.com`.

Things learned the hard way:

- Conditions inside a route's `Match` are **AND**ed. Host + `/catelog/{**rest}` means a request to `catelog.localhost/ping` matches *neither* and 404s at the gateway.
- YARP path values (e.g. `PathRemovePrefix`) must start with `/`.
- Config is baked into the image — editing `appsettings.json` requires `docker compose up --build gateway`, not just a restart.

## Health checks

Each service exposes `GET /health` (ASP.NET health checks). The gateway runs **active health probes** against them (see `HealthCheck.Active` on each cluster): every 5 s it probes `/health`; after consecutive failures the destination is pulled out of rotation and clients get an **instant 503** instead of a slow connection error. No extra code in the gateway — `AddReverseProxy()` wires the prober up; the config block switches it on.

Watch it work:

```bash
docker compose stop orders     # gateway logs probe failures ("Name or service not known"
                               # — Docker DNS drops stopped containers entirely)
curl -i http://orders.localhost:8080/ping    # immediate 503, gateway never dials out
docker compose start orders    # a probe interval later, traffic flows again — self-healed
```

## Service-to-service (east-west) call

`GET /with-orders` on `catelog` calls `orders` via a named `HttpClient` (`IHttpClientFactory`), base address `http://orders:8080` from `OrdersBaseUrl` in config. Two things this demonstrates:

- Internal calls go **direct**, not through the gateway — so they use container DNS names and the services' *own* routes. Gateway concerns (host names, prefixes) don't exist on this path.
- The base URL differs per environment: `http://orders:8080` in Compose, `http://localhost:5002` in local development (`appsettings.Development.json`).

## Gateway vs load balancer (the interview answer)

This gateway routes **different** requests to **different** services (routing by host/path). A load balancer spreads **identical** requests across **replicas of the same** service. YARP does both — the `Destinations` map under each cluster can hold many replicas — but with one destination per cluster, this project exercises only the gateway role. Load balancing proper is project 21.

## Not done (yet)

From the improvement list in `../PLAN.md`: Polly resilience on the east-west call (timeout + retry) and a correlation-ID header forwarded through the gateway.
