"""
Comprehensive metrics for TeamPreviewNet evaluation.

Computes all metrics needed for thesis reporting:
- Top-k set accuracy (bring-4, lead-2)
- Hamming accuracy (per-slot binary correctness)
- Overlap accuracy (|predicted intersection actual| / k)
- Per-slot precision / recall / F1
- Calibration metrics (ECE, reliability diagram data)
"""

from __future__ import annotations

from dataclasses import dataclass, asdict
import numpy as np
import torch
from torch.utils.data import DataLoader


@dataclass
class ComprehensiveMetrics:
    """All metrics from evaluating a model on a dataset."""

    # Bring metrics
    bring_set_accuracy: float
    bring_hamming_accuracy: float
    bring_overlap_accuracy: float
    bring_precision_per_slot: list[float]
    bring_recall_per_slot: list[float]
    bring_f1_per_slot: list[float]
    bring_macro_f1: float
    bring_ece: float
    bring_reliability: dict

    # Lead metrics
    lead_set_accuracy: float
    lead_hamming_accuracy: float
    lead_overlap_accuracy: float
    lead_precision_per_slot: list[float]
    lead_recall_per_slot: list[float]
    lead_f1_per_slot: list[float]
    lead_macro_f1: float
    lead_ece: float
    lead_reliability: dict

    # Loss
    total_loss: float
    bring_loss: float
    lead_loss: float
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
) -> ComprehensiveMetrics:
    """Run comprehensive evaluation on a dataset."""
    model.eval()
    all_bring_pred = []
    all_bring_tgt = []
    all_lead_pred = []
    all_lead_tgt = []

    bring_loss_fn = torch.nn.BCELoss()
    lead_loss_fn = torch.nn.BCELoss(reduction='none')
    total_bloss = total_lloss = 0.0
    n_batches = 0

    with torch.no_grad():
        for sids, mids, aids, iids, tids, bring_tgt, lead_tgt, val_tgt in loader:
            sids = sids.to(device, non_blocking=True)
            mids = mids.to(device, non_blocking=True)
            aids = aids.to(device, non_blocking=True)
            iids = iids.to(device, non_blocking=True)
            tids = tids.to(device, non_blocking=True)
            bring_tgt_d = bring_tgt.to(device, non_blocking=True)
            lead_tgt_d = lead_tgt.to(device, non_blocking=True)

            bring_pred, lead_pred = model(sids, mids, aids, iids, tids)

            b_loss = bring_loss_fn(bring_pred, bring_tgt_d)
            l_loss_raw = lead_loss_fn(lead_pred, lead_tgt_d)
            l_mask = bring_tgt_d
            l_loss = (l_loss_raw * l_mask).sum() / (l_mask.sum() + 1e-8)

            total_bloss += b_loss.item()
            total_lloss += l_loss.item()
            n_batches += 1

            all_bring_pred.append(bring_pred.cpu())
            all_bring_tgt.append(bring_tgt.cpu())
            all_lead_pred.append(lead_pred.cpu())
            all_lead_tgt.append(lead_tgt.cpu())

    bring_pred_np = torch.cat(all_bring_pred, dim=0).numpy()
    bring_tgt_np = torch.cat(all_bring_tgt, dim=0).numpy()
    lead_pred_np = torch.cat(all_lead_pred, dim=0).numpy()
    lead_tgt_np = torch.cat(all_lead_tgt, dim=0).numpy()
    n = bring_pred_np.shape[0]

    bring_metrics = _compute_task_metrics(bring_pred_np, bring_tgt_np, k=4)

    # Lead: mask non-brought to -inf before computing top-k
    lead_pred_masked = lead_pred_np.copy()
    lead_pred_masked[bring_tgt_np < 0.5] = -1e9
    lead_metrics = _compute_task_metrics(lead_pred_masked, lead_tgt_np, k=2)

    # Calibration for bring (all predictions)
    bring_ece = _compute_ece(bring_pred_np, bring_tgt_np)
    bring_rel = _compute_reliability(bring_pred_np, bring_tgt_np)

    # Calibration for lead (only brought slots)
    brought_mask = bring_tgt_np > 0.5
    if brought_mask.sum() > 0:
        lead_ece = _compute_ece(
            lead_pred_np[brought_mask], lead_tgt_np[brought_mask])
        lead_rel = _compute_reliability(
            lead_pred_np[brought_mask], lead_tgt_np[brought_mask])
    else:
        lead_ece = 0.0
        lead_rel = {'midpoints': [], 'accuracies': [], 'counts': []}

    avg_bloss = total_bloss / max(n_batches, 1)
    avg_lloss = total_lloss / max(n_batches, 1)

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
        lead_ece=lead_ece,
        lead_reliability=lead_rel,
        total_loss=avg_bloss + avg_lloss,
        bring_loss=avg_bloss,
        lead_loss=avg_lloss,
        n_samples=n,
    )


def _compute_task_metrics(
    pred: np.ndarray, tgt: np.ndarray, k: int,
) -> dict:
    """Compute set accuracy, hamming, overlap, per-slot P/R/F1."""
    pred_topk = np.argsort(-pred, axis=1)[:, :k]
    true_topk = np.argsort(-tgt, axis=1)[:, :k]

    pred_sets = [set(row) for row in pred_topk]
    true_sets = [set(row) for row in true_topk]

    set_accuracy = float(np.mean([p == t for p, t in zip(pred_sets, true_sets)]))
    overlap = float(np.mean([len(p & t) / k for p, t in zip(pred_sets, true_sets)]))

    pred_binary = (pred > 0.5).astype(float)
    tgt_binary = (tgt > 0.5).astype(float)
    hamming = float(np.mean(pred_binary == tgt_binary))

    precision_per_slot = []
    recall_per_slot = []
    f1_per_slot = []
    for slot in range(6):
        tp = np.sum((pred_binary[:, slot] == 1) & (tgt_binary[:, slot] == 1))
        fp = np.sum((pred_binary[:, slot] == 1) & (tgt_binary[:, slot] == 0))
        fn = np.sum((pred_binary[:, slot] == 0) & (tgt_binary[:, slot] == 1))
        prec = float(tp / (tp + fp + 1e-8))
        rec = float(tp / (tp + fn + 1e-8))
        f1 = float(2 * prec * rec / (prec + rec + 1e-8))
        precision_per_slot.append(prec)
        recall_per_slot.append(rec)
        f1_per_slot.append(f1)

    return {
        'set_accuracy': set_accuracy,
        'hamming_accuracy': hamming,
        'overlap_accuracy': overlap,
        'precision_per_slot': precision_per_slot,
        'recall_per_slot': recall_per_slot,
        'f1_per_slot': f1_per_slot,
        'macro_f1': float(np.mean(f1_per_slot)),
    }


def _compute_ece(
    pred: np.ndarray, tgt: np.ndarray, n_bins: int = 15,
) -> float:
    """Expected Calibration Error."""
    pred_flat = pred.flatten()
    tgt_flat = tgt.flatten()

    bin_boundaries = np.linspace(0, 1, n_bins + 1)
    ece = 0.0
    total = len(pred_flat)
    if total == 0:
        return 0.0

    for i in range(n_bins):
        mask = (pred_flat >= bin_boundaries[i]) & (pred_flat < bin_boundaries[i + 1])
        count = mask.sum()
        if count == 0:
            continue
        bin_acc = tgt_flat[mask].mean()
        bin_conf = pred_flat[mask].mean()
        ece += (count / total) * abs(bin_acc - bin_conf)

    return float(ece)


def _compute_reliability(
    pred: np.ndarray, tgt: np.ndarray, n_bins: int = 15,
) -> dict:
    """Compute reliability diagram data."""
    pred_flat = pred.flatten()
    tgt_flat = tgt.flatten()

    bin_boundaries = np.linspace(0, 1, n_bins + 1)
    midpoints = []
    accuracies = []
    counts = []

    for i in range(n_bins):
        mask = (pred_flat >= bin_boundaries[i]) & (pred_flat < bin_boundaries[i + 1])
        count = int(mask.sum())
        if count == 0:
            midpoints.append(float((bin_boundaries[i] + bin_boundaries[i + 1]) / 2))
            accuracies.append(0.0)
            counts.append(0)
        else:
            midpoints.append(float(pred_flat[mask].mean()))
            accuracies.append(float(tgt_flat[mask].mean()))
            counts.append(count)

    return {'midpoints': midpoints, 'accuracies': accuracies, 'counts': counts}
