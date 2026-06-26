# Interview angle — 07 · Blob / Object Storage

## Questions this prepares you for

- **"Where do you store user uploads (images/video/files)?"**
  → Object storage (S3/Blob/MinIO) for the bytes; metadata in your DB. Never the bytes in the relational DB.

- **"What's a presigned URL and why use it?"**
  → A signed, expiring URL letting a client upload/download a specific object directly to/from the store, so
  your API doesn't proxy bytes — saves bandwidth and scales the transfer path independently.

- **"How is object storage different from a database or file system?"**
  → Flat key→object namespace over HTTP, built for scale and durability, no transactions/queries, objects are
  replaced not edited in place.

- **"How is object storage so durable?"**
  → Replication across zones + erasure coding (data + parity shards) so it survives hardware loss.

- **"Design Dropbox/S3" (advanced):**
  → metadata service + object store + presigned transfers + chunking/dedup + lifecycle. This project is the
  storage core of that answer (chunking/sync come in 13/25).

## Trade-offs
- Direct (presigned) transfers scale but move access control to URL signing/expiry — get that right.
- Eventually-consistent listing in some stores; strong read-after-write in modern S3.
- Cost grows with storage + egress → lifecycle policies and CDN (project 13/30) matter.
