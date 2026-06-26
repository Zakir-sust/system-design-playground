# 04 · Message Broker (RabbitMQ + MassTransit)

**Building block:** Asynchronous messaging — message queue & pub/sub.
**Real scenario:** Move slow or optional work off the request path (send email, update analytics) so the API responds fast.
**Time:** ~3–4 hours · **Tier:** Building-block spike

## What you'll build
A producer API that **publishes** an event, and a separate **consumer** worker that processes it — connected
through **RabbitMQ** using **MassTransit** (the .NET messaging abstraction). You'll watch a message leave the
API, sit in a queue, and get handled by the worker independently.

## Why this matters
Synchronous calls couple services' availability and make slow work block the user. A broker **decouples**
producer from consumer in time and load — the foundation for notifications (12), image processing (13),
sagas (22), and event-driven architecture generally.

## What you'll be able to answer afterwards
- "When do you use a message queue instead of a direct HTTP call?"
- "Queue vs pub/sub? At-least-once vs at-most-once delivery?"
- "What happens when a consumer fails — retries, dead-letter queues?"

## Definition of Done (full version in `PLAN.md`)
- `POST /orders` returns immediately; the worker logs that it received `OrderPlaced` a moment later.
- Killing the consumer, publishing, then restarting it → the message is still processed (it waited in the queue).
- A message that always fails lands in a **dead-letter queue** instead of looping forever.

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`INTERVIEW.md`](INTERVIEW.md)
