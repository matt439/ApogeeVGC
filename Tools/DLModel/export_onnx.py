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
        vocab['num_species'],
        vocab['num_actions'],
        train_args['embed_dim'],
        train_args['hidden_dim'],
    )
    model.load_state_dict(checkpoint['model_state_dict'])
    model.eval()

    species_ids = torch.zeros(1, 8, dtype=torch.long)
    numeric = torch.zeros(1, 200, dtype=torch.float32)

    torch.onnx.export(
        model,
        (species_ids, numeric),
        output_path,
        input_names=['species_ids', 'numeric'],
        output_names=['value', 'policy_a', 'policy_b'],
        dynamic_axes={
            'species_ids': {0: 'batch'},
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
    print(f'  Species: {vocab["num_species"]}')
    print(f'  Actions: {vocab["num_actions"]}')
    print(f'  Epoch: {checkpoint.get("epoch", "?")}')
    print(f'  Val loss: {checkpoint.get("val_loss", "?"):.4f}')


def export_team_preview(checkpoint_path: str, output_path: str) -> None:
    checkpoint = torch.load(checkpoint_path, map_location='cpu',
                            weights_only=False)
    vocab = checkpoint['vocab']
    train_args = checkpoint['args']

    model = TeamPreviewNet(
        vocab['num_species'],
        train_args['embed_dim'],
        train_args['hidden_dim'],
    )
    model.load_state_dict(checkpoint['model_state_dict'])
    model.eval()

    species_ids = torch.zeros(1, 12, dtype=torch.long)

    torch.onnx.export(
        model,
        (species_ids,),
        output_path,
        input_names=['species_ids'],
        output_names=['bring_scores', 'lead_scores'],
        dynamic_axes={
            'species_ids': {0: 'batch'},
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
    print(f'  Species: {vocab["num_species"]}')
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
