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
from torch.utils.data import DataLoader

import sys
sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from team_preview_model import TeamPreviewNetV2, VGC_CONFIGS, NUM_VGC_CONFIGS

from .config import ExperimentConfig


@dataclass
class EpochMetrics:
    """Metrics recorded for a single epoch."""
    epoch: int
    train_loss: float
    val_loss: float
    val_config_accuracy: float
    val_bring_accuracy: float
    val_lead_accuracy: float
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
        torch.set_float32_matmul_precision('high')

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
    # Requires Triton (Linux/Mac only); skipped on Windows
    if (hasattr(torch, 'compile') and device.type == 'cuda'
            and sys.platform != 'win32'):
        try:
            model = torch.compile(model)
        except Exception:
            pass

    optimizer = torch.optim.AdamW(
        model.parameters(), lr=tc.lr, weight_decay=tc.weight_decay)

    if tc.scheduler == 'cosine':
        scheduler = torch.optim.lr_scheduler.CosineAnnealingLR(
            optimizer, tc.epochs)
    else:
        scheduler = torch.optim.lr_scheduler.ReduceLROnPlateau(
            optimizer, patience=3, factor=0.5)

    loss_fn = nn.CrossEntropyLoss()

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
        t_loss_acc = torch.tensor(0.0, device=device)
        n_batches = 0

        for sids, mids, aids, iids, tids, cfg_tgt, val_tgt in train_loader:
            sids = sids.to(device, non_blocking=True)
            mids = mids.to(device, non_blocking=True)
            aids = aids.to(device, non_blocking=True)
            iids = iids.to(device, non_blocking=True)
            tids = tids.to(device, non_blocking=True)
            cfg_tgt = cfg_tgt.to(device, non_blocking=True)

            logits = model(sids, mids, aids, iids, tids)
            loss = loss_fn(logits, cfg_tgt)

            optimizer.zero_grad(set_to_none=True)
            loss.backward()
            torch.nn.utils.clip_grad_value_(model.parameters(), tc.grad_clip)
            optimizer.step()

            t_loss_acc += loss.detach()
            n_batches += 1

        # Single GPU→CPU sync per epoch
        t_loss = t_loss_acc.item()

        # ── Validate ──
        model.eval()
        v_loss_acc = torch.tensor(0.0, device=device)
        config_acc_sum = bring_acc_sum = lead_acc_sum = 0.0
        n_vbatches = 0

        with torch.no_grad():
            for sids, mids, aids, iids, tids, cfg_tgt, val_tgt in val_loader:
                sids = sids.to(device, non_blocking=True)
                mids = mids.to(device, non_blocking=True)
                aids = aids.to(device, non_blocking=True)
                iids = iids.to(device, non_blocking=True)
                tids = tids.to(device, non_blocking=True)
                cfg_tgt = cfg_tgt.to(device, non_blocking=True)

                logits = model(sids, mids, aids, iids, tids)
                loss = loss_fn(logits, cfg_tgt)

                v_loss_acc += loss.detach()
                n_vbatches += 1

                config_acc_sum += _compute_config_accuracy(logits, cfg_tgt)
                bring_acc_sum += _compute_bring_accuracy(logits, cfg_tgt)
                lead_acc_sum += _compute_lead_accuracy(logits, cfg_tgt)

        # Single GPU→CPU sync for validation losses
        v_loss = v_loss_acc.item()

        if tc.scheduler == 'cosine':
            scheduler.step()
        else:
            scheduler.step(v_loss / max(n_vbatches, 1))

        lr = optimizer.param_groups[0]['lr']
        avg_v = v_loss / max(n_vbatches, 1)

        em = EpochMetrics(
            epoch=epoch + 1,
            train_loss=t_loss / max(n_batches, 1),
            val_loss=avg_v,
            val_config_accuracy=config_acc_sum / max(n_vbatches, 1),
            val_bring_accuracy=bring_acc_sum / max(n_vbatches, 1),
            val_lead_accuracy=lead_acc_sum / max(n_vbatches, 1),
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
    # torch.compile prefixes keys with '_orig_mod.' — strip that prefix
    state_dict = checkpoint['model_state_dict']
    state_dict = {k.removeprefix('_orig_mod.'): v for k, v in state_dict.items()}
    model.load_state_dict(state_dict)
    return model


def _compute_config_accuracy(
    logits: torch.Tensor, targets: torch.Tensor,
) -> float:
    """Exact config match accuracy."""
    preds = logits.argmax(dim=1)
    return (preds == targets).float().mean().item()


def _compute_bring_accuracy(
    logits: torch.Tensor, targets: torch.Tensor,
) -> float:
    """Bring-set accuracy: does the predicted config's bring set match?"""
    preds = logits.argmax(dim=1)
    correct = 0
    total = preds.size(0)
    for i in range(total):
        pred_bring = set(VGC_CONFIGS[preds[i].item()][0])
        true_bring = set(VGC_CONFIGS[targets[i].item()][0])
        if pred_bring == true_bring:
            correct += 1
    return correct / max(total, 1)


def _compute_lead_accuracy(
    logits: torch.Tensor, targets: torch.Tensor,
) -> float:
    """Lead-set accuracy: does the predicted config's lead pair match?"""
    preds = logits.argmax(dim=1)
    correct = 0
    total = preds.size(0)
    for i in range(total):
        pred_lead = set(VGC_CONFIGS[preds[i].item()][1])
        true_lead = set(VGC_CONFIGS[targets[i].item()][1])
        if pred_lead == true_lead:
            correct += 1
    return correct / max(total, 1)
