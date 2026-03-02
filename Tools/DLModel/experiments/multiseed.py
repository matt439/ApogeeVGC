"""
Multi-seed evaluation.

Trains the best configuration across multiple random seeds
and reports mean +/- std of all metrics on the held-out test set.
"""

from __future__ import annotations

import json
from dataclasses import dataclass
from pathlib import Path

import numpy as np
import torch
from torch.utils.data import DataLoader

import sys
sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from team_preview_dataset import TeamPreviewDataset

from .config import ExperimentConfig, TrainConfig, MULTI_SEED_SEEDS
from .training import train_model, load_model_from_checkpoint
from .metrics import evaluate_comprehensive, ComprehensiveMetrics
from .data import make_loaders


def run_multiseed(
    best_config: ExperimentConfig,
    train_games: list[dict],
    val_games: list[dict],
    test_games: list[dict],
    vocab: dict,
    output_dir: Path,
    seeds: list[int] | None = None,
    device: torch.device | None = None,
) -> dict:
    """Train best config across multiple seeds, evaluate on test set.

    Returns summary with mean +/- std for all metrics.
    """
    if device is None:
        device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    if seeds is None:
        seeds = MULTI_SEED_SEEDS

    multiseed_dir = output_dir / 'multiseed'
    all_metrics: list[ComprehensiveMetrics] = []

    for seed in seeds:
        print(f'\n=== Seed: {seed} ===')

        train_cfg = TrainConfig(
            lr=best_config.train.lr,
            weight_decay=best_config.train.weight_decay,
            batch_size=best_config.train.batch_size,
            epochs=best_config.train.epochs,
            patience=best_config.train.patience,
            grad_clip=best_config.train.grad_clip,
            scheduler=best_config.train.scheduler,
            seed=seed,
            min_rating=best_config.train.min_rating,
        )

        config = ExperimentConfig(
            name=f'seed_{seed}',
            model=best_config.model,
            train=train_cfg,
            data=best_config.data,
        )

        seed_dir = multiseed_dir / f'seed_{seed}'

        train_loader, val_loader = make_loaders(
            train_games, val_games, vocab,
            config.train.batch_size, device,
            winners_only=config.data.winners_only)

        result = train_model(
            config=config,
            train_loader=train_loader,
            val_loader=val_loader,
            vocab=vocab,
            output_dir=seed_dir,
            device=device,
        )

        # Evaluate on test set
        test_ds = TeamPreviewDataset(test_games, vocab, winners_only=config.data.winners_only)
        test_loader = DataLoader(
            test_ds, batch_size=config.train.batch_size,
            shuffle=False, num_workers=0)

        checkpoint = torch.load(
            seed_dir / 'model.pt', map_location=device, weights_only=False)
        model = load_model_from_checkpoint(checkpoint, vocab, device)

        test_metrics = evaluate_comprehensive(model, test_loader, device)
        all_metrics.append(test_metrics)

        with open(seed_dir / 'test_metrics.json', 'w') as f:
            json.dump(test_metrics.to_dict(), f, indent=2)

        # Save training log for learning curve plots
        with open(seed_dir / 'training_log.json', 'w') as f:
            json.dump({
                'epoch_metrics': [
                    {
                        'epoch': em.epoch,
                        'train_loss': em.train_loss,
                        'train_bring_loss': em.train_bring_loss,
                        'train_lead_loss': em.train_lead_loss,
                        'val_loss': em.val_loss,
                        'val_bring_loss': em.val_bring_loss,
                        'val_lead_loss': em.val_lead_loss,
                        'val_bring_acc_top4': em.val_bring_acc_top4,
                        'val_lead_acc_top2': em.val_lead_acc_top2,
                        'lr': em.lr,
                        'elapsed_sec': em.elapsed_sec,
                    }
                    for em in result.epoch_metrics
                ],
                'best_epoch': result.best_epoch,
                'best_val_loss': result.best_val_loss,
            }, f, indent=2)

        print(f'  bring_set={test_metrics.bring_set_accuracy:.3f} '
              f'lead_set={test_metrics.lead_set_accuracy:.3f} '
              f'val_loss={result.best_val_loss:.4f}')

    summary = _compute_summary(all_metrics, seeds)

    with open(multiseed_dir / 'summary.json', 'w') as f:
        json.dump(summary, f, indent=2)

    return summary


def _compute_summary(
    metrics_list: list[ComprehensiveMetrics],
    seeds: list[int],
) -> dict:
    """Compute mean +/- std across seeds for all scalar metrics."""
    summary: dict = {'seeds': seeds, 'n_seeds': len(seeds)}

    scalar_fields = [
        'bring_set_accuracy', 'bring_hamming_accuracy', 'bring_overlap_accuracy',
        'bring_macro_f1', 'bring_ece',
        'lead_set_accuracy', 'lead_hamming_accuracy', 'lead_overlap_accuracy',
        'lead_macro_f1', 'lead_ece',
        'total_loss', 'bring_loss', 'lead_loss',
    ]

    for field_name in scalar_fields:
        values = [getattr(m, field_name) for m in metrics_list]
        summary[field_name] = {
            'mean': float(np.mean(values)),
            'std': float(np.std(values)),
            'min': float(np.min(values)),
            'max': float(np.max(values)),
            'values': [float(v) for v in values],
        }

    return summary
