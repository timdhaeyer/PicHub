import os
import sys

repo_root = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
assets_dir = os.path.join(repo_root, 'frontend', 'src', 'assets')
out_dir = os.path.join(repo_root, 'frontend', 'public')
os.makedirs(out_dir, exist_ok=True)

variants = [
    ('pichub-logo-mono.svg', 'pichub-mono-256.png', 256),
    ('pichub-logo-mono.svg', 'pichub-mono-64.png', 64),
    ('pichub-logo-inverted.svg', 'pichub-inverted-256.png', 256),
    ('pichub-logo-inverted.svg', 'pichub-inverted-64.png', 64),
]

try:
    import cairosvg
except Exception as e:
    print('cairosvg not available:', e)
    print('Install with: pip install cairosvg')
    sys.exit(2)

for svg, outname, size in variants:
    svg_path = os.path.join(assets_dir, svg)
    out_path = os.path.join(out_dir, outname)
    if not os.path.exists(svg_path):
        print('Missing', svg_path)
        continue
    cairosvg.svg2png(url=svg_path, write_to=out_path,
                     output_width=size, output_height=size)
    print('Wrote', out_path)
