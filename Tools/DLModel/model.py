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
