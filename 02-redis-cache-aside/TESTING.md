# Testing 101 (your first tests) — using this project

You said you've never written tests. This is the gentlest possible start. Read it once top-to-bottom.

## The test pyramid (the mental model)
- **Unit tests** — test one piece of logic in isolation, no database/network. Fast (ms), many of them.
- **Integration tests** — test your code against **real** dependencies (a real Redis, a real Postgres). Slower, fewer.
- **End-to-end** — drive the whole system like a user. Slowest, fewest. (You'll meet these later.)

Rule of thumb: lots of unit tests, some integration tests, few e2e.

## xUnit basics (the .NET test framework)
- A test project: `dotnet new xunit -n Notes.Tests`, reference your API project.
- `[Fact]` = one test. `[Theory]` + `[InlineData(...)]` = same test with multiple inputs.
- Structure every test as **Arrange → Act → Assert**:
  ```
  // Arrange: set up inputs and the thing under test
  // Act: call the method
  // Assert: check the result (use FluentAssertions or xUnit's Assert)
  ```
- Run with `dotnet test`.

## Unit test for THIS project
Test the **cache-aside decision** without a real Redis or DB. Put the cache-aside logic behind an
interface (e.g. `ICache` with `Get/Set/Remove`) and inject it. Then in the test use a **fake/mock**
(hand-written fake, or Moq/NSubstitute):
- *Given the cache returns a value* → the DB is **never** called, the cached value is returned. (HIT)
- *Given the cache is empty* → the DB **is** called once, and the result is written back to the cache. (MISS)

This is why we code to an interface: it makes the logic testable in isolation.

## Integration test for THIS project (Testcontainers)
A unit test with a fake can't prove your real Redis serialization/keys work. An **integration test** runs
against a **real Redis in a throwaway container** that Testcontainers starts for the test and disposes after.

- Packages: `Testcontainers.Redis`, and for hitting your API in-memory: `Microsoft.AspNetCore.Mvc.Testing`
  (`WebApplicationFactory<Program>`).
- Pattern:
  1. Implement `IAsyncLifetime`; in `InitializeAsync` start a `RedisContainer` (and a `PostgreSqlContainer` if needed).
  2. Build a `WebApplicationFactory` that points your app's config at the container's **dynamic** connection
     string (Testcontainers assigns a random port — never hardcode it; pass it via `UseSetting`/config override).
  3. Call your endpoint twice with `factory.CreateClient()`; assert the second call is served from cache
     (e.g., assert the DB was hit once, or check a HIT marker).
  4. `DisposeAsync` stops the container.
- Requirement: Docker must be running (Testcontainers needs it). It already is on your machine.

## What "good" looks like here
- [ ] 2 unit tests: HIT path (DB not called), MISS path (DB called once + cache populated).
- [ ] 1 integration test: real Redis container, second request is a cache hit.
- [ ] `dotnet test` green.

## Carry-forward
Project **05** reuses this with Postgres **and** RabbitMQ containers to test the outbox end-to-end. The
`WebApplicationFactory` + Testcontainers combo is the backbone of .NET integration testing — you'll reuse it for the rest of the curriculum.

> Reference: Milan Jovanović, "Testcontainers Best Practices for .NET Integration Testing."
