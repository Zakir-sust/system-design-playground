# 27 · Payment / Ledger System

**Building block:** Idempotency, double-entry bookkeeping, payment state machine, reconciliation.
**Tier:** Capstone · **Time:** 2–3 days · **Interview question:** **"Design a payment system (Stripe)."**

> 📄 **STUB** — expand using projects 00–09 as the template.

## What you'll build
A Stripe-lite wallet/payments service where **correctness is non-negotiable**: no double-charges, money is
never created or lost, every movement is auditable.

## Builds on
05 (idempotency keys — critical here), 23 (ledger as events), 04 (webhooks/events), 22 (saga for multi-step payments).

## Initial → Improve
**Initial (M1):** naive `POST /charge` that updates a balance — then enumerate everything wrong with it (double-charge on retry, no audit, race conditions).
**Improvements:**
- **Idempotency keys** so client retries never double-charge (project 05).
- **Double-entry ledger**: every transaction is balanced debits+credits; balances are derived, never edited directly → money can't be created/lost.
- **Payment state machine** (pending → authorized → captured → settled / failed / refunded).
- **Webhooks** for async provider callbacks; **reconciliation** job comparing internal vs provider records (project 09).
- Auditability via the event log (ties to event sourcing, project 23).

## Key questions this answers
- How do you guarantee no double-charge under retries/concurrency? (idempotency keys)
- Why double-entry bookkeeping? (invariants: money conserved, fully auditable)
- How do you model payment lifecycle? (state machine)
- How do you reconcile with an external provider and handle async callbacks?
