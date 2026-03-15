# Honours Thesis Progress Diary — Checkpoint 1

**Student:** Matthew Harding
**Project:** ApogeeVGC — Deep Learning for Pokemon VGC Battle AI
**Period:** October 2025 – 16 March 2026 (pre-semester + Semester Weeks 1–2)

---

## Project Overview

ApogeeVGC is a Pokemon VGC (Video Game Championships) battle AI that combines Monte Carlo Tree Search (MCTS) with deep learning models for in-battle evaluation and team preview selection. The system includes a from-scratch C# battle simulator, a Python ML pipeline (data scraping, training, evaluation), and ONNX-based inference for real-time play.

---

## Pre-Semester Work (October 2025 – February 2026)

### October 2025 — Battle Simulator Foundation (293 commits)

- Implemented core battle engine event system in C# (delegates, event methods, conditions)
- Built format/ruleset framework supporting VGC regulations A–I
- Established project structure (species, moves, abilities, items as strongly-typed enums)
- Added Showdown protocol logging for replay compatibility

**Evidence:** Git history shows systematic implementation of event handlers, species data, and battle mechanics.

### November 2025 — Player Architecture & GUI (305 commits)

- Designed IPlayer interface with PlayerRandom baseline agent
- Built MonoGame GUI with sprite rendering and battle animations
- Implemented perspective classes (player/opponent views) for information hiding
- Added multi-target damage animations and switch animation system
- Refactored battle state to use modular perspective classes

**Evidence:** GUI player mode functional with animated battle rendering.

### December 2025 — Data Layer & Species Completion (253 commits)

- Entered all Gen 9 species data (Bulbasaur through full National Dex)
- Implemented all ability and item effect handlers
- Added enum-based ID system replacing string lookups for performance
- Refactored event handlers for consistency and clarity

**Evidence:** Complete species/ability/item data coverage for Gen 9 formats.

### January 2026 — Move Implementation & Review (369 commits)

- Implemented effects for all competitive Gen 9 moves (~400+ moves)
- Systematic code review of every move (commits show alphabetical review passes)
- Implemented side conditions, terrain, weather, and field effects
- Fixed move interaction edge cases (Temper Flare, two-turn moves, recoil)

**Evidence:** Move review commits show alphabetical audit from AuraWheel through all moves.

### February 2026 — MCTS, Deep Learning Pipeline & Equivalence Testing (566 commits)

- **MCTS Implementation:** Full tree search with expansion, parallel search with thread-safe updates, configurable via MctsConfig
- **Deep Learning Integration:**
  - Built data pipeline: Showdown replay scraper and parser
  - Implemented BattleNet (value + policy heads) and TeamPreviewNet models
  - Added training pipeline with AMP, cosine scheduling, early stopping
  - ONNX export for C# inference
  - Added DL-Greedy and MCTS-DL evaluation modes
- **Showdown Live Assist:** WebSocket server for real-time battle recommendations overlaid on Pokemon Showdown
- **Equivalence Testing:** Began comparing C# simulator output against Pokemon Showdown's JavaScript engine
  - Bulk fixture generator for automated testing
  - Initial target: 100,000 passing battles

**Evidence:** 566 commits — highest monthly output. DL pipeline end-to-end functional. Live Assist mode operational.

---

## Semester Period (Weeks 1–2: 3–16 March 2026)

### Week 1 (3–9 March) — Training Framework & Experimentation (52 commits)

**Achievements:**
- Enhanced training configuration with label smoothing, value smoothing, loss weighting, and LR warmup
- Added AMP support and gradient clip options to battle training
- Added norm_type and residual connection options to BattleNet architecture
- Refined batch sizes and training hyperparameters for GPU efficiency
- Built Showdown Live Assist mode (shadow battle + WebSocket server for real-time overlay)

**Problems/Risks:** None blocking. Training convergence required iterating on hyperparameter ranges.

### Week 2 (10–16 March) — Equivalence Testing Milestone & Experiment Pipeline (148 commits)

**Achievements:**

1. **1,000,000 Passing Equivalence Tests**
   - Scaled batch testing from 100K to 1M battles — all passing
   - Fixed 98+ simulator correctness bugs (each individually documented in Docs/bugfixes-equivalence/)
   - Key fixes: Dancer ability ordering, Substitute/status interaction, Pressure PP deduction, Aurora Veil double-application, Psychic Terrain immunity, Encore return value semantics
   - Added xUnit test project for CI-compatible sim correctness testing
   - Supports gen9randomdoublesbattle, gen9randombattle, gen9vgc2024regh/i formats

2. **Experiment Pipeline Completion**
   - Ran full TeamPreviewNet experiment pipeline (hparam search, ablation, baselines, multi-seed, figures) across 3 rating tiers (all, 1200+, 1500+)
   - Ran full BattleNet experiment pipeline across 3 rating tiers
   - 80 figures generated (learning curves, calibration plots, ablation bars, hparam sensitivity, multi-seed distributions, baseline comparisons)
   - Cross-tier statistical comparison with Welch's t-test and Cohen's d

3. **Per-Regulation Model Support**
   - Refactored C# Driver to load models from regulation-specific directories
   - Created export_best.py script to select best-performing seed and export ONNX models
   - Each regulation now has its own model folder (Tools/DLModel/models/<regulation>/)

4. **Performance Optimisation**
   - Optimised GC pressure and hot paths in battle simulation
   - RndVsRndEvaluation achieves ~5,000 battles/second on local machine — this throughput directly enables MCTS, where each node expansion requires a full forward simulation (rollout) of the game
   - Added BenchmarkDotNet project for systematic performance measurement
   - Refactored replay parser to track decision points instead of turns

**Problems/Risks:**
- Equivalence testing uncovered many subtle edge cases in the simulator. Each fix risked introducing regressions — mitigated by running the full 1M test suite after each change.
- Rating tier analysis showed some metrics vary across tiers — need to determine which tier is most appropriate for production model selection.

---

## Key Metrics Summary

| Metric | Value |
|--------|-------|
| Total commits (Oct–Mar) | 2,026 |
| Documented bug fixes | 98+ (equivalence testing) |
| Equivalence tests passing | 1,000,000 / 1,000,000 |
| Parsed replay games | 304,750 (gen9vgc2025regi) |
| Experiment figures generated | 80 |
| Rating tiers evaluated | 3 (all, 1200+, 1500+) |
| Multi-seed runs per model | 5 seeds per tier |
| Hyperparameter trials | 15+ per model per tier |
| Documentation files | 224 |

---

## Experiment Results (gen9vgc2025regi, "all" tier)

### TeamPreviewNet — Multi-Seed Test Metrics
| Metric | Mean | Std |
|--------|------|-----|
| Bring Set Accuracy | 59.5% | 0.9% |
| Bring Overlap Accuracy | 88.8% | 0.3% |
| Lead Set Accuracy | 54.4% | 0.1% |
| Lead Overlap Accuracy | 75.9% | 0.1% |

### BattleNet — Multi-Seed Test Metrics
| Metric | Mean | Std |
|--------|------|-----|
| Value (Win Prediction) Accuracy | 72.2% | 0.2% |
| Policy A Top-1 Accuracy | 61.9% | 0.3% |
| Policy B Top-1 Accuracy | 61.2% | 0.3% |
| Policy A Top-3 Accuracy | 92.2% | 0.1% |
| Policy B Top-3 Accuracy | 92.1% | 0.1% |

---

## Milestone Tracker

| Milestone | Status | Date |
|-----------|--------|------|
| Battle simulator core engine | Done | Oct 2025 |
| All Gen 9 species/moves/abilities/items | Done | Jan 2026 |
| GUI with battle animations | Done | Nov 2025 |
| MCTS implementation | Done | Feb 2026 |
| Replay scraper & parser | Done | Feb 2026 |
| BattleNet & TeamPreviewNet training | Done | Feb 2026 |
| ONNX export & C# inference | Done | Feb 2026 |
| Showdown Live Assist mode | Done | Mar 2026 |
| Equivalence testing (1M passing) | Done | 15 Mar 2026 |
| Full experiment pipeline (all tiers) | Done | 13 Mar 2026 |
| Per-regulation model directories | Done | 16 Mar 2026 |
| Thesis writing | Not started | — |

---

## Next Steps

- Begin thesis writing (literature review, methodology chapters)
- Evaluate MCTS+DL vs baselines in controlled match simulations
- Investigate production tier selection (all vs 1200+ vs 1500+)
- Consider additional regulations if time permits
- Set up regular supervisor meetings

---

## Appendix: Evidence Artifacts

All evidence is available in the project repository:

- **Git history:** `git log --since="2025-10-01"` (2,026 commits)
- **Bug fix documentation:** `Docs/bugfixes-equivalence/` (98+ files)
- **Equivalence test workflow:** `Docs/equivalence-testing-workflow.md`
- **Experiment results:** `Tools/DLModel/results/gen9vgc2025regi/`
- **Generated figures:** `Tools/DLModel/results/gen9vgc2025regi/{preview,battle}/{all,1200+,1500+}/figures/`
- **Pipeline instructions:** `Docs/AI_Pipeline_Instructions.md`
- **Architecture docs:** `Docs/MAIN_BATTLE_IMPLEMENTATION_GUIDE.md`, `Docs/AI_System.md`
