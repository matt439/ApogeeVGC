"""
BattleNet ablation study runner.

Trains the model with progressively more feature groups enabled:
1. Species only
2. Species + moves
3. Species + moves + abilities
4. Species + moves + abilities + items
5. Full (all features)

Each variant uses the best hyperparameters from the search,
with only feature_flags changed.
"""

from __future__ import annotations

import json
from pathlib import Path

import torch
from torch.utils.data import DataLoader

import sys
sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from .battle_config import BattleExperimentConfig, BattleModelConfig, BATTLE_ABLATION_CONFIGS
from .battle_training import train_battle_model, load_battle_model_from_checkpoint
from .battle_metrics import evaluate_battle_comprehensive
from .data import build_battle_datasets, build_battle_test_dataset, loaders_from_datasets


def run_battle_ablation(
    best_config: BattleExperimentConfig,
    train_games: list[dict],
    val_games: list[dict],
    test_games: list[dict],
    vocab: dict,
    output_dir: Path,
    device: torch.device | None = None,
) -> dict[str, dict]:
    """Run the full BattleNet ablation study.

    Uses best_config as the base, overriding only feature_flags.
    Returns dict mapping ablation name -> metrics dict.
    """
    if device is None:
        device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')

    ablation_dir = output_dir / 'ablation'
    results = {}

    # Build datasets once — reuse across all ablation variants
    winners_only = best_config.data.winners_only
    print('  Building datasets for ablation...')
    train_ds, val_ds = build_battle_datasets(
        train_games, val_games, vocab, winners_only=winners_only,
        cache_dir=output_dir)
    test_ds = build_battle_test_dataset(
        test_games, vocab, winners_only=winners_only, cache_dir=output_dir)
    if device.type == 'cuda':
        test_ds.to(device)
    print(f'  {len(train_ds):,} train, {len(val_ds):,} val, {len(test_ds):,} test')

    for ablation in BATTLE_ABLATION_CONFIGS:
        name = ablation['name']
        flags = ablation['flags']
        print(f'\n=== Ablation: {name} ===')

        model_cfg = BattleModelConfig(
            embed_dim=best_config.model.embed_dim,
            feat_embed_dim=best_config.model.feat_embed_dim,
            pokemon_dim=best_config.model.pokemon_dim,
            hidden_dim=best_config.model.hidden_dim,
            num_trunk_layers=best_config.model.num_trunk_layers,
            trunk_dropout=best_config.model.trunk_dropout,
            head_dim=best_config.model.head_dim,
            feature_flags=flags,
        )

        config = BattleExperimentConfig(
            name=f'ablation_{name}',
            model=model_cfg,
            train=best_config.train,
            data=best_config.data,
        )

        variant_dir = ablation_dir / name

        train_loader, val_loader = loaders_from_datasets(
            train_ds, val_ds, config.train.batch_size, device)

        result = train_battle_model(
            config=config,
            train_loader=train_loader,
            val_loader=val_loader,
            vocab=vocab,
            output_dir=variant_dir,
            device=device,
        )

        # Evaluate on test set (tensors already on GPU if CUDA)
        test_loader = DataLoader(
            test_ds, batch_size=config.train.batch_size,
            shuffle=False, num_workers=0, pin_memory=False)

        checkpoint = torch.load(
            variant_dir / 'model.pt', map_location=device, weights_only=False)
        model = load_battle_model_from_checkpoint(checkpoint, vocab, device)

        test_metrics = evaluate_battle_comprehensive(model, test_loader, device)

        metrics_dict = {
            'training': {
                'best_val_loss': result.best_val_loss,
                'best_epoch': result.best_epoch,
                'total_params': result.total_params,
            },
            'test': test_metrics.to_dict(),
        }

        with open(variant_dir / 'metrics.json', 'w') as f:
            json.dump(metrics_dict, f, indent=2)

        results[name] = metrics_dict
        print(f'  value_acc={test_metrics.value_accuracy:.3f} '
              f'policy_top1={test_metrics.policy_combined_top1_accuracy:.3f} '
              f'params={result.total_params:,}')

    with open(ablation_dir / 'summary.json', 'w') as f:
        json.dump(results, f, indent=2)

    return results
