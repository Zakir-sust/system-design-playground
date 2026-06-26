# 24 · Proximity / Geo Service

**Building block:** Geospatial indexing (geohashing/H3), nearest-neighbor, live location.
**Tier:** Scale/distributed · **Time:** 1–2 days · **Interview question:** **"Design Yelp / Uber proximity (nearby drivers/places)."**

> 📄 **STUB** — expand using projects 00–09 as the template.

## What you'll build
"Find things near me" — given a location, return nearby places/drivers ranked by distance, fast, with live
updates as things move.

## Builds on
00 (API+DB), 02 (Redis geo commands), 04 (live location update events).

## Initial → Improve
**Initial (M1):** naive **bounding-box** query in Postgres (`lat/lng BETWEEN ...`) — works small, scans big.
**Improvements:**
- **Geohashing** (or H3) to bucket coordinates; index by prefix so "nearby" = "same/adjacent buckets."
- **Redis GEO** commands (`GEOADD`/`GEOSEARCH`) for fast radius queries; or PostGIS spatial index.
- **Rank** by true distance; handle bucket-edge neighbors.
- **Live updates** — drivers push location via events (project 04); expire stale positions.
- Precision vs performance trade-off (geohash length).

## Key questions this answers
- How do you index locations for fast "nearby" queries? (geohash/quadtree/H3)
- Why does a naive bounding-box query fail at scale?
- How do you handle constantly-moving entities (drivers)? (updates, TTL)
- How do you rank and handle results spanning bucket boundaries?
