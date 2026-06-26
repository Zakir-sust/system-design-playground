# 01 · Gateway & Two Services

**Building block:** Reverse proxy / API gateway + service-to-service calls.
**Real scenario:** The "front door" of a microservice system — one public entry point that routes to many backend services.
**Time:** ~3–4 hours · **Tier:** Foundation spike

## Why this is project #2

Project 00 connected an API to a database. Now you add a second service and put a **gateway** in front of
both. This teaches the two ways services relate: a **client → gateway → service** path (north-south) and a
**service → service** call (east-west). Almost every real system has both. It also sets up the
**load-balancing** lesson in project 21 (a gateway and a load balancer are close cousins).

## What you'll build

Two small APIs — e.g. `catalog` and `orders` — plus a **YARP** reverse-proxy gateway. From the outside you
hit only the gateway on one port; it routes `/catalog/*` and `/orders/*` to the right service. Then you make
`orders` call `catalog` over HTTP using the Compose service name.

## What you'll be able to answer afterwards

- "What is an API gateway and why put one in front of your services?"
- "How is a gateway different from a load balancer?"
- "How do services call each other, and what goes wrong when they do (timeouts, retries, chatty calls)?"

## Definition of Done (full version in `PLAN.md`)

`docker compose up` → hitting **only** `http://localhost:8080`:
- `GET /catalog/ping` and `GET /orders/ping` both work (routed through the gateway).
- `GET /orders/with-catalog` returns data that `orders` fetched from `catalog` internally.

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`INTERVIEW.md`](INTERVIEW.md)
