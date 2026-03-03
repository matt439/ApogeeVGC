"""
Dataset statistics for scraped Pokemon Showdown replays.

Reports per-format stats from raw replay files and parsed JSONL:
  - Replay count
  - Elo mean and standard deviation (from raw replay metadata)
  - Parsed game count (if parsed.jsonl exists)
  - Player elo mean and std dev (from parsed player ratings)

Usage:
  python stats.py                          # stats for all formats (sample)
  python stats.py --format gen9vgc2025regi # specific format
  python stats.py --sample 500             # custom sample size for raw replays
  python stats.py --all                    # read every raw replay (slow)
"""

from __future__ import annotations

import argparse
import itertools
import json
import math
from pathlib import Path


def mean_std(values: list[int | float]) -> tuple[float, float]:
    """Compute mean and population standard deviation."""
    if not values:
        return 0.0, 0.0
    n = len(values)
    mu = sum(values) / n
    if n < 2:
        return mu, 0.0
    variance = sum((x - mu) ** 2 for x in values) / (n - 1)
    return mu, math.sqrt(variance)


def collect_raw_stats(replay_dir: Path, sample: int | None = None) -> dict:
    """Collect stats from raw replay JSON files.

    If sample is set, only read that many files (for speed).
    Total replay count is always derived from directory listing.
    """
    all_files = list(replay_dir.glob("*.json"))
    total = len(all_files)
    files = all_files if sample is None else all_files[:sample]

    ratings = []
    read = 0

    for filepath in files:
        try:
            with open(filepath, encoding="utf-8") as f:
                data = json.load(f)
            read += 1
            rating = data.get("rating")
            if rating is not None:
                ratings.append(rating)
        except (json.JSONDecodeError, OSError):
            continue

    elo_mean, elo_std = mean_std(ratings)
    return {
        "total": total,
        "sampled": read,
        "rated": len(ratings),
        "elo_mean": elo_mean,
        "elo_std": elo_std,
    }


def collect_parsed_stats(parsed_path: Path, sample: int | None = None) -> dict | None:
    """Collect stats from parsed JSONL file."""
    if not parsed_path.exists():
        return None

    count = 0
    player_ratings = []
    turn_counts = []
    winners = {"p1": 0, "p2": 0, "none": 0}

    with open(parsed_path, encoding="utf-8") as f:
        lines = f if sample is None else itertools.islice(f, sample)
        for line in lines:
            line = line.strip()
            if not line:
                continue
            try:
                game = json.loads(line)
            except json.JSONDecodeError:
                continue

            count += 1
            turn_counts.append(game.get("turn_count", 0))

            w = game.get("winner")
            winners[w if w in ("p1", "p2") else "none"] += 1

            players = game.get("players", {})
            for pid in ("p1", "p2"):
                pdata = players.get(pid, {})
                r = pdata.get("rating_before")
                if r is not None:
                    player_ratings.append(r)

    elo_mean, elo_std = mean_std(player_ratings)
    turn_mean, turn_std = mean_std(turn_counts)

    return {
        "games": count,
        "rated_players": len(player_ratings),
        "elo_mean": elo_mean,
        "elo_std": elo_std,
        "turn_mean": turn_mean,
        "turn_std": turn_std,
        "winners": winners,
    }


def format_stats(format_id: str, data_dir: Path, sample: int | None) -> None:
    """Print stats for a single format."""
    format_dir = data_dir / format_id
    replay_dir = format_dir / "replays"
    parsed_path = format_dir / "parsed.jsonl"

    print(f"=== {format_id} ===")

    if replay_dir.exists():
        raw = collect_raw_stats(replay_dir, sample)
        print(f"  Raw replays:   {raw['total']:,}")
        if sample is not None and raw['total'] > raw['sampled']:
            print(f"  Sampled:       {raw['sampled']:,}")
        print(f"  With rating:   {raw['rated']:,}")
        if raw["rated"] > 0:
            print(f"  Elo mean:      {raw['elo_mean']:.1f}")
            print(f"  Elo std dev:   {raw['elo_std']:.1f}")
    else:
        print("  No replay directory found")

    parsed = collect_parsed_stats(parsed_path, sample)
    if parsed is not None:
        print(f"  Parsed games:  {parsed['games']:,}")
        if parsed["rated_players"] > 0:
            print(f"  Player elo mean: {parsed['elo_mean']:.1f}")
            print(f"  Player elo std:  {parsed['elo_std']:.1f}")
        print(f"  Avg turns:     {parsed['turn_mean']:.1f} (std {parsed['turn_std']:.1f})")
        w = parsed["winners"]
        total = w["p1"] + w["p2"] + w["none"]
        if total > 0:
            print(f"  Win rate p1:   {w['p1']/total*100:.1f}%  p2: {w['p2']/total*100:.1f}%  draw/forfeit: {w['none']/total*100:.1f}%")
    else:
        print("  No parsed.jsonl found")

    print()


def main():
    parser = argparse.ArgumentParser(description="Dataset statistics for scraped replays")
    parser.add_argument(
        "--format",
        default=None,
        help="Specific format to report (default: all formats)",
    )
    parser.add_argument(
        "--sample",
        type=int,
        default=200,
        help="Max replays/lines to read per format (default: 200, fast)",
    )
    parser.add_argument(
        "--all",
        action="store_true",
        help="Read every file (slow for large datasets)",
    )
    args = parser.parse_args()

    sample = None if args.all else args.sample

    data_dir = Path(__file__).parent / "data"
    if not data_dir.exists():
        print(f"Data directory not found: {data_dir}")
        return

    if sample is not None:
        print(f"(sampling up to {sample} per format — use --all for full stats)\n")

    if args.format:
        format_stats(args.format, data_dir, sample)
    else:
        formats = sorted(
            d.name for d in data_dir.iterdir()
            if d.is_dir() and not d.name.startswith(".")
        )
        for fmt in formats:
            format_stats(fmt, data_dir, sample)


if __name__ == "__main__":
    main()
