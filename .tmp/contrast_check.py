from __future__ import annotations
import os
import math


def hex_to_rgb(hexstr: str):
    hexstr = hexstr.strip().lstrip('#')
    if len(hexstr) == 3:
        hexstr = ''.join([c*2 for c in hexstr])
    r = int(hexstr[0:2], 16)
    g = int(hexstr[2:4], 16)
    b = int(hexstr[4:6], 16)
    return (r/255.0, g/255.0, b/255.0)


def linearize_channel(c: float) -> float:
    if c <= 0.03928:
        return c / 12.92
    return ((c + 0.055) / 1.055) ** 2.4


def relative_luminance(rgb):
    r, g, b = rgb
    return 0.2126 * linearize_channel(r) + 0.7152 * linearize_channel(g) + 0.0722 * linearize_channel(b)


def contrast_ratio(hex1: str, hex2: str) -> float:
    l1 = relative_luminance(hex_to_rgb(hex1))
    l2 = relative_luminance(hex_to_rgb(hex2))
    lighter = max(l1, l2)
    darker = min(l1, l2)
    return (lighter + 0.05) / (darker + 0.05)


pairs = [
    ('--pichub-dark', '#1f1a14', '--pichub-bg', '#fdecd6'),
    ('--pichub-orange', '#ef7a1a', '--pichub-bg', '#fdecd6'),
    ('--pichub-dark', '#1f1a14', '--pichub-white', '#ffffff'),
    ('--pichub-muted', '#6b5e53', '--pichub-bg', '#fdecd6'),
]

results = []
for name1, hex1, name2, hex2 in pairs:
    ratio = contrast_ratio(hex1, hex2)
    results.append((name1, hex1, name2, hex2, round(ratio, 2)))

out = ['# Visual Style QA - Contrast Check\n']
out.append(
    'Pairs and contrast ratios (WCAG guidance: 4.5:1 for normal text, 3:1 for large text)\n')
out.append('')
for a, b, c, d, ratio in results:
    out.append(f'- {a} ({b}) vs {c} ({d}): {ratio}:1')

repo_root = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
qa_path = os.path.join(repo_root, 'specs', '002-visual-style', 'qa.md')
os.makedirs(os.path.dirname(qa_path), exist_ok=True)
with open(qa_path, 'w', encoding='utf-8') as f:
    f.write('\n'.join(out))

print('\n'.join(out))
