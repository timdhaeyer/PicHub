# Research: Album Uploader - Decisions

This document resolves the open `NEEDS CLARIFICATION` items from the implementation plan.

1. Metadata store: Azure SQL vs Cosmos DB

Decision: Use Azure SQL (relational) for album and media metadata in the initial implementation.

Rationale:

- Album, media, and admin relationships are naturally relational (joins between album and media, queries for totals, retention timestamps).
- Easier transactional guarantees for delete/export workflows.
- Lower operational complexity and predictable cost for initial scale.

Alternatives considered:

- Cosmos DB: better for massive scale and flexible documents, but complicates queries that need strong relational semantics (counts, joins) and may increase cost for small-medium datasets.

Migration note: Design the metadata layer with a repository abstraction so a future migration to Cosmos DB (or other stores) is straightforward.

2. Upload strategy: resumable chunked uploads vs single-shot uploads

Decision: Implement chunked resumable uploads for larger files (>= 10 MB) and direct single-shot uploads for small images (< 10 MB). Use client-side chunking with server-side assembly or Azure Blob staged blocks.

Rationale:

- Mobile networks are lossy and resumable uploads improve user experience for large videos.
- Blob staged blocks (block blobs) are supported by Azure SDK and allow efficient assembly.

Alternatives considered:

- Presigned direct-to-blob uploads simplify the server but require careful SAS handling and partial-upload cleanup. We'll use function-mediated SAS issuance for public link uploads.

3. Thumbnail generation

Decision: Generate thumbnails server-side in a background queue triggered by blob upload events (Azure Functions BlobTrigger or Event Grid + Function). Use `sharp` on Linux consumption or a dedicated thumbnail service for heavy workloads.

Rationale:

- Keeps client simple and ensures consistent thumbnail sizes/formatting.
- Allows offloading compute from upload request path for responsiveness.

Alternatives considered:

- Client-side thumbnails reduce server CPU but produce inconsistent results and security concerns.

4. Other infra notes

- Use Azurite for local dev (already scaffolded). Keep blob container names and conventions consistent between local and cloud.
- Retention cleanup: use Storage lifecycle rules for blob deletion plus a scheduled function to remove metadata records in SQL that correspond to deleted blobs.

Decision Summary (short):

- Metadata: Azure SQL
- Uploads: chunked resumable for >=10MB, single-shot for <10MB
- Thumbnails: server-side background generation using Azure Functions triggered by blob events
- Local dev: Azurite

\*\*\* End of research.md
