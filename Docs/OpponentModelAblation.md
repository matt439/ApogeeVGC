# Opponent Prediction Model Ablation

## Model Details

- **Architecture:** OpponentPredictionNet — same embeddings as BattleNet, 2-layer trunk, 254K parameters
- **Training data:** ~300K replays, both perspectives, all games (not winners-only)
- **Task:** Predict opponent's next action (per active slot)

## Offline Metrics

| Metric | Slot A | Slot B | Combined |
|---|---|---|---|
| Top-1 accuracy | 48.9% | 48.0% | 48.4% |
| Top-3 accuracy | 75.5% | 74.9% | 75.2% |

Test set: 60,910 games, 1,046,194 samples.

The model predicts the opponent's exact action correctly ~48% of the time. The correct action is in the top 3 predictions 75% of the time.

## Live Performance (Ensemble vs Random, 1000 iterations)

| Configuration | Win Rate | Notes |
|---|---|---|
| Ensemble (heuristic only, tuned weights) | **88%** | ProtectPrediction uses simple heuristic fallback |
| Ensemble (+ opponent model, old weights) | 68% | Weights tuned without DL, DL mispredictions hurt |
| Ensemble (+ opponent model, retuned weights) | 65% | Weights retuned with DL active |

## Analysis

The opponent prediction model **degrades** ensemble performance from 88% to 65% vs random.

**Root cause:** The ProtectPrediction mini-model has a high weight (3.86-4.31) because predicting Protect is strategically critical in VGC. When the DL model's predictions are wrong (52% of the time), this high-weight model confidently misdirects MCTS search toward wrong actions.

The heuristic fallback (checking if the opponent has a Protect stall counter active) is simpler but more reliable — it only activates when there's concrete evidence, rather than making probabilistic predictions that are wrong half the time.

**Weight shift with DL:** Retuning with the opponent model active produced dramatically different weights — TypePositioning rose from 2.3 to 4.4 (top), while DamageMax dropped from 3.7 to 0.4. This suggests the optimizer compensated for the noisy DL signal by deprioritizing damage-based decisions.

| Mini-Model | Without DL | With DL |
|---|---|---|
| TypePositioning | 2.32 | **4.43** |
| ProtectPrediction | 4.31 | **3.86** |
| DamageMin | 1.55 | **1.99** |
| KOAvoidance | 0.88 | **1.80** |
| StatusSpreading | 1.10 | **1.07** |
| KOSeeking | 1.78 | **1.03** |
| SpeedAdvantage | 3.31 | **0.49** |
| DamageMax | 3.65 | **0.44** |

## Conclusion

48% top-1 accuracy is insufficient for the opponent prediction model to contribute positively. The model needs higher accuracy (estimated >65%) before its predictions help more than the simple heuristic fallback.

Possible improvements for future work:
- **More training data** — 300K replays may be insufficient for opponent modeling
- **Higher-rated games only** — filter to 1500+ for more consistent opponent behavior
- **Simpler prediction target** — predict action category (attack/protect/switch) instead of exact action
- **Self-play** — train on the bot's own opponents rather than human replays

## Decision

The opponent model is **disabled** for the thesis evaluation. The heuristic-only ensemble (88% vs random, 53.2% vs standalone) is the primary result. The opponent model ablation is reported as an honest finding showing the accuracy threshold needed for DL opponent modeling to be beneficial.
