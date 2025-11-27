# Feature Specification: Album Uploader

**Feature Branch**: `001-album-uploader`  
**Created**: 2025-11-27  
**Status**: Draft  
**Input**: User description: "Build an easy to use photo/video uploader. Admins can create new albums and each album has a public link that can be shared with users. Main target is smartphones/tablets. Next to the uploads users can also see the pictures and video's in the album. Admins can download the album as a zip file, but users can't. Acceptance: users can access album, upload foto's and see them (no delete) admins can create new albums, share the link, download all pictures in zip and delete an album again"

**Tech Stack (PicHub)**: Frontend: React (Vite, TypeScript); Backend: C# (.NET 10, Azure Functions); Storage: Azure Table Storage / Azure SQL / Cosmos DB; File Storage: Azure Storage Account (Blobs); IaC: Bicep + GitHub Actions

## User Scenarios & Testing (mandatory)

### User Story 1 - View & Upload (Priority: P1)

As a mobile/tablet user, I can open a public album link, view all photos and videos in the album, and upload new photos/videos from my device so I can share content with the album owner.

**Why this priority**: Core user-facing behavior — must work on the first release.

**Independent Test**: Open a public album link on a mobile device, verify thumbnails load, upload 1 image and 1 short video, verify they appear in the album view within 10s.

**Acceptance Scenarios**:

1. Given a public album link, When a user visits the link, Then the album's thumbnails and metadata are visible and accessible.
2. Given the album page is open on a smartphone, When the user selects a photo/video and confirms upload, Then the media is uploaded and appears in the album (no delete option available to them).

---

### User Story 2 - Admin Album Management (Priority: P1)

As an admin, I can create a new album, get a shareable public link, and delete the album when needed.

**Why this priority**: Admins manage content lifecycle and sharing.

**Independent Test**: Log in as admin, create an album titled "Test Album", obtain share link, open link in private browser, verify view access. Delete the album and verify public link returns 404.

**Acceptance Scenarios**:

1. Given admin credentials, When the admin creates an album, Then a unique public link is generated and copyable.
2. Given an existing album, When the admin deletes it, Then all metadata and references are removed and the public link is invalidated.

---

### User Story 3 - Admin Download (Priority: P2)

As an admin, I can download all the pictures and videos in an album as a single ZIP archive for backup or offline consumption.

**Independent Test**: Admin clicks 'Download Album', a ZIP containing all media files is generated and downloads successfully; the archive contents match the album contents.

**Acceptance Scenarios**:

1. Given an album with media, When admin requests download, Then the backend generates a ZIP and the browser receives it within an acceptable time (depends on album size).

---

### Edge Cases

- Upload interrupted (network loss): uploads must be resumable or provide clear failure messages and retry options.
- Very large files: enforce configurable max file size and provide error messaging.
- Unsupported file types: validate client- and server-side and show user-friendly errors.
- High concurrency: ensure storage and functions scale and SLOs are met.

## Requirements (mandatory)

### Functional Requirements

- **FR-001**: Admins MUST be able to create albums with a title, optional description, and expiration (optional) metadata.
- **FR-002**: Each album MUST expose a unique public, unlisted link (unguessable token) that permits read and upload access for anonymous users (uploads allowed without login) as configured by admin.
- **FR-003**: Anonymous users (visiting the public link) MUST be able to view thumbnails and stream/download individual media items.
- **FR-004**: Anonymous users MUST be able to upload photos and videos to an album via the public link, subject to file type and size limits defined by admin/CI defaults (default 50 MB image/video per file).
- **FR-004a**: Admins MUST be able to select a maximum album size using T-shirt sizing when creating the album: `XS` (up to 1 GB), `S` (up to 5 GB), `M` (up to 10 GB), `L` (up to 50 GB), `XL` (up to 100 GB). The selected size enforces total album storage cap; uploads that would exceed the cap are rejected with clear messaging.
- **FR-005**: Admins MUST be able to generate and copy the public share link for an album.
- **FR-006**: Admins MUST be able to download all album assets as a ZIP archive.
- **FR-007**: Admins MUST be able to delete an album, which removes metadata and prevents further access to the public link.
- **FR-008**: Users MUST NOT be able to delete media or modify album metadata via the public link.

### Non-Functional Requirements

- **NFR-001**: Mobile-first responsive UI optimized for touch; primary user flows complete within 3 taps.
- **NFR-002**: Upload latency: small images (<1MB) should be visible in album view within 5 seconds 95% of the time under typical mobile networks.
- **NFR-003**: Availability: Storage and functions must be highly available; design for eventual consistency where needed.
- **NFR-004**: Security & Privacy: Media in storage MUST be protected with appropriate access policies; public links MUST be unguessable tokens, and sensitive metadata (PII) MUST be redacted or protected.

### Key Entities

- **Album**: id, title, description, createdBy (admin id), createdAt, publicToken, options (allowUploads: bool, maxFileSizeMB, expiresAt)
  - albumSizeTshirt (XS/S/M/L/XL), retentionPolicyDays
- **MediaItem**: id, albumId, filename, contentType, sizeBytes, storagePath, thumbnailPath, uploadedAt, uploadedBy (nullable for anonymous)
- **AdminUser**: id, displayName, email, role

## Success Criteria (mandatory)

### Measurable Outcomes

- **SC-001**: Users can open a public album link and view thumbnails within 3 seconds median on mobile networks.
- **SC-002**: Users can successfully upload images (<1MB) and see them appear in the album within 10 seconds in 95% of attempts.
- **SC-003**: Admins can create an album and obtain a share link within 10 seconds.
- **SC-004**: Admin download completes for albums up to 500 items within a reasonable window (configurable, e.g., 2 minutes for 500 small images); for larger sets use background export.

## Retention & GDPR

- **Retention**: By default, album media and metadata are retained for 30 days. The system MUST offer automatic cleanup using Storage Account lifecycle policies or scheduled functions that remove media and metadata after 30 days.
- **Admin override**: Admins may set a shorter expiry on album creation (e.g., 7 days) but NOT longer than 30 days unless explicitly permitted by policy.
- **GDPR/Privacy**: When an album is deleted or items expire, all associated blobs, thumbnails, and metadata MUST be permanently removed. The system MUST provide a way for admins to request data export (for compliance) and a record of deletions and exports. PII in metadata MUST be minimized and stored encrypted at rest.

## Constitution Compliance (required)

- **Principles Affected**: Code Quality, Testing Standards, UX Consistency, Performance, Observability, Versioning, Security & Privacy.
- **Compliance Checklist**:
  - Code Quality: Follow repo linters and analyzers; PRs for this feature MUST include unit tests and follow naming and SRP rules.
  - Testing Standards: Provide unit tests for upload handling, integration tests for end-to-end upload/download flow, and contract tests for the public link API.
  - UX Consistency: Use shared component library; ensure a11y checks and responsive breakpoints.
  - Performance: Define p95 upload latency target and include a performance smoke test harness.
  - Observability: Emit upload, download, and error metrics; capture traces for long-running ZIP exports.
  - Versioning: Public APIs used by external clients must follow semantic versioning and be documented.
  - Security & Privacy: Public links implemented as unguessable tokens; server-side validation of content-type and size; Azure Storage SAS tokens or function-mediated access for files.
- **Performance Goals**: Upload p95 < 10s for <1MB images on typical mobile networks; album view median <3s for thumbnail rendering.
- **Security & Privacy Notes**: Media storage blobs are private; access via signed URLs (SAS) or function proxies. Public album link grants only scoped upload/read for that album. Any PII in metadata must be minimized.

## Assumptions

- Anonymous uploads are acceptable for public links — admins control whether uploads are enabled per album.
- Authentication/authorization for admins already exists or will be provided by a separate identity service (e.g., Azure AD B2C or internal user store).
- Thumbnails will be generated by a server-side function (or durable function queue) after upload.
- ZIP generation is performed server-side, and large exports may be done asynchronously with notification to the admin if exceeding synchronous timeouts.

## Testing Plan

- Unit tests: upload validation, thumbnail generation trigger, public link token validation, album CRUD for admin paths.
- Integration tests: end-to-end upload and view via public link against dev environment (use Azure Storage emulator or test account), ZIP export flow for admins.
- Performance tests: smoke tests for upload latency (scripted uploading with simulated mobile bandwidth) and album view rendering.
- Security tests: dependency scanning, secret scanning, and edge-case tests for content-type spoofing.

## Implementation Notes (high-level)

- Frontend: mobile-first React (Vite, TypeScript) using a shared component library and the device file picker; upload client should use resumable uploads where possible (chunked uploads), and show progress.
- Backend: C# (.NET 10) Azure Functions (HTTP-triggered) endpoints for: create album (admin), get album (public), upload media (public token), list media (public), admin download (authenticated), delete album (admin).
- Storage: blobs for media; store metadata in Azure SQL or Cosmos DB; thumbnails stored alongside media or in a dedicated container.
- IaC: Bicep templates for storage account, function app, SQL/Cosmos, and related RBAC and network rules.

## Deliverables

- Frontend album page with upload flow and thumbnail gallery.
- Admin UI for album creation, link generation, and deletion.
- Backend functions for upload, listing, and admin export.
- CI pipeline additions: linters, tests, security scans, IaC deploy to test.

**_ End of spec _**
