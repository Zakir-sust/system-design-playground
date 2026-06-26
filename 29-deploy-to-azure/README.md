# 29 · Deploy to Azure (free tier)

**Building block:** Mapping local building blocks → managed cloud services; CI/CD.
**Tier:** Cloud · **Time:** 1–2 days · **Interview question:** "How would you deploy and operate this in production?"

> 📄 **STUB** — expand using projects 00–09 as the template. Uses your **1-year Azure free** resources.

## What you'll build
Take a polished app (the **URL shortener (10)** + one capstone, e.g. chat (14) or e-commerce (16)) and ship
it to Azure. The lesson: every local container maps to a **managed service**, and you stop running infra by hand.

## Builds on
Everything. Specifically the Docker images you built throughout, plus 00 (config via env), 02/07 (cache/blob).

## Local → Cloud mapping
| Local (Docker) | Azure (managed) |
|---|---|
| API container | **Azure Container Apps** |
| Postgres container | **Azure Database for PostgreSQL** |
| Redis container | **Azure Cache for Redis** |
| MinIO | **Azure Blob Storage** |
| RabbitMQ | **Azure Service Bus** (or container) |
| `.env` secrets | **Key Vault** + app settings |
| `docker compose up` | **GitHub Actions** CI/CD → deploy |

## Initial → Improve
**Initial (M1):** deploy the API to Container Apps + a managed Postgres; reach it on a public URL over HTTPS.
**Improvements:**
- Add **managed Redis** + **Blob Storage**; move secrets to **Key Vault**.
- **GitHub Actions** pipeline: build image → push to registry → deploy on merge.
- **Scaling rules** (scale on HTTP load), custom domain + TLS.
- Watch costs; stay in free limits; tear down when done.

## Key questions this answers
- How do local building blocks map to managed cloud services?
- How do you do zero-/low-cost deploys for learning? (free tiers, scale-to-zero)
- What changes between local and cloud? (secrets, networking, managed state, CI/CD)
