# Concept — Message brokers & async messaging

## What & why
A **message broker** (RabbitMQ) lets services talk **asynchronously**: a producer drops a message and moves
on; a consumer picks it up whenever it can. This **decouples** them in three ways:
- **Time** — the consumer can be down/slow; the message waits in the queue.
- **Load** — bursts are buffered; the consumer drains at its own pace (smoothing spikes).
- **Code** — producer doesn't know who consumes, or how many.

Use it when work is slow, optional, retryable, or fan-out: emails, thumbnails, analytics, search indexing,
cross-service events.

## Queue vs pub/sub
- **Queue (point-to-point)** — one message, one consumer processes it (competing consumers share the load).
- **Pub/sub (publish-subscribe)** — one message, *every* subscriber gets a copy.
In MassTransit: `Send` → a specific queue (point-to-point); `Publish` → all subscribers of that message type.

## Delivery semantics
- **At-most-once** — may lose messages (fire and forget).
- **At-least-once** — never lost, but may be **delivered twice** (so consumers must be **idempotent** — project 05). This is the common, practical default.
- **Exactly-once** — very hard end-to-end; usually approximated with at-least-once + idempotency.

## RabbitMQ terms (just enough)
- **Exchange** — where producers publish; routes messages to queues by rules.
- **Queue** — holds messages until consumed.
- **Binding** — the rule linking an exchange to a queue.
MassTransit sets most of this up for you from your message types, so you can focus on producers/consumers.

## Failure handling
- **Acknowledgement** — the broker keeps a message until the consumer acks success; a crash mid-process →
  redelivery (hence at-least-once).
- **Retry** — transient failures: retry a few times with backoff.
- **Dead-letter queue (DLQ)** — after retries are exhausted, park the message in an error queue for
  inspection instead of blocking the queue forever.

## .NET wiring (you write it)
- Packages: `MassTransit`, `MassTransit.RabbitMQ`.
- Configure the bus with the RabbitMQ host (service name `rabbitmq`), register consumers, set retry policy.
- Producer: inject `IPublishEndpoint`/`ISendEndpointProvider`. Consumer: implement `IConsumer<T>`.

## Pitfalls
- Assuming exactly-once — design consumers to be **idempotent**.
- Doing the DB write and the publish as two separate steps — if the publish fails after the commit, you
  lose the event (the **dual-write** problem → fixed by the outbox in project 05).
- Ignoring the DLQ — poison messages silently pile up or block the queue.
- Unbounded retries hammering a failing downstream.

## Further reading
- MassTransit docs (configuration, consumers, retry, redelivery).
- RabbitMQ "AMQP concepts."
