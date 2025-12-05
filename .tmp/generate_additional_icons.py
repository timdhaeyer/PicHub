from PIL import Image, ImageDraw
import os

repo_root = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
out_dir = os.path.join(repo_root, 'frontend', 'public')
os.makedirs(out_dir, exist_ok=True)

# Colors
bg = (0xef, 0x7a, 0x1a, 255)
dark = (0x1f, 0x1a, 0x14, 255)
accent = (0xfd, 0xec, 0xd6, 255)

# Draw simplified logo like before


def draw_logo(size):
    img = Image.new('RGBA', (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    radius = int(size * 0.12)
    draw.rounded_rectangle([(0, 0), (size-1, size-1)], radius=radius, fill=bg)
    scale = size / 512.0
    tx = int(56 * scale)
    ty = int(72 * scale)
    body_x0 = tx
    body_y0 = ty + int(64 * scale)
    body_w = int(400 * scale)
    body_h = int(272 * scale)
    draw.rounded_rectangle(
        [(body_x0, body_y0), (body_x0+body_w, body_y0+body_h)], radius=int(40*scale), fill=dark)
    cx = tx + int(200 * scale)
    cy = ty + int(200 * scale)
    r_outer = int(72 * scale)
    r_mid = int(42 * scale)
    r_inner = int(22 * scale)
    draw.ellipse([(cx-r_outer, cy-r_outer),
                 (cx+r_outer, cy+r_outer)], fill=dark)
    draw.ellipse([(cx-r_mid, cy-r_mid), (cx+r_mid, cy+r_mid)], fill=accent)
    draw.ellipse([(cx-r_inner, cy-r_inner),
                 (cx+r_inner, cy+r_inner)], fill=dark)
    return img


sizes = [180, 192, 512]
for s in sizes:
    img = draw_logo(s)
    path = os.path.join(
        out_dir, f'apple-touch-icon-{s}.png' if s == 180 else f'favicon-{s}.png')
    img.save(path, format='PNG')
    print('Wrote', path)
