# 07 · Blob / Object Storage (MinIO)

**Building block:** Object storage (S3-compatible) for large files.
**Real scenario:** Users upload images/documents/video — you must not stuff those bytes into your relational DB.
**Time:** ~3 hours · **Tier:** Building-block spike

## What you'll build
File upload/download against **MinIO** (an S3-compatible object store you run in Docker). You'll store the
**bytes** in MinIO and the **metadata** (owner, filename, content-type, key) in Postgres — the standard split.
Then add **presigned URLs** so clients upload/download directly without proxying bytes through your API.

## Why this matters
Blobs belong in object storage, not databases. The MinIO API is the same shape as **AWS S3** and
**Azure Blob**, so this skill maps straight to the cloud (project 29). It underpins pastebin (11), the image
service (13), and video (25).

## What you'll be able to answer afterwards
- "Where do you store user-uploaded files, and why not in the database?"
- "What's a presigned URL and why use one?"
- "How does object storage differ from a file system or a database?"

## Definition of Done (full version in `PLAN.md`)
- `POST /files` stores a file in MinIO + a metadata row in Postgres; `GET /files/{id}` returns it.
- A **presigned upload URL** lets you `PUT` a file straight to MinIO (bytes never touch your API).
- You can browse the bucket in the MinIO console and see your objects.

## Read in this order
1. [`CONCEPT.md`](CONCEPT.md) 2. [`PLAN.md`](PLAN.md) 3. [`INTERVIEW.md`](INTERVIEW.md)
