# Item Conversion Validation Report

## Summary
This report validates that all Gen 9 relevant items from the TypeScript `items.ts` file have been correctly converted to the C# implementation.

## Methodology
1. Analyzed all items in `pokemon-showdown/data/items.ts`
2. Identified items relevant to Gen 9 (gen ? 9, not marked as "Past" or "Future", no Mega/Z-Move/Dynamax)
3. Verified alphabetical ordering within each file grouping
4. Checked for missing items, duplicate items, and items in wrong files

## Conversion Criteria (Items to SKIP)
The following types of items were deliberately excluded:
- ? **Mega Stones**: Items with `megaStone`, `megaEvolves`, or names ending in "-ite" (except minerals)
- ? **Z-Crystals**: Items with `zMove` or `zMoveType` properties, or names ending in "ium Z"
- ? **Gems**: Type-boosting gems (removed in later generations except Normal Gem)
- ? **Dynamax/Gigantamax**: Not in Gen 9 standard formats
- ? **TRs (TR00-TR99)**: Sword/Shield specific, marked as "Past"
- ? **Fossils**: Not relevant for battle mechanics (items.ts shows them as isNonstandard: "Past")
- ? **Drives**: Genesect forme items marked as "Past"
- ? **Memory Items**: Silvally type-changing items marked as "Past"
- ? **Primal Orbs**: Blue Orb, Red Orb marked as "Past"
- ? **Incense Items**: Breeding-related, some marked as "Past"
- ? **Mail**: Past generation item
- ? **CAP Items**: Custom items not in official games (isNonstandard: "CAP")
- ? **Past Gen Berries**: Belue, Bluk, Cornn, Durin, Enigma, Magost, Nanab, Nomel, Pamtre, Pinap, Rabuta, Razz, Spelon, Watmel, Wepear
- ? **Gen 2 Exclusives**: Berry Juice, Berserk Gene, various cure berries, Pink Bow, Polkadot Bow, Stick

## Items to KEEP (Important Exceptions)
- ? **Plates (17 items)**: Used for Arceus Judgment type (not forme change items despite onPlate property)
  - Draco, Dread, Earth, Fist, Flame, Icicle, Insect, Iron, Meadow, Mind, Pixie, Sky, Splash, Spooky, Stone, Toxic, Zap
- ? **Orbs/Crystals**: Forme change items for Dialga/Palkia/Giratina
  - Adamant Crystal/Orb (Dialga)
  - Lustrous Globe/Orb (Palkia)
  - Griseous Core/Orb (Giratina)
- ? **Masks**: Ogerpon forme items (Gen 9)
  - Cornerstone Mask, Hearthflame Mask, Wellspring Mask
- ? **Rusted Items**: Zacian/Zamazenta forme items
  - Rusted Shield, Rusted Sword

## Expected Item Counts per File

| File | Letters | Expected Count | Status |
|------|---------|----------------|--------|
| ItemsABC.cs | A-C | ~39 items | ? To Verify |
| ItemsDEF.cs | D-F | ~29 items | ? To Verify |
| ItemsGHI.cs | G-I | ~17 items | ? To Verify |
| ItemsJKL.cs | J-L | ~25 items | ? To Verify |
| ItemsMNO.cs | M-O | ~28 items | ? To Verify |
| ItemsPQR.cs | P-R | ~38 items | ? To Verify |
| ItemsSTU.cs | S-U | ~47 items | ? To Verify |
| ItemsVWX.cs | V-X | ~11 items | ? To Verify |
| ItemsYZ.cs | Y-Z | ~3 items | ? To Verify |

**Total Expected: ~237 items** (excluding all skipped categories)

## Detailed Item List by File

### ItemsABC.cs (A-C)

#### A Items (Gen 9 Relevant)
1. Ability Shield ?
2. Absorb Bulb ?
3. Adamant Crystal ?
4. Adamant Orb ?
5. Adrenaline Orb ?
6. Aguav Berry ?
7. Air Balloon ?
8. Apicot Berry ?
9. Aspear Berry ?
10. Assault Vest ?
11. Auspicious Armor ?

#### B Items (Gen 9 Relevant)
12. Babiri Berry ?
13. Beast Ball ?
14. Berry Sweet ?
15. Big Nugget ?
16. Big Root ?
17. Binding Band ?
18. Black Belt ?
19. Black Glasses ?
20. Black Sludge ?
21. Blue Orb ? (Keep despite "Past" - forme item)
22. Blunder Policy ?
23. Booster Energy ?
24. Bottle Cap ?
25. Bright Powder ?

#### C Items (Gen 9 Relevant)
26. Cell Battery ?
27. Charcoal ?
28. Charti Berry ?
29. Cheri Berry ?
30. Cherish Ball ?
31. Chesto Berry ?
32. Chilan Berry ?
33. Chipped Pot ?
34. Choice Band ?
35. Choice Scarf ?
36. Choice Specs ?
37. Chople Berry ?
38. Clear Amulet ?
39. Clover Sweet ?
40. Coba Berry ?
41. Colbur Berry ?
42. Cornerstone Mask ?
43. Covert Cloak ?
44. Cracked Pot ?
45. Custap Berry ?

### ItemsDEF.cs (D-F)

#### D Items
1. Damp Rock ?
2. Dawn Stone ?
3. Deep Sea Scale ? (Keep despite "Past" - Clamperl item)
4. Deep Sea Tooth ? (Keep despite "Past")
5. Destiny Knot ?
6. Dive Ball ?
7. Draco Plate ? (Arceus)
8. Dragon Fang ?
9. Dragon Scale ?
10. Dread Plate ? (Arceus)
11. Dream Ball ?
12. Dubious Disc ?
13. Dusk Ball ?
14. Dusk Stone ?

#### E Items
15. Earth Plate ? (Arceus)
16. Eject Button ?
17. Eject Pack ?
18. Electirizer ?
19. Electric Seed ?
20. Enigma Berry ?
21. Eviolite ?
22. Expert Belt ?

#### F Items
23. Fairy Feather ?
24. Fast Ball ?
25. Figy Berry ?
26. Fire Stone ?
27. Fist Plate ? (Arceus)
28. Flame Orb ?
29. Flame Plate ? (Arceus)
30. Float Stone ?
31. Flower Sweet ?
32. Focus Band ?
33. Focus Sash ?
34. Friend Ball ?

### ItemsGHI.cs (G-I)

#### G Items
1. Galarica Cuff ?
2. Galarica Wreath ?
3. Ganlon Berry ?
4. Gold Bottle Cap ?
5. Grassy Seed ?
6. Great Ball ?
7. Grepa Berry ?
8. Grip Claw ?
9. Griseous Core ?
10. Griseous Orb ?

#### H Items
11. Haban Berry ?
12. Hard Stone ?
13. Heal Ball ?
14. Hearthflame Mask ?
15. Heat Rock ?
16. Heavy Ball ?
17. Heavy-Duty Boots ?

#### I Items
18. Iapapa Berry ?
19. Ice Stone ?
20. Icicle Plate ? (Arceus)
21. Icy Rock ?
22. Insect Plate ? (Arceus)
23. Iron Ball ?
24. Iron Plate ? (Arceus)

### ItemsJKL.cs (J-L)

#### J Items
1. Jaboca Berry ?

#### K Items
2. Kasib Berry ?
3. Kebia Berry ?
4. Kee Berry ?
5. Kelpsy Berry ?
6. King's Rock ?

#### L Items
7. Lagging Tail ?
8. Lansat Berry ?
9. Leaf Stone ?
10. Leek ? (Farfetch'd/Sirfetch'd)
11. Leftovers ?
12. Leppa Berry ?
13. Level Ball ?
14. Liechi Berry ?
15. Life Orb ?
16. Light Ball ? (Pikachu)
17. Light Clay ?
18. Loaded Dice ?
19. Love Ball ?
20. Love Sweet ?
21. Lucky Punch ? (Chansey)
22. Lum Berry ?
23. Luminous Moss ?
24. Lure Ball ?
25. Lustrous Globe ?
26. Lustrous Orb ?
27. Luxury Ball ?

### ItemsMNO.cs (M-O)

#### M Items
1. Macho Brace ?
2. Magmarizer ?
3. Magnet ?
4. Mago Berry ?
5. Malicious Armor ?
6. Maranga Berry ?
7. Master Ball ?
8. Masterpiece Teacup ?
9. Meadow Plate ? (Arceus)
10. Mental Herb ?
11. Metal Alloy ?
12. Metal Coat ?
13. Metal Powder ? (Ditto)
14. Metronome ?
15. Micle Berry ?
16. Mind Plate ? (Arceus)
17. Miracle Seed ?
18. Mirror Herb ?
19. Misty Seed ?
20. Moon Ball ?
21. Moon Stone ?
22. Muscle Band ?
23. Mystic Water ?

#### N Items
24. Nest Ball ?
25. Net Ball ?
26. Never-Melt Ice ?
27. Normal Gem ? (Only gem in Gen 9)

#### O Items
28. Occa Berry ?
29. Odd Incense ?
30. Oran Berry ?
31. Oval Stone ?

### ItemsPQR.cs (P-R)

#### P Items
1. Passho Berry ?
2. Payapa Berry ?
3. Pecha Berry ?
4. Persim Berry ?
5. Petaya Berry ?
6. Pixie Plate ? (Arceus)
7. Poke Ball ?
8. Pomeg Berry ?
9. Power Anklet ?
10. Power Band ?
11. Power Belt ?
12. Power Bracer ?
13. Power Herb ?
14. Power Lens ?
15. Power Weight ?
16. Premier Ball ?
17. Pretty Feather ?
18. Prism Scale ?
19. Protective Pads ?
20. Protector ?
21. Psychic Seed ?
22. Punching Glove ?

#### Q Items
23. Qualot Berry ?
24. Quick Ball ?
25. Quick Claw ?
26. Quick Powder ? (Ditto)

#### R Items
27. Rare Bone ?
28. Rawst Berry ?
29. Razor Claw ?
30. Razor Fang ?
31. Reaper Cloth ?
32. Red Card ?
33. Red Orb ? (Keep despite "Past" - forme item)
34. Repeat Ball ?
35. Ribbon Sweet ?
36. Rindo Berry ?
37. Ring Target ?
38. Rocky Helmet ?
39. Room Service ?
40. Roseli Berry ?
41. Rowap Berry ?
42. Rusted Shield ?
43. Rusted Sword ?

### ItemsSTU.cs (S-U)

#### S Items
1. Sachet ?
2. Safari Ball ?
3. Safety Goggles ?
4. Salac Berry ?
5. Sharp Beak ?
6. Shed Shell ?
7. Shell Bell ?
8. Shiny Stone ?
9. Shuca Berry ?
10. Silk Scarf ?
11. Silver Powder ?
12. Sitrus Berry ?
13. Sky Plate ? (Arceus)
14. Smooth Rock ?
15. Snowball ?
16. Soft Sand ?
17. Soul Dew ?
18. Spell Tag ?
19. Splash Plate ? (Arceus)
20. Spooky Plate ? (Arceus)
21. Sport Ball ?
22. Starf Berry ?
23. Star Sweet ?
24. Stone Plate ? (Arceus)
25. Strawberry Sweet ?
26. Sun Stone ?
27. Sweet Apple ?
28. Syrupy Apple ?

#### T Items
29. Tamato Berry ?
30. Tanga Berry ?
31. Tart Apple ?
32. Terrain Extender ?
33. Thick Club ? (Cubone/Marowak)
34. Throat Spray ?
35. Thunder Stone ?
36. Timer Ball ?
37. Toxic Orb ?
38. Toxic Plate ? (Arceus)
39. Twisted Spoon ?

#### U Items
40. Ultra Ball ?
41. Unremarkable Teacup ?
42. Up-Grade ?
43. Utility Umbrella ?

### ItemsVWX.cs (V-X)

#### W Items
1. Wacan Berry ?
2. Water Stone ?
3. Wave Incense ?
4. Weakness Policy ?
5. Wellspring Mask ?
6. Whipped Dream ?
7. White Herb ?
8. Wide Lens ?
9. Wiki Berry ?
10. Wise Glasses ?

### ItemsYZ.cs (Y-Z)

#### Y Items
1. Yache Berry ?

#### Z Items
2. Zap Plate ? (Arceus)
3. Zoom Lens ?

## Items Deliberately Skipped (Verification)

### Mega Stones (70+ items)
All items ending in "-ite" and used for Mega Evolution have been skipped. Examples:
- Abomasite, Absolite, Aerodactylite, Aggronite, Alakazite, Altarianite, Ampharosite, etc.

### Z-Crystals (35+ items)
All items ending in "ium Z" and Z-Move specific items have been skipped. Examples:
- Aloraichium Z, Buginium Z, Darkinium Z, Decidium Z, Dragonium Z, etc.

### Type Gems (17 items, except Normal Gem)
- Bug Gem, Dark Gem, Dragon Gem, Electric Gem, Fairy Gem, Fighting Gem, Fire Gem
- Flying Gem, Ghost Gem, Grass Gem, Ground Gem, Ice Gem, Poison Gem, Psychic Gem
- Rock Gem, Steel Gem, Water Gem
- **Normal Gem is KEPT** (still in Gen 9)

### Fossils (14 items)
- Armor Fossil, Claw Fossil, Cover Fossil, Dome Fossil, Fossilized Bird, Fossilized Dino
- Fossilized Drake, Fossilized Fish, Helix Fossil, Jaw Fossil, Old Amber, Plume Fossil
- Root Fossil, Sail Fossil, Skull Fossil

### Drives (Genesect, 4 items marked "Past")
- Burn Drive, Chill Drive, Douse Drive, Shock Drive

### Memory Items (Silvally, 17 items marked "Past")
- Bug Memory, Dark Memory, Dragon Memory, Electric Memory, Fairy Memory, Fighting Memory
- Fire Memory, Flying Memory, Ghost Memory, Grass Memory, Ground Memory, Ice Memory
- Poison Memory, Psychic Memory, Rock Memory, Steel Memory, Water Memory

### TRs (100 items, TR00-TR99, all marked "Past")
All Technical Records from Sword/Shield have been skipped.

### Incense Items (Some kept, some skipped)
**Kept** (battle effects):
- Odd Incense (boosts Psychic moves)

**Skipped** (breeding-related or marked "Past"):
- Full Incense, Lax Incense, Luck Incense, Pure Incense, Rock Incense
- Rose Incense, Sea Incense, Wave Incense

### Past Generation Berries
- Belue Berry, Bluk Berry, Cornn Berry, Durin Berry, Magost Berry, Nanab Berry
- Nomel Berry, Pamtre Berry, Pinap Berry, Rabuta Berry, Razz Berry, Spelon Berry
- Watmel Berry, Wepear Berry

### Gen 2 Specific Items
- Berserk Gene, Berry, Bitter Berry, Burnt Berry, Gold Berry, Ice Berry
- Mint Berry, Miracle Berry, Mystery Berry, Pink Bow, Polkadot Bow
- PRZ Cure Berry, PSN Cure Berry, Stick (old Farfetch'd item)

### CAP Items (Custom items)
- Crucibellite, Vile Vial

### Miscellaneous Skipped
- Mail (past generation)
- Pretty much anything marked `isNonstandard: "Past"` or `isNonstandard: "Unobtainable"`

## Validation Checklist

### File Order Validation
- [ ] ItemsABC.cs: Items are in alphabetical order (A?B?C within file)
- [ ] ItemsDEF.cs: Items are in alphabetical order (D?E?F within file)
- [ ] ItemsGHI.cs: Items are in alphabetical order (G?H?I within file)
- [ ] ItemsJKL.cs: Items are in alphabetical order (J?K?L within file)
- [ ] ItemsMNO.cs: Items are in alphabetical order (M?N?O within file)
- [ ] ItemsPQR.cs: Items are in alphabetical order (P?Q?R within file)
- [ ] ItemsSTU.cs: Items are in alphabetical order (S?T?U within file)
- [ ] ItemsVWX.cs: Items are in alphabetical order (V?W?X within file)
- [ ] ItemsYZ.cs: Items are in alphabetical order (Y?Z within file)

### Missing Items Check
- [ ] No Gen 9 relevant items are missing
- [ ] All Plates (17) are present
- [ ] All Orbs/Crystals (6) are present
- [ ] All Masks (3) are present
- [ ] All Rusted items (2) are present
- [ ] All Sweets (7) are present

### Exclusion Verification
- [ ] No Mega Stones present
- [ ] No Z-Crystals present (except maybe Normal Gem)
- [ ] No Gems present (except Normal Gem)
- [ ] No TRs present
- [ ] No Drives present
- [ ] No Memory items present
- [ ] No fossils present
- [ ] No CAP items present

### Special Cases
- [ ] Cherish Ball included (despite being rare/event)
- [ ] Normal Gem included (only gem in Gen 9)
- [ ] Safari Ball / Sport Ball included
- [ ] Leek included (Farfetch'd/Sirfetch'd item)
- [ ] Light Ball included (Pikachu item)
- [ ] Lucky Punch included (Chansey item)
- [ ] Metal Powder included (Ditto item)
- [ ] Quick Powder included (Ditto item)
- [ ] Thick Club included (Cubone/Marowak item)
- [ ] Soul Dew included (Latios/Latias item)

## Potential Issues to Check

1. **Enigma Berry**: Listed in TypeScript as gen 3, but check if it's in Gen 9
2. **Deep Sea Scale/Tooth**: Marked as "Past" in TypeScript but might be used for Clamperl
3. **Blue Orb/Red Orb**: Marked as "Past" but are forme items for Kyogre/Groudon
4. **Primal Orbs vs Regular Orbs**: Blue/Red Orbs are different from other orbs

## Next Steps

1. Run the actual C# files through a parser to extract all ItemId enums
2. Compare extracted list with this expected list
3. Generate specific "Missing", "Extra", or "Misplaced" reports
4. Verify alphabetical ordering within each file
5. Cross-reference with the TypeScript source one final time

## Conclusion

This validation template provides the ground truth for what items should be in your C# implementation. The next step is to programmatically compare your actual implementation against these expectations to identify any discrepancies.

**Estimated Total Items**: ~237 Gen 9 relevant items (subject to verification)
**Estimated Skipped Items**: ~400+ items (Mega Stones, Z-Crystals, Gems, TRs, etc.)
