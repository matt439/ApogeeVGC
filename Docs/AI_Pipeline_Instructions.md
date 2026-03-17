# AI Pipeline Instructions for Local PC

## Manual Pipeline

### Terminal 1 — Tools/ReplayScraper

pip install -r requirements.txt

#### Scrape replays
python scraper.py --format gen9vgc2025regi

#### Parse replays (produces parsed.jsonl, parsed_1200.jsonl, parsed_1500.jsonl)
python parser.py --format gen9vgc2025regi

### Terminal 2 — Tools/DLModel

pip install -r requirements.txt

#### Quick smoke test first (to catch errors before burning GPU hours)
python -m experiments.preview_run_all --regulation gen9vgc2025regi --n-trials 3 --epochs 5 --tiers all

#### Team Preview full pipeline (all rating tiers)
python -m experiments.preview_run_all --regulation gen9vgc2025regi

#### BattleNet full pipeline (all rating tiers)
python -m experiments.battle_run_all --regulation gen9vgc2025regi

#### Generate ONNX files (pick one tier for production — each overwrites the same output folder)
python export_best.py --regulation gen9vgc2025regi --tier all

## Automated Pipeline (overnight run)

From `Tools/DLModel/`:

#### Full pipeline: train both models, export ONNX, evaluate via bot-vs-bot, generate report
python pipeline.py --format gen9vgc2025regi

#### Skip training (use existing models), just evaluate and report
python pipeline.py --format gen9vgc2025regi --stages export evaluate report

#### Quick smoke test (no DL models needed, random vs greedy only)
python pipeline.py --format gen9vgc2025regi --stages evaluate report \
    --eval-ai-types random --eval-controls greedy --eval-battles 10

#### Custom evaluation with specific AI types and more battles
python pipeline.py --format gen9vgc2025regi --tier 1500+ \
    --eval-ai-types dlgreedy mctsdl \
    --eval-controls random greedy mcts_standalone \
    --eval-battles 500 --mcts-iterations 1000

#### Pipeline is resumable — re-running skips completed stages
#### To force re-evaluation, delete results/<format>/evaluation/<tier>/pipeline_state.json

### Available player types for --eval-ai-types and --eval-controls:
- `random` — uniform random action selection
- `greedy` — highest damage move (BasePower x type effectiveness x STAB)
- `mcts_standalone` — MCTS with heuristic eval, no DL models
- `dlgreedy` — DL policy argmax, no search (requires ONNX models)
- `mctsdl` — MCTS with DL policy priors + value eval (requires ONNX models)
- `mcts` — full MCTS with DL + information tracking (requires ONNX models)