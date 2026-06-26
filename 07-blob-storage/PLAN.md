# Plan — 07 · Blob / Object Storage

## Goal
Store and serve files with MinIO, keeping metadata in Postgres, and use presigned URLs.

## Scope
**In:** MinIO container, bucket, upload/download via API, metadata in DB, presigned URLs, lifecycle/expiry.
**Out:** CDN (project 13/30), chunked/resumable uploads (project 25), erasure coding internals.

## Milestones

- [ ] **M1 — Run MinIO + create a bucket.**
  Add a `minio` service to Compose (it has a web console — log in and look). Create a bucket (via console or
  the SDK on startup).

- [ ] **M2 — Upload/download through the API.**
  Package: `AWSSDK.S3` or `Minio`. `POST /files` streams the upload to MinIO under a generated key, saves a
  metadata row (id, key, filename, content-type, size, owner) to Postgres. `GET /files/{id}` looks up
  metadata and streams the object back.

- [ ] **M3 — Presigned URLs.**
  Generate a time-limited **presigned PUT** URL; the client uploads *directly* to MinIO with it. Same for a
  presigned GET for downloads. Notice your API no longer proxies the bytes — huge for scale.

## Initial → Improve

**Initial:** upload/download through the API with metadata in Postgres. ✅ core lesson.

**Improvements:**
- [ ] **Presigned URLs** for direct client upload/download (offload bandwidth from your API).
- [ ] **Lifecycle / expiry** — auto-delete objects after N days (bucket lifecycle policy).
- [ ] **Validation** — enforce content-type/size limits; virus-scan hook (conceptual).
- [ ] **Keys & layout** — design object keys (`user/{id}/{uuid}.{ext}`); discuss why a flat namespace scales.
- [ ] **Cloud parity** — note the identical flow with Azure Blob / AWS S3 (you'll do it for real in 29).

## Definition of Done
- [ ] File round-trips through MinIO; metadata persists in Postgres.
- [ ] A presigned URL uploads a file without the bytes passing through your API.
- [ ] You can explain the metadata-in-DB / bytes-in-blob split and why it matters.
