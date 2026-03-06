"""
Core training function for BattleNet experiments.

Does not parse command-line arguments. Accepts configuration objects directly.
Returns a structured BattleTrainResult with all metrics per epoch.
"""

from __future__ import annotations

import time
from dataclasses import dataclass
from pathlib import Path

import torch
import torch.nn as nn
from torch.amp import autocast, GradScaler
from torch.utils.data import DataLoader

import sys
sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from model import BattleNetV2

from .battle_config import BattleExperimentConfig


@dataclass
class BattleEpochMetrics:
    """Metrics recorded for a single BattleNet training epoch."""
    epoch: int
    train_loss: float
    train_value_loss: float
    train_policy_a_loss: float
    train_policy_b_loss: float
    val_loss: float
    val_value_loss: float
    val_policy_a_loss: float
    val_policy_b_loss: float
    val_value_accuracy: float
    val_policy_a_top1_accuracy: float
    val_policy_b_top1_accuracy: float
    lr: float
    elapsed_sec: float


@dataclass
class BattleTrainResult:
    """Complete BattleNet training result."""
    config: dict
    epoch_metrics: list[BattleEpochMetrics]
    best_epoch: int
    best_val_loss: float
    best_model_path: str
    total_params: int
    total_time_sec: float


def train_battle_model(
    config: BattleExperimentConfig,
    train_loader: DataLoader,
    val_loader: DataLoader,
    vocab: dict,
    output_dir: Path,
    device: torch.device | None = None,
    trial=None,
) -> BattleTrainResult:
    """Train a BattleNetV2 model from a config.

    Parameters
    ----------
    config : BattleExperimentConfig
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

    model = BattleNetV2(
        num_species=vocab['num_species'],
        num_actions=vocab['num_actions'],
        num_moves=vocab['num_moves'],
        num_abilities=vocab['num_abilities'],
        num_items=vocab['num_items'],
        num_tera_types=vocab['num_tera_types'],
        embed_dim=mc.embed_dim,
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
    # Default inductor backend requires Triton (Linux-only)
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

    value_loss_fn = nn.BCELoss()
    policy_loss_fn = nn.CrossEntropyLoss(ignore_index=0)

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
        # Accumulate losses on GPU to avoid per-batch .item() sync
        t_loss_acc = torch.tensor(0.0, device=device)
        t_vloss_acc = torch.tensor(0.0, device=device)
        t_paloss_acc = torch.tensor(0.0, device=device)
        t_pbloss_acc = torch.tensor(0.0, device=device)
        n_batches = 0

        for sids, mids, aids, iids, tids, num, vtgt, pa_tgt, pb_tgt in train_loader:
            sids = sids.to(device, non_blocking=True)
            mids = mids.to(device, non_blocking=True)
            aids = aids.to(device, non_blocking=True)
            iids = iids.to(device, non_blocking=True)
            tids = tids.to(device, non_blocking=True)
            num = num.to(device, non_blocking=True)
            vtgt = vtgt.to(device, non_blocking=True)
            pa_tgt = pa_tgt.to(device, non_blocking=True)
            pb_tgt = pb_tgt.to(device, non_blocking=True)

            with autocast('cuda', enabled=use_amp):
                value, pol_a, pol_b = model(sids, mids, aids, iids, tids, num)

            # BCELoss requires float32 — compute loss outside autocast
            v_loss = value_loss_fn(value.float(), vtgt)
            pa_loss = policy_loss_fn(pol_a.float(), pa_tgt)
            pb_loss = policy_loss_fn(pol_b.float(), pb_tgt)
            loss = v_loss + pa_loss + pb_loss

            optimizer.zero_grad()
            scaler.scale(loss).backward()
            scaler.unscale_(optimizer)
            torch.nn.utils.clip_grad_norm_(model.parameters(), tc.grad_clip)
            scaler.step(optimizer)
            scaler.update()

            t_loss_acc += loss.detach()
            t_vloss_acc += v_loss.detach()
            t_paloss_acc += pa_loss.detach()
            t_pbloss_acc += pb_loss.detach()
            n_batches += 1

        # Single GPU→CPU sync per epoch
        t_loss = t_loss_acc.item()
        t_vloss = t_vloss_acc.item()
        t_paloss = t_paloss_acc.item()
        t_pbloss = t_pbloss_acc.item()

        # ── Validate ──
        model.eval()
        v_loss_acc = torch.tensor(0.0, device=device)
        v_vloss_acc = torch.tensor(0.0, device=device)
        v_paloss_acc = torch.tensor(0.0, device=device)
        v_pbloss_acc = torch.tensor(0.0, device=device)
        val_acc_acc = torch.tensor(0.0, device=device)
        pa_correct_acc = torch.tensor(0, dtype=torch.long, device=device)
        pa_total_acc = torch.tensor(0, dtype=torch.long, device=device)
        pb_correct_acc = torch.tensor(0, dtype=torch.long, device=device)
        pb_total_acc = torch.tensor(0, dtype=torch.long, device=device)
        n_vbatches = 0

        with torch.no_grad():
            for sids, mids, aids, iids, tids, num, vtgt, pa_tgt, pb_tgt in val_loader:
                sids = sids.to(device, non_blocking=True)
                mids = mids.to(device, non_blocking=True)
                aids = aids.to(device, non_blocking=True)
                iids = iids.to(device, non_blocking=True)
                tids = tids.to(device, non_blocking=True)
                num = num.to(device, non_blocking=True)
                vtgt = vtgt.to(device, non_blocking=True)
                pa_tgt = pa_tgt.to(device, non_blocking=True)
                pb_tgt = pb_tgt.to(device, non_blocking=True)

                with autocast('cuda', enabled=use_amp):
                    value, pol_a, pol_b = model(sids, mids, aids, iids, tids, num)

                vl = value_loss_fn(value.float(), vtgt)
                pal = policy_loss_fn(pol_a.float(), pa_tgt)
                pbl = policy_loss_fn(pol_b.float(), pb_tgt)

                v_loss_acc += (vl + pal + pbl).detach()
                v_vloss_acc += vl.detach()
                v_paloss_acc += pal.detach()
                v_pbloss_acc += pbl.detach()
                n_vbatches += 1

                # Value accuracy
                val_acc_acc += ((value > 0.5).float() == vtgt).float().mean()

                # Policy accuracy (non-padded only)
                mask_a = pa_tgt > 0
                if mask_a.any():
                    pa_correct_acc += (pol_a.argmax(1)[mask_a] == pa_tgt[mask_a]).sum()
                    pa_total_acc += mask_a.sum()

                mask_b = pb_tgt > 0
                if mask_b.any():
                    pb_correct_acc += (pol_b.argmax(1)[mask_b] == pb_tgt[mask_b]).sum()
                    pb_total_acc += mask_b.sum()

        # Single GPU→CPU sync for validation
        v_loss_sum = v_loss_acc.item()
        v_vloss = v_vloss_acc.item()
        v_paloss = v_paloss_acc.item()
        v_pbloss = v_pbloss_acc.item()
        val_acc_sum = val_acc_acc.item()
        pa_correct = pa_correct_acc.item()
        pa_total = pa_total_acc.item()
        pb_correct = pb_correct_acc.item()
        pb_total = pb_total_acc.item()

        if tc.scheduler == 'cosine':
            scheduler.step()
        else:
            scheduler.step(v_loss_sum / max(n_vbatches, 1))

        lr = optimizer.param_groups[0]['lr']
        avg_v = v_loss_sum / max(n_vbatches, 1)

        em = BattleEpochMetrics(
            epoch=epoch + 1,
            train_loss=t_loss / max(n_batches, 1),
            train_value_loss=t_vloss / max(n_batches, 1),
            train_policy_a_loss=t_paloss / max(n_batches, 1),
            train_policy_b_loss=t_pbloss / max(n_batches, 1),
            val_loss=avg_v,
            val_value_loss=v_vloss / max(n_vbatches, 1),
            val_policy_a_loss=v_paloss / max(n_vbatches, 1),
            val_policy_b_loss=v_pbloss / max(n_vbatches, 1),
            val_value_accuracy=val_acc_sum / max(n_vbatches, 1),
            val_policy_a_top1_accuracy=pa_correct / max(pa_total, 1),
            val_policy_b_top1_accuracy=pb_correct / max(pb_total, 1),
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
                'model_type': 'battle',
                'args': {
                    'embed_dim': mc.embed_dim,
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

    return BattleTrainResult(
        config=config.to_dict(),
        epoch_metrics=epoch_metrics_list,
        best_epoch=best_epoch,
        best_val_loss=best_val_loss,
        best_model_path=str(model_path),
        total_params=total_params,
        total_time_sec=total_time,
    )


def load_battle_model_from_checkpoint(
    checkpoint: dict,
    vocab: dict,
    device: torch.device,
) -> BattleNetV2:
    """Instantiate and load a BattleNetV2 from a checkpoint dict."""
    args = checkpoint['args']
    model = BattleNetV2(
        num_species=vocab['num_species'],
        num_actions=vocab['num_actions'],
        num_moves=vocab['num_moves'],
        num_abilities=vocab['num_abilities'],
        num_items=vocab['num_items'],
        num_tera_types=vocab['num_tera_types'],
        embed_dim=args['embed_dim'],
        feat_embed_dim=args['feat_embed_dim'],
        pokemon_dim=args['pokemon_dim'],
        hidden_dim=args['hidden_dim'],
        num_trunk_layers=args.get('num_trunk_layers', 3),
        trunk_dropout=args.get('trunk_dropout', 0.3),
        head_dim=args.get('head_dim', 64),
        feature_flags=args.get('feature_flags'),
    ).to(device)
    state_dict = {k.removeprefix('_orig_mod.'): v for k, v in checkpoint['model_state_dict'].items()}
    model.load_state_dict(state_dict)
    return model
