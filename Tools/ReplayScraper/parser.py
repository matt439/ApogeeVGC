"""
Pokemon Showdown Replay Parser

Parses replay JSON files into structured data for analysis and DL training.

Output per replay:
  - Metadata: players, ratings (before/after), winner, format
  - Teams: full team preview + which 4 were brought
  - Decision points: state snapshot + actions at every player decision
  - Revealed info: moves, items, abilities, tera types seen per pokemon

Decision points are emitted whenever a player must choose an action:
  - turn_start:   both players choose moves/switches at the start of each turn
  - faint_switch:  a player must send in a replacement after a KO
  - force_switch:  a player chooses a switch-in after U-turn, Volt Switch, etc.

Always generates 3 output files per format, filtered by player rating:
  - parsed.jsonl      -- all games
  - parsed_1200.jsonl -- both players rated >= 1200
  - parsed_1500.jsonl -- both players rated >= 1500

Usage:
  python parser.py                                    # parse all downloaded replays
  python parser.py --format gen9vgc2025regi           # specific format
  python parser.py --output-dir path/to/dir           # custom output directory
  python parser.py --single path/to/replay.json       # parse one file (debug)
"""

from __future__ import annotations

import argparse
import json
import multiprocessing
import os
import re
import sys
from dataclasses import dataclass, field
from enum import Enum, auto
from pathlib import Path
from typing import Any

try:
    import orjson

    def json_loads(b):
        return orjson.loads(b)

    def json_dumps(obj, **kw):
        return orjson.dumps(obj).decode()

    def json_dumps_pretty(obj):
        return orjson.dumps(obj, option=orjson.OPT_INDENT_2).decode()
except ImportError:
    def json_loads(b):
        return json.loads(b)

    def json_dumps(obj, **kw):
        return json.dumps(obj, ensure_ascii=False)

    def json_dumps_pretty(obj):
        return json.dumps(obj, indent=2, ensure_ascii=False)


# -- Pre-compiled regexes -----------------------------------------------------

_RE_RATING = re.compile(
    r"(.+?)'s rating: (\d+) &rarr; <strong>(\d+)</strong>"
)
_RE_FROM_ABILITY = re.compile(r"\[from\] ability: (.+?)(?:\||\[|$)")
_RE_OF_SLOT = re.compile(r"\[of\] (p[12][ab]): (.+?)(?:\||$)")
_RE_FROM_ITEM = re.compile(r"\[from\] item: (.+?)(?:\||\[|$)")
_RE_FROM_MOVE = re.compile(r"\[from\] (?:move: )?(.+?)(?:\||\[|$)")

# Moves that cause the USER to switch out (player chooses replacement)
_SELF_SWITCH_MOVES = frozenset({
    "U-turn", "Volt Switch", "Flip Turn", "Parting Shot", "Baton Pass",
    "Teleport",
})

# Phazing moves drag opponent out with no choice (use |drag| not |switch|)
_PHAZING_MOVES = frozenset({
    "Whirlwind", "Roar", "Dragon Tail", "Circle Throw",
})


# -- Data structures ----------------------------------------------------------


@dataclass
class ActivePokemon:
    species: str
    slot: str  # e.g. "p1a", "p2b"
    nickname: str = ""
    hp: int = 100  # percentage
    max_hp: int = 100
    status: str | None = None  # par, brn, slp, psn, tox, frz
    boosts: dict[str, int] = field(default_factory=dict)
    tera_type: str | None = None  # set when terastallized
    fainted: bool = False


@dataclass
class FieldState:
    weather: str | None = None
    terrain: str | None = None
    trick_room: bool = False
    # per-side
    p1_tailwind: bool = False
    p2_tailwind: bool = False
    p1_reflect: bool = False
    p2_reflect: bool = False
    p1_light_screen: bool = False
    p2_light_screen: bool = False
    p1_aurora_veil: bool = False
    p2_aurora_veil: bool = False

    def to_dict(self) -> dict:
        d: dict[str, Any] = {}
        if self.weather:
            d["weather"] = self.weather
        if self.terrain:
            d["terrain"] = self.terrain
        if self.trick_room:
            d["trick_room"] = True
        for side in ("p1", "p2"):
            for screen in ("tailwind", "reflect", "light_screen", "aurora_veil"):
                if getattr(self, f"{side}_{screen}"):
                    d[f"{side}_{screen}"] = True
        return d

    def swap_sides(self) -> None:
        """Swap all side conditions between p1 and p2 (Court Change)."""
        for screen in ("tailwind", "reflect", "light_screen", "aurora_veil"):
            p1_attr = f"p1_{screen}"
            p2_attr = f"p2_{screen}"
            p1_val = getattr(self, p1_attr)
            p2_val = getattr(self, p2_attr)
            setattr(self, p1_attr, p2_val)
            setattr(self, p2_attr, p1_val)


@dataclass
class RevealedInfo:
    """Information revealed about a pokemon during the battle."""
    moves: list[str] = field(default_factory=list)
    item: str | None = None
    ability: str | None = None
    tera_type: str | None = None


class Phase(Enum):
    PRE_BATTLE = auto()     # before turn 1
    TURN_ACTIVE = auto()    # between |turn|N and |upkeep| (or next |turn|)
    BETWEEN_TURNS = auto()  # after |upkeep|, before next |turn|


# -- Parsing helpers -----------------------------------------------------------


def parse_hp(hp_str: str) -> tuple[int, int]:
    """Parse HP string like '80/100' or '0 fnt'. Returns (current, max)."""
    if "fnt" in hp_str:
        return 0, 100
    hp_str = hp_str.split()[0]  # strip status like "80/100 par"
    if "/" in hp_str:
        parts = hp_str.split("/")
        return int(parts[0]), int(parts[1])
    return int(hp_str), 100


def parse_status_from_hp(hp_str: str) -> str | None:
    """Extract status condition from HP string like '80/100 par'."""
    parts = hp_str.split()
    if len(parts) >= 2 and parts[-1] in ("par", "brn", "slp", "psn", "tox", "frz"):
        return parts[-1]
    return None


def parse_slot(pokemon_ref: str) -> tuple[str, str]:
    """Parse 'p1a: Nickname' into ('p1a', 'Nickname')."""
    parts = pokemon_ref.split(": ", 1)
    slot = parts[0].strip()
    nickname = parts[1].strip() if len(parts) > 1 else ""
    return slot, nickname


def parse_species_detail(detail: str) -> tuple[str, int, str | None]:
    """Parse 'Lunala, L50, F' into ('Lunala', 50, 'F')."""
    parts = [p.strip() for p in detail.split(",")]
    species = parts[0]
    species = species.replace(", shiny", "").strip()
    level = 50
    gender = None
    for p in parts[1:]:
        p = p.strip()
        if p.startswith("L"):
            try:
                level = int(p[1:])
            except ValueError:
                pass
        elif p in ("M", "F"):
            gender = p
        elif p in ("shiny",):
            pass
        elif p.startswith("tera:"):
            pass
    return species, level, gender


def player_side(slot: str) -> str:
    """'p1a' -> 'p1', 'p2b' -> 'p2'."""
    return slot[:2]


# -- Main parser ---------------------------------------------------------------


def parse_replay(replay_data: dict) -> dict | None:
    """Parse a full replay JSON into structured data. Returns None on parse failure."""
    log = replay_data.get("log", "")
    if not log:
        return None

    lines = log.split("\n")

    # -- Metadata --
    players: dict[str, dict] = {}
    team_preview: dict[str, list[dict]] = {"p1": [], "p2": []}
    team_brought: dict[str, list[str]] = {"p1": [], "p2": []}
    winner: str | None = None
    winner_name: str | None = None
    is_doubles = False

    # -- Runtime state --
    active: dict[str, ActivePokemon] = {}  # slot -> ActivePokemon
    bench: dict[str, dict[str, dict]] = {"p1": {}, "p2": {}}
    field_state = FieldState()
    nickname_to_species: dict[str, str] = {}
    slot_species: dict[str, str] = {}

    # -- Revealed info --
    revealed: dict[str, dict[str, RevealedInfo]] = {"p1": {}, "p2": {}}

    # -- Decision point tracking --
    decision_points: list[dict] = []
    current_turn = 0
    phase = Phase.PRE_BATTLE
    turn_actions: list[dict] = []
    turn_tera: dict[str, str] = {}
    pending_state: dict | None = None
    pending_field: dict | None = None
    pending_bench: dict | None = None

    # Faint switch tracking
    pending_faint_slots: list[str] = []  # slots needing replacement
    faint_switch_state: dict | None = None
    faint_switch_field: dict | None = None
    faint_switch_bench: dict | None = None
    faint_switch_actions: list[dict] = []

    def snapshot_active() -> dict:
        """Capture current active pokemon state."""
        snap = {}
        for slot, poke in active.items():
            if poke is not None:
                snap[slot] = {
                    "species": poke.species,
                    "hp": poke.hp,
                    "status": poke.status,
                    "boosts": dict(poke.boosts) if poke.boosts else {},
                    "tera": poke.tera_type,
                    "fainted": poke.fainted,
                }
        return snap

    def snapshot_bench() -> dict[str, list[dict]]:
        """Capture bench pokemon state for both sides."""
        result: dict[str, list[dict]] = {}
        for side in ("p1", "p2"):
            bench_list = []
            for species, info in bench[side].items():
                bench_list.append({
                    "species": species,
                    "hp": info["hp"],
                    "status": info["status"],
                    "fainted": info["fainted"],
                })
            if bench_list:
                result[side] = bench_list
        return result

    def get_revealed(side: str, species: str) -> RevealedInfo:
        if species not in revealed[side]:
            revealed[side][species] = RevealedInfo()
        return revealed[side][species]

    def resolve_species(slot: str, nickname: str) -> str:
        """Resolve a nickname to species, or return nickname if unknown."""
        if slot in slot_species:
            return slot_species[slot]
        key = f"{slot}: {nickname}"
        return nickname_to_species.get(key, nickname)

    def emit_turn_start():
        """Emit a turn_start decision point for the current turn."""
        nonlocal pending_state, pending_field, pending_bench
        if current_turn == 0 or pending_state is None:
            return

        dp = {
            "turn": current_turn,
            "type": "turn_start",
            "active": pending_state,
            "field": pending_field or {},
            "actions": list(turn_actions),
        }
        if pending_bench:
            dp["bench"] = pending_bench
        decision_points.append(dp)

    def emit_force_switch(slot: str, species_in: str, trigger: str):
        """Emit a force_switch decision point."""
        side = player_side(slot)
        dp = {
            "turn": current_turn,
            "type": "force_switch",
            "trigger": trigger,
            "deciding_side": side,
            "deciding_slots": [slot],
            "active": snapshot_active(),
            "field": field_state.to_dict(),
            "actions": [{"slot": slot, "type": "switch", "detail": species_in}],
        }
        b = snapshot_bench()
        if b:
            dp["bench"] = b
        decision_points.append(dp)

    def start_faint_switch_collection():
        """Snapshot state for pending faint switches."""
        nonlocal faint_switch_state, faint_switch_field, faint_switch_bench
        nonlocal faint_switch_actions
        faint_switch_state = snapshot_active()
        faint_switch_field = field_state.to_dict()
        faint_switch_bench = snapshot_bench()
        faint_switch_actions = []

    def flush_faint_switches():
        """Emit a faint_switch decision point if there are pending faint replacements."""
        nonlocal pending_faint_slots, faint_switch_state
        if not faint_switch_actions:
            pending_faint_slots.clear()
            faint_switch_state = None
            return

        # Determine which sides are deciding
        deciding_sides = sorted(set(
            player_side(a["slot"]) for a in faint_switch_actions
        ))
        deciding_slots = [a["slot"] for a in faint_switch_actions]

        dp = {
            "turn": current_turn,
            "type": "faint_switch",
            "deciding_sides": deciding_sides,
            "deciding_slots": deciding_slots,
            "active": faint_switch_state or {},
            "field": faint_switch_field or {},
            "actions": list(faint_switch_actions),
        }
        if faint_switch_bench:
            dp["bench"] = faint_switch_bench
        decision_points.append(dp)

        pending_faint_slots.clear()
        faint_switch_state = None
        faint_switch_actions.clear()

    def has_bench_alive(side: str) -> bool:
        """Check if a side has any non-fainted bench pokemon."""
        for info in bench[side].values():
            if not info.get("fainted", False):
                return True
        return False

    def do_switch(slot: str, nickname: str, detail: str, hp_str: str):
        """Process a switch/drag event and update state."""
        species, level, gender = parse_species_detail(detail)
        hp, max_hp = parse_hp(hp_str)
        status = parse_status_from_hp(hp_str)
        side = player_side(slot)

        # Save outgoing pokemon to bench
        if slot in active and active[slot] is not None:
            old = active[slot]
            bench[side][old.species] = {
                "hp": old.hp,
                "max_hp": old.max_hp,
                "status": old.status,
                "fainted": old.fainted,
            }

        # Remove incoming pokemon from bench
        if species in bench[side]:
            del bench[side][species]

        # Track mappings
        nickname_to_species[f"{slot}: {nickname}"] = species
        slot_species[slot] = species

        if species not in team_brought.get(side, []):
            team_brought[side].append(species)

        active[slot] = ActivePokemon(
            species=species,
            slot=slot,
            nickname=nickname,
            hp=hp,
            max_hp=max_hp,
            status=status,
        )

        return species

    # -- Parse lines --
    for line in lines:
        if not line.startswith("|"):
            continue

        parts = line.split("|")
        if len(parts) < 2:
            continue
        cmd = parts[1]

        try:
            # -- Metadata --
            if cmd == "player" and len(parts) >= 4:
                pid = parts[2].strip()
                name = parts[3].strip()
                rating = None
                if len(parts) >= 6 and parts[5].strip():
                    try:
                        rating = int(parts[5].strip())
                    except ValueError:
                        pass
                players[pid] = {"name": name, "rating_before": rating}

            elif cmd == "gametype":
                is_doubles = parts[2].strip() == "doubles"

            # -- Team preview --
            elif cmd == "poke" and len(parts) >= 4:
                side = parts[2].strip()
                detail = parts[3].strip()
                species, level, gender = parse_species_detail(detail)
                team_preview[side].append({
                    "species": species,
                    "level": level,
                    "gender": gender,
                })

            elif cmd == "teamsize":
                pass

            # -- Turn marker --
            elif cmd == "turn" and len(parts) >= 3:
                # Flush pending faint switches from between turns
                if pending_faint_slots:
                    flush_faint_switches()

                # Emit previous turn's decision point
                emit_turn_start()

                current_turn = int(parts[2].strip())
                phase = Phase.TURN_ACTIVE
                turn_actions = []
                turn_tera = {}
                pending_state = snapshot_active()
                pending_field = field_state.to_dict()
                pending_bench = snapshot_bench()

            # -- Upkeep (end of turn effects) --
            elif cmd == "upkeep":
                phase = Phase.BETWEEN_TURNS
                # Check if there are fainted pokemon that need replacements
                if pending_faint_slots:
                    # Filter to slots where the side has bench pokemon alive
                    pending_faint_slots = [
                        s for s in pending_faint_slots
                        if has_bench_alive(player_side(s))
                    ]
                    if pending_faint_slots:
                        start_faint_switch_collection()

            # -- Switch / Drag --
            elif cmd in ("switch", "drag", "replace") and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                detail = parts[3].strip()
                hp_str = parts[4].strip()
                side = player_side(slot)

                # Detect force switch from [from] tag
                from_tag = ""
                for p in parts[5:]:
                    stripped = p.strip()
                    if stripped.startswith("[from]"):
                        from_tag = stripped
                        break

                is_force_switch = False
                force_trigger = ""
                if cmd == "switch" and from_tag and current_turn > 0:
                    m = _RE_FROM_MOVE.search(from_tag)
                    if m:
                        trigger_name = m.group(1).strip()
                        if trigger_name not in _PHAZING_MOVES:
                            is_force_switch = True
                            force_trigger = trigger_name
                    elif "item:" in from_tag:
                        m2 = _RE_FROM_ITEM.search(from_tag)
                        if m2:
                            is_force_switch = True
                            force_trigger = m2.group(1).strip()
                    elif "ability:" in from_tag:
                        m2 = _RE_FROM_ABILITY.search(from_tag)
                        if m2:
                            is_force_switch = True
                            force_trigger = m2.group(1).strip()

                if is_force_switch:
                    # Snapshot state BEFORE the switch for the decision point
                    pre_switch_active = snapshot_active()
                    pre_switch_field = field_state.to_dict()
                    pre_switch_bench = snapshot_bench()

                    species_in = do_switch(slot, nickname, detail, hp_str)

                    # Emit force switch decision point with pre-switch state
                    dp = {
                        "turn": current_turn,
                        "type": "force_switch",
                        "trigger": force_trigger,
                        "deciding_side": side,
                        "deciding_slots": [slot],
                        "active": pre_switch_active,
                        "field": pre_switch_field,
                        "actions": [{"slot": slot, "type": "switch",
                                     "detail": species_in}],
                    }
                    if pre_switch_bench:
                        dp["bench"] = pre_switch_bench
                    decision_points.append(dp)

                elif cmd == "replace":
                    # Illusion breaking -- update species without treating as switch
                    species, level, gender = parse_species_detail(detail)
                    if slot in active:
                        old_species = active[slot].species
                        active[slot].species = species
                        active[slot].nickname = nickname
                        slot_species[slot] = species
                        nickname_to_species[f"{slot}: {nickname}"] = species
                        # Move revealed info from illusion species to real species
                        side = player_side(slot)
                        if old_species in revealed[side]:
                            old_info = revealed[side].pop(old_species)
                            ri = get_revealed(side, species)
                            ri.moves = old_info.moves
                            if old_info.item:
                                ri.item = old_info.item
                            if old_info.ability:
                                ri.ability = old_info.ability
                            if old_info.tera_type:
                                ri.tera_type = old_info.tera_type

                else:
                    # Check if this is a faint replacement (between turns)
                    is_faint_replacement = (
                        phase == Phase.BETWEEN_TURNS
                        and slot in pending_faint_slots
                    )

                    species_in = do_switch(slot, nickname, detail, hp_str)

                    if is_faint_replacement:
                        faint_switch_actions.append({
                            "slot": slot,
                            "type": "switch",
                            "detail": species_in,
                        })
                        pending_faint_slots.remove(slot)
                        # If all faint slots resolved, flush
                        if not pending_faint_slots:
                            flush_faint_switches()

                    elif current_turn > 0 and cmd == "switch":
                        # Voluntary switch as a turn action
                        turn_actions.append({
                            "slot": slot,
                            "type": "switch",
                            "detail": species_in,
                        })

            # -- Move --
            elif cmd == "move" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                move_name = parts[3].strip()
                target = ""
                if len(parts) >= 5 and parts[4].strip():
                    target_str = parts[4].strip()
                    if ":" in target_str and not target_str.startswith("["):
                        target, _ = parse_slot(target_str)

                side = player_side(slot)
                species = resolve_species(slot, nickname)
                ri = get_revealed(side, species)
                if move_name not in ri.moves:
                    ri.moves.append(move_name)

                tera = turn_tera.get(slot)

                action = {
                    "slot": slot,
                    "type": "move",
                    "detail": move_name,
                }
                if target:
                    action["target"] = target
                if tera:
                    action["tera"] = tera
                turn_actions.append(action)

            # -- Can't move --
            elif cmd == "cant" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                reason = parts[3].strip()
                turn_actions.append({
                    "slot": slot,
                    "type": "cant",
                    "detail": reason,
                })

            # -- Damage --
            elif cmd == "-damage" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                hp_str = parts[3].strip()
                hp, max_hp = parse_hp(hp_str)
                status = parse_status_from_hp(hp_str)
                if slot in active:
                    active[slot].hp = hp
                    active[slot].max_hp = max_hp
                    if status:
                        active[slot].status = status
                    if hp == 0:
                        active[slot].fainted = True

            # -- Heal --
            elif cmd == "-heal" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                hp_str = parts[3].strip()
                hp, max_hp = parse_hp(hp_str)
                if slot in active:
                    active[slot].hp = hp
                    active[slot].max_hp = max_hp
                    status = parse_status_from_hp(hp_str)
                    active[slot].status = status  # clear or set

            # -- Set HP (Pain Split) --
            elif cmd == "-sethp" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                hp_str = parts[3].strip()
                hp, max_hp = parse_hp(hp_str)
                if slot in active:
                    active[slot].hp = hp
                    active[slot].max_hp = max_hp
                # Pain Split can affect a second pokemon
                if len(parts) >= 6:
                    try:
                        slot2, nickname2 = parse_slot(parts[4].strip())
                        hp_str2 = parts[5].strip()
                        hp2, max_hp2 = parse_hp(hp_str2)
                        if slot2 in active:
                            active[slot2].hp = hp2
                            active[slot2].max_hp = max_hp2
                    except (IndexError, ValueError):
                        pass

            # -- Faint --
            elif cmd == "faint" and len(parts) >= 3:
                slot, nickname = parse_slot(parts[2].strip())
                if slot in active:
                    active[slot].hp = 0
                    active[slot].fainted = True
                    side = player_side(slot)
                    bench[side][active[slot].species] = {
                        "hp": 0,
                        "max_hp": active[slot].max_hp,
                        "status": active[slot].status,
                        "fainted": True,
                    }
                pending_faint_slots.append(slot)

            # -- Boost --
            elif cmd == "-boost" and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                stat = parts[3].strip()
                amount = int(parts[4].strip())
                if slot in active:
                    active[slot].boosts[stat] = active[slot].boosts.get(stat, 0) + amount

            # -- Unboost --
            elif cmd == "-unboost" and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                stat = parts[3].strip()
                amount = int(parts[4].strip())
                if slot in active:
                    active[slot].boosts[stat] = active[slot].boosts.get(stat, 0) - amount

            # -- Set boost (Belly Drum, etc.) --
            elif cmd == "-setboost" and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                stat = parts[3].strip()
                amount = int(parts[4].strip())
                if slot in active:
                    active[slot].boosts[stat] = amount

            # -- Clear boost (Clear Smog, etc.) --
            elif cmd == "-clearboost" and len(parts) >= 3:
                slot, nickname = parse_slot(parts[2].strip())
                if slot in active:
                    active[slot].boosts.clear()

            # -- Clear all boosts (Haze) --
            elif cmd == "-clearallboost":
                for poke in active.values():
                    if poke is not None:
                        poke.boosts.clear()

            # -- Copy boost (Psych Up, etc.) --
            elif cmd == "-copyboost" and len(parts) >= 4:
                src_slot, _ = parse_slot(parts[2].strip())
                dst_slot, _ = parse_slot(parts[3].strip())
                if src_slot in active and dst_slot in active:
                    active[dst_slot].boosts = dict(active[src_slot].boosts)

            # -- Swap boost (Speed Swap, etc.) --
            elif cmd == "-swapboost" and len(parts) >= 4:
                slot1, _ = parse_slot(parts[2].strip())
                slot2, _ = parse_slot(parts[3].strip())
                # parts[4] lists which stats, e.g. "spe" or "atk, def, spa, spd, spe"
                if slot1 in active and slot2 in active:
                    stats_to_swap = ["atk", "def", "spa", "spd", "spe"]
                    if len(parts) >= 5 and parts[4].strip():
                        stats_to_swap = [s.strip() for s in parts[4].strip().split(",")]
                    for stat in stats_to_swap:
                        v1 = active[slot1].boosts.get(stat, 0)
                        v2 = active[slot2].boosts.get(stat, 0)
                        if v2 != 0:
                            active[slot1].boosts[stat] = v2
                        elif stat in active[slot1].boosts:
                            del active[slot1].boosts[stat]
                        if v1 != 0:
                            active[slot2].boosts[stat] = v1
                        elif stat in active[slot2].boosts:
                            del active[slot2].boosts[stat]

            # -- Invert boost (Topsy-Turvy) --
            elif cmd == "-invertboost" and len(parts) >= 3:
                slot, _ = parse_slot(parts[2].strip())
                if slot in active:
                    active[slot].boosts = {
                        s: -v for s, v in active[slot].boosts.items()
                    }

            # -- Clear negative/positive boosts --
            elif cmd == "-clearnegativeboost" and len(parts) >= 3:
                slot, _ = parse_slot(parts[2].strip())
                if slot in active:
                    active[slot].boosts = {
                        s: v for s, v in active[slot].boosts.items() if v > 0
                    }

            elif cmd == "-clearpositiveboost" and len(parts) >= 3:
                slot, _ = parse_slot(parts[2].strip())
                if slot in active:
                    active[slot].boosts = {
                        s: v for s, v in active[slot].boosts.items() if v < 0
                    }

            # -- Status --
            elif cmd == "-status" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                status = parts[3].strip()
                if slot in active:
                    active[slot].status = status

            elif cmd == "-curestatus" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                if slot in active:
                    active[slot].status = None
                else:
                    # Bench pokemon cured (Heal Bell, Aromatherapy)
                    side = player_side(slot)
                    species = resolve_species(slot, nickname)
                    if species in bench[side]:
                        bench[side][species]["status"] = None

            elif cmd == "-cureteam" and len(parts) >= 3:
                # Heal Bell / Aromatherapy cures entire team
                slot, _ = parse_slot(parts[2].strip())
                side = player_side(slot)
                for poke in active.values():
                    if poke is not None and player_side(poke.slot) == side:
                        poke.status = None
                for info in bench[side].values():
                    info["status"] = None

            # -- Terastallize --
            elif cmd == "-terastallize" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                tera_type = parts[3].strip()
                if slot in active:
                    active[slot].tera_type = tera_type
                turn_tera[slot] = tera_type
                side = player_side(slot)
                species = resolve_species(slot, nickname)
                get_revealed(side, species).tera_type = tera_type

            # -- Weather --
            elif cmd == "-weather" and len(parts) >= 3:
                weather = parts[2].strip()
                if weather == "none":
                    field_state.weather = None
                elif "[upkeep]" not in line:
                    field_state.weather = weather

            # -- Field start/end (terrain, trick room) --
            elif cmd == "-fieldstart" and len(parts) >= 3:
                effect = parts[2].strip()
                effect_lower = effect.lower()
                if "terrain" in effect_lower:
                    field_state.terrain = effect.replace("move: ", "")
                elif "trick room" in effect_lower:
                    field_state.trick_room = True

            elif cmd == "-fieldend" and len(parts) >= 3:
                effect = parts[2].strip()
                effect_lower = effect.lower()
                if "terrain" in effect_lower:
                    field_state.terrain = None
                elif "trick room" in effect_lower:
                    field_state.trick_room = False

            # -- Side conditions (screens, tailwind) --
            elif cmd == "-sidestart" and len(parts) >= 4:
                side_str = parts[2].strip()
                side = "p1" if "p1" in side_str else "p2"
                effect = parts[3].strip().lower()
                if "tailwind" in effect:
                    setattr(field_state, f"{side}_tailwind", True)
                elif "reflect" in effect:
                    setattr(field_state, f"{side}_reflect", True)
                elif "light screen" in effect:
                    setattr(field_state, f"{side}_light_screen", True)
                elif "aurora veil" in effect:
                    setattr(field_state, f"{side}_aurora_veil", True)

            elif cmd == "-sideend" and len(parts) >= 4:
                side_str = parts[2].strip()
                side = "p1" if "p1" in side_str else "p2"
                effect = parts[3].strip().lower()
                if "tailwind" in effect:
                    setattr(field_state, f"{side}_tailwind", False)
                elif "reflect" in effect:
                    setattr(field_state, f"{side}_reflect", False)
                elif "light screen" in effect:
                    setattr(field_state, f"{side}_light_screen", False)
                elif "aurora veil" in effect:
                    setattr(field_state, f"{side}_aurora_veil", False)

            # -- Swap side conditions (Court Change) --
            elif cmd == "-swapsideconditions":
                field_state.swap_sides()

            # -- Item revealed --
            elif cmd in ("-enditem", "-item") and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                item = parts[3].strip()
                side = player_side(slot)
                species = resolve_species(slot, nickname)
                get_revealed(side, species).item = item

            # -- Ability revealed --
            elif cmd == "-ability" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                ability = parts[3].strip()
                side = player_side(slot)
                species = resolve_species(slot, nickname)
                get_revealed(side, species).ability = ability

            # -- Swap (slot swap, e.g. Ally Switch) --
            elif cmd == "swap" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                side = player_side(slot)
                slot_a = f"{side}a"
                slot_b = f"{side}b"
                if slot_a in active and slot_b in active:
                    active[slot_a], active[slot_b] = active[slot_b], active[slot_a]
                    active[slot_a].slot = slot_a
                    active[slot_b].slot = slot_b
                    slot_species[slot_a], slot_species[slot_b] = (
                        slot_species.get(slot_b, ""),
                        slot_species.get(slot_a, ""),
                    )

            # -- Form change (temporary, e.g. Aegislash) --
            elif cmd == "-formechange" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                detail = parts[3].strip()
                new_species, _, _ = parse_species_detail(detail)
                if slot in active:
                    active[slot].species = new_species
                slot_species[slot] = new_species

            # -- Details change (permanent form change) --
            elif cmd == "detailschange" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                detail = parts[3].strip()
                new_species, level, gender = parse_species_detail(detail)
                if slot in active:
                    active[slot].species = new_species
                slot_species[slot] = new_species

            # -- Transform (Ditto, etc.) --
            elif cmd == "-transform" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                target_ref = parts[3].strip()
                target_slot, target_nick = parse_slot(target_ref)
                if slot in active and target_slot in active:
                    target = active[target_slot]
                    active[slot].species = target.species
                    active[slot].boosts = dict(target.boosts)

            # -- Mega evolution --
            elif cmd == "-mega" and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                mega_species = parts[3].strip()
                mega_stone = parts[4].strip()
                side = player_side(slot)
                species = resolve_species(slot, nickname)
                get_revealed(side, species).item = mega_stone

            # -- Winner --
            elif cmd == "win" and len(parts) >= 3:
                winner_name = parts[2].strip()
                if players.get("p1", {}).get("name") == winner_name:
                    winner = "p1"
                elif players.get("p2", {}).get("name") == winner_name:
                    winner = "p2"

            # -- Rating changes --
            elif cmd == "raw" and len(parts) >= 3:
                raw_text = parts[2].strip()
                m = _RE_RATING.match(raw_text)
                if m:
                    name = m.group(1)
                    rating_before = int(m.group(2))
                    rating_after = int(m.group(3))
                    for pid, pdata in players.items():
                        if pdata["name"] == name:
                            pdata["rating_before"] = rating_before
                            pdata["rating_after"] = rating_after
                            break

            # -- Ability from [from] tags (in other events) --
            if "[from] ability:" in line:
                m = _RE_FROM_ABILITY.search(line)
                if m and len(parts) >= 3:
                    of_match = _RE_OF_SLOT.search(line)
                    if of_match:
                        slot = of_match.group(1)
                        side = player_side(slot)
                        species = resolve_species(slot, of_match.group(2).strip())
                        get_revealed(side, species).ability = m.group(1).strip()

            # -- Item from [from] item: tags --
            if "[from] item:" in line:
                m = _RE_FROM_ITEM.search(line)
                if m and len(parts) >= 3:
                    try:
                        slot, nickname = parse_slot(parts[2].strip())
                        side = player_side(slot)
                        species = resolve_species(slot, nickname)
                        get_revealed(side, species).item = m.group(1).strip()
                    except (IndexError, ValueError):
                        pass

        except (IndexError, ValueError):
            continue  # Skip malformed lines

    # Flush any pending faint switches
    if pending_faint_slots:
        flush_faint_switches()

    # Emit final turn's decision point
    emit_turn_start()

    if not players:
        return None

    # -- Build output --
    revealed_out: dict[str, dict[str, dict]] = {}
    for side in ("p1", "p2"):
        revealed_out[side] = {}
        for species, info in revealed[side].items():
            d: dict[str, Any] = {}
            if info.moves:
                d["moves"] = info.moves
            if info.item:
                d["item"] = info.item
            if info.ability:
                d["ability"] = info.ability
            if info.tera_type:
                d["tera_type"] = info.tera_type
            if d:
                revealed_out[side][species] = d

    result = {
        "id": replay_data.get("id", ""),
        "format": replay_data.get("formatid", ""),
        "players": players,
        "winner": winner,
        "team_preview": team_preview,
        "team_brought": team_brought,
        "decision_points": decision_points,
        "turn_count": current_turn,
        "revealed": revealed_out,
    }
    return result


# -- Rating tiers --------------------------------------------------------------

RATING_TIERS: list[tuple[str, int]] = [
    ("all", 0),
    ("1200+", 1200),
    ("1500+", 1500),
]


def tier_filename(tier_name: str) -> str:
    """Return the output filename for a rating tier."""
    if tier_name == "all":
        return "parsed.jsonl"
    return f"parsed_{tier_name.replace('+', '')}.jsonl"


# -- Worker for multiprocessing ------------------------------------------------


def _parse_file(filepath: str) -> tuple[str | None, str, int | None]:
    """Parse a single replay file.

    Returns (json_line | None, status, min_player_rating | None).
    status is one of: "ok", "error"
    min_player_rating is min(p1_rating_before, p2_rating_before) or None.
    """
    try:
        with open(filepath, "rb") as f:
            data = json_loads(f.read())

        result = parse_replay(data)
        if result is None:
            return None, "error", None

        players = result.get("players", {})
        r1 = players.get("p1", {}).get("rating_before")
        r2 = players.get("p2", {}).get("rating_before")
        min_rating = min(r1, r2) if r1 is not None and r2 is not None else None

        return json_dumps(result) + "\n", "ok", min_rating

    except Exception:
        return None, "error", None


# -- Batch processing ----------------------------------------------------------

_WRITE_BUFFER_SIZE = 4096  # flush to disk every N results


def parse_all(
    format_id: str,
    output_dir: str | None = None,
    single_file: str | None = None,
    workers: int | None = None,
) -> None:
    if single_file:
        with open(single_file, "rb") as f:
            data = json_loads(f.read())
        result = parse_replay(data)
        if result:
            sys.stdout.reconfigure(encoding="utf-8")
            print(json_dumps_pretty(result))
        else:
            print("Failed to parse replay", file=sys.stderr)
        return

    replay_dir = Path(__file__).parent / "data" / format_id / "replays"
    if not replay_dir.exists():
        print(f"Replay directory not found: {replay_dir}")
        return

    if output_dir is None:
        output_dir = str(Path(__file__).parent / "data" / format_id)

    out_dir = Path(output_dir)
    out_dir.mkdir(parents=True, exist_ok=True)

    files = [str(f) for f in replay_dir.glob("*.json")]
    total = len(files)
    if workers is None:
        workers = min(multiprocessing.cpu_count(), 16)

    tier_paths = {
        name: out_dir / tier_filename(name) for name, _ in RATING_TIERS
    }

    print(f"Parsing {total} replays from {replay_dir}")
    print(f"Output directory: {out_dir}")
    for name, min_r in RATING_TIERS:
        print(f"  {name} (min rating {min_r}): {tier_paths[name].name}")
    print(f"Workers: {workers}")

    parsed = 0
    skipped_error = 0
    tier_counts = {name: 0 for name, _ in RATING_TIERS}
    tier_bufs: dict[str, list[str]] = {name: [] for name, _ in RATING_TIERS}

    tier_files = {
        name: open(tier_paths[name], "w", encoding="utf-8")
        for name, _ in RATING_TIERS
    }

    try:
        with multiprocessing.Pool(processes=workers) as pool:
            for i, (line, status, min_player_rating) in enumerate(
                pool.imap_unordered(_parse_file, files, chunksize=256)
            ):
                if status == "ok":
                    parsed += 1
                    for name, min_r in RATING_TIERS:
                        if min_r == 0 or (
                            min_player_rating is not None
                            and min_player_rating >= min_r
                        ):
                            tier_bufs[name].append(line)
                            tier_counts[name] += 1
                            if len(tier_bufs[name]) >= _WRITE_BUFFER_SIZE:
                                tier_files[name].write(
                                    "".join(tier_bufs[name]))
                                tier_bufs[name].clear()
                else:
                    skipped_error += 1

                if (i + 1) % 10000 == 0:
                    print(f"  {i + 1}/{total} processed ({parsed} parsed)")

            for name, _ in RATING_TIERS:
                if tier_bufs[name]:
                    tier_files[name].write("".join(tier_bufs[name]))
    finally:
        for fh in tier_files.values():
            fh.close()

    print(f"\nDone:")
    print(f"  Total parsed: {parsed}")
    print(f"  Skipped (error): {skipped_error}")
    for name, min_r in RATING_TIERS:
        print(f"  {name}: {tier_counts[name]:,} games -> {tier_paths[name]}")


def main():
    parser = argparse.ArgumentParser(description="Parse Pokemon Showdown replays")
    parser.add_argument(
        "--format",
        default="gen9vgc2025regi",
        help="Showdown format ID (default: gen9vgc2025regi)",
    )
    parser.add_argument(
        "--output-dir",
        default=None,
        help="Output directory (default: data/<format>/)",
    )
    parser.add_argument(
        "--single",
        default=None,
        help="Parse a single replay file and print to stdout (for debugging)",
    )
    parser.add_argument(
        "--workers",
        type=int,
        default=None,
        help="Number of parallel workers (default: cpu_count, max 16)",
    )
    args = parser.parse_args()
    parse_all(args.format, args.output_dir, args.single, args.workers)


if __name__ == "__main__":
    main()
