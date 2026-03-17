"""
BattleNet experiment orchestrator.

Runs the full BattleNet pipeline across all rating tiers (all, 1200+, 1500+)
for rigorous comparison, then performs cross-tier statistical analysis.

Usage:
  python -m experiments.battle_run_all --regulation gen9vgc2025regi
  python -m experiments.battle_run_all --regulation gen9vgc2025regi --stages hparam ablation
  python -m experiments.battle_run_all --regulation gen9vgc2025regi --n-trials 50 --epochs 50

Stages (in order, run per tier):
  1. hparam    — Optuna hyperparameter search
  2. ablation  — Feature ablation study
  3. baselines — Baseline evaluation
  4. multiseed — Multi-seed evaluation with best config
  5. figures   — Generate thesis figures

After all tiers, a cross-tier statistical comparison is generated.
"""

from __future__ import annotations

import argparse
import json
import shutil
import sys
from pathlib import Path

import torch

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

from .battle_config import BattleExperimentConfig, BattleModelConfig
from .config import (
    TrainConfig, DataConfig, RATING_TIERS, tier_data_filename,
)
from .data import (
    load_games, create_splits, get_or_build_vocab,
    build_battle_datasets, build_battle_test_dataset,
    loaders_from_datasets, make_batch_iter,
)
from .battle_hparam_search import run_battle_hparam_search
from .battle_ablation import run_battle_ablation
from .battle_baselines import evaluate_random_battle_baseline, evaluate_popular_battle_baseline
from .battle_multiseed import run_battle_multiseed
from .battle_visualise import generate_all_battle_figures
from .rating_comparison import run_rating_comparison


ALL_STAGES = ['hparam', 'ablation', 'baselines', 'multiseed', 'figures']

BATTLE_COMPARISON_METRICS = [
    'value_accuracy', 'value_ece',
    'policy_combined_top1_accuracy',
    'policy_a_top1_accuracy', 'policy_b_top1_accuracy',
]


def run_tier(
    tier_name: str,
    min_rating: int,
    reg: str,
    args,
    device: torch.device,
    results_root: Path,
    data_root: str,
    strategy: str,
    shared_vocab: dict | None = None,
) -> dict | None:
    """Run all requested stages for a single rating tier.

    Returns the vocab (built from this tier's data or shared).
    """
    print(f'\n{"=" * 60}')
    print(f'  BattleNet — Rating Tier: {tier_name} (min rating: {min_rating})')
    print(f'{"=" * 60}')

    data_config = DataConfig(
        regulation=reg,
        data_root=data_root,
        train_frac=args.train_frac,
        val_frac=args.val_frac,
        test_frac=args.test_frac,
        training_strategy=strategy,
        rating_tier=tier_name,
    )
    winners_only = data_config.winners_only

    results_dir = results_root / tier_name
    results_dir.mkdir(parents=True, exist_ok=True)

    # ── Load data ──
    data_path = data_config.data_path
    if not data_path.exists():
        print(f'Error: {data_path} not found — '
              f'run parser.py first to generate tier files')
        return None

    print(f'Loading games from {data_path}...')
    games = load_games(data_path)
    print(f'  {len(games):,} games loaded')

    if len(games) < 100:
        print(f'Warning: too few games ({len(games)}) for tier {tier_name}, '
              f'skipping')
        return None

    # ── Vocab (shared from "all" tier, or build fresh) ──
    if shared_vocab is not None:
        vocab = shared_vocab
    else:
        vocab = get_or_build_vocab(data_path, results_dir)
    print(f'  Vocab: {vocab["num_species"]} species, '
          f'{vocab.get("num_actions", "?")} actions, '
          f'{vocab["num_moves"]} moves, '
          f'{vocab["num_abilities"]} abilities, '
          f'{vocab["num_items"]} items, '
          f'{vocab["num_tera_types"]} tera types')

    # ── Splits ──
    train_games, val_games, test_games = create_splits(
        games,
        train_frac=args.train_frac,
        val_frac=args.val_frac,
        test_frac=args.test_frac,
        seed=42,
        output_dir=results_dir,
    )
    print(f'  Split: {len(train_games):,} train, '
          f'{len(val_games):,} val, {len(test_games):,} test')

    # ── Base config ──
    base_config = BattleExperimentConfig(
        name=f'{reg}_{tier_name}_battle_base',
        model=BattleModelConfig(),
        train=TrainConfig(
            epochs=args.epochs,
            patience=args.patience,
        ),
        data=data_config,
    )

    # ── Stage 1: Hyperparameter search ──
    if 'hparam' in args.stages:
        print('\n--- Stage 1: Hyperparameter Search ---')
        study = run_battle_hparam_search(
            base_config=base_config,
            train_games=train_games,
            val_games=val_games,
            vocab=vocab,
            output_dir=results_dir,
            n_trials=args.n_trials,
            timeout_hours=args.timeout_hours,
            device=device,
        )
        print(f'  Best val loss: {study.best_value:.4f}')
        print(f'  Best params: {study.best_params}')

    # ── Load best config ──
    best_config_path = results_dir / 'hparam_search' / 'best_config.json'
    if best_config_path.exists():
        best_config = BattleExperimentConfig.from_json(best_config_path)
    else:
        print('  Warning: No hparam search results found, using defaults')
        best_config = base_config

    # ── Stage 2: Ablation study ──
    if 'ablation' in args.stages:
        print('\n--- Stage 2: Feature Ablation ---')
        ablation_results = run_battle_ablation(
            best_config=best_config,
            train_games=train_games,
            val_games=val_games,
            test_games=test_games,
            vocab=vocab,
            output_dir=results_dir,
            device=device,
        )
        for name, metrics in ablation_results.items():
            print(f'  {name}: '
                  f'value_acc={metrics["test"]["value_accuracy"]:.3f} '
                  f'policy_top1={metrics["test"]["policy_combined_top1_accuracy"]:.3f}')

    # ── Stage 3: Baselines ──
    if 'baselines' in args.stages:
        print('\n--- Stage 3: Baselines ---')
        baselines_dir = results_dir / 'baselines'
        baselines_dir.mkdir(parents=True, exist_ok=True)

        test_ds = build_battle_test_dataset(
            test_games, vocab, winners_only=winners_only, cache_dir=results_dir)
        test_loader = make_batch_iter(test_ds, 1024, device)
        train_ds, _ = build_battle_datasets(
            train_games, val_games, vocab, winners_only=winners_only,
            cache_dir=results_dir)
        train_loader = make_batch_iter(train_ds, 1024, device)

        random_m = evaluate_random_battle_baseline(test_loader, vocab)
        with open(baselines_dir / 'random_metrics.json', 'w') as f:
            json.dump(random_m.to_dict(), f, indent=2)
        print(f'  Random: value_acc={random_m.value_accuracy:.3f} '
              f'policy_top1={random_m.policy_combined_top1_accuracy:.3f}')

        popular_m = evaluate_popular_battle_baseline(
            train_loader, test_loader, vocab)
        with open(baselines_dir / 'popular_metrics.json', 'w') as f:
            json.dump(popular_m.to_dict(), f, indent=2)
        print(f'  Popular: value_acc={popular_m.value_accuracy:.3f} '
              f'policy_top1={popular_m.policy_combined_top1_accuracy:.3f}')

    # ── Stage 4: Multi-seed evaluation ──
    if 'multiseed' in args.stages:
        print('\n--- Stage 4: Multi-Seed Evaluation ---')
        summary = run_battle_multiseed(
            best_config=best_config,
            train_games=train_games,
            val_games=val_games,
            test_games=test_games,
            vocab=vocab,
            output_dir=results_dir,
            device=device,
        )
        for metric in ['value_accuracy', 'policy_combined_top1_accuracy',
                       'policy_a_top1_accuracy', 'policy_b_top1_accuracy']:
            m = summary[metric]
            print(f'  {metric}: {m["mean"]:.3f} +/- {m["std"]:.3f}')

    # ── Stage 5: Generate figures ──
    if 'figures' in args.stages:
        print('\n--- Stage 5: Generating Figures ---')
        generate_all_battle_figures(results_dir)
        print(f'  Figures saved to {results_dir / "figures"}')

    return vocab


def main():
    parser = argparse.ArgumentParser(
        description='Run BattleNet experimentation pipeline '
                    'across rating tiers')
    parser.add_argument(
        '--regulation', required=True,
        help='Regulation name (e.g., gen9vgc2025regi)')
    parser.add_argument(
        '--stages', nargs='+', default=ALL_STAGES,
        choices=ALL_STAGES,
        help='Which stages to run (default: all)')
    parser.add_argument(
        '--data-root', default='../ReplayScraper/data',
        help='Root directory for parsed data')
    parser.add_argument(
        '--results-root', default='results',
        help='Root directory for experiment results')
    parser.add_argument(
        '--n-trials', type=int, default=100,
        help='Number of Optuna trials')
    parser.add_argument(
        '--timeout-hours', type=float, default=12.0,
        help='Max wall-clock time for hparam search (hours)')
    parser.add_argument(
        '--epochs', type=int, default=50,
        help='Max training epochs per run')
    parser.add_argument(
        '--patience', type=int, default=7,
        help='Early stopping patience')
    parser.add_argument(
        '--train-frac', type=float, default=0.7,
        help='Training set fraction')
    parser.add_argument(
        '--val-frac', type=float, default=0.15,
        help='Validation set fraction')
    parser.add_argument(
        '--test-frac', type=float, default=0.15,
        help='Test set fraction')
    parser.add_argument(
        '--training-strategy', default='all_games',
        choices=['winners_only', 'all_games'],
        help='Training data strategy: all_games (default) trains on '
             'both perspectives; winners_only trains on winning player only')
    parser.add_argument(
        '--tiers', nargs='+', default=None,
        help='Rating tiers to run (default: all). '
             'Options: ' + ', '.join(t[0] for t in RATING_TIERS))
    parser.add_argument(
        '--clean', action='store_true',
        help='Delete existing results before starting (fresh run)')
    args = parser.parse_args()

    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    print(f'Device: {device}')
    if device.type == 'cuda':
        print(f'  GPU: {torch.cuda.get_device_name(0)}')
        mem = torch.cuda.get_device_properties(0).total_memory / 1024**3
        print(f'  VRAM: {mem:.1f} GB')
        print(f'  CUDA: {torch.version.cuda}  |  cuDNN: {torch.backends.cudnn.version()}')

    reg = args.regulation
    strategy = args.training_strategy

    # Battle results go under battle/ subdirectory
    results_root = Path(args.results_root) / reg / 'battle'
    results_root.mkdir(parents=True, exist_ok=True)

    # Determine which tiers to run
    if args.tiers:
        valid_tier_names = {t[0] for t in RATING_TIERS}
        tiers = [(n, r) for n, r in RATING_TIERS if n in args.tiers]
        unknown = set(args.tiers) - valid_tier_names
        if unknown:
            print(f'Warning: unknown tiers ignored: {unknown}')
    else:
        tiers = RATING_TIERS

    # Clean old results if requested
    if args.clean:
        for tier_name, _ in tiers:
            tier_dir = results_root / tier_name
            if tier_dir.exists():
                print(f'Cleaning {tier_dir}...')
                shutil.rmtree(tier_dir)
        # Also clean cross-tier comparison
        comparison_dir = results_root / 'rating_comparison'
        if comparison_dir.exists():
            print(f'Cleaning {comparison_dir}...')
            shutil.rmtree(comparison_dir)

    print(f'\nRegulation: {reg}')
    print(f'Training strategy: {strategy}')
    print(f'Rating tiers: {[t[0] for t in tiers]}')

    # ── Run each tier ──
    shared_vocab = None
    tier_dirs: dict[str, Path] = {}

    for tier_name, min_rating in tiers:
        vocab = run_tier(
            tier_name=tier_name,
            min_rating=min_rating,
            reg=reg,
            args=args,
            device=device,
            results_root=results_root,
            data_root=args.data_root,
            strategy=strategy,
            shared_vocab=shared_vocab,
        )
        if vocab is not None:
            tier_dirs[tier_name] = results_root / tier_name
            if shared_vocab is None:
                shared_vocab = vocab

    # ── Cross-tier statistical comparison ──
    if len(tier_dirs) >= 2 and 'multiseed' in args.stages:
        print(f'\n{"=" * 60}')
        print(f'  Cross-Tier Rating Comparison')
        print(f'{"=" * 60}')
        run_rating_comparison(
            results_root=results_root,
            tier_dirs=tier_dirs,
            metrics=BATTLE_COMPARISON_METRICS,
            model_name='BattleNet',
        )

    print('\nDone.')


if __name__ == '__main__':
    main()
