# ApogeeVGC Deep Learning Model Architecture

**Matthew Harding, 222580598, SIT746, 2026-T1**

This document describes the neural network architectures, input/output specifications, ground truth definitions, and design justifications for the two models in the Apogee system: **TeamPreviewNet** (pre-battle team selection) and **BattleNet** (in-battle evaluation and action selection). Both are designed for the VGC doubles format: 6-Pokemon team sheets, bring 4, lead 2.

---

## 1. TeamPreviewNet

### 1.1 Purpose

TeamPreviewNet selects the optimal **team preview configuration** — which 4 of 6 Pokemon to bring to battle, and which 2 of those 4 to lead — given both players' full team sheets.

### 1.2 Input Specification

The model receives both players' team sheet information as categorical features. All 12 Pokemon (6 own + 6 opponent) are encoded identically.

| Tensor | Shape | Dtype | Description |
|--------|-------|-------|-------------|
| `species_ids` | `[batch, 12]` | int64 | Species vocabulary index per Pokemon. Slots 0–5 are the player's team; slots 6–11 are the opponent's team. |
| `move_ids` | `[batch, 12, 4]` | int64 | Up to 4 move vocabulary indices per Pokemon. Padded with 0 for unknown/missing moves. |
| `ability_ids` | `[batch, 12]` | int64 | Ability vocabulary index per Pokemon. |
| `item_ids` | `[batch, 12]` | int64 | Held item vocabulary index per Pokemon. |
| `tera_ids` | `[batch, 12]` | int64 | Tera type vocabulary index per Pokemon. |

All vocabulary indices use `padding_idx=0` (zero vector for unknown/missing values) and index 1 for explicitly unknown tokens (`<unknown>`).

**Feature availability and train/inference distribution mismatch.** In training data, per-Pokemon features (moves, ability, item, tera) come from *revealed* information — what was observed across the entire game. The player's own team is always fully revealed. For the opponent, features are populated only for information that was actually observed during battle: moves that were used, abilities that triggered, items that activated, and tera types that were chosen. Opponent Pokemon that were never brought to battle have all features at the padding index (0), as do features that were never revealed (e.g., an opponent's fourth move if only three were used).

This creates a **distribution mismatch** with inference. At team preview time, the battle has not yet started — the opponent's moves, ability, item, and tera type are genuinely unknown for all 6 Pokemon, so all opponent feature slots are 0. During training, however, some opponent features are populated from post-hoc revealed data that the player did not have when making the team preview decision.

The mismatch is partially mitigated by the natural sparsity of revealed data: opponent Pokemon that were not brought (typically 2 of 6) already have all-zero features in training, and even brought Pokemon often have incomplete reveals (e.g., only 1–2 of 4 moves observed). The model therefore learns to function with partial or absent opponent features, falling back to species identity as the primary signal — which is also the only information genuinely available at team preview.

Crucially, while opponent features are absent at inference, they are **not unconstrained**. Each species has a narrow set of competitively viable moves, typically one or two legal abilities, and a small pool of commonly held items. In competitive VGC, movesets are highly concentrated: most species have a single dominant set used in the vast majority of games. This means the species embedding alone is a strong proxy for the missing features — knowing the opponent has Incineroar already implies Intimidate, Fake Out, and a defensive item with high probability. The species embedding can learn to encode these expected distributions implicitly, making the absent feature slots largely redundant for species with low set diversity. The mismatch matters most for species with genuinely diverse sets (e.g., Pokemon that can viably run physical or special builds), but these are a minority in any given metagame.

An alternative approach would be to **zero out all opponent features during training**, exactly matching the inference distribution. This eliminates the mismatch but discards potentially useful signal: training with revealed features lets the model learn species–feature correlations (e.g., that Incineroar typically carries Intimidate and Assault Vest), which may implicitly inform its species embeddings even when those features are absent at inference. Whether this trade-off helps or hurts accuracy is an empirical question suitable for ablation.

### 1.3 Output Specification

| Tensor | Shape | Dtype | Description |
|--------|-------|-------|-------------|
| `config_logits` | `[batch, 90]` | float32 | Raw logits over all 90 valid team preview configurations. |

At inference, softmax is applied to obtain a probability distribution. The argmax configuration is selected.

### 1.4 Configuration Enumeration

A VGC team preview configuration is a joint choice of:
1. **Bring set**: which 4 of 6 Pokemon to bring → C(6, 4) = 15 possibilities
2. **Lead pair**: which 2 of the 4 brought Pokemon to lead → C(4, 2) = 6 possibilities

Total: **15 × 6 = 90 configurations**.

Configurations are enumerated in lexicographic order using `itertools.combinations`. This enumeration is deterministic, so the Python training code and C# inference code produce identical index-to-configuration mappings from the same format specification.

Each configuration decomposes into three index tuples: `(bring, lead, bench)`, where `bring ⊂ {0..5}` with `|bring| = 4`, `lead ⊂ bring` with `|lead| = 2`, and `bench = bring \ lead`.

### 1.5 Ground Truth

The ground truth label for each sample is a single integer in `[0, 89]` — the configuration index corresponding to what the player actually did in the replay:

- **Bring set**: determined from the `team_brought` field in the parsed replay (which 4 Pokemon appeared in battle).
- **Lead pair**: determined from the first turn's `active` slots (which 2 Pokemon were in the active positions at turn 1).

Samples where the actual decision does not map to a valid configuration (e.g., the replay data is incomplete or the player forfeited before bringing a full team) are discarded.

**Training perspective.** By default, only the *winning* player's perspective is used (`winners_only=True`). The model performs behavioural cloning — it learns to replicate human team selection decisions. Training on losing players' choices teaches the model to imitate decisions that led to losses, diluting the signal from successful play. The `all_games` option (both perspectives) is available for experimental comparison.

### 1.6 Loss Function

**Cross-entropy loss** over the 90-class classification target.

This is a natural fit: the ground truth is a single discrete choice from a fixed set of mutually exclusive configurations. Cross-entropy directly maximises the log-probability assigned to the correct configuration.

### 1.7 Architecture

```
species_ids ──→ species_embed (dim=48) ──┐
move_ids ─────→ move_embed (dim=16) ─────┤  (sum over 4 moves)
ability_ids ──→ ability_embed (dim=16) ──┤
item_ids ─────→ item_embed (dim=16) ─────┤
tera_ids ─────→ tera_embed (dim=16) ─────┘
                                         │
                            concat → [batch, 12, 112]
                                         │
                         pokemon_encoder (Linear → ReLU)
                                         │
                              [batch, 12, pokemon_dim]
                                         │
                                  flatten → [batch, 12 × pokemon_dim]
                                         │
                         trunk (Linear → BN → ReLU → Dropout) × N
                                         │
                              [batch, hidden_dim / 2]
                                         │
                        config_head (Linear → ReLU → Linear)
                                         │
                              [batch, 90] config logits
```

#### Embedding Layer

Each Pokemon is represented by five categorical features, each mapped to a learned embedding vector:

| Feature | Embedding Dim | Rationale |
|---------|--------------|-----------|
| Species | 48 | Species is the highest-cardinality and most informative feature (hundreds of species). A larger embedding captures the rich variation in base stats, typing, and role. |
| Moves | 16 (×4, summed) | Moves are numerous but individually less discriminative than species. The 4 move embeddings are **summed** into a single vector — this bag-of-moves representation is order-invariant, reflecting that moveset composition matters but move ordering within a slot does not. |
| Ability | 16 | Single categorical feature per Pokemon. Moderate cardinality. |
| Item | 16 | Single categorical feature per Pokemon. Moderate cardinality. |
| Tera type | 16 | 19 types + pad/unknown. Low cardinality but strategically significant. |

All embeddings use `padding_idx=0`, which maps unknown/missing features to the zero vector. This means the model naturally handles partial information: if an opponent's moves are unknown, those embeddings contribute nothing, and the model relies on species identity alone.

**Why embeddings rather than one-hot encoding?** Pokemon species number in the hundreds, and moves in the thousands. One-hot encoding would produce extremely sparse, high-dimensional input. Learned embeddings compress this into dense, low-dimensional vectors where semantically similar entities (e.g., Pokemon with similar roles, or moves with similar effects) can cluster together in embedding space.

**Why sum moves rather than concatenate?** Concatenation would impose an artificial ordering on the 4 move slots and quadruple the per-Pokemon dimension. Summation treats the moveset as an unordered bag, which better matches the domain: a Pokemon's strategic role depends on *which* moves it has, not the order they are listed. Summation also keeps the per-Pokemon representation compact.

#### Per-Pokemon Encoder

A single shared `Linear(raw_dim, pokemon_dim) → ReLU` layer projects each Pokemon's concatenated embedding (species + moves + ability + item + tera = 112 dimensions at default settings) into a fixed-size representation. This encoder is **shared across all 12 slots** (weight-tied), which:

1. **Reduces parameters** — 12 separate encoders would multiply the embedding-layer parameter count by 12.
2. **Enables generalisation** — the same Pokemon species produces the same encoding regardless of which team slot it occupies, so the model can transfer knowledge about a species seen in one slot to another.

#### Trunk

The encoded representations of all 12 Pokemon are **flattened** into a single vector (`12 × pokemon_dim`) and passed through a multi-layer MLP trunk:

```
Linear(12 × pokemon_dim, hidden_dim) → BatchNorm → ReLU → Dropout
Linear(hidden_dim, hidden_dim)       → BatchNorm → ReLU → Dropout
Linear(hidden_dim, hidden_dim / 2)   → BatchNorm → ReLU → Dropout(reduced)
```

**Why flatten rather than use attention or set-based aggregation?** The team preview decision is fundamentally about interactions between specific Pokemon on both sides. Flattening preserves positional information (my slot 0 vs opponent slot 3), which matters because the model must reason about matchups between specific pairs. A set-based approach (e.g., DeepSets) would discard this structure. While attention mechanisms (e.g., a Transformer) could theoretically learn these interactions, the fixed input size of 12 Pokemon makes the quadratic cost unnecessary — the MLP trunk has sufficient capacity to learn the relevant cross-Pokemon interactions, and the combinatorial explosion is manageable at this scale.

**BatchNorm** stabilises training by normalising intermediate activations, enabling higher learning rates and faster convergence. **Dropout** (0.3, reduced to 0.2 in the final layer) provides regularisation to prevent overfitting on the training set's specific team compositions.

#### Output Head

```
Linear(hidden_dim / 2, head_dim) → ReLU → Linear(head_dim, 90)
```

Produces 90 raw logits, one per configuration.

### 1.8 Key Design Decision: Joint Configuration Classification

The most important architectural decision is modelling team preview as **classification over joint configurations** rather than independent per-slot predictions.

**Alternative considered: independent per-slot sigmoid outputs.** One natural approach is to produce 6 "bring" probabilities and 6 "lead" probabilities independently. However, this factored formulation cannot capture dependencies between slots:

- **Lead–bench synergy.** In VGC, the lead pair is chosen based on what backup options are on the bench. Leading Tornadus + Urshifu may be strong only if Rillaboom is on the bench to switch in. Independent lead predictions cannot condition on the bench composition.
- **Bring–bring dependencies.** Pokemon often function in pairs or cores. Bringing Kyogre without Tornadus (a common rain core) may be suboptimal, but independent per-slot predictions cannot express "bring Kyogre **if and only if** Tornadus is also brought."
- **Mutual exclusivity.** If slots 0 and 1 are both predicted as leads with 60% probability, an independent model gives no guidance on which to actually lead. The joint formulation produces a single coherent decision.

**Why 90 classes is tractable.** With only 90 configurations, the classification approach is computationally cheap (a single softmax over 90 logits). Cross-entropy loss naturally handles the mutual exclusivity between configurations. The softmax output also provides a calibrated probability distribution, enabling downstream use (e.g., sampling or temperature-based exploration).

**Why not enumerate all possible orderings?** The 90 configurations treat both the bring set and lead pair as **unordered sets**, not sequences. Leading Tornadus in slot A and Urshifu in slot B is the same configuration as the reverse. This reduces the configuration space and matches the game semantics — VGC does not distinguish between the two active slots at team preview time.

---

## 2. BattleNet

### 2.1 Purpose

BattleNet evaluates the current battle state and recommends actions for the active Pokemon. It produces:
1. A **value estimate** — the probability of winning from the current state.
2. **Policy distributions** — probability distributions over legal actions for each active Pokemon slot.

These outputs are used by the Monte Carlo Tree Search (MCTS) engine: the value estimate provides a state evaluation for leaf nodes, and the policy distributions provide move priors to guide tree exploration.

### 2.2 Input Specification

The model receives per-Pokemon categorical features (same as TeamPreviewNet) plus a numeric feature vector encoding the full battle state.

#### Categorical Inputs

| Tensor | Shape | Dtype | Description |
|--------|-------|-------|-------------|
| `species_ids` | `[batch, 8]` | int64 | Species index per Pokemon across 8 slots. |
| `move_ids` | `[batch, 8, 4]` | int64 | Up to 4 moves per Pokemon. |
| `ability_ids` | `[batch, 8]` | int64 | Ability per Pokemon. |
| `item_ids` | `[batch, 8]` | int64 | Held item per Pokemon. |
| `tera_ids` | `[batch, 8]` | int64 | Tera type per Pokemon. |

The 8 slots are arranged as:

| Slot Index | Role |
|-----------|------|
| 0 | My active slot A |
| 1 | My active slot B |
| 2 | Opponent active slot A |
| 3 | Opponent active slot B |
| 4 | My bench slot 0 |
| 5 | My bench slot 1 |
| 6 | Opponent bench slot 0 |
| 7 | Opponent bench slot 1 |

**Information asymmetry.** Own-team features are filled from end-of-game revealed data (the player always knows their own team). Opponent features are filled *progressively*: only moves and tera types that have been observed in prior turns. Opponent abilities and items remain at the padding index (0) since per-turn revelation cannot be reliably tracked from parsed replay actions. This mirrors the actual information available to a player during a battle.

#### Numeric Input

| Tensor | Shape | Dtype | Description |
|--------|-------|-------|-------------|
| `numeric` | `[batch, 244]` | float32 | Encoded battle state features. |

The 244-dimensional numeric vector encodes:

| Offset | Dim | Content |
|--------|-----|---------|
| 0–45 | 46 | My active A features |
| 46–91 | 46 | My active B features |
| 92–137 | 46 | Opponent active A features |
| 138–183 | 46 | Opponent active B features |
| 184–193 | 10 | My bench 0 features |
| 194–203 | 10 | My bench 1 features |
| 204–213 | 10 | Opponent bench 0 features |
| 214–223 | 10 | Opponent bench 1 features |
| 224–243 | 20 | Field + context features |

**Active Pokemon features (46 per slot):**

| Offset | Feature | Encoding |
|--------|---------|----------|
| 0 | HP | Fraction in [0, 1] (current HP / max HP × 100) |
| 1 | Fainted | Binary {0, 1} |
| 2–8 | Status condition | One-hot [7]: none, paralysis, burn, sleep, poison, toxic, freeze |
| 9–13 | Stat boosts | 5 values (atk/def/spa/spd/spe), each normalised by /6 to [−1, 1] |
| 14 | Terastallised | Binary {0, 1} |
| 15–34 | Tera type | One-hot [20]: none + 19 types |
| 35 | Substitute | Binary {0, 1} — behind a substitute |
| 36 | Confusion | Binary {0, 1} — confused |
| 37 | Taunt | Binary {0, 1} — taunted (can't use status moves) |
| 38 | Encore | Binary {0, 1} — encored (locked into one move) |
| 39 | Disable | Binary {0, 1} — one move disabled |
| 40 | Yawn | Binary {0, 1} — will fall asleep next turn |
| 41 | Leech Seed | Binary {0, 1} — seeded (HP drained each turn) |
| 42 | Perish Song | Binary {0, 1} — perish song active (must switch or faint) |
| 43 | Protected | Binary {0, 1} — used a protection move this turn |
| 44 | Torment | Binary {0, 1} — can't use same move consecutively |
| 45 | Imprison | Binary {0, 1} — opponent's shared moves are blocked |

**Bench Pokemon features (10 per slot):**

| Offset | Feature | Encoding |
|--------|---------|----------|
| 0 | HP | Fraction in [0, 1] |
| 1 | Fainted | Binary {0, 1} |
| 2–8 | Status condition | One-hot [7] |
| 9 | Present | Binary {0, 1} — whether this bench slot is occupied |

Bench Pokemon have fewer features because they cannot have stat boosts or be terastallised while on the bench.

**Field features (20):**

| Offset | Feature | Encoding |
|--------|---------|----------|
| 0–4 | Weather | One-hot [5]: none, Sun, Rain, Sand, Snow |
| 5–9 | Terrain | One-hot [5]: none, Electric, Grassy, Psychic, Misty |
| 10 | Trick Room | Binary {0, 1} |
| 11–18 | Side conditions | 8 binary values: my/opp × Tailwind, Reflect, Light Screen, Aurora Veil |
| 19 | Turn number | Normalised: turn / 20 |

**Why one-hot encode categorical state features instead of embedding them?** Status conditions, weather, and terrain have very low cardinality (5–7 values). One-hot encoding fully captures these without any information loss. Learned embeddings would add unnecessary parameters for negligible benefit. In contrast, species/moves/items have hundreds or thousands of values, making embeddings essential.

**Why normalise turn number by /20?** This scales the turn counter to a small range. Most VGC games end well within 20 turns, so the normalised value stays in roughly [0, 1]. This prevents the raw turn number from dominating other features during training.

### 2.3 Output Specification

| Tensor | Shape | Dtype | Description |
|--------|-------|-------|-------------|
| `value` | `[batch]` | float32 | Win probability, sigmoid-activated, in [0, 1]. |
| `policy_a` | `[batch, num_actions]` | float32 | Raw action logits for active slot A. |
| `policy_b` | `[batch, num_actions]` | float32 | Raw action logits for active slot B. |

At inference, policy logits are masked to legal actions and then softmax-normalised to obtain action probabilities.

**Action vocabulary.** Actions are encoded as strings of the form `move:<MoveName>` or `switch:<SpeciesName>`, mapped to integer indices via the shared vocabulary. Special tokens include `<pad>` (index 0, ignored in loss), `<none>` (no action available), and `<cant>` (forced action, e.g., Struggle or recharging).

### 2.4 Ground Truth

Each training sample is one decision point (turn) from one player's perspective.

**Value target:** Binary — 1.0 if this player ultimately won the game, 0.0 if they lost. This is the same value regardless of which turn the sample comes from; every turn in a won game has target 1.0. While this is a simplification (early-turn positions in a won game may have been losing at that moment), it provides a consistent signal that the model learns to smooth into a continuous win-probability estimate.

**Policy targets:** The action index corresponding to what the player actually chose at that turn. For each active slot, the action is either a move or a switch extracted from the replay's decision point data.

**Training perspective.** By default, BattleNet trains on **both perspectives** (`all_games`), unlike TeamPreviewNet's `winners_only` default. The rationale: the value head must predict win probability, which requires seeing both winning (target=1.0) and losing (target=0.0) examples. Training only on winners would make every value target 1.0, rendering the value head unable to distinguish favourable from unfavourable positions.

### 2.5 Loss Function

The total loss is a sum of three components:

```
loss = value_loss + policy_a_loss + policy_b_loss
```

| Component | Function | Description |
|-----------|----------|-------------|
| Value loss | `BCELoss` | Binary cross-entropy between the sigmoid value output and the binary win/loss target. |
| Policy A loss | `CrossEntropyLoss(ignore_index=0)` | Cross-entropy between slot A's action logits and the ground-truth action index. Padding targets (index 0) are ignored. |
| Policy B loss | `CrossEntropyLoss(ignore_index=0)` | Same as above for slot B. |

**Why BCE for value?** The value head outputs a single probability (win chance). BCE is the standard loss for binary probability estimation, directly optimising the model's predicted win probability toward the observed outcome.

**Why cross-entropy for policy?** Each active slot's action is a single discrete choice from the action vocabulary. Cross-entropy is the natural loss for categorical distributions and maximises the log-probability of the correct action.

**Why equal weighting?** Value and policy losses are summed with equal weight (both implicitly weighted at 1.0). This follows the approach used in AlphaZero-style architectures where the value and policy objectives are equally important. The value head guides MCTS node evaluation while the policy head guides exploration — neither should dominate training.

**Ignoring padding.** Policy targets use `ignore_index=0` to skip the `<pad>` token. This handles turns where a slot has no valid action (e.g., a fainted Pokemon that hasn't been replaced yet).

### 2.6 Architecture

```
species_ids ──→ species_embed (dim=32) ──┐
move_ids ─────→ move_embed (dim=16) ─────┤  (sum over 4 moves)
ability_ids ──→ ability_embed (dim=16) ──┤
item_ids ─────→ item_embed (dim=16) ─────┤
tera_ids ─────→ tera_embed (dim=16) ─────┘
                                         │
                            concat → [batch, 8, 96]
                                         │
                         pokemon_encoder (Linear → ReLU)
                                         │
                              [batch, 8, pokemon_dim]
                                         │
                                  flatten → [batch, 8 × pokemon_dim]
                                         │
                         concat with numeric → [batch, 8 × pokemon_dim + 244]
                                         │
                         trunk (Linear → BN → ReLU → Dropout) × N
                                         │
                              [batch, hidden_dim / 2]
                                         │
             ┌──────────────────────┬─────┴─────────────────────────────┐
             │                      │                                   │
        value_head            policy_head (slot A)              policy_head (slot B)
   (Linear → ReLU → Linear)  (concat trunk + enc[0])           (concat trunk + enc[1])
             │                (Linear → ReLU → Linear)          (Linear → ReLU → Linear)
             │                      │                                   │
      [batch] sigmoid        [batch, num_actions]               [batch, num_actions]
         value                  policy A logits                   policy B logits
```

#### Embedding and Per-Pokemon Encoder

Identical design to TeamPreviewNet (shared-weight encoder across all 8 slots), with the species embedding dimension reduced to 32. The smaller embedding reflects that battle-state reasoning relies more on the numeric features (HP, boosts, field conditions) than on species identity alone, compared to team preview where species composition is the primary signal.

#### Trunk

The trunk receives the concatenation of:
1. **Flattened Pokemon encodings** — `8 × pokemon_dim` dimensions capturing species, moves, abilities, items, and tera types.
2. **Numeric features** — 244 dimensions encoding HP, statuses, boosts, volatile conditions, field conditions, and turn number.

This concatenation gives the trunk simultaneous access to *what* is on the field (categorical) and *what state it's in* (numeric).

The trunk architecture matches TeamPreviewNet: stacked `Linear → BatchNorm → ReLU → Dropout` blocks with a dimension reduction in the final layer.

#### Value Head

```
Linear(hidden_dim / 2, head_dim) → ReLU → Linear(head_dim, 1) → Sigmoid
```

Produces a scalar win probability in [0, 1]. The sigmoid activation ensures the output is a valid probability.

**Why a single scalar rather than a categorical distribution (e.g., win/draw/loss)?** VGC games cannot end in a draw — one player always wins. A single sigmoid output is the simplest and most direct representation for binary outcome prediction.

#### Policy Head: Slot-Conditioned Design

A **single shared policy head** is used for both active slots, conditioned on the acting Pokemon's encoding:

```
input = concat(trunk_output, pokemon_encoder[slot_index])
Linear(hidden_dim / 2 + pokemon_dim, head_dim) → ReLU → Linear(head_dim, num_actions)
```

For slot A, the head receives `[trunk_output, enc[0]]`. For slot B, it receives `[trunk_output, enc[1]]`.

**Why a single shared head rather than two separate heads?**

- **Parameter efficiency.** Two heads would double the policy parameters. Since both active Pokemon face the same action space (moves + switches), a shared head can learn general action-selection behaviour.
- **Slot conditioning.** Concatenating the acting Pokemon's encoding gives the head direct access to *who* is acting, enabling slot-specific decisions. The trunk output provides the global battle context, and the slot encoding provides the local actor context.
- **Transfer learning.** A shared head means knowledge about action selection transfers between slots. If the model learns that "Protect is good when at low HP" for slot A, this knowledge automatically applies to slot B.

**Why not share the policy head with the value head?** The value and policy tasks have fundamentally different output structures (scalar vs. categorical distribution) and serve different purposes (state evaluation vs. action selection). Separate heads allow each to specialise, while the shared trunk captures the common battle-state representation that both tasks need.

### 2.7 Action Masking at Inference

The policy outputs are raw logits over the full action vocabulary. At inference time in C#, a **masked softmax** is applied: illegal actions are set to negative infinity before softmax, ensuring only legal actions receive nonzero probability. This is critical because:

- The set of legal moves varies per turn (e.g., move PP depletion, Encore, Choice item lock).
- The set of legal switches depends on which bench Pokemon are alive and not trapped.
- The model is trained on all actions in the vocabulary but only needs to rank legal ones at inference.

---

## 3. Shared Design Principles

### 3.1 Learned Embeddings for High-Cardinality Categoricals

Both models use learned embedding tables for species, moves, abilities, items, and tera types rather than hand-crafted features. This avoids:

- **Manual feature engineering** — no need to encode base stats, type matchup tables, or move damage calculations. The model discovers relevant patterns from data.
- **Incomplete domain knowledge** — hand-crafted features inevitably miss interactions. Embeddings can capture latent relationships (e.g., that Incineroar and Rillaboom share a defensive-support role) that are difficult to articulate as explicit features.
- **Maintenance burden** — new Pokemon, moves, or items in future generations require only vocabulary expansion, not feature redesign.

### 3.2 Shared-Weight Per-Pokemon Encoder

Both models use a single weight-tied encoder applied identically to every Pokemon slot. This reflects the domain invariance: a Pikachu's encoding should not depend on whether it appears in team slot 2 or opponent slot 5. Positional information (whose team, active vs. bench) is conveyed by the slot's position in the flattened vector, not by slot-specific encoder weights.

### 3.3 ONNX Export for Cross-Platform Inference

Both models are exported to ONNX format for inference in the C# game client. The ONNX interface (tensor names, shapes, and dtypes) is identical between V1 and V2 model variants, ensuring the C# code does not need to be modified when the Python-side architecture is changed for experimentation. This separation of concerns allows rapid iteration on model architecture without touching the inference integration.

### 3.4 Vocabulary System

A shared vocabulary maps string identifiers (species names, move names, etc.) to integer indices. The vocabulary is built once from the training data and exported alongside the ONNX model. Key properties:

- **Index 0 is padding** — maps to the zero vector via `padding_idx=0`. Used for missing/unknown features.
- **Index 1 is `<unknown>`** — a learned embedding for out-of-vocabulary tokens encountered at inference. This handles new Pokemon or moves not seen during training.
- **Deterministic ordering** — vocabulary entries are sorted alphabetically, ensuring reproducibility across runs.

---

## 4. Summary

| Aspect | TeamPreviewNet | BattleNet |
|--------|---------------|-----------|
| **Task** | Select bring-4 + lead-2 from team sheet | Evaluate state + recommend actions |
| **Input slots** | 12 (6 own + 6 opponent team sheet) | 8 (2+2 active + 2+2 bench) |
| **Categorical inputs** | species, moves, ability, item, tera | species, moves, ability, item, tera |
| **Numeric input** | None | 244-dim battle state vector |
| **Output** | 90-class config logits | value (scalar) + 2 policy heads (action logits) |
| **Ground truth** | Configuration index from replay | Win/loss outcome + chosen actions |
| **Loss** | CrossEntropyLoss | BCELoss (value) + CrossEntropyLoss × 2 (policy) |
| **Training perspective** | Winners only (default) | Both players (default) |
| **Key design choice** | Joint 90-class config over factored per-slot | Single shared slot-conditioned policy head |
| **Species embed dim** | 48 | 32 |
| **Inference output** | Softmax → argmax config | Sigmoid value + masked softmax policies |
