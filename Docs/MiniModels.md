# Mini-Model Ensemble — Current & Proposed

## Architecture

Each mini-model implements `IMiniModel`:
- Receives: battle state, our side, legal action edges, optional opponent prediction
- Returns: `MiniModelScore[]` with a preference value (0–1) and confidence (0–1) per action
- Aggregated by `EnsembleEvaluator` via weighted averaging:
  ```
  final(action) = Σ(weight_i × confidence_i × pref_i) / Σ(weight_i × confidence_i)
  ```
- Weights tuned by Optuna (`tune_ensemble.py`)

## Existing Mini-Models (9)

### 1. DamageMax
**Purpose:** Prefer actions that deal the most damage to opponents.
**Input:** Attacker moves, target type/HP, base power, type effectiveness, STAB.
**Confidence:** Always high (0.9) — damage output is always relevant.
**Weight (tuned):** 2.30

### 2. DamageMin
**Purpose:** Prefer defensive plays when our Pokemon are threatened.
**Input:** Opponent's best damage against each of our active Pokemon.
**Confidence:** Scales with threat level — high when our Pokemon face super-effective or high-power moves.
**Weight (tuned):** 4.19

### 3. KOSeeking
**Purpose:** Strongly prefer actions that guarantee a KO on an opponent Pokemon.
**Input:** Estimated damage vs remaining HP for each target.
**Confidence:** High (0.9) if a KO is possible, very low otherwise.
**Weight (tuned):** 0.34

### 4. KOAvoidance
**Purpose:** Prefer Protect or switching when our Pokemon is in KO range.
**Input:** Opponent's best damage against our active Pokemon, our remaining HP.
**Confidence:** Scales with how close we are to being KO'd.
**Weight (tuned):** 1.76

### 5. TypePositioning
**Purpose:** Prefer switches that put favorable type matchups on the field.
**Input:** Type effectiveness matrix for all our bench Pokemon vs opponent actives.
**Confidence:** Moderate (0.5) — type advantage matters but isn't everything.
**Weight (tuned):** 1.55

### 6. SpeedAdvantage
**Purpose:** Exploit speed tiers and Trick Room. Prefer actions where we move first.
**Input:** Speed stats of all active Pokemon, Trick Room status, priority moves.
**Confidence:** Moderate (0.6).
**Weight (tuned):** 1.32

### 7. StatusSpreading
**Purpose:** Prefer status moves (Thunder Wave, Will-O-Wisp, etc.) on unstatused targets.
**Input:** Opponent status conditions, our available status moves.
**Confidence:** Low-moderate (0.4) — status is valuable but situational.
**Weight (tuned):** 1.76

### 8. ProtectPrediction
**Purpose:** Deprioritize single-target moves into a target likely to Protect. Prioritize spread moves or targeting the other slot.
**Input:** Opponent prediction model output (if available) or heuristic Protect likelihood.
**Confidence:** From opponent model confidence, or low (0.3) for heuristic fallback.
**Weight (tuned):** 3.54

### 9. SwitchMomentum
**Purpose:** Penalize switching when we just switched in. Prevents the switching loop problem where the bot swaps back and forth.
**Input:** Whether each active Pokemon switched in on the previous turn.
**Confidence:** 0.7 when recently switched, 0.0 otherwise (only activates when relevant).
**Weight (tuned):** 1.23

## Proposed Mini-Models

### 10. CoordinatedAction
**Purpose:** Score joint actions that synergize between the two active slots.
**Examples:**
- Fake Out + setup move (one slot flinches, other uses Swords Dance)
- Redirect + nuke (Follow Me on one slot, powerful move on the other)
- Spread move + single-target (Earthquake + Flying-type partner)
- Double-target (both slots attack the same target for guaranteed KO)
**Why:** Currently each slot's action is scored independently. Coordinated play is a major gap.

### 11. WinConditionAwareness
**Purpose:** Adjust aggression based on game state advantage.
**When ahead:** Prefer safe plays (Protect, resist switches, don't trade). Conservative play preserves the lead.
**When behind:** Prefer high-risk/high-reward plays (aggressive tera, double-up on a target, risky predictions).
**Input:** Pokemon count advantage, HP advantage, remaining resources (tera, key moves).
**Why:** The bot currently plays the same regardless of whether it's winning 4-1 or losing 1-4.

### 12. TeraOptimization
**Purpose:** Evaluate when and on which Pokemon to terastallize.
**Considerations:**
- Tera is one-use per game — don't waste it
- Defensive tera (change typing to resist incoming attack)
- Offensive tera (boost STAB damage)
- Don't tera a Pokemon that's about to faint
**Why:** The bot currently teras opportunistically. Strategic tera timing is critical at high-level play.

### 13. EndgamePlanning
**Purpose:** Recognize favorable or unfavorable endgame scenarios and play toward/against them.
**Examples:**
- "Their last Pokemon is Trick Room setter + slow sweeper, don't let them set up"
- "We have a speed advantage in the 2v1 endgame, just attack"
- "Their Calyrex-Ice walls our remaining Pokemon, we need to keep our counter alive"
**Why:** Late-game play requires understanding win conditions over multiple turns, not just the current turn.

### 14. ItemAwareness
**Purpose:** Account for known or inferred held items in decision-making.
**Examples:**
- Choice Scarf — locked into one move, expect the same attack
- Focus Sash — won't be KO'd from full HP, don't double into it
- Assault Vest — can't use status moves, no Protect
- Leftovers/Sitrus Berry — sustain, may need to apply pressure
**Why:** Items dramatically change optimal play but are currently ignored by mini-models.

### 15. AbilityAwareness
**Purpose:** Account for known or inferred abilities.
**Examples:**
- Intimidate — expect attack drops when switching in
- Flash Fire — don't use Fire moves into it
- Storm Drain / Lightning Rod — redirects moves, don't target the partner
- Prankster — expect priority status moves
**Why:** Abilities create tactical constraints that the current ensemble doesn't reason about.

### 16. SetupPunishment
**Purpose:** Recognize when the opponent is setting up (Calm Mind, Swords Dance, Trick Room) and prioritize disruption.
**Responses:**
- Attack the setup user before they benefit
- Taunt to prevent status/setup moves
- Encore to lock them into the setup move
- Haze/Clear Smog to remove boosts
**Why:** Letting an opponent set up freely is often game-losing. Currently the bot has no urgency to prevent it.

### 17. FieldControl
**Purpose:** Value weather, terrain, and screen management.
**Examples:**
- Set sun if our team benefits (Chlorophyll users, Fire moves)
- Remove opponent's terrain if it powers their moves
- Set up Light Screen/Reflect when opponent has strong attacks
- Prioritize Tailwind for speed control
**Why:** Field effects multiply team power but are currently unvalued by the ensemble.

## Priority Ranking for Implementation

| Priority | Model | Impact | Effort |
|---|---|---|---|
| 1 | CoordinatedAction | Very High | Medium — need to score action pairs |
| 2 | WinConditionAwareness | High | Low — simple game state assessment |
| 3 | TeraOptimization | High | Medium — one-use resource evaluation |
| 4 | FieldControl | High | Medium — weather/terrain/screen logic |
| 5 | SetupPunishment | Medium | Low — detect setup moves, boost urgency |
| 6 | EndgamePlanning | Medium | High — needs multi-turn reasoning |
| 7 | ItemAwareness | Medium | Low — lookup-based |
| 8 | AbilityAwareness | Medium | Low — lookup-based |

## Notes

- All existing models operate on single-slot actions. CoordinatedAction would be the first to evaluate joint actions, which requires changes to the `IMiniModel` interface or a separate aggregation step.
- ItemAwareness and AbilityAwareness depend on the information system (revealed-info tracking) to know which items/abilities have been observed.
- Weights should be retuned after adding new models — the optimal balance shifts when new signals are introduced.
