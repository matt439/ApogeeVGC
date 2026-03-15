"""
Export the best-performing battle and team-preview models to ONNX.

Reads multiseed summary files produced by the experiment pipeline,
selects the seed with the lowest total_loss for each model type,
and exports both to the regulation-specific model directory used by
the C# Driver (Tools/DLModel/models/<regulation>/).

Usage:
  python export_best.py --regulation gen9vgc2025regi
  python export_best.py --regulation gen9vgc2025regi --tier 1500+
"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

from export_onnx import export_battle, export_team_preview


RESULTS_DIR = Path(__file__).parent / 'results'
MODELS_DIR = Path(__file__).parent / 'models'


def find_best_seed(summary_path: Path) -> int:
    """Return the seed with the lowest total_loss from a multiseed summary."""
    with open(summary_path) as f:
        summary = json.load(f)

    seeds = summary['seeds']
    losses = summary['total_loss']['values']
    best_idx = losses.index(min(losses))
    return seeds[best_idx]


def main() -> None:
    parser = argparse.ArgumentParser(
        description='Export best battle & team-preview models to ONNX')
    parser.add_argument('--regulation', required=True,
                        help='Regulation name (e.g. gen9vgc2025regi)')
    parser.add_argument('--tier', default='all',
                        help='Rating tier to export from (default: all)')
    args = parser.parse_args()

    reg_dir = RESULTS_DIR / args.regulation
    if not reg_dir.exists():
        print(f'Error: results directory not found: {reg_dir}')
        sys.exit(1)

    output_dir = MODELS_DIR / args.regulation
    output_dir.mkdir(parents=True, exist_ok=True)
    errors = []

    # ── Team Preview ──
    preview_summary = reg_dir / 'preview' / args.tier / 'multiseed' / 'summary.json'
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
        errors.append(f'Preview multiseed summary not found: {preview_summary}')

    # ── Battle ──
    battle_summary = reg_dir / 'battle' / args.tier / 'multiseed' / 'summary.json'
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


if __name__ == '__main__':
    main()
