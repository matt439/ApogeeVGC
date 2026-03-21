# Thesis Reporting Plan

## Narrative Arc

The thesis tells a story of iterative engineering and honest analysis:

1. **Attempt**: Train a monolithic DL model (AlphaZero-style behavioral cloning) to guide MCTS
2. **Failure**: Model performs at or below random in live play despite strong offline metrics
3. **Diagnosis**: Systematic isolation of policy vs value head reveals distribution shift as root cause
4. **Pivot**: Replace DL evaluation with domain-knowledge heuristic ensemble
5. **Success**: Ensemble MCTS outperforms all other approaches
6. **Insight**: DL succeeds at prediction (team preview, opponent modeling) but fails at evaluation with limited data
7. **Validation**: Live testing against human players on Pokemon Showdown ladder

## Chapter Structure

### Chapter 1: Background & Related Work
- VGC format rules and complexity
- MCTS fundamentals (PUCT, selection, expansion, evaluation, backpropagation)
- AlphaZero paradigm and behavioral cloning
- Prior work in Pokemon AI (if any)
- Imperfect information games

### Chapter 2: System Architecture
- Battle simulator (C# port of Showdown — 6000 battles/sec, 16 threads)
- Data pipeline (scraper → parser → dataset → training → ONNX → inference)
- Information model (OTS/CTS, revealed info tracking)
- Experiment framework (Optuna hparam search, ablation, multi-seed, rating tiers)

### Chapter 3: Deep Learning Approach
- **BattleNet architecture**: embeddings, trunk, value + policy heads
- **TeamPreviewNet architecture**: species embeddings → config classification
- **Training**: behavioral cloning from 300K human replays, 780K decision points
- **Offline metrics**: value accuracy 72.4%, policy top-1 61.4%, ECE 0.011
- **Ablation**: species-only → +moves → +abilities → +items → +tera (moves provide 95% of signal)

### Chapter 4: The DL Failure — Diagnosis and Analysis

**This is the most academically valuable chapter.** Honest negative results with rigorous analysis.

#### 4.1 DL-Greedy Results
| Configuration | Win Rate vs Random |
|---|---|
| DL-Greedy (original) | 49.06% (30K battles) |
| DL-Greedy (BCEWithLogitsLoss + value weight 2.0) | 46.35% |
| DL-Greedy (BCEWithLogitsLoss only) | 44.98% |
| DL-Greedy (+ 20 matchup features) | 41.2% |

Key finding: every attempted improvement made things worse.

#### 4.2 MCTS Integration Results
| Configuration | vs Random | vs Standalone |
|---|---|---|
| MCTS+DL (policy + value) | 55.0% | 38.8% |
| Hybrid (uniform + value only) | 62.0% | 43.0% |
| Hybrid + confidence blend | 67.5% | 47.0% |
| Standalone (uniform + heuristic) | — | 50.0% |

Key finding: removing the policy head helped (+7%), but value head still worse than heuristic.

#### 4.3 Root Cause Analysis
- **Compounding errors**: 61% per-decision accuracy → 0.003% chance of all-correct game
- **Distribution shift**: trained on human states, evaluated on MCTS-explored states
- **Insufficient data**: 780K samples insufficient for implicit game mechanics learning
- **More features ≠ better**: adding domain knowledge features degraded performance

### Chapter 5: Heuristic Approach

#### 5.1 Improved Heuristic Evaluation
- HP/alive score + type matchup + status conditions + stat boosts
- `sigmoid(3 × weighted_diff)` formula
- Standalone MCTS: 55% → 73.5% vs random (with improved heuristic)

#### 5.2 Mini-Model Ensemble
- Architecture: 9 focused heuristic models, each returns preferences + confidence
- Weighted aggregation: `Σ(weight × confidence × preference) / Σ(weight × confidence)`
- Ensemble only affects root priors — leaf evaluation uses proven heuristic
- Models: DamageMax, KOSeeking, KOAvoidance, TypePositioning, DamageMin, SpeedAdvantage, StatusSpreading, ProtectPrediction, SwitchMomentum

#### 5.3 Weight Tuning
- Bayesian optimization via Optuna (200 trials, 50 games/trial)
- No training data needed — optimizes directly on game outcomes
- ~30-45 minutes to tune (vs overnight for DL training)
- Final weights reveal VGC strategy priorities

#### 5.4 Rollout Depth Experiment
- K=0: pure heuristic evaluation (fastest, most iterations)
- K=1,2,3: partial rollout + heuristic (fewer iterations, deeper lookahead)
- K=∞: full rollout to completion (fewest iterations, ground truth value)
- Exploits the 6000 battles/sec simulator speed
- **Report as win rate vs rollout depth chart**

### Chapter 6: Evaluation

#### 6.1 Round-Robin Win Rate Matrix

**The centerpiece result.** N×N grid of all player types, each cell = win rate (P1 win %).

Rows/columns (player types):
- Random
- Greedy (max damage)
- MCTS Standalone (uniform priors + heuristic eval)
- MCTS+DL (DL policy + DL value)
- DL-Greedy (policy head only, no search)
- Ensemble MCTS (mini-model priors + heuristic eval)

|  | Random | Greedy | Standalone | MCTS+DL | DL-Greedy | Ensemble |
|---|---|---|---|---|---|---|
| **Random** | 50% | | | | | |
| **Greedy** | | 50% | | | | |
| **Standalone** | | | 50% | | | |
| **MCTS+DL** | | | | 50% | | |
| **DL-Greedy** | | | | | 50% | |
| **Ensemble** | | | | | | 50% |

Each cell: 400 battles, 10K iterations, 32 threads. ±5% confidence interval.

#### 6.2 Iteration Scaling
- Plot win rate vs iteration count (1K, 5K, 10K, 50K, 100K)
- Show ensemble advantage grows with iterations
- Compare ensemble vs standalone scaling curves

#### 6.3 TeamPreviewNet Results
- 95% bring set accuracy (clear DL win)
- Comparison to random (6.7%) and popular (17%) baselines
- This validates DL for prediction tasks

#### 6.4 Opponent Prediction Model Ablation
- 48.4% top-1 accuracy, 75.2% top-3
- Ensemble WITH opponent model: 65% vs random (worse)
- Ensemble WITHOUT opponent model: 88% vs random (better)
- Conclusion: 48% accuracy insufficient, model misdirects ProtectPrediction

#### 6.5 Showdown Ladder Results
- Win rate and ELO progression over N ladder games
- Opponent ELO distribution
- Turn count distribution
- Decision time distribution (iterations per turn)
- Notable games analysis (switching loops, good plays, errors)

### Chapter 7: Discussion

#### 7.1 Why DL Failed for Evaluation
- Distribution shift is the fundamental barrier for behavioral cloning in game search
- AlphaZero solves this with self-play — requires billions of games and massive compute
- With 300K replays, the model hits a capacity ceiling
- Offline metrics are misleading — 72% accuracy ≠ good play

#### 7.2 Why Heuristics Succeeded
- Domain knowledge is always correct (type chart, damage formula, speed tiers)
- No distribution shift — heuristics work on any game state
- Modular ensemble allows focused optimization per strategic dimension
- Weight tuning via game outcomes is fast and effective

#### 7.3 The Right Role for DL
- DL excels at prediction (team preview, opponent modeling) — no distribution shift
- DL fails at evaluation (value head) — distribution shift from MCTS exploration
- Separation principle: DL for prediction, heuristics for evaluation, MCTS for search

#### 7.4 Simulator Speed as Strategic Advantage
- 6000 battles/sec enables full rollouts that other approaches can't afford
- Full rollouts provide unbiased value estimates (ground truth from game outcomes)
- Combined with ensemble priors: best of both worlds

#### 7.5 Limitations
- Full observability in bot-vs-bot evaluation (information restriction exists but not fully tested)
- Single regulation tested (Reg I)
- Team composition fixed (not optimized)
- Switching loop behavior in live play

### Chapter 8: Conclusion & Future Work
- Summary of contributions
- Self-play as natural next step
- Information Set MCTS for imperfect information
- Adaptive time management for Showdown
- Multi-regulation generalization

## Data Collection Checklist

### Already collected:
- [x] BattleNet offline metrics (multi-seed, ablation, baselines)
- [x] TeamPreviewNet offline metrics
- [x] DL-Greedy vs Random (30K battles, multiple configurations)
- [x] MCTS+DL vs Random/Standalone
- [x] Hybrid and confidence-blend experiments
- [x] Heuristic improvement results
- [x] Ensemble vs Random (400 battles, 10K iterations)
- [x] Ensemble vs Standalone (400 battles, 10K iterations)
- [x] Ensemble vs Greedy (400 battles, 10K iterations)
- [x] Standalone vs Random baseline (400 battles, 10K iterations)
- [x] Opponent model ablation
- [x] Weight tuning results (multiple rounds)
- [x] Initial Showdown ladder results (small sample)

### Still needed:
- [ ] **Full round-robin matrix** (all pairs, 400 battles each)
- [ ] **Rollout depth experiment** (K=0,1,2,3,5,∞ at fixed iteration count)
- [ ] **Iteration scaling experiment** (1K, 5K, 10K, 50K, 100K)
- [ ] **Showdown ladder results** (100+ games with final ELO)
- [ ] **CTS evaluation** (with information tracker enabled)

### Figures needed:
- [ ] Round-robin heatmap
- [ ] Win rate vs rollout depth
- [ ] Win rate vs iteration count (ensemble vs standalone)
- [ ] ELO progression over ladder games
- [ ] Timer budget distribution per turn
- [ ] Mini-model weight bar chart
- [ ] DL failure progression chart (49% → 46% → 45% → 41%)
- [ ] Offline vs live performance comparison (BattleNet metrics vs actual win rate)
