# Literature Review Topics and Key References

This document catalogues the research areas, keywords, and seminal papers relevant to the ApogeeVGC honours thesis. Organised by theme, from foundational to domain-specific.

---

## 1. Game-Playing AI and Deep Reinforcement Learning

The core paradigm: combining neural network evaluation with tree search for superhuman game play.

**Keywords:** game-playing AI, deep reinforcement learning, self-play, neural network game evaluation, learned heuristics, superhuman performance

### Key Papers

- **Silver, D. et al. (2016).** *Mastering the game of Go with deep neural networks and tree search.* Nature, 529(7587), 484–489. — **AlphaGo**: first superhuman Go AI, combining supervised learning from human games with MCTS and value/policy networks.
- **Silver, D. et al. (2017).** *Mastering the game of Go without human knowledge.* Nature, 550(7676), 354–359. — **AlphaGo Zero**: learns entirely from self-play, no human data. Demonstrated that tabula rasa learning can surpass human-trained systems.
- **Silver, D. et al. (2018).** *A general reinforcement learning algorithm that masters chess, shogi, and Go through self-play.* Science, 362(6419), 1140–1144. — **AlphaZero**: generalised the approach across multiple perfect-information games. Introduced the PUCT selection formula used in this project.
- **Schrittwieser, J. et al. (2020).** *Mastering Atari, Go, chess and shogi by planning with a learned model.* Nature, 588(7839), 604–609. — **MuZero**: learns the environment dynamics model alongside policy and value, enabling planning without access to a simulator.
- **Vinyals, O. et al. (2019).** *Grandmaster level in StarCraft II using multi-agent reinforcement learning.* Nature, 575(7782), 350–354. — **AlphaStar**: imperfect information, real-time, multi-agent. Relevant for simultaneous-move game AI.

---

## 2. Monte Carlo Tree Search (MCTS)

The search algorithm at the heart of the system. Covers vanilla MCTS, UCT, and the neural-network-guided PUCT variant.

**Keywords:** Monte Carlo Tree Search, Upper Confidence bounds for Trees (UCT), PUCT, exploration–exploitation trade-off, bandit algorithms, tree policy, simulation policy, backup strategy

### Key Papers

- **Kocsis, L. & Szepesvári, C. (2006).** *Bandit based Monte-Carlo planning.* ECML 2006. — Introduced **UCT** (Upper Confidence bounds for Trees), the foundational selection formula for MCTS.
- **Coulom, R. (2006).** *Efficient selectivity and backup operators in Monte-Carlo tree search.* CG 2006. — Early formalisation of MCTS with selective search.
- **Browne, C.B. et al. (2012).** *A survey of Monte Carlo Tree Search methods.* IEEE Transactions on Computational Intelligence and AI in Games, 4(1), 1–43. — **Comprehensive survey** covering MCTS variants, enhancements, and applications. Essential reference.
- **Rosin, C.D. (2011).** *Multi-armed bandits with episode context.* Annals of Mathematics and Artificial Intelligence, 61(3), 203–230. — PUCT (Predictor + UCT), the selection formula used by AlphaZero and this project.

---

## 3. Imperfect Information Games

VGC is a game of imperfect information (hidden moves, items, abilities, EVs). This is a major differentiator from chess/Go.

**Keywords:** imperfect information games, information sets, partial observability, belief states, opponent modelling, hidden information, Bayesian updating, information asymmetry

### Key Papers

- **Bowling, M. et al. (2015).** *Heads-up limit hold'em poker is solved.* Science, 347(6218), 145–149. — **Cepheus**: first essentially solved imperfect-information game of practical scale.
- **Brown, N. & Sandholm, T. (2018).** *Superhuman AI for heads-up no-limit poker: Libratus beats top professionals.* Science, 359(6374), 418–424. — **Libratus**: superhuman no-limit poker via abstraction + subgame solving.
- **Brown, N. & Sandholm, T. (2019).** *Superhuman AI for multiplayer poker.* Science, 365(6456), 885–890. — **Pluribus**: multiplayer poker, uses blueprint strategy + real-time search. Relevant for multi-agent imperfect-information settings.
- **Zinkevich, M. et al. (2007).** *Regret minimization in games with incomplete information.* NeurIPS 2007. — **Counterfactual Regret Minimization (CFR)**, the dominant algorithm family for solving imperfect-information games.

---

## 4. Information Set MCTS (ISMCTS) and Determinization

The planned approach for handling hidden information: determinize (sample possible worlds) and search each one.

**Keywords:** Information Set Monte Carlo Tree Search, determinization, perfect-information Monte Carlo sampling, multiple-observer information sets, strategy fusion, belief-state sampling

### Key Papers

- **Cowling, P.I., Powley, E.J. & Whitehouse, D. (2012).** *Information Set Monte Carlo Tree Search.* IEEE Transactions on Computational Intelligence and AI in Games, 4(2), 120–143. — **ISMCTS**: the core algorithm for MCTS under imperfect information, directly relevant to the planned determinization approach.
- **Frank, I. & Basin, D. (1998).** *Search in games with incomplete information: A case study using Bridge.* Artificial Intelligence, 100(1–2), 87–123. — Early work on determinization for card games; discusses the "strategy fusion" problem where averaging over determinizations can be suboptimal.
- **Long, J.R., Sturtevant, N.R., Buro, M. & Furtak, T. (2010).** *Understanding the success of perfect information Monte Carlo sampling in game tree search.* AAAI 2010. — Analyses when and why determinization works well, and when it fails.

---

## 5. Simultaneous-Move Games

VGC has simultaneous moves (both players choose actions before resolution), unlike alternating-move games like chess.

**Keywords:** simultaneous-move games, normal-form games, Nash equilibrium, maximin, extensive-form games with simultaneous moves, double oracle

### Key Papers

- **Lanctot, M. et al. (2014).** *Monte Carlo Tree Search for simultaneous move games: A case study in the game of Tron.* CIG 2014. — Directly addresses MCTS adaptation for simultaneous-move settings.
- **Bošanský, B. et al. (2016).** *Algorithms for computing strategies in two-player simultaneous move games.* Artificial Intelligence, 237, 1–40. — Survey of algorithms for simultaneous-move game solving.
- **Kovarik, V., Schmid, M., Burch, N., Bowling, M. & Lisý, V. (2022).** *Rethinking formal models of partially observable multiagent decision making.* Artificial Intelligence, 303, 103645. — Modern treatment of partial observability in multi-agent settings.

---

## 6. Behavioural Cloning and Imitation Learning

The training paradigm: learning to predict human expert decisions from replay data, rather than self-play.

**Keywords:** behavioural cloning, imitation learning, learning from demonstrations, offline learning, policy distillation, supervised learning for game AI, distribution shift, covariate shift

### Key Papers

- **Pomerleau, D.A. (1991).** *Efficient training of artificial neural networks for autonomous navigation.* Neural Computation, 3(1), 88–97. — **ALVINN**: foundational behavioural cloning work (self-driving, but established the paradigm).
- **Ross, S., Gordon, G. & Bagnell, J.A. (2011).** *A reduction of imitation learning and structured prediction to no-regret online learning.* AISTATS 2011. — **DAgger**: addresses the distribution shift problem in behavioural cloning. Relevant limitation to discuss.
- **Ho, J. & Ermon, S. (2016).** *Generative adversarial imitation learning.* NeurIPS 2016. — **GAIL**: alternative imitation learning approach using adversarial training.

---

## 7. Multi-Task and Multi-Head Neural Networks

The battle model jointly predicts value and two policies. The team preview model jointly predicts bring and lead selections.

**Keywords:** multi-task learning, multi-head architecture, auxiliary tasks, shared representations, hard parameter sharing, task weighting, value-policy networks

### Key Papers

- **Caruana, R. (1997).** *Multitask learning.* Machine Learning, 28(1), 41–75. — Foundational multi-task learning paper; argues shared representations improve generalisation.
- **Ruder, S. (2017).** *An overview of multi-task learning in deep neural networks.* arXiv:1706.05098. — Comprehensive survey covering hard/soft parameter sharing, task weighting, and when multi-task helps.
- **Kendall, A., Gal, Y. & Cipolla, R. (2018).** *Multi-task learning using uncertainty to weigh losses for scene geometry and semantics.* CVPR 2018. — Principled approach to balancing multiple loss terms using learned uncertainty weights.

---

## 8. Entity Embeddings for Categorical Variables

Species, moves, items, abilities, and tera types are encoded via learned embeddings — a key architectural choice.

**Keywords:** entity embeddings, categorical embeddings, learned representations, embedding layers, representation learning for discrete variables

### Key Papers

- **Guo, C. & Berkhahn, F. (2016).** *Entity embeddings of categorical variables.* arXiv:1604.06737. — Directly relevant: demonstrates that learned embeddings for categorical variables outperform one-hot encoding in tabular/structured data tasks.
- **Mikolov, T. et al. (2013).** *Efficient estimation of word representations in vector space.* arXiv:1301.3781. — **Word2Vec**: the original learned embedding work that inspired entity embeddings.

---

## 9. Hyperparameter Optimisation

The experiment framework uses Bayesian optimisation (Optuna/TPE) for systematic hyperparameter search.

**Keywords:** hyperparameter optimisation, Bayesian optimisation, Tree-structured Parzen Estimator (TPE), automated machine learning (AutoML), random search, grid search, early stopping, pruning, neural architecture search

### Key Papers

- **Bergstra, J., Bardenet, R., Bengio, Y. & Kégl, B. (2011).** *Algorithms for hyper-parameter optimization.* NeurIPS 2011. — Introduced **TPE** (Tree-structured Parzen Estimator), the algorithm used by Optuna.
- **Bergstra, J. & Bengio, Y. (2012).** *Random search for hyper-parameter optimization.* JMLR, 13, 281–305. — Demonstrates random search is competitive with grid search; baseline for the project.
- **Akiba, T. et al. (2019).** *Optuna: A next-generation hyperparameter optimization framework.* KDD 2019. — The **Optuna** framework itself. Cite for methodology.
- **Snoek, J., Larochelle, H. & Adams, R.P. (2012).** *Practical Bayesian optimization of machine learning algorithms.* NeurIPS 2012. — Influential Bayesian optimisation paper using Gaussian processes.

---

## 10. Model Evaluation Methodology

Rigorous evaluation: ablation studies, calibration, multi-seed robustness, and baseline comparisons.

**Keywords:** ablation study, feature importance, model calibration, Expected Calibration Error (ECE), reliability diagram, statistical significance, random seed robustness, baseline comparison, cross-validation

### Key Papers

- **Guo, C., Pleiss, G., Sun, Y. & Weinberger, K.Q. (2017).** *On calibration of modern neural networks.* ICML 2017. — Modern neural networks are poorly calibrated; introduces temperature scaling. Directly relevant to the ECE and reliability diagrams in the project.
- **Naeini, M.P., Cooper, G.F. & Hauskrecht, M. (2015).** *Obtaining well calibrated probabilities using Bayesian binning into quantiles.* AAAI 2015. — Proposes ECE and calibration metrics used in the experiment framework.
- **Bouthillier, X. et al. (2021).** *Accounting for variance in machine learning benchmarks.* MLSys 2021. — Importance of multi-seed evaluation and reporting variance.
- **Melis, G., Dyer, C. & Blunsom, P. (2018).** *On the state of the art of evaluation in neural language models.* ICLR 2018. — Demonstrates that proper hyperparameter tuning and multiple seeds can reverse published results; motivates the project's rigorous evaluation.

---

## 11. ONNX and Model Deployment

Models are trained in Python (PyTorch) and deployed in C# via ONNX Runtime.

**Keywords:** ONNX, model interoperability, neural network deployment, inference optimisation, cross-platform model serving, edge inference, model export

### Key References

- **ONNX Specification.** *Open Neural Network Exchange.* https://onnx.ai/ — The interchange format used.
- **ONNX Runtime.** *Microsoft.* https://onnxruntime.ai/ — The inference engine used in C#.
- **PyTorch documentation: torch.onnx.** *Exporting models from PyTorch to ONNX.* — Technical reference for the export pipeline.

---

## 12. Pokemon and Competitive Game AI

Domain-specific work on Pokemon AI and competitive game AI more broadly.

**Keywords:** Pokemon AI, VGC, competitive game AI, metagame, team building, team selection, battle simulation, Pokemon Showdown

### Key Papers and Resources

- **Lee, Y., Kim, S. et al. (2017).** *Pokemon AI.* Various academic works on Pokemon battle AI using RL and tree search. (Search specifically for recent work — this is a niche but growing area.)
- **Chen, S. & Morimoto, T. (2020–2023).** Various works on applying RL to Pokemon battles. (Search for latest publications.)
- **pmariglia/showdown.** *Pokemon Showdown battle bot.* GitHub. — Open-source Pokemon Showdown AI using damage calculator and minimax. Relevant prior work.
- **Pokemon Showdown.** *Smogon / Guangcong Luo (Zarel).* https://pokemonshowdown.com/ — The simulator and data source.
- **Smogon usage statistics.** https://www.smogon.com/stats/ — Metagame prior distributions used for belief states.

---

## 13. Regularisation and Training Techniques

Techniques used throughout the training pipeline.

**Keywords:** dropout, batch normalisation, early stopping, learning rate scheduling, weight decay, L2 regularisation, data augmentation, overfitting, generalisation

### Key Papers

- **Srivastava, N. et al. (2014).** *Dropout: A simple way to prevent neural networks from overfitting.* JMLR, 15, 1929–1958. — Dropout, used throughout the project's MLP layers.
- **Ioffe, S. & Szegedy, C. (2015).** *Batch normalization: Accelerating deep network training by reducing internal covariate shift.* ICML 2015. — Batch normalisation, used in every trunk block.
- **Loshchilov, I. & Hutter, F. (2019).** *Decoupled weight decay regularization.* ICLR 2019. — **AdamW**: the decoupled weight decay optimiser commonly used in modern training.

---

## 14. Data Leakage and Experimental Rigour in ML

The project splits data at the game level to prevent leakage — an important methodological point.

**Keywords:** data leakage, train-test contamination, group-aware splitting, temporal splitting, data snooping, evaluation protocol, reproducibility

### Key Papers

- **Kaufman, S. et al. (2012).** *Leakage in data mining: Formulation, detection, and avoidance.* ACM TKDD, 6(4), 1–21. — Comprehensive treatment of data leakage in machine learning.
- **Kapoor, S. & Narayanan, A. (2023).** *Leakage and the reproducibility crisis in machine-learning-based science.* Patterns, 4(9). — Modern paper on widespread data leakage in ML research; strengthens the justification for game-level splitting.

---

## 15. Transfer Learning Across Game Formats

Each regulation gets a separate model; future work could explore transfer between formats.

**Keywords:** transfer learning, domain adaptation, fine-tuning, pre-training, meta-learning, few-shot learning, distribution shift

### Key Papers

- **Pan, S.J. & Yang, Q. (2010).** *A survey on transfer learning.* IEEE Transactions on Knowledge and Data Engineering, 22(10), 1345–1359. — Survey covering when and how transfer learning helps.
- **Yosinski, J. et al. (2014).** *How transferable are features in deep neural networks?* NeurIPS 2014. — Layer-by-layer analysis of feature transferability; relevant for deciding which layers to freeze/fine-tune.

---

## 16. Belief State Estimation and Bayesian Inference for Games

Relevant to the planned information model and belief-state tracking.

**Keywords:** belief state, Bayesian updating, particle filtering, state estimation, hidden Markov models, Bayesian opponent modelling, constraint propagation

### Key Papers

- **Thrun, S., Burgard, W. & Fox, D. (2005).** *Probabilistic Robotics.* MIT Press. — Foundational treatment of belief state estimation (particle filters, Bayesian filters) applicable to maintaining beliefs over opponent state.
- **Albrecht, S.V. & Stone, P. (2018).** *Autonomous agents modelling other agents: A comprehensive survey and open problems.* Artificial Intelligence, 258, 66–95. — Survey on opponent modelling; relevant to predicting opponent actions and hidden state.
- **Richards, M. & Amir, E. (2007).** *Opponent modeling in Scrabble.* AAAI 2007. — Imperfect-information game with belief tracking over opponent tiles; analogous to tracking opponent moves/items.

---

## 17. Data Collection from Online Platforms (Replay Mining)

The data pipeline scrapes and parses replays from Pokemon Showdown.

**Keywords:** replay analysis, game log mining, web scraping for ML data, data extraction, structured data from unstructured logs, protocol parsing

### Key References

- **Various Showdown protocol documentation.** The Pokemon Showdown battle protocol specification — the basis for the parser.
- **Thompson, J.J. et al. (2013).** *Characterising online games through gameplay data analysis.* (Search for works on mining competitive game replays for ML, e.g., StarCraft replay mining papers.)
- **Weber, B.G. & Mateas, M. (2009).** *A data mining approach to strategy prediction.* CIG 2009. — Mining StarCraft replays for strategy prediction; analogous methodology.

---

## 18. Loss Functions for Multi-Label and Multi-Output Classification

The team preview model uses BCE for multi-label bring/lead prediction; the battle model combines BCE (value) with cross-entropy (policy).

**Keywords:** binary cross-entropy, multi-label classification, multi-output learning, sigmoid activation, softmax, loss function design, class imbalance, masked loss

### Key Papers

- **Zhang, M.L. & Zhou, Z.H. (2014).** *A review on multi-label learning algorithms.* IEEE Transactions on Knowledge and Data Engineering, 26(8), 1819–1837. — Survey on multi-label learning approaches.

---

## Suggested Search Queries for Additional Papers

Use these queries to find the most recent and relevant work:

1. `"Monte Carlo Tree Search" "imperfect information" "simultaneous moves"`
2. `"Pokemon" "reinforcement learning" OR "deep learning" OR "AI"`
3. `"VGC" OR "Video Game Championships" "machine learning"`
4. `"behavioral cloning" "game AI" "replay"`
5. `"ISMCTS" "determinization" "card game" OR "hidden information"`
6. `"neural network" "game evaluation" "value network" "policy network"`
7. `"entity embeddings" "categorical" "game" OR "tabular"`
8. `"multi-task learning" "value" "policy" "game"`
9. `"AlphaZero" "extensions" OR "imperfect information" OR "simultaneous"`
10. `"Bayesian optimization" "hyperparameter" "neural network" "game"`
11. `"model calibration" "expected calibration error" "classification"`
12. `"data leakage" "train test split" "grouped" OR "game level"`

---

## Summary Table

| # | Topic | Relevance to Project |
|---|-------|---------------------|
| 1 | Game-playing AI / Deep RL | Core paradigm (AlphaZero-style value+policy network) |
| 2 | MCTS | Search engine (PUCT selection, neural evaluation) |
| 3 | Imperfect information games | VGC has hidden moves, items, abilities, EVs |
| 4 | ISMCTS / Determinization | Planned approach for handling hidden info |
| 5 | Simultaneous-move games | Both players act simultaneously each turn |
| 6 | Behavioural cloning | Training from human replay data, not self-play |
| 7 | Multi-task / multi-head networks | Joint value + policy prediction |
| 8 | Entity embeddings | Species/move/item/ability encoding |
| 9 | Hyperparameter optimisation | Optuna/TPE systematic search |
| 10 | Evaluation methodology | Ablation, calibration, multi-seed, baselines |
| 11 | ONNX deployment | PyTorch → ONNX → C# inference pipeline |
| 12 | Pokemon / competitive game AI | Domain-specific prior work |
| 13 | Regularisation techniques | Dropout, BatchNorm, early stopping, weight decay |
| 14 | Data leakage / experimental rigour | Game-level splitting, reproducibility |
| 15 | Transfer learning | Cross-regulation model reuse (future work) |
| 16 | Belief state estimation | Bayesian opponent modelling, info tracking |
| 17 | Replay mining | Data pipeline from online game platforms |
| 18 | Multi-label loss functions | BCE for bring/lead, CE for policy heads |
