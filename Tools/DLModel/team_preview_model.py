"""
TeamPreviewNet — Predicts the optimal team preview configuration for VGC.

Outputs a probability distribution over all 90 possible VGC team configurations
(C(6,4) bring choices × C(4,2) lead choices = 15 × 6 = 90).  Each configuration
specifies which 4 of your 6 Pokemon to bring and which 2 of those to lead.

This joint formulation captures synergy between lead pairs and bench composition,
unlike a factored per-Pokemon approach.

Inputs:
  species_ids: [batch, 12] int   — my 6 + opponent's 6 species indices
  move_ids:    [batch, 12, 4] int — up to 4 moves per pokemon
  ability_ids: [batch, 12] int   — ability per pokemon
  item_ids:    [batch, 12] int   — held item per pokemon
  tera_ids:    [batch, 12] int   — tera type per pokemon

Outputs:
  config_logits: [batch, 90] float — raw logits over VGC configurations
"""

import torch
import torch.nn as nn
from itertools import combinations


# ── VGC Configuration Enumeration ──────────────────────────────────────────
# Deterministic lexicographic order shared between Python and C#.

VGC_CONFIGS: list[tuple[tuple[int, ...], tuple[int, ...], tuple[int, ...]]] = []
_VGC_CONFIG_INDEX: dict[tuple[frozenset[int], frozenset[int]], int] = {}

for _bring in combinations(range(6), 4):
    for _lead in combinations(_bring, 2):
        _bench = tuple(b for b in _bring if b not in _lead)
        _idx = len(VGC_CONFIGS)
        VGC_CONFIGS.append((_bring, _lead, _bench))
        _VGC_CONFIG_INDEX[(frozenset(_bring), frozenset(_lead))] = _idx

NUM_VGC_CONFIGS = len(VGC_CONFIGS)  # 90


def config_to_index(bring_set: set[int], lead_set: set[int]) -> int | None:
    """Map a (bring, lead) pair of index sets to the config index (0–89)."""
    return _VGC_CONFIG_INDEX.get((frozenset(bring_set), frozenset(lead_set)))


def index_to_config(idx: int) -> tuple[tuple[int, ...], tuple[int, ...], tuple[int, ...]]:
    """Return (bring, lead, bench) tuples for a given config index."""
    return VGC_CONFIGS[idx]


class TeamPreviewNet(nn.Module):
    def __init__(
        self,
        num_species: int,
        num_moves: int,
        num_abilities: int,
        num_items: int,
        num_tera_types: int,
        species_embed_dim: int = 48,
        feat_embed_dim: int = 16,
        pokemon_dim: int = 64,
        hidden_dim: int = 256,
        head_dim: int = 64,
    ):
        super().__init__()

        # ── Embeddings (padding_idx=0 → zero vector for unknowns) ──
        self.species_embed = nn.Embedding(
            num_species, species_embed_dim, padding_idx=0)
        self.move_embed = nn.Embedding(
            num_moves, feat_embed_dim, padding_idx=0)
        self.ability_embed = nn.Embedding(
            num_abilities, feat_embed_dim, padding_idx=0)
        self.item_embed = nn.Embedding(
            num_items, feat_embed_dim, padding_idx=0)
        self.tera_embed = nn.Embedding(
            num_tera_types, feat_embed_dim, padding_idx=0)

        # ── Per-pokemon encoder (shared across all 12 slots) ──
        raw_dim = species_embed_dim + feat_embed_dim * 4
        self.pokemon_encoder = nn.Sequential(
            nn.Linear(raw_dim, pokemon_dim),
            nn.ReLU(),
        )

        # ── Trunk ──
        input_dim = 12 * pokemon_dim

        self.trunk = nn.Sequential(
            nn.Linear(input_dim, hidden_dim),
            nn.BatchNorm1d(hidden_dim),
            nn.ReLU(),
            nn.Dropout(0.3),
            nn.Linear(hidden_dim, hidden_dim),
            nn.BatchNorm1d(hidden_dim),
            nn.ReLU(),
            nn.Dropout(0.3),
            nn.Linear(hidden_dim, hidden_dim // 2),
            nn.BatchNorm1d(hidden_dim // 2),
            nn.ReLU(),
            nn.Dropout(0.2),
        )

        self.config_head = nn.Sequential(
            nn.Linear(hidden_dim // 2, head_dim),
            nn.ReLU(),
            nn.Linear(head_dim, NUM_VGC_CONFIGS),
        )

    def forward(
        self,
        species_ids: torch.Tensor,
        move_ids: torch.Tensor,
        ability_ids: torch.Tensor,
        item_ids: torch.Tensor,
        tera_ids: torch.Tensor,
    ) -> torch.Tensor:
        sp = self.species_embed(species_ids)            # [B, 12, E_s]
        mv = self.move_embed(move_ids).sum(dim=2)       # [B, 12, E_f]
        ab = self.ability_embed(ability_ids)             # [B, 12, E_f]
        it = self.item_embed(item_ids)                   # [B, 12, E_f]
        te = self.tera_embed(tera_ids)                   # [B, 12, E_f]

        per_poke = torch.cat([sp, mv, ab, it, te], dim=-1)  # [B, 12, raw]
        enc = self.pokemon_encoder(per_poke)                  # [B, 12, P]
        flat = enc.view(enc.size(0), -1)                      # [B, 12*P]

        x = self.trunk(flat)                                  # [B, H/2]

        return self.config_head(x)                            # [B, 90]


class TeamPreviewNetV2(nn.Module):
    """Parameterised TeamPreviewNet for experimentation.

    Extends the original with configurable trunk depth, dropout,
    head dimension, and feature flags for ablation studies.
    All inputs are always accepted (ONNX compatibility) but disabled
    feature groups are ignored internally.

    New parameters (all backward-compatible with V1 defaults):
      num_trunk_layers: int  — number of trunk MLP blocks (default 3)
      trunk_dropout: float   — dropout rate in trunk (default 0.3)
      head_dim: int          — intermediate dimension in output head (default 64)
      feature_flags: dict    — which feature groups are active (default all True)
    """

    def __init__(
        self,
        num_species: int,
        num_moves: int,
        num_abilities: int,
        num_items: int,
        num_tera_types: int,
        species_embed_dim: int = 48,
        feat_embed_dim: int = 16,
        pokemon_dim: int = 64,
        hidden_dim: int = 256,
        num_trunk_layers: int = 3,
        trunk_dropout: float = 0.3,
        head_dim: int = 64,
        feature_flags: dict | None = None,
    ):
        super().__init__()

        self.feature_flags = feature_flags or {
            'moves': True, 'abilities': True, 'items': True, 'tera': True,
        }

        # ── Embeddings (padding_idx=0 → zero vector for unknowns) ──
        self.species_embed = nn.Embedding(
            num_species, species_embed_dim, padding_idx=0)

        raw_dim = species_embed_dim

        if self.feature_flags.get('moves', True):
            self.move_embed = nn.Embedding(
                num_moves, feat_embed_dim, padding_idx=0)
            raw_dim += feat_embed_dim

        if self.feature_flags.get('abilities', True):
            self.ability_embed = nn.Embedding(
                num_abilities, feat_embed_dim, padding_idx=0)
            raw_dim += feat_embed_dim

        if self.feature_flags.get('items', True):
            self.item_embed = nn.Embedding(
                num_items, feat_embed_dim, padding_idx=0)
            raw_dim += feat_embed_dim

        if self.feature_flags.get('tera', True):
            self.tera_embed = nn.Embedding(
                num_tera_types, feat_embed_dim, padding_idx=0)
            raw_dim += feat_embed_dim

        # ── Per-pokemon encoder (shared across all 12 slots) ──
        self.pokemon_encoder = nn.Sequential(
            nn.Linear(raw_dim, pokemon_dim),
            nn.ReLU(),
        )

        # ── Trunk (variable depth) ──
        trunk_layers: list[nn.Module] = []
        in_dim = 12 * pokemon_dim
        for i in range(num_trunk_layers):
            out_dim = hidden_dim // 2 if i == num_trunk_layers - 1 else hidden_dim
            drop = trunk_dropout * 0.67 if i == num_trunk_layers - 1 else trunk_dropout
            trunk_layers.extend([
                nn.Linear(in_dim, out_dim),
                nn.BatchNorm1d(out_dim),
                nn.ReLU(),
                nn.Dropout(drop),
            ])
            in_dim = out_dim
        self.trunk = nn.Sequential(*trunk_layers)

        # ── Output head ──
        self.config_head = nn.Sequential(
            nn.Linear(hidden_dim // 2, head_dim),
            nn.ReLU(),
            nn.Linear(head_dim, NUM_VGC_CONFIGS),
        )

    def forward(
        self,
        species_ids: torch.Tensor,
        move_ids: torch.Tensor,
        ability_ids: torch.Tensor,
        item_ids: torch.Tensor,
        tera_ids: torch.Tensor,
    ) -> torch.Tensor:
        parts = [self.species_embed(species_ids)]

        if self.feature_flags.get('moves', True):
            parts.append(self.move_embed(move_ids).sum(dim=2))
        if self.feature_flags.get('abilities', True):
            parts.append(self.ability_embed(ability_ids))
        if self.feature_flags.get('items', True):
            parts.append(self.item_embed(item_ids))
        if self.feature_flags.get('tera', True):
            parts.append(self.tera_embed(tera_ids))

        per_poke = torch.cat(parts, dim=-1)
        enc = self.pokemon_encoder(per_poke)
        flat = enc.view(enc.size(0), -1)

        x = self.trunk(flat)

        return self.config_head(x)  # [B, 90] raw logits
