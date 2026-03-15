# AI Pipeline Instructions for Local PC

## Terminal 1 — Tools/ReplayScraper

pip install -r requirements.txt

#### Scrape replays
python scraper.py --format gen9vgc2025regi

#### Parse replays (produces parsed.jsonl, parsed_1200.jsonl, parsed_1500.jsonl)
python parser.py --format gen9vgc2025regi

## Terminal 2 — Tools/DLModel

pip install -r requirements.txt

#### Quick smoke test first (to catch errors before burning GPU hours)
python -m experiments.preview_run_all --regulation gen9vgc2025regi --n-trials 3 --epochs 5 --tiers all

#### Team Preview full pipeline (all rating tiers)
python -m experiments.preview_run_all --regulation gen9vgc2025regi

#### BattleNet full pipeline (all rating tiers)
python -m experiments.battle_run_all --regulation gen9vgc2025regi



python export_best.py --regulation gen9vgc2025regi --tier all
python export_best.py --regulation gen9vgc2025regi --tier 1200+
python export_best.py --regulation gen9vgc2025regi --tier 1500+