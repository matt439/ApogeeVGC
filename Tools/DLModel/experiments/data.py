"""
Data management: loading games, building vocab, creating persistent
train/val/test splits, and constructing DataLoaders.

Splits are done at the game level (not sample level) to prevent data
leakage — both player perspectives of the same game stay in the same split.
Split indices are persisted to JSON so every experiment uses identical data.

Dataset tensors are cached to .pt files so subsequent runs skip the
expensive JSONL parse + Python encoding loop entirely.
"""

from __future__ import annotations

import json
import os
import random
import time
from concurrent.futures import ProcessPoolExecutor
from pathlib import Path

import torch
from torch.utils.data import DataLoader

import sys
sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
from dataset import build_vocab, VGCDataset
from team_preview_dataset import TeamPreviewDataset

from .config import DataConfig


def load_games(data_path: str | Path, min_rating: int = 0) -> list[dict]:
    """Load parsed games, optionally filtering by minimum player rating."""
    games = []
    with open(data_path, encoding='utf-8') as f:
        for line in f:
            g = json.loads(line)
            if g.get('winner') not in ('p1', 'p2'):
                continue
            if min_rating > 0:
                r1 = g.get('players', {}).get('p1', {}).get('rating_before')
                r2 = g.get('players', {}).get('p2', {}).get('rating_before')
                if r1 is None or r2 is None or min(r1, r2) < min_rating:
                    continue
            games.append(g)
    return games


def create_splits(
    games: list[dict],
    train_frac: float,
    val_frac: float,
    test_frac: float,
    seed: int,
    output_dir: Path,
) -> tuple[list[dict], list[dict], list[dict]]:
    """Shuffle games with a fixed seed and split into train/val/test.

    If split files already exist in output_dir/splits/, loads them
    instead of re-splitting, ensuring all experiments use the same data.
    """
    split_dir = output_dir / 'splits'
    train_path = split_dir / 'train_indices.json'
    val_path = split_dir / 'val_indices.json'
    test_path = split_dir / 'test_indices.json'

    if train_path.exists() and val_path.exists() and test_path.exists():
        with open(train_path) as f:
            train_idx = json.load(f)
        with open(val_path) as f:
            val_idx = json.load(f)
        with open(test_path) as f:
            test_idx = json.load(f)
        return (
            [games[i] for i in train_idx],
            [games[i] for i in val_idx],
            [games[i] for i in test_idx],
        )

    n = len(games)
    indices = list(range(n))
    rng = random.Random(seed)
    rng.shuffle(indices)

    n_train = int(n * train_frac)
    n_val = int(n * val_frac)

    train_idx = sorted(indices[:n_train])
    val_idx = sorted(indices[n_train:n_train + n_val])
    test_idx = sorted(indices[n_train + n_val:])

    split_dir.mkdir(parents=True, exist_ok=True)
    for path, idx in [(train_path, train_idx),
                      (val_path, val_idx),
                      (test_path, test_idx)]:
        with open(path, 'w') as f:
            json.dump(idx, f)

    return (
        [games[i] for i in train_idx],
        [games[i] for i in val_idx],
        [games[i] for i in test_idx],
    )


def get_or_build_vocab(data_path: str | Path, output_dir: Path) -> dict:
    """Load vocab from cache or build and cache it."""
    vocab_path = output_dir / 'vocab.json'
    if vocab_path.exists():
        with open(vocab_path) as f:
            return json.load(f)
    vocab = build_vocab(str(data_path))
    vocab_path.parent.mkdir(parents=True, exist_ok=True)
    with open(vocab_path, 'w') as f:
        json.dump(vocab, f, indent=2)
    return vocab


_PREVIEW_TENSOR_ATTRS = [
    'species_ids', 'move_ids', 'ability_ids', 'item_ids', 'tera_ids',
    'bring_target', 'lead_target', 'value_target',
]

_BATTLE_TENSOR_ATTRS = [
    'species_ids', 'move_ids', 'ability_ids', 'item_ids', 'tera_ids',
    'numeric', 'value_targets', 'policy_a', 'policy_b',
]


def _save_dataset_cache(ds, attrs: list[str], path: Path) -> None:
    """Save a dataset's tensors to a .pt file."""
    data = {'n': ds.n, 'n_games': getattr(ds, '_n_games', -1)}
    for attr in attrs:
        data[attr] = getattr(ds, attr)
    path.parent.mkdir(parents=True, exist_ok=True)
    torch.save(data, path)


def _load_preview_cache(path: Path, n_games: int) -> TeamPreviewDataset | None:
    """Load a cached TeamPreviewDataset, or None if cache is stale."""
    if not path.exists():
        return None
    data = torch.load(path, weights_only=False)
    if data.get('n_games', -1) != n_games:
        return None
    ds = object.__new__(TeamPreviewDataset)
    ds.n = data['n']
    for attr in _PREVIEW_TENSOR_ATTRS:
        setattr(ds, attr, data[attr])
    return ds


def _load_battle_cache(path: Path, n_games: int) -> VGCDataset | None:
    """Load a cached VGCDataset, or None if cache is stale."""
    if not path.exists():
        return None
    data = torch.load(path, weights_only=False)
    if data.get('n_games', -1) != n_games:
        return None
    ds = object.__new__(VGCDataset)
    ds.n = data['n']
    for attr in _BATTLE_TENSOR_ATTRS:
        setattr(ds, attr, data[attr])
    return ds


def _build_and_cache_preview(
    games: list[dict], vocab: dict, winners_only: bool, cache_path: Path | None,
) -> TeamPreviewDataset:
    ds = TeamPreviewDataset(games, vocab, winners_only=winners_only)
    ds._n_games = len(games)
    if cache_path is not None:
        _save_dataset_cache(ds, _PREVIEW_TENSOR_ATTRS, cache_path)
    return ds


def _build_and_cache_battle(
    games: list[dict], vocab: dict, winners_only: bool, cache_path: Path | None,
) -> VGCDataset:
    ds = VGCDataset(games, vocab, winners_only=winners_only)
    ds._n_games = len(games)
    if cache_path is not None:
        _save_dataset_cache(ds, _BATTLE_TENSOR_ATTRS, cache_path)
    return ds


def _build_preview_chunk(args):
    """Build a TeamPreviewDataset from a chunk of games (top-level for pickling)."""
    games, vocab, winners_only = args
    return TeamPreviewDataset(games, vocab, winners_only=winners_only)


def _build_battle_chunk(args):
    """Build a VGCDataset from a chunk of games (top-level for pickling)."""
    games, vocab, winners_only = args
    return VGCDataset(games, vocab, winners_only=winners_only)


def _merge_datasets(datasets, attrs):
    """Merge partial datasets by concatenating their tensors."""
    merged = object.__new__(type(datasets[0]))
    for attr in attrs:
        setattr(merged, attr, torch.cat([getattr(ds, attr) for ds in datasets], dim=0))
    merged.n = sum(ds.n for ds in datasets)
    return merged


def _build_parallel(build_fn, games, vocab, winners_only, num_workers, attrs):
    """Build a dataset using multiple worker processes."""
    chunk_size = (len(games) + num_workers - 1) // num_workers
    chunks = [games[i:i + chunk_size] for i in range(0, len(games), chunk_size)]
    args_list = [(chunk, vocab, winners_only) for chunk in chunks]

    with ProcessPoolExecutor(max_workers=num_workers) as pool:
        datasets = list(pool.map(build_fn, args_list))

    return _merge_datasets(datasets, attrs)


def _default_num_workers() -> int:
    """Sensible default: half of CPU cores (at least 1)."""
    return max((os.cpu_count() or 2) // 2, 1)


def build_preview_datasets(
    train_games: list[dict],
    val_games: list[dict],
    vocab: dict,
    winners_only: bool = True,
    cache_dir: Path | None = None,
    num_workers: int = 0,
) -> tuple[TeamPreviewDataset, TeamPreviewDataset]:
    """Build TeamPreviewNet datasets (expensive — call once, reuse across trials).

    If cache_dir is provided, tensors are saved to / loaded from .pt files,
    skipping the JSONL encoding on subsequent runs.

    num_workers > 0 parallelises the build across that many processes.
    """
    wo_tag = 'wo' if winners_only else 'all'
    if cache_dir is not None:
        cd = cache_dir / 'dataset_cache'
        train_path = cd / f'preview_train_{wo_tag}.pt'
        val_path = cd / f'preview_val_{wo_tag}.pt'
        train_ds = _load_preview_cache(train_path, len(train_games))
        val_ds = _load_preview_cache(val_path, len(val_games))
        if train_ds is not None and val_ds is not None:
            print(f'  Loaded cached preview datasets from {cd}')
            return train_ds, val_ds

    nw = num_workers if num_workers > 0 else _default_num_workers()
    t0 = time.time()
    train_cache = (cd / f'preview_train_{wo_tag}.pt') if cache_dir else None
    val_cache = (cd / f'preview_val_{wo_tag}.pt') if cache_dir else None

    if nw > 1 and len(train_games) >= nw * 10:
        print(f'  Building preview datasets with {nw} workers...')
        train_ds = _build_parallel(
            _build_preview_chunk, train_games, vocab, winners_only,
            nw, _PREVIEW_TENSOR_ATTRS)
        train_ds._n_games = len(train_games)
        if train_cache is not None:
            _save_dataset_cache(train_ds, _PREVIEW_TENSOR_ATTRS, train_cache)
        val_ds = _build_and_cache_preview(val_games, vocab, winners_only, val_cache)
    else:
        train_ds = _build_and_cache_preview(train_games, vocab, winners_only, train_cache)
        val_ds = _build_and_cache_preview(val_games, vocab, winners_only, val_cache)

    print(f'  Built preview datasets in {time.time() - t0:.1f}s'
          + (f' (cached to {cd})' if cache_dir else ''))
    return train_ds, val_ds


def build_battle_datasets(
    train_games: list[dict],
    val_games: list[dict],
    vocab: dict,
    winners_only: bool = False,
    cache_dir: Path | None = None,
    num_workers: int = 0,
) -> tuple[VGCDataset, VGCDataset]:
    """Build BattleNet datasets (expensive — call once, reuse across trials).

    If cache_dir is provided, tensors are saved to / loaded from .pt files,
    skipping the JSONL encoding on subsequent runs.

    num_workers > 0 parallelises the build across that many processes.
    """
    wo_tag = 'wo' if winners_only else 'all'
    if cache_dir is not None:
        cd = cache_dir / 'dataset_cache'
        train_path = cd / f'battle_train_{wo_tag}.pt'
        val_path = cd / f'battle_val_{wo_tag}.pt'
        train_ds = _load_battle_cache(train_path, len(train_games))
        val_ds = _load_battle_cache(val_path, len(val_games))
        if train_ds is not None and val_ds is not None:
            print(f'  Loaded cached battle datasets from {cd}')
            return train_ds, val_ds

    nw = num_workers if num_workers > 0 else _default_num_workers()
    t0 = time.time()
    train_cache = (cd / f'battle_train_{wo_tag}.pt') if cache_dir else None
    val_cache = (cd / f'battle_val_{wo_tag}.pt') if cache_dir else None

    if nw > 1 and len(train_games) >= nw * 10:
        print(f'  Building battle datasets with {nw} workers...')
        train_ds = _build_parallel(
            _build_battle_chunk, train_games, vocab, winners_only,
            nw, _BATTLE_TENSOR_ATTRS)
        train_ds._n_games = len(train_games)
        if train_cache is not None:
            _save_dataset_cache(train_ds, _BATTLE_TENSOR_ATTRS, train_cache)
        val_ds = _build_and_cache_battle(val_games, vocab, winners_only, val_cache)
    else:
        train_ds = _build_and_cache_battle(train_games, vocab, winners_only, train_cache)
        val_ds = _build_and_cache_battle(val_games, vocab, winners_only, val_cache)

    print(f'  Built battle datasets in {time.time() - t0:.1f}s'
          + (f' (cached to {cd})' if cache_dir else ''))
    return train_ds, val_ds


def build_preview_test_dataset(
    test_games: list[dict],
    vocab: dict,
    winners_only: bool = True,
    cache_dir: Path | None = None,
) -> TeamPreviewDataset:
    """Build (or load cached) a single TeamPreviewDataset for test evaluation."""
    wo_tag = 'wo' if winners_only else 'all'
    if cache_dir is not None:
        path = cache_dir / 'dataset_cache' / f'preview_test_{wo_tag}.pt'
        ds = _load_preview_cache(path, len(test_games))
        if ds is not None:
            return ds
    ds = _build_and_cache_preview(
        test_games, vocab, winners_only,
        (cache_dir / 'dataset_cache' / f'preview_test_{wo_tag}.pt') if cache_dir else None,
    )
    return ds


def build_battle_test_dataset(
    test_games: list[dict],
    vocab: dict,
    winners_only: bool = False,
    cache_dir: Path | None = None,
) -> VGCDataset:
    """Build (or load cached) a single VGCDataset for test evaluation."""
    wo_tag = 'wo' if winners_only else 'all'
    if cache_dir is not None:
        path = cache_dir / 'dataset_cache' / f'battle_test_{wo_tag}.pt'
        ds = _load_battle_cache(path, len(test_games))
        if ds is not None:
            return ds
    ds = _build_and_cache_battle(
        test_games, vocab, winners_only,
        (cache_dir / 'dataset_cache' / f'battle_test_{wo_tag}.pt') if cache_dir else None,
    )
    return ds


class _GPUBatchIter:
    """Drop-in DataLoader replacement for GPU-resident datasets.

    Yields batches by direct tensor slicing — eliminates all Python
    per-sample overhead (no __getitem__, no collate_fn, no iterator protocol).
    """
    def __init__(self, ds, batch_size: int, shuffle: bool):
        self.ds = ds
        self.batch_size = batch_size
        self.shuffle = shuffle
        self.attrs = _BATTLE_TENSOR_ATTRS if hasattr(ds, 'numeric') else _PREVIEW_TENSOR_ATTRS

    def __iter__(self):
        n = len(self.ds)
        if self.shuffle:
            perm = torch.randperm(n, device=self.ds.species_ids.device)
        else:
            perm = torch.arange(n, device=self.ds.species_ids.device)
        for start in range(0, n, self.batch_size):
            idx = perm[start:start + self.batch_size]
            yield tuple(getattr(self.ds, attr)[idx] for attr in self.attrs)

    def __len__(self):
        return (len(self.ds) + self.batch_size - 1) // self.batch_size


def _estimate_dataset_bytes(ds) -> int:
    """Estimate total bytes of a dataset's tensors."""
    attrs = _BATTLE_TENSOR_ATTRS if hasattr(ds, 'numeric') else _PREVIEW_TENSOR_ATTRS
    total = 0
    for attr in attrs:
        t = getattr(ds, attr)
        total += t.nelement() * t.element_size()
    return total


def _try_move_to_gpu(ds, headroom_bytes: int) -> bool:
    """Move dataset to GPU if enough VRAM is available. Returns True if moved."""
    if not torch.cuda.is_available() or not hasattr(ds, 'to'):
        return False
    if _tensors_on_cuda(ds):
        return True
    needed = _estimate_dataset_bytes(ds)
    free, _ = torch.cuda.mem_get_info()
    if needed < free - headroom_bytes:
        ds.to(torch.device('cuda'))
        return True
    return False


# 2 GB headroom for model + activations + gradients
_VRAM_HEADROOM = 2 * 1024**3


def loaders_from_datasets(
    train_ds,
    val_ds,
    batch_size: int,
    device: torch.device,
):
    """Wrap pre-built datasets into batch iterators (cheap — safe to call per trial).

    Auto-detects available VRAM. If both datasets fit with 2 GB headroom,
    moves them to GPU and uses _GPUBatchIter (zero-overhead slicing).
    Otherwise keeps data on CPU with pinned-memory DataLoaders.
    """
    if device.type == 'cuda' and hasattr(train_ds, 'to'):
        total_bytes = (_estimate_dataset_bytes(train_ds)
                       + _estimate_dataset_bytes(val_ds))
        free, _ = torch.cuda.mem_get_info()
        if total_bytes < free - _VRAM_HEADROOM:
            train_ds.to(device)
            val_ds.to(device)
        else:
            needed_mb = total_bytes / 1024**2
            free_mb = free / 1024**2
            print(f'  Dataset ({needed_mb:.0f} MB) exceeds available VRAM '
                  f'({free_mb:.0f} MB - 2 GB headroom), keeping on CPU')

    if _tensors_on_cuda(train_ds):
        return (
            _GPUBatchIter(train_ds, batch_size, shuffle=True),
            _GPUBatchIter(val_ds, batch_size, shuffle=False),
        )

    pin = device.type == 'cuda'
    train_loader = DataLoader(
        train_ds, batch_size=batch_size, shuffle=True,
        num_workers=0, pin_memory=pin)
    val_loader = DataLoader(
        val_ds, batch_size=batch_size, shuffle=False,
        num_workers=0, pin_memory=pin)
    return train_loader, val_loader


def make_batch_iter(ds, batch_size: int, device: torch.device, shuffle: bool = False):
    """Create a batch iterator for a single dataset (e.g. test set).

    Auto-detects VRAM: moves to GPU if it fits, otherwise uses pinned DataLoader.
    """
    if not _tensors_on_cuda(ds) and device.type == 'cuda':
        _try_move_to_gpu(ds, _VRAM_HEADROOM)

    if _tensors_on_cuda(ds):
        return _GPUBatchIter(ds, batch_size, shuffle=shuffle)
    return DataLoader(
        ds, batch_size=batch_size, shuffle=shuffle,
        num_workers=0, pin_memory=device.type == 'cuda')


def _tensors_on_cuda(ds) -> bool:
    """Check if a dataset's tensors are already on CUDA."""
    if hasattr(ds, 'species_ids'):
        return ds.species_ids.is_cuda
    return False

