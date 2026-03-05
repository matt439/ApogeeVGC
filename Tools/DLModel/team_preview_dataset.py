"""
Team Preview Dataset

Encodes team preview decisions from parsed replays.

Per sample:
  species_ids:  [12] int    — my 6 Pokemon + opponent's 6 Pokemon
  move_ids:     [12, 4] int — up to 4 moves per Pokemon (from revealed data)
  ability_ids:  [12] int    — ability per Pokemon
  item_ids:     [12] int    — held item per Pokemon
  tera_ids:     [12] int    — tera type per Pokemon

  bring_target: [6] float   — binary, which of my 6 were brought
  lead_target:  [6] float   — binary, which of my 6 were leads
  value_target: float        — 1.0 if this player won, 0.0 if lost

By default, only the winning player's perspective is used (winners_only=True).
Set winners_only=False to get two samples per game (both perspectives).

Feature sources:
  Own team features come from revealed[perspective] — a lower bound of
  what the player actually knew (they always know their full team).
  Opponent features come from revealed[opponent] — under OTS the player
  knew everything, but the replay only records what was publicly observed.
  Unknown features stay at index 0 (padding) → zero embeddings.
"""

from __future__ import annotations

import torch
from torch.utils.data import Dataset


class TeamPreviewDataset(Dataset):
    def __init__(self, games: list[dict], vocab: dict, winners_only: bool = True):
        species_map = vocab['species']
        move_map = vocab['moves']
        ability_map = vocab['abilities']
        item_map = vocab['items']
        tera_map = vocab['tera_types']

        sp_unk = species_map.get('<unknown>', 1)
        mv_unk = move_map.get('<unknown>', 1)
        ab_unk = ability_map.get('<unknown>', 1)
        it_unk = item_map.get('<unknown>', 1)
        te_unk = tera_map.get('<unknown>', 1)

        valid = [g for g in games if g.get('winner') in ('p1', 'p2')]
        n = len(valid) * (1 if winners_only else 2)

        self.species_ids = torch.zeros(n, 12, dtype=torch.long)
        self.move_ids = torch.zeros(n, 12, 4, dtype=torch.long)
        self.ability_ids = torch.zeros(n, 12, dtype=torch.long)
        self.item_ids = torch.zeros(n, 12, dtype=torch.long)
        self.tera_ids = torch.zeros(n, 12, dtype=torch.long)
        self.bring_target = torch.zeros(n, 6, dtype=torch.float32)
        self.lead_target = torch.zeros(n, 6, dtype=torch.float32)
        self.value_target = torch.zeros(n, dtype=torch.float32)

        idx = 0
        for game in valid:
            winner = game['winner']
            preview = game.get('team_preview', {})
            brought = game.get('team_brought', {})
            revealed = game.get('revealed', {})
            turns = game.get('turns', [])

            # Extract leads from first turn's active slots
            leads: dict[str, set[str]] = {'p1': set(), 'p2': set()}
            if turns:
                first_active = turns[0].get('active', {})
                for slot, state in first_active.items():
                    side = slot[:2]
                    leads[side].add(state['species'])

            perspectives = (winner,) if winners_only else ('p1', 'p2')
            for perspective in perspectives:
                opp = 'p2' if perspective == 'p1' else 'p1'

                my_preview = preview.get(perspective, [])
                opp_preview = preview.get(opp, [])
                my_brought = set(brought.get(perspective, []))
                my_leads = leads.get(perspective, set())
                my_revealed = revealed.get(perspective, {})
                opp_revealed = revealed.get(opp, {})

                # ── Encode my 6 Pokemon (slots 0..5) ──
                for i, poke in enumerate(my_preview[:6]):
                    sp = poke['species']
                    self.species_ids[idx, i] = species_map.get(sp, sp_unk)
                    self._fill_features(
                        idx, i, sp, my_revealed,
                        move_map, ability_map, item_map, tera_map,
                        mv_unk, ab_unk, it_unk, te_unk,
                    )

                # ── Encode opponent's 6 Pokemon (slots 6..11) ──
                for i, poke in enumerate(opp_preview[:6]):
                    sp = poke['species']
                    self.species_ids[idx, 6 + i] = species_map.get(sp, sp_unk)
                    self._fill_features(
                        idx, 6 + i, sp, opp_revealed,
                        move_map, ability_map, item_map, tera_map,
                        mv_unk, ab_unk, it_unk, te_unk,
                    )

                # Bring targets
                for i, poke in enumerate(my_preview[:6]):
                    if poke['species'] in my_brought:
                        self.bring_target[idx, i] = 1.0

                # Lead targets
                for i, poke in enumerate(my_preview[:6]):
                    if poke['species'] in my_leads:
                        self.lead_target[idx, i] = 1.0

                # Value
                self.value_target[idx] = 1.0 if winner == perspective else 0.0

                idx += 1

        self.species_ids = self.species_ids[:idx]
        self.move_ids = self.move_ids[:idx]
        self.ability_ids = self.ability_ids[:idx]
        self.item_ids = self.item_ids[:idx]
        self.tera_ids = self.tera_ids[:idx]
        self.bring_target = self.bring_target[:idx]
        self.lead_target = self.lead_target[:idx]
        self.value_target = self.value_target[:idx]
        self.n = idx

    def _fill_features(
        self,
        sample_idx: int,
        slot: int,
        species: str,
        revealed: dict,
        move_map: dict,
        ability_map: dict,
        item_map: dict,
        tera_map: dict,
        mv_unk: int,
        ab_unk: int,
        it_unk: int,
        te_unk: int,
    ) -> None:
        """Populate move/ability/item/tera IDs for one pokemon slot."""
        info = revealed.get(species)
        if info is None:
            return

        for j, move in enumerate(info.get('moves', [])[:4]):
            self.move_ids[sample_idx, slot, j] = move_map.get(move, mv_unk)

        ability = info.get('ability')
        if ability:
            self.ability_ids[sample_idx, slot] = ability_map.get(ability, ab_unk)

        item = info.get('item')
        if item:
            self.item_ids[sample_idx, slot] = item_map.get(item, it_unk)

        tera_type = info.get('tera_type')
        if tera_type:
            self.tera_ids[sample_idx, slot] = tera_map.get(tera_type, te_unk)

    def to(self, device: torch.device) -> 'TeamPreviewDataset':
        """Move all tensors to the given device (e.g. GPU). Returns self."""
        self.species_ids = self.species_ids.to(device)
        self.move_ids = self.move_ids.to(device)
        self.ability_ids = self.ability_ids.to(device)
        self.item_ids = self.item_ids.to(device)
        self.tera_ids = self.tera_ids.to(device)
        self.bring_target = self.bring_target.to(device)
        self.lead_target = self.lead_target.to(device)
        self.value_target = self.value_target.to(device)
        return self

    def __len__(self) -> int:
        return self.n

    def __getitem__(self, idx: int):
        return (
            self.species_ids[idx],
            self.move_ids[idx],
            self.ability_ids[idx],
            self.item_ids[idx],
            self.tera_ids[idx],
            self.bring_target[idx],
            self.lead_target[idx],
            self.value_target[idx],
        )
