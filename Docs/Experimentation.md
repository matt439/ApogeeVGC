# Experimentation Framework

This document describes the ML experimentation pipeline for evaluating and tuning the Team Preview model. The framework provides rigorous methodology for hyperparameter optimisation, feature ablation, baseline comparison, and statistical evaluation.

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
python -m experiments.run_all --regulation gen9vgc2025regi

# Run specific stages only
python -m experiments.run_all --regulation gen9vgc2025regi --stages hparam ablation

# Quick test run (reduced compute)
python -m experiments.run_all --regulation gen9vgc2025regi --n-trials 3 --epochs 5

# Custom data split ratios
python -m experiments.run_all --regulation gen9vgc2025regi --train-frac 0.7 --val-frac 0.15 --test-frac 0.15
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
| `--min-rating` | 0 | Minimum player Elo to include games |
| `--epochs` | 50 | Max training epochs per run |
| `--patience` | 7 | Early stopping patience |
| `--train-frac` | 0.7 | Training set fraction |
| `--val-frac` | 0.15 | Validation set fraction |
| `--test-frac` | 0.15 | Test set fraction |

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

**Output:** `results/<regulation>/hparam_search/best_config.json`

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

**Output:** `results/<regulation>/ablation/summary.json`

### Stage 3: Baselines (`baselines`)

Evaluates two non-learned baselines on the test set for comparison:

1. **Random** — Uniformly random bring/lead selection, averaged over 100 trials to reduce variance
2. **Most-popular** — Computes the slot-wise marginal probability of each position being brought/led from the training set, and uses those as constant predictions

**Output:** `results/<regulation>/baselines/random_metrics.json`, `popular_metrics.json`

### Stage 4: Multi-Seed Evaluation (`multiseed`)

Trains the best configuration across 5 random seeds (42, 123, 456, 789, 1024) and evaluates each on the test set. Reports mean +/- std for all metrics.

This demonstrates that results are not an artefact of a particular random initialisation or data ordering.

**Output:** `results/<regulation>/multiseed/summary.json`

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

**Output:** `results/<regulation>/figures/`

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
| `experiments/run_all.py` | CLI orchestrator, `python -m experiments.run_all` |
| `experiments/utils.py` | `timer()`, `save_json()`, `load_json()` |

## Dependencies

Added to `Tools/DLModel/requirements.txt`:

```
optuna>=3.0      # Bayesian hyperparameter optimisation
matplotlib>=3.5  # Thesis-quality figure generation
pandas           # Trial data analysis and CSV export
```

These are in addition to the existing `torch>=2.0` and `numpy`.
