# Thesis Narrative: AI for VGC Pokemon Battles

## Abstract

This thesis investigates AI approaches for the Pokemon Video Game Championships (VGC) doubles format, combining Monte Carlo Tree Search (MCTS) with deep learning and domain-knowledge heuristics. An initial monolithic deep learning approach (behavioral cloning from 300K human replays) produced a model with strong offline metrics but catastrophic live performance — worse than random play. Systematic diagnosis revealed two root causes: harmful policy priors and value head distribution shift. This led to a pivot: replacing the monolithic DL model with a modular heuristic ensemble of 8 focused "mini-models" with Optuna-tuned weights, achieving 74% win rate vs random and 53.2% head-to-head vs uniform MCTS. Deep learning was repositioned from evaluation (where distribution shift kills it) to prediction (team preview selection at 95% accuracy, and opponent action prediction for strategic planning). The work demonstrates that for complex strategy games with limited training data, domain-knowledge heuristics outperform learned evaluation, while DL excels at focused prediction tasks.

## Chapter 1: The Deep Learning Approach

### 1.1 BattleNet Architecture

The initial approach followed the AlphaZero paradigm: train a neural network to provide value estimates (win probability) and policy priors (action preferences) for MCTS. BattleNet was trained via behavioral cloning on ~300,000 human replays from Pokemon Showdown, producing ~780,000 decision-point samples.

**Architecture:** Per-pokemon embeddings (species 32D, moves/abilities/items/tera 16D each) → shared encoder → 3-layer trunk (256→256→128) → value head (sigmoid, win probability) + slot-conditioned policy heads (action logits).

**Offline metrics were promising:**

| Metric | BattleNet | Random Baseline | Popular Baseline |
|---|---|---|---|
| Value accuracy | 72.4% | 50.0% | 50.0% |
| Value ECE (calibration) | 0.011 | 0.0 | 0.0 |
| Policy top-1 accuracy | 61.4% | 0.06% | 27.2% |
| Policy top-3 accuracy | 92.1% | 0.19% | 35.4% |

The model clearly learned meaningful patterns — 72% value accuracy with excellent calibration, 61% policy accuracy well above baselines.

### 1.2 The DL-Greedy Disaster

When deployed for live play (selecting the highest-probability action each turn), the model performed at or below random:

| Evaluation | DL-Greedy Win Rate |
|---|---|
| DL-Greedy vs Random (30K battles, original model) | **49.06%** |
| DL-Greedy vs Random (30K battles, after parser fixes) | **43.3%** |

**49% against random** — the model's "learned intuition" was no better than coin flipping. This was the first sign that strong offline metrics do not guarantee live performance.

### 1.3 Attempted Fixes — Each Made Things Worse

Several modifications were attempted to improve the model. Each degraded performance further:

#### BCEWithLogitsLoss + Value Weight 2.0

Replaced `sigmoid + BCELoss` with numerically more stable `BCEWithLogitsLoss` and doubled the value head's loss weight to prioritize value learning.

| Metric | Original | After Change |
|---|---|---|
| Win rate vs Random | 49.06% | **46.35%** |
| Value StdDev | 0.242 | 0.393 |
| Value ECE | 0.011 | 0.038 |
| Predictions in 0.4-0.6 | 25.5% | 7.5% |

The value head became overconfident and polarized (median prediction 0.75), while policy accuracy dropped. The increased value weight starved the policy head of gradient signal.

#### BCEWithLogitsLoss Only (Reverted Weight)

Kept BCEWithLogitsLoss but reverted value weight to 1.0.

| Metric | Original | After Change |
|---|---|---|
| Win rate vs Random | 49.06% | **44.98%** |
| Value Mean | 0.583 | 0.325 |
| Value Median | 0.613 | 0.189 |

The value head flipped from slightly optimistic to extremely pessimistic. The model now thought it was losing almost every position. The sigmoid saturation in the original setup was effectively acting as implicit gradient clipping — removing it destabilized training dynamics.

#### Domain Knowledge Features (Matchup Features)

Added 20 new numeric features to the model: damage matrix (8 floats), speed comparison (4 floats), Pokemon types (8 floats). These encoded the static game mechanics (type chart, base power, STAB) that the model was struggling to learn implicitly.

| Metric | Original (244D) | + Matchup Features (264D) |
|---|---|---|
| Value accuracy | **72.4%** | 69.8% |
| Policy top-1 | **61.4%** | 60.9% |
| Win rate vs Random | **49.06%** | 41.2% |

**Adding more features made everything worse.** The model had more input dimensions but the same amount of training data. The matchup features added noise rather than signal — the model was already at its capacity ceiling with 780K samples. More features with the same data = more to learn, less signal per feature.

### 1.4 The Fundamental Problem

Every attempted improvement to the DL model degraded live performance, even when offline metrics looked reasonable. The root cause was twofold:

1. **Compounding errors in sequential decisions.** 61% per-decision accuracy means 39% of decisions are wrong. In a ~15-turn doubles game with ~30 decisions, P(all correct) ≈ 0.61^30 ≈ 0.00003. One bad decision (failing to Protect, wasting tera, wrong switch) cascades into a loss.

2. **Distribution shift between training and evaluation.** The model was trained on human replay states (sensible positions from good play). During MCTS search, it evaluates novel states produced by exploratory/random moves — positions that look nothing like human games. The model has never seen these states and gives unreliable predictions.

## Chapter 2: Diagnosing the MCTS Integration

### 2.1 What DL Does for MCTS

The DL model feeds into two MCTS phases:
- **Selection (policy priors):** which actions to explore first via the PUCT formula
- **Evaluation (value head):** how good is a leaf node position

### 2.2 Isolating Policy vs Value

A hybrid player was created to test each component independently:

**Hybrid MCTS:** uniform priors (like standalone) + DL value head for leaf evaluation. This isolates the value head's contribution without the policy priors.

**Confidence-weighted blend:** when the DL value is near 0.5 (uncertain, likely out-of-distribution), defer to the heuristic.

| Configuration | vs Random | vs Standalone |
|---|---|---|
| MCTS+DL (policy + value) | 55.0% | 38.8% |
| Hybrid (uniform + value only) | 62.0% | 43.0% |
| Hybrid + confidence blend | 67.5% | 47.0% |
| Standalone (uniform + heuristic) | — | 50.0% |

**Removing the bad policy helped** (+7% vs random). **Adding confidence blending helped more** (+5.5%). But the value head still underperformed the simple HP-based heuristic (47% vs standalone). Both DL components were net negative — the policy was actively harmful, and the value head was worse than a handcrafted heuristic.

### 2.3 The Improved Heuristic

The original heuristic was `sigmoid(3 × (ourHPFrac - oppHPFrac))`. It was extended with:
- **Type matchup advantage** — best move effectiveness for each active Pokemon pair
- **Status condition penalties** — sleep/freeze (0.4), burn (0.2), paralysis (0.15), toxic (0.2)
- **Stat boost advantage** — weighted sum of boost stages

| Configuration | vs Random |
|---|---|
| Standalone (HP-only heuristic) | ~55% |
| Standalone (improved heuristic) | **73.5%** |

A simple handcrafted heuristic with domain knowledge destroyed the DL model's performance. This confirmed that the bottleneck was not the model's capacity — it was the approach.

### 2.4 Summary of DL Failures

| Model Configuration | vs Random | Verdict |
|---|---|---|
| DL-Greedy (original) | 49.06% | Random-level |
| DL-Greedy (BCEWithLogitsLoss + vw=2) | 46.35% | Worse than random |
| DL-Greedy (BCEWithLogitsLoss + vw=1) | 44.98% | Worse than random |
| DL-Greedy (+ matchup features) | 41.2% | Significantly worse |
| MCTS+DL vs Standalone | 34.5% | DL hurts MCTS |
| Hybrid+blend vs Standalone | 47.0% | Value head still worse than heuristic |

Every DL variant underperformed. The model learned meaningful patterns offline but could not transfer them to live play.

## Chapter 3: The Ensemble Approach

### 3.1 Design Rationale

The diagnosis revealed that:
- DL policy priors misdirect search (harmful)
- DL value evaluation suffers distribution shift (unreliable)
- Handcrafted heuristics are robust on any state (reliable)

This motivated a modular ensemble of focused heuristic "mini-models," each evaluating candidate actions from one strategic perspective.

### 3.2 Mini-Model Architecture

Each mini-model returns:
- `Preference[0-1]` per candidate action — how strongly it recommends this action
- `Confidence[0-1]` — how relevant this model's opinion is in the current situation

Eight mini-models were implemented:

| Mini-Model | Strategic Focus |
|---|---|
| DamageMax | Prefer highest damage actions |
| KOSeeking | Strongly prefer guaranteed KOs |
| KOAvoidance | Prefer Protect/switch when in KO range |
| TypePositioning | Prefer switches to favorable type matchups |
| DamageMin | Prefer defensive plays when threatened |
| SpeedAdvantage | Exploit speed tiers, Trick Room, priority moves |
| StatusSpreading | Prefer status moves on unstatused targets |
| ProtectPrediction | Deprioritize single-target into likely Protect |

### 3.3 Aggregation and Integration

The ensemble aggregates mini-model outputs into MCTS root priors via confidence-weighted averaging:

```
final(action) = Σ(weight_i × confidence_i × preference_i(action)) / Σ(weight_i × confidence_i)
```

Global weights per mini-model are tuned via Bayesian optimization (Optuna, 200 trials, ~1.5 hours).

**Key design decision:** The ensemble only affects **root priors** (which actions to explore first). Leaf evaluation uses the proven `HeuristicEval`. This means the ensemble can only improve search efficiency — it cannot degrade leaf evaluation quality.

### 3.4 Optuna Weight Tuning Results

| Mini-Model | Optimal Weight | Interpretation |
|---|---|---|
| ProtectPrediction | 4.31 | Most important — anticipating Protect is critical in VGC |
| DamageMax | 3.65 | Raw damage output is a strong signal |
| SpeedAdvantage | 3.31 | Speed exploitation matters in VGC doubles |
| TypePositioning | 2.32 | Type matchups guide switching decisions |
| KOSeeking | 1.78 | KO opportunities are important but situational |
| DamageMin | 1.55 | Defensive play has moderate value |
| StatusSpreading | 1.10 | Status moves are low priority for priors |
| KOAvoidance | 0.88 | Largely redundant with DamageMin + ProtectPrediction |

The weight distribution reveals VGC strategy: Protect prediction and damage output dominate, while pure defense and status are secondary.

### 3.5 Evaluation Results

400 battles, 10,000 MCTS iterations, 8 threads:

| Matchup | Win Rate |
|---|---|
| Standalone vs Random | 70.3% |
| **Ensemble vs Random** | **74.0%** |
| **Ensemble vs Standalone** | **53.2%** |
| Ensemble vs Greedy | 41.7% |

The tuned ensemble beats standalone by 3.7 percentage points vs random and wins 53.2% head-to-head. The advantage grows with iteration count (51% at 1K iterations → 53.2% at 10K → expected to increase further).

## Chapter 4: Repositioning Deep Learning

### 4.1 Where DL Failed and Why

DL failed at **evaluation** — scoring arbitrary game states. The model was trained on human replay states but MCTS explores novel states during search. This distribution shift made the value head unreliable and the policy priors actively harmful.

### 4.2 Where DL Succeeds

DL succeeds at **prediction** — tasks where it predicts at the current game state, not hypothetical future states:

- **TeamPreviewNet (95% bring set accuracy):** Predicts which 4 of 6 Pokemon to bring and which 2 to lead. No distribution shift because team preview always looks the same — 12 species, make a selection.

- **Opponent Prediction Model (planned):** Predicts the opponent's next action given the current state. No distribution shift because it evaluates the actual game state, not MCTS-explored states. Feeds into ProtectPrediction and KOAvoidance mini-models.

### 4.3 The Separation Principle

The thesis demonstrates a clean separation:
- **DL for prediction:** What will happen? (team selection, opponent behavior)
- **Heuristics for evaluation:** How good is this state? (domain knowledge, type charts, damage calculation)
- **MCTS for search:** Given good priors and evaluation, find the best action

This separation plays to each approach's strengths and avoids distribution shift entirely.

## Chapter 5: Information System

### 5.1 Observability Constraints

VGC battles operate under imperfect information. Under Closed Team Sheets (CTS), the opponent's moves, abilities, items, and tera types are hidden until revealed through gameplay. The AI must respect these constraints.

### 5.2 Implementation

A `BattleInfoTracker` tracks revealed information per opponent Pokemon from battle events. The heuristic evaluation and mini-models filter opponent move access through the tracker — under CTS, they only consider moves that have been observed.

MCTS simulation retains full information (needed for legal move generation), but evaluation is restricted to revealed information only.

## Chapter 6: Conclusions

### 6.1 Key Findings

1. **Offline metrics are misleading.** 72% value accuracy and 61% policy accuracy did not translate to live performance. The model performed at or below random in every configuration tested.

2. **Distribution shift is the fundamental barrier** for behavioral cloning in game AI with limited data. The model learned to evaluate human-like positions but failed on the novel positions MCTS explores.

3. **More features ≠ better performance** with limited data. Adding 20 domain-knowledge features degraded the model — 780K samples was insufficient for the increased input dimensionality.

4. **Simple heuristics outperform complex DL** for state evaluation with 300K replays. A handcrafted type-matchup heuristic (73.5% vs random) beat every DL configuration tested.

5. **Modular ensembles beat monolithic models.** 8 focused mini-models with tuned weights (74% vs random, 53.2% vs standalone) outperformed both uniform MCTS and DL-guided MCTS.

6. **DL excels at focused prediction tasks.** Team preview (95% accuracy) and opponent action prediction work because they predict at the current state, avoiding distribution shift.

### 6.2 The Path Not Taken: Self-Play

The AlphaZero approach (self-play reinforcement learning) would likely solve the distribution shift problem — the model would train on its own search-generated states. However, this requires:
- Billions of self-play games (vs 300K human replays)
- Massive compute (vs a single RTX 5080)
- A tight training loop between Python (DL) and C# (simulator)

This was infeasible for an honours thesis but represents the natural next step for this research.

### 6.3 Contributions

1. A complete VGC battle simulator with MCTS search engine
2. Rigorous evaluation of behavioral cloning for VGC (honest negative result with analysis)
3. A novel mini-model ensemble architecture for MCTS prior generation
4. Bayesian optimization of ensemble weights via game outcomes
5. Demonstration that focused DL prediction (team preview, opponent modeling) complements heuristic evaluation
6. An information system supporting both OTS and CTS observability modes
