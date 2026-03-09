"""
BattleNet-specific configuration dataclasses for the experimentation framework.

Mirrors config.py but with BattleNet's parameter names and defaults.
Reuses TrainConfig and DataConfig from config.py.
"""

from __future__ import annotations

import json
from dataclasses import dataclass, field, asdict
from pathlib import Path

from .config import TrainConfig, DataConfig


@dataclass
class BattleModelConfig:
    """Architecture hyperparameters for BattleNetV2."""
    embed_dim: int = 32
    feat_embed_dim: int = 16
    pokemon_dim: int = 48
    hidden_dim: int = 256
    num_trunk_layers: int = 3
    trunk_dropout: float = 0.3
    head_dim: int = 64
    feature_flags: dict = field(default_factory=lambda: {
        'moves': True, 'abilities': True, 'items': True, 'tera': True,
    })
    # Phase 2 architecture improvements
    norm_type: str = 'layer'       # 'layer' (LayerNorm) or 'batch' (BatchNorm1d)
    use_residual: bool = True      # skip connections in same-dim trunk layers


@dataclass
class BattleExperimentConfig:
    """Complete BattleNet experiment configuration."""
    name: str = 'default'
    model: BattleModelConfig = field(default_factory=BattleModelConfig)
    train: TrainConfig = field(default_factory=TrainConfig)
    data: DataConfig = field(default_factory=lambda: DataConfig(
        training_strategy='all_games',
    ))

    def to_dict(self) -> dict:
        return asdict(self)

    def to_json(self) -> str:
        return json.dumps(self.to_dict(), indent=2)

    @classmethod
    def from_dict(cls, d: dict) -> BattleExperimentConfig:
        return cls(
            name=d.get('name', 'default'),
            model=BattleModelConfig(**d.get('model', {})),
            train=TrainConfig(**d.get('train', {})),
            data=DataConfig(**{
                k: v for k, v in d.get('data', {}).items()
                if k not in ('data_path', 'winners_only')
            }),
        )

    @classmethod
    def from_json(cls, path: str | Path) -> BattleExperimentConfig:
        with open(path) as f:
            return cls.from_dict(json.load(f))

    def save(self, path: str | Path) -> None:
        Path(path).parent.mkdir(parents=True, exist_ok=True)
        with open(path, 'w') as f:
            f.write(self.to_json())


# ── Optuna search space ──

BATTLE_HPARAM_SEARCH_SPACE = {
    'embed_dim':         ('categorical', [16, 32, 48, 64]),
    'feat_embed_dim':    ('categorical', [8, 16, 24, 32]),
    'pokemon_dim':       ('categorical', [32, 48, 64, 96]),
    'hidden_dim':        ('categorical', [128, 192, 256, 384, 512]),
    'num_trunk_layers':  ('int', 2, 5),
    'trunk_dropout':     ('float', 0.1, 0.5),
    'head_dim':          ('categorical', [32, 64, 96, 128]),
    'lr':                ('loguniform', 1e-4, 5e-3),
    'weight_decay':      ('loguniform', 1e-6, 1e-3),
    'batch_size':        ('categorical', [2048, 4096, 8192, 16384]),
    # Phase 1 training improvements
    'label_smoothing':   ('float', 0.0, 0.2),
    'value_smoothing':   ('float', 0.0, 0.5),
    'value_weight':      ('float', 0.5, 3.0),
    'policy_weight':     ('float', 0.5, 2.0),
    'warmup_frac':       ('float', 0.0, 0.15),
    # Phase 2 architecture improvements
    'norm_type':         ('categorical', ['layer', 'batch']),
    'use_residual':      ('categorical', [True, False]),
    # Phase 3 training efficiency
    'grad_clip_type':    ('categorical', ['norm', 'value']),
}

# ── Ablation configurations (cumulative feature addition) ──

BATTLE_ABLATION_CONFIGS = [
    {'name': 'species_only',
     'flags': {'moves': False, 'abilities': False, 'items': False, 'tera': False}},
    {'name': 'species_moves',
     'flags': {'moves': True, 'abilities': False, 'items': False, 'tera': False}},
    {'name': 'species_moves_abilities',
     'flags': {'moves': True, 'abilities': True, 'items': False, 'tera': False}},
    {'name': 'species_moves_abilities_items',
     'flags': {'moves': True, 'abilities': True, 'items': True, 'tera': False}},
    {'name': 'full',
     'flags': {'moves': True, 'abilities': True, 'items': True, 'tera': True}},
]

BATTLE_MULTI_SEED_SEEDS = [42, 123, 456, 789, 1024]
