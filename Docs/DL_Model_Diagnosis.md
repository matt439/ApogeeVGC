# DL Model Diagnosis & Improvement Plan

## Current Performance Summary

### BattleNet (gen9vgc2025regi, all tier, 5-seed)

| Metric | Model | Random Baseline | Popular Baseline |
|---|---|---|---|
| Value accuracy | **72.4%** | 50.0% | 50.0% |
| Value ECE | **0.011** | 0.0 | 0.0 |
| Policy top-1 | **61.4%** | 0.06% | 27.2% |
| Policy top-3 | **92.1%** | 0.19% | 35.4% |

**DL-Greedy vs Random (30k battles): 49.06% win rate** — effectively random.

### TeamPreviewNet (gen9vgc2025regi, all tier, 5-seed)

| Metric | Model | Random Baseline | Popular Baseline |
|---|---|---|---|
| Config accuracy | **42.6%** | 1.1% | 4.1% |
| Bring set accuracy | **95.1%** | 6.7% | 17.0% |
| Top-5 config accuracy | **92.5%** | 5.6% | 4.1% |
| Lead set accuracy | **44.6%** | 12.7% | 12.7% |

TeamPreviewNet is performing well. BattleNet is the focus of this document.

### Why DL-Greedy Loses Despite Decent Offline Metrics

DL-Greedy uses **only the policy head** (value is logged but not used for action selection). 61% top-1 accuracy means 39% of decisions are wrong. In a ~15 turn doubles game with ~30 total decisions, errors compound:

- P(all 30 decisions correct) ≈ 0.61^30 ≈ 0.00003
- A single catastrophic choice (bad switch, wasting tera, not Protecting) can lose the game
- This is expected behaviour for behavioral cloning at this accuracy level — MCTS is needed to recover from individual bad decisions

### Ablation Results (what features matter)

| Configuration | Value Acc | Policy Top-1 | Delta |
|---|---|---|---|
| Species only | 68.8% | 47.3% | — |
| +Moves | 71.5% | 62.9% | +2.7 / +15.6 |
| +Abilities | 72.1% | 62.8% | +0.6 / -0.1 |
| +Items | 72.1% | 63.0% | 0.0 / +0.2 |
| +Tera (Full) | 72.3% | 63.0% | +0.2 / 0.0 |

Moves provide almost all the signal. Abilities, items, and tera contribute negligibly — opponent abilities/items are always 0 (can't track per-turn revelation), so the model can only learn from own-side, which is constant across all turns of a game.

### Data Sufficiency

~300k replays → 782,402 decision-point samples. ~1.67M model parameters gives a ~470:1 sample-to-parameter ratio. Multi-seed variance is extremely low (std ~0.1%), confirming the model is not overfitting. Data quantity is not the bottleneck.

---

## Things to Try

### High Impact

#### 1. MCTS+DL vs MCTS-Only Ablation
The real test of whether BattleNet is useful. DL-Greedy is inherently limited by compounding errors. If MCTS+DL beats MCTS with random rollouts, the model is pulling its weight. This is the primary thesis result.

#### 2. Switch to BCEWithLogitsLoss
The current value head applies sigmoid in the forward pass and uses `BCELoss(sigmoid(logits))`. This loses numerical precision — when the linear output is large (>10 or <-10), sigmoid saturates and gradients vanish. Fix:

```python
# In forward(): return raw logits, not sigmoid
value_logits = self.value_head(x).squeeze(-1)

# In training: use nn.BCEWithLogitsLoss (combines sigmoid + BCE, numerically stable)

# At inference only: apply sigmoid for probability output
```

#### 3. Rebalance Loss Weights
Currently total_loss = value_loss + policy_a_loss + policy_b_loss. Value gets 1/3 of gradient signal vs 2/3 for policy. The value loss (~0.52) is dwarfed by policy losses (~1.0 each). Try `value_weight=2.0, policy_weight=1.0` to give the value head more gradient signal, since that's what MCTS primarily uses.

#### 4. Increase Training Epochs
Every ablation configuration hit epoch 49 (the maximum). The models may not have fully converged. Increase to 100+ with patience 15+ to ensure convergence. Check if val loss is still decreasing at epoch 49.

### Medium Impact

#### 5. Turn-Dependent Value Smoothing
The infrastructure exists (`turn_progress` is computed in dataset.py but unused in training). Instead of hard 0/1 value targets, use targets like:

```python
smoothed_target = 0.5 + (raw_target - 0.5) * turn_progress
```

Early turns get targets closer to 0.5 (reflecting genuine uncertainty about who wins), late turns get targets closer to 0/1. This teaches the model appropriate confidence levels throughout the game.

#### 6. Train on Higher-Rating Games Only
The cross-tier experiment framework already supports this. Higher-rated players make more consistent decisions, giving cleaner signal for behavioral cloning. Try training on 1500+ data exclusively and compare.

#### 7. Value-State Correlation Diagnostic
Log `(value_prediction, actual_outcome)` pairs during validation and compute Spearman rank correlation. This tells how useful the value head is for *ranking* states (good vs bad positions), which is exactly what MCTS needs. A positive correlation (even 0.1-0.3) means MCTS can benefit from the value head even if greedy play fails.

### Lower Impact / Exploratory

#### 8. Deeper Value Head
Currently `Linear(128, 64) → ReLU → Linear(64, 1)`. The value head has far fewer parameters than the policy head. Try adding a second hidden layer or increasing to 128 dimensions, since win probability prediction from a battle state is arguably the harder task.

#### 9. Label Smoothing on Policy
The hparam search space includes label smoothing but the default is off. Smoothing policy targets (0.1-0.2) can help generalisation by preventing the model from becoming overconfident on common actions.

#### 10. Action Frequency Weighting
Common actions (Protect, switches to specific Pokemon) dominate the policy distribution. Rare but important actions (clutch tera, unusual tech moves) are underrepresented. Inverse-frequency weighting or focal loss on the policy head could help with tail accuracy.

### Diagnostic Tools to Build

#### 11. Value Prediction vs Turn Number Plot
Group validation predictions by turn number. Plot mean predicted value vs actual win rate per turn bucket. If the model is accurate late-game but random early-game, that explains poor greedy play but is fine for MCTS (which reaches late-game states via search).

#### 12. Policy Confidence Histogram
Plot the max softmax probability when the model makes correct vs incorrect predictions. If it's equally confident on right and wrong answers, the policy head isn't discriminative enough. If it's more confident when correct, MCTS can trust high-confidence predictions.

#### 13. Move-Type Accuracy Breakdown
Compute policy accuracy separately for:
- Offensive moves (attacks)
- Status moves (Will-O-Wisp, Thunder Wave, etc.)
- Protect / Detect
- Switches

If the model is just learning "use the strongest attack", it'll fail against opponents who punish predictable play. This breakdown reveals whether the model has learned strategic diversity.

---

## Thesis Framing

Frame the DL-Greedy result honestly: the model alone performs at random level when used greedily, which is expected for ~61% per-decision accuracy in a sequential decision game. The model's value is as a **heuristic for MCTS**, not as a standalone player.

The offline metrics (72% value accuracy, 61% policy, excellent calibration at ECE 0.011) demonstrate the model learns meaningful patterns from replay data. MCTS provides the search needed to convert imperfect per-turn evaluations into winning play. The MCTS+DL vs MCTS ablation is the primary evidence of the network's contribution.
