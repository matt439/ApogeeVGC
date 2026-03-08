"""
VGC Battle State Encoding & PyTorch Dataset

Encodes parsed replay data into fixed-size tensors for training.

State encoding (per sample):
  species_ids: [8] int — embedding indices for 8 pokemon slots
    [my_a, my_b, opp_a, opp_b, my_bench_0, my_bench_1, opp_bench_0, opp_bench_1]

  move_ids:    [8, 4] int — up to 4 moves per pokemon
  ability_ids: [8] int    — ability per pokemon
  item_ids:    [8] int    — held item per pokemon
  tera_ids:    [8] int    — tera type per pokemon

  Own-team slots use end-of-game revealed data (always known to the player).
  Opponent slots use progressively revealed data (moves/tera from turn
  actions only — no forward leak). Opponent abilities/items stay at 0
  since per-turn revelation can't be tracked from parsed actions.

  numeric: [200] float — concatenated feature vector:
    [0..34]    my_a active features (35)
    [35..69]   my_b active features (35)
    [70..104]  opp_a active features (35)
    [105..139] opp_b active features (35)
    [140..149] my_bench_0 features (10)
    [150..159] my_bench_1 features (10)
    [160..169] opp_bench_0 features (10)
    [170..179] opp_bench_1 features (10)
    [180..199] field + context (20)

  Active pokemon features (35 per slot):
    0:      hp [0,1]
    1:      is_fainted {0,1}
    2..8:   status one-hot [7] (none/par/brn/slp/psn/tox/frz)
    9..13:  boosts [5] (atk/def/spa/spd/spe) normalized /6
    14:     is_tera {0,1}
    15..34: tera_type one-hot [20] (none + 19 types)

  Bench pokemon features (10 per slot):
    0:     hp [0,1]
    1:     is_fainted {0,1}
    2..8:  status one-hot [7]
    9:     is_present {0,1}

  Field features (20):
    0..4:   weather one-hot [5] (none/Sun/Rain/Sand/Snow)
    5..9:   terrain one-hot [5] (none/Electric/Grassy/Psychic/Misty)
    10:     trick_room {0,1}
    11..18: side conditions (my/opp × tailwind/reflect/light_screen/aurora_veil)
    19:     turn_number / 20

This layout must be replicated exactly in C# for ONNX inference.
"""

from __future__ import annotations

import json
import torch
from torch.utils.data import Dataset

# ── Constants ────────────────────────────────────────────────────────────────

STATUSES = ['par', 'brn', 'slp', 'psn', 'tox', 'frz']
STAT_NAMES = ['atk', 'def', 'spa', 'spd', 'spe']
WEATHERS = ['SunnyDay', 'RainDance', 'Sandstorm', 'Snow']
TERRAINS = ['Electric Terrain', 'Grassy Terrain', 'Psychic Terrain', 'Misty Terrain']
TYPES = [
    'Normal', 'Fire', 'Water', 'Electric', 'Grass', 'Ice',
    'Fighting', 'Poison', 'Ground', 'Flying', 'Psychic', 'Bug',
    'Rock', 'Ghost', 'Dragon', 'Dark', 'Steel', 'Fairy', 'Stellar',
]

STATUS_IDX = {s: i + 1 for i, s in enumerate(STATUSES)}  # 0 = none
WEATHER_IDX = {w: i + 1 for i, w in enumerate(WEATHERS)}
TERRAIN_IDX = {t: i + 1 for i, t in enumerate(TERRAINS)}
TYPE_IDX = {t: i + 1 for i, t in enumerate(TYPES)}

ACTIVE_DIM = 35
BENCH_DIM = 10
FIELD_DIM = 20
NUM_SPECIES_SLOTS = 8
NUMERIC_DIM = 4 * ACTIVE_DIM + 4 * BENCH_DIM + FIELD_DIM  # 200


# ── Encoding functions ──────────────────────────────────────────────────────


def encode_active(feat: torch.Tensor, off: int, state: dict | None) -> None:
    """Encode one active pokemon's 35 numeric features at offset."""
    if state is None:
        return
    feat[off] = state.get('hp', 0) / 100.0
    feat[off + 1] = float(state.get('fainted', False) or state.get('hp', 0) == 0)

    status = state.get('status')
    feat[off + 2 + STATUS_IDX.get(status, 0)] = 1.0

    boosts = state.get('boosts', {})
    for i, s in enumerate(STAT_NAMES):
        feat[off + 9 + i] = boosts.get(s, 0) / 6.0

    tera = state.get('tera')
    if tera:
        feat[off + 14] = 1.0
        feat[off + 15 + TYPE_IDX.get(tera, 0)] = 1.0
    else:
        feat[off + 15] = 1.0  # none slot


def encode_bench(feat: torch.Tensor, off: int, hp: float, fainted: bool,
                 status: str | None, present: bool) -> None:
    """Encode one bench pokemon's 10 numeric features at offset."""
    feat[off] = hp / 100.0
    feat[off + 1] = float(fainted)
    feat[off + 2 + STATUS_IDX.get(status, 0)] = 1.0
    feat[off + 9] = float(present)


def encode_field(feat: torch.Tensor, off: int, field: dict,
                 perspective: str) -> None:
    """Encode 20 field features at offset."""
    opp = 'p2' if perspective == 'p1' else 'p1'

    weather = field.get('weather')
    feat[off + WEATHER_IDX.get(weather, 0)] = 1.0

    terrain = field.get('terrain')
    feat[off + 5 + TERRAIN_IDX.get(terrain, 0)] = 1.0

    feat[off + 10] = float(field.get('trick_room', False))
    feat[off + 11] = float(field.get(f'{perspective}_tailwind', False))
    feat[off + 12] = float(field.get(f'{opp}_tailwind', False))
    feat[off + 13] = float(field.get(f'{perspective}_reflect', False))
    feat[off + 14] = float(field.get(f'{opp}_reflect', False))
    feat[off + 15] = float(field.get(f'{perspective}_light_screen', False))
    feat[off + 16] = float(field.get(f'{opp}_light_screen', False))
    feat[off + 17] = float(field.get(f'{perspective}_aurora_veil', False))
    feat[off + 18] = float(field.get(f'{opp}_aurora_veil', False))


# ── Vocabulary ───────────────────────────────────────────────────────────────


def build_vocab(parsed_path: str) -> dict:
    """Scan parsed JSONL to build species/action/move/ability/item/tera vocabularies."""
    species: set[str] = set()
    actions: set[str] = set()
    moves: set[str] = set()
    abilities: set[str] = set()
    items: set[str] = set()
    tera_types: set[str] = set()

    with open(parsed_path, encoding='utf-8') as f:
        for line in f:
            game = json.loads(line)
            for side in ('p1', 'p2'):
                for poke in game.get('team_preview', {}).get(side, []):
                    species.add(poke['species'])
                for sp in game.get('team_brought', {}).get(side, []):
                    species.add(sp)

                # Collect from revealed info
                for sp, info in game.get('revealed', {}).get(side, {}).items():
                    for m in info.get('moves', []):
                        moves.add(m)
                    if info.get('ability'):
                        abilities.add(info['ability'])
                    if info.get('item'):
                        items.add(info['item'])
                    if info.get('tera_type'):
                        tera_types.add(info['tera_type'])

            for turn in game.get('turns', []):
                for slot, state in turn.get('active', {}).items():
                    species.add(state['species'])
                for action in turn.get('actions', []):
                    if action['type'] == 'move':
                        actions.add(f"move:{action['detail']}")
                    elif action['type'] == 'switch':
                        species.add(action['detail'])
                        actions.add(f"switch:{action['detail']}")

    species_list = ['<pad>', '<unknown>'] + sorted(species)
    action_list = ['<pad>', '<none>', '<cant>'] + sorted(actions)
    move_list = ['<pad>', '<unknown>'] + sorted(moves)
    ability_list = ['<pad>', '<unknown>'] + sorted(abilities)
    item_list = ['<pad>', '<unknown>'] + sorted(items)
    tera_list = ['<pad>', '<unknown>'] + sorted(tera_types)

    return {
        'species': {s: i for i, s in enumerate(species_list)},
        'actions': {a: i for i, a in enumerate(action_list)},
        'moves': {m: i for i, m in enumerate(move_list)},
        'abilities': {a: i for i, a in enumerate(ability_list)},
        'items': {it: i for i, it in enumerate(item_list)},
        'tera_types': {t: i for i, t in enumerate(tera_list)},
        'num_species': len(species_list),
        'num_actions': len(action_list),
        'num_moves': len(move_list),
        'num_abilities': len(ability_list),
        'num_items': len(item_list),
        'num_tera_types': len(tera_list),
    }


# ── Dataset ──────────────────────────────────────────────────────────────────


class VGCDataset(Dataset):
    """PyTorch dataset that encodes parsed VGC replays into tensors.

    Each sample represents one turn from one player's perspective.
    By default, two samples are generated per turn (one per player).
    Set winners_only=True to generate only the winning player's perspective.

    Per-pokemon features (moves, ability, item, tera) are encoded as:
      - Own team: from end-of-game revealed data (player always knows)
      - Opponent: progressively from turn actions (moves, tera only;
        abilities/items stay 0 since per-turn reveal can't be tracked)
    """

    def __init__(self, games: list[dict], vocab: dict, winners_only: bool = False):
        species_map = vocab['species']
        action_map = vocab['actions']
        move_map = vocab['moves']
        ability_map = vocab['abilities']
        item_map = vocab['items']
        tera_map = vocab['tera_types']
        none_id = action_map.get('<none>', 1)
        cant_id = action_map.get('<cant>', 2)
        mv_unk = move_map.get('<unknown>', 1)
        ab_unk = ability_map.get('<unknown>', 1)
        it_unk = item_map.get('<unknown>', 1)
        te_unk = tera_map.get('<unknown>', 1)

        # Pre-count samples (skip games with no winner)
        valid_games = [g for g in games if g.get('winner') in ('p1', 'p2')]
        samples_per_turn = 1 if winners_only else 2
        n = sum(len(g['turns']) * samples_per_turn for g in valid_games)

        self.species_ids = torch.zeros(n, NUM_SPECIES_SLOTS, dtype=torch.long)
        self.move_ids = torch.zeros(n, NUM_SPECIES_SLOTS, 4, dtype=torch.long)
        self.ability_ids = torch.zeros(n, NUM_SPECIES_SLOTS, dtype=torch.long)
        self.item_ids = torch.zeros(n, NUM_SPECIES_SLOTS, dtype=torch.long)
        self.tera_ids = torch.zeros(n, NUM_SPECIES_SLOTS, dtype=torch.long)
        self.numeric = torch.zeros(n, NUMERIC_DIM, dtype=torch.float32)
        self.value_targets = torch.zeros(n, dtype=torch.float32)
        self.policy_a = torch.zeros(n, dtype=torch.long)
        self.policy_b = torch.zeros(n, dtype=torch.long)
        self.turn_progress = torch.zeros(n, dtype=torch.float32)  # turn / max_turn per game

        def fill_own(idx: int, slot: int, species: str,
                     side_revealed: dict) -> None:
            """Fill features for an own-team slot from end-of-game revealed."""
            info = side_revealed.get(species)
            if info is None:
                return
            for j, m in enumerate(info.get('moves', [])[:4]):
                self.move_ids[idx, slot, j] = move_map.get(m, mv_unk)
            ab = info.get('ability')
            if ab:
                self.ability_ids[idx, slot] = ability_map.get(ab, ab_unk)
            it = info.get('item')
            if it:
                self.item_ids[idx, slot] = item_map.get(it, it_unk)
            te = info.get('tera_type')
            if te:
                self.tera_ids[idx, slot] = tera_map.get(te, te_unk)

        def fill_opp(idx: int, slot: int, species: str,
                     prog_moves: dict, prog_tera: dict) -> None:
            """Fill features for an opponent slot from progressive state."""
            moves = prog_moves.get(species, [])
            for j, m in enumerate(moves[:4]):
                self.move_ids[idx, slot, j] = move_map.get(m, mv_unk)
            te = prog_tera.get(species)
            if te:
                self.tera_ids[idx, slot] = tera_map.get(te, te_unk)

        idx = 0
        for game in valid_games:
            winner = game['winner']
            team_brought = game.get('team_brought', {})
            game_revealed = game.get('revealed', {})

            # Track last-known state for bench pokemon across turns
            last_known: dict[tuple[str, str], dict] = {}

            # Progressive revealed state (opponent moves/tera seen so far)
            prog_moves: dict[str, dict[str, list[str]]] = {
                'p1': {}, 'p2': {},
            }
            prog_tera: dict[str, dict[str, str]] = {'p1': {}, 'p2': {}}

            game_turns = game.get('turns', [])
            max_turn = max(len(game_turns), 1)

            for turn in game_turns:
                active = turn.get('active', {})

                # Update last-known from active slots
                for slot, state in active.items():
                    side = slot[:2]
                    sp = state['species']
                    last_known[(side, sp)] = {
                        'hp': state.get('hp', 100),
                        'fainted': state.get('fainted', False)
                                    or state.get('hp', 0) == 0,
                        'status': state.get('status'),
                    }

                # Build slot → species for progressive update later
                slot_species: dict[str, str] = {}
                for slot, state in active.items():
                    slot_species[slot] = state['species']

                perspectives = (winner,) if winners_only else ('p1', 'p2')
                for perspective in perspectives:
                    opp = 'p2' if perspective == 'p1' else 'p1'

                    my_a = active.get(f'{perspective}a')
                    my_b = active.get(f'{perspective}b')
                    opp_a = active.get(f'{opp}a')
                    opp_b = active.get(f'{opp}b')

                    my_revealed = game_revealed.get(perspective, {})
                    opp_prog_m = prog_moves[opp]
                    opp_prog_t = prog_tera[opp]

                    # ── Species IDs ──
                    for si, st in enumerate([my_a, my_b, opp_a, opp_b]):
                        if st:
                            self.species_ids[idx, si] = species_map.get(
                                st['species'], 1)

                    # ── Per-pokemon features (active) ──
                    for si, (st, is_own) in enumerate([
                        (my_a, True), (my_b, True),
                        (opp_a, False), (opp_b, False),
                    ]):
                        if st is None:
                            continue
                        sp = st['species']
                        if is_own:
                            fill_own(idx, si, sp, my_revealed)
                        else:
                            fill_opp(idx, si, sp, opp_prog_m, opp_prog_t)

                    # ── Active numeric features ──
                    feat = self.numeric[idx]
                    encode_active(feat, 0 * ACTIVE_DIM, my_a)
                    encode_active(feat, 1 * ACTIVE_DIM, my_b)
                    encode_active(feat, 2 * ACTIVE_DIM, opp_a)
                    encode_active(feat, 3 * ACTIVE_DIM, opp_b)

                    # ── Bench ──
                    bench_base = 4 * ACTIVE_DIM

                    my_active_sp = set()
                    if my_a:
                        my_active_sp.add(my_a['species'])
                    if my_b:
                        my_active_sp.add(my_b['species'])
                    my_bench = [s for s in team_brought.get(perspective, [])
                                if s not in my_active_sp]

                    for bi, sp in enumerate(my_bench[:2]):
                        off = bench_base + bi * BENCH_DIM
                        self.species_ids[idx, 4 + bi] = species_map.get(sp, 1)
                        fill_own(idx, 4 + bi, sp, my_revealed)
                        lk = last_known.get((perspective, sp))
                        if lk:
                            encode_bench(feat, off, lk['hp'], lk['fainted'],
                                         lk['status'], True)
                        else:
                            encode_bench(feat, off, 100, False, None, True)

                    opp_active_sp = set()
                    if opp_a:
                        opp_active_sp.add(opp_a['species'])
                    if opp_b:
                        opp_active_sp.add(opp_b['species'])
                    opp_bench = [s for s in team_brought.get(opp, [])
                                 if s not in opp_active_sp]

                    for bi, sp in enumerate(opp_bench[:2]):
                        off = bench_base + (2 + bi) * BENCH_DIM
                        self.species_ids[idx, 6 + bi] = species_map.get(sp, 1)
                        fill_opp(idx, 6 + bi, sp, opp_prog_m, opp_prog_t)
                        lk = last_known.get((opp, sp))
                        if lk:
                            encode_bench(feat, off, lk['hp'], lk['fainted'],
                                         lk['status'], True)
                        else:
                            encode_bench(feat, off, 100, False, None, True)

                    # ── Field ──
                    field_off = bench_base + 4 * BENCH_DIM
                    encode_field(feat, field_off,
                                 turn.get('field', {}), perspective)
                    feat[field_off + 19] = turn.get('turn', 1) / 20.0

                    # ── Value target ──
                    self.value_targets[idx] = (
                        1.0 if winner == perspective else 0.0)

                    # ── Turn progress (for value smoothing) ──
                    turn_num = turn.get('turn', 1)
                    self.turn_progress[idx] = min(turn_num / max_turn, 1.0)

                    # ── Policy targets ──
                    actions = turn.get('actions', [])
                    self.policy_a[idx] = self._action_id(
                        actions, f'{perspective}a', action_map, none_id,
                        cant_id)
                    self.policy_b[idx] = self._action_id(
                        actions, f'{perspective}b', action_map, none_id,
                        cant_id)

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

        # Trim in case some turns were skipped
        self.species_ids = self.species_ids[:idx]
        self.move_ids = self.move_ids[:idx]
        self.ability_ids = self.ability_ids[:idx]
        self.item_ids = self.item_ids[:idx]
        self.tera_ids = self.tera_ids[:idx]
        self.numeric = self.numeric[:idx]
        self.value_targets = self.value_targets[:idx]
        self.policy_a = self.policy_a[:idx]
        self.policy_b = self.policy_b[:idx]
        self.turn_progress = self.turn_progress[:idx]
        self.n = idx

    @staticmethod
    def _action_id(actions: list[dict], slot: str,
                   action_map: dict, none_id: int, cant_id: int) -> int:
        """Get the action vocab ID for the first action of a slot."""
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

    def to(self, device: torch.device) -> 'VGCDataset':
        """Move all tensors to the given device (e.g. GPU). Returns self."""
        self.species_ids = self.species_ids.to(device)
        self.move_ids = self.move_ids.to(device)
        self.ability_ids = self.ability_ids.to(device)
        self.item_ids = self.item_ids.to(device)
        self.tera_ids = self.tera_ids.to(device)
        self.numeric = self.numeric.to(device)
        self.value_targets = self.value_targets.to(device)
        self.policy_a = self.policy_a.to(device)
        self.policy_b = self.policy_b.to(device)
        self.turn_progress = self.turn_progress.to(device)
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
            self.value_targets[idx],
            self.policy_a[idx],
            self.policy_b[idx],
            self.turn_progress[idx],
        )
