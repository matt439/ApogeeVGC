"""
BattleNet — Neural network for VGC battle state evaluation.

Per-pokemon features (species + moves + ability + item + tera) are encoded
with shared weights, then concatenated with numeric state features and
fed through a trunk that produces value and policy outputs.

A single slot-conditioned policy head is used for both active slots —
it receives the trunk output concatenated with the specific slot's
Pokemon encoding, giving it direct access to who is acting.

Unknown features (padding_idx=0) produce zero vectors, handling partial
information (e.g. unrevealed opponent moves in CTS) gracefully.

Inputs:
  species_ids: [batch, 8] int      — species embedding indices
  move_ids:    [batch, 8, 4] int   — up to 4 moves per pokemon
  ability_ids: [batch, 8] int      — ability per pokemon
  item_ids:    [batch, 8] int      — held item per pokemon
  tera_ids:    [batch, 8] int      — tera type per pokemon
  numeric:     [batch, 200] float  — encoded battle state features

Outputs:
  value:    [batch] float — win probability (sigmoid)
  policy_a: [batch, num_actions] float — action logits for slot a
  policy_b: [batch, num_actions] float — action logits for slot b
"""

import torch
import torch.nn as nn

NUMERIC_DIM = 200
NUM_SPECIES_SLOTS = 8


class BattleNet(nn.Module):
    def __init__(
        self,
        num_species: int,
        num_actions: int,
        num_moves: int,
        num_abilities: int,
        num_items: int,
        num_tera_types: int,
        embed_dim: int = 32,
        feat_embed_dim: int = 16,
        pokemon_dim: int = 48,
        hidden_dim: int = 256,
    ):
        super().__init__()

        # ── Embeddings (padding_idx=0 → zero vector for unknowns) ──
        self.species_embed = nn.Embedding(
            num_species, embed_dim, padding_idx=0)
        self.move_embed = nn.Embedding(
            num_moves, feat_embed_dim, padding_idx=0)
        self.ability_embed = nn.Embedding(
            num_abilities, feat_embed_dim, padding_idx=0)
        self.item_embed = nn.Embedding(
            num_items, feat_embed_dim, padding_idx=0)
        self.tera_embed = nn.Embedding(
            num_tera_types, feat_embed_dim, padding_idx=0)

        # ── Per-pokemon encoder (shared across all 8 slots) ──
        raw_dim = embed_dim + feat_embed_dim * 4
        self.pokemon_encoder = nn.Sequential(
            nn.Linear(raw_dim, pokemon_dim),
            nn.ReLU(),
        )

        input_dim = NUM_SPECIES_SLOTS * pokemon_dim + NUMERIC_DIM

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

        self.value_head = nn.Sequential(
            nn.Linear(hidden_dim // 2, 64),
            nn.ReLU(),
            nn.Linear(64, 1),
        )

        # Single policy head conditioned on the acting slot's encoding
        self.policy_head = nn.Sequential(
            nn.Linear(hidden_dim // 2 + pokemon_dim, 128),
            nn.ReLU(),
            nn.Linear(128, num_actions),
        )

    def forward(
        self,
        species_ids: torch.Tensor,
        move_ids: torch.Tensor,
        ability_ids: torch.Tensor,
        item_ids: torch.Tensor,
        tera_ids: torch.Tensor,
        numeric: torch.Tensor,
    ) -> tuple[torch.Tensor, torch.Tensor, torch.Tensor]:
        # species_ids: [B, 8]
        # move_ids:    [B, 8, 4]
        # ability_ids: [B, 8]
        # item_ids:    [B, 8]
        # tera_ids:    [B, 8]
        # numeric:     [B, 200]

        sp = self.species_embed(species_ids)            # [B, 8, E_s]
        mv = self.move_embed(move_ids).sum(dim=2)       # [B, 8, E_f]
        ab = self.ability_embed(ability_ids)             # [B, 8, E_f]
        it = self.item_embed(item_ids)                   # [B, 8, E_f]
        te = self.tera_embed(tera_ids)                   # [B, 8, E_f]

        per_poke = torch.cat([sp, mv, ab, it, te], dim=-1)  # [B, 8, raw]
        enc = self.pokemon_encoder(per_poke)                  # [B, 8, P]
        flat = enc.view(enc.size(0), -1)                      # [B, 8*P]

        x = torch.cat([flat, numeric], dim=1)                 # [B, 8*P+200]
        x = self.trunk(x)                                     # [B, H/2]

        value = torch.sigmoid(self.value_head(x)).squeeze(-1)  # [B]

        # Slot-conditioned policy: concat trunk with acting slot's encoding
        slot_a = enc[:, 0, :]                                  # [B, P]
        slot_b = enc[:, 1, :]                                  # [B, P]
        policy_a = self.policy_head(torch.cat([x, slot_a], dim=1))  # [B, A]
        policy_b = self.policy_head(torch.cat([x, slot_b], dim=1))  # [B, A]

        return value, policy_a, policy_b


class _TrunkBlock(nn.Module):
    """Single trunk MLP block with optional residual connection."""

    def __init__(self, in_dim: int, out_dim: int, norm_type: str,
                 dropout: float, use_residual: bool):
        super().__init__()
        self.linear = nn.Linear(in_dim, out_dim)
        self.norm = (nn.LayerNorm(out_dim) if norm_type == 'layer'
                     else nn.BatchNorm1d(out_dim))
        self.act = nn.ReLU()
        self.drop = nn.Dropout(dropout)
        # Residual only when dimensions match
        self.residual = use_residual and (in_dim == out_dim)

    def forward(self, x: torch.Tensor) -> torch.Tensor:
        out = self.drop(self.act(self.norm(self.linear(x))))
        if self.residual:
            out = out + x
        return out


class BattleNetV2(nn.Module):
    """Parameterised BattleNet for experimentation.

    Extends the original with configurable trunk depth, dropout,
    head dimension, and feature flags for ablation studies.
    All inputs are always accepted (ONNX compatibility) but disabled
    feature groups are ignored internally.

    Parameters (all backward-compatible with V1 defaults):
      num_trunk_layers: int  — number of trunk MLP blocks (default 3)
      trunk_dropout: float   — dropout rate in trunk (default 0.3)
      head_dim: int          — intermediate dimension in output heads (default 64)
      feature_flags: dict    — which feature groups are active (default all True)
      norm_type: str         — 'layer' (LayerNorm) or 'batch' (BatchNorm1d)
      use_residual: bool     — add skip connections in same-dim trunk layers
    """

    def __init__(
        self,
        num_species: int,
        num_actions: int,
        num_moves: int,
        num_abilities: int,
        num_items: int,
        num_tera_types: int,
        embed_dim: int = 32,
        feat_embed_dim: int = 16,
        pokemon_dim: int = 48,
        hidden_dim: int = 256,
        num_trunk_layers: int = 3,
        trunk_dropout: float = 0.3,
        head_dim: int = 64,
        feature_flags: dict | None = None,
        norm_type: str = 'layer',
        use_residual: bool = True,
    ):
        super().__init__()

        self.feature_flags = feature_flags or {
            'moves': True, 'abilities': True, 'items': True, 'tera': True,
        }

        # ── Embeddings (padding_idx=0 → zero vector for unknowns) ──
        self.species_embed = nn.Embedding(
            num_species, embed_dim, padding_idx=0)

        raw_dim = embed_dim

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

        # ── Per-pokemon encoder (shared across all 8 slots) ──
        self.pokemon_encoder = nn.Sequential(
            nn.Linear(raw_dim, pokemon_dim),
            nn.ReLU(),
        )

        # ── Trunk (variable depth with optional residual + norm choice) ──
        trunk_blocks: list[nn.Module] = []
        in_dim = NUM_SPECIES_SLOTS * pokemon_dim + NUMERIC_DIM
        for i in range(num_trunk_layers):
            out_dim = hidden_dim // 2 if i == num_trunk_layers - 1 else hidden_dim
            drop = trunk_dropout * 0.67 if i == num_trunk_layers - 1 else trunk_dropout
            trunk_blocks.append(_TrunkBlock(
                in_dim, out_dim, norm_type, drop, use_residual))
            in_dim = out_dim
        self.trunk = nn.Sequential(*trunk_blocks)

        # ── Output heads ──
        self.value_head = nn.Sequential(
            nn.Linear(hidden_dim // 2, head_dim),
            nn.ReLU(),
            nn.Linear(head_dim, 1),
        )

        # Single policy head conditioned on the acting slot's encoding
        self.policy_head = nn.Sequential(
            nn.Linear(hidden_dim // 2 + pokemon_dim, head_dim),
            nn.ReLU(),
            nn.Linear(head_dim, num_actions),
        )

    def forward(
        self,
        species_ids: torch.Tensor,
        move_ids: torch.Tensor,
        ability_ids: torch.Tensor,
        item_ids: torch.Tensor,
        tera_ids: torch.Tensor,
        numeric: torch.Tensor,
    ) -> tuple[torch.Tensor, torch.Tensor, torch.Tensor]:
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

        x = torch.cat([flat, numeric], dim=1)
        x = self.trunk(x)

        value = torch.sigmoid(self.value_head(x)).squeeze(-1)

        # Slot-conditioned policy: concat trunk with acting slot's encoding
        slot_a = enc[:, 0, :]
        slot_b = enc[:, 1, :]
        policy_a = self.policy_head(torch.cat([x, slot_a], dim=1))
        policy_b = self.policy_head(torch.cat([x, slot_b], dim=1))

        return value, policy_a, policy_b
