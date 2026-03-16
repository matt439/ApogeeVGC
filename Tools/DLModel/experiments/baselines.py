"""
Baseline evaluation strategies for comparison (VGC config classification).

1. Random: uniformly random config selection
2. Most-popular: always pick the most frequently chosen configuration
"""

from __future__ import annotations

from pathlib import Path
import sys

import numpy as np
from torch.utils.data import DataLoader

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from format_spec import FormatSpec, VGC

from .metrics import ComprehensiveMetrics, _compute_ece, _compute_reliability


def evaluate_random_baseline(
    loader: DataLoader,
    n_trials: int = 100,
    seed: int = 42,
    format_spec: FormatSpec = VGC,
) -> ComprehensiveMetrics:
    """Random baseline: uniformly random config selection.

    Averaged over n_trials for stable estimates.
    """
    all_targets = []
    for batch in loader:
        _, _, _, _, _, cfg_tgt, _ = batch
        all_targets.append(cfg_tgt.cpu().numpy())

    targets = np.concatenate(all_targets, axis=0)  # [N]
    n = len(targets)

    rng = np.random.RandomState(seed)

    config_accs = []
    bring_accs = []
    bring_overlaps = []
    lead_accs = []
    lead_overlaps = []

    for _ in range(n_trials):
        preds = rng.randint(0, format_spec.num_configs, size=n)

        config_accs.append(float(np.mean(preds == targets)))

        bring_match = 0
        bring_overlap_sum = 0.0
        lead_match = 0
        lead_overlap_sum = 0.0

        for i in range(n):
            pred_bring = set(format_spec.configs[preds[i]][0])
            true_bring = set(format_spec.configs[targets[i]][0])
            pred_lead = set(format_spec.configs[preds[i]][1])
            true_lead = set(format_spec.configs[targets[i]][1])

            if pred_bring == true_bring:
                bring_match += 1
            bring_overlap_sum += len(pred_bring & true_bring) / format_spec.team_size

            if pred_lead == true_lead:
                lead_match += 1
            lead_overlap_sum += len(pred_lead & true_lead) / format_spec.num_leads

        bring_accs.append(bring_match / n)
        bring_overlaps.append(bring_overlap_sum / n)
        lead_accs.append(lead_match / n)
        lead_overlaps.append(lead_overlap_sum / n)

    return ComprehensiveMetrics(
        config_accuracy=float(np.mean(config_accs)),
        config_top3_accuracy=3 / format_spec.num_configs,  # analytical
        config_top5_accuracy=5 / format_spec.num_configs,
        bring_set_accuracy=float(np.mean(bring_accs)),
        bring_overlap_accuracy=float(np.mean(bring_overlaps)),
        lead_set_accuracy=float(np.mean(lead_accs)),
        lead_overlap_accuracy=float(np.mean(lead_overlaps)),
        loss=float('nan'),
        ece=0.0,
        reliability={'midpoints': [], 'accuracies': [], 'counts': []},
        mean_confidence=1.0 / format_spec.num_configs,
        n_samples=n,
    )


def evaluate_popular_baseline(
    train_loader: DataLoader,
    test_loader: DataLoader,
    format_spec: FormatSpec = VGC,
) -> ComprehensiveMetrics:
    """Most-popular baseline: always predict the most frequent config from training.

    Computes the empirical config frequency distribution from training data,
    then predicts the mode config for every test sample.
    """
    # Count config frequencies in training data
    config_counts = np.zeros(format_spec.num_configs, dtype=np.int64)
    for batch in train_loader:
        _, _, _, _, _, cfg_tgt, _ = batch
        for c in cfg_tgt.cpu().numpy():
            config_counts[c] += 1

    most_popular = int(np.argmax(config_counts))

    # Collect test targets
    all_targets = []
    for batch in test_loader:
        _, _, _, _, _, cfg_tgt, _ = batch
        all_targets.append(cfg_tgt.cpu().numpy())

    targets = np.concatenate(all_targets, axis=0)
    n = len(targets)

    preds = np.full(n, most_popular)

    config_accuracy = float(np.mean(preds == targets))

    bring_match = 0
    bring_overlap_sum = 0.0
    lead_match = 0
    lead_overlap_sum = 0.0

    for i in range(n):
        pred_bring = set(format_spec.configs[preds[i]][0])
        true_bring = set(format_spec.configs[targets[i]][0])
        pred_lead = set(format_spec.configs[preds[i]][1])
        true_lead = set(format_spec.configs[targets[i]][1])

        if pred_bring == true_bring:
            bring_match += 1
        bring_overlap_sum += len(pred_bring & true_bring) / 4.0

        if pred_lead == true_lead:
            lead_match += 1
        lead_overlap_sum += len(pred_lead & true_lead) / 2.0

    return ComprehensiveMetrics(
        config_accuracy=config_accuracy,
        config_top3_accuracy=config_accuracy,  # single prediction = same as top-1
        config_top5_accuracy=config_accuracy,
        bring_set_accuracy=bring_match / max(n, 1),
        bring_overlap_accuracy=bring_overlap_sum / max(n, 1),
        lead_set_accuracy=lead_match / max(n, 1),
        lead_overlap_accuracy=lead_overlap_sum / max(n, 1),
        loss=float('nan'),
        ece=0.0,
        reliability={'midpoints': [], 'accuracies': [], 'counts': []},
        mean_confidence=0.0,
        n_samples=n,
    )
