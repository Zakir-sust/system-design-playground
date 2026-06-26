# Plan — 06 · Snowflake ID Generator

## Goal
Implement a Snowflake-style ID generator, prove uniqueness under concurrency, and understand the alternatives.

## Scope
**In:** 64-bit Snowflake (timestamp + machine id + sequence), thread safety, decode, comparison with UUID/auto-inc.
**Out:** full distributed coordination (ZooKeeper for machine-id assignment) — discuss only.

## Milestones

- [ ] **M1 — Compare the options on paper (and in a quick test).**
  Auto-increment (DB), GUID (`Guid.NewGuid()`), UUIDv7 (time-ordered), Snowflake. Note size, ordering,
  coordination needs, index friendliness.

- [ ] **M2 — Implement Snowflake.**
  Layout a 64-bit long: e.g. 41 bits ms-timestamp (since a custom epoch) + 10 bits machine id + 12 bits
  sequence. Compose with bit-shifts.

- [ ] **M3 — Make it correct under concurrency.**
  Within the same millisecond, increment the sequence; if it overflows, spin-wait to the next ms. Guard with
  a lock so concurrent callers can't produce duplicates. Hammer it from many threads and assert zero dupes.

- [ ] **M4 — Decode.**
  Reverse an id into (timestamp, machine, sequence) to prove the layout and show the ordering property.

## Initial → Improve

**Initial:** thread-safe Snowflake generator, no duplicates under load. ✅ core lesson.

**Improvements:**
- [ ] **Machine id assignment** — where does the 10-bit machine id come from across many pods? (config, env, or a coordinator like ZooKeeper/etcd). Discuss clock-skew and the "clock moved backwards" hazard.
- [ ] **Compare UUIDv7** — .NET 9 has `Guid.CreateVersion7()`; it's time-ordered like Snowflake but 128-bit. When would you pick it?
- [ ] **Base62 encoding** — encode the id to a short string (sets up the URL shortener, project 10).
- [ ] **Throughput test** — measure ids/second; find the sequence-bit ceiling per ms.

## Definition of Done
- [ ] Concurrent generation produces zero duplicates.
- [ ] Different machine ids never collide.
- [ ] You can decode an id and explain each bit field.
- [ ] You can argue when you'd use Snowflake vs UUIDv7 vs auto-increment.
