"""
Baseline evaluation strategies for BattleNet comparison.

1. Random: value = 0.5, policy = uniform random over actions
2. Most-popular: value = training set win rate, policy = action frequency
"""

from __future__ import annotations

import numpy as np
import torch
from torch.utils.data import DataLoader

from .battle_metrics import BattleComprehensiveMetrics, _policy_topk_accuracy
from .metrics import _compute_ece, _compute_reliability


def evaluate_random_battle_baseline(
    loader: DataLoader,
    vocab: dict,
    n_trials: int = 100,
    seed: int = 42,
) -> BattleComprehensiveMetrics:
    """Random baseline: value=0.5, policy=uniform random.

    Averaged over n_trials for stable policy accuracy estimates.
    """
    all_vtgt = []
    all_pa_tgt = []
    all_pb_tgt = []

    for batch in loader:
        _, _, _, _, _, _, vtgt, pa_tgt, pb_tgt = batch
        all_vtgt.append(vtgt.cpu().numpy())
        all_pa_tgt.append(pa_tgt)
        all_pb_tgt.append(pb_tgt)

    value_tgt = np.concatenate(all_vtgt)
    pa_tgt = torch.cat(all_pa_tgt).cpu()
    pb_tgt = torch.cat(all_pb_tgt).cpu()
    n = len(value_tgt)
    num_actions = vocab['num_actions']

    # Value: always predict 0.5
    value_accuracy = float(np.mean(
        (np.full(n, 0.5) > 0.5).astype(float) == value_tgt))

    rng = np.random.RandomState(seed)
    pa_top1_trials = []
    pb_top1_trials = []
    pa_top3_trials = []
    pb_top3_trials = []

    for _ in range(n_trials):
        rand_pa = torch.tensor(rng.rand(n, num_actions), dtype=torch.float32)
        rand_pb = torch.tensor(rng.rand(n, num_actions), dtype=torch.float32)
        pa_top1_trials.append(_policy_topk_accuracy(rand_pa, pa_tgt, k=1))
        pb_top1_trials.append(_policy_topk_accuracy(rand_pb, pb_tgt, k=1))
        pa_top3_trials.append(_policy_topk_accuracy(rand_pa, pa_tgt, k=3))
        pb_top3_trials.append(_policy_topk_accuracy(rand_pb, pb_tgt, k=3))

    pa_top1 = float(np.mean(pa_top1_trials))
    pb_top1 = float(np.mean(pb_top1_trials))
    pa_top3 = float(np.mean(pa_top3_trials))
    pb_top3 = float(np.mean(pb_top3_trials))

    return BattleComprehensiveMetrics(
        value_accuracy=value_accuracy,
        value_ece=0.0,
        value_reliability={'midpoints': [], 'accuracies': [], 'counts': []},
        policy_a_top1_accuracy=pa_top1,
        policy_b_top1_accuracy=pb_top1,
        policy_combined_top1_accuracy=(pa_top1 + pb_top1) / 2,
        policy_a_top3_accuracy=pa_top3,
        policy_b_top3_accuracy=pb_top3,
        total_loss=float('nan'),
        value_loss=float('nan'),
        policy_a_loss=float('nan'),
        policy_b_loss=float('nan'),
        n_samples=n,
    )


def evaluate_popular_battle_baseline(
    train_loader: DataLoader,
    test_loader: DataLoader,
    vocab: dict,
) -> BattleComprehensiveMetrics:
    """Most-popular baseline: predict most frequent actions.

    Value: predict the training set win rate.
    Policy: use action frequency distributions from training set.
    """
    num_actions = vocab['num_actions']

    # Compute action frequencies from training set
    pa_counts = np.zeros(num_actions, dtype=np.float64)
    pb_counts = np.zeros(num_actions, dtype=np.float64)
    value_sum = 0.0
    n_train = 0

    for batch in train_loader:
        _, _, _, _, _, _, vtgt, pa_tgt, pb_tgt = batch
        value_sum += vtgt.cpu().numpy().sum()
        n_train += vtgt.shape[0]
        for a in pa_tgt.cpu().numpy():
            if a > 0:
                pa_counts[a] += 1
        for b in pb_tgt.cpu().numpy():
            if b > 0:
                pb_counts[b] += 1

    win_rate = value_sum / max(n_train, 1)

    # Normalize to logits (higher = more frequent)
    pa_logits = torch.tensor(pa_counts, dtype=torch.float32).unsqueeze(0)
    pb_logits = torch.tensor(pb_counts, dtype=torch.float32).unsqueeze(0)

    # Evaluate on test set
    all_vtgt = []
    all_pa_tgt = []
    all_pb_tgt = []

    for batch in test_loader:
        _, _, _, _, _, _, vtgt, pa_tgt, pb_tgt = batch
        all_vtgt.append(vtgt.cpu().numpy())
        all_pa_tgt.append(pa_tgt)
        all_pb_tgt.append(pb_tgt)

    value_tgt = np.concatenate(all_vtgt)
    pa_tgt = torch.cat(all_pa_tgt).cpu()
    pb_tgt = torch.cat(all_pb_tgt).cpu()
    n = len(value_tgt)

    # Value accuracy using training win rate as constant prediction
    value_pred = np.full(n, win_rate)
    value_accuracy = float(np.mean(
        (value_pred > 0.5).astype(float) == value_tgt))

    # Value calibration
    value_ece = _compute_ece(
        value_pred.reshape(-1, 1), value_tgt.reshape(-1, 1))
    value_rel = _compute_reliability(
        value_pred.reshape(-1, 1), value_tgt.reshape(-1, 1))

    # Policy accuracy (broadcast constant logits to all samples)
    pa_logits_all = pa_logits.expand(n, -1)
    pb_logits_all = pb_logits.expand(n, -1)

    pa_top1 = _policy_topk_accuracy(pa_logits_all, pa_tgt, k=1)
    pb_top1 = _policy_topk_accuracy(pb_logits_all, pb_tgt, k=1)
    pa_top3 = _policy_topk_accuracy(pa_logits_all, pa_tgt, k=3)
    pb_top3 = _policy_topk_accuracy(pb_logits_all, pb_tgt, k=3)

    return BattleComprehensiveMetrics(
        value_accuracy=value_accuracy,
        value_ece=value_ece,
        value_reliability=value_rel,
        policy_a_top1_accuracy=pa_top1,
        policy_b_top1_accuracy=pb_top1,
        policy_combined_top1_accuracy=(pa_top1 + pb_top1) / 2,
        policy_a_top3_accuracy=pa_top3,
        policy_b_top3_accuracy=pb_top3,
        total_loss=float('nan'),
        value_loss=float('nan'),
        policy_a_loss=float('nan'),
        policy_b_loss=float('nan'),
        n_samples=n,
    )
