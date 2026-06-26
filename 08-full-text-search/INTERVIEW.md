# Interview angle — 08 · Full-Text Search

## Questions this prepares you for

- **"How would you implement search over millions of products/posts?"**
  → A search engine (Elasticsearch) with an inverted index, fed from the source DB via events; ranked results,
  filters, facets; DB stays the source of truth.

- **"Why not SQL `LIKE`?"**
  → Can't use indexes with leading wildcards, no ranking, no stemming/typo tolerance, scans at scale.

- **"What's an inverted index?"**
  → term → documents mapping; queries intersect postings lists and score with BM25/TF-IDF.

- **"How do you keep the search index consistent with the database?"**
  → Event-driven sync (outbox → consumer → ES upsert), accepting eventual consistency; plus periodic full
  reindex for recovery.

- **"How does search scale?"**
  → ES shards the index across nodes and replicates shards; queries scatter-gather across shards. (Connects to
  sharding, project 20.)

## Trade-offs
- ES is eventually consistent with the DB — fine for search, not for "must be exact right now."
- Operating ES (memory-hungry, cluster management) is real ops cost.
- Relevance tuning is iterative; there's no single "correct" ranking.

## Connects to
Autocomplete/typeahead (project 18) uses the same engine with a completion suggester.
