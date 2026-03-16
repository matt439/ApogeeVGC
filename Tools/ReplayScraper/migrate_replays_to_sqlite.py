"""
One-time migration: ingests existing flat replay JSON files into SQLite databases.

Usage:
  python migrate_replays_to_sqlite.py                        # migrate all formats
  python migrate_replays_to_sqlite.py --format gen9vgc2025regi

Creates data/<format_id>.db for each format.
After verifying the .db files work correctly, you can delete the replays/ subdirectories.
"""

import argparse
import json
import time
from pathlib import Path

from replay_db import open_db


def migrate(format_id: str) -> None:
    data_dir = Path(__file__).parent / "data" / format_id
    replay_dir = data_dir / "replays"

    if not replay_dir.exists():
        print(f"  No replays directory found at {replay_dir}, skipping")
        return

    conn = open_db(format_id)

    # Check how many are already in the DB
    existing = conn.execute("SELECT COUNT(*) FROM replays").fetchone()[0]
    print(f"  Existing in DB: {existing}")

    # Collect all JSON files
    files = list(replay_dir.glob("*.json"))
    total = len(files)
    print(f"  Found {total} replay files in {replay_dir}")

    if total == 0:
        conn.close()
        return

    # Batch insert
    BATCH_SIZE = 1000
    migrated = 0
    skipped = 0
    t_start = time.time()

    batch = []
    for i, f in enumerate(files):
        # Derive replay ID from filename (reverse of replay_filename())
        replay_id = f.stem.replace("_", "/")

        try:
            json_str = f.read_text(encoding="utf-8")
            # Validate it's valid JSON
            json.loads(json_str)
        except Exception as e:
            print(f"  Error reading {f.name}: {e}")
            skipped += 1
            continue

        batch.append((replay_id, json_str))

        if len(batch) >= BATCH_SIZE:
            conn.executemany(
                "INSERT OR IGNORE INTO replays (id, json) VALUES (?, ?)",
                batch,
            )
            conn.commit()
            migrated += len(batch)
            batch = []

            if migrated % 10000 == 0:
                elapsed = time.time() - t_start
                rate = migrated / elapsed if elapsed > 0 else 0
                remaining = total - migrated - skipped
                eta = remaining / rate if rate > 0 else 0
                print(
                    f"  {migrated}/{total} migrated "
                    f"({rate:.0f}/s, ETA {eta:.0f}s)"
                )

    # Flush remaining
    if batch:
        conn.executemany(
            "INSERT OR IGNORE INTO replays (id, json) VALUES (?, ?)",
            batch,
        )
        conn.commit()
        migrated += len(batch)

    conn.close()

    elapsed = time.time() - t_start
    print(f"  Done: {migrated} migrated, {skipped} skipped in {elapsed:.1f}s")


def main():
    parser = argparse.ArgumentParser(
        description="Migrate replay flat files to SQLite"
    )
    parser.add_argument(
        "--format",
        nargs="+",
        default=None,
        help="Format ID(s) to migrate (default: auto-detect all)",
    )
    args = parser.parse_args()

    data_dir = Path(__file__).parent / "data"

    if args.format:
        formats = args.format
    else:
        # Auto-detect: find all subdirectories with a replays/ folder
        formats = [
            d.name
            for d in sorted(data_dir.iterdir())
            if d.is_dir() and (d / "replays").exists()
        ]

    if not formats:
        print("No formats found to migrate.")
        return

    print(f"Formats to migrate: {', '.join(formats)}\n")

    for fmt in formats:
        print(f"=== {fmt} ===")
        migrate(fmt)
        print()

    print("Migration complete.")
    print("After verifying the .db files work, delete the flat-file directories:")
    for fmt in formats:
        print(f"  rm -rf data/{fmt}/replays/")


if __name__ == "__main__":
    main()
