# Testing — integration tests across Postgres + RabbitMQ

This builds on the testing intro in **[02/TESTING.md](../02-redis-cache-aside/TESTING.md)**. Re-read the
test pyramid and xUnit basics there first. Here you go further: an integration test that spins up **two**
real dependencies and verifies behavior that *can't* be unit-tested — reliability under failure.

## What to unit-test here
- The **idempotency check**: given a message id already in the dedup table, the handler does nothing; given a
  new id, it processes and records it. (Fake the dedup store — pure logic, no containers.)

## What to integration-test here (the real value)
Reliability is an emergent property of real moving parts, so test against real ones via **Testcontainers**:

- **Containers:** `PostgreSqlContainer` + `RabbitMqContainer`, started in `IAsyncLifetime.InitializeAsync`.
- **Wiring:** a `WebApplicationFactory<Program>` whose config points at the containers' **dynamic**
  connection strings (never hardcode the ports — read them from the started containers and override config
  via `UseSetting`/`ConfigureWebHost`).
- **Run real EF migrations** against the Postgres container on startup, so you catch migration bugs too.

### Test 1 — "event is eventually published" (the outbox guarantee)
1. `POST /orders` through the test client.
2. Assert a row exists in `OutboxMessages`.
3. Let the relay run; assert the message arrives on RabbitMQ (consume it in the test, or assert the row flips to `sent`).
4. **Bonus (the real proof):** pause/stop the RabbitMQ container *before* the relay runs, confirm the order
   still committed and the outbox row is unsent; restart RabbitMQ; confirm it then publishes. This is the
   "no lost events" guarantee made concrete.

### Test 2 — "duplicate delivery is safe" (idempotency)
1. Deliver the same message id to the consumer **twice**.
2. Assert the side effect happened **once** (e.g. one inventory decrement), and the dedup table has one entry.

## xUnit mechanics you'll meet here
- **Async lifecycle:** `IAsyncLifetime` (`InitializeAsync`/`DisposeAsync`) to start/stop containers cleanly.
- **Shared context:** a **collection fixture** so the (slow) containers start once and are shared across the
  tests in a collection, instead of per-test. (See xUnit "shared context.")
- **Test isolation:** reset DB state between tests (truncate tables, or respawn) so tests don't bleed into each other.

## Definition of Done
- [ ] 1 unit test for the idempotency decision.
- [ ] Test 1 (outbox publishes, survives a broker outage) green.
- [ ] Test 2 (duplicate is ignored) green.
- [ ] `dotnet test` runs both with containers starting/stopping automatically.

## Why this is the template for the rest of the course
`WebApplicationFactory` + Testcontainers + collection fixtures is the standard way to integration-test .NET
services with real infra. From here on, "add a test" means this pattern. Reference: Testcontainers .NET docs
and Milan Jovanović's best-practices article.
