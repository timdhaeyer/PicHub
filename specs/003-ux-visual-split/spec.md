# Feature Specification: UX + Visual Split

**Feature Branch**: `003-ux-visual-split`
**Created**: 2025-12-05
**Status**: Draft
**Input**: User description: "I want to change 002-visual-style a bit. Not only a new visual style but also better UX. For example, split up the upload and gallery pages."

**Tech Stack (expected for PicHub)**: Frontend: Vite + TypeScript; Backend: .NET 10 (Azure Functions); Storage: Azure Storage; UI: small single-page app served by Vite

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Split Upload and Gallery (Priority: P1)

A user should be able to visit a dedicated Upload page to select files, preview, and start uploads, separate from the Gallery page which lists albums and media.

**Why this priority**: Separating concerns reduces user friction and simplifies the upload flow.

**Independent Test**: Navigate to `/upload` and `/gallery`. Upload page allows selecting files, shows previews, and triggers uploads. Gallery shows uploaded items independent of upload flow.

**Acceptance Scenarios**:

1. **Given** a signed-in user on Upload page, **When** they select images and click upload, **Then** images are uploaded and a success message displayed.
2. **Given** a user on Gallery page, **When** they view an album, **Then** media items load and are viewable without upload controls.

---

### User Story 2 - Streamlined Upload UX (Priority: P2)

Add inline previews, drag-and-drop area, progress bars, and clear success/failure states.

**Why this priority**: Increases reliability and improves perceived performance.

**Independent Test**: On Upload page, drag files into dropzone and observe previews, progress bars during upload, and post-upload confirmation.

**Acceptance Scenarios**:

1. **Given** files dropped into dropzone, **When** upload starts, **Then** per-file progress bars update until completion.
2. **Given** a failed upload, **When** the backend returns an error, **Then** the UI marks the file as failed and provides retry.

---

### User Story 3 - Clear Navigation & Accessibility (Priority: P3)

Improve navigation to explicitly switch between `Gallery` and `Upload` pages; ensure ARIA roles and keyboard focus management.

**Why this priority**: Improves discoverability and accessibility for keyboard and screen-reader users.

**Independent Test**: Use keyboard-only navigation to move between pages and ensure landmarks and skip links work.

**Acceptance Scenarios**:

1. **Given** the site header, **When** user tabs through, **Then** they can reach and activate `Gallery` and `Upload` links.
2. **Given** a focusable dialog (upload preview), **When** opened, **Then** focus is trapped and returned when closed.

---

### Edge Cases

- Large batches of files (50+) should show aggregated progress and not block the UI.
- Upload interruptions (network loss) must be recoverable with retry.
- Extremely large images should show a warning or require client-side resizing.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: Add route `/upload` and `/gallery` and navigation links in header.
- **FR-002**: Upload page must provide drag-and-drop, file list with previews, per-file progress, and retry for failures.
- **FR-003**: Gallery page must list albums and support pagination or lazy loading.
- **FR-004**: Existing `index.html` header must be updated to include navigation links and active states.
- **FR-005**: Maintain all accessibility features introduced in visual-style (skip link, focus outlines, brand alt text).

_Clarifications (max 3)_:

- **FR-006**: [NEEDS CLARIFICATION: Should uploads remain single-step (select->upload) or support background queued uploads?]

### Key Entities

- **UploadSession**: transient client-side object representing selected files and upload states (queued, uploading, success, failed)
- **MediaItem**: existing server-side model (filename, url, albumId, metadata)
- **Album**: grouping of MediaItems (id, title, visibility)

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: Users can navigate to `/upload` and complete an upload in under 2 minutes (measured in testing).
- **SC-002**: Upload success rate > 98% for single-file uploads under typical network conditions.
- **SC-003**: Gallery page initial paint time < 1.5s on 3G emulated device with cached assets.
- **SC-004**: Keyboard navigation allows reaching primary actions (Upload, Gallery) within 3 tab presses.

## Constitution Compliance (required)

- **Principles Affected**: UX Consistency, Accessibility, Testing Standards, Observability.
- **Compliance Checklist**: Implement automated tests for upload flow; include unit tests for UploadSession state transitions; add telemetry for upload success/failure events.
- **Performance Goals**: Gallery initial load p95 < 1500ms on constrained network.
- **Security & Privacy Notes**: Ensure uploads validate file types and enforce per-user quotas. Do not expose pre-signed URLs publicly for private albums.
