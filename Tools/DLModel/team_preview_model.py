"""
TeamPreviewNet — Predicts which Pokemon to bring and lead.

Inputs:
  species_ids: [batch, 12] int — my 6 + opponent's 6 species indices

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
        embed_dim: int = 48,
        hidden_dim: int = 256,
    ):
        super().__init__()
        self.species_embed = nn.Embedding(
            num_species, embed_dim, padding_idx=0)

        input_dim = 12 * embed_dim

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
        self, species_ids: torch.Tensor,
    ) -> tuple[torch.Tensor, torch.Tensor]:
        embeds = self.species_embed(species_ids)     # [B, 12, E]
        embeds = embeds.view(embeds.size(0), -1)     # [B, 12*E]
        x = self.trunk(embeds)                       # [B, H/2]

        bring = torch.sigmoid(self.bring_head(x))    # [B, 6]
        lead = torch.sigmoid(self.lead_head(x))      # [B, 6]

        return bring, lead
