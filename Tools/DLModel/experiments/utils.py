"""Shared utilities for logging, timing, and path management."""

from __future__ import annotations

import json
import time
from contextlib import contextmanager
from pathlib import Path


@contextmanager
def timer(label: str):
    """Context manager that prints elapsed time."""
    t0 = time.time()
    yield
    elapsed = time.time() - t0
    print(f'  [{label}] {elapsed:.1f}s')


def save_json(data: dict, path: str | Path) -> None:
    """Save dict to JSON file, creating parent dirs."""
    p = Path(path)
    p.parent.mkdir(parents=True, exist_ok=True)
    with open(p, 'w') as f:
        json.dump(data, f, indent=2)


def load_json(path: str | Path) -> dict:
    """Load JSON file."""
    with open(path) as f:
        return json.load(f)
