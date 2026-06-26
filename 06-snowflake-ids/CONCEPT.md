# Concept — Distributed unique IDs

## The problem
Many servers need to create unique IDs at once. Options trade off coordination, ordering, and size.

| Approach | Unique across servers? | Sortable by time? | Needs coordination? | Size | Notes |
|---|---|---|---|---|---|
| DB auto-increment | yes (single DB) | yes | the DB is the coordinator | 8B | round-trip to DB; bottleneck when sharded |
| GUID/UUIDv4 | yes | **no** (random) | no | 16B | random → bad index locality (page splits) |
| UUIDv7 | yes | yes (time-prefixed) | no | 16B | modern, .NET 9 `Guid.CreateVersion7()` |
| **Snowflake** | yes | yes (time-prefixed) | only machine-id assignment | 8B | decentralized, compact, sortable |

## Why "sortable" matters
Random IDs (UUIDv4) as a clustered primary key cause inserts to land all over the index → page splits and
fragmentation. Time-ordered IDs (Snowflake, UUIDv7) insert near the "end" → better write performance and
locality. Sortable IDs also give you a free rough chronological order.

## Snowflake layout (64-bit)
```
 0 | timestamp (41 bits, ms since custom epoch) | machine id (10 bits) | sequence (12 bits)
```
- **timestamp** — ms since a custom epoch → ~69 years of range; gives ordering.
- **machine id** — unique per node (10 bits = 1024 nodes) → no cross-node collision.
- **sequence** — counter within the same ms (12 bits = 4096 ids/ms/node).

To generate: take `now_ms`; if same ms as last, `sequence++`; if sequence overflows, wait for the next ms;
if different ms, reset sequence to 0. Then `(ts << 22) | (machine << 12) | sequence`. **Lock** this so
concurrent callers don't race.

## The hazards
- **Clock moving backwards** (NTP correction) → could repeat a timestamp. Detect it; refuse or wait.
- **Machine id collisions** → two nodes with the same id produce dupes. Assignment must be unique (config, orchestrator, or a coordinator like ZooKeeper/etcd).
- **Sequence overflow** in a hot ms → spin to next ms (caps throughput per node per ms).

## .NET wiring (you write it)
- A thread-safe generator (lock around timestamp+sequence state); register as a **singleton**.
- Expose via DI; optionally a `GET /id` endpoint for the demo.
- Consider `Guid.CreateVersion7()` (.NET 9) as a zero-effort sortable alternative and compare.

## Further reading
- Twitter Snowflake (original design).
- UUIDv7 spec; .NET 9 `Guid.CreateVersion7`.
