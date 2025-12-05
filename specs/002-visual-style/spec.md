# Visual style for PicHub

## Summary
Create a modern, friendly visual identity for PicHub that includes logo assets (SVG + favicon), color palette, typography, and updated frontend styles. Use the provided logo image (orange camera with dark lens and light accent) as the primary mark. Deliverables include SVG logo(s), CSS variables and styles, a favicon, and updated `index.html`/`styles.css` integration.

## User Scenarios
- Developer: Integrates the new logo and styles into the frontend repo to improve branding consistency.
- Designer: Reviews the palette, makes small iterations to contrast and spacing.
- End-user: Sees the new header/logo across PicHub pages and experiences improved visual hierarchy.

## Functional Requirements
- Provide an accessible SVG logo (`frontend/src/assets/pichub-logo.svg`) and a favicon.
- Add CSS variables for primary, accent, dark, and background colors in `frontend/src/styles.css`.
- Add a responsive header with logo and site title in `frontend/src/index.html`.
- Ensure header is mobile-friendly and the logo scales properly.
- Ensure color contrast meets WCAG AA for body text.

## Success Criteria
- Logo files added and referenced in `index.html`.
- `styles.css` defines CSS variables and component styles; header looks correct at 320px and 1280px widths.
- All frontend unit tests still pass after changes (no regressions caused by style edits).

## Key Entities
- SVG logo file
- CSS variables and styles

## Assumptions
- The frontend is a simple Vite app; updating `index.html` and `styles.css` is sufficient.
- No backend changes required.

## Out of Scope
- Full design system overhaul.
- Marketing assets beyond favicon + SVG.

## Deliverables
- `frontend/src/assets/pichub-logo.svg`
- favicon (32x32) at `frontend/public/favicon.ico` (optional)
- CSS updates in `frontend/src/styles.css`
- `index.html` header update

