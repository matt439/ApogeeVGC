"""
Export the best-performing battle and team-preview models to ONNX.

Reads multiseed summary files produced by the experiment pipeline,
selects the seed with the lowest total_loss for each model type,
and exports both to the commit-versioned models directory.

Supports mix-and-match: battle and preview models can come from
different experiment commits via --battle-commit / --preview-commit.

Usage:
  python export_best.py --regulation gen9vgc2025regi
  python export_best.py --regulation gen9vgc2025regi --tier 1500+
  python export_best.py --regulation gen9vgc2025regi --battle-commit 9483713e --preview-commit f22f721e
  python export_best.py --regulation gen9vgc2025regi --also-deploy
"""

from __future__ import annotations

import argparse
import json
import shutil
import sys
from pathlib import Path

from export_onnx import export_battle, export_team_preview

sys.path.insert(0, str(Path(__file__).resolve().parent))
from experiments.git_utils import resolve_commit


RESULTS_DIR = Path(__file__).parent / 'results'
MODELS_DIR = Path(__file__).parent / 'models'


def find_best_seed(summary_path: Path) -> int:
    """Return the seed with the lowest total_loss from a multiseed summary."""
    with open(summary_path) as f:
        summary = json.load(f)

    seeds = summary['seeds']
    losses = summary.get('total_loss', summary.get('loss', {}))['values']
    best_idx = losses.index(min(losses))
    return seeds[best_idx]


def main() -> None:
    parser = argparse.ArgumentParser(
        description='Export best battle & team-preview models to ONNX')
    parser.add_argument('--regulation', required=True,
                        help='Regulation name (e.g. gen9vgc2025regi)')
    parser.add_argument('--tier', default='all',
                        help='Rating tier to export from (default: all)')
    parser.add_argument('--commit', default='latest',
                        help='Commit hash for both models (default: latest)')
    parser.add_argument('--battle-commit', default=None,
                        help='Override commit for battle model '
                             '(for mix-and-match)')
    parser.add_argument('--preview-commit', default=None,
                        help='Override commit for preview model '
                             '(for mix-and-match)')
    parser.add_argument('--also-deploy', action='store_true',
                        help='Also copy to models/<reg>/ for C# '
                             'hardcoded paths')
    parser.add_argument('--battle-only', action='store_true',
                        help='Only export battle model (skip preview)')
    args = parser.parse_args()

    reg_dir = RESULTS_DIR / args.regulation
    if not reg_dir.exists():
        print(f'Error: results directory not found: {reg_dir}')
        sys.exit(1)

    # Resolve commit hashes
    battle_commit = resolve_commit(
        reg_dir, args.battle_commit or args.commit)
    preview_commit = resolve_commit(
        reg_dir, args.preview_commit or args.commit)

    print(f'Battle commit:  {battle_commit}')
    print(f'Preview commit: {preview_commit}')

    # Output to the battle commit's models dir by default
    output_dir = reg_dir / battle_commit / 'models'
    output_dir.mkdir(parents=True, exist_ok=True)
    errors = []

    # ── Team Preview ──
    if not args.battle_only:
        preview_summary = (reg_dir / preview_commit / 'preview' / args.tier
                           / 'multiseed' / 'summary.json')
        if preview_summary.exists():
            seed = find_best_seed(preview_summary)
            checkpoint = preview_summary.parent / f'seed_{seed}' / 'model.pt'
            if checkpoint.exists():
                out = str(output_dir / 'team_preview_model.onnx')
                print(f'[preview] Best seed: {seed} — exporting {checkpoint}')
                export_team_preview(str(checkpoint), out)
            else:
                errors.append(f'Checkpoint not found: {checkpoint}')
        else:
            errors.append(
                f'Preview multiseed summary not found: {preview_summary}')
    else:
        print('[preview] Skipped (--battle-only)')

    # ── Battle ──
    battle_summary = (reg_dir / battle_commit / 'battle' / args.tier
                      / 'multiseed' / 'summary.json')
    if battle_summary.exists():
        seed = find_best_seed(battle_summary)
        checkpoint = battle_summary.parent / f'seed_{seed}' / 'model.pt'
        if checkpoint.exists():
            out = str(output_dir / 'battle_model.onnx')
            print(f'[battle]  Best seed: {seed} — exporting {checkpoint}')
            export_battle(str(checkpoint), out)
        else:
            errors.append(f'Checkpoint not found: {checkpoint}')
    else:
        errors.append(f'Battle multiseed summary not found: {battle_summary}')

    if errors:
        print('\nErrors:')
        for e in errors:
            print(f'  - {e}')
        sys.exit(1)

    print(f'\nExported models to: {output_dir}')

    # ── Deploy to models/<reg>/ for C# ──
    if args.also_deploy:
        deploy_dir = MODELS_DIR / args.regulation
        deploy_dir.mkdir(parents=True, exist_ok=True)
        for f in output_dir.glob('*'):
            shutil.copy2(f, deploy_dir / f.name)
        print(f'Deployed to: {deploy_dir}')


if __name__ == '__main__':
    main()
