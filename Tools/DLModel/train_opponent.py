"""
Training script for OpponentPredictionNet.

Trains a model to predict what the opponent will do given the current
battle state. Uses the same input encoding as BattleNet but targets
opponent actions instead of own actions/value.

Usage:
  python train_opponent.py
  python train_opponent.py --data ../ReplayScraper/data/gen9vgc2025regi/parsed.jsonl
  python train_opponent.py --min-rating 1300 --epochs 50
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

from opponent_dataset import OpponentPredictionDataset
from opponent_model import OpponentPredictionNet
from dataset import build_vocab
from format_spec import FORMAT_REGISTRY


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


def train(args: argparse.Namespace) -> None:
    fmt = FORMAT_REGISTRY[args.format]
    torch.manual_seed(args.seed)
    random.seed(args.seed)

    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    print(f'Device: {device}')

    if device.type == 'cuda':
        torch.backends.cudnn.benchmark = True
        torch.set_float32_matmul_precision('high')

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
        print(f'  Saved to {vocab_path}')
    else:
        with open(vocab_path) as f:
            vocab = json.load(f)

    print(f'Vocab: {vocab["num_species"]} species, '
          f'{vocab["num_actions"]} actions, '
          f'{vocab["num_moves"]} moves')

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
    train_ds = OpponentPredictionDataset(train_games, vocab, format_spec=fmt)
    print(f'  {len(train_ds):,} samples ({time.time() - t0:.1f}s)')

    print('Building val dataset...')
    t0 = time.time()
    val_ds = OpponentPredictionDataset(val_games, vocab, format_spec=fmt)
    print(f'  {len(val_ds):,} samples ({time.time() - t0:.1f}s)')

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
    model = OpponentPredictionNet(
        num_species=vocab['num_species'],
        num_actions=vocab['num_actions'],
        num_moves=vocab['num_moves'],
        num_abilities=vocab['num_abilities'],
        num_items=vocab['num_items'],
        num_tera_types=vocab['num_tera_types'],
        format_spec=fmt,
        embed_dim=args.embed_dim,
        feat_embed_dim=args.feat_embed_dim,
        pokemon_dim=args.pokemon_dim,
        hidden_dim=args.hidden_dim,
        num_trunk_layers=args.num_trunk_layers,
        trunk_dropout=args.trunk_dropout,
        head_dim=args.head_dim,
    ).to(device)
    total_params = sum(p.numel() for p in model.parameters())
    print(f'Model: {total_params:,} parameters')
    print(f'Format: {fmt.name} ({fmt.num_battle_slots} slots, '
          f'{fmt.numeric_dim}D numeric)')

    optimizer = torch.optim.Adam(
        model.parameters(), lr=args.lr, weight_decay=1e-5)
    scheduler = torch.optim.lr_scheduler.CosineAnnealingLR(
        optimizer, args.epochs)

    policy_loss_fn = nn.CrossEntropyLoss(ignore_index=0)

    best_val_loss = float('inf')
    patience_counter = 0

    print(f'\nTraining for {args.epochs} epochs...\n')

    for epoch in range(args.epochs):
        # ── Train ──
        model.train()
        train_loss = 0.0
        n_batches = 0

        for batch in train_loader:
            sids, mids, aids, iids, tids, num, opa_tgt, opb_tgt = [
                x.to(device, non_blocking=True) for x in batch]

            with autocast('cuda', enabled=use_amp):
                policies = model(sids, mids, aids, iids, tids, num)

            loss_a = policy_loss_fn(policies[0].float(), opa_tgt)
            loss = loss_a
            if fmt.num_leads >= 2:
                loss_b = policy_loss_fn(policies[1].float(), opb_tgt)
                loss = loss + loss_b

            optimizer.zero_grad()
            scaler.scale(loss).backward()
            scaler.unscale_(optimizer)
            torch.nn.utils.clip_grad_norm_(model.parameters(), 1.0)
            scaler.step(optimizer)
            scaler.update()

            train_loss += loss.item()
            n_batches += 1

        scheduler.step()

        # ── Validate ──
        model.eval()
        val_loss = 0.0
        val_pacc_a = 0
        val_pacc_b = 0
        val_top3_a = 0
        val_top3_b = 0
        n_val_policy_a = 0
        n_val_policy_b = 0
        n_val_batches = 0

        with torch.no_grad():
            for batch in val_loader:
                sids, mids, aids, iids, tids, num, opa_tgt, opb_tgt = [
                    x.to(device, non_blocking=True) for x in batch]

                with autocast('cuda', enabled=use_amp):
                    policies = model(sids, mids, aids, iids, tids, num)

                loss_a = policy_loss_fn(policies[0].float(), opa_tgt)
                loss = loss_a
                if fmt.num_leads >= 2:
                    loss_b = policy_loss_fn(policies[1].float(), opb_tgt)
                    loss = loss + loss_b

                val_loss += loss.item()
                n_val_batches += 1

                # Top-1 accuracy
                mask_a = opa_tgt > 0
                if mask_a.any():
                    preds_a = policies[0].argmax(1)
                    val_pacc_a += (preds_a[mask_a] == opa_tgt[mask_a]).sum().item()
                    # Top-3 accuracy
                    top3_a = policies[0].topk(3, dim=1).indices
                    val_top3_a += (top3_a[mask_a] == opa_tgt[mask_a].unsqueeze(1)).any(1).sum().item()
                    n_val_policy_a += mask_a.sum().item()

                if fmt.num_leads >= 2:
                    mask_b = opb_tgt > 0
                    if mask_b.any():
                        preds_b = policies[1].argmax(1)
                        val_pacc_b += (preds_b[mask_b] == opb_tgt[mask_b]).sum().item()
                        top3_b = policies[1].topk(3, dim=1).indices
                        val_top3_b += (top3_b[mask_b] == opb_tgt[mask_b].unsqueeze(1)).any(1).sum().item()
                        n_val_policy_b += mask_b.sum().item()

        # ── Metrics ──
        t_loss = train_loss / n_batches
        v_loss = val_loss / n_val_batches
        acc_a = val_pacc_a / n_val_policy_a if n_val_policy_a else 0
        acc_b = val_pacc_b / n_val_policy_b if n_val_policy_b else 0
        t3_a = val_top3_a / n_val_policy_a if n_val_policy_a else 0
        t3_b = val_top3_b / n_val_policy_b if n_val_policy_b else 0
        lr = scheduler.get_last_lr()[0]

        print(
            f'Epoch {epoch + 1:2d}/{args.epochs} | '
            f'Train: {t_loss:.4f} | Val: {v_loss:.4f} | '
            f'Top1: a={acc_a:.3f} b={acc_b:.3f} | '
            f'Top3: a={t3_a:.3f} b={t3_b:.3f} | '
            f'LR: {lr:.6f}'
        )

        # ── Checkpointing ──
        if v_loss < best_val_loss:
            best_val_loss = v_loss
            patience_counter = 0
            torch.save({
                'model_state_dict': model.state_dict(),
                'vocab': vocab,
                'args': vars(args),
                'format': fmt.to_dict(),
                'epoch': epoch + 1,
                'val_loss': v_loss,
                'model_type': 'opponent_prediction',
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
        description='Train OpponentPredictionNet')
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
    parser.add_argument('--epochs', type=int, default=50)
    parser.add_argument('--batch-size', type=int, default=16384)
    parser.add_argument('--lr', type=float, default=1e-3)
    parser.add_argument('--embed-dim', type=int, default=32)
    parser.add_argument('--feat-embed-dim', type=int, default=16)
    parser.add_argument('--pokemon-dim', type=int, default=48)
    parser.add_argument('--hidden-dim', type=int, default=256)
    parser.add_argument('--num-trunk-layers', type=int, default=2)
    parser.add_argument('--trunk-dropout', type=float, default=0.3)
    parser.add_argument('--head-dim', type=int, default=64)
    parser.add_argument('--val-split', type=float, default=0.2)
    parser.add_argument('--patience', type=int, default=10)
    parser.add_argument('--seed', type=int, default=42)
    parser.add_argument('--format', default='vgc',
                        choices=list(FORMAT_REGISTRY.keys()),
                        help='Battle format')
    parser.add_argument('--output', default='opponent_model.pt')
    args = parser.parse_args()
    train(args)


if __name__ == '__main__':
    main()
