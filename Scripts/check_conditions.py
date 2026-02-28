#!/usr/bin/env python3
"""
Check that each ConditionId enum value has a corresponding entry in Conditions data files.
"""

import re
import glob

# Parse ConditionId enum from file
with open(r"c:\VSProjects\ApogeeVGC\ApogeeVGC\Sim\Conditions\ConditionId.cs", 'r') as f:
    content = f.read()
    # Find all enum entries (simple approach: lines with just a word and comma/no comma)
    enum_pattern = r'^\s+(\w+),?\s*$'
    condition_ids = []
    in_enum = False
    for line in content.split('\n'):
        if 'public enum ConditionId' in line:
            in_enum = True
            continue
        if in_enum and '}' in line:
            break
        if in_enum:
            match = re.match(enum_pattern, line)
            if match:
                condition_ids.append(match.group(1))

# Parse condition data entries from all Conditions*.cs files
found_conditions = set()
condition_files = glob.glob(r"c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Conditions\Conditions*.cs")
for filepath in condition_files:
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
        # Find all [ConditionId.XXX] = new() patterns
        pattern = r'\[ConditionId\.(\w+)\]\s*='
        matches = re.findall(pattern, content)
        found_conditions.update(matches)

# Find missing conditions
missing = []
for cond_id in condition_ids:
    if cond_id not in found_conditions:
        missing.append(cond_id)

# Results
print("="*60)
print("CONDITION COVERAGE REPORT")
print("="*60)
print(f"\nTotal ConditionId enum entries: {len(condition_ids)}")
print(f"Total implemented conditions: {len(found_conditions)}")
print(f"Missing implementations: {len(missing)}")

if missing:
    print("\n" + "="*60)
    print("MISSING CONDITIONS:")
    print("="*60)
    for i, cond in enumerate(sorted(missing), 1):
        print(f"{i:3}. {cond}")
else:
    print("\n✓ All conditions have data entries!")

# Extra conditions (in data but not in enum)
extra = found_conditions - set(condition_ids)
if extra:
    print("\n" + "="*60)
    print("EXTRA CONDITIONS (in data but not in enum):")
    print("="*60)
    for i, cond in enumerate(sorted(extra), 1):
        print(f"{i:3}. {cond}")

print("\n" + "="*60)
