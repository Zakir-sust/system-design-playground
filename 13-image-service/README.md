# 13 · Image Upload & Processing Service

**Building block:** Object storage + async processing pipeline + CDN concept.
**Tier:** Real app · **Time:** 1–2 days · **Interview question:** **"Design Instagram"** (the media/storage half).

> 📄 **STUB** — expand using projects 00–09 as the template.

## What you'll build
Upload a photo, store the original in object storage, and a background worker generates thumbnails/resized
variants asynchronously — the classic upload→process→serve pipeline.

## Builds on
07 (blob storage + presigned URLs), 04/05 (process events reliably), 02 (cache/serve).

## Initial → Improve
**Initial (M1):** `POST /images` stores the original in MinIO + metadata in Postgres; `GET /images/{id}` serves it.
**Improvements:**
- **Presigned upload** so clients upload straight to storage (project 07).
- On upload, publish an event → **worker** generates thumbnail + sizes (e.g. ImageSharp), stores variants.
- Serve via a **cache/CDN** layer; understand cache invalidation on re-upload.
- Status tracking (processing → ready); handle worker failures (retry/DLQ).
- Metadata, EXIF stripping, content-type validation.

## Key questions this answers
- How do you offload heavy processing from the request path? (event → worker)
- How do you serve media fast and cheaply at scale? (object storage + CDN)
- How do you represent "uploaded but not yet processed"? (state + async)
