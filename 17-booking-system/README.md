# 17 · Booking / Ticketing System

**Building block:** Concurrency control — optimistic/pessimistic locking, idempotency, distributed locks.
**Tier:** Real app · **Time:** 1–2 days · **Interview question:** **"Design hotel reservation / Ticketmaster."**

> 📄 **STUB** — expand using projects 00–09 as the template.

## What you'll build
Reserve a limited resource (hotel room / event seat) where **two people must never book the same seat**. The
whole project is about preventing double-booking under concurrency — first reproduce the bug, then fix it
several ways.

## Builds on
00 (API+DB), 02 (Redis for distributed locks), 05 (idempotency keys).

## Initial → Improve
**Initial (M1):** naive "check availability, then insert booking." **Reproduce the double-book** under concurrent requests (load test it).
**Improvements:**
- **Pessimistic locking** — `SELECT ... FOR UPDATE` on the seat row during booking.
- **Optimistic concurrency** — version/rowversion column; retry on conflict.
- **Idempotency key** so a client retry doesn't create a second booking (project 05).
- **Distributed lock** (Redis) when the resource spans services/instances; understand lock expiry/fencing.
- **Seat hold + expiry** — temporary reservation during checkout, released if not confirmed (a job, project 09).

## Key questions this answers
- How do you prevent double-booking under concurrency? (locking strategies and their costs)
- Optimistic vs pessimistic locking — when each?
- How do distributed locks work and how do they fail? (expiry, fencing tokens, split-brain)
- How do "held" reservations expire safely?
