# VGC Information Model

This document formalises what is known and unknown to each player at each phase
of a VGC battle, under both Open Team Sheets (OTS) and Closed Team Sheets (CTS).

---

## 1. Team Preview Phase

### What both players always see (OTS and CTS)

| Information          | Details                                      |
|----------------------|----------------------------------------------|
| Species (x6 each)   | Both players' full rosters of 6 species      |

**Important caveat — visually ambiguous forms:** Some Pokemon have
gameplay-distinct forms that are difficult to distinguish from the team preview
sprite alone. The key example:

| Pokemon | Hidden distinction                               |
|---------|--------------------------------------------------|
| Urshifu | Single Strike (Dark) vs Rapid Strike (Water)     |

Single Strike and Rapid Strike Urshifu have near-identical sprites — this is
genuinely hidden information at team preview under CTS. Under OTS the form is
revealed by the moveset.

Note: Other gender/form-variant Pokemon (Indeedee, Meowstic, Basculegion,
Toxtricity, Oinkologne) have visually distinct sprites and are identifiable at
team preview.

### Additional information visible under OTS

| Information | Details                                             |
|-------------|-----------------------------------------------------|
| Moves       | All 4 moves on each of the 12 Pokemon               |
| Ability     | The ability each Pokemon is running                  |
| Item        | The held item on each Pokemon                        |
| Tera type   | The tera type each Pokemon is set to                 |

### Information hidden at team preview (both OTS and CTS)

| Information       | Details                                                    |
|-------------------|------------------------------------------------------------|
| EVs / IVs         | Stat investment is never shown on team sheets              |
| Nature            | Not displayed on team sheets                               |
| Actual stat values | Derived from species base stats + EVs + IVs + nature      |
| Bring selection   | Which 4 of 6 each player will bring                        |
| Lead selection    | Which 2 of the 4 brought will start in the active slots    |

### Additional information hidden under CTS (beyond the above)

| Information | Details                                     |
|-------------|---------------------------------------------|
| Moves       | All 4 moves on each opponent Pokemon         |
| Ability     | Which ability each opponent Pokemon has       |
| Item        | Held items on opponent Pokemon                |
| Tera type   | Tera type of each opponent Pokemon            |

---

## 2. In-Battle Phase

### Always known (both OTS and CTS)

| Information                  | Details                                                    |
|------------------------------|------------------------------------------------------------|
| Own team (complete)          | Full knowledge of own Pokemon: stats, moves, items, etc.   |
| Active species               | Both players' active Pokemon species are visible *          |
| Current HP %                 | Shown for all active Pokemon (percentage, not exact value)  |
| Status conditions            | Burn, paralysis, sleep, poison, toxic, freeze — visible     |
| Stat boosts/drops            | All stage changes are announced and visible                 |
| Volatile conditions          | Confusion, encore, taunt, substitute, etc. — visible        |
| Weather                      | Active weather and remaining turns                          |
| Terrain                      | Active terrain and remaining turns                          |
| Trick Room                   | Whether active and remaining turns                          |
| Side conditions              | Tailwind, Reflect, Light Screen, Aurora Veil — visible      |
| Terastallized state          | Whether a Pokemon has terastallized (and to what type)      |
| Tera availability            | Whether each side has used their tera for the game          |
| Turn number                  | Current turn count                                          |

\* **Zoroark / Zoroark-Hisui exception:** Zoroark's Illusion ability disguises
it as the last Pokemon in the party. The opponent sees a fake species, type, and
appearance until Zoroark takes direct damage. When the opponent has a Zoroark on
their roster (visible at team preview), the identity of any active Pokemon is
uncertain until confirmed by taking a hit. This also means type matchups,
expected moves, and even damage calculations based on the displayed species may
be wrong.

### Progressively revealed during battle (OTS)

Under OTS, only the following are hidden at battle start and revealed during play:

| Information             | How it's revealed                                           |
|-------------------------|-------------------------------------------------------------|
| Which 4 were brought    | Revealed as each Pokemon enters the field                   |
| Lead selection          | Revealed at turn 1                                          |
| Speed tier              | Inferred from move order each turn                          |
| Bulk investment         | Inferred from damage taken                                  |
| Offensive investment    | Inferred from damage dealt                                  |

### Progressively revealed during battle (CTS)

Under CTS, all of the above plus:

| Information             | How it's revealed                                           |
|-------------------------|-------------------------------------------------------------|
| Moves (per Pokemon)     | Revealed one at a time as they are used                     |
| Ability                 | Revealed when triggered (e.g. Intimidate on switch-in)      |
| Item                    | Revealed when consumed, activated, or removed (Knock Off)   |
| Tera type               | Revealed when the Pokemon terastallizes                     |

### Abilities that reveal hidden information

Certain abilities passively reveal opponent information on switch-in or during
battle. These are relevant under CTS (and for stat inference under OTS):

| Ability   | What it reveals                                              |
|-----------|--------------------------------------------------------------|
| Trace     | Reveals the opponent's ability (copies it)                   |
| Frisk     | Reveals the opponent's held item                             |
| Forewarn  | Reveals the opponent's highest base power move               |
| Imposter  | Reveals full moveset (Ditto transforms into the target)      |

Other moves/effects that reveal information:

| Move / Effect | What it reveals                                           |
|---------------|-----------------------------------------------------------|
| Focus Sash    | Announces itself when activated ("hung on using its Focus Sash!") |
| Resist Berries | Announced when consumed (e.g. "Shuca Berry weakened the damage!") |
| Knock Off     | Reveals (and removes) the opponent's held item             |
| Trick / Switcheroo | Reveals item by swapping it                          |
| Thief / Covet | Reveals item by stealing it                               |
| Skill Swap    | Reveals the opponent's ability (and swaps it)              |
| Role Play     | Reveals the opponent's ability (copies it)                 |

---

## 3. Logical Deductions from Game Events

Certain game events allow a player to deduce hidden information with certainty,
**but only when all alternative explanations can be ruled out.** Each deduction
below lists its required preconditions.

### Item deductions

**100% accurate move misses → Bright Powder / Lax Incense**
- Preconditions: No evasion boosts, no accuracy drops, no Sand Veil/Snow Cloak
  (no active sand/snow + matching ability), no ability that raises evasion
  (e.g. Tangled Feet while confused). Only valid for moves with 100% accuracy.

**Outsped by Pokemon that cannot outspeed at max Speed investment → Choice Scarf**
- Preconditions: No Tailwind, no speed boosts (e.g. Unburden, Speed Boost,
  Dragon Dance), no paralysis on your side, not under Trick Room. Must verify
  that even with 252 Speed EVs + Speed-boosting nature, the opponent's Pokemon
  cannot outspeed yours. Also rule out Quick Claw (which gives priority, not
  speed — the message is different).

**Move deals ~1.5x expected damage → Choice Band / Choice Specs**
- Preconditions: Rule out Life Orb (check for end-of-turn recoil), Helping Hand,
  critical hit, stat boosts, weather/terrain damage boosts. Need known EVs on
  your side to calculate expected damage. Physical → Band, Special → Specs.

**Pokemon uses a different move on consecutive turns → not Choice-locked**
- Preconditions: Rule out Trick/Switcheroo swapping the item away between turns.
  Confirms item is not Choice Band/Specs/Scarf (or that they switched out and
  back in between, resetting the lock).

**HP restores at end of turn → Leftovers / Black Sludge**
- Preconditions: No Grassy Terrain active, no Ingrain, no Aqua Ring, no
  ability-based healing (e.g. Rain Dish in rain, Ice Body in snow). Poison-type
  healing = could be Black Sludge or Leftovers. Non-Poison healing = Leftovers.

**HP drops at end of turn (no visible cause) → Life Orb / Black Sludge**
- Preconditions: No poison/burn/weather chip damage, no Leech Seed, no other
  end-of-turn damage source. If Pokemon attacked that turn, likely Life Orb
  (10% recoil). If non-Poison type taking chip without attacking, Black Sludge.
  Note: Magic Guard prevents Life Orb recoil.

**Pokemon eats a berry at >50% HP → Gluttony ability**
- Preconditions: Pinch berries normally activate at ≤25% HP (or ≤50% with
  Gluttony). If a pinch berry activates above 25%, Gluttony is confirmed.
  Rule out Ripen (which doubles berry effect but doesn't change threshold).

### Ability deductions

**Stats don't drop from Intimidate → blocking ability**
- Narrows to: Clear Body, White Smoke, Full Metal Body, Hyper Cutter (Atk
  only), Inner Focus (Gen 9+), Oblivious (Gen 9+), Own Tempo, Scrappy
  — OR the Pokemon has Substitute up, OR it's already at -6 Atk.
- Each species can only have specific abilities, so cross-referencing with
  the species' ability pool often uniquely identifies it.

**Stats rise from Intimidate → reactive ability**
- Competitive: +2 SpA on any stat drop
- Defiant: +2 Atk on any stat drop
- Contrary: stat changes are inverted (Atk rises instead of dropping)
- Mirror Armor: reflects the stat drop back (Intimidate user's Atk drops)

**Weather changes on switch-in → weather ability**
- Sun → Drought, Rain → Drizzle, Sand → Sand Stream, Snow → Snow Warning
- These are unambiguous: the message names the ability.

**Terrain changes on switch-in → terrain ability**
- Electric → Electric Surge, Grassy → Grassy Surge, Psychic → Psychic Surge,
  Misty → Misty Surge
- Unambiguous from the message.

**Ground move does no damage to non-Flying/Levitating Pokemon → Levitate**
- Preconditions: Target is not Flying type, not holding Air Balloon (would be
  announced), no Magnet Rise active, no Telekinesis active.

**Status move fails unexpectedly → immunity ability**
- Overcoat: blocks powder/spore moves
- Oblivious: blocks Taunt, Attract, Intimidate
- Sweet Veil: blocks sleep on the Pokemon's side
- Aroma Veil: blocks Taunt, Encore, Disable, etc. on the Pokemon's side
- Must rule out type immunities and Substitute.

### Speed tier deductions

**Pokemon A moves before Pokemon B → speed comparison**
- Only valid when: both used moves of the same priority bracket, no speed
  manipulation occurred that turn (Tailwind just set, Trick Room just set),
  and neither used a priority move.
- Must account for: stat boosts, paralysis (25% speed), Choice Scarf (1.5x),
  Tailwind (2x), Trick Room (reversal), abilities (Swift Swim, Chlorophyll,
  Sand Rush, Slush Rush, Unburden, Speed Boost, Protosynthesis, Quark Drive).

**Pokemon outspeeds when it shouldn't → speed investment or item**
- Could indicate any of: Choice Scarf, max Speed EVs + boosting nature,
  speed-boosting ability active. Need to rule out each possibility
  systematically. If the species can't reach the required speed even at max
  investment, confirms Choice Scarf (after ruling out abilities/boosts).

**Pokemon underspeeds when it shouldn't → minimal speed or Trick Room set**
- Could indicate: 0 Speed IVs + speed-reducing nature (for Trick Room),
  or simply uninvested speed. Relevant for knowing if they plan to use
  Trick Room.

### Damage range deductions

**Damage exceeds maximum possible without offensive item → item confirmed**
- Preconditions: Must know your own Pokemon's defensive stats exactly. Must
  rule out critical hit (1.5x), weather/terrain boost, Helping Hand (1.5x),
  stat boosts, and ability boosts (e.g. Huge Power, Pure Power, Hustle).
  Remaining excess confirms an offensive item; amount of excess narrows which.

**Damage falls within a calculable range → narrows EV spread**
- Requires: known species base stats, known move base power, known item (or
  ruling out item boost). The observed damage percentage maps to a range of
  possible attack EVs + nature combinations. Multiple observations from the
  same Pokemon can narrow this significantly.

**Survival at a specific HP threshold → defensive investment**
- If a Pokemon survives a hit that would KO at lower defensive investment,
  narrows the minimum defensive EVs. Especially informative when the KO
  threshold is well-known (e.g. "survives Adamant 252 Atk Garchomp Earthquake").

### Move deductions (CTS only)

**3 moves revealed → 1 unknown remaining**
- The remaining slot can be inferred from usage statistics and team
  composition. Some movesets are so standard that 2 revealed moves can
  effectively confirm the other 2.

**Pokemon uses a second move → not Choice-locked**
- Preconditions: rule out Trick/Switcheroo between uses, and that the
  Pokemon didn't switch out and back in (resetting Choice lock).

**Protect used → confirms Protect, narrows item**
- Protect is incompatible with Choice items in practice (you'd never run
  Protect with Choice Band). Confirms the item is not a Choice item.

**Setup move used → item inference**
- Swords Dance, Calm Mind, Dragon Dance, etc. are incompatible with Choice
  items in practice. Confirms non-Choice item.

---

## 4. Probabilistic / Soft Inferences

These are not certain deductions but narrow the probability space:

| Inference                  | Method                                                        |
|----------------------------|---------------------------------------------------------------|
| Speed EV investment        | Move order vs known base speeds; tie-breaking with items      |
| Bulk EV investment         | Damage dealt to them vs known moves/stats                     |
| Offensive EV investment    | Damage they deal vs known defensive stats                     |
| Remaining moveset (CTS)    | Process of elimination: 4 slots minus revealed moves          |
| Likely item (CTS)          | Usage statistics + behaviour (e.g. no recoil = likely AV)    |
| Likely ability (CTS)       | Usage statistics + no trigger observed                        |
| Likely tera type (CTS)     | Usage statistics + team composition                           |
| Unrevealed bench Pokemon   | 6 shown at preview, 4 brought; 2+ revealed = narrows options |
| Opponent's likely play     | Game theory + patterns + risk assessment                      |

---

## 4. Information Categories by Certainty

### Locked (100% known)
- Own team state (always)
- Species of all 12 Pokemon (always)
- Moves, ability, item, tera type of opponent (OTS only)
- Any information revealed during battle (both modes)

### Observable (revealed by game events)
- Moves used, abilities triggered, items consumed
- Damage rolls (narrow stat ranges)
- Speed checks (each turn's move order)

### Inferred (estimated with varying confidence)
- EV spreads (narrowed by damage/speed observations)
- Unrevealed moves/items/abilities in CTS
- Bring/lead selection tendencies

### Unknown (no information available)
- Exact EVs/IVs/nature (never directly shown)
- Opponent's chosen action for the current turn
- Future RNG outcomes (damage rolls, secondary effects, speed ties)

---

## 5. Implications for AI Design

### OTS Mode
The information gap is small. The main unknowns are:
1. **Bring/lead selection** — narrows as Pokemon are revealed
2. **Stat spreads** — inferred from speed checks and damage
3. **Turn-by-turn decisions** — the core game theory problem

A full-observability model is a reasonable approximation for OTS. The neural
network can learn to handle spread uncertainty implicitly from training data.
The MCTS search handles decision uncertainty through lookahead.

### CTS Mode
The information gap is large. Additionally unknown:
1. **Moves** — 4 per Pokemon, revealed one at a time
2. **Ability, item, tera type** — revealed by specific game events
3. **Everything from OTS mode above**

CTS requires either:
- A model trained on partial information (sees only what's been revealed)
- Explicit belief tracking over unknowns
- Information Set MCTS (sampling possible worlds consistent with observations)

### Priority for implementation
1. Restrict opponent perspective to realistic observability
2. Add revealed-information tracking (moves seen, items revealed, etc.)
3. Encode observation state as model features (move known/unknown flags, etc.)
4. Retrain model on realistic partial-information states
5. (Optional) Add inference engine for stat spread estimation from damage/speed
