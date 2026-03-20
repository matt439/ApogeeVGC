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

### 3. Mini-Model Heuristic Ensemble (`MctsSearchEnsemble`)

Replaced the monolithic DL model with an ensemble of 8 focused heuristic mini-models. Each mini-model evaluates candidate actions from one strategic perspective and returns preferences + a confidence score. The ensemble aggregates these into informed MCTS root priors via weighted averaging:

```
final(action) = Sum(weight_i × confidence_i × pref_i(action)) / Sum(weight_i × confidence_i)
```

**Mini-models:**

| Mini-Model | Purpose | Confidence |
|---|---|---|
| DamageMax | Prefer highest damage actions | Always high (0.9) |
| KOSeeking | Strongly prefer guaranteed KOs | High if KO possible, low otherwise |
| KOAvoidance | Prefer Protect/switch when in KO range | Scales with threat level |
| TypePositioning | Prefer switches to favorable matchups | Moderate (0.5) |
| DamageMin | Prefer defensive plays when threatened | Scales with threat level |
| SpeedAdvantage | Exploit speed tiers, Trick Room, priority | Moderate (0.6) |
| StatusSpreading | Prefer status moves on unstatused targets | Low-moderate (0.4) |
| ProtectPrediction | Deprioritize single-target into likely Protect | From opponent model or heuristic |

**Key design:** The ensemble only affects root priors (which actions to explore first). Leaf evaluation uses the proven `HeuristicEval`. Internal nodes use uniform priors. This means the ensemble can only help search efficiency, never degrade leaf evaluation quality.

**Architecture:** `PlayerMctsEnsemble` is a thin factory that creates a `PlayerMctsStandalone` backed by `MctsSearchEnsemble`, reusing all proven player infrastructure (legal action enumeration, choice building, team preview).

**Files:**
- `ApogeeVGC/Mcts/Ensemble/IMiniModel.cs` — interface
- `ApogeeVGC/Mcts/Ensemble/EnsembleEvaluator.cs` — weighted aggregation
- `ApogeeVGC/Mcts/Ensemble/MiniModels/` — 8 mini-model implementations
- `ApogeeVGC/Mcts/Ensemble/ProtectDetector.cs` — Protect move identification
- `ApogeeVGC/Mcts/MctsSearchEnsemble.cs` — ensemble search
- `ApogeeVGC/Mcts/PlayerMctsEnsemble.cs` — factory
- `ApogeeVGC/Mcts/IMctsSearch.cs` — common search interface

**CLI name:** `ensemble`

### 4. Opponent Prediction Model (DL)

A focused DL model that predicts the opponent's next action. Unlike BattleNet (which suffered from distribution shift), this model predicts at the current state — no hypothetical future states — so distribution shift does not apply.

- Same input encoding as BattleNet (species/move/ability/item/tera embeddings + 264D numeric)
- Two output heads: opponent slot A and B action logits
- ~254K parameters (much smaller than BattleNet's 1.6M)
- Feeds into ProtectPrediction and KOAvoidance mini-models

**Python files:** `opponent_dataset.py`, `opponent_model.py`, `train_opponent.py`
**C# files:** `OpponentInference.cs`
**Export:** `python export_onnx.py opponent`

### 5. Ensemble Weight Tuning

Global weights for each mini-model are tuned via Optuna by playing evaluation games:
- 8 float parameters (one per mini-model), range [0, 5]
- Each trial plays 50 games, measures win rate
- 200 trials, ~1.5 hours total (no GPU needed)
- Best weights saved to `ensemble_config.json`

**File:** `Tools/DLModel/tune_ensemble.py`

### 6. Pipeline Changes

- `ensemble` added to default evaluation matchups
- `APOGEE_ENSEMBLE_CONFIG` env var for ensemble weights
- `APOGEE_OPPONENT_MODEL` env var for opponent prediction model
- `--battle-only` flag to skip TeamPreviewNet training

## Experimental Results

### Phase 1: Isolating the DL problem (with old HP-only heuristic)

| Configuration | vs Random | vs Standalone |
|---|---|---|
| MCTS+DL (policy + value) | 55.0% | 38.8% |
| Hybrid (uniform + value only) | 62.0% | 43.0% |
| Hybrid + confidence blend | 67.5% | 47.0% |
| Standalone (uniform + heuristic) | — | 50.0% |

Removing the bad policy improved results (+7%). Adding confidence blending closed the gap further (+5.5%). But the value head still underperformed the heuristic.

### Phase 2: Improved heuristic

| Configuration | vs Random | vs Standalone |
|---|---|---|
| Standalone (improved heuristic) | 73.5% | 50.0% |
| Hybrid + blend (improved heuristic) | 70.0% | 43.0% |

The improved heuristic was a major win for standalone (55% → 73.5% vs random). The DL value head still slightly degrades performance when blended in, confirming the distribution shift problem.

### Phase 3: Mini-model ensemble

| Configuration | vs Random | vs Standalone |
|---|---|---|
| Standalone (improved heuristic) | 68% (1k iter) | 50.0% |
| Ensemble (equal weights, 1k iter) | 70% | 51.0% |
| Ensemble (Optuna-tuned, 1k iter) | **88%** | 51.0% |
| Ensemble (Optuna-tuned, 10k iter) | — | **54.5%** |

The tuned ensemble beats standalone by 20 percentage points vs random (88% vs 68%) and shows a growing advantage with more iterations (51% → 54.5% head-to-head as iterations increase from 1k to 10k).

**Optimal weights (from 200-trial Optuna tuning):**

| Mini-Model | Weight | Interpretation |
|---|---|---|
| ProtectPrediction | 4.31 | Most important — even heuristic Protect prediction is valuable |
| DamageMax | 3.65 | Damage output is a strong signal |
| SpeedAdvantage | 3.31 | Speed exploitation matters in VGC |
| TypePositioning | 2.32 | Type matchups guide switching |
| KOSeeking | 1.78 | KO opportunities are important but situational |
| DamageMin | 1.55 | Defensive play has moderate value |
| StatusSpreading | 1.10 | Status moves are low priority |
| KOAvoidance | 0.88 | Largely redundant with DamageMin + ProtectPred |

## Design Rationale

### Why Mini-Models Over Monolithic DL

The monolithic BattleNet had 72% offline value accuracy but failed in live MCTS play due to distribution shift — it was trained on human replay states but evaluated on novel states MCTS explores during search. The mini-model ensemble avoids this entirely:

1. **No training data needed** — heuristics work on any state, no distribution shift
2. **Interpretable** — each model's contribution is visible and tunable
3. **Fast to iterate** — weight tuning takes hours, not overnight training runs
4. **Compositional** — new strategic insights become new mini-models
5. **Graceful degradation** — if a model is wrong, its low confidence limits damage

### Role of DL in the New Architecture

DL is repositioned from evaluation (where distribution shift kills it) to prediction (where it works):
- **TeamPreviewNet** (95% bring accuracy) — predicts which Pokemon to bring
- **OpponentPredictionNet** (planned) — predicts opponent's next action, feeds into mini-models
- Both predict at the current state, avoiding distribution shift

This separation — DL for prediction, heuristics for evaluation — plays to each approach's strengths.
