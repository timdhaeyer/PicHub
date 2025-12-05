# Quickstart — Visual Style (spec 002)

This quickstart describes how to use the PicHub visual tokens, logo assets, and spacing rules when implementing UI components.

Principles

- Use tokens (CSS variables) from `frontend/src/styles.css` for colors, spacing, and radii.
- Prefer scalable SVGs for logos; provide PNG fallbacks from `/public` for legacy use.
- Reserve `--pichub-orange` for brand accents; use `--pichub-orange-accessible` for body text on light backgrounds.

Logo files

- `frontend/src/assets/pichub-logo.svg` — primary stacked logo (SVG)
- `frontend/src/assets/pichub-logo-mono.svg` — mono variant (SVG)
- `frontend/src/assets/pichub-logo-inverted.svg` — inverted variant (SVG)
- PNG fallbacks in `frontend/public/`:
  - `pichub-mono-256.png`, `pichub-mono-64.png`
  - `pichub-inverted-256.png`, `pichub-inverted-64.png`
  - `favicon-512.png`, `favicon-192.png`, `favicon-32.png`, `favicon-16.png`, `favicon.ico`

Tokens (CSS variables)

- Colors:
  - `--pichub-orange`: #ef7a1a (use for accents and UI elements)
  - `--pichub-orange-accessible`: #a95612 (use for readable text on light backgrounds)
  - `--pichub-dark`: #1f1a14
  - `--pichub-bg`: #fdecd6
  - `--pichub-muted`: #6b5e53
  - `--pichub-white`: #ffffff
- Spacing & radius:
  - `--space-sm`, `--space-md`, `--space-lg` (see `styles.css` for values)
  - `--radius-sm`, `--radius-md`

Usage rules

- Header / Brand:
  - Use the primary `pichub-logo.svg` in the header at a height of 36–48px. Keep left padding 12px and right padding 8px.
  - On dark backgrounds, use `pichub-logo-inverted.svg`.
  - For monochrome contexts (favicons, high-contrast), use `pichub-logo-mono.svg`.
- Buttons:
  - Primary buttons: background `--pichub-orange`, text `--pichub-white`.
  - If a button appears on `--pichub-bg`, prefer `--pichub-orange-accessible` for the button background to maintain contrast.
- Text:
  - Body text should use `--pichub-dark` on `--pichub-bg` for readability.
  - Avoid using `--pichub-orange` for large blocks of body text — use accessible variant instead.
- Favicon & App Icons:
  - Use `favicon-32.png` and `favicon-16.png` for general web usage.
  - Provide `apple-touch-icon` (180px) and `manifest` icons (192/512) for mobile PWA scenarios.

Developer notes

- Regenerate PNG/ICO favicons from the SVG when updating the logo. Scripts are in `.tmp/`:
  - `.tmp/generate_additional_icons.py` — creates 180/192/512 PNGs and an apple-touch icon
  - `.tmp/rasterize_with_pillow.py` — approximates SVG → PNG for mono/inverted variants
- If you require pixel-perfect rasterization, use Inkscape or `cairosvg` with native Cairo installed.

Accessibility

- Contrast checks were run and recorded in `specs/002-visual-style/qa.md`.
- Use `--pichub-orange-accessible` for any text-on-background instances that need at least 4.5:1 contrast.

Examples

- Header markup (already in `frontend/src/index.html`):

```html
<a class="brand" href="/">
  <img src="./assets/pichub-logo.svg" alt="PicHub" class="brand-logo" />
  <span class="brand-title">PicHub</span>
</a>
```

- Example CSS usage:

```css
.btn-primary {
  background: var(--pichub-orange);
  color: var(--pichub-white);
  border-radius: var(--radius-sm);
}
```

Maintenance

- When updating tokens, add a short entry in `specs/002-visual-style/quickstart.md` describing the change and reason.
- Run `npm run build` in `frontend/` and verify the deployed site renders the updated tokens as expected.
