"""
BattleNet — Neural network for Pokemon battle state evaluation.

Per-pokemon features (species + moves + ability + item + tera) are encoded
with shared weights, then concatenated with numeric state features and
fed through a trunk that produces value and policy outputs.

A single slot-conditioned policy head is used for all active slots —
it receives the trunk output concatenated with the specific slot's
Pokemon encoding, giving it direct access to who is acting.

The number of pokemon slots and active slots (policy heads) is determined
by the FormatSpec — no if/else branches in forward(), just different
constructor dimensions.

Slot layout (for format with L leads and T team_size):
  [my_active_0..L-1, opp_active_0..L-1, my_bench_0..T-L-1, opp_bench_0..T-L-1]
  Total: 2*T slots

Inputs:
  species_ids: [batch, 2*T] int      — species embedding indices
  move_ids:    [batch, 2*T, 4] int   — up to 4 moves per pokemon
  ability_ids: [batch, 2*T] int      — ability per pokemon
  item_ids:    [batch, 2*T] int      — held item per pokemon
  tera_ids:    [batch, 2*T] int      — tera type per pokemon
  numeric:     [batch, D] float      — encoded battle state features

Outputs:
  value:    [batch] float — raw logits (apply sigmoid externally for probability)
  policy_i: [batch, num_actions] float — action logits per active slot (L outputs)
"""

import torch
import torch.nn as nn

from format_spec import FormatSpec, VGC, ACTIVE_DIM, BENCH_DIM, FIELD_DIM


class BattleNet(nn.Module):
    def __init__(
        self,
        num_species: int,
        num_actions: int,
        num_moves: int,
        num_abilities: int,
        num_items: int,
        num_tera_types: int,
        format_spec: FormatSpec = VGC,
        embed_dim: int = 32,
        feat_embed_dim: int = 16,
        pokemon_dim: int = 48,
        hidden_dim: int = 256,
    ):
        super().__init__()
        self.num_leads = format_spec.num_leads
        num_slots = format_spec.num_battle_slots
        numeric_dim = format_spec.numeric_dim

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

        # ── Per-pokemon encoder (shared across all slots) ──
        raw_dim = embed_dim + feat_embed_dim * 4
        self.pokemon_encoder = nn.Sequential(
            nn.Linear(raw_dim, pokemon_dim),
            nn.ReLU(),
        )

        input_dim = num_slots * pokemon_dim + numeric_dim

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
    ) -> tuple[torch.Tensor, ...]:
        sp = self.species_embed(species_ids)
        mv = self.move_embed(move_ids).sum(dim=2)
        ab = self.ability_embed(ability_ids)
        it = self.item_embed(item_ids)
        te = self.tera_embed(tera_ids)

        per_poke = torch.cat([sp, mv, ab, it, te], dim=-1)
        enc = self.pokemon_encoder(per_poke)
        flat = enc.view(enc.size(0), -1)

        x = torch.cat([flat, numeric], dim=1)
        x = self.trunk(x)

        value = self.value_head(x).squeeze(-1)

        # Slot-conditioned policy for each active slot
        policies = []
        for i in range(self.num_leads):
            slot = enc[:, i, :]
            policies.append(self.policy_head(torch.cat([x, slot], dim=1)))

        return (value, *policies)


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
    head dimension, feature flags, norm type, and residual connections.
    """

    def __init__(
        self,
        num_species: int,
        num_actions: int,
        num_moves: int,
        num_abilities: int,
        num_items: int,
        num_tera_types: int,
        format_spec: FormatSpec = VGC,
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
        self.num_leads = format_spec.num_leads
        num_slots = format_spec.num_battle_slots
        numeric_dim = format_spec.numeric_dim

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

        # ── Per-pokemon encoder (shared across all slots) ──
        self.pokemon_encoder = nn.Sequential(
            nn.Linear(raw_dim, pokemon_dim),
            nn.ReLU(),
        )

        # ── Trunk (variable depth with optional residual + norm choice) ──
        trunk_blocks: list[nn.Module] = []
        in_dim = num_slots * pokemon_dim + numeric_dim
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
    ) -> tuple[torch.Tensor, ...]:
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

        value = self.value_head(x).squeeze(-1)

        # Slot-conditioned policy for each active slot
        policies = []
        for i in range(self.num_leads):
            slot = enc[:, i, :]
            policies.append(self.policy_head(torch.cat([x, slot], dim=1)))

        return (value, *policies)
