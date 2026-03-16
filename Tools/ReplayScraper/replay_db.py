"""
SQLite-backed storage for replay JSON data.

Each format gets a single .db file at data/<format_id>.db containing:
  - replays table: id (replay ID string) -> json (raw replay JSON)
  - metadata table: key/value pairs (e.g. scraper state)

Thread-safe for concurrent writes (WAL mode) and multiprocessing reads.
"""

import sqlite3
from pathlib import Path


def get_db_path(format_id: str) -> Path:
    return Path(__file__).parent / "data" / f"{format_id}.db"


def open_db(format_id: str, *, readonly: bool = False) -> sqlite3.Connection:
    """Open (or create) the SQLite database for a format."""
    db_path = get_db_path(format_id)
    db_path.parent.mkdir(parents=True, exist_ok=True)

    if readonly:
        uri = f"file:{db_path}?mode=ro"
        conn = sqlite3.connect(uri, uri=True)
    else:
        conn = sqlite3.connect(str(db_path))

    conn.execute("PRAGMA journal_mode = WAL")
    conn.execute("PRAGMA synchronous = NORMAL")
    conn.execute("PRAGMA cache_size = -64000")  # 64 MB

    if not readonly:
        conn.execute("""
            CREATE TABLE IF NOT EXISTS replays (
                id TEXT PRIMARY KEY,
                json TEXT NOT NULL
            )
        """)
        conn.execute("""
            CREATE TABLE IF NOT EXISTS metadata (
                key TEXT PRIMARY KEY,
                value TEXT NOT NULL
            )
        """)
        conn.commit()

    return conn


def replay_exists(conn: sqlite3.Connection, replay_id: str) -> bool:
    row = conn.execute(
        "SELECT 1 FROM replays WHERE id = ?", (replay_id,)
    ).fetchone()
    return row is not None


def insert_replay(conn: sqlite3.Connection, replay_id: str, json_str: str) -> None:
    conn.execute(
        "INSERT OR IGNORE INTO replays (id, json) VALUES (?, ?)",
        (replay_id, json_str),
    )
    conn.commit()


def count_replays(conn: sqlite3.Connection) -> int:
    row = conn.execute("SELECT COUNT(*) FROM replays").fetchone()
    return row[0]


def iter_all_replay_ids(conn: sqlite3.Connection):
    """Yield all replay IDs (for checking which replays are already fetched)."""
    cursor = conn.execute("SELECT id FROM replays")
    for row in cursor:
        yield row[0]


def iter_all_replays(conn: sqlite3.Connection, batch_size: int = 1000):
    """Yield (id, json_str) for all replays in batches.

    Uses a server-side cursor to avoid loading everything into memory.
    """
    cursor = conn.execute("SELECT id, json FROM replays")
    while True:
        rows = cursor.fetchmany(batch_size)
        if not rows:
            break
        yield from rows
