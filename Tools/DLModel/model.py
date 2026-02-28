"""
BattleNet — Neural network for VGC battle state evaluation.

Inputs:
  species_ids: [batch, 8] int — species embedding indices
  numeric:     [batch, 200] float — encoded battle state features

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
        embed_dim: int = 32,
        hidden_dim: int = 256,
    ):
        super().__init__()
        self.species_embed = nn.Embedding(
            num_species, embed_dim, padding_idx=0)

        input_dim = NUM_SPECIES_SLOTS * embed_dim + NUMERIC_DIM

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

        self.policy_head_a = nn.Sequential(
            nn.Linear(hidden_dim // 2, 128),
            nn.ReLU(),
            nn.Linear(128, num_actions),
        )

        self.policy_head_b = nn.Sequential(
            nn.Linear(hidden_dim // 2, 128),
            nn.ReLU(),
            nn.Linear(128, num_actions),
        )

    def forward(
        self,
        species_ids: torch.Tensor,
        numeric: torch.Tensor,
    ) -> tuple[torch.Tensor, torch.Tensor, torch.Tensor]:
        embeds = self.species_embed(species_ids)        # [B, 8, E]
        embeds = embeds.view(embeds.size(0), -1)        # [B, 8*E]
        x = torch.cat([embeds, numeric], dim=1)         # [B, 8*E+200]
        x = self.trunk(x)                               # [B, H/2]

        value = torch.sigmoid(self.value_head(x)).squeeze(-1)  # [B]
        policy_a = self.policy_head_a(x)                       # [B, A]
        policy_b = self.policy_head_b(x)                       # [B, A]

        return value, policy_a, policy_b
