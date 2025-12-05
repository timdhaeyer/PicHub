from __future__ import annotations


def hex_to_rgb(hexstr: str):
    hexstr = hexstr.strip().lstrip('#')
    if len(hexstr) == 3:
        hexstr = ''.join([c*2 for c in hexstr])
    r = int(hexstr[0:2], 16)
    g = int(hexstr[2:4], 16)
    b = int(hexstr[4:6], 16)
    return (r/255.0, g/255.0, b/255.0)


def rgb_to_hex(rgb):
    return '#{:02x}{:02x}{:02x}'.format(int(rgb[0]*255), int(rgb[1]*255), int(rgb[2]*255))


def linearize_channel(c: float) -> float:
    if c <= 0.03928:
        return c / 12.92
    return ((c + 0.055) / 1.055) ** 2.4


def relative_luminance(rgb):
    r, g, b = rgb
    return 0.2126 * linearize_channel(r) + 0.7152 * linearize_channel(g) + 0.0722 * linearize_channel(b)


def contrast_ratio_hex(hex1, hex2):
    a = relative_luminance(hex_to_rgb(hex1))
    b = relative_luminance(hex_to_rgb(hex2))
    L1 = max(a, b)
    L2 = min(a, b)
    return (L1 + 0.05) / (L2 + 0.05)


orig = '#ef7a1a'
bg = '#fdecd6'
white = '#ffffff'

# Find darker variant by scaling RGB towards black
orig_rgb = list(hex_to_rgb(orig))

best = None
for i in range(1, 101):
    factor = 1.0 - (i/120.0)  # scale from ~0.991 downwards
    cand = (orig_rgb[0]*factor, orig_rgb[1]*factor, orig_rgb[2]*factor)
    hexcand = rgb_to_hex(cand)
    ratio = contrast_ratio_hex(hexcand, bg)
    if ratio >= 4.5:
        best = (hexcand, round(ratio, 2), i, factor)
        break

# If not found in this range, try darker by decreasing more
if not best:
    for i in range(120, 300):
        factor = max(0, 1.0 - (i/300.0))
        cand = (orig_rgb[0]*factor, orig_rgb[1]*factor, orig_rgb[2]*factor)
        hexcand = rgb_to_hex(cand)
        ratio = contrast_ratio_hex(hexcand, bg)
        if ratio >= 4.5:
            best = (hexcand, round(ratio, 2), i, factor)
            break

print('Original orange:', orig)
print('Background:', bg)
print('White:', white)
print('Contrast original vs bg:', round(contrast_ratio_hex(orig, bg), 2))
print('Contrast white on orig (text white on orange):',
      round(contrast_ratio_hex(white, orig), 2))
if best:
    print('\nSuggested accessible orange:', best[0])
    print('Contrast vs bg:', best[1])
else:
    print('\nNo accessible variant found via simple scaling')
