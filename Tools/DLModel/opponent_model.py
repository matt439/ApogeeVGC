"""
OpponentPredictionNet — Predicts what the opponent will do next turn.

Uses the same input encoding as BattleNet (species/move/ability/item/tera
embeddings + numeric state features) but outputs only opponent action
predictions — no value head.

Deliberately simpler than BattleNet: fewer trunk layers, smaller capacity.
The task (predict one of ~400 actions) is more constrained than full
battle evaluation.

Inputs:
  species_ids: [batch, 2*T] int
  move_ids:    [batch, 2*T, 4] int
  ability_ids: [batch, 2*T] int
  item_ids:    [batch, 2*T] int
  tera_ids:    [batch, 2*T] int
  numeric:     [batch, D] float

Outputs:
  opp_policy_a: [batch, num_actions] float — logits for opponent slot A
  opp_policy_b: [batch, num_actions] float — logits for opponent slot B
"""

import torch
import torch.nn as nn

from format_spec import FormatSpec, VGC


class OpponentPredictionNet(nn.Module):
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
        num_trunk_layers: int = 2,
        trunk_dropout: float = 0.3,
        head_dim: int = 64,
    ):
        super().__init__()
        self.num_leads = format_spec.num_leads
        num_slots = format_spec.num_battle_slots
        numeric_dim = format_spec.numeric_dim

        # ── Embeddings (same as BattleNet) ──
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

        # ── Trunk (simpler than BattleNet) ──
        input_dim = num_slots * pokemon_dim + numeric_dim
        layers: list[nn.Module] = []
        in_dim = input_dim
        for i in range(num_trunk_layers):
            out_dim = hidden_dim // 2 if i == num_trunk_layers - 1 else hidden_dim
            drop = trunk_dropout * 0.67 if i == num_trunk_layers - 1 else trunk_dropout
            layers.extend([
                nn.Linear(in_dim, out_dim),
                nn.LayerNorm(out_dim),
                nn.ReLU(),
                nn.Dropout(drop),
            ])
            in_dim = out_dim
        self.trunk = nn.Sequential(*layers)

        # ── Opponent policy heads ──
        # Conditioned on the opponent's slot encoding (slots at indices
        # num_leads..2*num_leads in our slot layout)
        self.opp_policy_head = nn.Sequential(
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

        # Opponent slot indices are [num_leads..2*num_leads)
        policies = []
        for i in range(self.num_leads):
            opp_slot = enc[:, self.num_leads + i, :]
            policies.append(self.opp_policy_head(
                torch.cat([x, opp_slot], dim=1)))

        return tuple(policies)
