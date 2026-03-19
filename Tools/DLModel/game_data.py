"""
Static game data for computing domain-knowledge features.

Provides type effectiveness chart, species data (types, base stats),
and move data (base power, type, category) for use in dataset encoding
and heuristic evaluation.
"""

import json
from pathlib import Path
from functools import lru_cache

_DATA_DIR = Path(__file__).parent / 'game_data'

# ── Type chart ────────────────────────────────────────────────────────

# Standard Gen 9 type ordering
TYPES = [
    'Normal', 'Fire', 'Water', 'Electric', 'Grass', 'Ice',
    'Fighting', 'Poison', 'Ground', 'Flying', 'Psychic', 'Bug',
    'Rock', 'Ghost', 'Dragon', 'Dark', 'Steel', 'Fairy',
]
TYPE_INDEX = {t: i for i, t in enumerate(TYPES)}

# 18×18 effectiveness matrix: _CHART[atk_type_idx][def_type_idx] = multiplier
# Row = attacking type, Column = defending type
# 0 = immune, 0.5 = not very effective, 1 = neutral, 2 = super effective
_CHART = [
    # Nor  Fir  Wat  Ele  Gra  Ice  Fig  Poi  Gro  Fly  Psy  Bug  Roc  Gho  Dra  Dar  Ste  Fai
    [1,    1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   0.5, 0,   1,   1,   0.5, 1   ],  # Normal
    [1,    0.5, 0.5, 1,   2,   2,   1,   1,   1,   1,   1,   2,   0.5, 1,   0.5, 1,   2,   1   ],  # Fire
    [1,    2,   0.5, 1,   0.5, 1,   1,   1,   2,   1,   1,   1,   2,   1,   0.5, 1,   1,   1   ],  # Water
    [1,    1,   2,   0.5, 0.5, 1,   1,   1,   0,   2,   1,   1,   1,   1,   0.5, 1,   1,   1   ],  # Electric
    [1,    0.5, 2,   1,   0.5, 1,   1,   0.5, 2,   0.5, 1,   0.5, 2,   1,   0.5, 1,   0.5, 1   ],  # Grass
    [1,    0.5, 0.5, 1,   2,   0.5, 1,   1,   2,   2,   1,   1,   1,   1,   2,   1,   0.5, 1   ],  # Ice
    [2,    1,   1,   1,   1,   2,   1,   0.5, 1,   0.5, 0.5, 0.5, 2,   0,   1,   2,   2,   0.5 ],  # Fighting
    [1,    1,   1,   1,   2,   1,   1,   0.5, 0.5, 1,   1,   1,   0.5, 0.5, 1,   1,   0,   2   ],  # Poison
    [1,    2,   1,   2,   0.5, 1,   1,   2,   1,   0,   1,   0.5, 2,   1,   1,   1,   2,   1   ],  # Ground
    [1,    1,   1,   0.5, 2,   1,   2,   1,   1,   1,   1,   2,   0.5, 1,   1,   1,   0.5, 1   ],  # Flying
    [1,    1,   1,   1,   1,   1,   2,   2,   1,   1,   0.5, 1,   1,   1,   1,   0,   0.5, 1   ],  # Psychic
    [1,    0.5, 1,   1,   2,   1,   0.5, 0.5, 1,   0.5, 2,   1,   1,   0.5, 1,   2,   0.5, 0.5 ],  # Bug
    [1,    2,   1,   1,   1,   2,   0.5, 1,   0.5, 2,   1,   2,   1,   1,   1,   1,   0.5, 1   ],  # Rock
    [0,    1,   1,   1,   1,   1,   1,   1,   1,   1,   2,   1,   1,   2,   1,   0.5, 1,   1   ],  # Ghost
    [1,    1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   2,   1,   0.5, 0   ],  # Dragon
    [1,    1,   1,   1,   1,   1,   0.5, 1,   1,   1,   2,   1,   1,   2,   1,   0.5, 0.5, 0.5 ],  # Dark
    [1,    0.5, 0.5, 0.5, 1,   2,   1,   1,   1,   1,   1,   1,   2,   1,   1,   1,   0.5, 2   ],  # Steel
    [1,    0.5, 1,   1,   1,   1,   2,   0.5, 1,   1,   1,   1,   1,   1,   2,   2,   0.5, 1   ],  # Fairy
]


def type_effectiveness(atk_type: str, def_types: list[str]) -> float:
    """Compute type effectiveness multiplier for a move type vs defender types."""
    atk_idx = TYPE_INDEX.get(atk_type)
    if atk_idx is None:
        return 1.0

    mult = 1.0
    for dt in def_types:
        def_idx = TYPE_INDEX.get(dt)
        if def_idx is not None:
            mult *= _CHART[atk_idx][def_idx]
    return mult


# ── Species data ──────────────────────────────────────────────────────

@lru_cache(maxsize=1)
def _load_species_data() -> dict:
    with open(_DATA_DIR / 'species_data.json') as f:
        return json.load(f)


def get_species_types(species: str) -> list[str]:
    """Get type list for a species. Returns ['Normal'] if unknown."""
    data = _load_species_data()
    entry = data.get(species)
    if entry:
        return entry['types']
    return ['Normal']


def get_species_base_stats(species: str) -> dict:
    """Get base stats dict {hp, atk, def, spa, spd, spe}. Returns 80 for all if unknown."""
    data = _load_species_data()
    entry = data.get(species)
    if entry:
        return entry['baseStats']
    return {'hp': 80, 'atk': 80, 'def': 80, 'spa': 80, 'spd': 80, 'spe': 80}


def get_species_base_speed(species: str) -> int:
    """Get base speed stat for a species."""
    return get_species_base_stats(species)['spe']


# ── Move data ─────────────────────────────────────────────────────────

@lru_cache(maxsize=1)
def _load_move_data() -> dict:
    with open(_DATA_DIR / 'move_data.json') as f:
        return json.load(f)


def get_move_info(move_name: str) -> dict | None:
    """Get move info dict {basePower, type, category, isSpread}. None if unknown."""
    data = _load_move_data()
    return data.get(move_name)


# ── Damage estimation ─────────────────────────────────────────────────

def estimate_damage_fraction(
    atk_species: str,
    atk_moves: list[str],
    def_species: str,
    def_types: list[str] | None = None,
) -> float:
    """
    Estimate the best damage fraction (damage / target_hp) for the attacker's
    known moves against the defender. Uses base stats, type effectiveness, STAB,
    and spread penalty. Returns 0 if no damaging moves or unknown.

    Assumes level 50, neutral nature, 0 EVs for a conservative estimate.
    """
    atk_stats = get_species_base_stats(atk_species)
    def_stats = get_species_base_stats(def_species)
    atk_types = get_species_types(atk_species)
    if def_types is None:
        def_types = get_species_types(def_species)

    # Level 50 HP formula: floor((2*base + IV) * 50/100) + 50 + 10
    # With 0 EVs, 15 IVs (average): floor((2*base + 15) * 50/100) + 60
    target_hp = (2 * def_stats['hp'] + 15) * 50 // 100 + 60

    best_frac = 0.0
    move_data = _load_move_data()

    for move_name in atk_moves:
        info = move_data.get(move_name)
        if info is None or info['category'] == 'Status' or info['basePower'] <= 0:
            continue

        bp = info['basePower']
        move_type = info['type']

        # Type effectiveness
        eff = type_effectiveness(move_type, def_types)
        if eff == 0:
            continue

        # STAB
        stab = 1.5 if move_type in atk_types else 1.0

        # Spread penalty
        spread = 0.75 if info['isSpread'] else 1.0

        # Pick correct attack/defense stats based on category
        if info['category'] == 'Physical':
            atk_stat = (2 * atk_stats['atk'] + 15) * 50 // 100 + 5
            def_stat = (2 * def_stats['def'] + 15) * 50 // 100 + 5
        else:  # Special
            atk_stat = (2 * atk_stats['spa'] + 15) * 50 // 100 + 5
            def_stat = (2 * def_stats['spd'] + 15) * 50 // 100 + 5

        # Simplified damage formula (level 50, no items/abilities/weather)
        damage = ((2 * 50 / 5 + 2) * bp * atk_stat / def_stat / 50 + 2)
        damage *= eff * stab * spread

        frac = damage / target_hp
        if frac > best_frac:
            best_frac = frac

    return best_frac
