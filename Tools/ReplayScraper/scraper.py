"""
Pokemon Showdown Replay Scraper

Scrapes battle replays from replay.pokemonshowdown.com for a given format.
Two-phase approach:
  1. Collect all replay IDs by paginating through the search API
  2. Fetch full replay JSON (including battle log) for each ID

Supports resumption — skips already-fetched replays on restart.

Usage:
  python scraper.py                          # uses defaults
  python scraper.py --format gen9vgc2025regi
  python scraper.py --format gen9vgc2024regh --concurrency 20
  python scraper.py --phase ids              # only collect IDs
  python scraper.py --phase replays          # only fetch replays (IDs must exist)
"""

import argparse
import asyncio
import json
import os
import sys
import time
from pathlib import Path

import aiohttp

BASE_URL = "https://replay.pokemonshowdown.com"
SEARCH_PAGE_SIZE = 51  # API returns up to 51 results per page


def get_output_dir(format_id: str) -> Path:
    return Path(__file__).parent / "data" / format_id


def get_ids_file(format_id: str) -> Path:
    return get_output_dir(format_id) / "_replay_ids.json"


def get_replay_dir(format_id: str) -> Path:
    d = get_output_dir(format_id) / "replays"
    d.mkdir(parents=True, exist_ok=True)
    return d


def replay_filename(replay_id: str) -> str:
    return replay_id.replace("/", "_") + ".json"


# ── Phase 1: Collect replay IDs ──────────────────────────────────────────────


async def fetch_search_page(
    session: aiohttp.ClientSession,
    format_id: str,
    before: int | None = None,
) -> list[dict]:
    params = {"format": format_id}
    if before is not None:
        params["before"] = str(before)

    url = f"{BASE_URL}/search.json"
    async with session.get(url, params=params) as resp:
        if resp.status != 200:
            print(f"  Search request failed with status {resp.status}")
            return []
        return await resp.json()


async def collect_all_ids(format_id: str, delay: float) -> list[dict]:
    """Paginate through search results to collect all replay metadata."""
    all_replays: list[dict] = []
    before: int | None = None
    page = 0

    ids_file = get_ids_file(format_id)

    # Resume from existing IDs file if it exists and was incomplete
    if ids_file.exists():
        with open(ids_file) as f:
            existing = json.load(f)
        if existing:
            all_replays = existing
            before = existing[-1]["uploadtime"]
            page = len(existing) // 50
            print(f"  Resuming ID collection from page {page} ({len(existing)} IDs so far)")

    async with aiohttp.ClientSession() as session:
        while True:
            page += 1
            results = await fetch_search_page(session, format_id, before)

            if not results:
                print(f"  Page {page}: no results — done.")
                break

            new_count = 0
            existing_ids = {r["id"] for r in all_replays}
            for r in results:
                if r["id"] not in existing_ids:
                    all_replays.append(r)
                    new_count += 1

            print(f"  Page {page}: {len(results)} results, {new_count} new (total: {len(all_replays)})")

            if len(results) < SEARCH_PAGE_SIZE:
                print(f"  Fewer than {SEARCH_PAGE_SIZE} results — reached the end.")
                break

            before = results[-1]["uploadtime"]

            # Save progress periodically
            if page % 10 == 0:
                ids_file.parent.mkdir(parents=True, exist_ok=True)
                with open(ids_file, "w") as f:
                    json.dump(all_replays, f)

            await asyncio.sleep(delay)

    # Final save
    ids_file.parent.mkdir(parents=True, exist_ok=True)
    with open(ids_file, "w") as f:
        json.dump(all_replays, f)

    return all_replays


# ── Phase 2: Fetch full replays ──────────────────────────────────────────────


async def fetch_replay(
    session: aiohttp.ClientSession,
    replay_id: str,
    replay_dir: Path,
    semaphore: asyncio.Semaphore,
    delay: float,
) -> bool:
    """Fetch a single replay's full JSON and save to disk. Returns True on success."""
    filename = replay_dir / replay_filename(replay_id)
    if filename.exists():
        return True  # Already fetched

    async with semaphore:
        url = f"{BASE_URL}/{replay_id}.json"
        try:
            async with session.get(url) as resp:
                if resp.status == 404:
                    print(f"  404: {replay_id}")
                    return False
                if resp.status != 200:
                    print(f"  HTTP {resp.status}: {replay_id}")
                    return False
                data = await resp.json()
        except Exception as e:
            print(f"  Error fetching {replay_id}: {e}")
            return False

        with open(filename, "w", encoding="utf-8") as f:
            json.dump(data, f)

        await asyncio.sleep(delay)
        return True


async def fetch_all_replays(
    format_id: str,
    concurrency: int,
    delay: float,
) -> None:
    """Fetch full replay data for all collected IDs."""
    ids_file = get_ids_file(format_id)
    if not ids_file.exists():
        print(f"No IDs file found at {ids_file}. Run phase 'ids' first.")
        return

    with open(ids_file) as f:
        replays = json.load(f)

    replay_dir = get_replay_dir(format_id)

    # Figure out which ones still need fetching
    already_fetched = {
        p.stem.replace("_", "/")
        for p in replay_dir.glob("*.json")
        if p.name != "_replay_ids.json"
    }
    to_fetch = [r for r in replays if r["id"] not in already_fetched]

    print(f"  Total IDs: {len(replays)}")
    print(f"  Already fetched: {len(already_fetched)}")
    print(f"  Remaining: {len(to_fetch)}")

    if not to_fetch:
        print("  Nothing to fetch — all done!")
        return

    semaphore = asyncio.Semaphore(concurrency)
    success = 0
    failed = 0

    connector = aiohttp.TCPConnector(limit=concurrency)
    async with aiohttp.ClientSession(connector=connector) as session:
        # Process in batches for progress reporting
        batch_size = 100
        for i in range(0, len(to_fetch), batch_size):
            batch = to_fetch[i : i + batch_size]
            tasks = [
                fetch_replay(session, r["id"], replay_dir, semaphore, delay)
                for r in batch
            ]
            results = await asyncio.gather(*tasks)
            batch_success = sum(1 for r in results if r)
            batch_failed = sum(1 for r in results if not r)
            success += batch_success
            failed += batch_failed
            total_done = len(already_fetched) + success + failed
            print(
                f"  Progress: {total_done}/{len(replays)} "
                f"({success} fetched, {failed} failed this session)"
            )

    print(f"\nDone. {success} fetched, {failed} failed.")


# ── Main ─────────────────────────────────────────────────────────────────────


def main():
    parser = argparse.ArgumentParser(description="Scrape Pokemon Showdown replays")
    parser.add_argument(
        "--format",
        default="gen9vgc2025regi",
        help="Showdown format ID (default: gen9vgc2025regi)",
    )
    parser.add_argument(
        "--phase",
        choices=["ids", "replays", "both"],
        default="both",
        help="Which phase to run (default: both)",
    )
    parser.add_argument(
        "--concurrency",
        type=int,
        default=10,
        help="Max concurrent replay fetches (default: 10)",
    )
    parser.add_argument(
        "--delay",
        type=float,
        default=0.1,
        help="Delay in seconds between requests (default: 0.1)",
    )
    args = parser.parse_args()

    print(f"Format: {args.format}")
    print(f"Output: {get_output_dir(args.format)}")
    print()

    if args.phase in ("ids", "both"):
        print("Phase 1: Collecting replay IDs...")
        t0 = time.time()
        replays = asyncio.run(collect_all_ids(args.format, args.delay))
        elapsed = time.time() - t0
        print(f"  Collected {len(replays)} IDs in {elapsed:.1f}s\n")

    if args.phase in ("replays", "both"):
        print(f"Phase 2: Fetching full replays (concurrency={args.concurrency})...")
        t0 = time.time()
        asyncio.run(fetch_all_replays(args.format, args.concurrency, args.delay))
        elapsed = time.time() - t0
        print(f"  Completed in {elapsed:.1f}s\n")


if __name__ == "__main__":
    main()
