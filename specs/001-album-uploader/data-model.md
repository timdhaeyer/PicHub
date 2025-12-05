# Data Model: Album Uploader

Entities and fields (high-level, technology-agnostic)

1. Album

- id: GUID (PK)
- title: string (required)
- description: string (optional)
- created_by: GUID (admin user id)
- created_at: timestamp
- public_token: string (unguessable)
- allow_uploads: boolean
- max_file_size_mb: integer
- album_size_tshirt: enum (XS, S, M, L, XL)
- total_bytes_used: bigint
- retention_days: integer
- expires_at: timestamp (nullable)

2. MediaItem

- id: GUID (PK)
- album_id: GUID (FK -> Album.id)
- filename: string
- content_type: string
- size_bytes: bigint
- storage_path: string (blob container + path)
- thumbnail_path: string (blob path)
- uploaded_at: timestamp
- uploaded_by: GUID (nullable)
- is_processed: boolean (thumbnail/metadata processed)

3. AdminUser (reference)

- id: GUID
- display_name: string
- email: string
- role: enum

Indexes and queries

- Album.public_token -> lookup albums by token (frequent, non-unique) (index)
- MediaItem.album_id -> list media for album (index)
- Album.created_by -> admin operations

Validation rules

- filename sanitization: only allow safe unicode and length limits
- content_type whitelist: images (jpeg/png/webp), video (mp4, webm)
- size check: per-file <= album.max_file_size_mb
- total album size: reject upload if new total > album cap

State transitions

- MediaItem: uploaded -> processing -> processed
- Album: active -> expired/deleted

Privacy and retention

- When an album expires or deleted: delete blobs and then delete metadata rows in SQL; record audit entries for deletions/exports.

\*\*\* End of data-model.md
