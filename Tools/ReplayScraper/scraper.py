"""
Pokemon Showdown Replay Scraper

Scrapes battle replays from replay.pokemonshowdown.com for a given format.
Two-phase approach:
  1. Collect all replay IDs by paginating through the search API
  2. Fetch full replay JSON (including battle log) for each ID

Supports resumption — skips already-fetched replays on restart.
Uses JSONL for the IDs file so resumption is fast (append-only, no full reload).

Usage:
  python scraper.py                          # uses defaults
  python scraper.py --format gen9vgc2025regi
  python scraper.py --format gen9vgc2025regi gen9randombattle  # multiple formats
  python scraper.py --format gen9vgc2024regh --concurrency 20
  python scraper.py --phase ids              # only collect IDs
  python scraper.py --phase replays          # only fetch replays (IDs must exist)
"""

import argparse
import json
import time
import urllib.request
import urllib.parse
from concurrent.futures import ThreadPoolExecutor, as_completed
from pathlib import Path

BASE_URL = "https://replay.pokemonshowdown.com"
SEARCH_PAGE_SIZE = 51  # API returns up to 51 results per page
_HEADERS = {"User-Agent": "Mozilla/5.0 (compatible; ApogeeVGC/1.0)"}


def _urlopen(url: str, timeout: int = 30):
    """urlopen wrapper that sets a proper User-Agent."""
    req = urllib.request.Request(url, headers=_HEADERS)
    return urllib.request.urlopen(req, timeout=timeout)


def get_output_dir(format_id: str) -> Path:
    return Path(__file__).parent / "data" / format_id


def get_ids_file(format_id: str) -> Path:
    return get_output_dir(format_id) / "_replay_ids.jsonl"


def get_replay_dir(format_id: str) -> Path:
    d = get_output_dir(format_id) / "replays"
    d.mkdir(parents=True, exist_ok=True)
    return d


def replay_filename(replay_id: str) -> str:
    return replay_id.replace("/", "_") + ".json"


def _migrate_json_to_jsonl(format_id: str) -> None:
    """One-time streaming migration from old _replay_ids.json to JSONL format.

    Uses incremental JSON parsing to avoid loading the entire file into memory.
    """
    old_file = get_output_dir(format_id) / "_replay_ids.json"
    new_file = get_ids_file(format_id)
    if not old_file.exists() or new_file.exists():
        return
    file_size = old_file.stat().st_size / (1024 * 1024)
    print(f"  Migrating {old_file.name} ({file_size:.1f}MB) -> {new_file.name}...", flush=True)
    new_file.parent.mkdir(parents=True, exist_ok=True)
    decoder = json.JSONDecoder()
    count = 0
    buf = ""
    with open(old_file, encoding="utf-8") as fin, \
         open(new_file, "w", encoding="utf-8") as fout:
        while True:
            chunk = fin.read(1024 * 1024)  # 1MB at a time
            if not chunk:
                break
            buf += chunk
            # Parse as many complete JSON objects as possible from buf
            while buf:
                buf = buf.lstrip(" \t\n\r,[]")
                if not buf:
                    break
                try:
                    obj, idx = decoder.raw_decode(buf)
                    fout.write(json.dumps(obj, ensure_ascii=False) + "\n")
                    buf = buf[idx:]
                    count += 1
                    if count % 50000 == 0:
                        print(f"    {count} entries migrated...", flush=True)
                except json.JSONDecodeError:
                    break  # incomplete object, need more data
    old_file.unlink()
    print(f"  Migrated {count} entries.", flush=True)


def _load_resume_state(ids_file: Path) -> tuple[set[str], int | None, int]:
    """Stream JSONL to build seen_ids set and find last uploadtime.

    Only keeps IDs in memory (not full dicts). Returns (seen_ids, before, count).
    """
    seen_ids: set[str] = set()
    before: int | None = None
    count = 0
    with open(ids_file, encoding="utf-8") as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            r = json.loads(line)
            seen_ids.add(r["id"])
            before = r["uploadtime"]
            count += 1
    return seen_ids, before, count


# ── Phase 1: Collect replay IDs ──────────────────────────────────────────────


def fetch_search_page(
    format_id: str,
    before: int | None = None,
) -> list[dict]:
    params = {"format": format_id}
    if before is not None:
        params["before"] = str(before)

    url = f"{BASE_URL}/search.json?{urllib.parse.urlencode(params)}"
    try:
        with _urlopen(url) as resp:
            if resp.status != 200:
                print(f"  Search request failed with status {resp.status}", flush=True)
                return []
            return json.loads(resp.read().decode("utf-8"))
    except Exception as e:
        print(f"  Search request error: {e}", flush=True)
        return []


def collect_all_ids(format_id: str, delay: float) -> int:
    """Paginate through search results to collect all replay metadata.

    Returns the total number of IDs collected.
    """
    seen_ids: set[str] = set()
    before: int | None = None
    page = 0
    count = 0

    ids_file = get_ids_file(format_id)
    _migrate_json_to_jsonl(format_id)

    # Resume from existing JSONL file
    if ids_file.exists():
        seen_ids, before, count = _load_resume_state(ids_file)
        if count > 0:
            page = count // 50
            print(f"  Resuming ID collection from page {page} ({count} IDs so far)", flush=True)

    ids_file.parent.mkdir(parents=True, exist_ok=True)

    # Open in append mode so new entries are added incrementally
    with open(ids_file, "a", encoding="utf-8") as out:
        while True:
            page += 1
            results = fetch_search_page(format_id, before)

            if not results:
                print(f"  Page {page}: no results — done.", flush=True)
                break

            new_count = 0
            for r in results:
                if r["id"] not in seen_ids:
                    seen_ids.add(r["id"])
                    out.write(json.dumps(r, ensure_ascii=False) + "\n")
                    count += 1
                    new_count += 1

            # Flush periodically so progress is durable
            if page % 10 == 0:
                out.flush()

            print(f"  Page {page}: {len(results)} results, {new_count} new (total: {count})", flush=True)

            if len(results) < SEARCH_PAGE_SIZE:
                print(f"  Fewer than {SEARCH_PAGE_SIZE} results — reached the end.", flush=True)
                break

            before = results[-1]["uploadtime"]

            time.sleep(delay)

    return count


# ── Phase 2: Fetch full replays ──────────────────────────────────────────────


def fetch_replay(
    replay_id: str,
    replay_dir: Path,
    delay: float,
) -> bool:
    """Fetch a single replay's full JSON and save to disk. Returns True on success."""
    filename = replay_dir / replay_filename(replay_id)
    if filename.exists():
        return True  # Already fetched

    url = f"{BASE_URL}/{replay_id}.json"
    try:
        with _urlopen(url) as resp:
            if resp.status == 404:
                print(f"  404: {replay_id}", flush=True)
                return False
            if resp.status != 200:
                print(f"  HTTP {resp.status}: {replay_id}", flush=True)
                return False
            data = json.loads(resp.read().decode("utf-8"))
    except Exception as e:
        print(f"  Error fetching {replay_id}: {e}", flush=True)
        return False

    with open(filename, "w", encoding="utf-8") as f:
        json.dump(data, f)

    time.sleep(delay)
    return True


def fetch_all_replays(
    format_id: str,
    concurrency: int,
    delay: float,
) -> None:
    """Fetch full replay data for all collected IDs."""
    ids_file = get_ids_file(format_id)
    if not ids_file.exists():
        print(f"No IDs file found at {ids_file}. Run phase 'ids' first.", flush=True)
        return

    # Stream JSONL — only keep IDs, not full dicts
    all_ids: list[str] = []
    with open(ids_file, encoding="utf-8") as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            r = json.loads(line)
            all_ids.append(r["id"])

    replay_dir = get_replay_dir(format_id)

    # Figure out which ones still need fetching
    already_fetched = {
        p.stem.replace("_", "/")
        for p in replay_dir.glob("*.json")
    }
    to_fetch = [rid for rid in all_ids if rid not in already_fetched]

    print(f"  Total IDs: {len(all_ids)}", flush=True)
    print(f"  Already fetched: {len(already_fetched)}", flush=True)
    print(f"  Remaining: {len(to_fetch)}", flush=True)

    if not to_fetch:
        print("  Nothing to fetch — all done!", flush=True)
        return

    success = 0
    failed = 0

    with ThreadPoolExecutor(max_workers=concurrency) as executor:
        # Process in batches for progress reporting
        batch_size = 100
        for i in range(0, len(to_fetch), batch_size):
            batch = to_fetch[i : i + batch_size]
            futures = {
                executor.submit(fetch_replay, rid, replay_dir, delay): rid
                for rid in batch
            }
            for future in as_completed(futures):
                if future.result():
                    success += 1
                else:
                    failed += 1

            total_done = len(already_fetched) + success + failed
            print(
                f"  Progress: {total_done}/{len(all_ids)} "
                f"({success} fetched, {failed} failed this session)",
                flush=True,
            )

    print(f"\nDone. {success} fetched, {failed} failed.", flush=True)


# ── Main ─────────────────────────────────────────────────────────────────────


def main():
    parser = argparse.ArgumentParser(description="Scrape Pokemon Showdown replays")
    parser.add_argument(
        "--format",
        nargs="+",
        default=["gen9vgc2025regi"],
        help="Showdown format ID(s) (default: gen9vgc2025regi)",
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

    for fmt in args.format:
        print(f"{'='*60}", flush=True)
        print(f"Format: {fmt}", flush=True)
        print(f"Output: {get_output_dir(fmt)}", flush=True)
        print(flush=True)

        if args.phase in ("ids", "both"):
            print("Phase 1: Collecting replay IDs...", flush=True)
            t0 = time.time()
            total = collect_all_ids(fmt, args.delay)
            elapsed = time.time() - t0
            print(f"  Collected {total} IDs in {elapsed:.1f}s\n", flush=True)

        if args.phase in ("replays", "both"):
            print(f"Phase 2: Fetching full replays (concurrency={args.concurrency})...", flush=True)
            t0 = time.time()
            fetch_all_replays(fmt, args.concurrency, args.delay)
            elapsed = time.time() - t0
            print(f"  Completed in {elapsed:.1f}s\n", flush=True)


if __name__ == "__main__":
    main()
