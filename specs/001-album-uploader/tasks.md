# Tasks: Album Uploader (001-album-uploader)

**Input**: Design documents in `specs/001-album-uploader/`

## Phase 1: Setup (Shared Infrastructure)

- [ ] T001 Initialize backend C# Azure Functions project in `backend/PicHub.AlbumUploader/` (ensure csproj TFM matches plan)
- [ ] T002 Initialize frontend React (Vite, TypeScript) project in `frontend/` (verify `package.json`, `src/`)
- [ ] T003 [P] Add linting/formatting for frontend (`frontend/.eslintrc`, `frontend/.prettierrc`) and backend (`.editorconfig`, dotnet analyzers) in repo root
- [ ] T004 [P] Add local Azurite run script and example `frontend/local-dev/.env.example` to document `AZURE_STORAGE_CONNECTION_STRING`
- [ ] T005 [P] Add initial README and quickstart in `specs/001-album-uploader/quickstart.md`
- [ ] T006 Create GitHub Actions CI stub file `.github/workflows/ci.yml` that runs linters and basic tests

## Phase 2: Foundational (Blocking Prerequisites)

- [ ] T007 Configure Azure Blob storage container conventions and local Azurite setup in `infra/` (IaC skeleton)
- [ ] T008 [P] Scaffold database schema and migrations for Azure SQL in `backend/migrations/` and put initial entity scripts in `specs/001-album-uploader/data-model.md`
- [ ] T008a Decide metadata store (Azure SQL vs Cosmos DB) and record decision in `specs/001-album-uploader/research.md` (block migrations until decided)
- [ ] T009 [P] Implement authentication/authorization placeholder for admin flows in `backend/src/Auth/` (stub middleware + docs in `backend/README.md`)
- [ ] T009 [P] Implement authentication/authorization placeholder for admin flows in `backend/src/Auth/` (stub middleware + docs in `backend/README.md`)
- [ ] T009a Implement RBAC and contract tests to ensure admin-only endpoints (create/delete/download) are protected (add tests in `tests/contract/`)
- [ ] T010 Implement configuration management and secret handling (use `local.settings.json` for Functions and `frontend/.env` for dev)
- [ ] T011 [P] Add observability scaffolding: Application Insights config placeholder in `infra/` and logging helpers in `backend/src/Logging/`
- [ ] T012 [P] Create integration test harness that can run against Azurite in `tests/integration/` with README `tests/integration/README.md`
- [ ] T013 Deploy IaC skeleton (Bicep) for dev resources in `infra/main.bicep` (storage account + function app skeleton)

## Phase 3: User Story 1 - View & Upload (Priority: P1) [US1]

Goal: Allow anonymous users to open a public album link, view thumbnails, and upload photos/videos.

- [ ] T014 [P] [US1] Add contract tests for public album endpoints in `tests/contract/test_albums_public.*`
- [ ] T015 [P] [US1] Add unit tests for upload validation and album quota logic in `tests/unit/`
- [ ] T016 [US1] Create `Album` model in `backend/src/Models/Album.cs` (schema documented in `specs/001-album-uploader/data-model.md`)
- [ ] T017 [US1] Create `MediaItem` model in `backend/src/Models/MediaItem.cs`
- [ ] T018 [US1] Implement public GET endpoint `GET /api/albums/{publicToken}` in `backend/src/Functions/AlbumsFunction.cs`
- [ ] T019 [US1] Implement public POST upload endpoint `POST /api/albums/{publicToken}` in `backend/src/Functions/UploadFunction.cs` with multipart handling and Azure Blob upload using `Azure.Storage.Blobs`
- [ ] T019 [US1] Implement public POST upload endpoint `POST /api/albums/{publicToken}` in `backend/PicHub.AlbumUploader/Functions/UploadFunction.cs` with multipart handling and Azure Blob upload using `Azure.Storage.Blobs`
- [ ] T020 [US1] Enforce per-file and album cap checks in `backend/src/Services/QuotaService.cs`
- [ ] T021 [US1] Implement frontend album page `frontend/src/pages/AlbumPage.tsx` with file picker, upload progress and thumbnail gallery
- [ ] T022 [US1] Wire frontend to backend endpoints via `frontend/src/services/api.ts`
- [ ] T023 [US1] Add thumbnail generation trigger (blob event) scaffold in `backend/src/Functions/ThumbnailProcessor.cs`

## Phase 4: User Story 2 - Admin Album Management (Priority: P1) [US2]

Goal: Allow admins to create albums, generate public links, and delete albums.

- [ ] T024 [P] [US2] Add contract tests for admin endpoints in `tests/contract/test_admin_albums.*`
- [ ] T025 [US2] Implement admin POST `POST /api/admin/albums` in `backend/src/Functions/AdminAlbumsFunction.cs` (creates Album, sets T-shirt size, retention)
- [ ] T025 [US2] Implement admin POST `POST /api/admin/albums` in `backend/PicHub.AlbumUploader/Functions/AdminAlbumsFunction.cs` (creates Album, sets T-shirt size, retention)
- [ ] T026 [US2] Implement admin DELETE `DELETE /api/admin/albums/{albumId}` in `backend/src/Functions/AdminAlbumsFunction.cs` (deletes metadata and triggers blob deletion)
- [ ] T026 [US2] Implement admin DELETE `DELETE /api/admin/albums/{albumId}` in `backend/PicHub.AlbumUploader/Functions/AdminAlbumsFunction.cs` (deletes metadata and triggers blob deletion)

## Security/Policy Tasks

- [ ] T039 [P] Implement data retention policy enforcement and audit logging for deletions and exports (function or scheduled job that enforces retention and records operations)
- [ ] T040 [P] Implement encryption-at-rest verification and key management documentation (KMS/TDE settings) and add CI verification steps
- [ ] T027 [US2] Implement public-token creation & storage in `backend/src/Services/TokenService.cs`
- [ ] T028 [US2] Add admin UI page `frontend/src/pages/AdminAlbums.tsx` for create/delete and link copy, guarded by admin auth stub
- [ ] T029 [US2] Ensure deletion audit log entry recorded in `backend/src/Services/AuditService.cs` and stored in `backend/logs/` or DB

## Phase 5: User Story 3 - Admin Download (Priority: P2) [US3]

Goal: Enable admins to download an album as a ZIP archive (sync for small, async for large).

- [ ] T030 [US3] Implement admin download endpoint `GET /api/admin/albums/{albumId}/download` in `backend/src/Functions/DownloadFunction.cs` that streams ZIP for small sets
- [ ] T031 [US3] Implement async export pipeline for large exports (Durable Function or background queue) in `backend/src/Functions/ExportOrchestrator.cs`
- [ ] T032 [US3] Add frontend admin download button and progress UI in `frontend/src/pages/AdminAlbums.tsx`
- [ ] T033 [US3] Add tests for ZIP generation and content verification in `tests/integration/test_export_*`

## Phase N: Polish & Cross-Cutting Concerns

- [ ] T034 [P] Add accessibility (a11y) checks for frontend components and automated a11y tests in `tests/a11y/`
- [ ] T035 [P] Add performance smoke tests for upload and album view in `tests/perf/` (scripts and README)
- [ ] T036 [P] Add security scans to CI (`.github/workflows/ci.yml` step) and dependency scanning config in repo root
- [ ] T037 [ ] Document operational runbook `docs/operational/album-uploader-runbook.md` with retention, deletion, and export procedures
- [ ] T038 [ ] Prepare PR with all changes on branch `001-album-uploader` and include checklist referencing the constitution in PR description

## Dependencies

- Database migrations (T008) must be applied before backend endpoints (T018..T026) are run against a real DB.
- IaC skeleton (T013) should be available for integration tests that run in a cloud-like dev environment.

## Parallel Execution Suggestions

- Frontend pages (T021, T028, T032) can be implemented in parallel with backend endpoints (T018, T025, T030) once contracts (T014, T024) are agreed.
- Linting/CI tasks (T003, T006, T036) can be worked on in parallel with functional implementation.

## Implementation Strategy

- MVP scope: Deliver US1 (View & Upload) + minimal admin create (US2 partial: POST album + token) to enable basic sharing and uploads. Defer large ZIP async export (US3 async) to Phase 2 after MVP.
- Iterate: Implement unit tests first for core logic, then contract tests, then UI.

\*\*\* End of tasks.md
