# 28 · Observability Stack

**Building block:** Logs, metrics, distributed tracing (OpenTelemetry + Prometheus + Grafana).
**Tier:** Capstone · **Time:** 2–3 days · **Interview question:** "Monitoring/metrics system; how do you operate this?"

> 📄 **STUB** — expand using projects 00–09 as the template. **This directly serves your "I can't visualize how it's all connected" goal.**

## What you'll build
Instrument your multi-service system (the e-commerce + saga services are ideal) so you can **see** a single
request flow across gateway → services → broker → DB, plus dashboards and alerts. This is the payoff for
everything you wired earlier.

## Builds on
00 (Serilog/Seq logs), 01 (correlation id at the gateway), 04/22 (multi-service async flows to trace).

## Initial → Improve
**Initial (M1):** structured logs to Seq (you already have this from project 00) with a correlation id propagated across services.
**Improvements:**
- **OpenTelemetry** tracing: one trace spans gateway → service → service → broker → consumer (see the async hop!).
- **Metrics** to Prometheus (request rate, latency, error rate, queue depth); **Grafana** dashboards.
- **Alerting** on SLO breaches (p99 latency, error budget).
- The three pillars together: jump from a slow trace → its logs → the metric spike.

## Key questions this answers
- How do you debug "why is this request slow?" across many services? (distributed tracing)
- Logs vs metrics vs traces — what each is for.
- What do you monitor and alert on? (the four golden signals, SLOs)
- How does a correlation id / trace context propagate through sync and async hops?
