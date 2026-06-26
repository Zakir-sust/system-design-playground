# Interview angle — 00 · Connected Hello World

This isn't a classic "design X" question, but **deployment fluency makes every other answer credible.**
When you say "...and then the cache talks to the API," the interviewer trusts you more if you can also
explain how those processes actually find and talk to each other.

## Questions this prepares you for

- **"Walk me through how your services are deployed locally."**
  → Compose file, each service is a container, they share a network, they reach each other by service name,
  state lives in volumes, config comes from env vars.

- **"Image vs container vs VM?"**
  → Image = read-only template; container = running instance sharing the host kernel (lightweight); VM =
  full guest OS (heavier). Containers start in ms and pack densely.

- **"How do two services discover each other?"**
  → In Compose, DNS by service name. In the cloud, a load balancer / service discovery / DNS. (Foreshadows projects 01 and 21.)

- **"How does configuration and secrets get into the app?"**
  → 12-factor: config from the environment. Env vars override `appsettings.json`; the `__` separator maps
  to nested keys. Secrets stay out of the image; locally in `.env`, in the cloud in a secret store.

- **"How do you make sure the app doesn't start before its database is ready?"**
  → Healthchecks + `depends_on: condition: service_healthy`, plus retry-on-connect in the app.

## Trade-offs to be able to discuss
- Applying EF migrations on startup is convenient but risky with multiple instances (two apps migrating at
  once) — fine now, revisit when you scale (project 21) and in cloud CI/CD (project 29).
- Compose is for local/dev. Production uses an orchestrator (Kubernetes) or a managed container host
  (Azure Container Apps — project 29). The *concepts* (networking, health, config) carry over.
