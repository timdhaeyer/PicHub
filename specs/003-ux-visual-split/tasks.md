# Tasks for UX + Visual Split (spec 003)

Phase 1 — Setup

- [x] T001 Create feature branch and scaffold spec files in `specs/003-ux-visual-split/` (already created)
- [ ] T002 [P] Add skeleton pages `frontend/src/pages/upload.ts` and `frontend/src/pages/gallery.ts`

Phase 2 — Foundational

- [ ] T003 [P] Update header markup to include `Upload` and `Gallery` links in `frontend/src/index.html`
- [ ] T004 [P] Add router registration for `/upload` and `/gallery` in `frontend/src/main.ts`
- [ ] T005 [P] Add basic navigation styles (active state) in `frontend/src/styles.css`

Phase 3 — User Story 1: Split Upload and Gallery (P1)

- [ ] T006 [US1] Implement Upload page skeleton (dropzone placeholder, file list area) in `frontend/src/pages/upload.ts`
- [ ] T007 [US1] Implement Gallery page skeleton (album list placeholder) in `frontend/src/pages/gallery.ts`
- [ ] T008 [US1] Verify routes and header navigation work end-to-end by running dev server (`frontend/`): `npm run dev` and checking `/upload` and `/gallery`

Phase 4 — User Story 2: Streamlined Upload UX (P2)

- [ ] T009 [P] Create client `UploadSession` module in `frontend/src/lib/upload_session.ts` (state: queued, uploading, success, failed)
- [ ] T010 [US2] Add drag-and-drop Dropzone component in `frontend/src/components/Dropzone.ts` and integrate previews in `frontend/src/pages/upload.ts`
- [ ] T011 [US2] Implement per-file upload progress UI in `frontend/src/components/UploadList.ts` and backend upload caller in `frontend/src/lib/upload_client.ts`

Phase 5 — User Story 3: Navigation & Accessibility (P3)

- [ ] T012 [US3] Add keyboard navigation and ARIA roles to header and pages (`frontend/src/index.html`, `frontend/src/pages/*.ts`)
- [ ] T013 [US3] Ensure skip-link and focus outlines in `frontend/src/styles.css` and verify tab order across header -> main content

Phase 6 — QA & Docs

- [ ] T014 [P] Add unit tests for `UploadSession` in `frontend/tests/upload_session.test.ts`
- [ ] T015 Create `specs/003-ux-visual-split/quickstart.md` with usage notes and developer steps
- [ ] T016 Create PR `003-ux-visual-split` on GitHub and request frontend review (link PR in `specs/003-ux-visual-split/`)

Phase 7 — Review & Polish

- [ ] T017 Address PR review comments and update `frontend/src/` and `specs/003-ux-visual-split/` accordingly
