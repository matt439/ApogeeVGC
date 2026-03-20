"""
Automated train-export-evaluate pipeline.

Trains both models (TeamPreviewNet + BattleNet), exports to ONNX, then runs
bot-vs-bot round-robin evaluation using the C# internal battle simulator.
Designed for overnight runs with structured JSON output.

Usage:
  # Full pipeline (train + export + evaluate + report)
  python pipeline.py --format gen9vgc2025regi

  # Skip training, just evaluate existing models
  python pipeline.py --format gen9vgc2025regi --stages export evaluate report

  # Quick smoke test (no DL models needed)
  python pipeline.py --format gen9vgc2025regi --stages evaluate report \
      --eval-ai-types random --eval-controls greedy --eval-battles 10
"""

from __future__ import annotations

import argparse
import json
import subprocess
import sys
import time
from itertools import combinations
from pathlib import Path

SCRIPT_DIR = Path(__file__).parent.resolve()
REPO_ROOT = (SCRIPT_DIR / '..' / '..').resolve()
CSHARP_PROJECT_DEFAULT = REPO_ROOT / 'ApogeeVGC'
RESULTS_DIR = SCRIPT_DIR / 'results'

ALL_STAGES = ['train', 'export', 'evaluate', 'report']
DEFAULT_AI_TYPES = ['dlgreedy', 'mctsdl', 'mctshybrid', 'ensemble']
DEFAULT_CONTROLS = ['random', 'greedy', 'mcts_standalone']

DL_PLAYER_TYPES = {'dlgreedy', 'mctsdl', 'mcts', 'dl_greedy', 'mcts_dl', 'mctshybrid', 'mcts_hybrid',
                   'ensemble', 'mctsensemble', 'mcts_ensemble'}


def resolve_eval_dir(args: argparse.Namespace) -> Path:
    """Build the evaluation directory from battle + preview commit hashes.

    Returns a path like:
      results/<format>/evaluation/battle-<hash>_preview-<hash>/<tier>/
    """
    from experiments.git_utils import resolve_commit

    reg_dir = RESULTS_DIR / args.format
    battle_commit = resolve_commit(reg_dir, args.battle_commit or args.commit)
    preview_commit = resolve_commit(reg_dir, args.preview_commit or args.commit)

    combo = f'battle-{battle_commit}_preview-{preview_commit}'
    return reg_dir / 'evaluation' / combo / args.tier


def log(msg: str) -> None:
    ts = time.strftime('%H:%M:%S')
    print(f'[pipeline {ts}] {msg}', flush=True)


def run_cmd(cmd: list[str], cwd: str | Path | None = None,
            label: str = '', env: dict[str, str] | None = None) -> int:
    """Run a subprocess, streaming output. Returns exit code."""
    cmd_str = ' '.join(str(c) for c in cmd)
    log(f'Running: {cmd_str}')
    if label:
        log(f'  ({label})')

    run_env = None
    if env:
        import os
        run_env = {**os.environ, **env}

    result = subprocess.run(cmd, cwd=cwd, env=run_env)
    if result.returncode != 0:
        log(f'ERROR: command exited with code {result.returncode}')
    return result.returncode


def load_state(state_path: Path) -> dict:
    if state_path.exists():
        with open(state_path) as f:
            return json.load(f)
    return {}


def save_state(state_path: Path, state: dict) -> None:
    state_path.parent.mkdir(parents=True, exist_ok=True)
    with open(state_path, 'w') as f:
        json.dump(state, f, indent=2)


# ── Stage: Train ──────────────────────────────────────────────────────────

def stage_train(args: argparse.Namespace) -> bool:
    """Train TeamPreviewNet and BattleNet via experiment orchestrators."""
    log('=== STAGE: TRAIN ===')

    common_args = [
        '--regulation', args.format,
        '--data-root', args.data_root,
        '--results-root', args.results_root,
        '--n-trials', str(args.n_trials),
        '--epochs', str(args.epochs),
        '--clean',
    ]

    # Train TeamPreviewNet
    if args.battle_only:
        log('Skipping TeamPreviewNet training (--battle-only)')
    else:
        log('Training TeamPreviewNet...')
        rc = run_cmd(
            [sys.executable, '-m', 'experiments.preview_run_all'] + common_args,
            cwd=SCRIPT_DIR,
            label='TeamPreviewNet experiment pipeline',
        )
        if rc != 0:
            log('TeamPreviewNet training failed')
            return False

    # Train BattleNet
    log('Training BattleNet...')
    rc = run_cmd(
        [sys.executable, '-m', 'experiments.battle_run_all'] + common_args,
        cwd=SCRIPT_DIR,
        label='BattleNet experiment pipeline',
    )
    if rc != 0:
        log('BattleNet training failed')
        return False

    log('Training complete')
    return True


# ── Stage: Export ─────────────────────────────────────────────────────────

def stage_export(args: argparse.Namespace) -> bool:
    """Export best models to ONNX."""
    log('=== STAGE: EXPORT ===')

    cmd = [
        sys.executable, 'export_best.py',
        '--regulation', args.format,
        '--tier', args.tier,
        '--commit', args.commit,
        '--also-deploy',
    ]
    if args.battle_only:
        cmd.append('--battle-only')
    if args.battle_commit:
        cmd.extend(['--battle-commit', args.battle_commit])
    if args.preview_commit:
        cmd.extend(['--preview-commit', args.preview_commit])

    rc = run_cmd(cmd, cwd=SCRIPT_DIR, label='ONNX export')
    if rc != 0:
        log('ONNX export failed')
        return False

    log('Export complete')
    return True


# ── Stage: Evaluate ───────────────────────────────────────────────────────

def build_matchups(ai_types: list[str],
                   controls: list[str]) -> list[tuple[str, str]]:
    """Build round-robin matchups: AI vs controls + AI vs AI."""
    matchups = []

    # Each AI type vs each control
    for ai in ai_types:
        for ctrl in controls:
            matchups.append((ai, ctrl))

    # AI types vs each other (if more than one)
    for a, b in combinations(ai_types, 2):
        matchups.append((a, b))

    return matchups


def stage_evaluate(args: argparse.Namespace) -> bool:
    """Run bot-vs-bot round-robin via C# evaluator."""
    log('=== STAGE: EVALUATE ===')

    eval_dir = resolve_eval_dir(args)
    matchup_dir = eval_dir / 'matchups'
    matchup_dir.mkdir(parents=True, exist_ok=True)

    matchups = build_matchups(args.eval_ai_types, args.eval_controls)
    log(f'{len(matchups)} matchups to evaluate')

    # Check if DL models are needed but missing
    all_players = set(args.eval_ai_types) | set(args.eval_controls)
    needs_dl = bool(all_players & DL_PLAYER_TYPES)
    if needs_dl:
        model_dir = SCRIPT_DIR / 'models' / args.format
        onnx_path = model_dir / 'battle_model.onnx'
        if not onnx_path.exists():
            log(f'ERROR: DL model not found at {onnx_path}')
            log('Run with --stages train export first, or remove DL player types')
            return False

    # Build C# project once
    log('Building C# project...')
    rc = run_cmd(
        ['dotnet', 'build', '-c', 'Release', '--nologo', '-v', 'quiet',
         str(args.csharp_project / 'ApogeeVGC.csproj')],
        label='dotnet build',
    )
    if rc != 0:
        log('C# build failed')
        return False

    # Pass absolute model paths via env vars so the C# app doesn't depend on cwd
    model_dir = SCRIPT_DIR / 'models' / args.format
    ensemble_config = SCRIPT_DIR / 'ensemble_config.json'
    model_env = {
        'APOGEE_BATTLE_MODEL': str(model_dir / 'battle_model.onnx'),
        'APOGEE_BATTLE_VOCAB': str(model_dir / 'battle_model_vocab.json'),
        'APOGEE_PREVIEW_MODEL': str(model_dir / 'team_preview_model.onnx'),
        'APOGEE_ENSEMBLE_CONFIG': str(ensemble_config),
    }

    failed = 0
    for i, (p1, p2) in enumerate(matchups, 1):
        output_path = matchup_dir / f'{p1}_vs_{p2}.json'

        # Skip if already completed
        if output_path.exists():
            log(f'[{i}/{len(matchups)}] {p1} vs {p2} — already done, skipping')
            continue

        log(f'[{i}/{len(matchups)}] {p1} vs {p2} ({args.eval_battles} battles)')

        rc = run_cmd(
            ['dotnet', 'run', '--project',
             str(args.csharp_project / 'ApogeeVGC.csproj'),
             '-c', 'Release', '--no-build', '--',
             '--mode', 'Evaluate',
             '--format', args.format,
             '--player1', p1,
             '--player2', p2,
             '--battles', str(args.eval_battles),
             '--mcts-iterations', str(args.mcts_iterations),
             '--threads', str(args.eval_threads),
             '--output', str(output_path)],
            env=model_env,
            label=f'{p1} vs {p2}',
        )
        if rc != 0:
            log(f'WARNING: {p1} vs {p2} failed')
            failed += 1

    if failed > 0:
        log(f'{failed}/{len(matchups)} matchups failed')
        return False

    # Verify that output files were actually produced (catches silent failures
    # where the C# process exits 0 but writes no output, e.g. model-not-found)
    produced = list(matchup_dir.glob('*.json'))
    if not produced:
        log('ERROR: No matchup output files were produced — evaluation failed silently')
        return False

    log('Evaluation complete')
    return True


# ── Stage: Report ─────────────────────────────────────────────────────────

def stage_report(args: argparse.Namespace) -> bool:
    """Generate summary report from evaluation results."""
    log('=== STAGE: REPORT ===')

    eval_dir = resolve_eval_dir(args)
    matchup_dir = eval_dir / 'matchups'

    if not matchup_dir.exists():
        log('No matchup results found — run evaluate stage first')
        return False

    # Collect all matchup results
    results = {}
    for f in sorted(matchup_dir.glob('*.json')):
        with open(f) as fh:
            data = json.load(fh)
        key = f'{data["player1"]} vs {data["player2"]}'
        results[key] = data

    if not results:
        log('No matchup JSON files found')
        return False

    # Build summary
    summary = {
        'format': args.format,
        'tier': args.tier,
        'num_matchups': len(results),
        'matchups': {},
    }

    # Collect all unique players for win rate matrix
    all_players: set[str] = set()
    for data in results.values():
        all_players.add(data['player1'])
        all_players.add(data['player2'])
    player_list = sorted(all_players)

    # Win rate matrix (row = player, col = opponent)
    win_matrix: dict[str, dict[str, float | None]] = {
        p: {q: None for q in player_list} for p in player_list
    }

    for data in results.values():
        p1 = data['player1']
        p2 = data['player2']
        combined = data['combined']
        total = combined['p1_wins'] + combined['p2_wins'] + combined['ties']
        if total > 0:
            p1_wr = combined['p1_wins'] / total
            win_matrix[p1][p2] = round(p1_wr, 3)
            win_matrix[p2][p1] = round(1 - p1_wr, 3)

        summary['matchups'][f'{p1}_vs_{p2}'] = {
            'p1': p1,
            'p2': p2,
            'p1_wins': combined['p1_wins'],
            'p2_wins': combined['p2_wins'],
            'ties': combined['ties'],
            'p1_win_rate': combined['p1_win_rate'],
            'avg_turns': data['avg_turns'],
        }

    summary['win_rate_matrix'] = {
        'players': player_list,
        'matrix': win_matrix,
    }

    # Write summary
    summary_path = eval_dir / 'round_robin_summary.json'
    with open(summary_path, 'w') as f:
        json.dump(summary, f, indent=2)
    log(f'Summary written to {summary_path}')

    # Print console summary
    print()
    print('=' * 70)
    print(f'  Round-Robin Results: {args.format} (tier: {args.tier})')
    print('=' * 70)
    print()

    # Win rate table
    col_w = max(len(p) for p in player_list)
    header = f'{"":>{col_w}}  ' + '  '.join(f'{p:>{col_w}}' for p in player_list)
    print(header)
    print('-' * len(header))
    for row_player in player_list:
        cells = []
        for col_player in player_list:
            val = win_matrix[row_player][col_player]
            if val is None:
                cells.append(f'{"—":>{col_w}}')
            else:
                cells.append(f'{val:>{col_w}.1%}')
        print(f'{row_player:>{col_w}}  ' + '  '.join(cells))

    print()

    # Per-matchup details
    for key, data in sorted(summary['matchups'].items()):
        p1, p2 = data['p1'], data['p2']
        wr = data['p1_win_rate']
        turns = data['avg_turns']
        print(f'  {p1} vs {p2}: {wr:.1%} win rate, {turns:.1f} avg turns')

    print()
    print('=' * 70)

    # Try to generate figures
    try:
        from experiments.report import generate_evaluation_figures
        figures_dir = eval_dir / 'figures'
        figures_dir.mkdir(parents=True, exist_ok=True)
        generate_evaluation_figures(summary, figures_dir)
        log(f'Figures saved to {figures_dir}')
    except ImportError:
        log('Skipping figure generation (experiments.report not found)')
    except Exception as e:
        log(f'Figure generation failed: {e}')

    log('Report complete')
    return True


# ── Main ──────────────────────────────────────────────────────────────────

def main() -> None:
    parser = argparse.ArgumentParser(
        description='Automated train-export-evaluate pipeline',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=__doc__,
    )

    # Core args
    parser.add_argument('--format', required=True,
                        help='Regulation name (e.g. gen9vgc2025regi)')
    parser.add_argument('--tier', default='all',
                        help='Rating tier for export and evaluation (default: all)')
    parser.add_argument('--stages', nargs='+', default=ALL_STAGES,
                        choices=ALL_STAGES,
                        help='Pipeline stages to run (default: all)')

    # Training args (passed through to experiment orchestrators)
    parser.add_argument('--data-root', default='../ReplayScraper/data',
                        help='Data directory (default: ../ReplayScraper/data)')
    parser.add_argument('--results-root', default='results',
                        help='Results directory (default: results)')
    parser.add_argument('--n-trials', type=int, default=100,
                        help='Optuna trials for hparam search (default: 100)')
    parser.add_argument('--epochs', type=int, default=50,
                        help='Max training epochs (default: 50)')
    parser.add_argument('--battle-only', action='store_true',
                        help='Skip TeamPreviewNet training (only train BattleNet)')

    # Evaluation args
    parser.add_argument('--eval-ai-types', nargs='+', default=DEFAULT_AI_TYPES,
                        help='AI player types to evaluate (default: dlgreedy mctsdl)')
    parser.add_argument('--eval-controls', nargs='+', default=DEFAULT_CONTROLS,
                        help='Control bot types (default: random greedy mcts_standalone)')
    parser.add_argument('--eval-battles', type=int, default=1000,
                        help='Battles per matchup, split for side-swap (default: 1000)')
    parser.add_argument('--mcts-iterations', type=int, default=10000,
                        help='MCTS iteration budget (default: 10000)')
    parser.add_argument('--eval-threads', type=int, default=32,
                        help='Parallel threads for evaluation (default: 32)')

    # Commit versioning
    parser.add_argument('--commit', default='latest',
                        help='Commit hash for both models (default: latest)')
    parser.add_argument('--battle-commit', default=None,
                        help='Override commit for battle model (mix-and-match)')
    parser.add_argument('--preview-commit', default=None,
                        help='Override commit for preview model (mix-and-match)')

    # Paths
    parser.add_argument('--csharp-project', type=Path,
                        default=CSHARP_PROJECT_DEFAULT,
                        help='Path to C# project directory')

    args = parser.parse_args()

    log(f'Pipeline starting: format={args.format}, tier={args.tier}')
    log(f'Stages: {", ".join(args.stages)}')

    start_time = time.time()

    # Train and export don't need the eval dir
    needs_eval = bool({'evaluate', 'report'} & set(args.stages))

    # ── Train ──
    if 'train' in args.stages:
        ok = stage_train(args)
        if not ok:
            log('Stage train failed — aborting pipeline')
            sys.exit(1)

    # ── Export ──
    if 'export' in args.stages:
        ok = stage_export(args)
        if not ok:
            log('Stage export failed — aborting pipeline')
            sys.exit(1)

    # ── Evaluate + Report (need commit-versioned eval dir) ──
    if needs_eval:
        eval_dir = resolve_eval_dir(args)
        state_path = eval_dir / 'pipeline_state.json'
        state = load_state(state_path)
        log(f'Evaluation dir: {eval_dir}')

        for stage_name in ('evaluate', 'report'):
            if stage_name not in args.stages:
                continue
            if stage_name != 'report' and state.get(f'{stage_name}_completed'):
                log(f'Skipping {stage_name} (already completed)')
                continue

            func = {'evaluate': stage_evaluate, 'report': stage_report}[stage_name]
            ok = func(args)

            if ok:
                state[f'{stage_name}_completed'] = True
                state[f'{stage_name}_timestamp'] = time.strftime('%Y-%m-%d %H:%M:%S')
                save_state(state_path, state)
            else:
                log(f'Stage {stage_name} failed — aborting pipeline')
                sys.exit(1)

    elapsed = time.time() - start_time
    hours = int(elapsed // 3600)
    minutes = int((elapsed % 3600) // 60)
    log(f'Pipeline complete in {hours}h {minutes}m')


if __name__ == '__main__':
    main()
