# AI System

This document describes the AI components of ApogeeVGC: the deep learning models and (once implemented) the MCTS engine.

## Overview

The AI system has three main components:

1. **Team Preview Model** — chooses which 4 Pokemon to bring and which 2 to lead
2. **Battle Model** — evaluates battle states and suggests actions (move/switch) each turn
3. **MCTS Engine** — uses the battle model to search the game tree (TODO)

The DL models are trained in Python (PyTorch), exported to ONNX, and loaded in C# via `Microsoft.ML.OnnxRuntime` for inference.

## Data Pipeline

```
replay.pokemonshowdown.com
        │
        ▼
  Tools/ReplayScraper/scraper.py     Scrapes replay JSON files
        │
        ▼
  Tools/ReplayScraper/parser.py      Extracts structured turn-by-turn data
        │
        ▼
  parsed.jsonl                       Training data (state, action, outcome per turn)
        │
        ▼
  Tools/DLModel/train*.py            Trains models on GPU
        │
        ▼
  model.onnx                         Deployed in C# for inference
```

### Replay Scraper

**Location:** `Tools/ReplayScraper/scraper.py`

Two-phase approach:
1. Paginate through `search.json?format=<id>&before=<timestamp>` to collect all replay IDs
2. Fetch each replay's full JSON (including battle log) individually

Supports resumption — skips already-downloaded replays on restart.

```
python scraper.py --format gen9vgc2025regi --concurrency 10
```

### Replay Parser

**Location:** `Tools/ReplayScraper/parser.py`

Parses the Showdown battle log protocol into structured JSONL. Per replay it extracts:

- **Players**: names, Elo ratings before and after the game
- **Teams**: full 6-Pokemon team preview + which 4 were brought
- **Turn-by-turn state**: active Pokemon (species, HP%, status, boosts, tera), field conditions (weather, terrain, trick room, screens), and actions taken per slot
- **Revealed info**: moves, items, abilities, tera types observed per Pokemon
- **Winner**

The field state is snapshotted at each `|turn|` marker (before that turn's actions execute), ensuring the state represents the decision point.

```
python parser.py --format gen9vgc2025regi --min-rating 1300
```

## Team Preview Model

**Location:** `Tools/DLModel/team_preview_model.py`, `train_team_preview.py`

### Purpose

Given both players' 6-Pokemon teams, predict:
1. Which 4 of your 6 Pokemon to bring
2. Which 2 of those 4 to lead

### Architecture

```
Input:  species_ids [12] — my 6 + opponent's 6

        ┌──────────────────────┐
        │  Species Embedding   │  12 × 48-dim
        │  (shared weights)    │
        └──────────┬───────────┘
                   │ flatten → [576]
        ┌──────────▼───────────┐
        │       Trunk MLP      │  576 → 256 → 256 → 128
        │  (BN + ReLU + Drop)  │
        └──────────┬───────────┘
              ┌────┴────┐
        ┌─────▼─────┐ ┌─▼──────────┐
        │ Bring Head │ │ Lead Head  │
        │ 128→64→6   │ │ 128→64→6   │
        │ (sigmoid)  │ │ (sigmoid)  │
        └────────────┘ └────────────┘

Output: bring_scores [6], lead_scores [6]
```

### Training

- **Loss**: BCE on bring scores (all 6 slots) + BCE on lead scores (masked to brought Pokemon only)
- **Bring accuracy**: exact match of predicted top-4 vs actual top-4 (order-independent)
- **Lead accuracy**: exact match of predicted top-2 vs actual top-2 (among brought, order-independent)

### Inference

1. Embed your 6 + opponent's 6 species
2. Sort by bring score → top 4 are your team
3. Among those 4, sort by lead score → top 2 are your leads

```
python train_team_preview.py --epochs 30 --min-rating 1300
```

## Battle Model

**Location:** `Tools/DLModel/model.py`, `dataset.py`, `train.py`

### Purpose

Given the current battle state, predict:
1. **Value**: win probability (0–1)
2. **Policy A**: action probability distribution for active slot A
3. **Policy B**: action probability distribution for active slot B

### State Encoding

Each sample encodes the battle from one player's perspective. Two samples are generated per turn (one per player).

**Species IDs** — 8 slots embedded via shared embedding layer:
```
[my_a, my_b, opp_a, opp_b, my_bench_0, my_bench_1, opp_bench_0, opp_bench_1]
```

**Numeric features** — 200-dimensional vector:

| Offset | Size | Description |
|--------|------|-------------|
| 0–34 | 35 | My active slot A (HP, fainted, status, boosts, tera) |
| 35–69 | 35 | My active slot B |
| 70–104 | 35 | Opponent active slot A |
| 105–139 | 35 | Opponent active slot B |
| 140–149 | 10 | My bench slot 0 (HP, fainted, status, present) |
| 150–159 | 10 | My bench slot 1 |
| 160–169 | 10 | Opponent bench slot 0 |
| 170–179 | 10 | Opponent bench slot 1 |
| 180–199 | 20 | Field state + turn number |

Active Pokemon features (35 per slot):
- `[0]` HP as fraction [0,1]
- `[1]` is_fainted {0,1}
- `[2..8]` status one-hot (none/par/brn/slp/psn/tox/frz)
- `[9..13]` boosts (atk/def/spa/spd/spe) normalized by /6
- `[14]` is_tera {0,1}
- `[15..34]` tera_type one-hot (none + 19 types)

Field features (20):
- `[0..4]` weather one-hot (none/SunnyDay/RainDance/Sandstorm/Snow)
- `[5..9]` terrain one-hot (none/Electric/Grassy/Psychic/Misty Terrain)
- `[10]` trick_room
- `[11..18]` side conditions (my/opp × tailwind/reflect/light_screen/aurora_veil)
- `[19]` turn_number / 20

### Architecture

```
Input:  species_ids [8], numeric [200]

        ┌──────────────────────┐
        │  Species Embedding   │  8 × 32-dim
        └──────────┬───────────┘
                   │ flatten → [256]
                   │ concat with numeric → [456]
        ┌──────────▼───────────┐
        │       Trunk MLP      │  456 → 256 → 256 → 128
        │  (BN + ReLU + Drop)  │
        └──────────┬───────────┘
          ┌────────┼────────┐
     ┌────▼────┐ ┌─▼─────┐ ┌▼────────┐
     │  Value  │ │Pol. A │ │ Pol. B  │
     │128→64→1 │ │128→A  │ │ 128→A   │
     │(sigmoid)│ │(logits)│ │(logits) │
     └─────────┘ └───────┘ └─────────┘

Output: value [1], policy_a [num_actions], policy_b [num_actions]
```

### Action Vocabulary

Actions are encoded as string keys mapped to integer indices:
- `move:<MoveName>` — e.g., `move:Earthquake`, `move:Protect`
- `switch:<Species>` — e.g., `switch:Incineroar`
- `<none>` — no action (fainted, empty slot)
- `<cant>` — unable to act (flinch, sleep, etc.)
- `<pad>` — padding (ignored in loss)

The vocabulary is built from the training data and saved alongside the ONNX model.

### Training

- **Value loss**: Binary cross-entropy
- **Policy loss**: Cross-entropy (ignoring `<pad>` targets)
- **Total loss**: value_loss + policy_loss_a + policy_loss_b
- Early stopping on validation loss with patience

```
python train.py --epochs 30 --batch-size 512 --min-rating 1300
```

### ONNX Export

```
python export_onnx.py battle          # → battle_model.onnx + battle_model_vocab.json
python export_onnx.py team_preview    # → team_preview_model.onnx + team_preview_model_vocab.json
```

The vocab JSON is saved alongside the ONNX file so C# can map species/action names to embedding indices.

## MCTS Engine

**Location:** `ApogeeVGC/Mcts/` (TODO)

### Planned Architecture

The MCTS engine will use the battle model to guide search:

- **Value head** evaluates leaf nodes (replaces random rollouts)
- **Policy head** provides soft priors for UCB exploration (moves rated higher by the model are explored first, but no moves are hard-pruned)

Key challenges for VGC:
- **Imperfect information**: opponent's moves, items, bench are hidden. Use determinization (sample opponent states from metagame priors, updated via Bayesian inference as info is revealed)
- **Simultaneous moves**: both players choose at the same time. The tree branches on joint actions.

### Integration with C#

Load ONNX models via `Microsoft.ML.OnnxRuntime`:

```csharp
// Pseudocode
var session = new InferenceSession("battle_model.onnx");
var speciesIds = new DenseTensor<long>(new long[] { ... }, new[] { 1, 8 });
var numeric = new DenseTensor<float>(new float[] { ... }, new[] { 1, 200 });
var inputs = new List<NamedOnnxValue> {
    NamedOnnxValue.CreateFromTensor("species_ids", speciesIds),
    NamedOnnxValue.CreateFromTensor("numeric", numeric),
};
var results = session.Run(inputs);
float winProb = results[0].AsEnumerable<float>().First();
float[] policyA = results[1].AsEnumerable<float>().ToArray();
```

The encoding logic in `dataset.py` must be replicated exactly in C# — the numeric feature layout documented above defines the contract between training and inference.
