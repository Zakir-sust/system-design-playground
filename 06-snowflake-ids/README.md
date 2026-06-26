# 06 · Snowflake ID Generator

**Building block:** Distributed unique ID generation.
**Real scenario:** Generate unique, roughly time-ordered IDs across many servers — without a central coordinator or DB round-trip.
**Time:** ~2–3 hours · **Tier:** Building-block spike (short)

## What you'll build
A service/library that mints 64-bit unique IDs in the **Snowflake** style (timestamp + machine id + sequence),
plus a comparison of the alternatives (auto-increment, GUID/UUID, UUIDv7). You'll feed these IDs into later
projects (the URL shortener's short codes in 10, sharded keys in 20).

## Why this matters
"How do you generate IDs at scale?" is a classic follow-up in almost every design question (URL shortener,
chat, feed). Auto-increment needs the DB; GUIDs are unordered and index-unfriendly. Snowflake gives you
decentralized, sortable IDs.

## What you'll be able to answer afterwards
- **"Design a unique ID generator."**
- "Auto-increment vs UUID vs Snowflake — trade-offs?"
- "Why do roughly-sortable IDs help database index performance?"

## Definition of Done (full version in `PLAN.md`)
- `GET /id` returns a unique 64-bit id; hammering it concurrently never repeats.
- Two instances with different machine ids never collide.
- You can decode an id back into its timestamp/machine/sequence parts.

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`INTERVIEW.md`](INTERVIEW.md)
