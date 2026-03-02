"""
Baseline evaluation strategies for comparison.

1. Random: uniformly random bring/lead selection
2. Most-popular: always pick the most frequently brought/led Pokemon
"""

from __future__ import annotations

import numpy as np
from torch.utils.data import DataLoader

from .metrics import (
    ComprehensiveMetrics, _compute_task_metrics,
    _compute_ece, _compute_reliability,
)


def evaluate_random_baseline(
    loader: DataLoader,
    n_trials: int = 100,
    seed: int = 42,
) -> ComprehensiveMetrics:
    """Random baseline: randomly select 4 to bring, 2 to lead.

    Averaged over n_trials for stable estimates.
    """
    all_bring_tgt = []
    all_lead_tgt = []

    for batch in loader:
        _, _, _, _, _, bring_tgt, lead_tgt, _ = batch
        all_bring_tgt.append(bring_tgt.numpy())
        all_lead_tgt.append(lead_tgt.numpy())

    bring_tgt = np.concatenate(all_bring_tgt, axis=0)
    lead_tgt = np.concatenate(all_lead_tgt, axis=0)
    n = bring_tgt.shape[0]

    rng = np.random.RandomState(seed)

    all_bring_m = []
    all_lead_m = []

    for _ in range(n_trials):
        random_bring = rng.rand(n, 6)
        all_bring_m.append(_compute_task_metrics(random_bring, bring_tgt, k=4))

        random_lead = rng.rand(n, 6)
        random_lead[bring_tgt < 0.5] = -1e9
        all_lead_m.append(_compute_task_metrics(random_lead, lead_tgt, k=2))

    avg_bring = _average_metrics(all_bring_m)
    avg_lead = _average_metrics(all_lead_m)

    return ComprehensiveMetrics(
        bring_set_accuracy=avg_bring['set_accuracy'],
        bring_hamming_accuracy=avg_bring['hamming_accuracy'],
        bring_overlap_accuracy=avg_bring['overlap_accuracy'],
        bring_precision_per_slot=avg_bring['precision_per_slot'],
        bring_recall_per_slot=avg_bring['recall_per_slot'],
        bring_f1_per_slot=avg_bring['f1_per_slot'],
        bring_macro_f1=avg_bring['macro_f1'],
        bring_ece=0.0,
        bring_reliability={'midpoints': [], 'accuracies': [], 'counts': []},
        lead_set_accuracy=avg_lead['set_accuracy'],
        lead_hamming_accuracy=avg_lead['hamming_accuracy'],
        lead_overlap_accuracy=avg_lead['overlap_accuracy'],
        lead_precision_per_slot=avg_lead['precision_per_slot'],
        lead_recall_per_slot=avg_lead['recall_per_slot'],
        lead_f1_per_slot=avg_lead['f1_per_slot'],
        lead_macro_f1=avg_lead['macro_f1'],
        lead_ece=0.0,
        lead_reliability={'midpoints': [], 'accuracies': [], 'counts': []},
        total_loss=float('nan'),
        bring_loss=float('nan'),
        lead_loss=float('nan'),
        n_samples=n,
    )


def evaluate_popular_baseline(
    train_loader: DataLoader,
    test_loader: DataLoader,
) -> ComprehensiveMetrics:
    """Most-popular baseline: predict slot-wise bring/lead frequencies.

    Computes marginal probability of each slot being brought/led
    from the training set, then uses those as constant predictions.
    """
    bring_sum = np.zeros(6, dtype=np.float64)
    lead_sum = np.zeros(6, dtype=np.float64)
    n_train = 0

    for batch in train_loader:
        _, _, _, _, _, bring_tgt, lead_tgt, _ = batch
        bring_sum += bring_tgt.numpy().sum(axis=0)
        lead_sum += lead_tgt.numpy().sum(axis=0)
        n_train += bring_tgt.shape[0]

    bring_freq = bring_sum / n_train
    lead_freq = lead_sum / n_train

    all_bring_tgt = []
    all_lead_tgt = []

    for batch in test_loader:
        _, _, _, _, _, bring_tgt, lead_tgt, _ = batch
        all_bring_tgt.append(bring_tgt.numpy())
        all_lead_tgt.append(lead_tgt.numpy())

    bring_tgt = np.concatenate(all_bring_tgt, axis=0)
    lead_tgt = np.concatenate(all_lead_tgt, axis=0)
    n = bring_tgt.shape[0]

    bring_pred = np.tile(bring_freq, (n, 1))
    lead_pred = np.tile(lead_freq, (n, 1))

    bring_metrics = _compute_task_metrics(bring_pred, bring_tgt, k=4)
    lead_pred_masked = lead_pred.copy()
    lead_pred_masked[bring_tgt < 0.5] = -1e9
    lead_metrics = _compute_task_metrics(lead_pred_masked, lead_tgt, k=2)

    bring_ece = _compute_ece(bring_pred, bring_tgt)
    bring_rel = _compute_reliability(bring_pred, bring_tgt)

    return ComprehensiveMetrics(
        bring_set_accuracy=bring_metrics['set_accuracy'],
        bring_hamming_accuracy=bring_metrics['hamming_accuracy'],
        bring_overlap_accuracy=bring_metrics['overlap_accuracy'],
        bring_precision_per_slot=bring_metrics['precision_per_slot'],
        bring_recall_per_slot=bring_metrics['recall_per_slot'],
        bring_f1_per_slot=bring_metrics['f1_per_slot'],
        bring_macro_f1=bring_metrics['macro_f1'],
        bring_ece=bring_ece,
        bring_reliability=bring_rel,
        lead_set_accuracy=lead_metrics['set_accuracy'],
        lead_hamming_accuracy=lead_metrics['hamming_accuracy'],
        lead_overlap_accuracy=lead_metrics['overlap_accuracy'],
        lead_precision_per_slot=lead_metrics['precision_per_slot'],
        lead_recall_per_slot=lead_metrics['recall_per_slot'],
        lead_f1_per_slot=lead_metrics['f1_per_slot'],
        lead_macro_f1=lead_metrics['macro_f1'],
        lead_ece=0.0,
        lead_reliability={'midpoints': [], 'accuracies': [], 'counts': []},
        total_loss=float('nan'),
        bring_loss=float('nan'),
        lead_loss=float('nan'),
        n_samples=n,
    )


def _average_metrics(metrics_list: list[dict]) -> dict:
    """Average a list of metric dicts across trials."""
    result = {}
    for key in metrics_list[0]:
        vals = [m[key] for m in metrics_list]
        if isinstance(vals[0], list):
            result[key] = [float(np.mean([v[i] for v in vals]))
                           for i in range(len(vals[0]))]
        else:
            result[key] = float(np.mean(vals))
    return result
