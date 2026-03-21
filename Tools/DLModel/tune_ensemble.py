"""
Ensemble Weight Tuning via Optuna

Runs evaluation games of ensemble MCTS vs random/greedy to find optimal
global weights for each mini-model.

Modes:
  tune     — Tune all model weights together (default)
  ablation — Solo test each model, then additive build-up in order of benefit

Usage:
  python tune_ensemble.py --format gen9vgc2025regi
  python tune_ensemble.py --format gen9vgc2025regi --mode ablation
  python tune_ensemble.py --format gen9vgc2025regi --n-trials 200 --games-per-trial 100
"""

from __future__ import annotations

import argparse
import datetime
import json
import os
import subprocess
import sys
import tempfile
from pathlib import Path

import optuna

SCRIPT_DIR = Path(__file__).resolve().parent
PROJECT_ROOT = SCRIPT_DIR.parent.parent
CSHARP_PROJECT = PROJECT_ROOT / 'ApogeeVGC' / 'ApogeeVGC.csproj'

MINI_MODEL_NAMES = [
    'DamageMax',
    'KOSeeking',
    'KOAvoidance',
    'TypePositioning',
    'DamageMin',
    'SpeedAdvantage',
    'StatusSpreading',
    'ProtectPrediction',
    'SwitchMomentum',
    'CoordinatedAction',
    'WinConditionAwareness',
    'TempoPreservation',
]


def write_config(weights: dict[str, float], path: Path) -> None:
    config = {'Weights': weights}
    with open(path, 'w') as f:
        json.dump(config, f, indent=2)


def run_evaluation(
    format_id: str,
    player1: str,
    player2: str,
    battles: int,
    mcts_iterations: int,
    threads: int,
    config_path: Path,
) -> float | None:
    """Run C# evaluation and return player1 win rate, or None on failure."""

    model_dir = SCRIPT_DIR / 'models' / format_id
    env = {
        **os.environ,
        'APOGEE_BATTLE_MODEL': str(model_dir / 'battle_model.onnx'),
        'APOGEE_BATTLE_VOCAB': str(model_dir / 'battle_model_vocab.json'),
        'APOGEE_PREVIEW_MODEL': str(model_dir / 'team_preview_model.onnx'),
        'APOGEE_ENSEMBLE_CONFIG': str(config_path),
    }

    with tempfile.NamedTemporaryFile(suffix='.json', delete=False, mode='w') as tmp:
        output_path = tmp.name

    try:
        result = subprocess.run(
            ['dotnet', 'run', '--project', str(CSHARP_PROJECT),
             '-c', 'Release', '--no-build', '--',
             '--mode', 'Evaluate',
             '--format', format_id,
             '--player1', player1,
             '--player2', player2,
             '--battles', str(battles),
             '--mcts-iterations', str(mcts_iterations),
             '--threads', str(threads),
             '--output', output_path],
            env=env,
            capture_output=True, text=True, timeout=600,
        )

        if result.returncode != 0:
            print(f'  Evaluation failed: {result.stderr[:200]}')
            return None

        with open(output_path) as f:
            data = json.load(f)

        combined = data.get('combined', {})
        p1_wins = combined.get('p1_wins', 0)
        total = data.get('successful_battles', data.get('total_battles', battles))
        return p1_wins / total if total > 0 else 0.5

    except (subprocess.TimeoutExpired, json.JSONDecodeError, FileNotFoundError) as e:
        print(f'  Evaluation error: {e}')
        return None
    finally:
        try:
            os.unlink(output_path)
        except OSError:
            pass


def tune_subset(
    model_names: list[str],
    args: argparse.Namespace,
    n_trials: int = 100,
    label: str = '',
) -> tuple[dict[str, float], float]:
    """Tune weights for a subset of models. Returns (best_weights, best_win_rate)."""

    def objective(trial: optuna.Trial) -> float:
        # Active models get tuned weights, inactive get 0
        weights = {name: 0.0 for name in MINI_MODEL_NAMES}
        for name in model_names:
            weights[name] = trial.suggest_float(name, 0.1, 5.0)

        config_path = SCRIPT_DIR / 'ensemble_config.json'
        write_config(weights, config_path)

        win_rate = run_evaluation(
            format_id=args.format,
            player1='ensemble',
            player2=args.opponent,
            battles=args.games_per_trial,
            mcts_iterations=args.mcts_iterations,
            threads=args.threads,
            config_path=config_path,
        )

        if win_rate is None:
            raise optuna.TrialPruned()
        return win_rate

    study_name = f'ablation_{label}_{datetime.datetime.now():%Y%m%d_%H%M%S}'
    study = optuna.create_study(
        study_name=study_name,
        direction='maximize',
    )

    optuna.logging.set_verbosity(optuna.logging.WARNING)
    study.optimize(objective, n_trials=n_trials, show_progress_bar=True)

    best_weights = {name: 0.0 for name in MINI_MODEL_NAMES}
    for name in model_names:
        best_weights[name] = study.best_params.get(name, 1.0)

    return best_weights, study.best_value


def run_ablation(args: argparse.Namespace) -> None:
    """Solo test each model, then additive build-up."""

    solo_trials = args.ablation_solo_trials
    additive_trials = args.ablation_additive_trials

    print('=' * 60)
    print('  ABLATION STUDY')
    print('=' * 60)
    print(f'  Models: {len(MINI_MODEL_NAMES)}')
    print(f'  Opponent: {args.opponent}')
    print(f'  Games/trial: {args.games_per_trial}')
    print(f'  MCTS iterations: {args.mcts_iterations}')
    print(f'  Solo trials: {solo_trials}')
    print(f'  Additive trials: {additive_trials}')
    print()

    results: dict[str, dict] = {}

    # ── Phase 1: Solo test each model ──
    print('--- Phase 1: Solo Model Evaluation ---')
    solo_results: list[tuple[str, float]] = []

    for i, name in enumerate(MINI_MODEL_NAMES):
        print(f'\n  [{i+1}/{len(MINI_MODEL_NAMES)}] Testing {name} alone...')
        weights, win_rate = tune_subset(
            [name], args, n_trials=solo_trials, label=f'solo_{name}')
        solo_results.append((name, win_rate))
        print(f'    {name}: {win_rate:.1%} (weight={weights[name]:.2f})')

    # Sort by win rate descending
    solo_results.sort(key=lambda x: x[1], reverse=True)

    print('\n--- Solo Rankings ---')
    for rank, (name, wr) in enumerate(solo_results, 1):
        print(f'  {rank:2d}. {name:30s} {wr:.1%}')

    results['solo'] = {name: wr for name, wr in solo_results}

    # ── Phase 2: Additive build-up ──
    print('\n--- Phase 2: Additive Build-Up ---')
    included: list[str] = []
    additive_results: list[dict] = []

    # Baseline: no models (uniform priors)
    print('\n  Baseline (no models)...')
    baseline_weights = {name: 0.0 for name in MINI_MODEL_NAMES}
    config_path = SCRIPT_DIR / 'ensemble_config.json'
    write_config(baseline_weights, config_path)
    baseline_wr = run_evaluation(
        format_id=args.format,
        player1='ensemble',
        player2=args.opponent,
        battles=args.games_per_trial * 2,  # More games for baseline
        mcts_iterations=args.mcts_iterations,
        threads=args.threads,
        config_path=config_path,
    ) or 0.5
    print(f'    Baseline (uniform priors): {baseline_wr:.1%}')
    additive_results.append({
        'models': [],
        'win_rate': baseline_wr,
        'delta': 0.0,
        'weights': {},
    })

    best_wr = baseline_wr
    ranked_names = [name for name, _ in solo_results]

    for i, candidate in enumerate(ranked_names):
        test_set = included + [candidate]
        print(f'\n  [{i+1}/{len(ranked_names)}] Adding {candidate} '
              f'(total: {len(test_set)} models)...')

        weights, win_rate = tune_subset(
            test_set, args, n_trials=additive_trials,
            label=f'add_{len(test_set)}')

        delta = win_rate - best_wr
        print(f'    {", ".join(test_set)}: {win_rate:.1%} (delta: {delta:+.1%})')

        additive_results.append({
            'models': list(test_set),
            'win_rate': win_rate,
            'delta': delta,
            'weights': {k: v for k, v in weights.items() if v > 0},
        })

        if delta >= -0.02:  # Accept if not worse by more than 2%
            included.append(candidate)
            best_wr = max(best_wr, win_rate)
            print(f'    → Included (best so far: {best_wr:.1%})')
        else:
            print(f'    → Excluded (hurt performance by {-delta:.1%})')

    # ── Final summary ──
    print('\n' + '=' * 60)
    print('  ABLATION RESULTS')
    print('=' * 60)

    print('\n  Solo Rankings:')
    for rank, (name, wr) in enumerate(solo_results, 1):
        marker = ' ✓' if name in included else '  '
        print(f'    {rank:2d}. {name:30s} {wr:.1%}{marker}')

    print('\n  Additive Build-Up:')
    for step in additive_results:
        models = step['models']
        label = ', '.join(models) if models else '(baseline)'
        print(f'    {len(models):2d} models: {step["win_rate"]:.1%} '
              f'({step["delta"]:+.1%}) — {label}')

    print(f'\n  Final ensemble: {", ".join(included)}')
    print(f'  Final win rate: {best_wr:.1%}')

    results['additive'] = additive_results
    results['final_models'] = included
    results['final_win_rate'] = best_wr

    # ── Save final config ──
    if additive_results:
        # Use weights from the last included step
        for step in reversed(additive_results):
            if step['models'] and step['models'][-1] in included:
                final_weights = {name: 0.0 for name in MINI_MODEL_NAMES}
                final_weights.update(step['weights'])
                write_config(final_weights, config_path)
                print(f'\n  Saved final weights to {config_path}')
                break

    # Save full ablation results
    ablation_path = SCRIPT_DIR / 'ablation_results.json'
    with open(ablation_path, 'w') as f:
        json.dump(results, f, indent=2)
    print(f'  Saved ablation results to {ablation_path}')


def objective(trial: optuna.Trial, args: argparse.Namespace) -> float:
    # Sample weights for each mini-model
    weights = {}
    for name in MINI_MODEL_NAMES:
        weights[name] = trial.suggest_float(name, 0.0, 5.0)

    # Write config
    config_path = SCRIPT_DIR / 'ensemble_config.json'
    write_config(weights, config_path)

    # Run evaluation
    win_rate = run_evaluation(
        format_id=args.format,
        player1='ensemble',
        player2=args.opponent,
        battles=args.games_per_trial,
        mcts_iterations=args.mcts_iterations,
        threads=args.threads,
        config_path=config_path,
    )

    if win_rate is None:
        raise optuna.TrialPruned()

    print(f'  Trial {trial.number}: win_rate={win_rate:.3f} '
          f'weights={{{", ".join(f"{k}={v:.2f}" for k, v in weights.items())}}}')

    return win_rate


def main():
    parser = argparse.ArgumentParser(
        description='Tune ensemble mini-model weights via Optuna')
    parser.add_argument('--format', default='gen9vgc2025regi')
    parser.add_argument('--mode', choices=['tune', 'ablation'], default='tune',
                        help='tune: optimize all weights. ablation: solo + additive analysis')
    parser.add_argument('--opponent', default='random',
                        help='Opponent player type to tune against')
    parser.add_argument('--n-trials', type=int, default=200,
                        help='Number of Optuna trials (tune mode)')
    parser.add_argument('--games-per-trial', type=int, default=50,
                        help='Games per evaluation')
    parser.add_argument('--mcts-iterations', type=int, default=1000,
                        help='MCTS iterations per search')
    parser.add_argument('--threads', type=int, default=16)
    parser.add_argument('--study-name', default=None,
                        help='Optuna study name (default: auto-generated)')
    parser.add_argument('--storage', default=None,
                        help='Optuna storage URL (default: in-memory)')
    parser.add_argument('--ablation-solo-trials', type=int, default=50,
                        help='Optuna trials per solo model test')
    parser.add_argument('--ablation-additive-trials', type=int, default=100,
                        help='Optuna trials per additive step')
    args = parser.parse_args()

    # Build C# once before tuning
    print('Building C# project...')
    result = subprocess.run(
        ['dotnet', 'build', '-c', 'Release', '--nologo', '-v', 'quiet',
         str(CSHARP_PROJECT)],
        capture_output=True, text=True,
    )
    if result.returncode != 0:
        print(f'Build failed:\n{result.stderr}')
        sys.exit(1)
    print('Build succeeded.')

    if args.mode == 'ablation':
        run_ablation(args)
        return

    # ── Standard tuning mode ──
    study_name = args.study_name or f'ensemble_{datetime.datetime.now():%Y%m%d_%H%M%S}'
    study = optuna.create_study(
        study_name=study_name,
        storage=args.storage,
        direction='maximize',
        load_if_exists=True,
    )

    print(f'\nTuning {len(MINI_MODEL_NAMES)} mini-model weights')
    print(f'  Opponent: {args.opponent}')
    print(f'  Games/trial: {args.games_per_trial}')
    print(f'  MCTS iterations: {args.mcts_iterations}')
    print(f'  Trials: {args.n_trials}')
    print()

    study.optimize(
        lambda trial: objective(trial, args),
        n_trials=args.n_trials,
        show_progress_bar=True,
    )

    # Save best weights
    print(f'\nBest trial: {study.best_trial.number}')
    print(f'  Win rate: {study.best_value:.3f}')
    print(f'  Weights:')
    for name, value in study.best_params.items():
        print(f'    {name}: {value:.3f}')

    # Write final config
    config_path = SCRIPT_DIR / 'ensemble_config.json'
    write_config(study.best_params, config_path)
    print(f'\nSaved best weights to {config_path}')

    # Also save study results
    results_path = SCRIPT_DIR / 'ensemble_tuning_results.json'
    results = {
        'best_trial': study.best_trial.number,
        'best_win_rate': study.best_value,
        'best_weights': study.best_params,
        'n_trials': len(study.trials),
        'opponent': args.opponent,
        'games_per_trial': args.games_per_trial,
        'mcts_iterations': args.mcts_iterations,
    }
    with open(results_path, 'w') as f:
        json.dump(results, f, indent=2)
    print(f'Saved tuning results to {results_path}')


if __name__ == '__main__':
    main()
