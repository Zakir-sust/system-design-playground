# Interview angle — 03 · Rate Limiter

**"Design a rate limiter" is one of the most common standalone system-design questions.** This project lets
you answer it from hands-on experience.

## How to drive the answer
1. **Clarify:** limit by what (IP/user/API key)? what limits (e.g. 100/min)? global or per-endpoint? hard
   reject or queue?
2. **Pick an algorithm** and justify: token bucket (burst-friendly), sliding window (precise). Explain fixed
   window's boundary flaw.
3. **Where it runs:** gateway vs per-service middleware; edge is cheaper.
4. **Distributed state:** counters in Redis so the limit is global across instances; emphasize **atomic**
   increment (Lua) to avoid races.
5. **Response:** `429` + `Retry-After` + `X-RateLimit-*` headers.
6. **Scale/availability:** Redis is now on the hot path — replicate it; decide fail-open vs fail-closed if
   Redis is down (availability vs protection trade-off).

## Sharp questions you should be able to field
- "Fixed vs sliding window vs token bucket — pick one for a login endpoint." → sliding/token; precise + burst control; protect against brute force.
- "Your limiter's Redis goes down — what happens?" → decide: fail-open (serve, lose protection) or fail-closed (reject, hurt availability). State the choice and why.
- "How do you limit across 50 servers?" → shared counter in Redis, atomic ops; not in-memory.
- "Per-user fairness with one noisy tenant?" → per-key buckets, possibly weighted/tiered.

## Trade-offs
- Edge enforcement saves work but the edge needs the shared state; per-service is simpler but duplicated.
- Strict accuracy (sliding log) costs memory; counters are cheaper but approximate.
- The limiter must be **faster and more available than what it protects**, or it becomes the bottleneck.
