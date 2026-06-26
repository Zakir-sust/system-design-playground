# Concept — Search indexes

## Why not SQL `LIKE`?
`WHERE name LIKE '%phone%'` can't use a normal index (leading wildcard → full scan), doesn't rank results,
ignores word stems/synonyms/typos, and degrades badly at scale. Search engines are purpose-built for ranked
text retrieval.

## The inverted index (the core idea)
A normal index maps row → values. An **inverted index** maps **term → list of documents containing it**:
```
"phone"  -> [doc1, doc7, doc9]
"cheap"  -> [doc7, doc12]
```
A query for "cheap phone" intersects/unions these postings lists and **scores** each document. This is why
search is fast and rankable. Building it involves **analysis**: tokenizing text into terms, lowercasing,
removing stop words, **stemming** ("running" → "run").

## Relevance ranking
Documents get a score (classic TF-IDF / BM25): terms that are rare across the corpus but frequent in a
document score higher; shorter fields weigh more. You can **boost** fields (title matches > body matches) and
allow **fuzziness** for typos.

## Where it fits in your architecture
Elasticsearch is **not** your source of truth — your transactional DB is. ES is a derived, query-optimized
**read model** kept in sync from the DB. That sync is **eventually consistent** (a small delay), which is an
acceptable trade for search. Two sync strategies:
- **Event-driven** (preferred): DB change → event (outbox, project 05) → consumer upserts ES.
- **Batch reindex**: periodically rebuild from the DB (also your recovery path).

## .NET wiring (you write it)
- Package: `Elastic.Clients.Elasticsearch` (or OpenSearch client).
- Define an index mapping: `text` for analyzed/searchable fields, `keyword` for exact filtering/aggregations.
- Index documents (single + bulk); query with `match`, combine with `filter`; add aggregations for facets.
- A consumer that upserts/deletes ES docs on domain events.

## Pitfalls
- Treating ES as the system of record (it's a derived index — you must be able to rebuild it).
- `text` vs `keyword` confusion (filter/sort/aggregate on `keyword`, search on `text`).
- Deep pagination (`from: 10000`) is expensive → use `search_after`.
- Forgetting to handle deletes/reindex in the sync pipeline.

## Further reading
- Elasticsearch "inverted index" + BM25 docs.
- `Elastic.Clients.Elasticsearch` .NET getting started.
