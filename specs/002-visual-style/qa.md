# Visual Style QA - Contrast Check

Pairs and contrast ratios (WCAG guidance: 4.5:1 for normal text, 3:1 for large text)

- --pichub-dark (#1f1a14) vs --pichub-bg (#fdecd6): 14.92:1
- --pichub-orange (#ef7a1a) vs --pichub-bg (#fdecd6): 2.43:1
- --pichub-dark (#1f1a14) vs --pichub-white (#ffffff): 17.27:1
- --pichub-muted (#6b5e53) vs --pichub-bg (#fdecd6): 5.41:1

Recommendation:

- The primary orange `--pichub-orange` (#ef7a1a) has insufficient contrast on `--pichub-bg` for body text (2.43:1). Use it only for accents or with large/bold text. For accessible text usage, prefer `--pichub-orange-accessible` (#a95612) which meets 4.5:1 against `--pichub-bg`.

CSS token to add:

```
--pichub-orange-accessible: #a95612; /* contrast 4.51:1 vs --pichub-bg */
```

Next steps:

- Replace occurrences of `color: var(--pichub-orange)` on light backgrounds with `var(--pichub-dark)` or `var(--pichub-orange-accessible)` where appropriate.
- Reserve `--pichub-orange` for filled buttons (white text on orange) and small accents.
## Build artifacts (fallback for Lighthouse)\n\n\nNote: Chrome/Chromium not found on PATH so Lighthouse couldn't run in this environment. To run Lighthouse, install Chrome/Chromium and run 
px lighthouse http://localhost:4173 --output=json --output-path=./lighthouse-report.json.
