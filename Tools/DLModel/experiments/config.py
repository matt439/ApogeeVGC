"""
Centralised configuration dataclasses for the experimentation framework.

All experiment parameters are defined here. Configurations are serialisable
to/from JSON for reproducibility and logging.
"""

from __future__ import annotations

import json
from dataclasses import dataclass, field, asdict
from pathlib import Path


@dataclass
class ModelConfig:
    """Architecture hyperparameters for TeamPreviewNetV2."""
    species_embed_dim: int = 48
    feat_embed_dim: int = 16
    pokemon_dim: int = 64
    hidden_dim: int = 256
    num_trunk_layers: int = 3
    trunk_dropout: float = 0.3
    head_dim: int = 64
    feature_flags: dict = field(default_factory=lambda: {
        'moves': True, 'abilities': True, 'items': True, 'tera': True,
    })


@dataclass
class TrainConfig:
    """Training hyperparameters."""
    lr: float = 1e-3
    weight_decay: float = 1e-5
    batch_size: int = 16384
    epochs: int = 50
    patience: int = 7
    grad_clip: float = 1.0
    scheduler: str = 'cosine'
    seed: int = 42
    min_rating: int = 0

    # Phase 1 training improvements
    label_smoothing: float = 0.0       # Policy CrossEntropyLoss label smoothing (0.0 = off)
    value_smoothing: float = 0.0       # Turn-dependent value target smoothing (0.0 = hard 0/1)
    value_weight: float = 1.0          # Weight for value loss in combined loss
    policy_weight: float = 1.0         # Weight for policy loss in combined loss
    warmup_frac: float = 0.0           # Fraction of total steps for LR warmup (0.0 = off)

    # Phase 3 training efficiency
    use_amp: bool = True               # Mixed precision training (auto-disabled on CPU)
    grad_clip_type: str = 'norm'       # 'norm' (clip_grad_norm_) or 'value' (clip_grad_value_)


# ── Rating tiers for comparative analysis ──

RATING_TIERS: list[tuple[str, int]] = [
    ('all', 0),
    ('1200+', 1200),
    ('1500+', 1500),
]


def tier_data_filename(tier_name: str) -> str:
    """Return the parsed data filename for a rating tier."""
    if tier_name == 'all':
        return 'parsed.jsonl'
    return f'parsed_{tier_name.replace("+", "")}.jsonl'


@dataclass
class DataConfig:
    """Data-related configuration."""
    regulation: str = 'gen9vgc2025regi'
    data_root: str = '../ReplayScraper/data'
    train_frac: float = 0.7
    val_frac: float = 0.15
    test_frac: float = 0.15
    training_strategy: str = 'winners_only'  # 'winners_only' or 'all_games'
    rating_tier: str = 'all'  # 'all', '1200+', '1500+'

    @property
    def data_path(self) -> Path:
        return (Path(self.data_root) / self.regulation
                / tier_data_filename(self.rating_tier))

    @property
    def winners_only(self) -> bool:
        return self.training_strategy == 'winners_only'


@dataclass
class ExperimentConfig:
    """Complete experiment configuration."""
    name: str = 'default'
    model: ModelConfig = field(default_factory=ModelConfig)
    train: TrainConfig = field(default_factory=TrainConfig)
    data: DataConfig = field(default_factory=DataConfig)

    def to_dict(self) -> dict:
        return asdict(self)

    def to_json(self) -> str:
        return json.dumps(self.to_dict(), indent=2)

    @classmethod
    def from_dict(cls, d: dict) -> ExperimentConfig:
        return cls(
            name=d.get('name', 'default'),
            model=ModelConfig(**d.get('model', {})),
            train=TrainConfig(**d.get('train', {})),
            data=DataConfig(**{
                k: v for k, v in d.get('data', {}).items()
                if k not in ('data_path', 'winners_only')
            }),
        )

    @classmethod
    def from_json(cls, path: str | Path) -> ExperimentConfig:
        with open(path) as f:
            return cls.from_dict(json.load(f))

    def save(self, path: str | Path) -> None:
        Path(path).parent.mkdir(parents=True, exist_ok=True)
        with open(path, 'w') as f:
            f.write(self.to_json())


# ── Optuna search space ──

HPARAM_SEARCH_SPACE = {
    'species_embed_dim': ('categorical', [16, 32, 48, 64, 96]),
    'feat_embed_dim':    ('categorical', [8, 16, 24, 32]),
    'pokemon_dim':       ('categorical', [32, 48, 64, 96, 128]),
    'hidden_dim':        ('categorical', [128, 192, 256, 384, 512]),
    'num_trunk_layers':  ('int', 2, 5),
    'trunk_dropout':     ('float', 0.1, 0.5),
    'head_dim':          ('categorical', [32, 64, 96, 128]),
    'lr':                ('loguniform', 1e-4, 5e-3),
    'weight_decay':      ('loguniform', 1e-6, 1e-3),
    'batch_size':        ('categorical', [4096, 8192, 16384, 32768]),
}

# ── Ablation configurations (cumulative feature addition) ──

ABLATION_CONFIGS = [
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

MULTI_SEED_SEEDS = [42, 123, 456, 789, 1024]
