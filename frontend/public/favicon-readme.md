Favicon generation instructions

This project includes SVG favicon fallbacks in `frontend/public/`:

- `favicon.svg` (primary)
- `favicon-32.svg`
- `favicon-16.svg`

To generate PNG and ICO files locally (requires ImageMagick or svgo + pngcrush):

ImageMagick example (Windows PowerShell):

```powershell
# From repository root
magick convert frontend/public/favicon.svg -background none -resize 32x32 frontend/public/favicon-32.png
magick convert frontend/public/favicon.svg -background none -resize 16x16 frontend/public/favicon-16.png

# Create multi-size ICO
magick convert frontend/public/favicon-16.png frontend/public/favicon-32.png frontend/public/favicon-32.png frontend/public/favicon.ico
```

Alternative (using rsvg-convert + pngquant):

```powershell
rsvg-convert -w 32 -h 32 frontend/public/favicon.svg -o frontend/public/favicon-32.png
rsvg-convert -w 16 -h 16 frontend/public/favicon.svg -o frontend/public/favicon-16.png
```

Add generated files to git and update `frontend/src/index.html` if paths differ.
