# Mini-Model Ensemble — Current & Proposed

## Architecture

Each mini-model implements `IMiniModel`:
- Receives: battle state, our side, legal action edges, optional opponent prediction, optional info tracker
- Returns: `MiniModelScore[]` with a preference value (0–1) and confidence (0–1) per action
- Aggregated by `EnsembleEvaluator` via weighted averaging:
  ```
  final(action) = Σ(weight_i × confidence_i × pref_i) / Σ(weight_i × confidence_i)
  ```
- Weights tuned by Optuna (`tune_ensemble.py`)

**Files:**
- `ApogeeVGC/Mcts/Ensemble/IMiniModel.cs` — interface + MiniModelScore struct
- `ApogeeVGC/Mcts/Ensemble/EnsembleEvaluator.cs` — weighted aggregation
- `ApogeeVGC/Mcts/Ensemble/MiniModels/` — all implementations
- `Tools/DLModel/tune_ensemble.py` — Optuna weight tuning
- `ensemble_config.json` — tuned weights

---

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

---

## Proposed Mini-Models (19)

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

### 13. FieldControl
**Purpose:** Value weather, terrain, and screen management.
**Examples:**
- Set sun if our team benefits (Chlorophyll users, Fire moves)
- Remove opponent's terrain if it powers their moves
- Set up Light Screen/Reflect when opponent has strong attacks
- Prioritize Tailwind for speed control
**Why:** Field effects multiply team power but are currently unvalued by the ensemble.

### 14. SetupPunishment
**Purpose:** Recognize when the opponent is setting up (Calm Mind, Swords Dance, Trick Room) and prioritize disruption.
**Responses:**
- Attack the setup user before they benefit
- Taunt to prevent status/setup moves
- Encore to lock them into the setup move
- Haze/Clear Smog to remove boosts
**Why:** Letting an opponent set up freely is often game-losing. Currently the bot has no urgency to prevent it.

### 15. EndgamePlanning
**Purpose:** Recognize favorable or unfavorable endgame scenarios and play toward/against them.
**Examples:**
- "Their last Pokemon is Trick Room setter + slow sweeper, don't let them set up"
- "We have a speed advantage in the 2v1 endgame, just attack"
- "Their Calyrex-Ice walls our remaining Pokemon, we need to keep our counter alive"
**Why:** Late-game play requires understanding win conditions over multiple turns, not just the current turn.

### 16. ItemAwareness
**Purpose:** Account for known or inferred held items in decision-making.
**Examples:**
- Choice Scarf — locked into one move, expect the same attack
- Focus Sash — won't be KO'd from full HP, don't double into it
- Assault Vest — can't use status moves, no Protect
- Leftovers/Sitrus Berry — sustain, may need to apply pressure
**Why:** Items dramatically change optimal play but are currently ignored by mini-models.

### 17. AbilityAwareness
**Purpose:** Account for known or inferred abilities.
**Examples:**
- Intimidate — expect attack drops when switching in
- Flash Fire — don't use Fire moves into it
- Storm Drain / Lightning Rod — redirects moves, don't target the partner
- Prankster — expect priority status moves
**Why:** Abilities create tactical constraints that the current ensemble doesn't reason about.

### 18. TrickRoomManagement
**Purpose:** Reason about setting, reversing, preventing, and stalling out Trick Room.
**Scenarios:**
- **Setting (we're the TR team):** Score TR setup when slow attackers are alive and ready to sweep. Penalize when slow mons are low HP or setter is in KO range.
- **Reversing (we're the fast team):** Evaluate counterplay — Taunt the setter, KO the setter before it moves, bring our own setter to reverse, or stall remaining turns with Protect.
- **Re-setting:** Recognize when TR is about to expire and slow mons still need it.
- **Turn counting:** Track remaining TR turns (5 total including setup turn). Stall with Protect when 1–2 turns remain rather than committing to a reverse.
- **Speed inversion evaluation:** Score TR setup proportional to team-wide benefit, not just current actives.
- **Setter threat tracking:** If their setter is KO'd with no backup, deactivate the threat assessment.
**Interactions:** CoordinatedAction (setter + Follow Me/Protect pairs), FieldControl (Tailwind as alternative), WinConditionAwareness (desperation TR flips).
**Confidence:** High when TR is active, about to be set, or about to expire. Low in neutral game states with no TR threat.
**Why:** SpeedAdvantage only accounts for who moves first under current conditions. It doesn't reason about whether to set, reverse, or prevent TR — which is often the single most game-defining decision.

### 19. ThreatPrioritization
**Purpose:** Determine which opponent slot to focus based on strategic threat, not just damage output.
**Examples:**
- Their Rillaboom is the only answer to our Kyogre in the back — it needs to die even if the other slot is easier to KO
- Their support mon (Amoonguss, Indeedee) enables their sweeper — removing it first cripples their gameplan
- Their last restricted legendary is the win condition — all pressure goes there
**Input:** Opponent team composition, remaining Pokemon, role identification (sweeper/support/tank), matchup dependency analysis.
**Confidence:** Moderate-high (0.6–0.8).
**Why:** DamageMax picks the highest-damage move regardless of target. Human players constantly evaluate which target is more dangerous to their gameplan.

### 20. SacrificePlay
**Purpose:** Recognize when letting a Pokemon faint is correct because it enables a free switch-in or preserves more valuable resources.
**Examples:**
- Incineroar has already used Fake Out and Intimidate — let it go down to bring in sweeper
- A weakened support mon is expendable if its death lets you position your win condition safely
- Sacking into a predicted spread move so the important slot can Protect
**Input:** Remaining utility of each active Pokemon, bench Pokemon value, opponent's likely actions.
**Confidence:** Low-moderate (0.3–0.5) — sack plays are high-skill reads and should only nudge, not dominate.
**Why:** KOAvoidance uniformly tries to keep everything alive. Sometimes the correct play is to deliberately not protect an expendable mon.

### 21. ProtectCadence
**Purpose:** Evaluate when and which slot should use Protect proactively, beyond just reacting to opponent threats.
**Examples:**
- Protect to scout the opponent's targeting and move choices before committing
- Protect to stall out Tailwind, Trick Room, weather, or terrain turns
- Protect one slot to buy time for the partner's setup move or slow pivot
- Avoid Protecting when the opponent expects it (diminishing returns on consecutive Protects)
**Input:** Remaining field effect turns, opponent's likely aggression level, consecutive Protect history.
**Confidence:** Moderate (0.5).
**Why:** ProtectPrediction handles "will they Protect?" but doesn't reason about our own Protect usage.

### 22. IntimidateCycling
**Purpose:** Value pivot plays that re-trigger switch-in abilities like Intimidate, Fake Out, Regenerator, or Download.
**Examples:**
- Switch Incineroar out after Fake Out + Intimidate are spent, then back in later for another round
- Pivot Amoonguss out to recover HP via Regenerator, then bring it back for redirection
- Cycle Landorus-T Intimidates to progressively weaken physical attackers
**Input:** Switch-in ability status (used/available), bench Pokemon with re-triggerable abilities.
**Confidence:** Moderate (0.5–0.6).
**Why:** The current ensemble treats switching as positioning (TypePositioning) or penalizes it (SwitchMomentum). It doesn't value re-triggering key abilities, which is a core VGC pattern.

### 23. InformationAsymmetry
**Purpose:** Exploit the gap between revealed and unrevealed information to maintain strategic surprise.
**Examples:**
- Favor unrevealed coverage moves when multiple options are close in value
- Delay revealing a surprise Tera type until it creates maximum impact
- If the opponent hasn't revealed their fourth move, factor in uncertainty about hidden threats
**Input:** Revealed moves/abilities/items for both sides, remaining unrevealed slots.
**Confidence:** Low (0.2–0.3) — a tiebreaker when options are close, not a primary decision driver.
**Why:** Human players are deeply conscious of information management. This model adds a small bias toward preserving hidden options.

### 24. SwitchInPunishment
**Purpose:** Predict opponent switches and fire moves that hit the incoming Pokemon hard.
**Examples:**
- Opponent's active has a terrible matchup and is likely to switch — throw a move that hits common switch-ins super-effectively
- Opponent just lost a mon and must send something in — predict what's coming and pre-target it
- Use spread moves to hedge when a switch-in is likely but the target is uncertain
**Input:** Opponent's likely switch candidates (from bench), type coverage against likely switch-ins.
**Confidence:** Low-moderate (0.3–0.5) — switch reads are high-variance predictions.
**Why:** TypePositioning handles our own switches. This is the offensive counterpart — punishing the opponent's switches.

### 25. TempoPreservation
**Purpose:** Penalize plays that concede tempo without strategic compensation.
**Examples:**
- Switching both slots in the same turn gives the opponent two free attacks
- Using non-damaging moves on both slots simultaneously lets the opponent act freely
- Protecting when there's no real threat wastes a turn
**Input:** Number of offensive actions this turn, opponent's threat level, game pace assessment.
**Confidence:** Moderate (0.5).
**Why:** Subtly different from WinConditionAwareness — you can be ahead and still bleed your lead through passive play. Acts as a penalty layer.

### 26. RecoveryTiming
**Purpose:** Evaluate when HP preservation through healing outvalues offensive pressure.
**Examples:**
- Use Recover/Roost on a win-condition mon when it can survive the turn
- Stall with Protect + Leftovers/Grassy Terrain recovery to accumulate passive healing
- Recognize when healing is futile (opponent's damage output exceeds recovery rate)
**Input:** Healing moves available, passive recovery sources, mon's role as win condition.
**Confidence:** Low-moderate (0.3–0.5).
**Why:** No current model reasons about healing. The choice between attacking and recovering is a frequent decision point.

### 27. ArchetypeCounterStrategy
**Purpose:** Recognize the opponent's team archetype and bias the ensemble's overall behavior accordingly.
**Examples:**
- **vs Hard Trick Room:** Apply maximum early pressure, prioritize Taunt/KO on setter
- **vs Hyper Offense:** Value defensive pivots and Intimidate cycling more highly
- **vs Sun/Rain:** Prioritize weather control, lead with counter-weather setter
- **vs Balance/Bulky Offense:** Expect longer games, value chip damage and status spreading
**Input:** Opponent's revealed team composition, identified archetype pattern.
**Confidence:** Moderate (0.5–0.6) in early game when archetype is identified.
**Why:** Architecturally unique — rather than scoring individual actions, it could dynamically adjust other models' weights (e.g., boost SetupPunishment against setup-heavy teams).

### 28. SecondaryEffectExploitation
**Purpose:** Value the probability-weighted upside of move secondary effects beyond raw damage.
**Examples:**
- Rock Slide: 30% flinch chance per target in doubles — massive expected value when outspeeding
- Icy Wind: guaranteed speed drop on both targets, can flip speed tiers
- Snarl: SpAtk drop on both targets, neuters special attackers
- Body Slam/Nuzzle: paralysis chance cripples fast sweepers permanently
**Input:** Secondary effect type and probability, number of targets hit, strategic value in current state.
**Confidence:** Low-moderate (0.3–0.5) — secondary effects are probabilistic, not guaranteed.
**Why:** DamageMax only considers raw damage. A move with slightly less power but a game-swinging secondary effect is often the correct choice.

---

## Priority Ranking (All Proposed Models)

| Priority | Model | Impact | Effort | Notes |
|---|---|---|---|---|
| 1 | CoordinatedAction | Very High | Medium | First joint-action model, interface changes needed |
| 2 | TrickRoomManagement | Very High | Medium | Often the single most game-defining decision |
| 3 | WinConditionAwareness | High | Low | Simple game state assessment |
| 4 | TeraOptimization | High | Medium | One-use resource evaluation |
| 5 | FieldControl | High | Medium | Weather/terrain/screen logic |
| 6 | ThreatPrioritization | High | Medium | Target selection beyond raw damage |
| 7 | SetupPunishment | Medium | Low | Detect setup moves, boost urgency |
| 8 | TempoPreservation | Medium | Low | Penalty layer, not full scoring model |
| 9 | SacrificePlay | Medium | Medium | Requires role/value assessment per mon |
| 10 | ArchetypeCounterStrategy | Medium | High | Meta-weight-adjuster, architecturally unique |
| 11 | ProtectCadence | Medium | Low | Extends existing Protect logic |
| 12 | SecondaryEffectExploitation | Medium | Low | Lookup-based with situational multiplier |
| 13 | IntimidateCycling | Medium | Low | Ability state tracking |
| 14 | EndgamePlanning | Medium | High | Needs multi-turn reasoning |
| 15 | ItemAwareness | Medium | Low | Lookup-based |
| 16 | AbilityAwareness | Medium | Low | Lookup-based |
| 17 | SwitchInPunishment | Low-Med | Medium | High-variance prediction |
| 18 | InformationAsymmetry | Low | Low | Tiebreaker model only |
| 19 | RecoveryTiming | Low | Low | Niche — only relevant for specific mons |

---

## Architectural Notes

- **CoordinatedAction** would be the first model to evaluate joint actions across both slots. May require changes to the `IMiniModel` interface or a separate aggregation step.
- **SacrificePlay** acts as a conditional negative modifier on KOAvoidance rather than a standalone scorer.
- **TempoPreservation** functions as a penalty layer — it penalizes tempo-negative patterns.
- **ArchetypeCounterStrategy** is architecturally distinct — it adjusts other models' weights rather than scoring actions directly. May need a separate integration point outside `IMiniModel`.
- **TrickRoomManagement** interacts heavily with CoordinatedAction, FieldControl, SpeedAdvantage, and WinConditionAwareness.
- **ItemAwareness** and **AbilityAwareness** depend on the information system (BattleInfoTracker) to know which items/abilities have been observed.
- **InformationAsymmetry** and **SacrificePlay** should have low base weights — refinement signals, not primary decision drivers.
- All new models require ensemble weight retuning via Optuna after addition.
