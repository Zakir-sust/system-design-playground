# 20 · Sharding / Partitioning

**Building block:** Horizontal partitioning, shard keys, consistent hashing.
**Tier:** Scale · **Time:** ~1 day · **Interview question:** **"Sharding / key-value store partitioning."**

> 📄 **STUB** — expand using projects 00–09 as the template. Apply on the URL shortener (10) or chat messages (14).

## What you'll build
Split one dataset across multiple databases so writes/size scale beyond a single node. You choose a **shard
key**, route each row to the right shard (often via **consistent hashing**), and feel the costs.

## Builds on
06 (sortable/uniform keys), 19 (replication contrast), 02 (consistent hashing also used for cache).

## Initial → Improve
**Initial (M1):** single DB. Identify the shard key (e.g. short-code, user id) and write a routing function.
**Improvements:**
- Stand up **2–3 shards** (separate Postgres containers); route reads/writes by `hash(key) → shard`.
- Use **consistent hashing** so adding a shard moves minimal data (vs `% N` reshuffling everything).
- Confront the hard parts: **cross-shard queries**, **hot shards**, and **resharding/rebalancing**.
- Combine with replication (each shard has replicas) — the real-world layout.

## Key questions this answers
- When do you shard, and how do you pick a shard key? (write/size limits; even distribution)
- Why consistent hashing over modulo? (minimal data movement on scale change)
- What gets hard once sharded? (cross-shard joins, transactions, hotspots, rebalancing)
- How do sharding and replication combine?
