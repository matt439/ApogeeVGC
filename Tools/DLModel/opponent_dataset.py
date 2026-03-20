"""
Opponent Prediction Dataset

Encodes parsed replay data into tensors for training an opponent action
prediction model. Uses the same state encoding as VGCDataset but targets
the *opponent's* actions rather than our own.

Each sample represents one decision point from one player's perspective.
The target is what the opponent did at that decision point.

Input encoding is identical to VGCDataset (same species/move/ability/item/tera
embeddings, same 264D numeric features). Only the targets differ.
"""

from __future__ import annotations

import json
import torch
from torch.utils.data import Dataset

from format_spec import FormatSpec, VGC, ACTIVE_DIM, BENCH_DIM, FIELD_DIM, MATCHUP_DIM
from game_data import (
    estimate_damage_fraction, get_species_types, get_species_base_speed,
    TYPE_INDEX,
)
from dataset import (
    build_vocab, encode_active, encode_bench, encode_field, encode_matchup,
    STATUSES, STAT_NAMES, VOLATILES, VOLATILE_IDX,
    WEATHERS, TERRAINS, TYPES,
)


class OpponentPredictionDataset(Dataset):
    """Dataset that predicts what the opponent will do.

    Uses the same state encoding as VGCDataset but the policy targets
    are the *opponent's* actions, not our own. No value head target.

    Always uses both perspectives (winners_only is not applicable here —
    we want to learn opponent behavior regardless of game outcome).
    """

    def __init__(self, games: list[dict], vocab: dict,
                 format_spec: FormatSpec = VGC):
        num_leads = format_spec.num_leads
        num_slots = format_spec.num_battle_slots
        numeric_dim = format_spec.numeric_dim
        bench_per_side = format_spec.num_bench_per_side
        slot_letters = [chr(ord('a') + i) for i in range(num_leads)]

        species_map = vocab['species']
        action_map = vocab['actions']
        move_map = vocab['moves']
        ability_map = vocab['abilities']
        item_map = vocab['items']
        tera_map = vocab['tera_types']

        sp_unk = species_map.get('<unknown>', 1)
        mv_unk = move_map.get('<unknown>', 1)
        ab_unk = ability_map.get('<unknown>', 1)
        it_unk = item_map.get('<unknown>', 1)
        te_unk = tera_map.get('<unknown>', 1)
        none_id = action_map.get('<none>', 1)
        cant_id = action_map.get('<cant>', 2)

        # Count samples (2 per turn: both perspectives)
        total = 0
        valid_games = []
        for game in games:
            turns = game.get('decision_points', game.get('turns', []))
            if not turns:
                continue
            valid_games.append(game)
            total += len(turns) * 2

        # Pre-allocate tensors
        self.species_ids = torch.zeros(total, num_slots, dtype=torch.long)
        self.move_ids = torch.zeros(total, num_slots, 4, dtype=torch.long)
        self.ability_ids = torch.zeros(total, num_slots, dtype=torch.long)
        self.item_ids = torch.zeros(total, num_slots, dtype=torch.long)
        self.tera_ids = torch.zeros(total, num_slots, dtype=torch.long)
        self.numeric = torch.zeros(total, numeric_dim)
        # Opponent action targets (one per opponent active slot)
        self.opp_policy_a = torch.zeros(total, dtype=torch.long)
        self.opp_policy_b = torch.zeros(total, dtype=torch.long)

        def fill_own(idx: int, slot: int, species: str,
                     revealed: dict) -> None:
            info = revealed.get(species, {})
            for j, m in enumerate(info.get('moves', [])[:4]):
                self.move_ids[idx, slot, j] = move_map.get(m, mv_unk)
            if info.get('ability'):
                self.ability_ids[idx, slot] = ability_map.get(
                    info['ability'], ab_unk)
            if info.get('item'):
                self.item_ids[idx, slot] = item_map.get(
                    info['item'], it_unk)
            if info.get('tera_type'):
                self.tera_ids[idx, slot] = tera_map.get(
                    info['tera_type'], te_unk)

        def fill_opp(idx: int, slot: int, species: str,
                     prog_moves: dict, prog_tera: dict) -> None:
            moves = prog_moves.get(species, [])
            for j, m in enumerate(moves[:4]):
                self.move_ids[idx, slot, j] = move_map.get(m, mv_unk)
            te = prog_tera.get(species)
            if te:
                self.tera_ids[idx, slot] = tera_map.get(te, te_unk)

        def _action_id(actions: list[dict], slot: str,
                       action_map: dict, none_id: int, cant_id: int) -> int:
            for a in actions:
                if a.get('slot') != slot:
                    continue
                atype = a.get('type', '')
                if atype == 'move':
                    key = f"move:{a['detail']}"
                    return action_map.get(key, none_id)
                elif atype == 'switch':
                    key = f"switch:{a['detail']}"
                    return action_map.get(key, none_id)
                elif atype == 'cant':
                    return cant_id
                else:
                    return none_id
            return none_id

        idx = 0
        for game in valid_games:
            winner = game['winner']
            team_brought = game.get('team_brought', {})
            game_revealed = game.get('revealed', {})

            last_known: dict[tuple[str, str], dict] = {}
            prog_moves: dict[str, dict[str, list[str]]] = {
                'p1': {}, 'p2': {},
            }
            prog_tera: dict[str, dict[str, str]] = {'p1': {}, 'p2': {}}
            seen_pokemon: dict[str, list[str]] = {'p1': [], 'p2': []}

            game_turns = game.get('decision_points', game.get('turns', []))

            for turn in game_turns:
                active = turn.get('active', {})

                for slot, state in active.items():
                    side = slot[:2]
                    sp = state['species']
                    last_known[(side, sp)] = {
                        'hp': state.get('hp', 100),
                        'fainted': state.get('fainted', False)
                                    or state.get('hp', 0) == 0,
                        'status': state.get('status'),
                    }
                    if sp not in seen_pokemon[side]:
                        seen_pokemon[side].append(sp)

                slot_species: dict[str, str] = {}
                for slot, state in active.items():
                    slot_species[slot] = state['species']

                # Always both perspectives
                for perspective in ('p1', 'p2'):
                    opp = 'p2' if perspective == 'p1' else 'p1'

                    my_active = [active.get(f'{perspective}{l}')
                                 for l in slot_letters]
                    opp_active = [active.get(f'{opp}{l}')
                                  for l in slot_letters]

                    my_revealed = game_revealed.get(perspective, {})
                    opp_prog_m = prog_moves[opp]
                    opp_prog_t = prog_tera[opp]

                    # ── Species IDs + per-pokemon features (active) ──
                    feat = self.numeric[idx]
                    for si, st in enumerate(my_active):
                        if st:
                            self.species_ids[idx, si] = species_map.get(
                                st['species'], 1)
                            fill_own(idx, si, st['species'], my_revealed)
                        encode_active(feat, si * ACTIVE_DIM, st)

                    for si, st in enumerate(opp_active):
                        slot_idx = num_leads + si
                        if st:
                            self.species_ids[idx, slot_idx] = species_map.get(
                                st['species'], 1)
                            fill_opp(idx, slot_idx, st['species'],
                                     opp_prog_m, opp_prog_t)
                        encode_active(feat, (num_leads + si) * ACTIVE_DIM, st)

                    # ── Bench ──
                    bench_base = num_leads * 2 * ACTIVE_DIM
                    bench_slot_base = num_leads * 2

                    my_active_sp = {st['species'] for st in my_active if st}
                    my_bench = [s for s in seen_pokemon[perspective]
                                if s not in my_active_sp]

                    for bi, sp in enumerate(my_bench[:bench_per_side]):
                        off = bench_base + bi * BENCH_DIM
                        slot_idx = bench_slot_base + bi
                        self.species_ids[idx, slot_idx] = species_map.get(sp, 1)
                        fill_own(idx, slot_idx, sp, my_revealed)
                        lk = last_known.get((perspective, sp))
                        if lk:
                            encode_bench(feat, off, lk['hp'], lk['fainted'],
                                         lk['status'], True)
                        else:
                            encode_bench(feat, off, 100, False, None, True)

                    opp_active_sp = {st['species'] for st in opp_active if st}
                    opp_bench = [s for s in seen_pokemon[opp]
                                 if s not in opp_active_sp]

                    for bi, sp in enumerate(opp_bench[:bench_per_side]):
                        off = bench_base + (bench_per_side + bi) * BENCH_DIM
                        slot_idx = bench_slot_base + bench_per_side + bi
                        self.species_ids[idx, slot_idx] = species_map.get(sp, 1)
                        fill_opp(idx, slot_idx, sp, opp_prog_m, opp_prog_t)
                        lk = last_known.get((opp, sp))
                        if lk:
                            encode_bench(feat, off, lk['hp'], lk['fainted'],
                                         lk['status'], True)
                        else:
                            encode_bench(feat, off, 100, False, None, True)

                    # ── Field ──
                    field_off = bench_base + bench_per_side * 2 * BENCH_DIM
                    encode_field(feat, field_off,
                                 turn.get('field', {}), perspective)
                    feat[field_off + 19] = turn.get('turn', 1) / 20.0

                    # ── Matchup features ──
                    matchup_off = field_off + FIELD_DIM
                    trick_room = bool(turn.get('field', {}).get('trick_room'))
                    encode_matchup(feat, matchup_off,
                                   my_active, opp_active,
                                   my_revealed, opp_prog_m,
                                   trick_room)

                    # ── Opponent action targets ──
                    actions = turn.get('actions', [])
                    self.opp_policy_a[idx] = _action_id(
                        actions, f'{opp}a', action_map, none_id, cant_id)
                    if num_leads >= 2:
                        self.opp_policy_b[idx] = _action_id(
                            actions, f'{opp}b', action_map, none_id, cant_id)

                    idx += 1

                # ── Update progressive revealed from this turn's actions ──
                for action in turn.get('actions', []):
                    side = action['slot'][:2]
                    sp = slot_species.get(action['slot'])
                    if sp is None:
                        continue
                    if action['type'] == 'move':
                        sp_moves = prog_moves[side].setdefault(sp, [])
                        if action['detail'] not in sp_moves:
                            sp_moves.append(action['detail'])
                    tera = action.get('tera')
                    if tera:
                        prog_tera[side][sp] = tera

        # Trim
        self.species_ids = self.species_ids[:idx]
        self.move_ids = self.move_ids[:idx]
        self.ability_ids = self.ability_ids[:idx]
        self.item_ids = self.item_ids[:idx]
        self.tera_ids = self.tera_ids[:idx]
        self.numeric = self.numeric[:idx]
        self.opp_policy_a = self.opp_policy_a[:idx]
        self.opp_policy_b = self.opp_policy_b[:idx]
        self.n = idx

    def to(self, device: torch.device) -> 'OpponentPredictionDataset':
        self.species_ids = self.species_ids.to(device)
        self.move_ids = self.move_ids.to(device)
        self.ability_ids = self.ability_ids.to(device)
        self.item_ids = self.item_ids.to(device)
        self.tera_ids = self.tera_ids.to(device)
        self.numeric = self.numeric.to(device)
        self.opp_policy_a = self.opp_policy_a.to(device)
        self.opp_policy_b = self.opp_policy_b.to(device)
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
            self.numeric[idx],
            self.opp_policy_a[idx],
            self.opp_policy_b[idx],
        )
