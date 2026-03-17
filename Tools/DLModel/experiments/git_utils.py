"""
Git utilities for commit-based experiment versioning.

Provides functions to detect the current commit hash, manage a
``latest.txt`` pointer, and resolve commit identifiers for use
in results directory paths.
"""

from __future__ import annotations

import subprocess
import sys
from datetime import datetime
from pathlib import Path


# Repo root: three levels up from experiments/ → DLModel/ → Tools/ → ApogeeVGC/
_REPO_ROOT = Path(__file__).resolve().parents[3]


def get_commit_hash(short: bool = True, warn_dirty: bool = True) -> str:
    """Return the current HEAD commit hash.

    Parameters
    ----------
    short : bool
        If True, return only the first 8 characters.
    warn_dirty : bool
        If True, append ``-dirty`` and print a warning when the
        working tree has uncommitted changes.

    Returns
    -------
    str
        Commit hash, e.g. ``"9483713e"`` or ``"9483713e-dirty"``.
        Falls back to ``"nogit_<timestamp>"`` if git is unavailable.
    """
    try:
        result = subprocess.run(
            ['git', 'rev-parse', 'HEAD'],
            cwd=_REPO_ROOT,
            capture_output=True, text=True, check=True,
        )
        commit = result.stdout.strip()
        if short:
            commit = commit[:8]

        if warn_dirty:
            status = subprocess.run(
                ['git', 'status', '--porcelain'],
                cwd=_REPO_ROOT,
                capture_output=True, text=True,
            )
            if status.stdout.strip():
                print(f'WARNING: Working tree is dirty — '
                      f'results tagged as {commit}-dirty',
                      file=sys.stderr)
                commit += '-dirty'

        return commit

    except (FileNotFoundError, subprocess.CalledProcessError):
        ts = datetime.now().strftime('%Y%m%d_%H%M%S')
        fallback = f'nogit_{ts}'
        print(f'WARNING: git not available — using {fallback}',
              file=sys.stderr)
        return fallback


def update_latest_pointer(results_reg_dir: Path, commit_hash: str) -> None:
    """Write ``commit_hash`` into ``results_reg_dir/latest.txt``."""
    results_reg_dir.mkdir(parents=True, exist_ok=True)
    (results_reg_dir / 'latest.txt').write_text(commit_hash + '\n')


def resolve_commit(results_reg_dir: Path, commit_or_latest: str) -> str:
    """Resolve a commit identifier.

    If *commit_or_latest* is ``"latest"``, reads the hash from
    ``results_reg_dir/latest.txt``.  Otherwise returns the argument
    unchanged.

    Raises
    ------
    FileNotFoundError
        If ``"latest"`` is requested but ``latest.txt`` does not exist.
    """
    if commit_or_latest == 'latest':
        latest_path = results_reg_dir / 'latest.txt'
        if not latest_path.exists():
            raise FileNotFoundError(
                f'No latest.txt found in {results_reg_dir}. '
                f'Run an experiment first, or specify --commit explicitly.')
        return latest_path.read_text().strip()
    return commit_or_latest
