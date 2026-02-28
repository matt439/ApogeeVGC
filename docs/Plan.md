# SIT746 Honours Thesis Plan

## Overview

This project investigates whether an AI system combining Monte Carlo Tree Search (MCTS) and deep learning can achieve competitive performance in Pokemon Video Game Championships (VGC). The AI will assist a human player during live ranked battles in Pokemon Scarlet, with performance evaluated against the ranked ladder population.

## Hypothesis

A simulation program designed for Pokemon battles under the current VGC rules, using deep learning and MCTS, can achieve a player rank 2 standard deviations above the mean.

**Null hypothesis:** The AI-assisted player's final rating is not significantly different from the population mean.

The ranked rating distribution's mean and standard deviation will need to be established from available ladder data or inferred from the player population.

## Literature Review

Key areas to cover:

- **MCTS + Deep Learning for games:** AlphaGo (Silver et al., 2016), AlphaZero (Silver et al., 2018) — foundational work combining MCTS with neural network evaluation and policy guidance.
- **Imperfect information game AI:** Pluribus (Brown & Sandholm, 2019) — superhuman poker AI handling hidden information and opponent modeling. Closest analogue to the VGC problem.
- **Information Set MCTS:** Cowling et al. (2012) — foundational paper for applying MCTS to imperfect information games via determinization and information set search.
- **Simultaneous Move MCTS:** Lanctot et al. (2014) — handling games where both players choose actions simultaneously, directly applicable to VGC's turn structure.
- **Existing Pokemon AI work:** Search Google Scholar for existing approaches (e.g., showdown bots, reinforcement learning applied to Pokemon battles). Position this work against simpler existing methods.

## Ethics

Although the AI is used as a decision-support tool by a human player (not autonomously playing), opponents on the ranked ladder are unaware they face an AI-assisted player. Confirm with supervisor whether low-risk ethics clearance is required.

## Stages

### 1. Create Pokemon Simulator

The Pokemon Showdown simulator is open source and highly accurate. However, it is written in JavaScript and has poor performance. The simulator was converted to C# targeting .NET 10, achieving significantly better performance for the high volume of simulations MCTS requires.

The simulator supports:
- VGC doubles format (Gen 9, Regulation I)
- Full move, ability, and item mechanics (687 moves, 674+ abilities, 251 items)
- Perspective-based state representation with proper hidden information handling
- Headless evaluation mode (no GUI overhead) for fast MCTS playouts

**Status: ~95% complete.**

### 2. Implement MCTS

VGC presents two key challenges for standard MCTS:

1. **Imperfect information** — the opponent's moves, items, bench Pokemon, and EVs are hidden. This requires **determinization**: sampling possible opponent states from a probability distribution and running MCTS on each sampled state. Alternatively, **Information Set MCTS (IS-MCTS)** operates on information sets rather than concrete game states.

2. **Simultaneous moves** — both players choose actions at the same time, so the game tree branches on joint actions rather than sequential ones.

The implementation will:
- Use the C# simulator to run playouts from the current game state
- Use the existing random player (`PlayerRandom`) as the default rollout policy
- Use a heuristic evaluation for non-terminal states (e.g., remaining HP ratio, Pokemon count advantage, field control)
- Later integrate the deep learning model to guide search (see Stage 6)

**Fallback plan:** If deep learning integration is not completed in time, pure MCTS with heuristic evaluation is a valid contribution. Scaling compute (high thread count EC2 instance) can compensate partially for the lack of a learned evaluation function.

### 3. Scrape Showdown Replays

https://replay.pokemonshowdown.com/ records previous battles. Create a program which scrapes and stores the battle replay strings.

- Target the correct VGC format matching the current Scarlet ranked ladder ruleset
- Store replays in a structured format for downstream parsing
- Aim to collect a large corpus (tens of thousands of replays minimum for DL training)

### 4. Analyse Replays

Parse the scraped replay strings into structured data:

- **(State, Action, Outcome) tuples** — the primary training data for the deep learning model. Each turn produces a battle state, the action chosen by each player, and the eventual game result (win/loss).
- **Metagame statistics** — Pokemon usage rates, common movesets, item frequencies, ability distributions, Tera type choices. These serve as **priors for opponent modeling**.
- **Lead patterns** — common opening Pokemon pairs and their win rates.
- **Conditional action probabilities** — given the visible game state, what actions do players tend to take?

The metagame statistics feed directly into the opponent modeling system: when the opponent's Pokemon details are hidden, the AI uses metagame priors (e.g., "73% of Incineroar carry Fake Out") and updates them via **Bayesian inference** as information is revealed during the battle.

### 5. Create Deep Learning Model

Train a neural network to provide both a **value function** (win probability estimate) and a **policy function** (action probability distribution) for guiding MCTS:

- **Input:** Encoded battle state derived from the simulator's `BattlePerspective` — Pokemon species, HP percentages, stat boosts, active conditions, field state, known and unknown opponent information.
- **Output:** Value head (scalar win probability) and policy head (probability distribution over legal actions).
- **Architecture:** To be determined through experimentation (feedforward network or transformer).
- **Training data:** Supervised learning on (state, action, outcome) tuples extracted from replay analysis.
- **Training infrastructure:** Rented GPU compute (minimum RTX 5090 equivalent) for model training.
- **Deployment:** Train in Python (PyTorch), export via ONNX for inference in C#.

**Self-play** is out of scope for this thesis but is noted as future work. Supervised learning on human replays provides a well-understood training pipeline with predictable results.

### 6. Integrate Deep Learning with MCTS

The deep learning model will be used as a **soft prior** to guide MCTS exploration, following the AlphaZero approach:

- The **policy head** output weights the UCB exploration term — moves the model rates highly are explored first, moves rated poorly are explored less, but **no moves are hard-pruned**. This avoids the risk of eliminating the best move due to model error.
- The **value head** evaluates leaf nodes, replacing or supplementing the heuristic evaluation and random rollouts.

This approach gives most of the computational benefit of reducing the effective search space while maintaining the safety of never completely eliminating a legal move from consideration.

**Inference infrastructure:** AWS EC2 instance with sufficient compute for real-time MCTS during live battles (must return a decision within the turn timer).

### 7. Create Battle State Quick-Input Interface

An interface is needed between Pokemon Scarlet running on a Switch and the AI. While full image recognition on the video output would be complex to implement, targeted OCR could supplement manual input (e.g., reading HP bars from a capture card).

The primary interface will be a program allowing rapid input of the battle state through keyboard and mouse shortcuts, using dense keybinds (similar to MMO hotkey layouts) to handle many different functions quickly.

Key considerations:
- Input speed must be fast enough to operate within the battle turn timer
- Needs testing to establish realistic input times per turn
- Semi-automated OCR for HP values could reduce manual input burden

### 8. Test Against Scarlet Ranked Ladder

Play ranked battles on the Scarlet ladder using the AI as a decision-support tool. The human player inputs the current battle state via the quick-input interface, the AI computes the optimal action, and the player executes it on the Switch.

- Use established meta teams from the replay analysis (no algorithmic team optimization — this reduces complexity and keeps focus on the AI's decision-making)
- Play a sufficient number of games to reach a steady-state rating
- Target a minimum of **~200 games** based on power analysis (see Statistics section)

### 9. Perform Statistics on Results

- **Win rate** with confidence intervals (binomial proportion confidence interval)
- **Rating trajectory** over time — plot rating progression to show convergence to steady state
- **Comparison against baselines:** random player (in simulator), pure MCTS without DL (in simulator), and the human player's unassisted rating (on ladder)
- **Statistical significance testing:** binomial test against 50% win rate null hypothesis; test whether final rating exceeds 2 SD above the mean
- **Power analysis:** A priori calculation of required sample size. For a binomial test with expected 60% win rate vs 50% null, α = 0.05, power = 0.80: approximately 200 games needed. For a smaller effect (55%), approximately 800 games would be needed, which may be impractical given manual input constraints.
- **Failure mode analysis** — categorise losses to identify systematic weaknesses (e.g., specific matchups, late-game scenarios, information gaps)
- **Decision time analysis** — record time per decision to verify the AI operates within turn timer constraints

## Timeline

| Stage | Description | Deadline |
|-------|-------------|----------|
| 1 | Pokemon Simulator | *~Complete* |
| 2 | MCTS Implementation | |
| 3 | Scrape Showdown Replays | |
| 4 | Analyse Replays | |
| 5 | Deep Learning Model | |
| 6 | Integrate DL with MCTS | |
| 7 | Quick-Input Interface | |
| 8 | Ranked Ladder Testing | |
| 9 | Statistical Analysis | |
| — | Thesis Writing | |
| — | **Submission Deadline** | |

*Fill in dates based on your submission deadline. Work backwards — leave at least 3-4 weeks for writing and revision.*

## Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| DL model doesn't train well | No learned evaluation | Fall back to pure MCTS + heuristic with high compute |
| Insufficient replays scraped | Poor training data | Supplement with self-play data from simulator |
| Quick-input too slow for timer | Can't complete live battles | Add OCR for HP bars; pre-populate known Pokemon data |
| Insufficient games for statistical significance | Weak conclusions | Focus on larger effect size; report confidence intervals regardless |
| Compute costs exceed budget | Can't run live inference | Reduce MCTS simulation count; rely more on DL policy |
