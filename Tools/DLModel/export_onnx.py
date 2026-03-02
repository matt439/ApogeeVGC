"""
Export trained models to ONNX for inference in C#.

Usage:
  python export_onnx.py battle                                        # BattleNet
  python export_onnx.py team_preview                                  # TeamPreviewNet
  python export_onnx.py battle --checkpoint model.pt --output m.onnx  # custom paths
"""

from __future__ import annotations

import argparse
import json

import torch

from model import BattleNet
from team_preview_model import TeamPreviewNet


def export_battle(checkpoint_path: str, output_path: str) -> None:
    checkpoint = torch.load(checkpoint_path, map_location='cpu',
                            weights_only=False)
    vocab = checkpoint['vocab']
    train_args = checkpoint['args']

    model = BattleNet(
        num_species=vocab['num_species'],
        num_actions=vocab['num_actions'],
        num_moves=vocab['num_moves'],
        num_abilities=vocab['num_abilities'],
        num_items=vocab['num_items'],
        num_tera_types=vocab['num_tera_types'],
        embed_dim=train_args['embed_dim'],
        feat_embed_dim=train_args['feat_embed_dim'],
        pokemon_dim=train_args['pokemon_dim'],
        hidden_dim=train_args['hidden_dim'],
    )
    model.load_state_dict(checkpoint['model_state_dict'])
    model.eval()

    species_ids = torch.zeros(1, 8, dtype=torch.long)
    move_ids = torch.zeros(1, 8, 4, dtype=torch.long)
    ability_ids = torch.zeros(1, 8, dtype=torch.long)
    item_ids = torch.zeros(1, 8, dtype=torch.long)
    tera_ids = torch.zeros(1, 8, dtype=torch.long)
    numeric = torch.zeros(1, 200, dtype=torch.float32)

    torch.onnx.export(
        model,
        (species_ids, move_ids, ability_ids, item_ids, tera_ids, numeric),
        output_path,
        input_names=[
            'species_ids', 'move_ids', 'ability_ids', 'item_ids', 'tera_ids',
            'numeric',
        ],
        output_names=['value', 'policy_a', 'policy_b'],
        dynamic_axes={
            'species_ids': {0: 'batch'},
            'move_ids': {0: 'batch'},
            'ability_ids': {0: 'batch'},
            'item_ids': {0: 'batch'},
            'tera_ids': {0: 'batch'},
            'numeric': {0: 'batch'},
            'value': {0: 'batch'},
            'policy_a': {0: 'batch'},
            'policy_b': {0: 'batch'},
        },
        opset_version=17,
    )

    vocab_out = output_path.replace('.onnx', '_vocab.json')
    with open(vocab_out, 'w') as f:
        json.dump(vocab, f, indent=2)

    print(f'Exported BattleNet to {output_path}')
    print(f'Exported vocab to {vocab_out}')
    print(f'  Species: {vocab["num_species"]}, Actions: {vocab["num_actions"]}, '
          f'Moves: {vocab["num_moves"]}, Abilities: {vocab["num_abilities"]}, '
          f'Items: {vocab["num_items"]}, Tera types: {vocab["num_tera_types"]}')
    print(f'  Epoch: {checkpoint.get("epoch", "?")}')
    print(f'  Val loss: {checkpoint.get("val_loss", "?"):.4f}')


def export_team_preview(checkpoint_path: str, output_path: str) -> None:
    checkpoint = torch.load(checkpoint_path, map_location='cpu',
                            weights_only=False)
    vocab = checkpoint['vocab']
    train_args = checkpoint['args']

    model = TeamPreviewNet(
        num_species=vocab['num_species'],
        num_moves=vocab['num_moves'],
        num_abilities=vocab['num_abilities'],
        num_items=vocab['num_items'],
        num_tera_types=vocab['num_tera_types'],
        species_embed_dim=train_args['embed_dim'],
        feat_embed_dim=train_args['feat_embed_dim'],
        pokemon_dim=train_args['pokemon_dim'],
        hidden_dim=train_args['hidden_dim'],
    )
    model.load_state_dict(checkpoint['model_state_dict'])
    model.eval()

    species_ids = torch.zeros(1, 12, dtype=torch.long)
    move_ids = torch.zeros(1, 12, 4, dtype=torch.long)
    ability_ids = torch.zeros(1, 12, dtype=torch.long)
    item_ids = torch.zeros(1, 12, dtype=torch.long)
    tera_ids = torch.zeros(1, 12, dtype=torch.long)

    torch.onnx.export(
        model,
        (species_ids, move_ids, ability_ids, item_ids, tera_ids),
        output_path,
        input_names=[
            'species_ids', 'move_ids', 'ability_ids', 'item_ids', 'tera_ids',
        ],
        output_names=['bring_scores', 'lead_scores'],
        dynamic_axes={
            'species_ids': {0: 'batch'},
            'move_ids': {0: 'batch'},
            'ability_ids': {0: 'batch'},
            'item_ids': {0: 'batch'},
            'tera_ids': {0: 'batch'},
            'bring_scores': {0: 'batch'},
            'lead_scores': {0: 'batch'},
        },
        opset_version=17,
    )

    vocab_out = output_path.replace('.onnx', '_vocab.json')
    with open(vocab_out, 'w') as f:
        json.dump(vocab, f, indent=2)

    print(f'Exported TeamPreviewNet to {output_path}')
    print(f'Exported vocab to {vocab_out}')
    print(f'  Species: {vocab["num_species"]}, Moves: {vocab["num_moves"]}, '
          f'Abilities: {vocab["num_abilities"]}, Items: {vocab["num_items"]}, '
          f'Tera types: {vocab["num_tera_types"]}')
    print(f'  Epoch: {checkpoint.get("epoch", "?")}')
    print(f'  Val loss: {checkpoint.get("val_loss", "?"):.4f}')


def main():
    parser = argparse.ArgumentParser(description='Export models to ONNX')
    parser.add_argument('model_type', choices=['battle', 'team_preview'])
    parser.add_argument('--checkpoint', default=None)
    parser.add_argument('--output', default=None)
    args = parser.parse_args()

    if args.model_type == 'battle':
        cp = args.checkpoint or 'model.pt'
        out = args.output or 'battle_model.onnx'
        export_battle(cp, out)
    else:
        cp = args.checkpoint or 'team_preview_model.pt'
        out = args.output or 'team_preview_model.onnx'
        export_team_preview(cp, out)


if __name__ == '__main__':
    main()
