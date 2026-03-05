# Experimentation Framework

This document describes the ML experimentation pipelines for evaluating and tuning the **Team Preview** and **BattleNet** models. The framework provides rigorous methodology for hyperparameter optimisation, feature ablation, baseline comparison, and statistical evaluation.

**Location:** `Tools/DLModel/experiments/`

## Motivation

Academic research requires empirical justification for architectural decisions. The experiment framework answers:

- **Are the hyperparameters optimal?** — Bayesian search over 10 parameters
- **Does each feature group contribute?** — Cumulative ablation study
- **How does the model compare to baselines?** — Random and popularity baselines
- **Are the results stable?** — Multi-seed evaluation with mean +/- std
- **Are the predictions calibrated?** — Expected Calibration Error and reliability diagrams

## Quick Start

```bash
cd Tools/DLModel

# Full pipeline for a regulation
python -m experiments.preview_run_all --regulation gen9vgc2025regi

# Run specific stages only
python -m experiments.preview_run_all --regulation gen9vgc2025regi --stages hparam ablation

# Quick test run (reduced compute)
python -m experiments.preview_run_all --regulation gen9vgc2025regi --n-trials 3 --epochs 5

# Custom data split ratios
python -m experiments.preview_run_all --regulation gen9vgc2025regi --train-frac 0.7 --val-frac 0.15 --test-frac 0.15
```

### CLI Reference

| Flag | Default | Description |
|------|---------|-------------|
| `--regulation` | (required) | Regulation name, e.g. `gen9vgc2025regi` |
| `--stages` | all | Space-separated list: `hparam ablation baselines multiseed figures` |
| `--data-root` | `../ReplayScraper/data` | Root directory containing regulation subdirectories |
| `--results-root` | `results` | Where to write experiment outputs |
| `--n-trials` | 100 | Number of Optuna hyperparameter search trials |
| `--timeout-hours` | 12.0 | Wall-clock limit for hyperparameter search |
| `--epochs` | 50 | Max training epochs per run |
| `--patience` | 7 | Early stopping patience |
| `--train-frac` | 0.7 | Training set fraction |
| `--val-frac` | 0.15 | Validation set fraction |
| `--test-frac` | 0.15 | Test set fraction |
| `--training-strategy` | `winners_only` | `winners_only` trains on winning player's choices only; `all_games` trains on both perspectives |
| `--tiers` | all tiers | Space-separated list of rating tiers to run: `all 1200+ 1500+` |

## Rating Tier Comparison

The pipeline automatically runs across three rating tiers for rigorous comparison:

| Tier | Filter | Description |
|------|--------|-------------|
| `all` | No filter | All parsed games regardless of player rating |
| `1200+` | Both players >= 1200 | Games between intermediate+ players |
| `1500+` | Both players >= 1500 | Games between experienced players |

The parser (`Tools/ReplayScraper/parser.py`) generates three pre-filtered data files when run: `parsed.jsonl`, `parsed_1200.jsonl`, and `parsed_1500.jsonl`. Each tier receives its own hyperparameter search, ablation study, baselines, multi-seed evaluation, and figures.

After all tiers complete, a cross-tier statistical comparison is generated with:
- Welch's t-test between each pair of tiers for all key metrics
- Cohen's d effect sizes with magnitude classification
- Grouped bar charts with significance brackets
- Box plots showing metric distributions per tier

**Output:** `results/<regulation>/rating_comparison/` (preview) or `results/<regulation>/battle/rating_comparison/` (battle)

## Training Strategy

By default, the model trains **only on the winning player's perspective** (`--training-strategy winners_only`). The rationale: the model performs behavioural cloning — it learns to replicate human team selection decisions. Training on losing players' choices teaches the model to imitate decisions that led to losses, diluting the signal from successful play.

The `all_games` option (both perspectives) is available for experimental comparison but is not recommended as a default.

## Pipeline Stages

The pipeline runs five stages sequentially. Each stage can be run independently.

### Stage 1: Hyperparameter Search (`hparam`)

Uses [Optuna](https://optuna.org/) with Tree-structured Parzen Estimator (TPE) sampling for Bayesian optimisation. The search minimises validation loss.

**Search space:**

| Parameter | Range | Type | Description |
|-----------|-------|------|-------------|
| `species_embed_dim` | {16, 32, 48, 64, 96} | categorical | Species embedding dimension |
| `feat_embed_dim` | {8, 16, 24, 32} | categorical | Move/ability/item/tera embedding dimension |
| `pokemon_dim` | {32, 48, 64, 96, 128} | categorical | Per-Pokemon encoder output dimension |
| `hidden_dim` | {128, 192, 256, 384, 512} | categorical | Trunk hidden layer width |
| `num_trunk_layers` | [2, 5] | integer | Number of trunk MLP blocks |
| `trunk_dropout` | [0.1, 0.5] | float | Dropout rate in trunk |
| `head_dim` | {32, 64, 96, 128} | categorical | Output head intermediate dimension |
| `lr` | [1e-4, 5e-3] | log-uniform | Learning rate |
| `weight_decay` | [1e-6, 1e-3] | log-uniform | L2 regularisation |
| `batch_size` | {256, 512, 1024, 2048} | categorical | Training batch size |

**Pruning:** Median pruner kills unpromising trials early — if a trial's validation loss at epoch *e* is worse than the median of all completed trials at epoch *e*, it is pruned. The first 10 trials run to completion (startup), and pruning decisions begin after 5 epochs (warmup).

**Storage:** Results are stored in an SQLite database (`optuna_study.db`) so the search can be resumed if interrupted. All trials are also exported to `trials.csv` for analysis.

**Output:** `results/<regulation>/<tier>/hparam_search/best_config.json`

### Stage 2: Feature Ablation (`ablation`)

Trains the model with progressively more feature groups enabled, using the best hyperparameters from Stage 1. This quantifies the contribution of each feature embedding.

| Variant | Species | Moves | Abilities | Items | Tera |
|---------|---------|-------|-----------|-------|------|
| `species_only` | yes | | | | |
| `species_moves` | yes | yes | | | |
| `species_moves_abilities` | yes | yes | yes | | |
| `species_moves_abilities_items` | yes | yes | yes | yes | |
| `full` | yes | yes | yes | yes | yes |

Each variant is evaluated on the held-out test set with comprehensive metrics.

**Output:** `results/<regulation>/<tier>/ablation/summary.json`

### Stage 3: Baselines (`baselines`)

Evaluates two non-learned baselines on the test set for comparison:

1. **Random** — Uniformly random bring/lead selection, averaged over 100 trials to reduce variance
2. **Most-popular** — Computes the slot-wise marginal probability of each position being brought/led from the training set, and uses those as constant predictions

**Output:** `results/<regulation>/<tier>/baselines/random_metrics.json`, `popular_metrics.json`

### Stage 4: Multi-Seed Evaluation (`multiseed`)

Trains the best configuration across 5 random seeds (42, 123, 456, 789, 1024) and evaluates each on the test set. Reports mean +/- std for all metrics.

This demonstrates that results are not an artefact of a particular random initialisation or data ordering.

**Output:** `results/<regulation>/<tier>/multiseed/summary.json`

### Stage 5: Figures (`figures`)

Generates thesis-quality visualisations as both PDF (for LaTeX) and PNG:

| Figure | Source Data | Description |
|--------|------------|-------------|
| `learning_curves` | multiseed training logs | Train/val loss and accuracy over epochs, with mean +/- std band across seeds |
| `hparam_sensitivity` | Optuna trials.csv | Scatter plots of validation loss vs each hyperparameter |
| `ablation_bars` | ablation summary | Grouped bar chart of bring/lead accuracy per feature configuration |
| `calibration` | multiseed test metrics | Reliability diagram showing predicted probability vs observed frequency |
| `baseline_comparison` | baselines + multiseed | Bar chart comparing model vs baselines |
| `multiseed_distribution` | multiseed summary | Box plot of metric distribution across seeds |

**Output:** `results/<regulation>/<tier>/figures/`

## Data Management

### Three-Way Split

Games are split 70/15/15 into train/validation/test sets. The split is performed at the **game level**, not the sample level — both player perspectives of the same game always land in the same split. This prevents data leakage (the model cannot learn from the opponent's perspective of a game it will be tested on).

Split indices are persisted to `results/<regulation>/splits/` so every experiment uses identical data.

### Vocabulary

The vocabulary (species, moves, abilities, items, tera type mappings) is built from the full dataset and cached at `results/<regulation>/vocab.json`. All experiments for a regulation share the same vocabulary.

### Multi-Regulation Support

Each regulation is fully independent — separate data, vocabulary, splits, and results:

```
results/
  gen9vgc2025regi/    # Regulation I (current)
    splits/
    hparam_search/
    ablation/
    baselines/
    multiseed/
    figures/
  gen9vgc2024regf/    # Regulation F (when data is available)
    ...
```

To train a model for a different regulation, point the scraper/parser at that format's replays, then run the pipeline with `--regulation <name>`.

## Evaluation Metrics

The framework computes comprehensive metrics beyond the original top-k set accuracy:

### Set-Level Metrics

| Metric | Description |
|--------|-------------|
| **Set accuracy** | Exact match: predicted top-k set == actual top-k set (order-independent) |
| **Overlap accuracy** | \|predicted set &cap; actual set\| / k — softer than exact match |
| **Hamming accuracy** | Per-slot binary correctness (threshold at 0.5) |

### Per-Slot Metrics

| Metric | Description |
|--------|-------------|
| **Precision** | Of the Pokemon the model predicts to bring/lead in slot *i*, what fraction are correct? |
| **Recall** | Of the Pokemon that should be brought/led in slot *i*, what fraction does the model identify? |
| **F1** | Harmonic mean of precision and recall |
| **Macro F1** | Average F1 across all 6 slots |

### Calibration Metrics

| Metric | Description |
|--------|-------------|
| **ECE** | Expected Calibration Error — weighted average of \|accuracy - confidence\| across probability bins |
| **Reliability diagram** | Plots predicted probability vs observed frequency to visualise calibration |

Calibration matters because the sigmoid outputs are used as probabilities downstream. If the model outputs 0.8 for a slot, that slot should actually be brought ~80% of the time.

## Model Architecture (V2)

The experiment framework uses `TeamPreviewNetV2`, an extension of the original `TeamPreviewNet` that parameterises:

- **Trunk depth** (`num_trunk_layers`) — variable number of Linear + BatchNorm + ReLU + Dropout blocks
- **Dropout rate** (`trunk_dropout`) — uniform across trunk layers (final layer uses 67% of this rate)
- **Head dimension** (`head_dim`) — intermediate dimension in bring/lead output heads
- **Feature flags** (`feature_flags`) — dict controlling which embedding groups are active, enabling clean ablation without masking

The original `TeamPreviewNet` class is untouched for backward compatibility. Existing checkpoints and `train_team_preview.py` continue to work.

### ONNX Export

`export_onnx.py` detects `model_version: 2` in experiment checkpoints and instantiates `TeamPreviewNetV2` accordingly. The ONNX interface (input/output tensor names and shapes) is identical to V1, so no C# changes are needed.

## Results Directory Structure

```
results/<regulation>/
  splits/
    train_indices.json       # Persisted game-level split indices
    val_indices.json
    test_indices.json
  vocab.json                 # Cached vocabulary
  hparam_search/
    optuna_study.db          # SQLite database (resumable)
    trials.csv               # All trials in tabular format
    best_config.json         # Best hyperparameter configuration
    trial_0/                 # Per-trial checkpoint and config
    trial_1/
    ...
  ablation/
    species_only/
      model.pt               # Best checkpoint for this variant
      metrics.json            # Comprehensive test metrics
    species_moves/
    species_moves_abilities/
    species_moves_abilities_items/
    full/
    summary.json             # Combined results across all variants
  baselines/
    random_metrics.json
    popular_metrics.json
  multiseed/
    seed_42/
      model.pt
      test_metrics.json
      training_log.json      # Per-epoch metrics for learning curves
    seed_123/
    seed_456/
    seed_789/
    seed_1024/
    summary.json             # Mean +/- std across all seeds
  figures/
    learning_curves.pdf/.png
    hparam_sensitivity.pdf/.png
    ablation_bars.pdf/.png
    calibration.pdf/.png
    baseline_comparison.pdf/.png
    multiseed_distribution.pdf/.png
```

## Module Reference

| Module | Purpose |
|--------|---------|
| `experiments/config.py` | Dataclasses (`ModelConfig`, `TrainConfig`, `DataConfig`, `ExperimentConfig`), search space, ablation configs, seed list |
| `experiments/data.py` | Game loading, persistent train/val/test splitting, vocabulary caching, DataLoader construction |
| `experiments/training.py` | Callable training function returning `TrainResult` with per-epoch `EpochMetrics`; supports Optuna pruning |
| `experiments/metrics.py` | `evaluate_comprehensive()` → `ComprehensiveMetrics` dataclass with all metrics |
| `experiments/baselines.py` | `evaluate_random_baseline()`, `evaluate_popular_baseline()` |
| `experiments/hparam_search.py` | `run_hparam_search()` → Optuna study with TPE + median pruner |
| `experiments/ablation.py` | `run_ablation()` → trains all feature-flag variants |
| `experiments/multiseed.py` | `run_multiseed()` → trains across seeds, computes mean +/- std |
| `experiments/visualise.py` | `generate_all_figures()` → PDF + PNG plots |
| `experiments/preview_run_all.py` | CLI orchestrator, `python -m experiments.preview_run_all` |
| `experiments/utils.py` | `timer()`, `save_json()`, `load_json()` |

## Dependencies

Added to `Tools/DLModel/requirements.txt`:

```
optuna>=3.0      # Bayesian hyperparameter optimisation
matplotlib>=3.5  # Thesis-quality figure generation
pandas           # Trial data analysis and CSV export
```

These are in addition to the existing `torch>=2.0` and `numpy`.

---

# BattleNet Experimentation Framework

The BattleNet pipeline mirrors the Team Preview framework above, adapted for BattleNet's distinct architecture: 3 output heads (value + 2 policy), per-turn samples, and 200 numeric features.

**Location:** `Tools/DLModel/experiments/battle_*.py`

## Key Differences from Team Preview

| Aspect | Team Preview | BattleNet |
|--------|-------------|-----------|
| **Output heads** | 2 (bring, lead) — sigmoid | 3 (value, policy_a, policy_b) — sigmoid + softmax |
| **Loss** | BCELoss (bring + lead) | BCELoss (value) + CrossEntropyLoss (policy_a, policy_b) |
| **Samples per game** | 1 per game (team preview only) | 1 per turn per perspective |
| **Input features** | 12 pokemon × embeddings | 8 pokemon × embeddings + 200 numeric features |
| **Default training strategy** | `winners_only` | `all_games` |
| **Metrics** | Set/overlap/hamming accuracy, per-slot F1 | Value accuracy, policy top-1/top-3 accuracy |

## Training Strategy

BattleNet defaults to **`all_games`** (both perspectives) rather than `winners_only`. The rationale: BattleNet's value head predicts win probability. Training only on winners makes every value target 1.0, which is useless for learning win prediction. The model needs both winning and losing examples.

The `winners_only` option is available for experimental comparison via `--training-strategy winners_only`.

## Quick Start

```bash
cd Tools/DLModel

# Full pipeline
python -m experiments.battle_run_all --regulation gen9vgc2025regi

# Run specific stages
python -m experiments.battle_run_all --regulation gen9vgc2025regi --stages hparam ablation

# Quick test run
python -m experiments.battle_run_all --regulation gen9vgc2025regi --n-trials 3 --epochs 5
```

### CLI Reference

Same flags as Team Preview (`--regulation`, `--stages`, `--data-root`, `--results-root`, `--n-trials`, `--timeout-hours`, `--epochs`, `--patience`, `--train-frac`, `--val-frac`, `--test-frac`, `--training-strategy`, `--tiers`), with the key difference that `--training-strategy` defaults to `all_games`. The same rating tier comparison (all, 1200+, 1500+) is performed automatically.

## Pipeline Stages

### Stage 1: Hyperparameter Search (`hparam`)

Same Optuna TPE + median pruner approach as Team Preview. Search space:

| Parameter | Range | Type |
|-----------|-------|------|
| `embed_dim` | {16, 32, 48, 64} | categorical |
| `feat_embed_dim` | {8, 16, 24, 32} | categorical |
| `pokemon_dim` | {32, 48, 64, 96} | categorical |
| `hidden_dim` | {128, 192, 256, 384, 512} | categorical |
| `num_trunk_layers` | [2, 5] | integer |
| `trunk_dropout` | [0.1, 0.5] | float |
| `head_dim` | {32, 64, 96, 128} | categorical |
| `lr` | [1e-4, 5e-3] | log-uniform |
| `weight_decay` | [1e-6, 1e-3] | log-uniform |
| `batch_size` | {256, 512, 1024} | categorical |

Note: batch size is capped at 1024 (no 2048) due to larger per-sample memory from 200 numeric features.

**Output:** `results/<regulation>/battle/<tier>/hparam_search/best_config.json`

### Stage 2: Feature Ablation (`ablation`)

Same cumulative feature ablation as Team Preview (species only → full). Each variant uses the best hyperparameters with only `feature_flags` changed.

**Output:** `results/<regulation>/battle/<tier>/ablation/summary.json`

### Stage 3: Baselines (`baselines`)

Two non-learned baselines:

1. **Random** — Value = 0.5 (coin flip), policy = uniform over actions, averaged over 100 trials
2. **Most-popular** — Value = training set win rate, policy = slot-wise action frequency from training set

**Output:** `results/<regulation>/battle/<tier>/baselines/random_metrics.json`, `popular_metrics.json`

### Stage 4: Multi-Seed Evaluation (`multiseed`)

Same 5-seed evaluation (42, 123, 456, 789, 1024) as Team Preview.

**Output:** `results/<regulation>/battle/<tier>/multiseed/summary.json`

### Stage 5: Figures (`figures`)

| Figure | Description |
|--------|-------------|
| `learning_curves` | 3-panel: loss, value accuracy, policy accuracy with mean +/- std bands across seeds |
| `hparam_sensitivity` | Scatter plots from trials.csv |
| `ablation_bars` | Grouped bars: value accuracy, policy A top-1, policy B top-1 |
| `calibration` | Reliability diagram for value predictions |
| `baseline_comparison` | Model vs baselines on value accuracy and policy top-1 |
| `multiseed_distribution` | Box plot of value/policy accuracies across seeds |

**Output:** `results/<regulation>/battle/<tier>/figures/`

## Evaluation Metrics

### Value Head

| Metric | Description |
|--------|-------------|
| **Value accuracy** | Binary accuracy (threshold 0.5): did the model correctly predict win/loss? |
| **Value ECE** | Expected Calibration Error for the value prediction |
| **Reliability diagram** | Predicted win probability vs observed win rate |

### Policy Heads

| Metric | Description |
|--------|-------------|
| **Top-1 accuracy** | Did the model's top-predicted action match the actual action? (per head: A, B, combined) |
| **Top-3 accuracy** | Was the actual action in the model's top-3 predictions? (per head: A, B) |

Policy metrics only count non-padded targets (where target > 0) to avoid inflating accuracy on padding.

## Model Architecture (BattleNetV2)

`BattleNetV2` extends `BattleNet` with parameterised architecture for experimentation:

- **Trunk depth** (`num_trunk_layers`) — variable number of MLP blocks
- **Dropout rate** (`trunk_dropout`) — uniform across trunk; final layer uses 67% of this rate
- **Head dimension** (`head_dim`) — intermediate dimension in value and policy heads
- **Feature flags** (`feature_flags`) — dict controlling which embedding groups are active
- **Slot-conditioned policy** — policy head concatenates trunk output with acting slot's encoding

The original `BattleNet` class is untouched. ONNX export detects `model_version: 2` in checkpoints and instantiates `BattleNetV2` accordingly. The ONNX interface is identical to V1.

## Results Directory Structure

BattleNet results are stored under `battle/` to separate from Team Preview. Vocab and splits are shared at the regulation level.

```
results/<regulation>/
  splits/                        # Shared: same splits for both models
  vocab.json                     # Shared: same vocabulary
  hparam_search/                 # Team Preview results
  ablation/
  baselines/
  multiseed/
  figures/
  battle/                        # BattleNet results
    hparam_search/
      optuna_study.db
      trials.csv
      best_config.json
    ablation/
      species_only/
      species_moves/
      species_moves_abilities/
      species_moves_abilities_items/
      full/
      summary.json
    baselines/
      random_metrics.json
      popular_metrics.json
    multiseed/
      seed_42/
      seed_123/
      seed_456/
      seed_789/
      seed_1024/
      summary.json
    figures/
```

## BattleNet Module Reference

| Module | Purpose |
|--------|---------|
| `experiments/battle_config.py` | `BattleModelConfig`, `BattleExperimentConfig`, search space, ablation configs |
| `experiments/battle_training.py` | Training loop with value + 2 policy losses; `BattleTrainResult` |
| `experiments/battle_metrics.py` | `evaluate_battle_comprehensive()` → `BattleComprehensiveMetrics` |
| `experiments/battle_baselines.py` | `evaluate_random_battle_baseline()`, `evaluate_popular_battle_baseline()` |
| `experiments/battle_hparam_search.py` | Optuna search for BattleNet |
| `experiments/battle_ablation.py` | Feature ablation for BattleNet |
| `experiments/battle_multiseed.py` | Multi-seed evaluation for BattleNet |
| `experiments/battle_visualise.py` | BattleNet thesis-quality plots |
| `experiments/battle_run_all.py` | CLI: `python -m experiments.battle_run_all` |
