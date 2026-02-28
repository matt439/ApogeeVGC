#!/usr/bin/env python3
"""
Extract condition definitions from moves.ts
"""

import re

moves_file = r"c:\VSProjects\ApogeeVGC\pokemon-showdown\data\moves.ts"

# Missing conditions we're looking for
missing_conditions = {
    'yawn', 'substitute', 'wish', 'nightmare', 'taunt', 'disable', 'rest', 'roost',
    'fly', 'dig', 'dive', 'bounce', 'shadowforce', 'phantomforce', 'skydrop', 'torment',
    'defensecurl', 'embargo', 'healblock', 'focusenergy', 'laserfocus', 'dragóncheer',
    'protosynthesis', 'grassyterrain', 'psychicterrain', 'spikes', 'stealthrock',
    'toxicspikes', 'stickyweb', 'safeguard', 'mist', 'lightscreen', 'reflect'
}

with open(moves_file, 'r', encoding='utf-8') as f:
    content = f.read()

# Find all moves with conditions
# Pattern: movename: { ... condition: { ... }, ... },
move_pattern = r'(\w+):\s*\{[^}]*?condition:\s*\{'

found_moves = []
for match in re.finditer(move_pattern, content):
    move_name = match.group(1).lower()
    found_moves.append(move_name)

# Check which missing conditions have move definitions
found_relevant = []
for move in found_moves:
    if move in missing_conditions:
        found_relevant.append(move)

print(f"Found {len(found_moves)} moves with condition definitions")
print(f"\nRelevant moves (from our missing list):")
for move in sorted(found_relevant):
    print(f"  - {move}")

print(f"\nTotal relevant: {len(found_relevant)}")
