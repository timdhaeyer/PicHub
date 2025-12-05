# Tasks for Visual Style (spec 002)

Phase 1: Setup

- [x] T001 Add `frontend/src/assets/pichub-logo.svg` and confirm `index.html` references `src/assets/pichub-logo.svg`
- [ ] T002 Add `frontend/public/favicon.ico` and sample PNGs at `frontend/public/favicon-32.png` and `frontend/public/favicon-16.png` and link them in `frontend/src/index.html`

Phase 2: Foundational

- [ ] T003 [P] Define CSS variables in `frontend/src/styles.css` for primary (`--pichub-orange`), dark (`--pichub-dark`), background (`--pichub-bg`), and muted (`--pichub-muted`)
- [ ] T004 [P] Add web-safe font stack and preferred font fallback in `frontend/src/styles.css`
- [ ] T005 Verify color contrast for text using the variables in `frontend/src/styles.css` and document results in `specs/002-visual-style/qa.md`

Phase 3: Implementation - Header

- [ ] T006 [US1] Add header HTML markup to `frontend/src/index.html` including logo image at `src/assets/pichub-logo.svg` and site title
- [ ] T007 [US1] Add responsive header CSS to `frontend/src/styles.css` to support mobile and desktop layouts
- [ ] T008 [US1] Replace hard-coded colors in `frontend/src/pages` files with CSS variables defined in `frontend/src/styles.css` (update files under `frontend/src/pages/`)

Phase 4: Assets

- [ ] T009 [P] Generate favicon files `frontend/public/favicon-32.png`, `frontend/public/favicon-16.png`, and `frontend/public/favicon.ico` from `frontend/src/assets/pichub-logo.svg`
- [ ] T010 [P] Add inverted/mono logo variants at `frontend/src/assets/pichub-logo-inverted.svg` and `frontend/src/assets/pichub-logo-mono.svg`

Phase 5: QA & Docs

- [ ] T011 Create `specs/002-visual-style/quickstart.md` with logo usage, spacing rules, and token list (colors and fonts)
- [ ] T012 [P] Run Lighthouse and accessibility checks against the dev site and save results to `specs/002-visual-style/qa.md`

Phase 6: PR & Review

- [ ] T013 Create PR `feat/visual-style` with the visual assets and request frontend review
- [ ] T014 Address PR review comments and update files under `frontend/src/` and `specs/002-visual-style/` as needed

Notes:

- Tasks marked `[P]` are parallelizable.
- Tasks labeled `[US1]` are the primary visible user story for this feature (header + brand integration).

Generated: 2025-12-05
