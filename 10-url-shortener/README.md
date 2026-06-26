# 10 · URL Shortener ⭐ (flagship)

**Building block:** Base62 encoding, key-value lookup, read-heavy caching, click analytics.
**Tier:** Real app · **Time:** 1–2 days · **Interview question:** **"Design a URL shortener (TinyURL/bit.ly)"** — the most common starter.

> 📄 **STUB** — expand into the full guide set (README · PLAN · CONCEPT · INTERVIEW) using projects 00–09 as the template when you reach it.

## What you'll build
A service that turns a long URL into a short code and redirects `GET /{code}` → original URL. This is the
canonical "looks easy, has real depth" project — the depth is in **ID/code generation** and **read-heavy scaling**.

## Builds on
00 (API+DB), 02 (Redis cache), 03 (rate limit), 06 (Base62/Snowflake codes), 04/05 (analytics events).

## Initial → Improve
**Initial (M1):** `POST /shorten` stores (code → long URL) in Postgres; `GET /{code}` 301-redirects. Codes via Base62 of an id.
**Improvements:**
- Cache hot redirects in **Redis** (reads ≫ writes — the central scaling point).
- Code generation strategies: counter+Base62 vs hashing vs Snowflake (project 06); collision handling.
- **Custom aliases** and **expiration** (TTL).
- **Click analytics** — publish a click event (project 04) → async aggregation (don't slow the redirect).
- **Rate limit** create endpoint (project 03).
- Later: scale reads with replicas (19), shard the key space (20).

## Key questions this answers
- How do you generate short, unique codes? (encoding + uniqueness)
- How do you serve a massively read-heavy redirect path fast? (cache, replicas)
- How do you track analytics without slowing redirects? (async events)
- 301 vs 302 redirect — caching implications?
