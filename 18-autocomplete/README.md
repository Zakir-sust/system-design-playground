# 18 · Search Autocomplete / Typeahead 🖥️ (small Angular UI)

**Building block:** Trie / prefix index, top-k ranking, low-latency caching.
**Tier:** Real app · **Time:** 1–2 days · **Interview question:** **"Design search autocomplete/typeahead."**

> 📄 **STUB** — expand using projects 00–09 as the template. Includes a small Angular search box to feel the latency.

## What you'll build
As-you-type suggestions that return in well under 100 ms, ranked by popularity. The lesson is data structures
+ caching for latency, building on the search engine from project 08.

## Builds on
08 (Elasticsearch), 02 (Redis cache for hot prefixes), 06 (ranking by counts).

## Initial → Improve
**Initial (M1):** prefix query against the DB (`WHERE term LIKE 'pre%'`) — works, but watch the latency and ranking gaps.
**Improvements:**
- **Trie / prefix structure** (in memory or Redis) returning **top-k** by popularity.
- **ES completion suggester** for a managed approach; compare to the hand-built trie.
- **Caching** hot prefixes in Redis; **debounce** on the client.
- **Typo tolerance** (fuzzy), result ranking by frequency/recency.
- Updating popularity counts from search events (project 04).

## Key questions this answers
- How do you serve suggestions in <100 ms? (prefix structures + cache, precomputation)
- How do you rank suggestions? (popularity/top-k, recency)
- Trie vs search-engine suggester — trade-offs?
- How do you keep popularity fresh without slowing queries?
