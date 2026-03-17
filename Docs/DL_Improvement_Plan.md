# DL Model Improvement Plan

**Timeline**: 9 weeks until thesis due. Overnight experiment runs are the bottleneck.

## Night 1: Code Fixes (REVERTED)

All three changes were tested and **reverted** — they made performance worse:

- [x] ~~**BCEWithLogitsLoss**~~: Tested. Disrupted shared trunk representations — value head gradients without sigmoid saturation dominated the trunk, hurting policy accuracy. Win rate dropped from 49% to 45%. **Reverted.**
- [x] ~~**Loss weight rebalancing**~~: `value_weight=2.0` starved policy head. Policy top-1 dropped 1.6%. **Reverted.**
- [x] **Increase max epochs**: Raised to 100 with patience 15. Harmless — early stopping kicks in naturally. **Kept.**

Lesson: the sigmoid saturation in the original BCELoss setup was acting as implicit gradient clipping on the value head, preventing it from dominating the shared trunk.

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
