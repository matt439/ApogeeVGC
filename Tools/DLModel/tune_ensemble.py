"""
Ensemble Weight Tuning via Optuna

Runs evaluation games of ensemble MCTS vs random/greedy to find optimal
global weights for each mini-model. Each trial:
  1. Sets 8 float weights (one per mini-model)
  2. Writes them to ensemble_config.json
  3. Runs N games via the C# evaluator
  4. Returns the win rate as the objective

Usage:
  python tune_ensemble.py --format gen9vgc2025regi
  python tune_ensemble.py --format gen9vgc2025regi --n-trials 200 --games-per-trial 100
  python tune_ensemble.py --format gen9vgc2025regi --opponent random
"""

from __future__ import annotations

import argparse
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
    parser.add_argument('--opponent', default='random',
                        help='Opponent player type to tune against')
    parser.add_argument('--n-trials', type=int, default=200,
                        help='Number of Optuna trials')
    parser.add_argument('--games-per-trial', type=int, default=50,
                        help='Games per evaluation')
    parser.add_argument('--mcts-iterations', type=int, default=1000,
                        help='MCTS iterations per search (lower for speed during tuning)')
    parser.add_argument('--threads', type=int, default=8)
    parser.add_argument('--study-name', default='ensemble_weights')
    parser.add_argument('--storage', default=None,
                        help='Optuna storage URL (default: in-memory)')
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

    # Create Optuna study
    study = optuna.create_study(
        study_name=args.study_name,
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
