"""
Training script for TeamPreviewNet.

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
from torch.utils.data import DataLoader

from dataset import build_vocab
from team_preview_dataset import TeamPreviewDataset
from team_preview_model import TeamPreviewNet


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


def compute_accuracy(
    scores: torch.Tensor,
    targets: torch.Tensor,
    k: int,
) -> float:
    """Top-k set accuracy: what fraction of samples have the predicted
    top-k set exactly matching the target top-k set."""
    pred_topk = scores.topk(k, dim=1).indices  # [B, k]
    true_topk = targets.topk(k, dim=1).indices  # [B, k]

    # Sort both so order doesn't matter
    pred_sorted = pred_topk.sort(dim=1).values
    true_sorted = true_topk.sort(dim=1).values

    match = (pred_sorted == true_sorted).all(dim=1)
    return match.float().mean().item()


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
    else:
        with open(vocab_path) as f:
            vocab = json.load(f)

    print(f'Vocab: {vocab["num_species"]} species')

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
    train_ds = TeamPreviewDataset(train_games, vocab)
    val_ds = TeamPreviewDataset(val_games, vocab)
    print(f'  {len(train_ds):,} train, {len(val_ds):,} val '
          f'({time.time() - t0:.1f}s)')

    train_loader = DataLoader(
        train_ds, batch_size=args.batch_size, shuffle=True,
        num_workers=0, pin_memory=(device.type == 'cuda'))
    val_loader = DataLoader(
        val_ds, batch_size=args.batch_size, shuffle=False,
        num_workers=0, pin_memory=(device.type == 'cuda'))

    # ── Model ──
    model = TeamPreviewNet(
        vocab['num_species'], args.embed_dim, args.hidden_dim,
    ).to(device)
    total_params = sum(p.numel() for p in model.parameters())
    print(f'Model: {total_params:,} parameters')

    optimizer = torch.optim.Adam(
        model.parameters(), lr=args.lr, weight_decay=1e-5)
    scheduler = torch.optim.lr_scheduler.CosineAnnealingLR(
        optimizer, args.epochs)

    bring_loss_fn = nn.BCELoss()
    lead_loss_fn = nn.BCELoss(reduction='none')

    best_val_loss = float('inf')
    patience_counter = 0

    print(f'\nTraining for {args.epochs} epochs...\n')

    for epoch in range(args.epochs):
        # ── Train ──
        model.train()
        t_loss = 0.0
        t_bloss = 0.0
        t_lloss = 0.0
        n_batches = 0

        for sids, bring_tgt, lead_tgt, val_tgt in train_loader:
            sids = sids.to(device)
            bring_tgt = bring_tgt.to(device)
            lead_tgt = lead_tgt.to(device)

            bring_pred, lead_pred = model(sids)

            b_loss = bring_loss_fn(bring_pred, bring_tgt)

            # Lead loss only for brought Pokemon (bring_tgt == 1)
            l_loss_raw = lead_loss_fn(lead_pred, lead_tgt)
            l_mask = bring_tgt  # only compute lead loss where brought
            l_loss = (l_loss_raw * l_mask).sum() / (l_mask.sum() + 1e-8)

            loss = b_loss + l_loss

            optimizer.zero_grad()
            loss.backward()
            torch.nn.utils.clip_grad_norm_(model.parameters(), 1.0)
            optimizer.step()

            t_loss += loss.item()
            t_bloss += b_loss.item()
            t_lloss += l_loss.item()
            n_batches += 1

        scheduler.step()

        # ── Validate ──
        model.eval()
        v_loss = 0.0
        v_bloss = 0.0
        v_lloss = 0.0
        bring_acc_sum = 0.0
        lead_acc_sum = 0.0
        n_vbatches = 0

        with torch.no_grad():
            for sids, bring_tgt, lead_tgt, val_tgt in val_loader:
                sids = sids.to(device)
                bring_tgt = bring_tgt.to(device)
                lead_tgt = lead_tgt.to(device)

                bring_pred, lead_pred = model(sids)

                b_loss = bring_loss_fn(bring_pred, bring_tgt)
                l_loss_raw = lead_loss_fn(lead_pred, lead_tgt)
                l_mask = bring_tgt
                l_loss = (l_loss_raw * l_mask).sum() / (l_mask.sum() + 1e-8)
                loss = b_loss + l_loss

                v_loss += loss.item()
                v_bloss += b_loss.item()
                v_lloss += l_loss.item()
                n_vbatches += 1

                # Bring accuracy: predicted top-4 matches actual top-4
                bring_acc_sum += compute_accuracy(bring_pred, bring_tgt, 4)

                # Lead accuracy: among brought, predicted top-2 matches leads
                # Mask non-brought to -inf for lead scoring
                lead_masked = lead_pred.clone()
                lead_masked[bring_tgt < 0.5] = -1e9
                lead_acc_sum += compute_accuracy(lead_masked, lead_tgt, 2)

        avg_t = t_loss / n_batches
        avg_v = v_loss / n_vbatches
        bring_acc = bring_acc_sum / n_vbatches
        lead_acc = lead_acc_sum / n_vbatches
        lr = scheduler.get_last_lr()[0]

        print(
            f'Epoch {epoch + 1:2d}/{args.epochs} | '
            f'Train: {avg_t:.4f} '
            f'(b={t_bloss / n_batches:.4f} l={t_lloss / n_batches:.4f}) | '
            f'Val: {avg_v:.4f} '
            f'(b={v_bloss / n_vbatches:.4f} l={v_lloss / n_vbatches:.4f}) | '
            f'Acc: bring={bring_acc:.3f} lead={lead_acc:.3f} | '
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
    parser.add_argument('--hidden-dim', type=int, default=256)
    parser.add_argument('--val-split', type=float, default=0.2)
    parser.add_argument('--patience', type=int, default=5)
    parser.add_argument('--seed', type=int, default=42)
    parser.add_argument('--output', default='team_preview_model.pt')
    args = parser.parse_args()
    train(args)


if __name__ == '__main__':
    main()
