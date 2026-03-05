"""
Pokemon Showdown Replay Parser

Parses replay JSON files into structured data for analysis and DL training.

Output per replay:
  - Metadata: players, ratings (before/after), winner, format
  - Teams: full team preview + which 4 were brought
  - Turn-by-turn: state snapshot at start of each turn + actions taken
  - Revealed info: moves, items, abilities, tera types seen per pokemon

Always generates 3 output files per format, filtered by player rating:
  - parsed.jsonl      — all games
  - parsed_1200.jsonl — both players rated >= 1200
  - parsed_1500.jsonl — both players rated >= 1500

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


# ── Pre-compiled regexes ──────────────────────────────────────────────────────

_RE_RATING = re.compile(
    r"(.+?)'s rating: (\d+) &rarr; <strong>(\d+)</strong>"
)
_RE_FROM_ABILITY = re.compile(r"\[from\] ability: (.+?)(?:\||\[|$)")
_RE_OF_SLOT = re.compile(r"\[of\] (p[12][ab]): (.+?)(?:\||$)")
_RE_FROM_ITEM = re.compile(r"\[from\] item: (.+?)(?:\||\[|$)")


# ── Data structures ──────────────────────────────────────────────────────────


@dataclass
class Pokemon:
    species: str
    level: int = 50
    gender: str | None = None  # M, F, or None (genderless)


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
        d = {}
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


@dataclass
class TurnAction:
    """An action taken by one active pokemon in a turn."""
    slot: str  # p1a, p1b, p2a, p2b
    action_type: str  # "move", "switch", "cant", "none"
    detail: str = ""  # move name, or switch-in species, or cant reason
    target: str = ""  # target slot if applicable
    tera: str | None = None  # tera type if terastallized this turn
    mega: bool = False


@dataclass
class TurnData:
    turn: int
    # State at start of turn (before actions)
    active: dict[str, dict]  # slot -> {species, hp, status, boosts, tera}
    field: dict  # field state
    # Actions taken this turn
    actions: list[dict]  # list of action dicts
    # What happened
    faints: list[str]  # slots that fainted (e.g. ["p1a", "p2b"])
    switches_in: list[dict]  # forced switches after faints


@dataclass
class RevealedInfo:
    """Information revealed about a pokemon during the battle."""
    moves: list[str] = field(default_factory=list)
    item: str | None = None
    ability: str | None = None
    tera_type: str | None = None


# ── Parser ───────────────────────────────────────────────────────────────────


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
    # Strip shiny/tera markers from species detail
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
        elif p == "shiny":
            pass
        elif p.startswith("tera:"):
            pass  # already terastallized info
    return species, level, gender


def player_side(slot: str) -> str:
    """'p1a' -> 'p1', 'p2b' -> 'p2'."""
    return slot[:2]


def parse_replay(replay_data: dict) -> dict | None:
    """Parse a full replay JSON into structured data. Returns None on parse failure."""
    log = replay_data.get("log", "")
    if not log:
        return None

    lines = log.split("\n")

    # ── Metadata ──
    players: dict[str, dict] = {}
    team_preview: dict[str, list[dict]] = {"p1": [], "p2": []}
    team_brought: dict[str, list[str]] = {"p1": [], "p2": []}
    winner: str | None = None  # "p1", "p2", or None (tie/forfeit)
    winner_name: str | None = None
    is_doubles = False

    # ── Runtime state ──
    active: dict[str, ActivePokemon] = {}  # slot -> ActivePokemon
    # bench tracks pokemon not currently on field: species -> {hp, max_hp, status, fainted}
    bench: dict[str, dict[str, dict]] = {"p1": {}, "p2": {}}
    field = FieldState()
    nickname_to_species: dict[str, str] = {}  # "p1a: Lunala" key -> species
    slot_species: dict[str, str] = {}  # slot -> current species

    # ── Revealed info ──
    revealed: dict[str, dict[str, RevealedInfo]] = {"p1": {}, "p2": {}}

    # ── Turn tracking ──
    turns: list[dict] = []
    current_turn = 0
    turn_actions: list[TurnAction] = []
    turn_faints: list[str] = []
    turn_switches: list[dict] = []
    turn_tera: dict[str, str] = {}
    pending_state: dict | None = None  # state snapshot at turn start
    pending_field: dict | None = None  # field snapshot at turn start
    pending_bench: dict | None = None  # bench snapshot at turn start

    def snapshot_state() -> dict:
        """Capture current active pokemon state."""
        active_snapshot = {}
        for slot, poke in active.items():
            if poke is not None:
                active_snapshot[slot] = {
                    "species": poke.species,
                    "hp": poke.hp,
                    "status": poke.status,
                    "boosts": dict(poke.boosts) if poke.boosts else {},
                    "tera": poke.tera_type,
                    "fainted": poke.fainted,
                }
        return active_snapshot

    def snapshot_bench() -> dict[str, list[dict]]:
        """Capture bench pokemon state for both sides."""
        result = {}
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
        key = f"{slot}: {nickname}"
        # Try exact slot match first
        if slot in slot_species:
            return slot_species[slot]
        # Fall back to nickname map
        return nickname_to_species.get(key, nickname)

    def flush_turn():
        """Save the current turn's data."""
        nonlocal pending_state, pending_field, pending_bench
        if current_turn == 0:
            return
        if pending_state is None:
            return

        actions = []
        for a in turn_actions:
            ad = {
                "slot": a.slot,
                "type": a.action_type,
                "detail": a.detail,
            }
            if a.target:
                ad["target"] = a.target
            if a.tera:
                ad["tera"] = a.tera
            actions.append(ad)

        turn_data = {
            "turn": current_turn,
            "active": pending_state,
            "field": pending_field or {},
            "actions": actions,
            "faints": list(turn_faints),
        }
        if pending_bench:
            turn_data["bench"] = pending_bench
        turns.append(turn_data)

    # ── Parse lines ──
    for line in lines:
        if not line.startswith("|"):
            continue

        parts = line.split("|")
        # parts[0] is empty string before first |
        if len(parts) < 2:
            continue
        cmd = parts[1]

        try:
            # ── Metadata ──
            if cmd == "player" and len(parts) >= 4:
                pid = parts[2].strip()  # p1 or p2
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

            # ── Team preview ──
            elif cmd == "poke" and len(parts) >= 4:
                side = parts[2].strip()
                detail = parts[3].strip()
                # Strip trailing | item info
                species, level, gender = parse_species_detail(detail)
                team_preview[side].append({
                    "species": species,
                    "level": level,
                    "gender": gender,
                })

            # ── Team brought (from teamsize) ──
            elif cmd == "teamsize" and len(parts) >= 4:
                pass  # We track brought via switches

            # ── Turn marker ──
            elif cmd == "turn" and len(parts) >= 3:
                # Flush previous turn
                flush_turn()
                current_turn = int(parts[2].strip())
                turn_actions = []
                turn_faints = []
                turn_switches = []
                turn_tera = {}
                pending_state = snapshot_state()
                pending_field = field.to_dict()
                pending_bench = snapshot_bench()

            # ── Switch / Drag (forced switch) ──
            elif cmd in ("switch", "drag") and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                detail = parts[3].strip()
                hp_str = parts[4].strip()
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

                # Track nickname -> species mapping
                nickname_to_species[f"{slot}: {nickname}"] = species
                slot_species[slot] = species

                if species not in team_brought.get(side, []):
                    if side not in team_brought:
                        team_brought[side] = []
                    team_brought[side].append(species)

                active[slot] = ActivePokemon(
                    species=species,
                    slot=slot,
                    nickname=nickname,
                    hp=hp,
                    max_hp=max_hp,
                    status=status,
                )

                # If this is within a turn (not initial leads), record as action
                if current_turn > 0 and cmd == "switch":
                    turn_actions.append(TurnAction(
                        slot=slot,
                        action_type="switch",
                        detail=species,
                    ))

            # ── Move ──
            elif cmd == "move" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                move_name = parts[3].strip()
                target = ""
                if len(parts) >= 5 and parts[4].strip():
                    target_str = parts[4].strip()
                    if ":" in target_str:
                        target, _ = parse_slot(target_str)

                side = player_side(slot)
                species = resolve_species(slot, nickname)
                ri = get_revealed(side, species)
                if move_name not in ri.moves:
                    ri.moves.append(move_name)

                # Check if tera happened this turn for this slot
                tera = turn_tera.get(slot)

                turn_actions.append(TurnAction(
                    slot=slot,
                    action_type="move",
                    detail=move_name,
                    target=target,
                    tera=tera,
                ))

            # ── Can't move ──
            elif cmd == "cant" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                reason = parts[3].strip()
                turn_actions.append(TurnAction(
                    slot=slot,
                    action_type="cant",
                    detail=reason,
                ))

            # ── Damage ──
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

            # ── Heal ──
            elif cmd == "-heal" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                hp_str = parts[3].strip()
                hp, max_hp = parse_hp(hp_str)
                if slot in active:
                    active[slot].hp = hp
                    active[slot].max_hp = max_hp
                    status = parse_status_from_hp(hp_str)
                    if status:
                        active[slot].status = status

            # ── Faint ──
            elif cmd == "faint" and len(parts) >= 3:
                slot, nickname = parse_slot(parts[2].strip())
                if slot in active:
                    active[slot].hp = 0
                    active[slot].fainted = True
                    # Move fainted pokemon to bench so it appears in bench snapshots
                    side = player_side(slot)
                    bench[side][active[slot].species] = {
                        "hp": 0,
                        "max_hp": active[slot].max_hp,
                        "status": active[slot].status,
                        "fainted": True,
                    }
                turn_faints.append(slot)

            # ── Boost / Unboost ──
            elif cmd == "-boost" and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                stat = parts[3].strip()
                amount = int(parts[4].strip())
                if slot in active:
                    active[slot].boosts[stat] = active[slot].boosts.get(stat, 0) + amount

            elif cmd == "-unboost" and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                stat = parts[3].strip()
                amount = int(parts[4].strip())
                if slot in active:
                    active[slot].boosts[stat] = active[slot].boosts.get(stat, 0) - amount

            # ── Status ──
            elif cmd == "-status" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                status = parts[3].strip()
                if slot in active:
                    active[slot].status = status

            elif cmd == "-curestatus" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                if slot in active:
                    active[slot].status = None

            # ── Terastallize ──
            elif cmd == "-terastallize" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                tera_type = parts[3].strip()
                if slot in active:
                    active[slot].tera_type = tera_type
                turn_tera[slot] = tera_type
                side = player_side(slot)
                species = resolve_species(slot, nickname)
                get_revealed(side, species).tera_type = tera_type

            # ── Weather ──
            elif cmd == "-weather" and len(parts) >= 3:
                weather = parts[2].strip()
                if weather == "none":
                    field.weather = None
                elif "[upkeep]" not in line:
                    field.weather = weather

            # ── Field start/end (terrain, trick room) ──
            elif cmd == "-fieldstart" and len(parts) >= 3:
                effect = parts[2].strip()
                effect_lower = effect.lower()
                if "terrain" in effect_lower:
                    field.terrain = effect.replace("move: ", "")
                elif "trick room" in effect_lower:
                    field.trick_room = True

            elif cmd == "-fieldend" and len(parts) >= 3:
                effect = parts[2].strip()
                effect_lower = effect.lower()
                if "terrain" in effect_lower:
                    field.terrain = None
                elif "trick room" in effect_lower:
                    field.trick_room = False

            # ── Side conditions (screens, tailwind) ──
            elif cmd == "-sidestart" and len(parts) >= 4:
                side_str = parts[2].strip()
                side = "p1" if "p1" in side_str else "p2"
                effect = parts[3].strip().lower()
                if "tailwind" in effect:
                    setattr(field, f"{side}_tailwind", True)
                elif "reflect" in effect:
                    setattr(field, f"{side}_reflect", True)
                elif "light screen" in effect:
                    setattr(field, f"{side}_light_screen", True)
                elif "aurora veil" in effect:
                    setattr(field, f"{side}_aurora_veil", True)

            elif cmd == "-sideend" and len(parts) >= 4:
                side_str = parts[2].strip()
                side = "p1" if "p1" in side_str else "p2"
                effect = parts[3].strip().lower()
                if "tailwind" in effect:
                    setattr(field, f"{side}_tailwind", False)
                elif "reflect" in effect:
                    setattr(field, f"{side}_reflect", False)
                elif "light screen" in effect:
                    setattr(field, f"{side}_light_screen", False)
                elif "aurora veil" in effect:
                    setattr(field, f"{side}_aurora_veil", False)

            # ── Item revealed ──
            elif cmd in ("-enditem", "-item") and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                item = parts[3].strip()
                side = player_side(slot)
                species = resolve_species(slot, nickname)
                get_revealed(side, species).item = item

            # ── Ability revealed ──
            elif cmd == "-ability" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                ability = parts[3].strip()
                side = player_side(slot)
                species = resolve_species(slot, nickname)
                get_revealed(side, species).ability = ability

            # ── Swap (slot swap, e.g. Ally Switch) ──
            elif cmd == "swap" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                # In doubles, swap slots a <-> b on same side
                side = player_side(slot)
                slot_a = f"{side}a"
                slot_b = f"{side}b"
                if slot_a in active and slot_b in active:
                    active[slot_a], active[slot_b] = active[slot_b], active[slot_a]
                    active[slot_a].slot = slot_a
                    active[slot_b].slot = slot_b
                    # Update slot_species
                    slot_species[slot_a], slot_species[slot_b] = (
                        slot_species.get(slot_b, ""),
                        slot_species.get(slot_a, ""),
                    )

            # ── Details change (form change) ──
            elif cmd == "detailschange" and len(parts) >= 4:
                slot, nickname = parse_slot(parts[2].strip())
                detail = parts[3].strip()
                new_species, level, gender = parse_species_detail(detail)
                if slot in active:
                    active[slot].species = new_species
                slot_species[slot] = new_species

            # ── Winner ──
            elif cmd == "win" and len(parts) >= 3:
                winner_name = parts[2].strip()
                if players.get("p1", {}).get("name") == winner_name:
                    winner = "p1"
                elif players.get("p2", {}).get("name") == winner_name:
                    winner = "p2"

            # ── Rating changes ──
            elif cmd == "raw" and len(parts) >= 3:
                raw_text = parts[2].strip()
                # Parse: "Name's rating: 1305 &rarr; <strong>1333</strong><br />..."
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

            # ── Ability from [from] tags (in other events) ──
            # Many events have [from] ability: X at the end
            if "[from] ability:" in line:
                m = _RE_FROM_ABILITY.search(line)
                if m and len(parts) >= 3:
                    # Try to find which pokemon this is about via [of]
                    of_match = _RE_OF_SLOT.search(line)
                    if of_match:
                        slot = of_match.group(1)
                        side = player_side(slot)
                        species = resolve_species(slot, of_match.group(2).strip())
                        get_revealed(side, species).ability = m.group(1).strip()

            # ── Item from [from] item: tags ──
            if "[from] item:" in line:
                m = _RE_FROM_ITEM.search(line)
                if m and len(parts) >= 3:
                    # The pokemon with the item is the subject of the event
                    try:
                        slot, nickname = parse_slot(parts[2].strip())
                        side = player_side(slot)
                        species = resolve_species(slot, nickname)
                        get_revealed(side, species).item = m.group(1).strip()
                    except (IndexError, ValueError):
                        pass

        except (IndexError, ValueError):
            continue  # Skip malformed lines

    # Flush final turn
    flush_turn()

    if not players:
        return None

    # ── Build output ──
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
        "turns": turns,
        "turn_count": current_turn,
        "revealed": revealed_out,
    }
    return result


# ── Rating tiers ─────────────────────────────────────────────────────────────

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


# ── Worker for multiprocessing ────────────────────────────────────────────────


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

        # Extract minimum player rating for tier assignment
        players = result.get("players", {})
        r1 = players.get("p1", {}).get("rating_before")
        r2 = players.get("p2", {}).get("rating_before")
        min_rating = min(r1, r2) if r1 is not None and r2 is not None else None

        return json_dumps(result) + "\n", "ok", min_rating

    except Exception:
        return None, "error", None


# ── Batch processing ─────────────────────────────────────────────────────────

_WRITE_BUFFER_SIZE = 4096  # flush to disk every N results


def parse_all(
    format_id: str,
    output_dir: str | None = None,
    single_file: str | None = None,
    workers: int | None = None,
) -> None:
    if single_file:
        # Parse a single file for debugging
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

    # Build output paths for each rating tier
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

    # Open all output files
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
                    # Write to each tier file where the game qualifies
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

            # Flush remaining buffers
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
        print(f"  {name}: {tier_counts[name]:,} games → {tier_paths[name]}")


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
