"""
Main experiment orchestrator.

Usage:
  python -m experiments.run_all --regulation gen9vgc2025regi
  python -m experiments.run_all --regulation gen9vgc2025regi --stages hparam ablation
  python -m experiments.run_all --regulation gen9vgc2025regi --n-trials 50 --epochs 50

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
from team_preview_dataset import TeamPreviewDataset

from .config import ExperimentConfig, DataConfig, ModelConfig, TrainConfig
from .data import load_games, create_splits, get_or_build_vocab, make_loaders
from .hparam_search import run_hparam_search
from .ablation import run_ablation
from .baselines import evaluate_random_baseline, evaluate_popular_baseline
from .multiseed import run_multiseed
from .visualise import generate_all_figures


ALL_STAGES = ['hparam', 'ablation', 'baselines', 'multiseed', 'figures']


def main():
    parser = argparse.ArgumentParser(
        description='Run TeamPreviewNet experimentation pipeline')
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
        '--training-strategy', default='winners_only',
        choices=['winners_only', 'all_games'],
        help='Training data strategy: winners_only (default) trains on '
             'winning players only; all_games trains on both perspectives')
    args = parser.parse_args()

    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    print(f'Device: {device}')

    reg = args.regulation
    print(f'\n{"=" * 60}')
    print(f'  Regulation: {reg}')
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

    results_dir = Path(args.results_root) / reg
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
    vocab = get_or_build_vocab(data_path, results_dir)
    print(f'  Vocab: {vocab["num_species"]} species, '
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
    base_config = ExperimentConfig(
        name=f'{reg}_base',
        model=ModelConfig(),
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
        study = run_hparam_search(
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
        best_config = ExperimentConfig.from_json(best_config_path)
    else:
        print('  Warning: No hparam search results found, using defaults')
        best_config = base_config

    # ── Stage 2: Ablation study ──
    if 'ablation' in args.stages:
        print('\n--- Stage 2: Feature Ablation ---')
        ablation_results = run_ablation(
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
                  f'bring_set={metrics["test"]["bring_set_accuracy"]:.3f} '
                  f'lead_set={metrics["test"]["lead_set_accuracy"]:.3f}')

    # ── Stage 3: Baselines ──
    if 'baselines' in args.stages:
        print('\n--- Stage 3: Baselines ---')
        baselines_dir = results_dir / 'baselines'
        baselines_dir.mkdir(parents=True, exist_ok=True)

        test_ds = TeamPreviewDataset(test_games, vocab, winners_only=winners_only)
        test_loader = DataLoader(
            test_ds, batch_size=1024, shuffle=False, num_workers=0)
        train_ds = TeamPreviewDataset(train_games, vocab, winners_only=winners_only)
        train_loader = DataLoader(
            train_ds, batch_size=1024, shuffle=False, num_workers=0)

        random_m = evaluate_random_baseline(test_loader)
        with open(baselines_dir / 'random_metrics.json', 'w') as f:
            json.dump(random_m.to_dict(), f, indent=2)
        print(f'  Random: bring_set={random_m.bring_set_accuracy:.3f} '
              f'lead_set={random_m.lead_set_accuracy:.3f}')

        popular_m = evaluate_popular_baseline(train_loader, test_loader)
        with open(baselines_dir / 'popular_metrics.json', 'w') as f:
            json.dump(popular_m.to_dict(), f, indent=2)
        print(f'  Popular: bring_set={popular_m.bring_set_accuracy:.3f} '
              f'lead_set={popular_m.lead_set_accuracy:.3f}')

    # ── Stage 4: Multi-seed evaluation ──
    if 'multiseed' in args.stages:
        print('\n--- Stage 4: Multi-Seed Evaluation ---')
        summary = run_multiseed(
            best_config=best_config,
            train_games=train_games,
            val_games=val_games,
            test_games=test_games,
            vocab=vocab,
            output_dir=results_dir,
            device=device,
        )
        for metric in ['bring_set_accuracy', 'lead_set_accuracy',
                       'bring_overlap_accuracy', 'lead_overlap_accuracy']:
            m = summary[metric]
            print(f'  {metric}: {m["mean"]:.3f} +/- {m["std"]:.3f}')

    # ── Stage 5: Generate figures ──
    if 'figures' in args.stages:
        print('\n--- Stage 5: Generating Figures ---')
        generate_all_figures(results_dir)
        print(f'  Figures saved to {results_dir / "figures"}')

    print('\nDone.')


if __name__ == '__main__':
    main()
