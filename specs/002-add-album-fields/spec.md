# Feature Specification: Add Album Create Fields

**Feature Branch**: `002-add-album-fields`
**Created**: 2025-12-05
**Status**: Draft
**Input**: User description: "The page to create albums does not ask for all the fields needed to create one"

**Tech Stack (expected for PicHub)**: Frontend: TypeScript + Vite; Backend: .NET 9/10 (Azure Functions); Storage: Azure SQL / Blob Storage

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Create album with necessary metadata (Priority: P1)

A site admin needs to create an album and specify all relevant metadata so the resulting album behaves as expected (upload allowed, file size limits, public token generation).

**Why this priority**: Creating albums is core to the product; missing metadata causes unexpected behaviour or broken upload flows.

**Independent Test**: Using the admin page, an admin fills the form, submits it, and the backend returns Created with a public token and public URL. The created album record in the database contains the provided metadata.

**Acceptance Scenarios**:

1. Given an admin is authenticated, when they provide Title, AllowUploads (checkbox), MaxFileSizeMb (optional numeric), and submit, then the backend returns 201 Created and the album has the requested properties.
2. Given invalid MaxFileSizeMb (e.g. negative), when submitting, then the backend returns 400 with a validation message.

---

### User Story 2 - Provide sensible defaults (Priority: P2)

When the admin omits optional fields like Description or MaxFileSizeMb, reasonable defaults are used (e.g., AllowUploads=true, AlbumSizeTshirt="M").

**Independent Test**: Create an album providing only Title; backend returns Created and stored album uses defaults.

**Acceptance Scenarios**:

1. Given an admin submits only Title, then the album is created with AllowUploads=true and AlbumSizeTshirt="M" unless explicitly overridden.

---

### Edge Cases

- Title empty -> validation error (400)
- MaxFileSizeMb too large ( > 500 ) -> validation error or capped to limit
- AllowUploads false but MaxFileSizeMb provided -> MaxFileSizeMb ignored or validated

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The admin UI MUST collect: `Title` (required), `Description` (optional), `AllowUploads` (boolean, default true), `MaxFileSizeMb` (optional positive integer), `AlbumSizeTshirt` (optional enum: XS/S/M/L/XL, default M).
- **FR-002**: The backend API MUST accept the additional fields in the create album request and persist them to the album record.
- **FR-003**: The backend MUST validate `MaxFileSizeMb` is positive when provided and return 400 on invalid input.
- **FR-004**: The frontend MUST send `X-Admin-Auth` header as already implemented for admin routes.
- **FR-005**: UI MUST show validation errors returned by backend.

### Key Entities

- **Album**: id, title, description, publicToken, allowUploads (bool), albumSizeTshirt (enum), totalBytesUsed, maxFileSizeMb

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: Admins can create albums with all required metadata in one flow (end-to-end test passes).
- **SC-002**: 100% of create-album API calls include Title; optional fields accepted or defaulted.
- **SC-003**: Validation errors for invalid inputs are returned within 500ms on average.

## Constitution Compliance (required)

- **Principles Affected**: UX Consistency, Data Validation, Security (admin auth), Observability (log creation events).
- **Compliance Checklist**: Input validation, unit tests for handler, integration test for end-to-end create flow.
- **Performance Goals**: N/A for this small feature.
- **Security & Privacy Notes**: Admin-only endpoint; ensure `X-Admin-Auth` validation unchanged. No PII involved.

## Assumptions

- Admin auth is handled by `AdminAuthService` and remains the same.
- Backend storage supports the additional fields.
- Frontend `API_BASE` and CORS are configured for local development.

## Implementation Notes (developer-facing)

- Frontend: Update admin form to include the new fields and add client-side validation for numeric values.
- Backend: Update `CreateAlbumRequest` DTO and `CreateAlbumCommand` to include fields; update handler to persist defaults when missing.
- Tests: Add unit tests for handler validation and integration test for the function endpoint.
