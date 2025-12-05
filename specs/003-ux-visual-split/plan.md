# Plan — UX + Visual Split (spec 003)

1. Create routes `/upload` and `/gallery` in the frontend router and wire header navigation.
2. Implement Upload page UI: dropzone, file previews, per-file progress, and retry.
3. Implement Gallery page: list albums, lazy-load media, and album detail view.
4. Add unit tests for UploadSession logic and e2e test for basic upload flow (optional: use Playwright).
5. Update documentation: `specs/003-ux-visual-split/spec.md` and `quickstart.md` entry.
6. Run Lighthouse and accessibility checks, adjust based on results.
7. Open PR `003-ux-visual-split` and request frontend review.
