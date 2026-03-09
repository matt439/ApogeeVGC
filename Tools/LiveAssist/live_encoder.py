"""
Live State Encoder — Converts LiveBattleState to ONNX-ready numpy arrays.

Ports dataset.py's encoding functions from torch to numpy. Produces the same
tensor layout that the trained ONNX models expect:

  species_ids: [1, 8]    — embedding indices for 8 pokemon slots
  move_ids:    [1, 8, 4] — up to 4 moves per pokemon
  ability_ids: [1, 8]    — ability per pokemon
  item_ids:    [1, 8]    — held item per pokemon
  tera_ids:    [1, 8]    — tera type per pokemon
  numeric:     [1, 200]  — concatenated feature vector

Slot layout: [my_a, my_b, opp_a, opp_b, my_bench0, my_bench1, opp_bench0, opp_bench1]

Own-team slots use full data from |request|.
Opponent slots use progressively revealed data (moves/tera only, ability/item stay 0).
"""

from __future__ import annotations

import json
import numpy as np
from live_parser import LiveBattleState

# ── Constants (must match dataset.py exactly) ────────────────────────────────

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


# ── Encoding functions (numpy versions of dataset.py) ────────────────────────


def encode_active(feat: np.ndarray, off: int, state: dict | None) -> None:
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


def encode_bench(feat: np.ndarray, off: int, hp: float, fainted: bool,
                 status: str | None, present: bool) -> None:
    """Encode one bench pokemon's 10 numeric features at offset."""
    feat[off] = hp / 100.0
    feat[off + 1] = float(fainted)
    feat[off + 2 + STATUS_IDX.get(status, 0)] = 1.0
    feat[off + 9] = float(present)


def encode_field(feat: np.ndarray, off: int, field: dict,
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


# ── Showdown ID normalization ────────────────────────────────────────────────

# Showdown sends move/ability/item names as lowercase IDs (e.g. "fakeout",
# "intimidate", "assaultvest"). The vocab uses display names (e.g. "Fake Out",
# "Intimidate", "Assault Vest"). We build reverse maps for lookup.

def _build_id_to_name(name_map: dict[str, int]) -> dict[str, str]:
    """Build lowercase-no-spaces ID -> display name mapping."""
    result = {}
    for name in name_map:
        if name.startswith("<"):
            continue
        # Showdown ID format: lowercase, remove spaces/hyphens/special chars
        sid = name.lower().replace(" ", "").replace("-", "").replace("'", "").replace(".", "")
        result[sid] = name
    return result


class LiveEncoder:
    """Encodes LiveBattleState into ONNX-ready numpy arrays."""

    def __init__(self, vocab_path: str):
        with open(vocab_path, encoding="utf-8") as f:
            self.vocab = json.load(f)

        self.species_map: dict[str, int] = self.vocab["species"]
        self.action_map: dict[str, int] = self.vocab["actions"]
        self.move_map: dict[str, int] = self.vocab["moves"]
        self.ability_map: dict[str, int] = self.vocab["abilities"]
        self.item_map: dict[str, int] = self.vocab["items"]
        self.tera_map: dict[str, int] = self.vocab["tera_types"]

        self.sp_unk = self.species_map.get("<unknown>", 1)
        self.mv_unk = self.move_map.get("<unknown>", 1)
        self.ab_unk = self.ability_map.get("<unknown>", 1)
        self.it_unk = self.item_map.get("<unknown>", 1)
        self.te_unk = self.tera_map.get("<unknown>", 1)

        # Build Showdown ID -> display name maps
        self._move_id_to_name = _build_id_to_name(self.move_map)
        self._ability_id_to_name = _build_id_to_name(self.ability_map)
        self._item_id_to_name = _build_id_to_name(self.item_map)
        self._tera_id_to_name = _build_id_to_name(self.tera_map)

        # Reverse action map for decoding
        self._action_id_to_name = {v: k for k, v in self.action_map.items()}

    def _resolve_move(self, move_id: str) -> str:
        """Convert Showdown move ID to display name."""
        # Try direct lookup first (already display name from protocol lines)
        if move_id in self.move_map:
            return move_id
        # Try ID normalization
        sid = move_id.lower().replace(" ", "").replace("-", "").replace("'", "").replace(".", "")
        return self._move_id_to_name.get(sid, move_id)

    def _resolve_ability(self, ability_id: str) -> str:
        if ability_id in self.ability_map:
            return ability_id
        sid = ability_id.lower().replace(" ", "").replace("-", "").replace("'", "").replace(".", "")
        return self._ability_id_to_name.get(sid, ability_id)

    def _resolve_item(self, item_id: str) -> str:
        if item_id in self.item_map:
            return item_id
        sid = item_id.lower().replace(" ", "").replace("-", "").replace("'", "").replace(".", "")
        return self._item_id_to_name.get(sid, item_id)

    def _resolve_tera(self, tera_id: str) -> str:
        if tera_id in self.tera_map:
            return tera_id
        sid = tera_id.lower().replace(" ", "").replace("-", "").replace("'", "").replace(".", "")
        return self._tera_id_to_name.get(sid, tera_id)

    def encode_battle(self, state: LiveBattleState) -> dict[str, np.ndarray]:
        """Encode current battle state for the battle model.

        Returns dict of numpy arrays matching ONNX input names, each with
        batch dimension 1.
        """
        if not state.my_side:
            raise ValueError("Cannot encode: my_side not identified (no |request| received)")

        perspective = state.my_side
        opp = state.opp_side

        snap = state.snapshot()
        active = snap["active"]
        field_dict = snap["field"]

        my_a = active.get(f"{perspective}a")
        my_b = active.get(f"{perspective}b")
        opp_a = active.get(f"{opp}a")
        opp_b = active.get(f"{opp}b")

        species_ids = np.zeros(NUM_SPECIES_SLOTS, dtype=np.int64)
        move_ids = np.zeros((NUM_SPECIES_SLOTS, 4), dtype=np.int64)
        ability_ids = np.zeros(NUM_SPECIES_SLOTS, dtype=np.int64)
        item_ids = np.zeros(NUM_SPECIES_SLOTS, dtype=np.int64)
        tera_ids = np.zeros(NUM_SPECIES_SLOTS, dtype=np.int64)
        numeric = np.zeros(NUMERIC_DIM, dtype=np.float32)

        # ── Active slots: species IDs ──
        for si, st in enumerate([my_a, my_b, opp_a, opp_b]):
            if st:
                species_ids[si] = self.species_map.get(st["species"], self.sp_unk)

        # ── Own active slots: full features from own_team ──
        own_by_species = {p.species: p for p in state.own_team}
        for si, st in enumerate([my_a, my_b]):
            if st is None:
                continue
            sp = st["species"]
            poke = own_by_species.get(sp)
            if poke:
                for j, m in enumerate(poke.moves[:4]):
                    move_name = self._resolve_move(m)
                    move_ids[si, j] = self.move_map.get(move_name, self.mv_unk)
                ab = self._resolve_ability(poke.ability)
                ability_ids[si] = self.ability_map.get(ab, self.ab_unk)
                it = self._resolve_item(poke.item)
                item_ids[si] = self.item_map.get(it, self.it_unk)
                te = self._resolve_tera(poke.tera_type)
                tera_ids[si] = self.tera_map.get(te, self.te_unk)

        # ── Opponent active slots: progressive revealed ──
        opp_revealed = state.revealed.get(opp, {})
        for si_offset, st in enumerate([opp_a, opp_b]):
            si = si_offset + 2
            if st is None:
                continue
            sp = st["species"]
            ri = opp_revealed.get(sp)
            if ri:
                for j, m in enumerate(ri.moves[:4]):
                    move_ids[si, j] = self.move_map.get(m, self.mv_unk)
                if ri.tera_type:
                    tera_ids[si] = self.tera_map.get(ri.tera_type, self.te_unk)
                # Note: opponent ability/item stay at 0 (matches training)

        # ── Active numeric features ──
        encode_active(numeric, 0 * ACTIVE_DIM, my_a)
        encode_active(numeric, 1 * ACTIVE_DIM, my_b)
        encode_active(numeric, 2 * ACTIVE_DIM, opp_a)
        encode_active(numeric, 3 * ACTIVE_DIM, opp_b)

        # ── Bench ──
        bench_base = 4 * ACTIVE_DIM

        # Own bench
        my_active_sp = set()
        if my_a:
            my_active_sp.add(my_a["species"])
        if my_b:
            my_active_sp.add(my_b["species"])
        my_bench = [p for p in state.own_team if p.species not in my_active_sp]

        for bi, poke in enumerate(my_bench[:2]):
            off = bench_base + bi * BENCH_DIM
            species_ids[4 + bi] = self.species_map.get(poke.species, self.sp_unk)
            # Fill moves/ability/item/tera from own_team
            for j, m in enumerate(poke.moves[:4]):
                move_name = self._resolve_move(m)
                move_ids[4 + bi, j] = self.move_map.get(move_name, self.mv_unk)
            ab = self._resolve_ability(poke.ability)
            ability_ids[4 + bi] = self.ability_map.get(ab, self.ab_unk)
            it = self._resolve_item(poke.item)
            item_ids[4 + bi] = self.item_map.get(it, self.it_unk)
            te = self._resolve_tera(poke.tera_type)
            tera_ids[4 + bi] = self.tera_map.get(te, self.te_unk)
            encode_bench(numeric, off, poke.hp, poke.fainted, poke.status, True)

        # Opponent bench
        opp_active_sp = set()
        if opp_a:
            opp_active_sp.add(opp_a["species"])
        if opp_b:
            opp_active_sp.add(opp_b["species"])
        opp_bench_species = [s for s in state.team_brought.get(opp, [])
                             if s not in opp_active_sp]

        for bi, sp in enumerate(opp_bench_species[:2]):
            off = bench_base + (2 + bi) * BENCH_DIM
            species_ids[6 + bi] = self.species_map.get(sp, self.sp_unk)
            # Opponent bench: progressive moves/tera only
            ri = opp_revealed.get(sp)
            if ri:
                for j, m in enumerate(ri.moves[:4]):
                    move_ids[6 + bi, j] = self.move_map.get(m, self.mv_unk)
                if ri.tera_type:
                    tera_ids[6 + bi] = self.tera_map.get(ri.tera_type, self.te_unk)
            # Bench state from state.bench
            bench_info = state.bench.get(opp, {}).get(sp)
            if bench_info:
                encode_bench(numeric, off, bench_info["hp"], bench_info.get("fainted", False),
                             bench_info.get("status"), True)
            else:
                encode_bench(numeric, off, 100, False, None, True)

        # ── Field ──
        field_off = bench_base + 4 * BENCH_DIM
        encode_field(numeric, field_off, field_dict, perspective)
        numeric[field_off + 19] = state.current_turn / 20.0

        # Add batch dimension
        return {
            "species_ids": species_ids.reshape(1, -1),
            "move_ids": move_ids.reshape(1, NUM_SPECIES_SLOTS, 4),
            "ability_ids": ability_ids.reshape(1, -1),
            "item_ids": item_ids.reshape(1, -1),
            "tera_ids": tera_ids.reshape(1, -1),
            "numeric": numeric.reshape(1, -1),
        }

    def encode_team_preview(self, state: LiveBattleState) -> dict[str, np.ndarray]:
        """Encode team preview state for the team preview model.

        12 slots: 6 own + 6 opponent. Own team from |request|, opponent from |poke|.
        """
        if not state.my_side:
            raise ValueError("Cannot encode: my_side not identified")

        perspective = state.my_side
        opp = state.opp_side

        species_ids = np.zeros(12, dtype=np.int64)
        move_ids = np.zeros((12, 4), dtype=np.int64)
        ability_ids = np.zeros(12, dtype=np.int64)
        item_ids = np.zeros(12, dtype=np.int64)
        tera_ids = np.zeros(12, dtype=np.int64)

        # Own team (slots 0-5) from own_team
        for i, poke in enumerate(state.own_team[:6]):
            species_ids[i] = self.species_map.get(poke.species, self.sp_unk)
            for j, m in enumerate(poke.moves[:4]):
                move_name = self._resolve_move(m)
                move_ids[i, j] = self.move_map.get(move_name, self.mv_unk)
            ab = self._resolve_ability(poke.ability)
            ability_ids[i] = self.ability_map.get(ab, self.ab_unk)
            it = self._resolve_item(poke.item)
            item_ids[i] = self.item_map.get(it, self.it_unk)
            te = self._resolve_tera(poke.tera_type)
            tera_ids[i] = self.tera_map.get(te, self.te_unk)

        # Opponent team (slots 6-11) from team_preview
        opp_preview = state.team_preview.get(opp, [])
        for i, poke in enumerate(opp_preview[:6]):
            si = 6 + i
            species_ids[si] = self.species_map.get(poke["species"], self.sp_unk)
            # No moves/ability/item/tera known at team preview for opponent

        return {
            "species_ids": species_ids.reshape(1, -1),
            "move_ids": move_ids.reshape(1, 12, 4),
            "ability_ids": ability_ids.reshape(1, -1),
            "item_ids": item_ids.reshape(1, -1),
            "tera_ids": tera_ids.reshape(1, -1),
        }

    def build_action_mask(self, request: dict) -> tuple[np.ndarray, np.ndarray]:
        """Build legal action masks from |request| JSON for slot A and slot B.

        Returns two boolean arrays over the action vocab. True = legal.
        """
        num_actions = len(self.action_map)
        mask_a = np.zeros(num_actions, dtype=bool)
        mask_b = np.zeros(num_actions, dtype=bool)

        active_moves = request.get("active", [])
        force_switch = request.get("forceSwitch", [])
        side_pokemon = request.get("side", {}).get("pokemon", [])

        for slot_idx, mask in enumerate([mask_a, mask_b]):
            is_force = slot_idx < len(force_switch) and force_switch[slot_idx]

            if not is_force and slot_idx < len(active_moves):
                # Legal moves
                for move_data in active_moves[slot_idx].get("moves", []):
                    if move_data.get("disabled"):
                        continue
                    # move_data["move"] is display name, move_data["id"] is Showdown ID
                    move_name = move_data.get("move", "")
                    action_key = f"move:{move_name}"
                    if action_key in self.action_map:
                        mask[self.action_map[action_key]] = True
                    else:
                        # Try resolving from ID
                        resolved = self._resolve_move(move_data.get("id", ""))
                        action_key = f"move:{resolved}"
                        if action_key in self.action_map:
                            mask[self.action_map[action_key]] = True

            # Legal switches — always check (move turn or force switch)
            for poke in side_pokemon:
                if poke.get("active"):
                    continue
                condition = poke.get("condition", "")
                if "fnt" in condition:
                    continue
                details = poke.get("details", "")
                species = details.split(",")[0].strip()
                action_key = f"switch:{species}"
                if action_key in self.action_map:
                    mask[self.action_map[action_key]] = True

        return mask_a, mask_b

    def decode_actions(self, probs: np.ndarray, top_k: int = 5) -> list[dict]:
        """Decode top-k actions from probability array to human-readable labels."""
        top_indices = np.argsort(probs)[::-1][:top_k]
        result = []
        for idx in top_indices:
            if probs[idx] < 0.001:
                continue
            action_name = self._action_id_to_name.get(int(idx), f"action_{idx}")
            result.append({
                "action": action_name,
                "prob": float(probs[idx]),
                "index": int(idx),
            })
        return result
