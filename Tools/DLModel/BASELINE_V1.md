# BattleNet Baseline (V1) — Pre-Phase-1 Training Improvements

This document records the architecture, training configuration, and performance of the
BattleNet model **before** Phase 1 training improvements (label smoothing, value smoothing,
loss weighting, LR warmup). It serves as the comparison baseline for evaluating those changes.

---

## Architecture: BattleNetV2

### Input Representation (per sample)

| Tensor | Shape | Description |
|--------|-------|-------------|
| `species_ids` | [8] | Species embedding indices (pad=0, unknown=1) |
| `move_ids` | [8, 4] | Move embedding indices per pokemon (summed) |
| `ability_ids` | [8] | Ability embedding indices |
| `item_ids` | [8] | Item embedding indices |
| `tera_ids` | [8] | Tera type embedding indices |
| `numeric` | [200] | Battle state features (HP, status, boosts, tera, field) |

Slot layout: [my_active_a, my_active_b, opp_active_a, opp_active_b, my_bench_0, my_bench_1, opp_bench_0, opp_bench_1]

Opponent slots: ability/item/move embeddings are always 0 (pad) to match progressive revelation in training data.

### Embedding Layers (all use padding_idx=0)

| Embedding | Vocab Size | Dimension |
|-----------|-----------|-----------|
| Species | 970 | embed_dim |
| Moves | 698 | feat_embed_dim (summed over 4 moves) |
| Abilities | 192 | feat_embed_dim |
| Items | 174 | feat_embed_dim |
| Tera Types | 21 | feat_embed_dim |

### Pokemon Encoder (shared across 8 slots)

```
Linear(embed_dim + 4*feat_embed_dim, pokemon_dim) -> ReLU
```
Single layer. Output: [B, 8, pokemon_dim]

### Trunk

Input: flatten 8 pokemon encodings + 200 numeric features = [B, 8*pokemon_dim + 200]

```
For each layer (num_trunk_layers):
  Linear(in_dim, hidden_dim) -> BatchNorm1d -> ReLU -> Dropout(trunk_dropout)
  (final layer: hidden_dim//2, dropout * 0.67)
```

No residual connections. No LayerNorm option.

### Value Head

```
Linear(hidden_dim//2, head_dim) -> ReLU -> Linear(head_dim, 1) -> Sigmoid
```
Output: [B] win probability in [0, 1]

### Policy Head (slot-conditioned, shared weights for both slots)

```
Input: cat(trunk_output, slot_pokemon_encoding)  # [B, hidden_dim//2 + pokemon_dim]
Linear(hidden_dim//2 + pokemon_dim, head_dim) -> ReLU -> Linear(head_dim, num_actions)
```
Output: [B, 1578] raw logits (masked softmax applied at inference)

---

## Training Configuration

| Parameter | Value |
|-----------|-------|
| Optimizer | AdamW |
| Weight Decay | 1e-5 (fixed) |
| LR Schedule | CosineAnnealingLR (no warmup) |
| Gradient Clipping | max_norm=1.0 |
| Early Stopping | patience epochs |
| AMP | Enabled on CUDA |
| Data Split | 70/15/15 (train/val/test) at game level |

### Loss Function

```
loss = BCELoss(value_pred, value_target)
     + CrossEntropyLoss(policy_a_logits, policy_a_target, ignore_index=0)
     + CrossEntropyLoss(policy_b_logits, policy_b_target, ignore_index=0)
```

- **No label smoothing** on policy loss
- **No loss weighting** — value and policy losses summed equally
- **Hard value targets** — 1.0 (winner) or 0.0 (loser) for every turn regardless of game progress
- Policy loss (~2.77) dominates value loss (~0.61), so ~82% of gradient signal goes to policy

### Value Targets

Binary: 1.0 if perspective player wins the game, 0.0 otherwise.
Applied identically to every turn (turn 1 and final turn get the same target).

---

## Trained Models

### All-Tier Model (was deployed as ONNX)

| Parameter | Value |
|-----------|-------|
| Training Data | all ratings (304,750 games) |
| embed_dim | 64 |
| feat_embed_dim | 32 |
| pokemon_dim | 48 |
| hidden_dim | 512 |
| num_trunk_layers | 2 |
| trunk_dropout | 0.231 |
| head_dim | 128 |
| Total Parameters | 814,075 |
| Best Epoch | 50 |
| Val Loss | 2.533 |

**Test Metrics (multiseed mean +/- std, 5 seeds):**

| Metric | Value |
|--------|-------|
| Value Accuracy | 72.9% +/- 0.1% |
| Value ECE | 0.015 +/- 0.002 |
| Policy Combined Top-1 | 61.5% +/- 0.04% |
| Policy A Top-3 | 93.0% +/- 0.03% |
| Total Loss | 2.529 +/- 0.004 |
| Value Loss | 0.517 +/- 0.001 |
| Policy A Loss | 1.000 +/- 0.001 |

### 1200+ Tier Model

| Parameter | Value |
|-----------|-------|
| Training Data | 1200+ rated games (11,022 games) |
| embed_dim | 32 |
| feat_embed_dim | 24 |
| pokemon_dim | 32 |
| hidden_dim | 192 |
| num_trunk_layers | 2 |
| trunk_dropout | 0.449 |
| head_dim | 128 |
| Total Parameters | 400,675 |
| Best Epoch | 46 |
| Val Loss | 3.444 |

**Test Metrics (multiseed mean +/- std, 5 seeds):**

| Metric | Value |
|--------|-------|
| Value Accuracy | 67.3% +/- 0.4% |
| Value ECE | 0.077 +/- 0.006 |
| Policy Combined Top-1 | 52.4% +/- 0.2% |
| Policy A Top-3 | 88.2% +/- 0.1% |
| Total Loss | 3.387 +/- 0.011 |
| Value Loss | 0.612 +/- 0.005 |
| Policy A Loss | 1.370 +/- 0.005 |

### Baselines (1200+ tier)

| Baseline | Value Accuracy | Policy Top-1 |
|----------|---------------|--------------|
| Random | 50.0% | 0.07% |
| Most Popular | 50.0% | 12.5% |
| **Model (full)** | **67.3%** | **52.4%** |

### Feature Ablation (1200+ tier, test set)

| Configuration | Policy Top-1 | Value Accuracy | Total Loss |
|--------------|-------------|---------------|------------|
| Species only | 37.9% | 64.2% | 4.741 |
| + Moves | 52.1% | 67.0% | 3.378 |
| + Abilities | 52.4% | 66.9% | 3.387 |
| + Items | 52.1% | 68.4% | 3.387 |
| + Tera (full) | 52.4% | 68.0% | 3.372 |

Moves are the dominant feature (+14.2% policy). Abilities, items, tera contribute <0.3% each.

---

## In-Game Performance (DL-Greedy vs Random, 1000 battles)

The DL-Greedy player uses argmax of the masked policy softmax each turn (no search).

### All-Tier Model

| Metric | Value |
|--------|-------|
| **DL-Greedy Win Rate** | **42.9%** |
| Random Win Rate | 57.1% |
| Mean Turns | 26.6 |
| Value Head Mean | 0.631 |
| Value Head StdDev | 0.319 |
| Predictions >0.6 | 60.8% |

### 1200+ Tier Model

| Metric | Value |
|--------|-------|
| **DL-Greedy Win Rate** | **42.3%** |
| Random Win Rate | 57.7% |
| Mean Turns | 28.9 |
| Value Head Mean | 0.805 |
| Value Head StdDev | 0.252 |
| Predictions >0.6 | 83.9% |

### Comparison Players (vs Random, 1000 battles)

| Player | Win Rate | Notes |
|--------|----------|-------|
| MCTS-Standalone (1000 iter) | 69% | Heuristic eval, uniform priors |
| MCTS-DL (1000 iter) | 58% | DL policy priors + value eval |
| **DL-Greedy** | **42-43%** | **Pure model, no search** |
| Random | 50% | Baseline |

---

## Key Observations

1. **DL-Greedy loses to random** — the model's policy is not strong enough for pure greedy play
2. **Value head is severely miscalibrated in-game** — predicts winning (mean 0.63-0.80) while losing (42% win rate)
3. **1200+ model is MORE miscalibrated** than all-tier (ECE 0.077 vs 0.015) despite higher-quality training data
4. **MCTS search compensates** for weak policy (58% with search vs 42% without)
5. **Policy loss dominates training** — value head gets only ~18% of gradient signal
6. **Hard value targets** cause overconfidence — every turn trained as if outcome is certain

## Known Issues

- No label smoothing: model is overconfident in single-action predictions
- No value smoothing: early-turn value predictions are meaningless but trained on hard 0/1
- No loss weighting: policy gradient dominates, value head under-trained
- No LR warmup: potentially unstable early training
- No residual connections: limits effective trunk depth
- BatchNorm instead of LayerNorm: train/eval mode mismatch
