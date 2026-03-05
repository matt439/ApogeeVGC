# Vast.ai GPU Compute Setup Instructions

## Terminal 1

ssh -p 40346 root@203.121.204.241 -L 8080:localhost:8080 -i ~/.ssh/id_ed25519

mkdir -p /workspace/data/gen9vgc2025regi

## Terminal 2

scp -P 40346 c:/VSProjects/ApogeeVGC/Tools/ReplayScraper/data/gen9vgc2025regi/*.jsonl root@203.121.204.241:/workspace/data/gen9vgc2025regi/

scp -P 40346 -i ~/.ssh/id_ed25519 -r c:/VSProjects/ApogeeVGC/Tools/DLModel root@203.121.204.241:/workspace/DLModel

## Terminal 1

cd /workspace/DLModel

pip install -r requirements.txt

python -c "import torch; print(torch.cuda.get_device_name(0))"

#### Team Preview full pipeline
python -m experiments.run_all --regulation gen9vgc2025regi --data-root /workspace/data --results-root /workspace/results

#### BattleNet full pipeline
python -m experiments.battle_run_all regulation gen9vgc2025regi --data-root /workspace/data --results-root /workspace/results

#### Quick smoke test first (to catch errors before burning GPU hours)
python -m experiments.run_all --regulation gen9vgc2025regi --data-root /workspace/data --n-trials 3 --epochs 5





