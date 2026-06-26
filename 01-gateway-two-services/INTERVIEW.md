# Interview angle — 01 · Gateway & Two Services

## Questions this prepares you for

- **"What is an API gateway and why use one?"**
  → Single entry point; hides internal topology from clients; centralizes cross-cutting concerns (auth,
  TLS, rate limiting, logging); lets you evolve services without breaking clients.

- **"Gateway vs load balancer?"**
  → Gateway routes to *different* services by path/host and does cross-cutting work; a load balancer spreads
  traffic across *identical* replicas of one service. They're often layered (gateway in front, LB behind, or
  the gateway itself balancing a cluster).

- **"How do microservices communicate?"**
  → Synchronously over HTTP/gRPC (east-west), or asynchronously via a message broker (projects 04, 22).
  Sync is simple but couples availability; async decouples but adds eventual consistency.

- **"What goes wrong with service-to-service calls?"**
  → Cascading failures (a slow dependency hangs callers), retries amplifying load, chatty fan-out latency.
  Mitigate with timeouts, retries with backoff, circuit breakers (Polly), and bulkheads.

## Trade-offs to discuss
- Gateways add a hop (latency) and become a critical path — they must be highly available themselves.
- Sync calls are easy to reason about but create runtime coupling; prefer async (events) for non-critical-path work.
- A correlation ID set at the gateway and propagated downstream is the seed of distributed tracing (project 28).
