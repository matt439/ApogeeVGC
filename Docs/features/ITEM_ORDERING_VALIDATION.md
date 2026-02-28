# Alphabetical Ordering Validation Report

## Executive Summary
This report validates the alphabetical ordering of items within each of your C# item files. Each file is analyzed to ensure items are in strict alphabetical order as they should be grouped.

---

## ? ItemsABC.cs - VALIDATION PASSED

### Ordering Check
**Status**: ? **CORRECT** - All items are in proper alphabetical order

### Item Sequence:
#### A Items (11 items)
1. ? Ability Shield
2. ? Absorb Bulb
3. ? Adamant Crystal
4. ? Adamant Orb
5. ? Adrenaline Orb
6. ? Aguav Berry
7. ? Air Balloon
8. ? Apicot Berry
9. ? Aspear Berry
10. ? Assault Vest
11. ? Auspicious Armor

#### B Items (14 items)
12. ? Babiri Berry
13. ? Beast Ball
14. ? Berry Sweet
15. ? Big Nugget
16. ? Big Root
17. ? Binding Band
18. ? Black Belt
19. ? Black Glasses
20. ? Black Sludge
21. ? Blue Orb
22. ? Blunder Policy (MISSING - should be added)
23. ? Booster Energy (MISSING - should be added)
24. ? Bottle Cap
25. ? Bright Powder

**NOTE**: Blunder Policy and Booster Energy are not present in your ItemsABC.cs file. These are Gen 9 items that should be added.

#### C Items (20 items)
26. ? Cell Battery
27. ? Charcoal
28. ? Charti Berry
29. ? Cheri Berry
30. ? Cherish Ball (MISSING - should be added after Cheri Berry)
31. ? Chesto Berry
32. ? Chilan Berry
33. ? Chipped Pot (MISSING - should be added)
34. ? Choice Band
35. ? Choice Scarf
36. ? Choice Specs
37. ? Chople Berry
38. ? Clear Amulet
39. ? Clover Sweet (MISSING - should be added)
40. ? Coba Berry
41. ? Colbur Berry
42. ? Cornerstone Mask
43. ? Covert Cloak
44. ? Cracked Pot (MISSING - should be added)
45. ? Custap Berry

**Ordering Result**: Correct sequence, but missing several items

---

## ? ItemsDEF.cs - VALIDATION PASSED WITH MINOR ISSUES

### Ordering Check
**Status**: ? **CORRECT** - All items are in proper alphabetical order

### Item Sequence:
#### D Items (15 items)
1. ? Damp Rock
2. ? Dawn Stone
3. ? Deep Sea Scale
4. ? Deep Sea Tooth
5. ? Destiny Knot
6. ? Dive Ball
7. ? Draco Plate
8. ? Dragon Fang
9. ? Dragon Scale
10. ? Dread Plate
11. ? Dream Ball
12. ? Dubious Disc
13. ? Dusk Ball
14. ? Dusk Stone

#### E Items (8 items)
15. ? Earth Plate
16. ? Eject Button
17. ? Eject Pack
18. ? Electirizer
19. ? Electric Seed
20. ? Eviolite

**NOTE**: Missing "Enigma Berry" (should be between Electric Seed and Eviolite)

21. ? Expert Belt

#### F Items (11 items)
22. ? Fairy Feather
23. ? Fast Ball
24. ? Figy Berry
25. ? Fire Stone
26. ? Fist Plate
27. ? Flame Orb
28. ? Flame Plate
29. ? Float Stone
30. ? Flower Sweet
31. ? Focus Band
32. ? Focus Sash
33. ? Friend Ball

**Ordering Result**: Correct sequence, missing Enigma Berry

---

## ? ItemsGHI.cs - VALIDATION PASSED

### Ordering Check
**Status**: ? **CORRECT** - All items are in proper alphabetical order

### Item Sequence:
#### G Items (11 items)
1. ? Galarica Cuff
2. ? Galarica Wreath
3. ? Ganlon Berry
4. ? Gold Bottle Cap
5. ? Grassy Seed
6. ? Great Ball
7. ? Grepa Berry
8. ? Grip Claw
9. ? Griseous Core
10. ? Griseous Orb

#### H Items (8 items)
11. ? Haban Berry
12. ? Hard Stone
13. ? Heal Ball
14. ? Hearthflame Mask
15. ? Heat Rock
16. ? Heavy Ball
17. ? Heavy-Duty Boots
18. ? Hondew Berry

#### I Items (7 items)
19. ? Iapapa Berry
20. ? Ice Stone
21. ? Icicle Plate
22. ? Icy Rock
23. ? Insect Plate
24. ? Iron Ball
25. ? Iron Plate

**Ordering Result**: ? Perfect - All items in correct order

---

## ? ItemsJKL.cs - VALIDATION PASSED

### Ordering Check
**Status**: ? **CORRECT** - All items are in proper alphabetical order

### Item Sequence:
#### J Items (1 item)
1. ? Jaboca Berry

#### K Items (6 items)
2. ? Kasib Berry
3. ? Kebia Berry
4. ? Kee Berry
5. ? Kelpsy Berry
6. ? King's Rock

#### L Items (26 items)
7. ? Lagging Tail
8. ? Lansat Berry
9. ? Lax Incense
10. ? Leaf Stone
11. ? Leek
12. ? Leftovers
13. ? Leppa Berry
14. ? Level Ball
15. ? Liechi Berry
16. ? Life Orb
17. ? Light Ball
18. ? Light Clay
19. ? Loaded Dice
20. ? Love Ball
21. ? Love Sweet
22. ? Lucky Punch
23. ? Lum Berry
24. ? Luminous Moss
25. ? Lure Ball
26. ? Lustrous Globe
27. ? Lustrous Orb
28. ? Luxury Ball

**Ordering Result**: ? Perfect - All items in correct order

---

## ? ItemsMNO.cs - VALIDATION FAILED

### Ordering Check
**Status**: ? **INCORRECT** - Missing several M items at the beginning

### Critical Issues Found:
1. ? **File starts with "Magnet"** - Missing multiple M items that should come before:
   - Macho Brace
   - Magmarizer

2. ? **Missing M Items** (should be added before Magnet):
   - Macho Brace
   - Magmarizer
   - Malicious Armor (after Mago Berry)
   - Master Ball (after Maranga Berry)
   - Masterpiece Teacup
   - Meadow Plate
   - Mental Herb
   - Metal Alloy
   - Metal Coat
   - Metal Powder
   - Metronome
   - Micle Berry
   - Mind Plate
   - Miracle Seed
   - Mirror Herb
   - Misty Seed
   - Moon Ball
   - Moon Stone
   - Muscle Band
   - Mystic Water

### Current Sequence (Partial):
1. ? **WRONG START** - Should start with Macho Brace
2. ? Magnet (but many items missing before this)
3. ? Mago Berry
4. ? Maranga Berry

**N Items Expected**: Nest Ball, Net Ball, Never-Melt Ice, Normal Gem

**O Items Expected**: Occa Berry, Odd Incense, Oran Berry, Oval Stone

#### ItemsPQR.cs
**P Items Expected**: Passho Berry, Payapa Berry, Pecha Berry, Persim Berry, Petaya Berry, Pixie Plate, Poke Ball, Pomeg Berry, Power Anklet, Power Band, Power Belt, Power Bracer, Power Herb, Power Lens, Power Weight, Premier Ball, Pretty Feather, Prism Scale, Protective Pads, Protector, Psychic Seed, Punching Glove

**Q Items Expected**: Qualot Berry, Quick Ball, Quick Claw, Quick Powder

**R Items Expected**: Rare Bone, Rawst Berry, Razor Claw, Razor Fang, Reaper Cloth, Red Card, Red Orb, Repeat Ball, Ribbon Sweet, Rindo Berry, Ring Target, Rocky Helmet, Room Service, Roseli Berry, Rowap Berry, Rusted Shield, Rusted Sword

#### ItemsSTU.cs
**S Items Expected**: Sachet, Safari Ball, Safety Goggles, Salac Berry, Sharp Beak, Shed Shell, Shell Bell, Shiny Stone, Shuca Berry, Silk Scarf, Silver Powder, Sitrus Berry, Sky Plate, Smooth Rock, Snowball, Soft Sand, Soul Dew, Spell Tag, Splash Plate, Spooky Plate, Sport Ball, Starf Berry, Star Sweet, Stone Plate, Strawberry Sweet, Sun Stone, Sweet Apple, Syrupy Apple

**T Items Expected**: Tamato Berry, Tanga Berry, Tart Apple, Terrain Extender, Thick Club, Throat Spray, Thunder Stone, Timer Ball, Toxic Orb, Toxic Plate, Twisted Spoon

**U Items Expected**: Ultra Ball, Unremarkable Teacup, Up-Grade, Utility Umbrella

#### ItemsVWX.cs
**V Items Expected**: (None in Gen 9)

**W Items Expected**: Wacan Berry, Water Stone, Wave Incense, Weakness Policy, Wellspring Mask, Whipped Dream, White Herb, Wide Lens, Wiki Berry, Wise Glasses

**X Items Expected**: (None in Gen 9)

#### ItemsYZ.cs
**Y Items Expected**: Yache Berry

**Z Items Expected**: Zap Plate, Zoom Lens

---

## ?? Common Ordering Issues to Watch For

### 1. **Hyphenated Items**
Items with hyphens should be alphabetized by their full name, ignoring the hyphen:
- ? Heavy-Duty Boots comes after Heavy Ball
- ? Never-Melt Ice is alphabetized as "NeverMeltIce"
- ? Up-Grade is alphabetized as "UpGrade"

### 2. **Apostrophes**
Items with apostrophes should be alphabetized as if the apostrophe doesn't exist:
- ? King's Rock is alphabetized as "KingsRock"

### 3. **Space Handling**
Items with spaces should be alphabetized as a single word:
- ? "Absorb Bulb" is alphabetized as "AbsorbBulb"
- ? "Deep Sea Scale" comes before "Deep Sea Tooth"

### 4. **Number Prefixes**
Items with numbers should follow standard ASCII ordering:
- Numbers come before letters in ASCII

---

## ? Validation Results Summary

| File | Status | Item Count | Issues Found |
|------|--------|------------|--------------|
| ItemsABC.cs | ?? MOSTLY PASS | 45 | Missing: Blunder Policy, Booster Energy, Cherish Ball, Chipped Pot, Clover Sweet, Cracked Pot |
| ItemsDEF.cs | ?? MOSTLY PASS | 33 | Missing: Enigma Berry |
| ItemsGHI.cs | ? PERFECT | 26 | None - Perfect! |
| ItemsJKL.cs | ? PERFECT | 33 | None - Perfect! |
| ItemsMNO.cs | ? FAIL | ~13 | **CRITICAL**: Missing ~20 M items at start (Macho Brace through Mystic Water) |
| ItemsPQR.cs | ? PENDING | ~41 | Needs verification |
| ItemsSTU.cs | ? PENDING | ~42 | Needs verification |
| ItemsVWX.cs | ? PENDING | ~11 | Needs verification |
| ItemsYZ.cs | ? PENDING | ~3 | Needs verification |

---

## ?? CRITICAL ISSUES FOUND

### ItemsMNO.cs - Major Ordering Problem
**Severity**: ?? **HIGH** - File is missing approximately **20 M items** that should come alphabetically before "Magnet"

**Issue**: The file currently starts with:
```csharp
[ItemId.Magnet] = new() { ... }
[ItemId.MagoBerry] = new() { ... }
[ItemId.MarangaBerry] = new() { ... }
```

**Required Fix**: Add these items **BEFORE** Magnet:
1. Macho Brace
2. Magmarizer
3. (Then Magnet - already present)
4. (Then Mago Berry - already present)
5. Malicious Armor *(after Mago Berry)*
6. (Then Maranga Berry - already present)
7. Master Ball
8. Masterpiece Teacup
9. Meadow Plate
10. Mental Herb
11. Metal Alloy
12. Metal Coat
13. Metal Powder
14. Metronome
15. Micle Berry
16. Mind Plate
17. Miracle Seed
18. Mirror Herb
19. Misty Seed
20. Moon Ball
21. Moon Stone
22. Muscle Band
23. Mystic Water

---

## ?? Missing Items Detected

### ItemsABC.cs Missing Items:
1. ? **Blunder Policy** - Should be after Blue Orb, before Booster Energy
2. ? **Booster Energy** - Should be after Blunder Policy, before Bottle Cap
3. ? **Cherish Ball** - Should be after Cheri Berry, before Chesto Berry
4. ? **Chipped Pot** - Should be after Chilan Berry, before Choice Band
5. ? **Clover Sweet** - Should be after Clear Amulet, before Coba Berry
6. ? **Cracked Pot** - Should be after Covert Cloak, before Custap Berry

### ItemsDEF.cs Missing Items:
1. ? **Enigma Berry** - Should be after Electric Seed, before Eviolite

---

## ?? Recommendations

### High Priority
1. ? **Add missing Gen 9 items** to ItemsABC.cs:
   - Blunder Policy (Gen 8, but used in Gen 9)
   - Booster Energy (Gen 9 new item)
   - Cherish Ball (rare but valid)
   - Chipped Pot (Gen 8 evolution item)
   - Clover Sweet (Gen 8 Alcremie)
   - Cracked Pot (Gen 8 evolution item)

2. ? **Add missing berry** to ItemsDEF.cs:
   - Enigma Berry (Gen 3, still in Gen 9)

### Medium Priority
3. ? **Verify remaining files** (MNO through YZ) for:
   - Alphabetical ordering within each section
   - Missing items from the expected lists
   - Incorrect file placement (e.g., an M item in the PQR file)

### Low Priority
4. ? **Consider adding TODO comments** for complex implementations that are incomplete
5. ? **Document any deliberately skipped items** with comments explaining why

---

## ?? Next Steps

To complete the validation:

1. **Run extraction tool** to programmatically list all items in MNO, PQR, STU, VWX, YZ files
2. **Compare against expected lists** to find missing items
3. **Verify alphabetical order** programmatically
4. **Generate final report** with all discrepancies
5. **Create fix script** to add missing items and reorder if needed

Would you like me to:
- A) Check the remaining files (MNO through YZ) manually?
- B) Create a tool to automatically extract and validate all items?
- C) Generate code to add the missing items to ABC and DEF files?
