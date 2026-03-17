# DL Model Improvement Plan

**Timeline**: 9 weeks until thesis due. Overnight experiment runs are the bottleneck.

## Night 1: Code Fixes (batch together)

- [ ] **BCEWithLogitsLoss**: Remove sigmoid from value head forward pass, switch training loss to `BCEWithLogitsLoss`. Apply sigmoid only at inference. Pure correctness fix.
- [ ] **Loss weight rebalancing**: Add configurable `value_weight` to training. Default to `value_weight=2.0` so value head gets proportional gradient signal (currently drowned out by 2× policy losses).
- [ ] **Increase max epochs**: Raise cap from 50 to 100+, increase patience to 15+. Every ablation run hit epoch 49 — training may have been cut short.

Run full experiment pipeline (hparam → ablation → multiseed) overnight.

## Night 2: MCTS Ablation + Diagnostics

- [ ] Build diagnostics during the day (run on existing checkpoints, no training needed):
  - Value prediction vs turn number plot
  - Policy confidence histogram (correct vs incorrect)
  - Move-type accuracy breakdown (attacks / status / Protect / switches)
- [ ] Run MCTS+DL vs MCTS-only evaluation overnight — **this is the primary thesis result**

## Night 3+: Conditional on Results

Based on Night 1/2 results, pick at most one:
- [ ] Turn-dependent value smoothing (if value-vs-turn diagnostic shows poor early-game calibration)
- [ ] Train on 1500+ rating tier only (if cross-tier comparison shows significant quality gap)
- [ ] Action frequency weighting / focal loss (if move-type breakdown shows the model only learns common actions)

## Thesis Framing

- Report DL-Greedy vs Random honestly (expected result for behavioral cloning at 61% per-decision accuracy)
- MCTS+DL vs MCTS-only is the primary evidence of the network's contribution
- Offline metrics (72% value, 61% policy, ECE 0.011) demonstrate the model learns meaningful patterns
