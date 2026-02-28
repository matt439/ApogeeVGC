"""
Team Preview Dataset

Encodes team preview decisions from parsed replays.

Per sample:
  species_ids: [12] int — my 6 Pokemon + opponent's 6 Pokemon
    [my_0..my_5, opp_0..opp_5]

  bring_target: [6] float — binary, which of my 6 were brought
  lead_target:  [6] float — binary, which of my 6 were leads
  value_target: float     — 1.0 if this player won, 0.0 if lost

Two samples per game (one per player perspective).
"""

from __future__ import annotations

import json
import torch
from torch.utils.data import Dataset


class TeamPreviewDataset(Dataset):
    def __init__(self, games: list[dict], vocab: dict):
        species_map = vocab['species']
        unk_id = species_map.get('<unknown>', 1)

        valid = [g for g in games if g.get('winner') in ('p1', 'p2')]
        n = len(valid) * 2

        self.species_ids = torch.zeros(n, 12, dtype=torch.long)
        self.bring_target = torch.zeros(n, 6, dtype=torch.float32)
        self.lead_target = torch.zeros(n, 6, dtype=torch.float32)
        self.value_target = torch.zeros(n, dtype=torch.float32)

        idx = 0
        for game in valid:
            winner = game['winner']
            preview = game.get('team_preview', {})
            brought = game.get('team_brought', {})
            turns = game.get('turns', [])

            # Extract leads from first turn's active slots
            leads: dict[str, set[str]] = {'p1': set(), 'p2': set()}
            if turns:
                first_active = turns[0].get('active', {})
                for slot, state in first_active.items():
                    side = slot[:2]
                    leads[side].add(state['species'])

            for perspective in ('p1', 'p2'):
                opp = 'p2' if perspective == 'p1' else 'p1'

                my_preview = preview.get(perspective, [])
                opp_preview = preview.get(opp, [])
                my_brought = set(brought.get(perspective, []))
                my_leads = leads.get(perspective, set())

                # Encode species IDs: my 6 then opp 6
                for i, poke in enumerate(my_preview[:6]):
                    sp = poke['species']
                    self.species_ids[idx, i] = species_map.get(sp, unk_id)

                for i, poke in enumerate(opp_preview[:6]):
                    sp = poke['species']
                    self.species_ids[idx, 6 + i] = species_map.get(sp, unk_id)

                # Bring targets
                for i, poke in enumerate(my_preview[:6]):
                    sp = poke['species']
                    if sp in my_brought:
                        self.bring_target[idx, i] = 1.0

                # Lead targets
                for i, poke in enumerate(my_preview[:6]):
                    sp = poke['species']
                    if sp in my_leads:
                        self.lead_target[idx, i] = 1.0

                # Value
                self.value_target[idx] = 1.0 if winner == perspective else 0.0

                idx += 1

        self.species_ids = self.species_ids[:idx]
        self.bring_target = self.bring_target[:idx]
        self.lead_target = self.lead_target[:idx]
        self.value_target = self.value_target[:idx]
        self.n = idx

    def __len__(self) -> int:
        return self.n

    def __getitem__(self, idx: int):
        return (
            self.species_ids[idx],
            self.bring_target[idx],
            self.lead_target[idx],
            self.value_target[idx],
        )
