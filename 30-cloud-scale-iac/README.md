# 30 · Cloud Scale & Infrastructure-as-Code *(optional stretch)*

**Building block:** IaC (Bicep/Terraform), autoscaling, CDN, multi-cloud literacy.
**Tier:** Cloud stretch · **Time:** 1–2 days · **Interview question:** "Reproducible infra / multi-cloud."

> 📄 **STUB** — expand using projects 00–09 as the template. Optional — do it if you want depth on cloud/ops.

## What you'll build
Turn the click-ops deploy from project 29 into **reproducible, version-controlled infrastructure**, add
autoscaling and a CDN, and (optionally) replicate a slice on **AWS** with your $100 credit for comparison.

## Builds on
29 (the deployed app), 13/25 (assets to put behind a CDN).

## Initial → Improve
**Initial (M1):** codify the project-29 resources as **Bicep** (Azure-native) or **Terraform** — `deploy` from a single command.
**Improvements:**
- **Autoscaling** rules (CPU/queue-depth based); load-test and watch it scale.
- **CDN** (Azure Front Door) in front of static/media assets; cache + TLS at the edge.
- **AWS comparison**: deploy the same container to **ECS/Fargate** + **S3** ($100 credit) — see how the concepts map across clouds.
- **Cost guardrails**: budgets/alerts; teardown scripts.

## Key questions this answers
- Why Infrastructure-as-Code? (reproducible, reviewable, no snowflake servers)
- How does autoscaling work and what do you scale on?
- How do the same building blocks map across Azure and AWS?
- How do you keep a learning project free? (budgets, scale-to-zero, teardown)
