"""
Evaluation report figure generation for the automated pipeline.

Generates thesis-quality visualizations from round-robin evaluation results.
Called by pipeline.py after the evaluate stage completes.
"""

from __future__ import annotations

from pathlib import Path

import matplotlib.pyplot as plt
import numpy as np


def generate_evaluation_figures(summary: dict, figures_dir: Path) -> None:
    """Generate all evaluation figures from a round-robin summary dict."""
    _plot_win_rate_matrix(summary, figures_dir / 'win_rate_matrix.png')
    _plot_win_rate_bars(summary, figures_dir / 'win_rate_bars.png')


def _plot_win_rate_matrix(summary: dict, output_path: Path) -> None:
    """Heatmap of win rates (row beats column)."""
    matrix_data = summary['win_rate_matrix']
    players = matrix_data['players']
    matrix = matrix_data['matrix']
    n = len(players)

    data = np.full((n, n), np.nan)
    for i, p1 in enumerate(players):
        for j, p2 in enumerate(players):
            val = matrix[p1][p2]
            if val is not None:
                data[i, j] = val

    fig, ax = plt.subplots(figsize=(max(6, n * 1.5), max(5, n * 1.2)))
    im = ax.imshow(data, cmap='RdYlGn', vmin=0, vmax=1, aspect='equal')

    ax.set_xticks(range(n))
    ax.set_yticks(range(n))
    ax.set_xticklabels(players, rotation=45, ha='right', fontsize=10)
    ax.set_yticklabels(players, fontsize=10)

    # Annotate cells
    for i in range(n):
        for j in range(n):
            if not np.isnan(data[i, j]):
                val = data[i, j]
                color = 'white' if val < 0.3 or val > 0.7 else 'black'
                ax.text(j, i, f'{val:.1%}', ha='center', va='center',
                        color=color, fontsize=11, fontweight='bold')
            elif i == j:
                ax.text(j, i, '—', ha='center', va='center',
                        color='gray', fontsize=11)

    ax.set_xlabel('Opponent (column)', fontsize=11)
    ax.set_ylabel('Player (row)', fontsize=11)
    ax.set_title(f'Win Rate Matrix — {summary["format"]} ({summary["tier"]})',
                 fontsize=13, fontweight='bold')

    fig.colorbar(im, ax=ax, label='Win Rate', shrink=0.8)
    fig.tight_layout()
    fig.savefig(output_path, dpi=150, bbox_inches='tight')
    plt.close(fig)


def _plot_win_rate_bars(summary: dict, output_path: Path) -> None:
    """Bar chart showing each matchup's win rate."""
    matchups = summary['matchups']
    if not matchups:
        return

    labels = []
    win_rates = []
    for key in sorted(matchups.keys()):
        m = matchups[key]
        labels.append(f'{m["p1"]} vs {m["p2"]}')
        win_rates.append(m['p1_win_rate'])

    fig, ax = plt.subplots(figsize=(max(8, len(labels) * 0.8), 5))

    colors = ['#2ecc71' if wr > 0.5 else '#e74c3c' if wr < 0.5 else '#95a5a6'
              for wr in win_rates]

    bars = ax.barh(range(len(labels)), win_rates, color=colors, edgecolor='white')

    ax.set_yticks(range(len(labels)))
    ax.set_yticklabels(labels, fontsize=10)
    ax.set_xlabel('Player 1 Win Rate', fontsize=11)
    ax.set_title(f'Matchup Results — {summary["format"]} ({summary["tier"]})',
                 fontsize=13, fontweight='bold')
    ax.axvline(x=0.5, color='black', linestyle='--', linewidth=0.8, alpha=0.5)
    ax.set_xlim(0, 1)

    # Annotate bars
    for bar, wr in zip(bars, win_rates):
        ax.text(bar.get_width() + 0.02, bar.get_y() + bar.get_height() / 2,
                f'{wr:.1%}', va='center', fontsize=9)

    fig.tight_layout()
    fig.savefig(output_path, dpi=150, bbox_inches='tight')
    plt.close(fig)
