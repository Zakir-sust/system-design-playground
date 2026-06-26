# 15 · News Feed / Timeline

**Building block:** Fan-out on write vs read, feed caching, social graph, ranking.
**Tier:** Real app · **Time:** 1–2 days · **Interview question:** **"Design a news feed (Facebook/Twitter)."**

> 📄 **STUB** — expand using projects 00–09 as the template.

## What you'll build
A timeline where users follow others and see a merged feed of their posts. The whole project is about the
**fan-out trade-off**: precompute feeds on write, or assemble them on read?

## Builds on
00 (API+DB), 02 (Redis feed cache), 04 (fan-out events), 06 (sortable post ids).

## Initial → Improve
**Initial (M1):** **pull/fan-out-on-read** — to build a user's feed, query recent posts from everyone they follow, merge, sort.
**Improvements:**
- **Fan-out-on-write** — when a user posts, push the post id into each follower's precomputed feed (Redis list); reads become a cheap lookup.
- **Hybrid** — fan-out-on-write for most, fan-out-on-read for celebrities (millions of followers) to avoid write storms.
- **Ranking** beyond chronological; pagination (cursor, not offset).
- Cache + invalidation; handle new follows/unfollows.

## Key questions this answers
- Fan-out on write vs read — the central feed trade-off and when each wins.
- How do you handle "celebrity" accounts (the fan-out explosion)?
- How do you paginate an infinite, changing feed? (cursors)
- Read-heavy vs write-heavy: where does the cost go?
