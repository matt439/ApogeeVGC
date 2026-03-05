"""
Core training function, callable programmatically by the experiment runner.

Does not parse command-line arguments. Accepts configuration objects directly.
Returns a structured TrainResult with all metrics per epoch.
"""

from __future__ import annotations

import json
import time
from dataclasses import dataclass
from pathlib import Path

import torch
import torch.nn as nn
from torch.amp import autocast, GradScaler
from torch.utils.data import DataLoader

import sys
sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from team_preview_model import TeamPreviewNetV2

from .config import ExperimentConfig


@dataclass
class EpochMetrics:
    """Metrics recorded for a single epoch."""
    epoch: int
    train_loss: float
    train_bring_loss: float
    train_lead_loss: float
    val_loss: float
    val_bring_loss: float
    val_lead_loss: float
    val_bring_acc_top4: float
    val_lead_acc_top2: float
    lr: float
    elapsed_sec: float


@dataclass
class TrainResult:
    """Complete training result."""
    config: dict
    epoch_metrics: list[EpochMetrics]
    best_epoch: int
    best_val_loss: float
    best_model_path: str
    total_params: int
    total_time_sec: float


def train_model(
    config: ExperimentConfig,
    train_loader: DataLoader,
    val_loader: DataLoader,
    vocab: dict,
    output_dir: Path,
    device: torch.device | None = None,
    trial=None,
) -> TrainResult:
    """Train a TeamPreviewNetV2 model from a config.

    Parameters
    ----------
    config : ExperimentConfig
    train_loader, val_loader : DataLoader
    vocab : dict
    output_dir : Path
        Where to save the best model checkpoint.
    device : torch.device, optional
    trial : optuna.trial.Trial, optional
        If provided, reports intermediate values for pruning.
    """
    if device is None:
        device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')

    if device.type == 'cuda':
        torch.backends.cudnn.benchmark = True

    use_amp = device.type == 'cuda'
    scaler = GradScaler('cuda', enabled=use_amp)

    torch.manual_seed(config.train.seed)

    mc = config.model
    tc = config.train

    model = TeamPreviewNetV2(
        num_species=vocab['num_species'],
        num_moves=vocab['num_moves'],
        num_abilities=vocab['num_abilities'],
        num_items=vocab['num_items'],
        num_tera_types=vocab['num_tera_types'],
        species_embed_dim=mc.species_embed_dim,
        feat_embed_dim=mc.feat_embed_dim,
        pokemon_dim=mc.pokemon_dim,
        hidden_dim=mc.hidden_dim,
        num_trunk_layers=mc.num_trunk_layers,
        trunk_dropout=mc.trunk_dropout,
        head_dim=mc.head_dim,
        feature_flags=mc.feature_flags,
    ).to(device)

    total_params = sum(p.numel() for p in model.parameters())

    # torch.compile fuses GPU ops → fewer kernel launches, higher utilization
    if hasattr(torch, 'compile') and device.type == 'cuda':
        try:
            model = torch.compile(model)
        except Exception:
            pass  # graceful fallback if compile unavailable

    optimizer = torch.optim.AdamW(
        model.parameters(), lr=tc.lr, weight_decay=tc.weight_decay)

    if tc.scheduler == 'cosine':
        scheduler = torch.optim.lr_scheduler.CosineAnnealingLR(
            optimizer, tc.epochs)
    else:
        scheduler = torch.optim.lr_scheduler.ReduceLROnPlateau(
            optimizer, patience=3, factor=0.5)

    bring_loss_fn = nn.BCELoss()
    lead_loss_fn = nn.BCELoss(reduction='none')

    best_val_loss = float('inf')
    best_epoch = 0
    patience_counter = 0
    epoch_metrics_list = []

    output_dir.mkdir(parents=True, exist_ok=True)
    model_path = output_dir / 'model.pt'

    t_start = time.time()

    for epoch in range(tc.epochs):
        t_epoch = time.time()

        # ── Train ──
        model.train()
        t_loss = t_bloss = t_lloss = 0.0
        n_batches = 0

        for sids, mids, aids, iids, tids, bring_tgt, lead_tgt, val_tgt in train_loader:
            sids = sids.to(device, non_blocking=True)
            mids = mids.to(device, non_blocking=True)
            aids = aids.to(device, non_blocking=True)
            iids = iids.to(device, non_blocking=True)
            tids = tids.to(device, non_blocking=True)
            bring_tgt = bring_tgt.to(device, non_blocking=True)
            lead_tgt = lead_tgt.to(device, non_blocking=True)

            with autocast('cuda', enabled=use_amp):
                bring_pred, lead_pred = model(sids, mids, aids, iids, tids)

            # BCELoss requires float32 — compute loss outside autocast
            bring_pred = bring_pred.float()
            lead_pred = lead_pred.float()
            b_loss = bring_loss_fn(bring_pred, bring_tgt)
            l_loss_raw = lead_loss_fn(lead_pred, lead_tgt)
            l_mask = bring_tgt
            l_loss = (l_loss_raw * l_mask).sum() / (l_mask.sum() + 1e-8)
            loss = b_loss + l_loss

            optimizer.zero_grad()
            scaler.scale(loss).backward()
            scaler.unscale_(optimizer)
            torch.nn.utils.clip_grad_norm_(model.parameters(), tc.grad_clip)
            scaler.step(optimizer)
            scaler.update()

            t_loss += loss.item()
            t_bloss += b_loss.item()
            t_lloss += l_loss.item()
            n_batches += 1

        # ── Validate ──
        model.eval()
        v_loss = v_bloss = v_lloss = 0.0
        bring_acc_sum = lead_acc_sum = 0.0
        n_vbatches = 0

        with torch.no_grad():
            for sids, mids, aids, iids, tids, bring_tgt, lead_tgt, val_tgt in val_loader:
                sids = sids.to(device, non_blocking=True)
                mids = mids.to(device, non_blocking=True)
                aids = aids.to(device, non_blocking=True)
                iids = iids.to(device, non_blocking=True)
                tids = tids.to(device, non_blocking=True)
                bring_tgt = bring_tgt.to(device, non_blocking=True)
                lead_tgt = lead_tgt.to(device, non_blocking=True)

                with autocast('cuda', enabled=use_amp):
                    bring_pred, lead_pred = model(sids, mids, aids, iids, tids)

                bring_pred = bring_pred.float()
                lead_pred = lead_pred.float()
                b_loss = bring_loss_fn(bring_pred, bring_tgt)
                l_loss_raw = lead_loss_fn(lead_pred, lead_tgt)
                l_mask = bring_tgt
                l_loss = (l_loss_raw * l_mask).sum() / (l_mask.sum() + 1e-8)
                loss = b_loss + l_loss

                v_loss += loss.item()
                v_bloss += b_loss.item()
                v_lloss += l_loss.item()
                n_vbatches += 1

                bring_acc_sum += _compute_accuracy(bring_pred, bring_tgt, 4)

                lead_masked = lead_pred.clone()
                lead_masked[bring_tgt < 0.5] = -1e9
                lead_acc_sum += _compute_accuracy(lead_masked, lead_tgt, 2)

        if tc.scheduler == 'cosine':
            scheduler.step()
        else:
            scheduler.step(v_loss / max(n_vbatches, 1))

        lr = optimizer.param_groups[0]['lr']
        avg_v = v_loss / max(n_vbatches, 1)

        em = EpochMetrics(
            epoch=epoch + 1,
            train_loss=t_loss / max(n_batches, 1),
            train_bring_loss=t_bloss / max(n_batches, 1),
            train_lead_loss=t_lloss / max(n_batches, 1),
            val_loss=avg_v,
            val_bring_loss=v_bloss / max(n_vbatches, 1),
            val_lead_loss=v_lloss / max(n_vbatches, 1),
            val_bring_acc_top4=bring_acc_sum / max(n_vbatches, 1),
            val_lead_acc_top2=lead_acc_sum / max(n_vbatches, 1),
            lr=lr,
            elapsed_sec=time.time() - t_epoch,
        )
        epoch_metrics_list.append(em)

        # Optuna pruning
        if trial is not None:
            import optuna
            trial.report(avg_v, epoch)
            if trial.should_prune():
                raise optuna.TrialPruned()

        # Early stopping + checkpointing
        if avg_v < best_val_loss:
            best_val_loss = avg_v
            best_epoch = epoch + 1
            patience_counter = 0
            torch.save({
                'model_state_dict': model.state_dict(),
                'vocab': vocab,
                'config': config.to_dict(),
                'epoch': epoch + 1,
                'val_loss': avg_v,
                'model_version': 2,
                'args': {
                    'embed_dim': mc.species_embed_dim,
                    'feat_embed_dim': mc.feat_embed_dim,
                    'pokemon_dim': mc.pokemon_dim,
                    'hidden_dim': mc.hidden_dim,
                    'num_trunk_layers': mc.num_trunk_layers,
                    'trunk_dropout': mc.trunk_dropout,
                    'head_dim': mc.head_dim,
                    'feature_flags': mc.feature_flags,
                },
            }, model_path)
        else:
            patience_counter += 1
            if patience_counter >= tc.patience:
                break

    total_time = time.time() - t_start

    return TrainResult(
        config=config.to_dict(),
        epoch_metrics=epoch_metrics_list,
        best_epoch=best_epoch,
        best_val_loss=best_val_loss,
        best_model_path=str(model_path),
        total_params=total_params,
        total_time_sec=total_time,
    )


def load_model_from_checkpoint(
    checkpoint: dict,
    vocab: dict,
    device: torch.device,
) -> TeamPreviewNetV2:
    """Instantiate and load a TeamPreviewNetV2 from a checkpoint dict."""
    args = checkpoint['args']
    model = TeamPreviewNetV2(
        num_species=vocab['num_species'],
        num_moves=vocab['num_moves'],
        num_abilities=vocab['num_abilities'],
        num_items=vocab['num_items'],
        num_tera_types=vocab['num_tera_types'],
        species_embed_dim=args['embed_dim'],
        feat_embed_dim=args['feat_embed_dim'],
        pokemon_dim=args['pokemon_dim'],
        hidden_dim=args['hidden_dim'],
        num_trunk_layers=args.get('num_trunk_layers', 3),
        trunk_dropout=args.get('trunk_dropout', 0.3),
        head_dim=args.get('head_dim', 64),
        feature_flags=args.get('feature_flags'),
    ).to(device)
    model.load_state_dict(checkpoint['model_state_dict'])
    return model


def _compute_accuracy(
    scores: torch.Tensor, targets: torch.Tensor, k: int,
) -> float:
    """Top-k set accuracy (order-independent)."""
    pred_topk = scores.topk(k, dim=1).indices
    true_topk = targets.topk(k, dim=1).indices
    pred_sorted = pred_topk.sort(dim=1).values
    true_sorted = true_topk.sort(dim=1).values
    match = (pred_sorted == true_sorted).all(dim=1)
    return match.float().mean().item()
