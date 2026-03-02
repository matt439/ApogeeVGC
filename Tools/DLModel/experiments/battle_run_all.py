"""
BattleNet experiment orchestrator.

Usage:
  python -m experiments.battle_run_all --regulation gen9vgc2025regi
  python -m experiments.battle_run_all --regulation gen9vgc2025regi --stages hparam ablation
  python -m experiments.battle_run_all --regulation gen9vgc2025regi --n-trials 50 --epochs 50

Stages (in order):
  1. hparam    — Optuna hyperparameter search
  2. ablation  — Feature ablation study
  3. baselines — Baseline evaluation
  4. multiseed — Multi-seed evaluation with best config
  5. figures   — Generate thesis figures
"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

import torch
from torch.utils.data import DataLoader

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from dataset import VGCDataset

from .battle_config import BattleExperimentConfig, BattleModelConfig
from .config import TrainConfig, DataConfig
from .data import load_games, create_splits, get_or_build_vocab, make_battle_loaders
from .battle_hparam_search import run_battle_hparam_search
from .battle_ablation import run_battle_ablation
from .battle_baselines import evaluate_random_battle_baseline, evaluate_popular_battle_baseline
from .battle_multiseed import run_battle_multiseed
from .battle_visualise import generate_all_battle_figures


ALL_STAGES = ['hparam', 'ablation', 'baselines', 'multiseed', 'figures']


def main():
    parser = argparse.ArgumentParser(
        description='Run BattleNet experimentation pipeline')
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
        '--min-rating', type=int, default=0,
        help='Minimum player rating filter')
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
    args = parser.parse_args()

    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    print(f'Device: {device}')

    reg = args.regulation
    print(f'\n{"=" * 60}')
    print(f'  BattleNet — Regulation: {reg}')
    print(f'{"=" * 60}')

    strategy = args.training_strategy
    data_config = DataConfig(
        regulation=reg,
        data_root=args.data_root,
        train_frac=args.train_frac,
        val_frac=args.val_frac,
        test_frac=args.test_frac,
        training_strategy=strategy,
    )
    winners_only = data_config.winners_only
    print(f'Training strategy: {strategy}')

    # Results go under battle/ subdirectory to separate from TeamPreview
    results_dir = Path(args.results_root) / reg / 'battle'
    results_dir.mkdir(parents=True, exist_ok=True)

    # ── Load data ──
    data_path = data_config.data_path
    if not data_path.exists():
        print(f'Error: {data_path} not found')
        sys.exit(1)

    print(f'Loading games from {data_path}...')
    games = load_games(data_path, args.min_rating)
    print(f'  {len(games):,} games loaded')

    if len(games) < 100:
        print('Error: too few games for meaningful experiments')
        sys.exit(1)

    # ── Vocab ──
    # Use shared vocab (at regulation level, not under battle/)
    vocab_dir = Path(args.results_root) / reg
    vocab_dir.mkdir(parents=True, exist_ok=True)
    vocab = get_or_build_vocab(data_path, vocab_dir)
    print(f'  Vocab: {vocab["num_species"]} species, '
          f'{vocab.get("num_actions", "?")} actions, '
          f'{vocab["num_moves"]} moves, '
          f'{vocab["num_abilities"]} abilities, '
          f'{vocab["num_items"]} items, '
          f'{vocab["num_tera_types"]} tera types')

    # ── Splits ──
    # Use shared splits (at regulation level)
    train_games, val_games, test_games = create_splits(
        games,
        train_frac=args.train_frac,
        val_frac=args.val_frac,
        test_frac=args.test_frac,
        seed=42,
        output_dir=vocab_dir,
    )
    print(f'  Split: {len(train_games):,} train, '
          f'{len(val_games):,} val, {len(test_games):,} test')

    # ── Base config ──
    base_config = BattleExperimentConfig(
        name=f'{reg}_battle_base',
        model=BattleModelConfig(),
        train=TrainConfig(
            epochs=args.epochs,
            patience=args.patience,
            min_rating=args.min_rating,
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

        test_ds = VGCDataset(test_games, vocab, winners_only=winners_only)
        test_loader = DataLoader(
            test_ds, batch_size=1024, shuffle=False, num_workers=0)
        train_ds = VGCDataset(train_games, vocab, winners_only=winners_only)
        train_loader = DataLoader(
            train_ds, batch_size=1024, shuffle=False, num_workers=0)

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

    print('\nDone.')


if __name__ == '__main__':
    main()
