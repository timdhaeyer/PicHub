from PIL import Image, ImageDraw, ImageOps
import os
from xml.etree import ElementTree as ET

repo_root = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
assets_dir = os.path.join(repo_root, 'frontend', 'src', 'assets')
out_dir = os.path.join(repo_root, 'frontend', 'public')
os.makedirs(out_dir, exist_ok=True)

# Simple renderer: parse svg fills and paint shapes to approximate the SVG for these simple logos


def render_svg_like(svg_path, size):
    tree = ET.parse(svg_path)
    root = tree.getroot()
    img = Image.new('RGBA', (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Background rounded rect (if present)
    for rect in root.findall('.//{http://www.w3.org/2000/svg}rect'):
        x = int(float(rect.get('x', '0')))
        y = int(float(rect.get('y', '0')))
        w = int(float(rect.get('width', '512')))
        h = int(float(rect.get('height', '512')))
        fill = rect.get('fill') or rect.attrib.get(
            '{http://www.w3.org/1999/xlink}href')
        if fill in (None, 'none'):
            continue
        # map fill hex to rgba
        color = tuple(int(fill.lstrip('#')[i:i+2], 16)
                      for i in (0, 2, 4)) + (255,)
        # scale
        scale = size / 512.0
        draw.rounded_rectangle([(int(x*scale), int(y*scale)), (int((x+w)*scale),
                               int((y+h)*scale))], radius=int(24*scale), fill=color)
    # group transforms exist; we'll draw nested shapes by tag
    for g in root.findall('.//{http://www.w3.org/2000/svg}g'):
        for child in g:
            tag = child.tag
            if tag.endswith('rect'):
                x = int(float(child.get('x', '0')))
                y = int(float(child.get('y', '0')))
                w = int(float(child.get('width', '0')))
                h = int(float(child.get('height', '0')))
                rx = int(float(child.get('rx', '0')))
                fill = child.get('fill') or child.attrib.get('class')
                if fill and fill.startswith('#'):
                    color = tuple(
                        int(fill.lstrip('#')[i:i+2], 16) for i in (0, 2, 4)) + (255,)
                else:
                    # fallback map for classes used in pichub-logo.svg
                    cls = child.get('class')
                    if cls == 'cam' or child.get('fill') is None:
                        color = (0xef, 0x7a, 0x1a, 255)
                    elif cls == 'lens':
                        color = (0x1f, 0x1a, 0x14, 255)
                    elif cls == 'accent':
                        color = (0xfd, 0xec, 0xd6, 255)
                    else:
                        color = (0, 0, 0, 0)
                scale = size / 512.0
                draw.rounded_rectangle([(int(x*scale), int(y*scale)), (int(
                    (x+w)*scale), int((y+h)*scale))], radius=int(rx*scale), fill=color)
            if tag.endswith('circle'):
                cx = int(float(child.get('cx', '0')))
                cy = int(float(child.get('cy', '0')))
                r = int(float(child.get('r', '0')))
                fill = child.get('fill')
                if fill and fill.startswith('#'):
                    color = tuple(
                        int(fill.lstrip('#')[i:i+2], 16) for i in (0, 2, 4)) + (255,)
                else:
                    cls = child.get('class')
                    if cls == 'cam':
                        color = (0xef, 0x7a, 0x1a, 255)
                    elif cls == 'lens':
                        color = (0x1f, 0x1a, 0x14, 255)
                    elif cls == 'accent':
                        color = (0xfd, 0xec, 0xd6, 255)
                    else:
                        color = (0, 0, 0, 0)
                scale = size / 512.0
                draw.ellipse([(int((cx-r)*scale), int((cy-r)*scale)),
                             (int((cx+r)*scale), int((cy+r)*scale))], fill=color)
    return img


variants = [
    ('pichub-logo-mono.svg', 'pichub-mono-256.png', 256),
    ('pichub-logo-mono.svg', 'pichub-mono-64.png', 64),
    ('pichub-logo-inverted.svg', 'pichub-inverted-256.png', 256),
    ('pichub-logo-inverted.svg', 'pichub-inverted-64.png', 64),
]

for svg, outname, size in variants:
    svg_path = os.path.join(assets_dir, svg)
    out_path = os.path.join(out_dir, outname)
    if not os.path.exists(svg_path):
        print('Missing', svg_path)
        continue
    img = render_svg_like(svg_path, size)
    img.save(out_path, format='PNG')
    print('Wrote', out_path)
