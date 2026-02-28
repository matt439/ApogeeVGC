"""
Export trained BattleNet to ONNX for inference in C#.

Usage:
  python export_onnx.py                               # defaults
  python export_onnx.py --checkpoint model.pt --output model.onnx
"""

from __future__ import annotations

import argparse
import json

import torch

from model import BattleNet


def export(checkpoint_path: str, output_path: str) -> None:
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

    # Dummy inputs
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

    # Also save vocab alongside for C# to load
    vocab_out = output_path.replace('.onnx', '_vocab.json')
    with open(vocab_out, 'w') as f:
        json.dump(vocab, f, indent=2)

    print(f'Exported ONNX model to {output_path}')
    print(f'Exported vocab to {vocab_out}')
    print(f'  Species: {vocab["num_species"]}')
    print(f'  Actions: {vocab["num_actions"]}')
    print(f'  Epoch: {checkpoint.get("epoch", "?")}')
    print(f'  Val loss: {checkpoint.get("val_loss", "?"):.4f}')


def main():
    parser = argparse.ArgumentParser(description='Export BattleNet to ONNX')
    parser.add_argument('--checkpoint', default='model.pt')
    parser.add_argument('--output', default='model.onnx')
    args = parser.parse_args()
    export(args.checkpoint, args.output)


if __name__ == '__main__':
    main()
