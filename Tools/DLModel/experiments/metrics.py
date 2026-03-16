"""
Comprehensive metrics for TeamPreviewNet evaluation (VGC config classification).

Computes all metrics needed for thesis reporting:
- Config accuracy (exact match of full configuration)
- Bring-set accuracy (4-of-6 selection matches)
- Lead-set accuracy (2-of-4 lead pair matches)
- Bring/lead overlap accuracy (partial credit)
- Top-k config accuracy
- Calibration metrics (ECE, reliability diagram data)
"""

from __future__ import annotations

from dataclasses import dataclass, asdict
from pathlib import Path
import sys

import numpy as np
import torch
from torch.utils.data import DataLoader

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from format_spec import FormatSpec, VGC


@dataclass
class ComprehensiveMetrics:
    """All metrics from evaluating a model on a dataset."""

    # Config-level metrics
    config_accuracy: float
    config_top3_accuracy: float
    config_top5_accuracy: float

    # Decomposed set metrics
    bring_set_accuracy: float
    bring_overlap_accuracy: float
    lead_set_accuracy: float
    lead_overlap_accuracy: float

    # Loss and calibration
    loss: float
    ece: float
    reliability: dict
    mean_confidence: float

    n_samples: int

    def to_dict(self) -> dict:
        d = asdict(self)
        # Ensure all values are JSON-serialisable
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


def evaluate_comprehensive(
    model: torch.nn.Module,
    loader: DataLoader,
    device: torch.device,
    format_spec: FormatSpec = VGC,
) -> ComprehensiveMetrics:
    """Run comprehensive evaluation on a dataset."""
    model.eval()
    all_logits = []
    all_targets = []

    loss_fn = torch.nn.CrossEntropyLoss()
    total_loss = 0.0
    n_batches = 0

    with torch.no_grad():
        for sids, mids, aids, iids, tids, cfg_tgt, val_tgt in loader:
            sids = sids.to(device, non_blocking=True)
            mids = mids.to(device, non_blocking=True)
            aids = aids.to(device, non_blocking=True)
            iids = iids.to(device, non_blocking=True)
            tids = tids.to(device, non_blocking=True)
            cfg_tgt_d = cfg_tgt.to(device, non_blocking=True)

            logits = model(sids, mids, aids, iids, tids)
            loss = loss_fn(logits, cfg_tgt_d)
            total_loss += loss.item()
            n_batches += 1

            all_logits.append(logits.cpu())
            all_targets.append(cfg_tgt.cpu())

    logits_t = torch.cat(all_logits, dim=0)        # [N, 90]
    targets_t = torch.cat(all_targets, dim=0)       # [N]
    n = targets_t.size(0)

    # Softmax probabilities
    probs = torch.softmax(logits_t, dim=1).numpy()  # [N, 90]
    targets = targets_t.numpy()                      # [N]
    preds = probs.argmax(axis=1)                     # [N]

    # ── Config accuracy ──
    config_accuracy = float(np.mean(preds == targets))

    # Top-k
    top3 = np.argsort(-probs, axis=1)[:, :3]
    config_top3 = float(np.mean([targets[i] in top3[i] for i in range(n)]))
    top5 = np.argsort(-probs, axis=1)[:, :5]
    config_top5 = float(np.mean([targets[i] in top5[i] for i in range(n)]))

    # ── Decomposed bring/lead metrics ──
    bring_set_match = 0
    bring_overlap_sum = 0.0
    lead_set_match = 0
    lead_overlap_sum = 0.0

    for i in range(n):
        pred_bring = set(format_spec.configs[preds[i]][0])
        true_bring = set(format_spec.configs[targets[i]][0])
        pred_lead = set(format_spec.configs[preds[i]][1])
        true_lead = set(format_spec.configs[targets[i]][1])

        if pred_bring == true_bring:
            bring_set_match += 1
        bring_overlap_sum += len(pred_bring & true_bring) / format_spec.team_size

        if pred_lead == true_lead:
            lead_set_match += 1
        lead_overlap_sum += len(pred_lead & true_lead) / format_spec.num_leads

    bring_set_accuracy = bring_set_match / max(n, 1)
    bring_overlap_accuracy = bring_overlap_sum / max(n, 1)
    lead_set_accuracy = lead_set_match / max(n, 1)
    lead_overlap_accuracy = lead_overlap_sum / max(n, 1)

    # ── Calibration (confidence-based) ──
    confidences = probs[np.arange(n), preds]   # predicted prob of top class
    correctness = (preds == targets).astype(float)
    mean_confidence = float(np.mean(confidences))

    ece = _compute_ece(confidences, correctness)
    reliability = _compute_reliability(confidences, correctness)

    avg_loss = total_loss / max(n_batches, 1)

    return ComprehensiveMetrics(
        config_accuracy=config_accuracy,
        config_top3_accuracy=config_top3,
        config_top5_accuracy=config_top5,
        bring_set_accuracy=bring_set_accuracy,
        bring_overlap_accuracy=bring_overlap_accuracy,
        lead_set_accuracy=lead_set_accuracy,
        lead_overlap_accuracy=lead_overlap_accuracy,
        loss=avg_loss,
        ece=ece,
        reliability=reliability,
        mean_confidence=mean_confidence,
        n_samples=n,
    )


def _compute_ece(
    confidences: np.ndarray, correctness: np.ndarray, n_bins: int = 15,
) -> float:
    """Expected Calibration Error over top-1 confidence."""
    total = len(confidences)
    if total == 0:
        return 0.0

    bin_boundaries = np.linspace(0, 1, n_bins + 1)
    ece = 0.0

    for i in range(n_bins):
        mask = (confidences >= bin_boundaries[i]) & (confidences < bin_boundaries[i + 1])
        count = mask.sum()
        if count == 0:
            continue
        bin_acc = correctness[mask].mean()
        bin_conf = confidences[mask].mean()
        ece += (count / total) * abs(bin_acc - bin_conf)

    return float(ece)


def _compute_reliability(
    confidences: np.ndarray, correctness: np.ndarray, n_bins: int = 15,
) -> dict:
    """Compute reliability diagram data."""
    bin_boundaries = np.linspace(0, 1, n_bins + 1)
    midpoints = []
    accuracies = []
    counts = []

    for i in range(n_bins):
        mask = (confidences >= bin_boundaries[i]) & (confidences < bin_boundaries[i + 1])
        count = int(mask.sum())
        if count == 0:
            midpoints.append(float((bin_boundaries[i] + bin_boundaries[i + 1]) / 2))
            accuracies.append(0.0)
            counts.append(0)
        else:
            midpoints.append(float(confidences[mask].mean()))
            accuracies.append(float(correctness[mask].mean()))
            counts.append(count)

    return {'midpoints': midpoints, 'accuracies': accuracies, 'counts': counts}
