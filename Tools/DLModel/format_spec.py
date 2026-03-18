"""
Format specifications for different Pokemon battle formats.

Defines the structural parameters that vary across formats:
- team_size: how many pokemon are brought to battle
- num_leads: how many are active at once per side
- total_pokemon: team sheet size (always 6)

Everything else (config enumeration, slot counts, numeric dimensions)
is derived from these three values. Both team preview and battle models
use FormatSpec to parameterize their architecture.

The config/slot layout is deterministic (lexicographic combinations),
so Python and C# produce identical enumerations from the same spec.
"""

from __future__ import annotations

from dataclasses import dataclass
from functools import cached_property
from itertools import combinations


# Per-slot numeric feature dimensions (shared across all formats)
ACTIVE_DIM = 46  # 35 base + 11 volatile flags
BENCH_DIM = 10
FIELD_DIM = 20


@dataclass(frozen=True)
class FormatSpec:
    """Structural specification for a Pokemon battle format."""

    name: str
    team_size: int        # pokemon brought to battle (4 for VGC, 6 for OU/singles)
    num_leads: int        # active pokemon per side (2 for doubles, 1 for singles)
    total_pokemon: int = 6  # team sheet size

    # ── Derived properties: Team Preview ──

    @cached_property
    def num_preview_slots(self) -> int:
        """Input slots for team preview model (always full team × 2)."""
        return self.total_pokemon * 2

    @cached_property
    def configs(self) -> list[tuple[tuple[int, ...], tuple[int, ...], tuple[int, ...]]]:
        """All valid team preview configurations as (bring, lead, bench) tuples.

        Enumerated in lexicographic order (matches itertools.combinations).
        """
        result = []
        for bring in combinations(range(self.total_pokemon), self.team_size):
            for lead in combinations(bring, self.num_leads):
                bench = tuple(b for b in bring if b not in lead)
                result.append((bring, lead, bench))
        return result

    @cached_property
    def num_configs(self) -> int:
        """Number of team preview configurations."""
        return len(self.configs)

    @cached_property
    def config_index(self) -> dict[tuple[frozenset[int], frozenset[int]], int]:
        """Map (bring_set, lead_set) -> config index."""
        return {
            (frozenset(bring), frozenset(lead)): i
            for i, (bring, lead, _) in enumerate(self.configs)
        }

    def config_to_index(self, bring_set: set[int], lead_set: set[int]) -> int | None:
        """Map a (bring, lead) pair of index sets to the config index."""
        return self.config_index.get((frozenset(bring_set), frozenset(lead_set)))

    def index_to_config(self, idx: int) -> tuple[tuple[int, ...], tuple[int, ...], tuple[int, ...]]:
        """Return (bring, lead, bench) tuples for a given config index."""
        return self.configs[idx]

    # ── Derived properties: Battle ──

    @cached_property
    def num_battle_slots(self) -> int:
        """Total pokemon slots in battle state encoding (team_size × 2)."""
        return self.team_size * 2

    @cached_property
    def num_bench_per_side(self) -> int:
        """Bench pokemon per side."""
        return self.team_size - self.num_leads

    @cached_property
    def numeric_dim(self) -> int:
        """Total numeric feature vector dimension for battle state."""
        num_active = self.num_leads * 2
        num_bench = self.num_bench_per_side * 2
        return num_active * ACTIVE_DIM + num_bench * BENCH_DIM + FIELD_DIM

    def to_dict(self) -> dict:
        """Serialize for storing in checkpoints and vocab JSON."""
        return {
            'name': self.name,
            'team_size': self.team_size,
            'num_leads': self.num_leads,
            'total_pokemon': self.total_pokemon,
        }

    @classmethod
    def from_dict(cls, d: dict) -> FormatSpec:
        """Deserialize from checkpoint or vocab JSON."""
        return cls(
            name=d['name'],
            team_size=d['team_size'],
            num_leads=d['num_leads'],
            total_pokemon=d.get('total_pokemon', 6),
        )


# ── Pre-defined formats ──

VGC = FormatSpec('vgc', team_size=4, num_leads=2)
DOUBLES_OU = FormatSpec('doubles_ou', team_size=6, num_leads=2)
SINGLES = FormatSpec('singles', team_size=6, num_leads=1)

FORMAT_REGISTRY: dict[str, FormatSpec] = {
    'vgc': VGC,
    'doubles_ou': DOUBLES_OU,
    'singles': SINGLES,
}


def get_format(name: str) -> FormatSpec:
    """Look up a format by name. Raises KeyError if not found."""
    return FORMAT_REGISTRY[name]
