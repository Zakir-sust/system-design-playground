# Interview angle — 06 · Snowflake ID Generator

## Questions this prepares you for

- **"Design a unique ID generator for a distributed system."**
  → Requirements: unique, ~64-bit, high throughput, ideally time-sortable, no single bottleneck. Walk through
  Snowflake's bit layout; explain machine-id assignment and clock-skew handling.

- **"Auto-increment vs UUID vs Snowflake?"**
  → Auto-increment: simple, ordered, but the DB is a coordinator/bottleneck and leaks counts; bad across
  shards. UUIDv4: decentralized but random → poor index locality. Snowflake/UUIDv7: decentralized **and**
  time-ordered. Pick based on coordination tolerance and index needs.

- **"Why not just use the database's identity column?"**
  → Works for one DB; once you shard or want to generate IDs before insert / without a round-trip, you need a
  decentralized scheme.

- **"Why are time-ordered IDs better for indexes?"**
  → Inserts cluster at the end of the B-tree → fewer page splits, better cache locality and write throughput.

## Trade-offs
- Snowflake depends on clocks → clock skew/regression is the main failure mode; needs handling.
- Machine-id assignment is the one bit of coordination you can't avoid.
- Exposing sortable IDs can leak ordering/volume info; if that matters, add randomness or use opaque public ids.

## Connects to
URL shortener short codes (10, via Base62), sharding keys (20).
