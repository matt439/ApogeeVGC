"""
Pokemon Showdown Replay Parser

Parses replay JSON files into structured data for analysis and DL training.

Output per replay:
  - Metadata: players, ratings (before/after), winner, format
  - Teams: full team preview + which 4 were brought
  - Turn-by-turn: state snapshot at start of each turn + actions taken
  - Revealed info: moves, items, abilities, tera types seen per pokemon

Usage:
  python parser.py                                    # parse all downloaded replays
  python parser.py --format gen9vgc2025regi           # specific format
  python parser.py --min-rating 1400                  # filter by min avg rating
  python parser.py --output parsed.jsonl              # custom output path
  python parser.py --single path/to/replay.json       # parse one file (debug)
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any


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
        nonlocal pending_state, pending_field
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

        turns.append({
            "turn": current_turn,
            "active": pending_state,
            "field": pending_field or {},
            "actions": actions,
            "faints": list(turn_faints),
        })

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

            # ── Switch / Drag (forced switch) ──
            elif cmd in ("switch", "drag") and len(parts) >= 5:
                slot, nickname = parse_slot(parts[2].strip())
                detail = parts[3].strip()
                hp_str = parts[4].strip()
                species, level, gender = parse_species_detail(detail)
                hp, max_hp = parse_hp(hp_str)
                status = parse_status_from_hp(hp_str)

                # Track nickname -> species mapping
                nickname_to_species[f"{slot}: {nickname}"] = species
                slot_species[slot] = species

                side = player_side(slot)
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
                m = re.match(
                    r"(.+?)'s rating: (\d+) &rarr; <strong>(\d+)</strong>",
                    raw_text,
                )
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
                m = re.search(r"\[from\] ability: (.+?)(?:\||\[|$)", line)
                if m and len(parts) >= 3:
                    # Try to find which pokemon this is about via [of]
                    of_match = re.search(r"\[of\] (p[12][ab]): (.+?)(?:\||$)", line)
                    if of_match:
                        slot = of_match.group(1)
                        side = player_side(slot)
                        species = resolve_species(slot, of_match.group(2).strip())
                        get_revealed(side, species).ability = m.group(1).strip()

            # ── Item from [from] item: tags ──
            if "[from] item:" in line:
                m = re.search(r"\[from\] item: (.+?)(?:\||\[|$)", line)
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


# ── Batch processing ─────────────────────────────────────────────────────────


def parse_all(
    format_id: str,
    min_rating: int = 0,
    output_path: str | None = None,
    single_file: str | None = None,
) -> None:
    if single_file:
        # Parse a single file for debugging
        with open(single_file, encoding="utf-8") as f:
            data = json.load(f)
        result = parse_replay(data)
        if result:
            sys.stdout.reconfigure(encoding="utf-8")
            print(json.dumps(result, indent=2, ensure_ascii=False))
        else:
            print("Failed to parse replay", file=sys.stderr)
        return

    replay_dir = Path(__file__).parent / "data" / format_id / "replays"
    if not replay_dir.exists():
        print(f"Replay directory not found: {replay_dir}")
        return

    if output_path is None:
        output_path = str(Path(__file__).parent / "data" / format_id / "parsed.jsonl")

    files = list(replay_dir.glob("*.json"))
    print(f"Parsing {len(files)} replays from {replay_dir}")
    print(f"Output: {output_path}")
    if min_rating > 0:
        print(f"Min rating filter: {min_rating}")

    parsed = 0
    skipped_rating = 0
    skipped_error = 0

    with open(output_path, "w", encoding="utf-8") as out:
        for i, filepath in enumerate(files):
            try:
                with open(filepath, encoding="utf-8") as f:
                    data = json.load(f)

                # Rating filter (on the search metadata rating)
                if min_rating > 0:
                    rating = data.get("rating")
                    if rating is None or rating < min_rating:
                        skipped_rating += 1
                        continue

                result = parse_replay(data)
                if result is None:
                    skipped_error += 1
                    continue

                out.write(json.dumps(result, ensure_ascii=False) + "\n")
                parsed += 1

            except (json.JSONDecodeError, OSError) as e:
                skipped_error += 1
                continue

            if (i + 1) % 1000 == 0:
                print(f"  {i + 1}/{len(files)} processed ({parsed} parsed)")

    print(f"\nDone:")
    print(f"  Parsed: {parsed}")
    print(f"  Skipped (rating): {skipped_rating}")
    print(f"  Skipped (error): {skipped_error}")
    print(f"  Output: {output_path}")


def main():
    parser = argparse.ArgumentParser(description="Parse Pokemon Showdown replays")
    parser.add_argument(
        "--format",
        default="gen9vgc2025regi",
        help="Showdown format ID (default: gen9vgc2025regi)",
    )
    parser.add_argument(
        "--min-rating",
        type=int,
        default=0,
        help="Minimum rating to include (default: 0 = all)",
    )
    parser.add_argument(
        "--output",
        default=None,
        help="Output JSONL file path (default: data/<format>/parsed.jsonl)",
    )
    parser.add_argument(
        "--single",
        default=None,
        help="Parse a single replay file and print to stdout (for debugging)",
    )
    args = parser.parse_args()
    parse_all(args.format, args.min_rating, args.output, args.single)


if __name__ == "__main__":
    main()
