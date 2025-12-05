# Implementation Plan — 003-ux-visual-split

Milestones

1. Routing & Header (1d)
   - Add `/upload` and `/gallery` routes
   - Update header navigation with active states and skip link focus
   - Acceptance: Navigation links work, route renders skeleton pages

2. Upload UX MVP (2d)
   - Implement dropzone, file list with previews, per-file progress bars
   - Client-side UploadSession state machine with retry
   - Acceptance: Select files, start upload, progress visible, success/failure shown

3. Gallery MVP (2d)
   - List albums, click into album to view media items
   - Lazy-load media thumbs (intersection observer)
   - Acceptance: Gallery lists albums, opens album, thumbs load as scrolled

4. Tests & Accessibility (1d)
   - Unit tests for UploadSession
   - Keyboard navigation tests and ARIA roles verified
   - Acceptance: Tests pass and basic keyboard flows are validated

5. QA & PR (1d)
   - Run Lighthouse (local), fix top issues, update docs
   - Create PR, request review

Notes
- Total estimated effort: 7 working days (MVP + tests + PR).
- Start with routing and header to keep changes isolated and reviewable.
