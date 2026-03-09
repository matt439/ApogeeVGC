"""
Incremental Battle State Tracker for Live Assist

Adapts parser.py's line-processing logic for streaming use with Pokemon Showdown's
WebSocket protocol. Instead of parsing a complete replay, processes lines as they
arrive and maintains mutable state.

Also handles |request| JSON from Showdown, which contains full own-team data
(species, moves, ability, item, HP, status) and legal actions.
"""

from __future__ import annotations

import re
import sys
from dataclasses import dataclass, field
from typing import Any

# Import shared dataclasses and helpers from parser.py
sys.path.insert(0, str(__import__("pathlib").Path(__file__).resolve().parent.parent / "ReplayScraper"))
from parser import (
    ActivePokemon,
    FieldState,
    RevealedInfo,
    parse_hp,
    parse_slot,
    parse_species_detail,
    parse_status_from_hp,
    player_side,
)


# ── Pre-compiled regexes (from parser.py) ────────────────────────────────────

_RE_FROM_ABILITY = re.compile(r"\[from\] ability: (.+?)(?:\||\[|$)")
_RE_OF_SLOT = re.compile(r"\[of\] (p[12][ab]): (.+?)(?:\||$)")
_RE_FROM_ITEM = re.compile(r"\[from\] item: (.+?)(?:\||\[|$)")


# ── Own-team Pokemon from |request| ─────────────────────────────────────────

@dataclass
class OwnPokemon:
    """Full info about an own-team pokemon from |request| side.pokemon[]."""
    species: str
    moves: list[str]
    ability: str
    item: str
    tera_type: str = ""
    hp: int = 100
    max_hp: int = 100
    status: str | None = None
    fainted: bool = False
    active: bool = False
    # Stats from |request| (not used for encoding, but useful for display)
    level: int = 50


# ── Live Battle State ────────────────────────────────────────────────────────

class LiveBattleState:
    """Incremental battle state tracker for live Showdown battles.

    Processes protocol lines the same way parser.py does, but mutably
    updates internal state instead of building a replay structure.
    """

    def __init__(self):
        # Identity
        self.my_side: str | None = None  # "p1" or "p2" (from |request|)
        self.opp_side: str | None = None

        # Team preview
        self.team_preview: dict[str, list[dict]] = {"p1": [], "p2": []}

        # Active pokemon (slot -> ActivePokemon)
        self.active: dict[str, ActivePokemon] = {}

        # Bench tracks pokemon not on field: side -> species -> {hp, max_hp, status, fainted}
        self.bench: dict[str, dict[str, dict]] = {"p1": {}, "p2": {}}

        # Field state
        self.field = FieldState()

        # Name resolution
        self.nickname_to_species: dict[str, str] = {}  # "p1a: Nickname" -> species
        self.slot_species: dict[str, str] = {}  # slot -> current species

        # Revealed info (progressive)
        self.revealed: dict[str, dict[str, RevealedInfo]] = {"p1": {}, "p2": {}}

        # Team brought (species seen on field)
        self.team_brought: dict[str, list[str]] = {"p1": [], "p2": []}

        # Own-team data from |request|
        self.own_team: list[OwnPokemon] = []

        # Turn tracking
        self.current_turn: int = 0
        self.phase: str = "init"  # init / teampreview / battle / ended

        # Last request data (raw dict)
        self.last_request: dict | None = None

        # Tera tracking for current turn
        self._turn_tera: dict[str, str] = {}

    def get_revealed(self, side: str, species: str) -> RevealedInfo:
        if species not in self.revealed[side]:
            self.revealed[side][species] = RevealedInfo()
        return self.revealed[side][species]

    def resolve_species(self, slot: str, nickname: str) -> str:
        key = f"{slot}: {nickname}"
        if slot in self.slot_species:
            return self.slot_species[slot]
        return self.nickname_to_species.get(key, nickname)

    def update(self, lines: list[str]) -> None:
        """Process protocol lines — same logic as parser.py lines 329-619."""
        for line in lines:
            if not line.startswith("|"):
                continue

            parts = line.split("|")
            if len(parts) < 2:
                continue
            cmd = parts[1]

            try:
                self._process_line(cmd, parts, line)
            except (IndexError, ValueError):
                continue  # Skip malformed lines

    def _process_line(self, cmd: str, parts: list[str], line: str) -> None:
        """Process a single protocol line."""

        # ── Player ──
        if cmd == "player" and len(parts) >= 4:
            pass  # We identify our side from |request|

        elif cmd == "gametype":
            pass  # Always doubles for VGC

        # ── Team preview ──
        elif cmd == "poke" and len(parts) >= 4:
            side = parts[2].strip()
            detail = parts[3].strip()
            species, level, gender = parse_species_detail(detail)
            self.team_preview[side].append({
                "species": species,
                "level": level,
                "gender": gender,
            })
            self.phase = "teampreview"

        # ── Turn marker ──
        elif cmd == "turn" and len(parts) >= 3:
            self.current_turn = int(parts[2].strip())
            self._turn_tera = {}
            self.phase = "battle"

        # ── Switch / Drag ──
        elif cmd in ("switch", "drag") and len(parts) >= 5:
            slot, nickname = parse_slot(parts[2].strip())
            detail = parts[3].strip()
            hp_str = parts[4].strip()
            species, level, gender = parse_species_detail(detail)
            hp, max_hp = parse_hp(hp_str)
            status = parse_status_from_hp(hp_str)

            side = player_side(slot)

            # Save outgoing pokemon to bench
            if slot in self.active and self.active[slot] is not None:
                old = self.active[slot]
                self.bench[side][old.species] = {
                    "hp": old.hp,
                    "max_hp": old.max_hp,
                    "status": old.status,
                    "fainted": old.fainted,
                }

            # Remove incoming pokemon from bench
            if species in self.bench[side]:
                del self.bench[side][species]

            # Track nickname -> species
            self.nickname_to_species[f"{slot}: {nickname}"] = species
            self.slot_species[slot] = species

            if species not in self.team_brought.get(side, []):
                self.team_brought[side].append(species)

            self.active[slot] = ActivePokemon(
                species=species,
                slot=slot,
                nickname=nickname,
                hp=hp,
                max_hp=max_hp,
                status=status,
            )

            if self.phase == "init":
                self.phase = "battle"

        # ── Move ──
        elif cmd == "move" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            move_name = parts[3].strip()

            side = player_side(slot)
            species = self.resolve_species(slot, nickname)
            ri = self.get_revealed(side, species)
            if move_name not in ri.moves:
                ri.moves.append(move_name)

        # ── Can't move ──
        elif cmd == "cant" and len(parts) >= 4:
            pass  # No state change needed

        # ── Damage ──
        elif cmd == "-damage" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            hp_str = parts[3].strip()
            hp, max_hp = parse_hp(hp_str)
            status = parse_status_from_hp(hp_str)
            if slot in self.active:
                self.active[slot].hp = hp
                self.active[slot].max_hp = max_hp
                if status:
                    self.active[slot].status = status
                if hp == 0:
                    self.active[slot].fainted = True

        # ── Heal ──
        elif cmd == "-heal" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            hp_str = parts[3].strip()
            hp, max_hp = parse_hp(hp_str)
            if slot in self.active:
                self.active[slot].hp = hp
                self.active[slot].max_hp = max_hp
                status = parse_status_from_hp(hp_str)
                if status:
                    self.active[slot].status = status

        # ── Faint ──
        elif cmd == "faint" and len(parts) >= 3:
            slot, nickname = parse_slot(parts[2].strip())
            if slot in self.active:
                self.active[slot].hp = 0
                self.active[slot].fainted = True
                side = player_side(slot)
                self.bench[side][self.active[slot].species] = {
                    "hp": 0,
                    "max_hp": self.active[slot].max_hp,
                    "status": self.active[slot].status,
                    "fainted": True,
                }

        # ── Boost / Unboost ──
        elif cmd == "-boost" and len(parts) >= 5:
            slot, nickname = parse_slot(parts[2].strip())
            stat = parts[3].strip()
            amount = int(parts[4].strip())
            if slot in self.active:
                self.active[slot].boosts[stat] = self.active[slot].boosts.get(stat, 0) + amount

        elif cmd == "-unboost" and len(parts) >= 5:
            slot, nickname = parse_slot(parts[2].strip())
            stat = parts[3].strip()
            amount = int(parts[4].strip())
            if slot in self.active:
                self.active[slot].boosts[stat] = self.active[slot].boosts.get(stat, 0) - amount

        # ── Status ──
        elif cmd == "-status" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            status = parts[3].strip()
            if slot in self.active:
                self.active[slot].status = status

        elif cmd == "-curestatus" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            if slot in self.active:
                self.active[slot].status = None

        # ── Terastallize ──
        elif cmd == "-terastallize" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            tera_type = parts[3].strip()
            if slot in self.active:
                self.active[slot].tera_type = tera_type
            self._turn_tera[slot] = tera_type
            side = player_side(slot)
            species = self.resolve_species(slot, nickname)
            self.get_revealed(side, species).tera_type = tera_type

        # ── Weather ──
        elif cmd == "-weather" and len(parts) >= 3:
            weather = parts[2].strip()
            if weather == "none":
                self.field.weather = None
            elif "[upkeep]" not in line:
                self.field.weather = weather

        # ── Field start/end ──
        elif cmd == "-fieldstart" and len(parts) >= 3:
            effect = parts[2].strip()
            effect_lower = effect.lower()
            if "terrain" in effect_lower:
                self.field.terrain = effect.replace("move: ", "")
            elif "trick room" in effect_lower:
                self.field.trick_room = True

        elif cmd == "-fieldend" and len(parts) >= 3:
            effect = parts[2].strip()
            effect_lower = effect.lower()
            if "terrain" in effect_lower:
                self.field.terrain = None
            elif "trick room" in effect_lower:
                self.field.trick_room = False

        # ── Side conditions ──
        elif cmd == "-sidestart" and len(parts) >= 4:
            side_str = parts[2].strip()
            side = "p1" if "p1" in side_str else "p2"
            effect = parts[3].strip().lower()
            if "tailwind" in effect:
                setattr(self.field, f"{side}_tailwind", True)
            elif "reflect" in effect:
                setattr(self.field, f"{side}_reflect", True)
            elif "light screen" in effect:
                setattr(self.field, f"{side}_light_screen", True)
            elif "aurora veil" in effect:
                setattr(self.field, f"{side}_aurora_veil", True)

        elif cmd == "-sideend" and len(parts) >= 4:
            side_str = parts[2].strip()
            side = "p1" if "p1" in side_str else "p2"
            effect = parts[3].strip().lower()
            if "tailwind" in effect:
                setattr(self.field, f"{side}_tailwind", False)
            elif "reflect" in effect:
                setattr(self.field, f"{side}_reflect", False)
            elif "light screen" in effect:
                setattr(self.field, f"{side}_light_screen", False)
            elif "aurora veil" in effect:
                setattr(self.field, f"{side}_aurora_veil", False)

        # ── Item revealed ──
        elif cmd in ("-enditem", "-item") and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            item = parts[3].strip()
            side = player_side(slot)
            species = self.resolve_species(slot, nickname)
            self.get_revealed(side, species).item = item

        # ── Ability revealed ──
        elif cmd == "-ability" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            ability = parts[3].strip()
            side = player_side(slot)
            species = self.resolve_species(slot, nickname)
            self.get_revealed(side, species).ability = ability

        # ── Swap (Ally Switch) ──
        elif cmd == "swap" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            side = player_side(slot)
            slot_a = f"{side}a"
            slot_b = f"{side}b"
            if slot_a in self.active and slot_b in self.active:
                self.active[slot_a], self.active[slot_b] = self.active[slot_b], self.active[slot_a]
                self.active[slot_a].slot = slot_a
                self.active[slot_b].slot = slot_b
                self.slot_species[slot_a], self.slot_species[slot_b] = (
                    self.slot_species.get(slot_b, ""),
                    self.slot_species.get(slot_a, ""),
                )

        # ── Details change (form change) ──
        elif cmd == "detailschange" and len(parts) >= 4:
            slot, nickname = parse_slot(parts[2].strip())
            detail = parts[3].strip()
            new_species, level, gender = parse_species_detail(detail)
            if slot in self.active:
                self.active[slot].species = new_species
            self.slot_species[slot] = new_species

        # ── Win ──
        elif cmd == "win" and len(parts) >= 3:
            self.phase = "ended"

        # ── [from] ability / item tags ──
        if "[from] ability:" in line:
            m = _RE_FROM_ABILITY.search(line)
            if m and len(parts) >= 3:
                of_match = _RE_OF_SLOT.search(line)
                if of_match:
                    slot = of_match.group(1)
                    side = player_side(slot)
                    species = self.resolve_species(slot, of_match.group(2).strip())
                    self.get_revealed(side, species).ability = m.group(1).strip()

        if "[from] item:" in line:
            m = _RE_FROM_ITEM.search(line)
            if m and len(parts) >= 3:
                try:
                    slot, nickname = parse_slot(parts[2].strip())
                    side = player_side(slot)
                    species = self.resolve_species(slot, nickname)
                    self.get_revealed(side, species).item = m.group(1).strip()
                except (IndexError, ValueError):
                    pass

    def update_request(self, request: dict) -> None:
        """Process |request| JSON from Showdown.

        The request contains:
        - side.name, side.id (our player id, e.g. "p1")
        - side.pokemon[] — full own-team data
        - active[].moves[] — legal moves with disabled flags
        - forceSwitch[] — whether we must switch (after faint)
        - requestType — "move", "switch", "teampreview", "wait"
        """
        self.last_request = request

        # Identify our side
        side_data = request.get("side", {})
        side_id = side_data.get("id")
        if side_id:
            self.my_side = side_id
            self.opp_side = "p2" if side_id == "p1" else "p1"

        # Parse own-team pokemon
        self.own_team = []
        for poke in side_data.get("pokemon", []):
            # ident: "p1: Nickname"
            # details: "Species, L50, M" or "Species, L50"
            details = poke.get("details", "")
            species, level, gender = parse_species_detail(details)

            # Condition: "267/267" or "0 fnt" or "267/267 par"
            condition = poke.get("condition", "100/100")
            hp, max_hp = parse_hp(condition)
            status = parse_status_from_hp(condition)
            fainted = "fnt" in condition

            moves = poke.get("moves", [])
            # Move names from request are lowercase-no-spaces (e.g. "fakeout")
            # We need display names — Showdown sends baseAbility/ability/item as IDs too
            # The overlay will send these as-is; encoding handles name normalization

            ability = poke.get("ability", "")
            item = poke.get("item", "")
            tera_type = poke.get("teraType", "")

            self.own_team.append(OwnPokemon(
                species=species,
                moves=moves,
                ability=ability,
                item=item,
                tera_type=tera_type,
                hp=hp,
                max_hp=max_hp,
                status=status,
                fainted=fainted,
                active=poke.get("active", False),
                level=level,
            ))

            # Also update revealed info for own side with full data
            if self.my_side:
                ri = self.get_revealed(self.my_side, species)
                # Use baseAbility for revealed (the original ability)
                base_ability = poke.get("baseAbility", ability)
                if base_ability:
                    ri.ability = base_ability
                if item:
                    ri.item = item
                if tera_type:
                    ri.tera_type = tera_type
                # Moves from request are IDs — store them for encoding
                for m in moves:
                    if m not in ri.moves:
                        ri.moves.append(m)

        # Update phase
        req_type = request.get("requestType") or request.get("teamPreview")
        if request.get("teamPreview"):
            self.phase = "teampreview"
        elif request.get("forceSwitch"):
            pass  # Stay in battle phase, just a forced switch
        elif request.get("wait"):
            pass  # Waiting for opponent
        elif request.get("active"):
            self.phase = "battle"

    def snapshot(self) -> dict:
        """Return current state in a format matching parser.py's turn data.

        This produces the same structure that dataset.py expects for encoding.
        """
        active_snapshot = {}
        for slot, poke in self.active.items():
            if poke is not None:
                active_snapshot[slot] = {
                    "species": poke.species,
                    "hp": poke.hp,
                    "status": poke.status,
                    "boosts": dict(poke.boosts) if poke.boosts else {},
                    "tera": poke.tera_type,
                    "fainted": poke.fainted,
                }

        bench_snapshot = {}
        for side in ("p1", "p2"):
            bench_list = []
            for species, info in self.bench[side].items():
                bench_list.append({
                    "species": species,
                    "hp": info["hp"],
                    "status": info.get("status"),
                    "fainted": info.get("fainted", False),
                })
            if bench_list:
                bench_snapshot[side] = bench_list

        return {
            "turn": self.current_turn,
            "active": active_snapshot,
            "field": self.field.to_dict(),
            "bench": bench_snapshot,
        }

    def get_own_active_species(self) -> set[str]:
        """Get species names of own pokemon currently on field."""
        if not self.my_side:
            return set()
        result = set()
        for slot in (f"{self.my_side}a", f"{self.my_side}b"):
            if slot in self.active and not self.active[slot].fainted:
                result.add(self.active[slot].species)
        return result

    def get_own_bench_species(self) -> list[str]:
        """Get species names of own pokemon on bench (not active, not fainted)."""
        if not self.my_side:
            return []
        active_sp = self.get_own_active_species()
        result = []
        for poke in self.own_team:
            if poke.species not in active_sp and not poke.fainted:
                result.append(poke.species)
        return result
