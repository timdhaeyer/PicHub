Implementation Plan: Visual style (spec 002)

Goal: Deliver a cohesive visual identity for PicHub (logo assets, CSS design tokens, header integration, favicon), ensure accessibility, and provide a short style guide for developers.

Phases

1. Research & Decisions (1 day)

- Confirm color contrast meets WCAG AA (text-on-bg and UI elements). Use `--pichub-dark` on `--pichub-bg` and test.
- Choose web-safe fallback fonts and add Inter as preferred.
- Decide on primary logo variants (stacked: logo+wordmark, horizontal: logo left of wordmark).

2. Assets (0.5 day)

- Export `pichub-logo.svg` (stacked) — done.
- Create favicon PNG/ICO (32x32, 16x16) and place in `frontend/public/`.
- Generate optimized SVG variations (mono, inverted).

3. Implementation (1 day)

- Add CSS variables in `frontend/src/styles.css` (done partially).
- Add header template in `index.html` (done).
- Replace inline colors in pages with CSS variables.

4. QA & Accessibility (0.5 day)

- Run Lighthouse contrast audits and manually verify AA for body text and interactive elements.
- Check mobile header layout at 320px and 480px.

5. Developer guidance & PR (0.5 day)

- Add `specs/002-visual-style/quickstart.md` with usage rules, spacing, and color tokens.
- Open PR and request review.

Deliverables

- `frontend/src/assets/pichub-logo.svg` (stacked)
- `frontend/public/favicon.ico` (32x32)
- `frontend/src/styles.css` with CSS variables and header styles
- `specs/002-visual-style/quickstart.md`

Acceptance criteria

- Header displays logo and title at mobile and desktop widths
- Color contrast passes WCAG AA for body text
- Developers can use variables documented in quickstart

Risks & Mitigations

- Slight color mismatches in different displays: provide accessible fallback palette and test on multiple screens
- SVG rendering differences: provide PNG fallbacks for critical icons

\*\*\* End of plan
