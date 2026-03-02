"""
Bayesian hyperparameter optimisation using Optuna.

Searches over architecture and training hyperparameters using
TPE (Tree-structured Parzen Estimator) with median pruning.
"""

from __future__ import annotations

import json
from pathlib import Path

import optuna
from optuna.pruners import MedianPruner
from optuna.samplers import TPESampler

import torch

from .config import (
    ExperimentConfig, ModelConfig, TrainConfig,
    HPARAM_SEARCH_SPACE,
)
from .training import train_model
from .data import make_loaders


def _sample_param(trial: optuna.Trial, name: str, spec: tuple):
    """Sample a single hyperparameter from its search space spec."""
    stype = spec[0]
    if stype == 'categorical':
        return trial.suggest_categorical(name, spec[1])
    elif stype == 'int':
        return trial.suggest_int(name, spec[1], spec[2])
    elif stype == 'float':
        return trial.suggest_float(name, spec[1], spec[2])
    elif stype == 'loguniform':
        return trial.suggest_float(name, spec[1], spec[2], log=True)
    raise ValueError(f'Unknown search type: {stype}')


def create_trial_config(
    trial: optuna.Trial,
    base_config: ExperimentConfig,
) -> ExperimentConfig:
    """Sample hyperparameters from the search space for one Optuna trial."""
    space = HPARAM_SEARCH_SPACE

    model_cfg = ModelConfig(
        species_embed_dim=_sample_param(trial, 'species_embed_dim', space['species_embed_dim']),
        feat_embed_dim=_sample_param(trial, 'feat_embed_dim', space['feat_embed_dim']),
        pokemon_dim=_sample_param(trial, 'pokemon_dim', space['pokemon_dim']),
        hidden_dim=_sample_param(trial, 'hidden_dim', space['hidden_dim']),
        num_trunk_layers=_sample_param(trial, 'num_trunk_layers', space['num_trunk_layers']),
        trunk_dropout=_sample_param(trial, 'trunk_dropout', space['trunk_dropout']),
        head_dim=_sample_param(trial, 'head_dim', space['head_dim']),
    )

    train_cfg = TrainConfig(
        lr=_sample_param(trial, 'lr', space['lr']),
        weight_decay=_sample_param(trial, 'weight_decay', space['weight_decay']),
        batch_size=_sample_param(trial, 'batch_size', space['batch_size']),
        epochs=base_config.train.epochs,
        patience=base_config.train.patience,
        grad_clip=base_config.train.grad_clip,
        scheduler=base_config.train.scheduler,
        seed=base_config.train.seed,
        min_rating=base_config.train.min_rating,
    )

    return ExperimentConfig(
        name=f'trial_{trial.number}',
        model=model_cfg,
        train=train_cfg,
        data=base_config.data,
    )


def run_hparam_search(
    base_config: ExperimentConfig,
    train_games: list[dict],
    val_games: list[dict],
    vocab: dict,
    output_dir: Path,
    n_trials: int = 100,
    timeout_hours: float = 12.0,
    device: torch.device | None = None,
) -> optuna.Study:
    """Run Optuna hyperparameter search.

    Returns the completed study with all trial results.
    """
    if device is None:
        device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')

    search_dir = output_dir / 'hparam_search'
    search_dir.mkdir(parents=True, exist_ok=True)

    storage = f'sqlite:///{search_dir / "optuna_study.db"}'

    study = optuna.create_study(
        study_name=f'team_preview_{base_config.data.regulation}',
        storage=storage,
        load_if_exists=True,
        direction='minimize',
        sampler=TPESampler(seed=base_config.train.seed),
        pruner=MedianPruner(
            n_startup_trials=10,
            n_warmup_steps=5,
            interval_steps=1,
        ),
    )

    def objective(trial: optuna.Trial) -> float:
        config = create_trial_config(trial, base_config)

        train_loader, val_loader = make_loaders(
            train_games, val_games, vocab,
            config.train.batch_size, device,
            winners_only=config.data.winners_only)

        trial_dir = search_dir / f'trial_{trial.number}'

        result = train_model(
            config=config,
            train_loader=train_loader,
            val_loader=val_loader,
            vocab=vocab,
            output_dir=trial_dir,
            device=device,
            trial=trial,
        )

        config.save(trial_dir / 'config.json')
        with open(trial_dir / 'result.json', 'w') as f:
            json.dump({
                'best_val_loss': result.best_val_loss,
                'best_epoch': result.best_epoch,
                'total_params': result.total_params,
                'total_time_sec': result.total_time_sec,
            }, f, indent=2)

        return result.best_val_loss

    study.optimize(
        objective,
        n_trials=n_trials,
        timeout=timeout_hours * 3600,
        show_progress_bar=True,
    )

    # Save best config
    best_trial = study.best_trial
    best_config = create_trial_config(best_trial, base_config)
    best_config.save(search_dir / 'best_config.json')

    # Save all trials as CSV
    df = study.trials_dataframe()
    df.to_csv(search_dir / 'trials.csv', index=False)

    return study
