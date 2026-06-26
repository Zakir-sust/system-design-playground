# 08 · Full-Text Search (Elasticsearch)

**Building block:** Search index (inverted index, relevance ranking).
**Real scenario:** "Search products / articles / messages" — something `LIKE '%term%'` in SQL can't do well.
**Time:** ~3–4 hours · **Tier:** Building-block spike

## What you'll build
Index documents (say, products or notes) into **Elasticsearch** and query them with real relevance ranking,
filters, and facets. You'll keep ES in sync with Postgres by **publishing change events** (reusing the broker
from project 04) so the search index updates when data changes.

## Why this matters
Search is its own building block. A relational DB is great for exact lookups and joins but poor at ranked
text search across large corpora. ES (and the inverted index behind it) is the standard answer, and it
directly sets up **autocomplete** (project 18).

## What you'll be able to answer afterwards
- "Why not just use SQL `LIKE` for search? When do you reach for a search engine?"
- "What's an inverted index? How does relevance ranking work?"
- "How do you keep the search index in sync with the source-of-truth database?"

## Definition of Done (full version in `PLAN.md`)
- Documents are indexed; a query returns **ranked** results (best matches first), not just rows.
- Filtering/faceting works (e.g. by category).
- Updating a record in Postgres updates the ES document (via an event), within a short delay.

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`INTERVIEW.md`](INTERVIEW.md)
