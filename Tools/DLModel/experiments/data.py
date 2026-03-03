"""
Data management: loading games, building vocab, creating persistent
train/val/test splits, and constructing DataLoaders.

Splits are done at the game level (not sample level) to prevent data
leakage — both player perspectives of the same game stay in the same split.
Split indices are persisted to JSON so every experiment uses identical data.
"""

from __future__ import annotations

import json
import random
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


def make_loaders(
    train_games: list[dict],
    val_games: list[dict],
    vocab: dict,
    batch_size: int,
    device: torch.device,
    winners_only: bool = True,
) -> tuple[DataLoader, DataLoader]:
    """Build train and val DataLoaders from game lists."""
    train_ds = TeamPreviewDataset(train_games, vocab, winners_only=winners_only)
    val_ds = TeamPreviewDataset(val_games, vocab, winners_only=winners_only)

    cuda = device.type == 'cuda'
    train_loader = DataLoader(
        train_ds, batch_size=batch_size, shuffle=True,
        num_workers=2, pin_memory=cuda,
        persistent_workers=True)
    val_loader = DataLoader(
        val_ds, batch_size=batch_size, shuffle=False,
        num_workers=2, pin_memory=cuda,
        persistent_workers=True)

    return train_loader, val_loader


def make_battle_loaders(
    train_games: list[dict],
    val_games: list[dict],
    vocab: dict,
    batch_size: int,
    device: torch.device,
    winners_only: bool = False,
) -> tuple[DataLoader, DataLoader]:
    """Build train and val DataLoaders for BattleNet from game lists."""
    train_ds = VGCDataset(train_games, vocab, winners_only=winners_only)
    val_ds = VGCDataset(val_games, vocab, winners_only=winners_only)

    cuda = device.type == 'cuda'
    train_loader = DataLoader(
        train_ds, batch_size=batch_size, shuffle=True,
        num_workers=2, pin_memory=cuda,
        persistent_workers=True)
    val_loader = DataLoader(
        val_ds, batch_size=batch_size, shuffle=False,
        num_workers=2, pin_memory=cuda,
        persistent_workers=True)

    return train_loader, val_loader
