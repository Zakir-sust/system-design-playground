# 12 · Notification Service

**Building block:** Queues, fan-out, provider abstraction, retries, idempotency.
**Tier:** Real app · **Time:** 1–2 days · **Interview question:** **"Design a notification system."**

> 📄 **STUB** — expand using projects 00–09 as the template.

## What you'll build
A service that accepts "send notification" requests and reliably delivers them across channels (email, SMS,
push) via background workers — decoupled from callers through a broker.

## Builds on
04 (broker), 05 (outbox + idempotency), 09 (jobs/retries, fan-out).

## Initial → Improve
**Initial (M1):** `POST /notify` enqueues a message; an **email** consumer "sends" it (log/Mailhog).
**Improvements:**
- Multiple **channels** behind a provider interface (email/SMS/push); pick per user preference.
- **Fan-out**: one event → multiple channels (project 09).
- **Retries + dead-letter** for failed sends; **idempotency** so retries don't double-send (project 05).
- **User preferences** & quiet hours; **rate-limit per provider** (project 03).
- **Templates** and localization; **dedup** of identical notifications.

## Key questions this answers
- How do you deliver reliably when providers fail? (retries, DLQ, idempotency)
- How do you support many channels without rewiring callers? (abstraction + queue)
- How do you respect user preferences and avoid spamming? (prefs, dedup, rate limits)
