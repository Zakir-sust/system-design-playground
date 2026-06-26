# 14 · Real-time Chat 🖥️ (Angular UI)

**Building block:** WebSockets/SignalR, presence, message ordering, Redis backplane for scale-out.
**Tier:** Real app + capstone · **Time:** 2–3 days · **Interview question:** **"Design a chat system (WhatsApp/Slack)."**

> 📄 **STUB** — expand using projects 00–09 as the template. **First project with a real Angular frontend.**

## What you'll build
A chat app with live messaging over **SignalR**, persisted history, presence, and (the scaling lesson) a
**Redis backplane** so it still works when you run multiple server instances.

## Builds on
00 (API+DB), 02 (Redis), 04 (queues for delivery/offline), 21 (multi-instance later).

## Initial → Improve
**Initial (M1):** 1:1 chat over a SignalR hub; persist messages in Postgres; Angular client shows live messages.
**Improvements:**
- **Group chat** and rooms; **presence/heartbeat** (online/last-seen).
- **Redis backplane** — run 2+ server instances and keep messages flowing across them (the key scale lesson).
- **Delivery/read receipts**, message ordering, pagination of history.
- **Offline delivery** via a queue + push fallback (project 12).
- Typing indicators; basic auth on the hub.

## Key questions this answers
- WebSockets vs long-polling — why push for chat?
- How do you scale stateful socket connections across servers? (backplane / sticky + shared state)
- How do you guarantee ordering and handle offline users?
- Where do messages get persisted vs just relayed?
