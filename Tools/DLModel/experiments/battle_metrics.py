"""
Comprehensive metrics for BattleNet evaluation.

Computes all metrics needed for thesis reporting:
- Value accuracy (binary classification at 0.5 threshold)
- Policy top-1 and top-3 accuracy (non-padded samples only)
- Value calibration (ECE, reliability diagram data)
- Decomposed losses (value, policy_a, policy_b)
"""

from __future__ import annotations

from dataclasses import dataclass, asdict

import numpy as np
import torch
import torch.nn as nn
from torch.utils.data import DataLoader

from .metrics import _compute_ece, _compute_reliability


@dataclass
class BattleComprehensiveMetrics:
    """All metrics from evaluating a BattleNet model on a dataset."""

    # Value metrics
    value_accuracy: float
    value_ece: float
    value_reliability: dict

    # Policy metrics
    policy_a_top1_accuracy: float
    policy_b_top1_accuracy: float
    policy_combined_top1_accuracy: float
    policy_a_top3_accuracy: float
    policy_b_top3_accuracy: float

    # Loss metrics
    total_loss: float
    value_loss: float
    policy_a_loss: float
    policy_b_loss: float
    n_samples: int

    def to_dict(self) -> dict:
        d = asdict(self)

        def _convert(obj):
            if isinstance(obj, (np.floating, np.integer)):
                return float(obj)
            if isinstance(obj, np.ndarray):
                return obj.tolist()
            if isinstance(obj, dict):
                return {k: _convert(v) for k, v in obj.items()}
            if isinstance(obj, list):
                return [_convert(v) for v in obj]
            return obj
        return _convert(d)


def evaluate_battle_comprehensive(
    model: torch.nn.Module,
    loader: DataLoader,
    device: torch.device,
) -> BattleComprehensiveMetrics:
    """Run comprehensive evaluation on a BattleNet dataset."""
    model.eval()

    all_value_pred = []
    all_value_tgt = []
    all_pa_logits = []
    all_pb_logits = []
    all_pa_tgt = []
    all_pb_tgt = []

    value_loss_fn = nn.BCELoss()
    policy_loss_fn = nn.CrossEntropyLoss(ignore_index=0)

    total_vloss = total_paloss = total_pbloss = 0.0
    n_batches = 0

    with torch.no_grad():
        for sids, mids, aids, iids, tids, num, vtgt, pa_tgt, pb_tgt in loader:
            sids = sids.to(device)
            mids = mids.to(device)
            aids = aids.to(device)
            iids = iids.to(device)
            tids = tids.to(device)
            num = num.to(device)
            vtgt_d = vtgt.to(device)
            pa_tgt_d = pa_tgt.to(device)
            pb_tgt_d = pb_tgt.to(device)

            value, pol_a, pol_b = model(sids, mids, aids, iids, tids, num)

            v_loss = value_loss_fn(value, vtgt_d)
            pa_loss = policy_loss_fn(pol_a, pa_tgt_d)
            pb_loss = policy_loss_fn(pol_b, pb_tgt_d)

            total_vloss += v_loss.item()
            total_paloss += pa_loss.item()
            total_pbloss += pb_loss.item()
            n_batches += 1

            all_value_pred.append(value.cpu())
            all_value_tgt.append(vtgt)
            all_pa_logits.append(pol_a.cpu())
            all_pb_logits.append(pol_b.cpu())
            all_pa_tgt.append(pa_tgt)
            all_pb_tgt.append(pb_tgt)

    value_pred = torch.cat(all_value_pred).numpy()
    value_tgt = torch.cat(all_value_tgt).numpy()
    pa_logits = torch.cat(all_pa_logits)
    pb_logits = torch.cat(all_pb_logits)
    pa_tgt = torch.cat(all_pa_tgt)
    pb_tgt = torch.cat(all_pb_tgt)
    n = len(value_pred)

    # Value accuracy
    value_accuracy = float(np.mean(
        (value_pred > 0.5).astype(float) == value_tgt))

    # Value calibration
    value_ece = _compute_ece(
        value_pred.reshape(-1, 1), value_tgt.reshape(-1, 1))
    value_rel = _compute_reliability(
        value_pred.reshape(-1, 1), value_tgt.reshape(-1, 1))

    # Policy accuracies (non-padded only)
    pa_top1 = _policy_topk_accuracy(pa_logits, pa_tgt, k=1)
    pb_top1 = _policy_topk_accuracy(pb_logits, pb_tgt, k=1)
    pa_top3 = _policy_topk_accuracy(pa_logits, pa_tgt, k=3)
    pb_top3 = _policy_topk_accuracy(pb_logits, pb_tgt, k=3)

    avg_vloss = total_vloss / max(n_batches, 1)
    avg_paloss = total_paloss / max(n_batches, 1)
    avg_pbloss = total_pbloss / max(n_batches, 1)

    return BattleComprehensiveMetrics(
        value_accuracy=value_accuracy,
        value_ece=value_ece,
        value_reliability=value_rel,
        policy_a_top1_accuracy=pa_top1,
        policy_b_top1_accuracy=pb_top1,
        policy_combined_top1_accuracy=(pa_top1 + pb_top1) / 2,
        policy_a_top3_accuracy=pa_top3,
        policy_b_top3_accuracy=pb_top3,
        total_loss=avg_vloss + avg_paloss + avg_pbloss,
        value_loss=avg_vloss,
        policy_a_loss=avg_paloss,
        policy_b_loss=avg_pbloss,
        n_samples=n,
    )


def _policy_topk_accuracy(
    logits: torch.Tensor, targets: torch.Tensor, k: int,
) -> float:
    """Top-k accuracy for policy predictions, ignoring padded targets (0)."""
    mask = targets > 0
    if mask.sum() == 0:
        return 0.0
    logits_m = logits[mask]
    targets_m = targets[mask]
    topk = logits_m.topk(k, dim=1).indices
    correct = (topk == targets_m.unsqueeze(1)).any(dim=1)
    return float(correct.float().mean().item())
