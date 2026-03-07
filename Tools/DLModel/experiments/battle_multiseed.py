"""
BattleNet multi-seed evaluation.

Trains the best configuration across multiple random seeds
and reports mean +/- std of all metrics on the held-out test set.
"""

from __future__ import annotations

import json
from pathlib import Path

import numpy as np
import torch

import sys
sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from .battle_config import BattleExperimentConfig, BATTLE_MULTI_SEED_SEEDS
from .config import TrainConfig
from .battle_training import train_battle_model, load_battle_model_from_checkpoint
from .battle_metrics import evaluate_battle_comprehensive, BattleComprehensiveMetrics
from .data import (
    build_battle_datasets, build_battle_test_dataset,
    loaders_from_datasets, make_batch_iter,
)


def run_battle_multiseed(
    best_config: BattleExperimentConfig,
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
        seeds = BATTLE_MULTI_SEED_SEEDS

    multiseed_dir = output_dir / 'multiseed'
    all_metrics: list[BattleComprehensiveMetrics] = []

    # Build datasets once — reuse across all seeds
    winners_only = best_config.data.winners_only
    print('  Building datasets for multiseed...')
    train_ds, val_ds = build_battle_datasets(
        train_games, val_games, vocab, winners_only=winners_only,
        cache_dir=output_dir)
    test_ds = build_battle_test_dataset(
        test_games, vocab, winners_only=winners_only, cache_dir=output_dir)
    print(f'  {len(train_ds):,} train, {len(val_ds):,} val, {len(test_ds):,} test')

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

        config = BattleExperimentConfig(
            name=f'seed_{seed}',
            model=best_config.model,
            train=train_cfg,
            data=best_config.data,
        )

        seed_dir = multiseed_dir / f'seed_{seed}'

        train_loader, val_loader = loaders_from_datasets(
            train_ds, val_ds, config.train.batch_size, device)

        result = train_battle_model(
            config=config,
            train_loader=train_loader,
            val_loader=val_loader,
            vocab=vocab,
            output_dir=seed_dir,
            device=device,
        )

        # Evaluate on test set (tensors already on GPU if CUDA)
        test_loader = make_batch_iter(
            test_ds, config.train.batch_size, device)

        checkpoint = torch.load(
            seed_dir / 'model.pt', map_location=device, weights_only=False)
        model = load_battle_model_from_checkpoint(checkpoint, vocab, device)

        test_metrics = evaluate_battle_comprehensive(model, test_loader, device)
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
                        'train_value_loss': em.train_value_loss,
                        'train_policy_a_loss': em.train_policy_a_loss,
                        'train_policy_b_loss': em.train_policy_b_loss,
                        'val_loss': em.val_loss,
                        'val_value_loss': em.val_value_loss,
                        'val_policy_a_loss': em.val_policy_a_loss,
                        'val_policy_b_loss': em.val_policy_b_loss,
                        'val_value_accuracy': em.val_value_accuracy,
                        'val_policy_a_top1_accuracy': em.val_policy_a_top1_accuracy,
                        'val_policy_b_top1_accuracy': em.val_policy_b_top1_accuracy,
                        'lr': em.lr,
                        'elapsed_sec': em.elapsed_sec,
                    }
                    for em in result.epoch_metrics
                ],
                'best_epoch': result.best_epoch,
                'best_val_loss': result.best_val_loss,
            }, f, indent=2)

        print(f'  value_acc={test_metrics.value_accuracy:.3f} '
              f'policy_top1={test_metrics.policy_combined_top1_accuracy:.3f} '
              f'val_loss={result.best_val_loss:.4f}')

    summary = _compute_battle_summary(all_metrics, seeds)

    with open(multiseed_dir / 'summary.json', 'w') as f:
        json.dump(summary, f, indent=2)

    return summary


def _compute_battle_summary(
    metrics_list: list[BattleComprehensiveMetrics],
    seeds: list[int],
) -> dict:
    """Compute mean +/- std across seeds for all scalar metrics."""
    summary: dict = {'seeds': seeds, 'n_seeds': len(seeds)}

    scalar_fields = [
        'value_accuracy', 'value_ece',
        'policy_a_top1_accuracy', 'policy_b_top1_accuracy',
        'policy_combined_top1_accuracy',
        'policy_a_top3_accuracy', 'policy_b_top3_accuracy',
        'total_loss', 'value_loss', 'policy_a_loss', 'policy_b_loss',
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
