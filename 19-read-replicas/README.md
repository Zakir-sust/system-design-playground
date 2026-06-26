# 19 · Read Replicas + Caching at Scale

**Building block:** Database replication, read/write splitting, replication lag.
**Tier:** Scale · **Time:** ~1 day · **Interview question:** "How do you scale database reads?"

> 📄 **STUB** — expand using projects 00–09 as the template. Best applied on top of the URL shortener (10) or feed (15).

## What you'll build
Take a read-heavy app and add a **Postgres read replica**: writes go to the primary, reads to the replica.
Then confront **replication lag** (the replica is slightly behind) and decide where stale reads are acceptable.

## Builds on
10/15 (a read-heavy app), 02 (cache as the first line of defense before replicas).

## Initial → Improve
**Initial (M1):** single Postgres. Establish a read-heavy baseline (measure read load).
**Improvements:**
- Add a **streaming replica**; route read queries to it, writes to the primary (two connection strings / a routing layer).
- Observe and handle **replication lag**: "read-your-own-writes" problem (after you post, you don't see it) → read from primary for that case, or wait.
- Layer **caching** (project 02) in front to cut replica load further.
- Failover basics: what happens when the primary dies (promote a replica).

## Key questions this answers
- How do you scale reads when one DB can't keep up? (replicas + cache)
- What is replication lag and what does it break? (read-your-writes, stale reads)
- When is a stale read acceptable, and when must you hit the primary?
- Replication vs sharding — different problems (reads vs writes/size).
