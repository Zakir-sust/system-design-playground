# Plan — 01 · Gateway & Two Services

## Goal
Put a YARP gateway in front of two services and make one service call the other, all in Compose.

## Scope
**In:** two minimal APIs, one YARP gateway, path-based routing, one service-to-service HTTP call.
**Out:** auth, rate limiting at the gateway (project 03/10), real load balancing across replicas (project 21).

## Milestones

- [ ] **M1 — Two services, reusing the project-00 Compose skeleton.**
  `catalog` and `orders`, each a minimal API with a `GET /ping`. Run both in Compose. Confirm each works on its own port.

- [ ] **M2 — Add the YARP gateway.**
  New service `gateway` (ASP.NET + `Yarp.ReverseProxy`). Configure **routes** (match `/catalog/{**rest}`) and
  **clusters** (destination `http://catalog:8080`). Expose only the gateway's port (8080). Now you reach both
  services through one door.

- [ ] **M3 — Service-to-service call.**
  Add `GET /orders/with-catalog` to `orders` that calls `catalog` via `HttpClient` (base address
  `http://catalog:8080`, registered with `IHttpClientFactory`). Note: this call uses the **service name**, same DNS rule as project 00.

## Initial → Improve

**Initial:** path-based routing through the gateway + one service-to-service call. ✅ core lesson.

**Improvements:**
- [ ] **Path rewrite** — strip the `/catalog` prefix before forwarding (transforms in YARP).
- [ ] **Host-based routing** — route `catalog.localhost` vs `orders.localhost` instead of by path.
- [ ] **Health-based routing** — add `/health` to each service; configure YARP active health checks so it stops routing to an unhealthy destination.
- [ ] **Resilience** — wrap the `orders→catalog` `HttpClient` with **Polly** (timeout + retry). Observe what happens when `catalog` is down with vs without it.
- [ ] **Correlation ID** — generate one at the gateway, forward it as a header, log it in both services (a taste of distributed tracing — project 28).

## Definition of Done
- [ ] Only the gateway port is published; both services are reachable *through* it.
- [ ] `orders` successfully calls `catalog` internally.
- [ ] You can explain the difference between this gateway (routes to *different* services) and a load balancer (spreads load across *identical* replicas).
