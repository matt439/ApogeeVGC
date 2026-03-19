# MCTS + DL System Changes

## Problem Statement

The DL-guided MCTS player was performing worse than both the standalone MCTS (no DL) and a simple greedy max-damage player. Evaluation results (1000 battles each, 10000 iterations):

| Player | vs Random | vs Standalone MCTS |
|---|---|---|
| DL-Greedy (policy head only) | 43.3% | — |
| MCTS+DL (policy priors + value head) | 55.0% | 38.8% |
| MCTS Standalone (uniform priors + HP heuristic) | ~55% | 50.0% |
| Greedy (max damage, no search) | ~84% | — |

The DL model was actively hurting MCTS performance. Two root causes were identified:

1. **Bad policy priors** — the policy head (61% top-1 accuracy) directs MCTS toward wrong moves, wasting the search budget
2. **Value head distribution shift** — trained on human replay states, the value head gives unreliable evaluations on the novel states MCTS explores during search

## Changes Made

### 1. Hybrid MCTS Player (`MctsSearchHybrid`, `PlayerMctsHybrid`)

A new MCTS variant that isolates the value head from the policy head:
- **Uniform priors** (like standalone) — removes the harmful policy head from search guidance
- **DL value head for leaf evaluation** — tests whether the value network alone provides useful signal
- **Confidence-weighted blending** — blends DL value with heuristic based on the model's certainty:

```
confidence = |dl_value - 0.5| × 2     // 0 = uncertain, 1 = confident
leaf_value = confidence × dl_value + (1 - confidence) × heuristic_value
```

When the DL model predicts near 0.5 (uncertain, likely out-of-distribution), it defers to the heuristic. When confident (near 0 or 1), it trusts its own evaluation.

**Files:** `MctsSearchHybrid.cs`, `PlayerMctsHybrid.cs`
**CLI name:** `mctshybrid`

### 2. Improved Heuristic Evaluation (`HeuristicEval`)

The original heuristic was `sigmoid(3 × (ourHPFrac - oppHPFrac))` — only HP and alive count. This was improved with:

**Type matchup advantage** — for each active Pokemon pair, computes the best move's `BasePower × TypeEffectiveness × STAB` score. A side with super-effective coverage scores higher.

**Status condition penalties** — sleep/freeze (can't act: 0.4), burn (0.2, but -0.1 for Guts users), paralysis (0.15), toxic (0.2), poison (0.1).

**Stat boost advantage** — sum of boost stages weighted by importance (offensive boosts 1.2×, speed 1.0×, defensive 0.8×).

Combined formula:
```
diff = (ourScore - oppScore) + 0.15 × matchupAdv + 0.10 × statusAdv + 0.05 × boostAdv
value = sigmoid(3 × diff)
```

**File:** `HeuristicEval.cs`

### 3. Domain Knowledge Features for BattleNet

The most significant change. The DL model previously received only embedding IDs (species, moves, abilities, items, tera) and basic numeric features (HP, status, boosts, field conditions). It had to implicitly learn the entire type effectiveness chart, move base powers, STAB relationships, and speed tiers purely from win/loss signal — requiring orders of magnitude more data than available.

20 new numeric features are added to the battle state encoding (dimension 244 → 264):

**Damage matrix (8 floats)** — estimated best damage fraction for each attacker→target pair among the 4 active Pokemon. Computes `max(BasePower × TypeEff × STAB × SpreadPenalty × StatRatio) / TargetHP` across the attacker's known moves. This is the single feature that makes the greedy player so strong.

```
[0] my_a → opp_a    [1] my_a → opp_b
[2] my_b → opp_a    [3] my_b → opp_b
[4] opp_a → my_a    [5] opp_a → my_b
[6] opp_b → my_a    [7] opp_b → my_b
```

Information asymmetry is preserved:
- Own moves: always fully known (end-of-game revealed data in training, full info in C#)
- Opponent moves: only progressively revealed moves used (matches training data distribution)

**Speed comparison (4 floats)** — binary flags indicating whether each of our active Pokemon is faster than each opponent active. Accounts for Trick Room (reverses the comparison).

**Pokemon types (8 floats)** — type1 and type2 indices for each active slot, normalized by /18. Lets the model reason about resistances without re-deriving types from species embeddings.

Spread moves apply a 0.75× damage penalty (standard doubles mechanic) to prevent overvaluing moves like Dazzling Gleam in the damage matrix.

**Python files:** `game_data.py` (type chart, damage estimation), `dataset.py` (encode_matchup), `format_spec.py` (MATCHUP_DIM)
**C# files:** `StateEncoder.cs` (EncodeMatchup, BestDamageFraction)
**Data files:** `game_data/species_data.json` (1321 species), `game_data/move_data.json` (686 moves)

### 4. MCTS Configuration Changes

- Default iterations: 200 → 10,000
- Standalone iterations: 1,000 → 10,000
- Pipeline evaluation iterations: 1,000 → 10,000

With ~400 joint actions in VGC doubles (20 moves × 20 moves), 200 iterations couldn't visit each action even once. 10,000 provides ~25 visits per action on average.

### 5. Pipeline Changes

- `--battle-only` flag to skip TeamPreviewNet training (only retrain BattleNet)
- `mctshybrid` added to default evaluation matchups
- Evaluation battle count default remains 1000 per matchup

## Experimental Results

### Isolating the problem (with old HP-only heuristic)

| Configuration | vs Random | vs Standalone |
|---|---|---|
| MCTS+DL (policy + value) | 55.0% | 38.8% |
| Hybrid (uniform + value only) | 62.0% | 43.0% |
| Hybrid + confidence blend | 67.5% | 47.0% |
| Standalone (uniform + heuristic) | — | 50.0% |

Removing the bad policy improved results (+7%). Adding confidence blending closed the gap further (+5.5%). But the value head still underperformed the heuristic.

### With improved heuristic

| Configuration | vs Random | vs Standalone |
|---|---|---|
| Standalone (improved heuristic) | 73.5% | 50.0% |
| Hybrid + blend (improved heuristic) | 70.0% | 43.0% |

The improved heuristic was a major win for standalone (55% → 73.5% vs random). The DL value head still slightly degrades performance when blended in, confirming the distribution shift problem.

## Design Rationale: Domain Knowledge as Input Features

The BattleNet model must learn from ~300K replays with ~780K decision points. Without explicit domain knowledge, it must implicitly discover:
- The 18×18 type effectiveness chart (324 entries)
- Base power and category of 272 moves
- STAB relationships for 978 species
- Speed tier orderings

These are static lookup tables that don't change between games. Asking the model to rediscover them from win/loss signal alone would require billions of samples (as in AlphaZero-style self-play) rather than the hundreds of thousands available from human replays.

The 20 new features encode this static game knowledge explicitly, freeing the model to focus on learning dynamic strategic patterns: when to Protect, optimal switching, tera timing, and team composition interactions. This is a principled design choice that reduces sample complexity from self-play scale to behavioral cloning scale.

## Retraining Required

Only BattleNet requires retraining (numeric input dimension changed 244 → 264). TeamPreviewNet is unaffected — it uses separate embedding-only inputs with no numeric features.

```bash
python pipeline.py --format gen9vgc2025regi --battle-only
```
