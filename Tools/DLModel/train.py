"""
Training script for BattleNet.

Usage:
  python train.py                                          # train with defaults
  python train.py --data ../ReplayScraper/data/gen9vgc2025regi/parsed.jsonl
  python train.py --min-rating 1300 --epochs 50
  python train.py --batch-size 1024 --lr 5e-4
"""

from __future__ import annotations

import argparse
import json
import random
import time
from pathlib import Path

import torch
import torch.nn as nn
from torch.utils.data import DataLoader

from dataset import VGCDataset, build_vocab
from model import BattleNet


def load_games(path: str, min_rating: int = 0) -> list[dict]:
    """Load parsed games, optionally filtering by minimum player rating."""
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


def train(args: argparse.Namespace) -> None:
    torch.manual_seed(args.seed)
    random.seed(args.seed)

    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    print(f'Device: {device}')

    # ── Vocab ──
    vocab_path = Path(args.vocab)
    if not vocab_path.exists():
        print(f'Building vocab from {args.data}...')
        vocab = build_vocab(args.data)
        vocab_path.parent.mkdir(parents=True, exist_ok=True)
        with open(vocab_path, 'w') as f:
            json.dump(vocab, f, indent=2)
        print(f'  Saved to {vocab_path}')
    else:
        with open(vocab_path) as f:
            vocab = json.load(f)

    print(f'Vocab: {vocab["num_species"]} species, '
          f'{vocab["num_actions"]} actions')

    # ── Data ──
    print(f'Loading games from {args.data}...')
    games = load_games(args.data, args.min_rating)
    print(f'  {len(games)} games loaded')
    if not games:
        print('No games found. Check --data path and --min-rating.')
        return

    random.shuffle(games)
    split = int(len(games) * (1 - args.val_split))
    train_games = games[:split]
    val_games = games[split:]
    print(f'  Split: {len(train_games)} train, {len(val_games)} val')

    print('Building train dataset...')
    t0 = time.time()
    train_ds = VGCDataset(train_games, vocab)
    print(f'  {len(train_ds):,} samples ({time.time() - t0:.1f}s)')

    print('Building val dataset...')
    t0 = time.time()
    val_ds = VGCDataset(val_games, vocab)
    print(f'  {len(val_ds):,} samples ({time.time() - t0:.1f}s)')

    train_loader = DataLoader(
        train_ds, batch_size=args.batch_size, shuffle=True,
        num_workers=0, pin_memory=(device.type == 'cuda'))
    val_loader = DataLoader(
        val_ds, batch_size=args.batch_size, shuffle=False,
        num_workers=0, pin_memory=(device.type == 'cuda'))

    # ── Model ──
    model = BattleNet(
        vocab['num_species'], vocab['num_actions'],
        args.embed_dim, args.hidden_dim,
    ).to(device)
    total_params = sum(p.numel() for p in model.parameters())
    print(f'Model: {total_params:,} parameters')

    optimizer = torch.optim.Adam(
        model.parameters(), lr=args.lr, weight_decay=1e-5)
    scheduler = torch.optim.lr_scheduler.CosineAnnealingLR(
        optimizer, args.epochs)

    value_loss_fn = nn.BCELoss()
    policy_loss_fn = nn.CrossEntropyLoss(ignore_index=0)  # ignore <pad>

    best_val_loss = float('inf')
    patience_counter = 0

    print(f'\nTraining for {args.epochs} epochs...\n')

    for epoch in range(args.epochs):
        # ── Train ──
        model.train()
        train_loss = 0.0
        train_vloss = 0.0
        train_ploss = 0.0
        n_batches = 0

        for batch in train_loader:
            sids, num, vtgt, pa_tgt, pb_tgt = [
                x.to(device) for x in batch]

            value, pol_a, pol_b = model(sids, num)

            v_loss = value_loss_fn(value, vtgt)
            p_loss_a = policy_loss_fn(pol_a, pa_tgt)
            p_loss_b = policy_loss_fn(pol_b, pb_tgt)
            loss = v_loss + p_loss_a + p_loss_b

            optimizer.zero_grad()
            loss.backward()
            torch.nn.utils.clip_grad_norm_(model.parameters(), 1.0)
            optimizer.step()

            train_loss += loss.item()
            train_vloss += v_loss.item()
            train_ploss += (p_loss_a.item() + p_loss_b.item()) / 2
            n_batches += 1

        scheduler.step()

        # ── Validate ──
        model.eval()
        val_loss = 0.0
        val_vloss = 0.0
        val_ploss = 0.0
        val_vacc = 0
        val_pacc_a = 0
        val_pacc_b = 0
        n_val_samples = 0
        n_val_policy_a = 0
        n_val_policy_b = 0
        n_val_batches = 0

        with torch.no_grad():
            for batch in val_loader:
                sids, num, vtgt, pa_tgt, pb_tgt = [
                    x.to(device) for x in batch]

                value, pol_a, pol_b = model(sids, num)

                v_loss = value_loss_fn(value, vtgt)
                p_loss_a = policy_loss_fn(pol_a, pa_tgt)
                p_loss_b = policy_loss_fn(pol_b, pb_tgt)
                loss = v_loss + p_loss_a + p_loss_b

                val_loss += loss.item()
                val_vloss += v_loss.item()
                val_ploss += (p_loss_a.item() + p_loss_b.item()) / 2
                n_val_batches += 1

                bs = vtgt.size(0)
                n_val_samples += bs
                val_vacc += ((value > 0.5).float() == vtgt).sum().item()

                mask_a = pa_tgt > 0
                if mask_a.any():
                    val_pacc_a += (
                        pol_a.argmax(1)[mask_a] == pa_tgt[mask_a]
                    ).sum().item()
                    n_val_policy_a += mask_a.sum().item()

                mask_b = pb_tgt > 0
                if mask_b.any():
                    val_pacc_b += (
                        pol_b.argmax(1)[mask_b] == pb_tgt[mask_b]
                    ).sum().item()
                    n_val_policy_b += mask_b.sum().item()

        # ── Metrics ──
        t_loss = train_loss / n_batches
        v_loss_avg = val_loss / n_val_batches
        v_vacc = val_vacc / n_val_samples if n_val_samples else 0
        v_pacc_a = val_pacc_a / n_val_policy_a if n_val_policy_a else 0
        v_pacc_b = val_pacc_b / n_val_policy_b if n_val_policy_b else 0
        lr = scheduler.get_last_lr()[0]

        print(
            f'Epoch {epoch + 1:2d}/{args.epochs} | '
            f'Train: {t_loss:.4f} '
            f'(v={train_vloss / n_batches:.4f} '
            f'p={train_ploss / n_batches:.4f}) | '
            f'Val: {v_loss_avg:.4f} '
            f'(v={val_vloss / n_val_batches:.4f} '
            f'p={val_ploss / n_val_batches:.4f}) | '
            f'Acc: v={v_vacc:.3f} pa={v_pacc_a:.3f} pb={v_pacc_b:.3f} | '
            f'LR: {lr:.6f}'
        )

        # ── Checkpointing ──
        if v_loss_avg < best_val_loss:
            best_val_loss = v_loss_avg
            patience_counter = 0
            torch.save({
                'model_state_dict': model.state_dict(),
                'vocab': vocab,
                'args': vars(args),
                'epoch': epoch + 1,
                'val_loss': v_loss_avg,
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
    parser = argparse.ArgumentParser(description='Train BattleNet')
    parser.add_argument(
        '--data',
        default='../ReplayScraper/data/gen9vgc2025regi/parsed.jsonl',
        help='Path to parsed JSONL data')
    parser.add_argument(
        '--vocab', default='vocab.json',
        help='Path to vocab JSON (auto-built if missing)')
    parser.add_argument(
        '--min-rating', type=int, default=0,
        help='Minimum player rating filter (0 = all)')
    parser.add_argument('--epochs', type=int, default=30)
    parser.add_argument('--batch-size', type=int, default=512)
    parser.add_argument('--lr', type=float, default=1e-3)
    parser.add_argument('--embed-dim', type=int, default=32)
    parser.add_argument('--hidden-dim', type=int, default=256)
    parser.add_argument('--val-split', type=float, default=0.2)
    parser.add_argument('--patience', type=int, default=5)
    parser.add_argument('--seed', type=int, default=42)
    parser.add_argument('--output', default='model.pt')
    args = parser.parse_args()
    train(args)


if __name__ == '__main__':
    main()
