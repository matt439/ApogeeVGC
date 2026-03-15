# Vast.ai GPU Compute Setup Instructions

## Terminal 1

ssh -p PORT root@HOST -L 8080:localhost:8080 -i ~/.ssh/id_ed25519

mkdir -p /workspace/data/gen9vgc2025regi

## Terminal 2

# Copies all 3 tier files: parsed.jsonl, parsed_1200.jsonl, parsed_1500.jsonl
scp -P PORT -i ~/.ssh/id_ed25519 c:/VSProjects/ApogeeVGC/Tools/ReplayScraper/data/gen9vgc2025regi/*.jsonl root@HOST:/workspace/data/gen9vgc2025regi/

scp -P PORT -i ~/.ssh/id_ed25519 -r c:/VSProjects/ApogeeVGC/Tools/DLModel root@HOST:/workspace/DLModel

## Terminal 1

cd /workspace/DLModel

pip install -r requirements.txt

python -c "import torch; print(torch.cuda.get_device_name(0))"

#### Quick smoke test first (to catch errors before burning GPU hours)
python -m experiments.preview_run_all --regulation gen9vgc2025regi --data-root /workspace/data --results-root /workspace/results --n-trials 3 --epochs 5 --tiers all

#### Team Preview full pipeline (all rating tiers)
python -m experiments.preview_run_all --regulation gen9vgc2025regi --data-root /workspace/data --results-root /workspace/results

#### BattleNet full pipeline (all rating tiers)
python -m experiments.battle_run_all --regulation gen9vgc2025regi --data-root /workspace/data --results-root /workspace/results

## Retrieving results (Terminal 2)

scp -P PORT -i ~/.ssh/id_ed25519 -r root@HOST:/workspace/results c:/VSProjects/ApogeeVGC/Tools/DLModel/results