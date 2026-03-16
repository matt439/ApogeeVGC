"""
Generate thesis-quality figures from experiment results.

All figures are saved as both PDF (for LaTeX) and PNG (for quick preview).
"""

from __future__ import annotations

import json
from pathlib import Path

import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import matplotlib.ticker as ticker
import numpy as np


THESIS_STYLE = {
    'figure.figsize': (6, 4),
    'font.size': 11,
    'font.family': 'serif',
    'axes.labelsize': 12,
    'axes.titlesize': 13,
    'xtick.labelsize': 10,
    'ytick.labelsize': 10,
    'legend.fontsize': 10,
    'lines.linewidth': 1.5,
    'axes.grid': True,
    'grid.alpha': 0.3,
    'figure.dpi': 150,
    'savefig.dpi': 300,
    'savefig.bbox': 'tight',
}


def generate_all_figures(results_dir: Path) -> None:
    """Generate all figures for a regulation's results."""
    with plt.style.context(THESIS_STYLE):
        fig_dir = results_dir / 'figures'
        fig_dir.mkdir(parents=True, exist_ok=True)

        plot_learning_curves(results_dir, fig_dir)
        plot_hparam_sensitivity(results_dir, fig_dir)
        plot_ablation_bars(results_dir, fig_dir)
        plot_calibration(results_dir, fig_dir)
        plot_baseline_comparison(results_dir, fig_dir)
        plot_multiseed_distribution(results_dir, fig_dir)


def plot_learning_curves(results_dir: Path, fig_dir: Path) -> None:
    """Plot training and validation loss/accuracy over epochs.

    Uses multi-seed data for mean +/- std bands.
    """
    multiseed_dir = results_dir / 'multiseed'
    if not multiseed_dir.exists():
        return

    all_train_loss = []
    all_val_loss = []
    all_config_acc = []
    all_bring_acc = []
    all_lead_acc = []

    for seed_dir in sorted(multiseed_dir.glob('seed_*')):
        log_path = seed_dir / 'training_log.json'
        if not log_path.exists():
            continue
        with open(log_path) as f:
            log = json.load(f)
        epochs = log['epoch_metrics']
        all_train_loss.append([e['train_loss'] for e in epochs])
        all_val_loss.append([e['val_loss'] for e in epochs])
        all_config_acc.append([e['val_config_accuracy'] for e in epochs])
        all_bring_acc.append([e['val_bring_accuracy'] for e in epochs])
        all_lead_acc.append([e['val_lead_accuracy'] for e in epochs])

    if not all_train_loss:
        return

    fig, axes = plt.subplots(1, 2, figsize=(12, 4.5))

    ax = axes[0]
    _plot_mean_std_band(ax, all_train_loss, label='Train', color='#2196F3')
    _plot_mean_std_band(ax, all_val_loss, label='Validation', color='#FF5722')
    ax.set_xlabel('Epoch')
    ax.set_ylabel('Loss (Cross-Entropy)')
    ax.set_title('Training & Validation Loss')
    ax.legend()
    ax.xaxis.set_major_locator(ticker.MaxNLocator(integer=True))

    ax = axes[1]
    _plot_mean_std_band(ax, all_config_acc, label='Config (exact)', color='#F44336')
    _plot_mean_std_band(ax, all_bring_acc, label='Bring (set)', color='#4CAF50')
    _plot_mean_std_band(ax, all_lead_acc, label='Lead (set)', color='#9C27B0')
    ax.set_xlabel('Epoch')
    ax.set_ylabel('Accuracy')
    ax.set_title('Validation Accuracy')
    ax.legend()
    ax.xaxis.set_major_locator(ticker.MaxNLocator(integer=True))

    plt.tight_layout()
    _save_fig(fig, fig_dir / 'learning_curves')


def plot_hparam_sensitivity(results_dir: Path, fig_dir: Path) -> None:
    """Scatter plots: val_loss vs each hyperparameter from Optuna trials."""
    try:
        import pandas as pd
    except ImportError:
        print('  Warning: pandas not installed, skipping hparam sensitivity plot')
        return

    trials_path = results_dir / 'hparam_search' / 'trials.csv'
    if not trials_path.exists():
        return

    df = pd.read_csv(trials_path)
    df = df[df['state'] == 'COMPLETE']

    if df.empty:
        return

    hparam_cols = [c for c in df.columns if c.startswith('params_')]
    n_params = len(hparam_cols)
    if n_params == 0:
        return

    ncols = 3
    nrows = (n_params + ncols - 1) // ncols

    fig, axes = plt.subplots(nrows, ncols, figsize=(4 * ncols, 3.5 * nrows))
    if nrows == 1:
        axes = axes.reshape(1, -1)
    axes_flat = axes.flatten()

    for i, col in enumerate(hparam_cols):
        ax = axes_flat[i]
        name = col.replace('params_', '')
        ax.scatter(df[col], df['value'], alpha=0.5, s=20, c='#1976D2')
        ax.set_xlabel(name)
        ax.set_ylabel('Val Loss')
        ax.set_title(f'Sensitivity: {name}')

    for i in range(n_params, len(axes_flat)):
        axes_flat[i].set_visible(False)

    plt.tight_layout()
    _save_fig(fig, fig_dir / 'hparam_sensitivity')


def plot_ablation_bars(results_dir: Path, fig_dir: Path) -> None:
    """Grouped bar chart: metrics for each ablation variant."""
    summary_path = results_dir / 'ablation' / 'summary.json'
    if not summary_path.exists():
        return

    with open(summary_path) as f:
        summary = json.load(f)

    names = list(summary.keys())
    config_acc = [summary[n]['test']['config_accuracy'] for n in names]
    bring_set = [summary[n]['test']['bring_set_accuracy'] for n in names]
    lead_set = [summary[n]['test']['lead_set_accuracy'] for n in names]

    x = np.arange(len(names))
    width = 0.25

    fig, ax = plt.subplots(figsize=(8, 5))
    ax.bar(x - width, config_acc, width, label='Config Acc.', color='#F44336')
    ax.bar(x, bring_set, width, label='Bring Set Acc.', color='#2196F3')
    ax.bar(x + width, lead_set, width, label='Lead Set Acc.', color='#FF9800')

    ax.set_xlabel('Feature Configuration')
    ax.set_ylabel('Accuracy')
    ax.set_title('Feature Ablation Study')
    ax.set_xticks(x)
    ax.set_xticklabels([n.replace('_', '\n') for n in names], fontsize=9)
    ax.legend()
    ax.set_ylim(0, 1)

    plt.tight_layout()
    _save_fig(fig, fig_dir / 'ablation_bars')


def plot_calibration(results_dir: Path, fig_dir: Path) -> None:
    """Reliability diagram (calibration plot) averaged across seeds."""
    multiseed_dir = results_dir / 'multiseed'
    if not multiseed_dir.exists():
        return

    all_mid = []
    all_acc = []

    for seed_dir in sorted(multiseed_dir.glob('seed_*')):
        metrics_path = seed_dir / 'test_metrics.json'
        if not metrics_path.exists():
            continue
        with open(metrics_path) as f:
            m = json.load(f)

        rel = m.get('reliability', {})
        if rel and rel.get('midpoints'):
            all_mid.append(rel['midpoints'])
            all_acc.append(rel['accuracies'])

    if not all_mid:
        return

    midpoints = np.mean(all_mid, axis=0)
    accuracies = np.mean(all_acc, axis=0)

    fig, ax = plt.subplots(figsize=(5, 5))
    ax.plot([0, 1], [0, 1], 'k--', alpha=0.5, label='Perfect calibration')
    ax.bar(midpoints, accuracies, width=0.05, alpha=0.7, color='#2196F3',
           label='Model')
    ax.set_xlabel('Predicted Confidence')
    ax.set_ylabel('Observed Accuracy')
    ax.set_title('Calibration Plot (Config Predictions)')
    ax.legend()
    ax.set_xlim(0, 1)
    ax.set_ylim(0, 1)
    ax.set_aspect('equal')

    plt.tight_layout()
    _save_fig(fig, fig_dir / 'calibration')


def plot_baseline_comparison(results_dir: Path, fig_dir: Path) -> None:
    """Bar chart comparing model vs baselines."""
    baselines_dir = results_dir / 'baselines'
    multiseed_dir = results_dir / 'multiseed'
    if not baselines_dir.exists():
        return

    methods = {}
    for f in baselines_dir.glob('*_metrics.json'):
        name = f.stem.replace('_metrics', '')
        with open(f) as fh:
            methods[name] = json.load(fh)

    # Load model metrics from multiseed summary
    summary_path = multiseed_dir / 'summary.json'
    if summary_path.exists():
        with open(summary_path) as fh:
            model_summary = json.load(fh)
        methods['model'] = {
            'config_accuracy': model_summary['config_accuracy']['mean'],
            'bring_set_accuracy': model_summary['bring_set_accuracy']['mean'],
            'lead_set_accuracy': model_summary['lead_set_accuracy']['mean'],
        }

    if not methods:
        return

    names = list(methods.keys())
    config_acc = [methods[n].get('config_accuracy', 0) for n in names]
    bring_set = [methods[n].get('bring_set_accuracy', 0) for n in names]
    lead_set = [methods[n].get('lead_set_accuracy', 0) for n in names]

    x = np.arange(len(names))
    width = 0.25

    fig, ax = plt.subplots(figsize=(7, 5))
    ax.bar(x - width, config_acc, width, label='Config Acc.', color='#F44336')
    ax.bar(x, bring_set, width, label='Bring Set Acc.', color='#2196F3')
    ax.bar(x + width, lead_set, width, label='Lead Set Acc.', color='#FF9800')

    ax.set_ylabel('Accuracy')
    ax.set_title('Model vs Baselines')
    ax.set_xticks(x)
    ax.set_xticklabels([n.replace('_', ' ').title() for n in names])
    ax.legend()
    ax.set_ylim(0, 1)

    plt.tight_layout()
    _save_fig(fig, fig_dir / 'baseline_comparison')


def plot_multiseed_distribution(results_dir: Path, fig_dir: Path) -> None:
    """Box plot showing metric distribution across seeds."""
    summary_path = results_dir / 'multiseed' / 'summary.json'
    if not summary_path.exists():
        return

    with open(summary_path) as f:
        summary = json.load(f)

    metrics = ['config_accuracy', 'bring_set_accuracy',
               'lead_set_accuracy', 'bring_overlap_accuracy',
               'lead_overlap_accuracy']
    labels = ['Config\nExact', 'Bring\nSet Acc.', 'Lead\nSet Acc.',
              'Bring\nOverlap', 'Lead\nOverlap']

    data = []
    for m in metrics:
        if m in summary and 'values' in summary[m]:
            data.append(summary[m]['values'])
        else:
            return

    fig, ax = plt.subplots(figsize=(7, 4))
    bp = ax.boxplot(data, labels=labels, patch_artist=True)

    colors = ['#F44336', '#2196F3', '#FF9800', '#4CAF50', '#9C27B0']
    for patch, color in zip(bp['boxes'], colors):
        patch.set_facecolor(color)
        patch.set_alpha(0.6)

    ax.set_ylabel('Accuracy')
    ax.set_title(f'Metric Distribution Across Seeds (n={len(data[0])})')

    plt.tight_layout()
    _save_fig(fig, fig_dir / 'multiseed_distribution')


def _plot_mean_std_band(ax, data_list, label, color):
    """Plot mean line with +/- 1 std shaded band."""
    max_len = max(len(d) for d in data_list)
    padded = []
    for d in data_list:
        padded.append(d + [d[-1]] * (max_len - len(d)))
    arr = np.array(padded)
    mean = arr.mean(axis=0)
    std = arr.std(axis=0)
    epochs = np.arange(1, max_len + 1)

    ax.plot(epochs, mean, color=color, label=label)
    ax.fill_between(epochs, mean - std, mean + std, color=color, alpha=0.15)


def _save_fig(fig, path: Path) -> None:
    """Save figure as both PDF and PNG."""
    fig.savefig(str(path) + '.pdf')
    fig.savefig(str(path) + '.png')
    plt.close(fig)
