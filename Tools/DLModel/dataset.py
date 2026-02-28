"""
VGC Battle State Encoding & PyTorch Dataset

Encodes parsed replay data into fixed-size tensors for training.

State encoding (per sample):
  species_ids: [8] int — embedding indices for 8 pokemon slots
    [my_a, my_b, opp_a, opp_b, my_bench_0, my_bench_1, opp_bench_0, opp_bench_1]

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
    """Scan parsed JSONL to build species/action vocabularies."""
    species: set[str] = set()
    actions: set[str] = set()

    with open(parsed_path, encoding='utf-8') as f:
        for line in f:
            game = json.loads(line)
            for side in ('p1', 'p2'):
                for poke in game.get('team_preview', {}).get(side, []):
                    species.add(poke['species'])
                for sp in game.get('team_brought', {}).get(side, []):
                    species.add(sp)

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

    return {
        'species': {s: i for i, s in enumerate(species_list)},
        'actions': {a: i for i, a in enumerate(action_list)},
        'num_species': len(species_list),
        'num_actions': len(action_list),
    }


# ── Dataset ──────────────────────────────────────────────────────────────────


class VGCDataset(Dataset):
    """PyTorch dataset that encodes parsed VGC replays into tensors.

    Each sample represents one turn from one player's perspective.
    Two samples are generated per turn (one per player).
    """

    def __init__(self, games: list[dict], vocab: dict):
        species_map = vocab['species']
        action_map = vocab['actions']
        none_id = action_map.get('<none>', 1)
        cant_id = action_map.get('<cant>', 2)

        # Pre-count samples (skip games with no winner)
        valid_games = [g for g in games if g.get('winner') in ('p1', 'p2')]
        n = sum(len(g['turns']) * 2 for g in valid_games)

        self.species_ids = torch.zeros(n, NUM_SPECIES_SLOTS, dtype=torch.long)
        self.numeric = torch.zeros(n, NUMERIC_DIM, dtype=torch.float32)
        self.value_targets = torch.zeros(n, dtype=torch.float32)
        self.policy_a = torch.zeros(n, dtype=torch.long)
        self.policy_b = torch.zeros(n, dtype=torch.long)

        idx = 0
        for game in valid_games:
            winner = game['winner']
            team_brought = game.get('team_brought', {})

            # Track last-known state for bench pokemon across turns
            last_known: dict[tuple[str, str], dict] = {}

            for turn in game.get('turns', []):
                # Update last-known from active slots
                for slot, state in turn.get('active', {}).items():
                    side = slot[:2]
                    sp = state['species']
                    last_known[(side, sp)] = {
                        'hp': state.get('hp', 100),
                        'fainted': state.get('fainted', False)
                                    or state.get('hp', 0) == 0,
                        'status': state.get('status'),
                    }

                for perspective in ('p1', 'p2'):
                    opp = 'p2' if perspective == 'p1' else 'p1'
                    active = turn.get('active', {})

                    my_a = active.get(f'{perspective}a')
                    my_b = active.get(f'{perspective}b')
                    opp_a = active.get(f'{opp}a')
                    opp_b = active.get(f'{opp}b')

                    # ── Species IDs ──
                    for si, st in enumerate([my_a, my_b, opp_a, opp_b]):
                        if st:
                            self.species_ids[idx, si] = species_map.get(
                                st['species'], 1)

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

                    # ── Policy targets ──
                    actions = turn.get('actions', [])
                    self.policy_a[idx] = self._action_id(
                        actions, f'{perspective}a', action_map, none_id,
                        cant_id)
                    self.policy_b[idx] = self._action_id(
                        actions, f'{perspective}b', action_map, none_id,
                        cant_id)

                    idx += 1

        # Trim in case some turns were skipped
        self.species_ids = self.species_ids[:idx]
        self.numeric = self.numeric[:idx]
        self.value_targets = self.value_targets[:idx]
        self.policy_a = self.policy_a[:idx]
        self.policy_b = self.policy_b[:idx]
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

    def __len__(self) -> int:
        return self.n

    def __getitem__(self, idx: int):
        return (
            self.species_ids[idx],
            self.numeric[idx],
            self.value_targets[idx],
            self.policy_a[idx],
            self.policy_b[idx],
        )
