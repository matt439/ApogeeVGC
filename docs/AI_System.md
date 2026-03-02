# AI System

This document describes the AI components of ApogeeVGC: the deep learning models, the MCTS engine, and the information model.

## Overview

The AI system has four main components:

1. **Team Preview Model** — chooses which 4 Pokemon to bring and which 2 to lead
2. **Battle Model** — evaluates battle states and suggests actions (move/switch) each turn
3. **MCTS Engine** — uses the battle model to search the game tree
4. **Information Model** — formalises what is known and unknown at each phase of battle

### System Integration

The three battle systems are not independent — they form a loop where each feeds into the others:

```
 Information Model
   ├─ Deduction engine → hard constraints on opponent state
   ├─ Revealed-info tracking → extra features for DL input
   │
   ▼
 Determinize: sample K plausible opponent states
   │
   ▼  (for each determinization)
 MCTS iteration:
   ├─ SELECT:   PUCT using DL policy priors (P) + visit counts (N) + avg value (Q)
   ├─ EXPAND:   generate legal children at leaf
   ├─ EVALUATE: DL value head scores the leaf
   └─ BACKPROP: update Q and N up the tree
   │
   ▼
 Aggregate visit counts across K trees → choose action
```

**DL Model** feeds into two MCTS phases:
- **Selection** — the policy heads provide the prior P(s,a) in the PUCT formula, biasing search toward promising moves first
- **Evaluation** — the value head provides the leaf score, replacing random rollouts. This is the signal that gets backpropagated up the tree

The model is called once per leaf expansion and provides both signals simultaneously.

**Information Model** operates upstream of and around MCTS:
- **Determinization** (pre-search) — the deduction engine eliminates impossible opponent states (e.g., "this Pokemon definitely has Choice Scarf" becomes a hard constraint), and revealed-info tracking narrows what remains uncertain. This produces higher-quality determinizations for MCTS to search over
- **State encoding** (feeds into DL) — revealed and deduced information becomes input features to the model (move known/unknown flags, confirmed items, etc.), which improves both the policy priors and value estimates the model produces

The information model does not directly plug into PUCT selection — it improves MCTS indirectly by constraining the search space (fewer/better determinizations) and enriching the DL model's inputs (better features → better priors and values).

**Team preview** uses DL only (no MCTS or information model) — the team preview model directly predicts bring/lead selections from the 12 species.

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

**Experimentation:** See [`Docs/Experimentation.md`](Experimentation.md) for the full evaluation pipeline (hyperparameter search, ablation studies, baselines, multi-seed evaluation, and figure generation).

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

**Location:** `ApogeeVGC/Mcts/`

### How DL and MCTS Work Together

The DL model is "fast intuition" — given a state it instantly returns a win probability and action suggestions. MCTS is "slow deliberation" — it uses that intuition to systematically search the game tree and find moves that are better than what the network alone would suggest.

Each turn, when the MCTS player must choose:

```
1. Observe battle state (our perspective)
2. Determinize — sample K plausible opponent hidden states
3. For each determinization, run N iterations of MCTS:
     a. SELECT  — walk tree using PUCT (UCB + policy prior)
     b. EXPAND  — at a leaf, generate legal children
     c. EVALUATE — run the DL model on the leaf state
     d. BACKPROP — propagate value up the path
4. Aggregate visit counts across all K determinizations
5. Return the action pair with the highest visit count
```

The model is called **once per leaf expansion** and provides both signals:

| Model Output | MCTS Role | Replaces |
|---|---|---|
| `value` | Leaf evaluation score | Random rollouts |
| `policy_a` | Prior on slot A edges | Uniform priors |
| `policy_b` | Prior on slot B edges | Uniform priors |

### Selection (PUCT)

The standard AlphaZero formula selects which edge to explore:

```
PUCT(s, a) = Q(s,a) + c_puct * P(s,a) * sqrt(N(s)) / (1 + N(s,a))
```

- **Q(s,a)** = average value from simulations through this action
- **P(s,a)** = prior from the policy network (softmaxed over legal actions only)
- **N(s)** / **N(s,a)** = visit counts for the node / edge
- **c_puct** = exploration constant (tunable, typically 1.0–2.5)

The policy prior makes search efficient — moves the network thinks are strong get explored first, but nothing is hard-pruned. Given enough iterations, the search can override the network's initial assessment.

### Turn Structure in the Tree

A normal turn is straightforward: we pick `(action_a, action_b)` for our two active slots, the opponent picks theirs, the turn resolves, and we reach the next state. The branching factor for our side is roughly `|legal_a| × |legal_b|` ≈ 100–200 combinations. The policy prior keeps the effective branching much smaller.

The opponent's actions are handled by running the model from the opponent's perspective (the model is trained symmetrically — each turn produces one sample per player). This can be used as a prior to sample likely opponent actions during search.

**Mid-turn forced switches** are the real complication. These occur when:
- A Pokemon faints → that player must choose a replacement before the turn finishes
- U-turn / Volt Switch / Flip Turn → the user chooses who to switch in
- Eject Button, Red Card → items force a switch

These create extra decision points *inside* turn resolution. The simulator already models this (it sends `SwitchRequest` mid-turn), so the tree accommodates them as additional nodes — same PUCT machinery, different legal action set (switches only, no moves).

### Current Implementation

The current MCTS runs single-tree search under full observability — the opponent's full state is visible. This is a reasonable starting point for OTS (Open Team Sheets) mode, where the main unknowns are stat spreads and turn-by-turn decisions. Against a random opponent, MCTS with policy priors achieves ~67% win rate.

### Imperfect Information (Future)

The opponent's hidden info includes unrevealed moves, items, abilities, bench Pokemon species/sets, and tera type. The classical approach is ISMCTS (Information Set MCTS) via determinization:

1. **Maintain a belief state** — probability distribution over opponent sets, starting from metagame priors (usage stats)
2. **Update beliefs** — as info is revealed during battle (opponent uses a move, reveals an item, teras), narrow the distribution via Bayesian update
3. **Sample K determinizations** — draw K complete opponent states from the belief distribution
4. **Run MCTS independently** on each determinization (each is a perfect-information game)
5. **Aggregate** — average visit counts across determinizations to choose an action robust across plausible opponent states

However, ISMCTS determinization may not scale well for VGC due to the enormous hidden state space (moves × abilities × items × tera types × EVs across multiple Pokemon). The planned approach is a hybrid architecture — see the Information Model section below.

### Performance Considerations

Each MCTS iteration calls the model once at leaf expansion. With N iterations × K determinizations, inference count adds up. Mitigations:
- **Batch inference** — collect multiple leaves before running the model
- **Cache evaluations** — reuse results for identical states across determinizations
- **Limit search depth** — most useful signal is in the first 2–3 turns of lookahead
- **GPU inference** — OnnxRuntime supports CUDA for batched inference

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

## Information Model

**Full reference:** [`Docs/information_model.md`](information_model.md)

### Summary

The information model formalises what each player knows, can observe, and can deduce at each phase of a VGC battle. It covers both Open Team Sheets (OTS) and Closed Team Sheets (CTS) formats.

**Team preview** — Both players always see all 12 species. Under OTS, moves, abilities, items, and tera types are also visible. Under CTS, only species are known. EVs, IVs, natures, and the bring/lead selection are always hidden.

**In battle** — Active species, HP%, status, boosts, field conditions, and tera state are always visible. Under CTS, moves are revealed one at a time as used, abilities when triggered, items when consumed/activated, and tera type when terastallised. Certain abilities (Trace, Frisk, Forewarn) and moves (Knock Off, Trick, Skill Swap) passively reveal opponent information.

### Logical Deduction Engine

A key insight is that certain game events allow a player to deduce hidden information with certainty — but only when all alternative explanations can be ruled out. For example:

- **Choice Scarf**: A Pokemon outspeeds yours when even max Speed investment can't explain it — but only after ruling out Tailwind, speed boosts, and speed-boosting abilities
- **Bright Powder**: A 100% accuracy move misses — but only if there are no evasion boosts, no Sand Veil/Snow Cloak in active weather, and no other evasion sources
- **Not Choice-locked**: A Pokemon uses two different moves across turns — but only if it didn't switch out and back in between

The full document catalogues these deductions with their required preconditions across items, abilities, speed tiers, damage ranges, and moves.

### Hybrid Architecture (Planned)

Rather than relying solely on ISMCTS determinization (which struggles with VGC's enormous hidden state space), the planned approach combines:

1. **Logical deduction engine** — deterministic rules that lock in information when preconditions are met (e.g., "this Pokemon has Choice Scarf" becomes a fact, not a probability)
2. **Neural network estimation** — the model handles probabilistic inference for things that can't be deduced with certainty (likely remaining moves, probable EV spreads, expected opponent behaviour)
3. **Revealed-information tracking** — maintain what has been observed per opponent Pokemon (moves seen, ability triggered, item revealed, tera type used) and feed this as features to the model

This hybrid gives the AI capabilities that pure MCTS+DL cannot deliver — the deduction engine provides hard logical constraints that narrow the uncertainty space before the neural network estimates the rest.

### Priority for Implementation

1. Restrict opponent perspective to realistic observability (currently full-information)
2. Add revealed-information tracking (moves seen, items revealed, etc.)
3. Encode observation state as model features (move known/unknown flags, etc.)
4. Retrain model on realistic partial-information states
5. Build logical deduction engine for deterministic inferences
6. Integrate deduction outputs as additional model features

## Evaluation: Player Variants

To measure the contribution of each component, we test several player variants. Each adds exactly one element over the previous, enabling clean ablation.

### Variants (weakest → strongest)

1. **Random** — picks uniformly from legal actions. Baseline floor.
2. **Policy Network Only** — runs the DL model once per turn, picks the highest-probability legal action per slot. No search. Isolates the value of learned intuition.
3. **One-ply Greedy (value head)** — enumerates all legal `(action_a, action_b)` pairs, simulates each (with a sampled opponent action), evaluates the resulting state with the value head, picks the pair with the highest win probability. Search depth of 1, no tree.
4. **MCTS with Uniform Priors** — full tree search but all legal actions get equal prior (no policy network guidance). Isolates the contribution of search itself.
5. **MCTS with Policy Priors** — the complete system: PUCT with DL-provided priors and value evaluation. Expected to be the strongest.

### What Each Comparison Shows

| Comparison | Question Answered |
|---|---|
| 2 vs 1 | Does the model learn anything useful? |
| 3 vs 2 | Does one-step lookahead improve on raw policy? |
| 5 vs 4 | Does the policy prior improve search efficiency? |
| 5 vs 2 | How much does search add on top of the network? |
| 5 vs 3 | Does deeper search beat greedy? |

Additionally, varying search budget (N iterations) for variants 4 and 5 produces a strength-vs-compute curve.
