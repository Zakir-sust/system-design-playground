# 26 · Collaborative Editor 🖥️ (Angular UI)

**Building block:** Real-time collaboration — CRDT/OT, conflict resolution, presence.
**Tier:** Capstone · **Time:** 3–5 days · **Interview question:** **"Design Google Docs."**

> 📄 **STUB** — expand using projects 00–09 as the template. Portfolio piece.

## What you'll build
A shared document multiple people edit at once, with changes merging without clobbering each other and live
presence (who's here, cursors). The hard core is **concurrent conflict resolution**.

## Builds on
14 (SignalR/WebSockets, presence), 00 (persistence), 02 (Redis backplane for scale).

## Initial → Improve
**Initial (M1):** single-user document with autosave over SignalR (no concurrency yet).
**Improvements:**
- **Multi-user editing** with conflict resolution via **CRDT** (e.g. an LWW/sequence CRDT) or **OT** — pick one, understand the other.
- **Presence** (active users, live cursors).
- **Offline edits** that merge on reconnect (a CRDT strength).
- Persistence of document state + operation history; scale across instances (backplane).

## Key questions this answers
- How do you merge concurrent edits without a central lock? (CRDT vs OT)
- What are the trade-offs of CRDT vs Operational Transformation?
- How do you show presence and handle reconnection/offline merges?
- Why is this "advanced" — what makes real-time collaboration hard?
