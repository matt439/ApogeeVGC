"""
Training script for TeamPreviewNet (VGC — bring 4, lead 2).

Trains the model to classify team preview decisions into one of 90
configurations (C(6,4) × C(4,2) = 15 × 6 = 90) using cross-entropy loss.

Usage:
  python train_team_preview.py
  python train_team_preview.py --min-rating 1300 --epochs 50
"""

from __future__ import annotations

import argparse
import json
import random
import time
from pathlib import Path

import torch
import torch.nn as nn
from torch.amp import autocast, GradScaler
from torch.utils.data import DataLoader

from dataset import build_vocab
from team_preview_dataset import TeamPreviewDataset
from team_preview_model import TeamPreviewNet
from format_spec import FormatSpec, VGC, FORMAT_REGISTRY


def load_games(path: str, min_rating: int = 0) -> list[dict]:
    games = []
    with open(path, encoding='utf-8') as f:
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


def compute_config_accuracy(logits: torch.Tensor, targets: torch.Tensor) -> float:
    """Exact config match accuracy."""
    preds = logits.argmax(dim=1)
    return (preds == targets).float().mean().item()


def compute_bring_accuracy(logits: torch.Tensor, targets: torch.Tensor,
                           fmt: FormatSpec) -> float:
    """Bring-set accuracy: does the predicted config's bring set match?"""
    preds = logits.argmax(dim=1)
    configs = fmt.configs
    correct = 0
    total = preds.size(0)
    for i in range(total):
        pred_bring = set(configs[preds[i].item()][0])
        true_bring = set(configs[targets[i].item()][0])
        if pred_bring == true_bring:
            correct += 1
    return correct / max(total, 1)


def compute_lead_accuracy(logits: torch.Tensor, targets: torch.Tensor,
                          fmt: FormatSpec) -> float:
    """Lead-set accuracy: does the predicted config's lead pair match?"""
    preds = logits.argmax(dim=1)
    configs = fmt.configs
    correct = 0
    total = preds.size(0)
    for i in range(total):
        pred_lead = set(configs[preds[i].item()][1])
        true_lead = set(configs[targets[i].item()][1])
        if pred_lead == true_lead:
            correct += 1
    return correct / max(total, 1)


def train(args: argparse.Namespace) -> None:
    fmt = FORMAT_REGISTRY[args.format]
    torch.manual_seed(args.seed)
    random.seed(args.seed)

    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    print(f'Device: {device}')

    if device.type == 'cuda':
        torch.backends.cudnn.benchmark = True

    use_amp = device.type == 'cuda'
    scaler = GradScaler('cuda', enabled=use_amp)

    # ── Vocab ──
    vocab_path = Path(args.vocab)
    if not vocab_path.exists():
        print(f'Building vocab from {args.data}...')
        vocab = build_vocab(args.data)
        vocab_path.parent.mkdir(parents=True, exist_ok=True)
        with open(vocab_path, 'w') as f:
            json.dump(vocab, f, indent=2)
    else:
        with open(vocab_path) as f:
            vocab = json.load(f)

    print(f'Vocab: {vocab["num_species"]} species, '
          f'{vocab["num_moves"]} moves, {vocab["num_abilities"]} abilities, '
          f'{vocab["num_items"]} items, {vocab["num_tera_types"]} tera types')

    # ── Data ──
    print(f'Loading games from {args.data}...')
    games = load_games(args.data, args.min_rating)
    print(f'  {len(games)} games')

    if not games:
        print('No games found.')
        return

    random.shuffle(games)
    split = int(len(games) * (1 - args.val_split))
    train_games = games[:split]
    val_games = games[split:]

    print('Building datasets...')
    t0 = time.time()
    train_ds = TeamPreviewDataset(train_games, vocab, format_spec=fmt)
    val_ds = TeamPreviewDataset(val_games, vocab, format_spec=fmt)
    print(f'  {len(train_ds):,} train, {len(val_ds):,} val '
          f'({time.time() - t0:.1f}s)')

    cuda = device.type == 'cuda'
    train_loader = DataLoader(
        train_ds, batch_size=args.batch_size, shuffle=True,
        num_workers=4, pin_memory=cuda,
        persistent_workers=True)
    val_loader = DataLoader(
        val_ds, batch_size=args.batch_size, shuffle=False,
        num_workers=4, pin_memory=cuda,
        persistent_workers=True)

    # ── Model ──
    model = TeamPreviewNet(
        num_species=vocab['num_species'],
        num_moves=vocab['num_moves'],
        num_abilities=vocab['num_abilities'],
        num_items=vocab['num_items'],
        num_tera_types=vocab['num_tera_types'],
        format_spec=fmt,
        species_embed_dim=args.embed_dim,
        feat_embed_dim=args.feat_embed_dim,
        pokemon_dim=args.pokemon_dim,
        hidden_dim=args.hidden_dim,
    ).to(device)
    total_params = sum(p.numel() for p in model.parameters())
    print(f'Model: {total_params:,} parameters')
    print(f'Format: {fmt.name} (bring {fmt.team_size}, lead {fmt.num_leads}, '
          f'{fmt.num_configs} configs)')

    optimizer = torch.optim.Adam(
        model.parameters(), lr=args.lr, weight_decay=1e-5)
    scheduler = torch.optim.lr_scheduler.CosineAnnealingLR(
        optimizer, args.epochs)

    loss_fn = nn.CrossEntropyLoss()

    best_val_loss = float('inf')
    patience_counter = 0

    print(f'\nTraining for {args.epochs} epochs...\n')

    for epoch in range(args.epochs):
        # ── Train ──
        model.train()
        t_loss = 0.0
        n_batches = 0

        for sids, mids, aids, iids, tids, cfg_tgt, val_tgt in train_loader:
            sids = sids.to(device, non_blocking=True)
            mids = mids.to(device, non_blocking=True)
            aids = aids.to(device, non_blocking=True)
            iids = iids.to(device, non_blocking=True)
            tids = tids.to(device, non_blocking=True)
            cfg_tgt = cfg_tgt.to(device, non_blocking=True)

            with autocast('cuda', enabled=use_amp):
                logits = model(sids, mids, aids, iids, tids)
                loss = loss_fn(logits, cfg_tgt)

            optimizer.zero_grad()
            scaler.scale(loss).backward()
            scaler.unscale_(optimizer)
            torch.nn.utils.clip_grad_norm_(model.parameters(), 1.0)
            scaler.step(optimizer)
            scaler.update()

            t_loss += loss.item()
            n_batches += 1

        scheduler.step()

        # ── Validate ──
        model.eval()
        v_loss = 0.0
        config_acc_sum = 0.0
        bring_acc_sum = 0.0
        lead_acc_sum = 0.0
        n_vbatches = 0

        with torch.no_grad():
            for sids, mids, aids, iids, tids, cfg_tgt, val_tgt in val_loader:
                sids = sids.to(device, non_blocking=True)
                mids = mids.to(device, non_blocking=True)
                aids = aids.to(device, non_blocking=True)
                iids = iids.to(device, non_blocking=True)
                tids = tids.to(device, non_blocking=True)
                cfg_tgt = cfg_tgt.to(device, non_blocking=True)

                with autocast('cuda', enabled=use_amp):
                    logits = model(sids, mids, aids, iids, tids)
                    loss = loss_fn(logits, cfg_tgt)

                v_loss += loss.item()
                config_acc_sum += compute_config_accuracy(logits, cfg_tgt)
                bring_acc_sum += compute_bring_accuracy(logits, cfg_tgt, fmt)
                lead_acc_sum += compute_lead_accuracy(logits, cfg_tgt, fmt)
                n_vbatches += 1

        avg_t = t_loss / n_batches
        avg_v = v_loss / n_vbatches
        config_acc = config_acc_sum / n_vbatches
        bring_acc = bring_acc_sum / n_vbatches
        lead_acc = lead_acc_sum / n_vbatches
        lr = scheduler.get_last_lr()[0]

        print(
            f'Epoch {epoch + 1:2d}/{args.epochs} | '
            f'Train: {avg_t:.4f} | Val: {avg_v:.4f} | '
            f'Acc: config={config_acc:.3f} bring={bring_acc:.3f} lead={lead_acc:.3f} | '
            f'LR: {lr:.6f}'
        )

        if avg_v < best_val_loss:
            best_val_loss = avg_v
            patience_counter = 0
            torch.save({
                'model_state_dict': model.state_dict(),
                'vocab': vocab,
                'args': vars(args),
                'epoch': epoch + 1,
                'val_loss': avg_v,
            }, args.output)
            print(f'  -> Saved best model to {args.output}')
        else:
            patience_counter += 1
            if patience_counter >= args.patience:
                print(f'  Early stopping after {args.patience} epochs '
                      f'without improvement')
                break

    print(f'\nBest val loss: {best_val_loss:.4f}')
    print(f'Model saved to {args.output}')


def main():
    parser = argparse.ArgumentParser(
        description='Train TeamPreviewNet')
    parser.add_argument(
        '--data',
        default='../ReplayScraper/data/gen9vgc2025regi/parsed.jsonl')
    parser.add_argument('--vocab', default='vocab.json')
    parser.add_argument('--min-rating', type=int, default=0)
    parser.add_argument('--epochs', type=int, default=30)
    parser.add_argument('--batch-size', type=int, default=1024)
    parser.add_argument('--lr', type=float, default=1e-3)
    parser.add_argument('--embed-dim', type=int, default=48)
    parser.add_argument('--feat-embed-dim', type=int, default=16)
    parser.add_argument('--pokemon-dim', type=int, default=64)
    parser.add_argument('--hidden-dim', type=int, default=256)
    parser.add_argument('--val-split', type=float, default=0.2)
    parser.add_argument('--patience', type=int, default=5)
    parser.add_argument('--seed', type=int, default=42)
    parser.add_argument('--format', default='vgc',
                        choices=list(FORMAT_REGISTRY.keys()),
                        help='Battle format (determines config space)')
    parser.add_argument('--output', default='team_preview_model.pt')
    args = parser.parse_args()
    train(args)


if __name__ == '__main__':
    main()
