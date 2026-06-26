# Concept — Object / blob storage

## What & why
Object storage holds large unstructured files ("objects") addressed by a **key** within a **bucket**. It's
built for scale and durability, not for queries or transactions. MinIO is an S3-compatible store you can run
locally; the same API/shape is AWS S3 and Azure Blob.

**Why not the database?** Relational DBs are for structured, queryable, transactional data. Large binaries
bloat the DB, blow up backups, hurt cache locality, and don't need SQL. Standard practice:
> **bytes → object storage; metadata (owner, key, content-type, size) → your database.**

## Object storage vs file system vs DB
- **File system** — hierarchical paths, server-local, doesn't scale across machines easily.
- **Database** — structured + transactional; wrong tool for big blobs.
- **Object storage** — flat key→bytes namespace, HTTP API, massively scalable, highly durable, no in-place edit (you replace whole objects).

## Presigned URLs (the key scaling trick)
Instead of clients uploading *through* your API (your servers carrying every byte), your API generates a
**time-limited, signed URL** that grants permission to `PUT` (upload) or `GET` (download) one specific object
directly to/from the store. Benefits: your API does no byte-pushing (saves bandwidth/CPU), clients get
direct, fast transfers, and access stays controlled (the URL expires).

## Durability & availability (for the interview)
Cloud object stores replicate objects across machines/zones and use **erasure coding** for very high
durability ("11 nines" in S3 marketing) without storing full copies. You won't implement this, but know the
concept: data is split into shards + parity so it survives disk/node loss.

## .NET wiring (you write it)
- Package: `AWSSDK.S3` (point it at the MinIO endpoint with path-style addressing) or the `Minio` SDK.
- Create the bucket on startup if missing.
- Upload: stream the request body to `PutObject`; persist metadata via EF Core.
- Presign: `GetPreSignedURL` with an expiry and the verb (PUT/GET).

## Pitfalls
- Streaming large files into memory (`byte[]`) instead of streaming → OOM. Stream them.
- Storing blobs in Postgres "for simplicity" — bites you later.
- Public buckets by accident — keep private; use presigned URLs for access.
- Forgetting lifecycle rules → storage grows forever (cost).

## Further reading
- AWS S3 / Azure Blob "presigned/SAS URL" docs.
- MinIO quickstart; `AWSSDK.S3` with custom endpoint.
