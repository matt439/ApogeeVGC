"""
Cross-rating-tier statistical comparison.

Loads multiseed results from each rating tier and produces:
  - Welch's t-test between each pair of tiers
  - Cohen's d effect sizes
  - Comparison bar charts with error bars
  - Summary JSON with all statistical test results
"""

from __future__ import annotations

import json
from itertools import combinations
from pathlib import Path

import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import numpy as np

from .config import RATING_TIERS
from .visualise import THESIS_STYLE, _save_fig


def _welch_ttest(a: list[float], b: list[float]) -> tuple[float, float]:
    """Welch's t-test (unequal variance). Returns (t_stat, p_value).

    Falls back to manual computation to avoid scipy dependency.
    """
    try:
        from scipy.stats import ttest_ind
        result = ttest_ind(a, b, equal_var=False)
        return float(result.statistic), float(result.pvalue)
    except ImportError:
        # Manual Welch's t-test
        na, nb = len(a), len(b)
        ma, mb = np.mean(a), np.mean(b)
        va, vb = np.var(a, ddof=1), np.var(b, ddof=1)
        t = (ma - mb) / np.sqrt(va / na + vb / nb)
        df = (va / na + vb / nb) ** 2 / (
            (va / na) ** 2 / (na - 1) + (vb / nb) ** 2 / (nb - 1)
        )
        # Two-tailed p-value approximation using normal (conservative for small n)
        from math import erfc, sqrt
        p = erfc(abs(float(t)) / sqrt(2))
        return float(t), p


def _cohens_d(a: list[float], b: list[float]) -> float:
    """Cohen's d effect size (pooled std)."""
    na, nb = len(a), len(b)
    ma, mb = np.mean(a), np.mean(b)
    va, vb = np.var(a, ddof=1), np.var(b, ddof=1)
    pooled_std = np.sqrt(((na - 1) * va + (nb - 1) * vb) / (na + nb - 2))
    if pooled_std == 0:
        return 0.0
    return float((ma - mb) / pooled_std)


def run_rating_comparison(
    results_root: Path,
    tier_dirs: dict[str, Path],
    metrics: list[str],
    model_name: str = 'TeamPreviewNet',
) -> dict | None:
    """Run cross-tier statistical comparison on multiseed results.

    Args:
        results_root: Root directory to save comparison results.
        tier_dirs: {tier_name: results_dir} for each tier.
        metrics: List of metric names to compare (from multiseed summary).
        model_name: For figure titles.

    Returns:
        Comparison summary dict, or None if insufficient data.
    """
    # Load multiseed summaries from each tier
    tier_summaries: dict[str, dict] = {}
    for tier_name, tier_dir in tier_dirs.items():
        summary_path = tier_dir / 'multiseed' / 'summary.json'
        if not summary_path.exists():
            print(f'  Warning: no multiseed summary for tier {tier_name}')
            continue
        with open(summary_path) as f:
            tier_summaries[tier_name] = json.load(f)

    if len(tier_summaries) < 2:
        print('  Skipping rating comparison: need at least 2 tiers with results')
        return None

    comparison_dir = results_root / 'rating_comparison'
    comparison_dir.mkdir(parents=True, exist_ok=True)

    # ── Statistical tests ──
    comparison: dict = {'tiers': list(tier_summaries.keys()), 'metrics': {}}

    for metric in metrics:
        metric_data: dict = {'per_tier': {}, 'pairwise': []}

        # Per-tier stats
        for tier_name, summary in tier_summaries.items():
            if metric in summary and 'values' in summary[metric]:
                vals = summary[metric]['values']
                metric_data['per_tier'][tier_name] = {
                    'mean': float(np.mean(vals)),
                    'std': float(np.std(vals)),
                    'values': vals,
                }

        # Pairwise comparisons
        tier_names = list(metric_data['per_tier'].keys())
        for t1, t2 in combinations(tier_names, 2):
            v1 = metric_data['per_tier'][t1]['values']
            v2 = metric_data['per_tier'][t2]['values']
            t_stat, p_val = _welch_ttest(v1, v2)
            d = _cohens_d(v1, v2)

            metric_data['pairwise'].append({
                'tier_a': t1,
                'tier_b': t2,
                't_statistic': round(t_stat, 4),
                'p_value': round(p_val, 4),
                'significant_005': p_val < 0.05,
                'cohens_d': round(d, 4),
                'effect_size': (
                    'negligible' if abs(d) < 0.2 else
                    'small' if abs(d) < 0.5 else
                    'medium' if abs(d) < 0.8 else
                    'large'
                ),
            })

        comparison['metrics'][metric] = metric_data

    # Save comparison JSON
    with open(comparison_dir / 'comparison.json', 'w') as f:
        json.dump(comparison, f, indent=2)

    # ── Generate figures ──
    with plt.style.context(THESIS_STYLE):
        fig_dir = comparison_dir / 'figures'
        fig_dir.mkdir(parents=True, exist_ok=True)
        _plot_tier_comparison_bars(comparison, metrics, fig_dir, model_name)
        _plot_tier_comparison_box(tier_summaries, metrics, fig_dir, model_name)

    # Print summary
    print(f'\n  Rating Tier Comparison ({model_name}):')
    for metric in metrics:
        md = comparison['metrics'].get(metric, {})
        parts = []
        for tier_name, stats in md.get('per_tier', {}).items():
            parts.append(f'{tier_name}={stats["mean"]:.3f}±{stats["std"]:.3f}')
        print(f'    {metric}: {", ".join(parts)}')
        for pw in md.get('pairwise', []):
            sig = '*' if pw['significant_005'] else ''
            print(f'      {pw["tier_a"]} vs {pw["tier_b"]}: '
                  f'p={pw["p_value"]:.4f}{sig} d={pw["cohens_d"]:.3f} '
                  f'({pw["effect_size"]})')

    return comparison


def _plot_tier_comparison_bars(
    comparison: dict,
    metrics: list[str],
    fig_dir: Path,
    model_name: str,
) -> None:
    """Grouped bar chart: mean ± std for each metric across rating tiers."""
    tier_names = comparison['tiers']
    n_tiers = len(tier_names)
    n_metrics = len(metrics)

    if n_metrics == 0:
        return

    fig, ax = plt.subplots(figsize=(max(8, n_metrics * 2), 5))

    x = np.arange(n_metrics)
    width = 0.8 / n_tiers
    colors = ['#2196F3', '#4CAF50', '#FF9800', '#9C27B0', '#F44336']

    for i, tier_name in enumerate(tier_names):
        means, stds = [], []
        for metric in metrics:
            md = comparison['metrics'].get(metric, {})
            pt = md.get('per_tier', {}).get(tier_name, {})
            means.append(pt.get('mean', 0))
            stds.append(pt.get('std', 0))

        offset = (i - n_tiers / 2 + 0.5) * width
        ax.bar(x + offset, means, width, yerr=stds,
               label=tier_name, color=colors[i % len(colors)],
               capsize=3, alpha=0.85)

    ax.set_ylabel('Score')
    ax.set_title(f'{model_name} — Performance by Rating Tier')
    ax.set_xticks(x)
    ax.set_xticklabels(
        [m.replace('_', '\n') for m in metrics], fontsize=9)
    ax.legend()
    ax.set_ylim(0, 1)

    # Add significance markers
    _add_significance_brackets(ax, comparison, metrics, x, width, n_tiers)

    plt.tight_layout()
    _save_fig(fig, fig_dir / 'rating_tier_comparison')


def _add_significance_brackets(
    ax, comparison, metrics, x_positions, width, n_tiers
):
    """Add significance brackets above bars for significant pairwise tests."""
    y_max = ax.get_ylim()[1]
    bracket_y = y_max * 0.92

    for mi, metric in enumerate(metrics):
        md = comparison['metrics'].get(metric, {})
        sig_pairs = [
            pw for pw in md.get('pairwise', [])
            if pw['significant_005']
        ]
        for pi, pw in enumerate(sig_pairs):
            tier_names = comparison['tiers']
            i1 = tier_names.index(pw['tier_a']) if pw['tier_a'] in tier_names else -1
            i2 = tier_names.index(pw['tier_b']) if pw['tier_b'] in tier_names else -1
            if i1 < 0 or i2 < 0:
                continue

            x1 = x_positions[mi] + (i1 - n_tiers / 2 + 0.5) * width
            x2 = x_positions[mi] + (i2 - n_tiers / 2 + 0.5) * width
            y = bracket_y + pi * y_max * 0.04

            ax.plot([x1, x1, x2, x2], [y - 0.005, y, y, y - 0.005],
                    'k-', linewidth=0.8)
            stars = '***' if pw['p_value'] < 0.001 else (
                '**' if pw['p_value'] < 0.01 else '*')
            ax.text((x1 + x2) / 2, y + 0.002, stars,
                    ha='center', va='bottom', fontsize=9)


def _plot_tier_comparison_box(
    tier_summaries: dict[str, dict],
    metrics: list[str],
    fig_dir: Path,
    model_name: str,
) -> None:
    """Box plots comparing metric distributions across rating tiers."""
    tier_names = list(tier_summaries.keys())
    n_metrics = len(metrics)

    if n_metrics == 0:
        return

    ncols = min(n_metrics, 3)
    nrows = (n_metrics + ncols - 1) // ncols

    fig, axes = plt.subplots(nrows, ncols,
                             figsize=(5 * ncols, 4 * nrows),
                             squeeze=False)

    colors = ['#2196F3', '#4CAF50', '#FF9800', '#9C27B0', '#F44336']

    for mi, metric in enumerate(metrics):
        ax = axes[mi // ncols][mi % ncols]
        data = []
        labels = []
        for tier_name in tier_names:
            summary = tier_summaries[tier_name]
            if metric in summary and 'values' in summary[metric]:
                data.append(summary[metric]['values'])
                labels.append(tier_name)

        if not data:
            ax.set_visible(False)
            continue

        bp = ax.boxplot(data, labels=labels, patch_artist=True)
        for patch, color in zip(bp['boxes'],
                                colors[:len(data)]):
            patch.set_facecolor(color)
            patch.set_alpha(0.6)

        ax.set_title(metric.replace('_', ' ').title(), fontsize=10)
        ax.set_ylabel('Score')

    # Hide unused axes
    for mi in range(n_metrics, nrows * ncols):
        axes[mi // ncols][mi % ncols].set_visible(False)

    fig.suptitle(f'{model_name} — Distribution by Rating Tier', fontsize=13)
    plt.tight_layout()
    _save_fig(fig, fig_dir / 'rating_tier_boxplots')
