# 25 · Video Streaming 🖥️ (Angular UI)

**Building block:** Chunked upload, transcoding pipeline, HLS adaptive bitrate, CDN.
**Tier:** Capstone · **Time:** 3–5 days · **Interview question:** **"Design YouTube/Netflix."**

> 📄 **STUB** — expand using projects 00–09 as the template. Portfolio piece with an Angular player.

## What you'll build
Upload a video, transcode it into multiple resolutions, and stream it with **adaptive bitrate (HLS)** so the
player picks quality based on bandwidth — the core of every video platform.

## Builds on
07 (blob storage), 04/09 (transcoding as a background pipeline), 13 (async processing), 02 (caching), 30 (CDN).

## Initial → Improve
**Initial (M1):** upload one MP4 to object storage; Angular plays it directly. No transcoding yet.
**Improvements:**
- **Transcoding worker**: on upload, a job runs **ffmpeg** to produce HLS renditions (240p/480p/720p) + segments.
- **Adaptive bitrate** playback (HLS `.m3u8` + segments); player switches quality automatically.
- **Chunked/resumable upload** for large files.
- Serve segments via a **cache/CDN**; thumbnails; **view counts** via async events.
- Status pipeline (uploaded → transcoding → ready); failure handling.

## Key questions this answers
- How does video streaming actually work? (segments, manifests, adaptive bitrate)
- How do you handle huge uploads and heavy transcoding? (chunking + async workers)
- How do you deliver at scale and cheaply? (object storage + CDN)
- How do you decouple a long pipeline from the user request?
