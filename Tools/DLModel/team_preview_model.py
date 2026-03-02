"""
TeamPreviewNet — Predicts which Pokemon to bring and lead.

Per-pokemon features are encoded with shared weights then flattened
into a trunk that produces bring/lead scores.  Unknown features
(padding_idx=0) produce zero vectors, so the model gracefully handles
both OTS (all features known) and CTS (opponent detail features zeroed).

Inputs:
  species_ids: [batch, 12] int   — my 6 + opponent's 6 species indices
  move_ids:    [batch, 12, 4] int — up to 4 moves per pokemon
  ability_ids: [batch, 12] int   — ability per pokemon
  item_ids:    [batch, 12] int   — held item per pokemon
  tera_ids:    [batch, 12] int   — tera type per pokemon

Outputs:
  bring_scores: [batch, 6] float — sigmoid score per my Pokemon (bring or not)
  lead_scores:  [batch, 6] float — sigmoid score per my Pokemon (lead or not)
"""

import torch
import torch.nn as nn


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

        self.bring_head = nn.Sequential(
            nn.Linear(hidden_dim // 2, 64),
            nn.ReLU(),
            nn.Linear(64, 6),
        )

        self.lead_head = nn.Sequential(
            nn.Linear(hidden_dim // 2, 64),
            nn.ReLU(),
            nn.Linear(64, 6),
        )

    def forward(
        self,
        species_ids: torch.Tensor,
        move_ids: torch.Tensor,
        ability_ids: torch.Tensor,
        item_ids: torch.Tensor,
        tera_ids: torch.Tensor,
    ) -> tuple[torch.Tensor, torch.Tensor]:
        # species_ids: [B, 12]
        # move_ids:    [B, 12, 4]
        # ability_ids: [B, 12]
        # item_ids:    [B, 12]
        # tera_ids:    [B, 12]

        sp = self.species_embed(species_ids)            # [B, 12, E_s]
        mv = self.move_embed(move_ids).sum(dim=2)       # [B, 12, E_f]
        ab = self.ability_embed(ability_ids)             # [B, 12, E_f]
        it = self.item_embed(item_ids)                   # [B, 12, E_f]
        te = self.tera_embed(tera_ids)                   # [B, 12, E_f]

        per_poke = torch.cat([sp, mv, ab, it, te], dim=-1)  # [B, 12, raw]
        enc = self.pokemon_encoder(per_poke)                  # [B, 12, P]
        flat = enc.view(enc.size(0), -1)                      # [B, 12*P]

        x = self.trunk(flat)                                  # [B, H/2]

        bring = torch.sigmoid(self.bring_head(x))             # [B, 6]
        lead = torch.sigmoid(self.lead_head(x))               # [B, 6]

        return bring, lead


class TeamPreviewNetV2(nn.Module):
    """Parameterised TeamPreviewNet for experimentation.

    Extends the original with configurable trunk depth, dropout,
    head dimension, and feature flags for ablation studies.
    All inputs are always accepted (ONNX compatibility) but disabled
    feature groups are ignored internally.

    New parameters (all backward-compatible with V1 defaults):
      num_trunk_layers: int  — number of trunk MLP blocks (default 3)
      trunk_dropout: float   — dropout rate in trunk (default 0.3)
      head_dim: int          — intermediate dimension in output heads (default 64)
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

        # ── Output heads ──
        self.bring_head = nn.Sequential(
            nn.Linear(hidden_dim // 2, head_dim),
            nn.ReLU(),
            nn.Linear(head_dim, 6),
        )

        self.lead_head = nn.Sequential(
            nn.Linear(hidden_dim // 2, head_dim),
            nn.ReLU(),
            nn.Linear(head_dim, 6),
        )

    def forward(
        self,
        species_ids: torch.Tensor,
        move_ids: torch.Tensor,
        ability_ids: torch.Tensor,
        item_ids: torch.Tensor,
        tera_ids: torch.Tensor,
    ) -> tuple[torch.Tensor, torch.Tensor]:
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

        bring = torch.sigmoid(self.bring_head(x))
        lead = torch.sigmoid(self.lead_head(x))

        return bring, lead
