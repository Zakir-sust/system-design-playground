# 11 · Pastebin

**Building block:** Blob storage for large content, expiration, access control.
**Tier:** Real app · **Time:** ~1 day · **Interview question:** **"Design Pastebin."**

> 📄 **STUB** — expand using projects 00–09 as the template.

## What you'll build
Create/read text "pastes" with a shareable link. Small pastes live in Postgres; large ones go to object
storage (project 07). Pastes can be private/unlisted and can expire.

## Builds on
07 (blob storage), 02 (cache reads), 06 (short ids/links), 09 (expiry cleanup job).

## Initial → Improve
**Initial (M1):** `POST /pastes` stores text + returns a link; `GET /{id}` returns it.
**Improvements:**
- Large pastes (> N KB) stored in **MinIO**; metadata in Postgres (the split from project 07).
- **TTL/expiration** (auto-delete via a recurring job, project 09).
- **Visibility**: public / unlisted / private (access control).
- **View limits** ("burn after reading"), syntax-highlight metadata.
- Cache hot pastes (project 02).

## Key questions this answers
- When does content belong in the DB vs object storage? (size threshold)
- How do you implement expiration at scale? (lazy delete vs scheduled cleanup)
- Access control for shareable links (unguessable id vs auth)?
