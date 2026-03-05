# Burndown — ApogeeVGC Honours Thesis

**Deadline:** 29 May 2026 (~12.5 weeks from 3 March 2026)

This is the master task list. Each item has a status, estimated effort, and phase. Work roughly in phase order, but thesis writing overlaps with everything from Phase 3 onward.

**Status key:** `[x]` done, `[-]` in progress, `[ ]` not started

---

## Phase 0 — Already Complete

These are done. Listed for sanity and so you can see how much is behind you.

- [x] C# Pokemon simulator (~95% of mechanics)
- [x] MCTS engine with PUCT, policy priors, value evaluation
- [x] Replay scraper (scraper.py) — 136k gen9vgc2025regi replays collected
- [x] Replay parser (parser.py) — structured JSONL output with turn-by-turn state
- [x] BattleNet V1 + V2 architecture (model.py)
- [x] TeamPreviewNet V1 + V2 architecture (team_preview_model.py)
- [x] Dataset classes (dataset.py, team_preview_dataset.py)
- [x] Training scripts (train.py, train_team_preview.py)
- [x] ONNX export pipeline (export_onnx.py)
- [x] C# ONNX inference integration (ModelInference.cs, StateEncoder.cs, ActionMapper.cs)
- [x] Experiment framework — both TeamPreview and BattleNet (experiments/)
- [x] Information model documentation (information_model.md)
- [x] BattleInfoTracker for revealed-info tracking in C#
- [x] Literature review topics and references catalogued
- [x] Thesis plan and hypothesis defined

**You have built the entire system.** What remains is running experiments at scale, evaluating the system, and writing it up.

---

## Phase 1 — GPU Training (Weeks 1–3)

**Goal:** Run the full experiment pipelines on rented GPU to get all the numbers for the thesis.

### 1.1 vast.ai Setup
- [ ] Rent RTX 5090 instance on vast.ai
- [ ] Set up environment: Python, PyTorch (CUDA), dependencies from requirements.txt
- [ ] Upload gen9vgc2025regi parsed data to the instance
- [ ] Verify training runs on GPU (quick smoke test: 3 trials, 5 epochs)

**Effort:** Half a day. **Risk:** Low.

### 1.2 TeamPreview Full Experiment Pipeline
- [-] Hyperparameter search — 100 Optuna trials (~40 done locally, restart or continue on GPU)
- [ ] Ablation study — 5 feature configs × best hparams
- [ ] Baselines — random + most-popular
- [ ] Multi-seed evaluation — 5 seeds with best config
- [ ] Figure generation — all 6 thesis-quality plots

```bash
python -m experiments.preview_run_all --regulation gen9vgc2025regi --n-trials 100 --min-rating 1300
```

**Effort:** ~6–12 hours wall-clock on GPU (mostly unattended). **Risk:** Low — framework is built and tested.

### 1.3 BattleNet Full Experiment Pipeline
- [-] Hyperparameter search — 100 Optuna trials (~40 done locally)
- [ ] Ablation study — 5 feature configs × best hparams
- [ ] Baselines — random + most-popular
- [ ] Multi-seed evaluation — 5 seeds with best config
- [ ] Figure generation — all 6 thesis-quality plots

```bash
python -m experiments.battle_run_all --regulation gen9vgc2025regi --n-trials 100 --min-rating 1300
```

**Effort:** ~12–24 hours wall-clock (BattleNet has larger dataset — 2 samples/turn). **Risk:** Low.

### 1.4 Export Final Models
- [ ] Export best TeamPreview model to ONNX (from multiseed best or best hparam)
- [ ] Export best BattleNet model to ONNX
- [ ] Copy ONNX files + vocab into C# project
- [ ] Smoke-test inference in C# with new models

**Effort:** 1–2 hours. **Risk:** Low — export pipeline exists.

### 1.5 Download & Archive Results
- [ ] Download all results/ from vast.ai before terminating instance
- [ ] Verify all figures, summaries, and metrics are intact
- [ ] Terminate vast.ai instance to stop billing

---

## Phase 2 — Player Variant Evaluation (Weeks 2–4)

**Goal:** Run the ablation study described in AI_System.md § Evaluation: Player Variants. This is a core thesis contribution — measuring each component's value.

### 2.1 Implement Evaluation Harness
- [ ] Create a headless evaluation script that plays N games between two player types
- [ ] Record win rate, confidence intervals, average game length, decision time
- [ ] Support all 5 player variants:
  1. Random
  2. Policy Network Only (DL model, argmax per slot, no search)
  3. One-ply Greedy (enumerate legal pairs, value-head eval, pick best)
  4. MCTS with Uniform Priors (no policy guidance)
  5. MCTS with Policy Priors (full system)

**Effort:** 1–2 days (most logic exists in PlayerRandom, PlayerMcts already). **Risk:** Medium — need to implement Policy-Only and Greedy players.

### 2.2 Run Round-Robin or Ladder
- [ ] Each variant vs Random (1000+ games each) — establishes absolute strength
- [ ] Full system (variant 5) vs each other variant (500+ games each) — pairwise comparison
- [ ] Vary MCTS budget (N=50, 100, 200, 400, 800 iterations) for strength-vs-compute curve
- [ ] Record and tabulate all results

**Effort:** Several hours of compute (can run overnight). **Risk:** Low — simulator is fast in headless mode.

### 2.3 Generate Figures
- [ ] Win rate bar chart across variants
- [ ] Strength-vs-compute curve (iterations vs win rate)
- [ ] Decision time vs iteration budget

**Effort:** Half a day. **Risk:** Low.

---

## Phase 3 — Quick-Input Interface (Weeks 3–5)

**Goal:** Build the bridge between Pokemon Scarlet on Switch and the AI. Enables live ladder testing.

### 3.1 Design Input Scheme
- [ ] Map all battle actions to keyboard shortcuts (species selection, move selection, switch targets)
- [ ] Design the flow: team preview input → per-turn state input → AI recommends action
- [ ] Decide minimum viable input: what *must* the human enter each turn?
  - Active species (maybe auto-tracked after team preview)
  - HP% for all visible Pokemon
  - Status conditions, boosts, field state
  - Opponent's revealed moves/items/abilities (for info model)

### 3.2 Implement Interface
- [ ] Console-based or lightweight GUI (Spectre.Console already in deps — use it)
- [ ] Team preview phase: input both teams → model recommends bring/lead
- [ ] Battle phase: input current state → MCTS runs → display recommended action
- [ ] Track game state across turns (auto-fill what hasn't changed)

**Effort:** 3–5 days. **Risk:** Medium — UX/speed is the concern, not the AI logic.

### 3.3 Test Input Speed
- [ ] Practice entering battle states against AI/friend battles
- [ ] Measure time per turn input — must fit within Scarlet's 45-second turn timer (with margin)
- [ ] Iterate on shortcuts if too slow

**Effort:** 1–2 days. **Risk:** Medium — if too slow, may need OCR assist for HP bars.

---

## Phase 4 — Ranked Ladder Testing (Weeks 5–8)

**Goal:** Collect the data that tests the thesis hypothesis.

### 4.1 Preparation
- [ ] Select a meta team from replay analysis (don't build a custom team — reduces confounds)
- [ ] Establish the human player's unassisted baseline rating (play ~30 games without AI)
- [ ] Set up recording/logging: per-game result, rating, AI recommendation vs actual action taken

### 4.2 Play Ranked Games
- [ ] Target ~200 games minimum (power analysis says 200 for 60% win rate detection)
- [ ] Log every game: starting rating, ending rating, win/loss, number of turns
- [ ] Play in focused sessions (fatigue affects input accuracy)

**Effort:** 200 games × ~15 min each = ~50 hours of play over 3 weeks. **Risk:** High — this is the most time-consuming phase and depends on input interface working well.

### 4.3 Handle Edge Cases During Play
- [ ] Note games where AI recommendation was overridden and why
- [ ] Note games where input error occurred (wrong HP%, missed status)
- [ ] Categorise losses (bad matchup, AI error, input error, timer pressure)

---

## Phase 5 — Statistical Analysis (Weeks 8–9)

**Goal:** Rigorously analyse ladder results and simulation results.

### 5.1 Ladder Statistics
- [ ] Win rate with 95% confidence interval (binomial proportion)
- [ ] Rating trajectory plot (rating over time, annotate convergence)
- [ ] Binomial test: is win rate significantly > 50%?
- [ ] Test hypothesis: is final rating > 2 SD above population mean?
- [ ] Establish population mean and SD (from Showdown ladder data or estimate)

### 5.2 Simulation Statistics
- [ ] Player variant win rates with confidence intervals (from Phase 2)
- [ ] Statistical significance tests for pairwise comparisons (binomial or chi-squared)
- [ ] Effect sizes for each component (how much does policy prior add? how much does search add?)

### 5.3 Failure Analysis
- [ ] Categorise losses by type (matchup, prediction error, input error, timer)
- [ ] Identify systematic weaknesses (specific Pokemon/moves the AI handles poorly)
- [ ] Decision time distribution — did any games time out?

**Effort:** 3–5 days. **Risk:** Low — it's analysis, not engineering.

---

## Phase 6 — Thesis Writing (Weeks 6–12, overlapping)

**Goal:** Write the thesis. Start no later than week 6. Earlier sections can start sooner.

### 6.1 Structure (suggested chapter outline)

| Chapter | Content | Can Start |
|---------|---------|-----------|
| 1. Introduction | Problem statement, motivation, hypothesis, contributions | Now |
| 2. Literature Review | Game AI, MCTS, imperfect info, behavioural cloning, embeddings | Now |
| 3. System Design | Simulator, models, MCTS, information model, data pipeline | Now |
| 4. Experimental Methodology | Hparam search, ablation, baselines, multi-seed, calibration | After Phase 1 |
| 5. DL Model Results | TeamPreview + BattleNet experiment results, figures, discussion | After Phase 1 |
| 6. MCTS + Player Variant Results | Variant ablation, strength-vs-compute, component contribution | After Phase 2 |
| 7. Live Evaluation | Ladder testing methodology, results, statistical analysis | After Phase 5 |
| 8. Discussion | Limitations, what worked, what didn't, comparison to prior work | After Phase 5 |
| 9. Conclusion & Future Work | Summary, self-play, transfer learning, improved info model | After Phase 5 |

### 6.2 Figures Checklist
- [ ] System architecture diagram (the pipeline from AI_System.md)
- [ ] Model architecture diagrams (BattleNet, TeamPreviewNet)
- [ ] MCTS flow diagram
- [ ] All experiment figures from Phase 1 (learning curves, ablation, calibration, etc.)
- [ ] Player variant comparison charts from Phase 2
- [ ] Rating trajectory plot from Phase 4
- [ ] Win rate comparison chart (AI-assisted vs baselines)

### 6.3 Writing Milestones
- [ ] Chapters 1–3 draft (Introduction, Lit Review, System Design) — by week 8
- [ ] Chapters 4–6 draft (Methodology, DL Results, MCTS Results) — by week 9
- [ ] Chapters 7–9 draft (Live Eval, Discussion, Conclusion) — by week 10
- [ ] Full draft to supervisor for review — by week 11
- [ ] Revisions and final polish — weeks 11–12
- [ ] Submit — 29 May 2026

**Effort:** This is the majority of the remaining work. Budget 4–6 weeks. **Risk:** High — writing always takes longer than expected. Start early.

---

## Contingency: What to Cut if Time Runs Short

If you're running behind, cut in this order (least important first):

1. **Reduce ladder games** — 100 games instead of 200 (wider confidence intervals but still publishable)
2. **Skip quick-input interface** — report only simulation results (no live evaluation). Weaker thesis but still valid — the AI system + simulation evaluation is a complete contribution
3. **Simplify player variant evaluation** — just Random vs Full MCTS (skip intermediate variants). Loses the clean ablation but keeps the main result
4. **Skip BattleNet ablation** — if GPU time is limited, run hparam + multiseed only (skip ablation and baselines). You still get the core results

**Do NOT cut:** Multi-seed evaluation (reviewers will question single-seed results), or the experiment framework itself (it demonstrates rigour).

---

## Weekly Checkpoint Schedule

| Week | Dates | Primary Focus | Milestone |
|------|-------|---------------|-----------|
| 1 | 3–9 Mar | vast.ai setup + TeamPreview experiments | TeamPreview pipeline complete |
| 2 | 10–16 Mar | BattleNet experiments + start player variants | BattleNet pipeline complete |
| 3 | 17–23 Mar | Player variant evaluation + export models | All experiment numbers in hand |
| 4 | 24–30 Mar | Quick-input interface v1 | Can input a battle state and get AI recommendation |
| 5 | 31 Mar–6 Apr | Polish interface + practice + unassisted baseline | Interface fast enough for live play |
| 6 | 7–13 Apr | Start ladder games + start writing Ch 1–3 | ~50 games played, intro drafted |
| 7 | 14–20 Apr | Continue ladder games + writing | ~100 games played, lit review drafted |
| 8 | 21–27 Apr | Continue ladder games + Ch 4–6 writing | ~150 games played, system design + methodology drafted |
| 9 | 28 Apr–4 May | Finish ladder games + statistical analysis | 200 games, all analysis complete |
| 10 | 5–11 May | Write Ch 7–9 (results, discussion, conclusion) | Full draft complete |
| 11 | 12–18 May | Full draft to supervisor, begin revisions | Supervisor feedback received |
| 12 | 19–25 May | Final revisions and polish | Thesis complete |
| -- | 29 May | **SUBMIT** | |

---

## vast.ai Specific Notes

- **What to upload:** `Tools/DLModel/` (all Python code) + `Tools/ReplayScraper/data/gen9vgc2025regi/` (parsed JSONL)
- **What to download:** `Tools/DLModel/results/gen9vgc2025regi/` (all experiment outputs)
- **Estimated GPU time:** ~24–48 hours total for both pipelines (hparam search dominates)
- **Cost estimate:** RTX 5090 on vast.ai is roughly $1–2/hr → budget ~$50–100 total
- **Tip:** Run TeamPreview first (faster, smaller data) to verify everything works before committing to the longer BattleNet run
- **Tip:** Use `tmux` or `screen` so training survives SSH disconnects
