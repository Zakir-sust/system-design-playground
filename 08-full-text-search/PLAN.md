# Plan — 08 · Full-Text Search

## Goal
Index and search documents in Elasticsearch with ranking and filters, kept in sync with Postgres via events.

## Scope
**In:** ES container, index + mapping, indexing docs, query (match/relevance), filters/facets, DB→ES sync.
**Out:** autocomplete/suggesters (project 18), cluster sharding internals, ML ranking.

## Milestones

- [ ] **M1 — Run Elasticsearch (or OpenSearch).**
  Add the container to Compose (single-node, security disabled for local). Confirm it's up (`GET /` returns cluster info). Optionally add Kibana/Dejavu to browse.

- [ ] **M2 — Create an index + index documents.**
  Package: `Elastic.Clients.Elasticsearch` (or `NEST`/OpenSearch client). Define a mapping for your document
  (text fields analyzed, keyword fields for filtering). Index a batch of records.

- [ ] **M3 — Query with relevance.**
  Run a `match` query; observe results come back **scored/ranked**, not in table order. Add a keyword
  **filter** (category) and a simple **facet/aggregation** (counts per category).

- [ ] **M4 — Keep it in sync with Postgres.**
  Postgres is the source of truth. On create/update/delete, publish an event (project 04 broker / project 05
  outbox); a consumer upserts/deletes the ES document. Accept that ES is **eventually consistent** with the DB.

## Initial → Improve

**Initial:** index + ranked query + one filter. ✅ core lesson.

**Improvements:**
- [ ] **Sync pipeline** — wire DB changes → events → ES upserts (reuse 04/05). Handle deletes and reindex.
- [ ] **Analyzers** — lowercase, stemming, stop words; see how they change matches.
- [ ] **Relevance tuning** — boost fields (title > body), fuzziness for typos.
- [ ] **Pagination** — `from`/`size` and why deep pagination is expensive (search-after).
- [ ] **Bulk indexing** — index large batches efficiently.

## Definition of Done
- [ ] A text query returns ranked results with the best match first.
- [ ] Filtering/faceting by a keyword field works.
- [ ] Editing a row in Postgres is reflected in search results after the sync event.
- [ ] You can explain inverted index + why ES ≠ your transactional DB (eventual consistency).
