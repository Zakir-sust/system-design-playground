# 21 · Load Balancing N Instances

**Building block:** Horizontal scaling of stateless services, LB strategies, sticky sessions, state externalization.
**Tier:** Scale · **Time:** ~half day · **Interview question:** **"Load balancing / scale stateless services."**

> 📄 **STUB** — expand using projects 00–09 as the template. Extends the gateway from project 01.

## What you'll build
Run **multiple replicas** of one API behind a load balancer (YARP/nginx) and make the system correct and
faster as you scale out — which forces services to be **stateless** (shared state in Redis/DB).

## Builds on
01 (reverse proxy/gateway), 02 (Redis for shared state), 03 (distributed rate-limit counters), 09 (single-run jobs).

## Initial → Improve
**Initial (M1):** 1 API instance behind the gateway. Baseline throughput.
**Improvements:**
- Scale to **3 replicas** (`docker compose up --scale`); balance with **round-robin** then **least-connections**.
- **Health checks** so the LB stops routing to a dead/unhealthy instance.
- Break in-memory state: move sessions/counters/cache to **Redis** (this is why earlier projects externalized state).
- **Sticky sessions** — what they are, why to avoid needing them.
- Load-test (k6/bombardier) and watch latency/throughput change with replica count.

## Key questions this answers
- How do you scale a service horizontally, and what must be true for it? (statelessness)
- Round-robin vs least-connections vs hashing — when each?
- How do health checks prevent routing to dead instances?
- Why are sticky sessions a smell, and how do you avoid them? (externalize state)
