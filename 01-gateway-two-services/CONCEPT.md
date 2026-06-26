# Concept — Reverse proxy & API gateway

## What & why

A **reverse proxy** sits in front of your services and forwards client requests to them. An **API gateway**
is a reverse proxy that also does cross-cutting jobs: routing, TLS termination, auth, rate limiting, request
logging. One public entry point instead of exposing every service directly.

**Why you want one:**
- **Single entry point** — clients know one address; they don't need to know your internal topology.
- **Decoupling** — you can split, rename, or move services behind the gateway without changing clients.
- **Cross-cutting concerns in one place** — auth, rate limiting (project 03), TLS, logging.

In .NET the idiomatic gateway is **YARP** (Yet Another Reverse Proxy), configured with **routes** (what to
match) and **clusters** (where to send it).

## Topology

```
   client ──► gateway :8080 ──┬── /catalog/* ──► catalog :8080
   (one door)                 └── /orders/*  ──► orders  :8080
                                                   │  (east-west)
                                                   └── HttpClient ──► catalog :8080
```
North-south = client→gateway→service. East-west = service→service.

## YARP config shape (in the gateway's appsettings)

```jsonc
"ReverseProxy": {
  "Routes": {
    "catalog": { "ClusterId": "catalog", "Match": { "Path": "/catalog/{**rest}" } },
    "orders":  { "ClusterId": "orders",  "Match": { "Path": "/orders/{**rest}" } }
  },
  "Clusters": {
    "catalog": { "Destinations": { "d1": { "Address": "http://catalog:8080" } } },
    "orders":  { "Destinations": { "d1": { "Address": "http://orders:8080" } } }
  }
}
```
Note the destinations use **service names** (`catalog`, `orders`) — Compose DNS again.

## Service-to-service calls (the east-west part)

`orders` calls `catalog` with `HttpClient`. Register it via `IHttpClientFactory` (a **typed client**) with
base address `http://catalog:8080`. `IHttpClientFactory` matters because it pools and reuses connections —
`new HttpClient()` per request leaks sockets.

## Gateway vs load balancer (don't confuse them)

| | API gateway | Load balancer |
|---|---|---|
| Routes to | **different** services by path/host | **identical** replicas of one service |
| Job | front door, cross-cutting concerns | spread load, health-based failover |
| Project | this one (01) | **21** |

In practice a cluster can have *multiple* destinations — then YARP load-balances across them too. That's the
bridge to project 21.

## Pitfalls
- **Exposing services directly** — publish only the gateway's port; keep the rest internal to the network.
- **`new HttpClient()` everywhere** — use `IHttpClientFactory`; otherwise socket exhaustion.
- **No timeout on service calls** — a slow `catalog` will hang `orders`. Add timeouts/retries (Polly).
- **Chatty east-west calls** — one client request fanning into many internal calls kills latency. Watch for it.

## Further reading
- YARP docs (routes, clusters, transforms, health checks).
- `IHttpClientFactory` and typed clients.
- Polly resilience policies (timeout, retry, circuit breaker).
