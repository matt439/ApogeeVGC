"""
EC2 Deployment Script for ApogeeVGC Showdown Battler

Automates the full cycle: build → package → launch EC2 → upload → run → stream logs → download results → terminate.

Prerequisites:
  - AWS CLI configured (`aws configure`)
  - SSH key pair created in AWS (`aws ec2 create-key-pair --key-name apogee ...`)
  - .pem file saved locally

Usage:
  python deploy_ec2.py                                          # smoke test (t3.medium, 2 battles)
  python deploy_ec2.py --instance-type c7i.48xlarge --battles 100  # production run
  python deploy_ec2.py --skip-build                              # reuse existing publish/
  python deploy_ec2.py --instance-id i-1234567890abcdef0          # reuse running instance
"""

import argparse
import json
import os
import subprocess
import sys
import time
from pathlib import Path

SCRIPT_DIR = Path(__file__).resolve().parent
PROJECT_ROOT = SCRIPT_DIR.parent
PUBLISH_DIR = PROJECT_ROOT / 'publish'
CSPROJ = PROJECT_ROOT / 'ApogeeVGC' / 'ApogeeVGC.csproj'
ARCHIVE_NAME = 'apogee.tar.gz'

# Amazon Linux 2023 AMI (us-east-1) — update if using a different region
DEFAULT_AMI = 'ami-0c421724a94bba6d6'
DEFAULT_KEY_NAME = 'apogee'
DEFAULT_KEY_PATH = str(PROJECT_ROOT / 'apogee.pem')
DEFAULT_SECURITY_GROUP = 'apogee-sg'
DEFAULT_REGION = 'us-east-1'


def run(cmd: str | list, check: bool = True, capture: bool = False, **kwargs) -> subprocess.CompletedProcess:
    if isinstance(cmd, str):
        cmd = cmd.split()
    print(f'  $ {" ".join(cmd[:6])}{"..." if len(cmd) > 6 else ""}')
    return subprocess.run(cmd, check=check, capture_output=capture, text=True, **kwargs)


def build_and_package(args: argparse.Namespace) -> Path:
    """Build headless Linux binary and create tar.gz archive."""
    print('\n=== Building headless Linux binary ===')
    run(['dotnet', 'publish', str(CSPROJ), '-c', 'Release',
         '--self-contained', '-r', 'linux-x64', '-p:Headless=true',
         '-o', str(PUBLISH_DIR)])

    # Copy config files
    for f in ['showdown_config.json', 'team.txt']:
        src = PROJECT_ROOT / f
        if src.exists():
            import shutil
            shutil.copy2(src, PUBLISH_DIR / f)
            print(f'  Copied {f}')

    # Copy ensemble config
    ensemble_src = PROJECT_ROOT / 'Tools' / 'DLModel' / 'ensemble_config.json'
    if ensemble_src.exists():
        dest = PUBLISH_DIR / 'Tools' / 'DLModel'
        dest.mkdir(parents=True, exist_ok=True)
        import shutil
        shutil.copy2(ensemble_src, dest / 'ensemble_config.json')
        print('  Copied ensemble_config.json')

    # Update showdown_config.json with battle count
    config_path = PUBLISH_DIR / 'showdown_config.json'
    if config_path.exists():
        with open(config_path) as f:
            config = json.load(f)
        config['numBattles'] = args.battles
        config['player'] = args.player
        with open(config_path, 'w') as f:
            json.dump(config, f, indent=4)
        print(f'  Updated config: {args.battles} battles, player={args.player}')

    # Create archive
    archive_path = PROJECT_ROOT / ARCHIVE_NAME
    print(f'\n=== Packaging ({ARCHIVE_NAME}) ===')
    run(['tar', '-czf', str(archive_path), '-C', str(PUBLISH_DIR), '.'])

    size_mb = archive_path.stat().st_size / 1024 / 1024
    print(f'  Archive size: {size_mb:.1f} MB')
    return archive_path


def ensure_security_group(args: argparse.Namespace) -> str:
    """Create security group if it doesn't exist."""
    result = run(['aws', 'ec2', 'describe-security-groups',
                  '--group-names', args.security_group,
                  '--region', args.region],
                 check=False, capture=True)

    if result.returncode != 0:
        print(f'\n=== Creating security group: {args.security_group} ===')
        result = run(['aws', 'ec2', 'create-security-group',
                      '--group-name', args.security_group,
                      '--description', 'ApogeeVGC Showdown battler',
                      '--region', args.region],
                     capture=True)
        sg_id = json.loads(result.stdout)['GroupId']

        # Allow SSH
        run(['aws', 'ec2', 'authorize-security-group-ingress',
             '--group-id', sg_id,
             '--protocol', 'tcp', '--port', '22', '--cidr', '0.0.0.0/0',
             '--region', args.region])
        # Allow MCTS worker port
        run(['aws', 'ec2', 'authorize-security-group-ingress',
             '--group-id', sg_id,
             '--protocol', 'tcp', '--port', '9100', '--cidr', '0.0.0.0/0',
             '--region', args.region])
        print(f'  Created {sg_id} with SSH + worker port access')
        return sg_id
    else:
        sg_id = json.loads(result.stdout)['SecurityGroups'][0]['GroupId']
        print(f'  Using existing security group: {sg_id}')
        return sg_id


def launch_instance(args: argparse.Namespace, sg_id: str) -> str:
    """Launch EC2 instance and return instance ID."""
    print(f'\n=== Launching {args.instance_type} instance ===')
    result = run(['aws', 'ec2', 'run-instances',
                  '--image-id', args.ami,
                  '--instance-type', args.instance_type,
                  '--key-name', args.key_name,
                  '--security-group-ids', sg_id,
                  '--region', args.region,
                  '--count', '1',
                  '--tag-specifications',
                  'ResourceType=instance,Tags=[{Key=Name,Value=ApogeeVGC-Battler}]'],
                 capture=True)

    instance_id = json.loads(result.stdout)['Instances'][0]['InstanceId']
    print(f'  Instance: {instance_id}')

    # Wait for running state
    print('  Waiting for instance to start...')
    run(['aws', 'ec2', 'wait', 'instance-running',
         '--instance-ids', instance_id,
         '--region', args.region])

    # Get public IP
    result = run(['aws', 'ec2', 'describe-instances',
                  '--instance-ids', instance_id,
                  '--region', args.region,
                  '--query', 'Reservations[0].Instances[0].PublicIpAddress',
                  '--output', 'text'],
                 capture=True)
    ip = result.stdout.strip()
    print(f'  IP: {ip}')

    return instance_id, ip


def wait_for_ssh(ip: str, key_path: str, max_retries: int = 30):
    """Wait until SSH is available."""
    print('  Waiting for SSH...')
    for i in range(max_retries):
        result = subprocess.run(
            ['ssh', '-i', key_path, '-o', 'StrictHostKeyChecking=no',
             '-o', 'ConnectTimeout=5', f'ec2-user@{ip}', 'echo ok'],
            capture_output=True, text=True)
        if result.returncode == 0:
            print('  SSH ready.')
            return
        time.sleep(5)
    raise TimeoutError('SSH not available after retries')


def upload_and_extract(ip: str, key_path: str, archive_path: Path):
    """Upload archive and extract on EC2."""
    print('\n=== Uploading to EC2 ===')
    run(['scp', '-i', key_path, '-o', 'StrictHostKeyChecking=no',
         str(archive_path), f'ec2-user@{ip}:~/'])

    print('\n=== Extracting on EC2 ===')
    ssh(ip, key_path, f'mkdir -p ~/apogee && cd ~/apogee && tar -xzf ~/{ARCHIVE_NAME} && chmod +x ApogeeVGC',
        timeout=120)
    print('  Extracted. Installing dependencies...')
    ssh(ip, key_path, 'sudo yum install -y libicu 2>/dev/null; true', timeout=120)
    print('  Done.')


def ssh(ip: str, key_path: str, command: str, stream: bool = False,
        timeout: int | None = 60) -> subprocess.CompletedProcess:
    """Run command on EC2 via SSH."""
    cmd = ['ssh', '-i', key_path, '-o', 'StrictHostKeyChecking=no',
           f'ec2-user@{ip}', command]
    if stream:
        return subprocess.run(cmd)
    return subprocess.run(cmd, capture_output=True, text=True, timeout=timeout)


def start_worker(ip: str, key_path: str, args: argparse.Namespace):
    """Start the MCTS worker on EC2 in background."""
    print('\n=== Starting MCTS Worker on EC2 ===')

    # Start worker in background using ssh -f (forks SSH to background after command starts)
    cmd = ['ssh', '-i', key_path, '-o', 'StrictHostKeyChecking=no', '-f',
           f'ec2-user@{ip}',
           f'cd ~/apogee && nohup ./ApogeeVGC --mode MctsWorker --format {args.format} --port 9100 '
           f'> worker.log 2>&1 < /dev/null &']
    subprocess.run(cmd, timeout=30)

    # Wait for worker to start listening
    print('  Waiting for worker to start...')
    import time
    for i in range(30):
        time.sleep(2)
        result = ssh(ip, key_path, 'cat ~/apogee/worker.log 2>/dev/null | tail -3')
        if 'Listening on port' in (result.stdout or ''):
            print('  Worker is ready.')
            return
        if 'Error' in (result.stdout or '') or 'error' in (result.stderr or ''):
            print(f'  Worker error: {result.stdout}')
            raise RuntimeError('Worker failed to start')
    raise TimeoutError('Worker did not start in time')


def run_local_client(ip: str, args: argparse.Namespace):
    """Run the local Showdown client that connects to the EC2 worker."""
    print('\n=== Running Local Showdown Client ===')
    print(f'  Connecting to worker at {ip}:9100')
    print('  (Ctrl+C to stop)')
    print()

    # Run the local client (not on EC2 — runs on home machine)
    csproj = str(PROJECT_ROOT / 'ApogeeVGC' / 'ApogeeVGC.csproj')
    subprocess.run(
        ['dotnet', 'run', '--project', csproj, '-c', 'Release', '--no-build', '--',
         '--mode', 'ShowdownBattlerRemote',
         '--format', args.format,
         '--worker-host', ip,
         '--worker-port', '9100'])


def download_worker_logs(ip: str, key_path: str):
    """Download worker log from EC2 for diagnostics."""
    print('\n=== Downloading worker logs ===')
    local_dir = PROJECT_ROOT / 'logs' / 'ec2_worker'
    local_dir.mkdir(parents=True, exist_ok=True)

    run(['scp', '-i', key_path, '-o', 'StrictHostKeyChecking=no',
         f'ec2-user@{ip}:~/apogee/worker.log',
         str(local_dir / 'worker.log')], check=False)
    print(f'  Worker log saved to: {local_dir / "worker.log"}')
    print(f'  Battle results are in: {PROJECT_ROOT / "logs" / "showdown"}')


def terminate_instance(instance_id: str, region: str):
    """Terminate the EC2 instance."""
    print(f'\n=== Terminating instance {instance_id} ===')
    run(['aws', 'ec2', 'terminate-instances',
         '--instance-ids', instance_id,
         '--region', region])
    print('  Instance terminated.')


def main():
    parser = argparse.ArgumentParser(description='Deploy ApogeeVGC to EC2')
    parser.add_argument('--instance-type', default='t3.medium',
                        help='EC2 instance type (default: t3.medium for smoke test)')
    parser.add_argument('--battles', type=int, default=2,
                        help='Number of Showdown battles to run')
    parser.add_argument('--format', default='gen9vgc2026regi',
                        help='Pokemon Showdown format')
    parser.add_argument('--player', default='ensemble',
                        help='Player type (ensemble, dlgreedy, random)')
    parser.add_argument('--ami', default=DEFAULT_AMI,
                        help='AMI ID (default: Amazon Linux 2023 us-east-1)')
    parser.add_argument('--key-name', default=DEFAULT_KEY_NAME,
                        help='EC2 key pair name')
    parser.add_argument('--key-path', default=DEFAULT_KEY_PATH,
                        help='Path to .pem file')
    parser.add_argument('--security-group', default=DEFAULT_SECURITY_GROUP,
                        help='Security group name')
    parser.add_argument('--region', default=DEFAULT_REGION)
    parser.add_argument('--skip-build', action='store_true',
                        help='Skip build, reuse existing publish/')
    parser.add_argument('--instance-id', default=None,
                        help='Reuse a running instance (skip launch)')
    parser.add_argument('--no-terminate', action='store_true',
                        help='Keep instance running after battles complete')
    parser.add_argument('--download-only', action='store_true',
                        help='Just download results from a running instance')
    args = parser.parse_args()

    instance_id = args.instance_id
    ip = None

    try:
        # Get IP for existing instance
        if instance_id:
            result = run(['aws', 'ec2', 'describe-instances',
                          '--instance-ids', instance_id,
                          '--region', args.region,
                          '--query', 'Reservations[0].Instances[0].PublicIpAddress',
                          '--output', 'text'], capture=True)
            ip = result.stdout.strip()
            print(f'Using existing instance: {instance_id} ({ip})')

        if args.download_only:
            if not ip:
                print('Error: --download-only requires --instance-id')
                sys.exit(1)
            download_results(ip, args.key_path)
            return

        # Build and package
        if not args.skip_build:
            archive_path = build_and_package(args)
        else:
            archive_path = PROJECT_ROOT / ARCHIVE_NAME
            if not archive_path.exists():
                print(f'Error: {archive_path} not found. Run without --skip-build first.')
                sys.exit(1)
            print(f'Using existing archive: {archive_path}')

        # Launch instance if needed
        if not instance_id:
            sg_id = ensure_security_group(args)
            instance_id, ip = launch_instance(args, sg_id)

        # Wait for SSH and deploy
        wait_for_ssh(ip, args.key_path)
        upload_and_extract(ip, args.key_path, archive_path)

        # Start worker on EC2
        start_worker(ip, args.key_path, args)

        # Run local client (connects to Showdown from home IP, forwards to EC2)
        run_local_client(ip, args)

        # Download worker logs from EC2 (battle logs are already local)
        download_worker_logs(ip, args.key_path)

    except KeyboardInterrupt:
        print('\n\nInterrupted by user.')
        if ip:
            print('Downloading worker logs...')
            download_worker_logs(ip, args.key_path)

    finally:
        if instance_id and not args.no_terminate and not args.instance_id:
            terminate_instance(instance_id, args.region)
        elif instance_id:
            print(f'\nInstance still running: {instance_id} ({ip})')
            print(f'  SSH: ssh -i {args.key_path} ec2-user@{ip}')
            print(f'  Terminate: aws ec2 terminate-instances --instance-ids {instance_id}')


if __name__ == '__main__':
    main()
