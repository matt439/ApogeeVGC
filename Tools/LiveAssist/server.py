"""
Live Assist WebSocket Server

Receives battle protocol messages from the Tampermonkey userscript,
maintains battle state, runs ONNX inference, and sends recommendations
back to the overlay. Also displays a rich terminal UI.

Usage:
    python server.py gen9vgc2025regi
    python server.py gen9vgc2025regi --port 9876

Files are resolved relative to the repo root (Tools/DLModel/):
    vocab:         Tools/DLModel/battle_model_vocab.json
    battle model:  Tools/DLModel/battle_model.onnx
    preview model: Tools/DLModel/team_preview_model.onnx
"""

from __future__ import annotations

import argparse
import asyncio
import json
import sys
from pathlib import Path

import numpy as np
import onnxruntime as ort
import websockets
from rich.console import Console
from rich.panel import Panel
from rich.table import Table
from rich.text import Text

from live_parser import LiveBattleState
from live_encoder import LiveEncoder

console = Console()


# ── Masked softmax (matches C# ModelInference.MaskedSoftmax) ─────────────────

def masked_softmax(logits: np.ndarray, legal_mask: np.ndarray) -> np.ndarray:
    """Apply softmax to logits, masked to only legal actions."""
    result = np.zeros_like(logits)
    legal_indices = np.where(legal_mask)[0]
    if len(legal_indices) == 0:
        return result
    legal_logits = logits[legal_indices]
    max_val = np.max(legal_logits)
    exp_vals = np.exp(legal_logits - max_val)
    total = np.sum(exp_vals)
    if total > 0:
        result[legal_indices] = exp_vals / total
    return result


# ── Terminal display ─────────────────────────────────────────────────────────

def display_state(state: LiveBattleState) -> None:
    """Display current battle state in the terminal."""
    if not state.my_side:
        return

    table = Table(title=f"Turn {state.current_turn}", show_header=True)
    table.add_column("Slot", style="bold")
    table.add_column("Species")
    table.add_column("HP")
    table.add_column("Status")
    table.add_column("Boosts")
    table.add_column("Tera")

    for slot in sorted(state.active.keys()):
        poke = state.active[slot]
        side = "MY" if slot.startswith(state.my_side) else "OPP"
        style = "green" if slot.startswith(state.my_side) else "red"

        hp_str = f"{poke.hp}/{poke.max_hp}"
        if poke.fainted:
            hp_str = "FAINTED"

        boosts = ", ".join(f"{k}:{v:+d}" for k, v in poke.boosts.items() if v != 0)
        tera = poke.tera_type or ""

        table.add_row(
            f"[{style}]{side} {slot[-1].upper()}[/{style}]",
            poke.species,
            hp_str,
            poke.status or "",
            boosts or "-",
            tera,
        )

    console.print(table)

    # Field conditions
    field = state.field
    conditions = []
    if field.weather:
        conditions.append(f"Weather: {field.weather}")
    if field.terrain:
        conditions.append(f"Terrain: {field.terrain}")
    if field.trick_room:
        conditions.append("Trick Room")
    for side_label, side_id in [("My", state.my_side), ("Opp", state.opp_side)]:
        for screen in ("tailwind", "reflect", "light_screen", "aurora_veil"):
            if getattr(field, f"{side_id}_{screen}", False):
                conditions.append(f"{side_label} {screen.replace('_', ' ').title()}")
    if conditions:
        console.print(f"  Field: {', '.join(conditions)}")


def display_recommendations(
    value: float,
    slot_a_actions: list[dict],
    slot_b_actions: list[dict],
) -> None:
    """Display model recommendations in the terminal."""
    # Value bar
    pct = value * 100
    color = "green" if pct >= 55 else "yellow" if pct >= 45 else "red"
    console.print(f"\n  Win probability: [{color}]{pct:.1f}%[/{color}]")

    for label, actions in [("Slot A", slot_a_actions), ("Slot B", slot_b_actions)]:
        table = Table(title=label, show_header=True, title_style="bold cyan")
        table.add_column("Action")
        table.add_column("Prob", justify="right")

        for a in actions:
            prob = a["prob"] * 100
            style = "bold green" if prob >= 50 else "green" if prob >= 20 else "dim"
            table.add_row(f"[{style}]{a['action']}[/{style}]", f"{prob:.1f}%")

        console.print(table)


def display_team_preview(
    own_team: list[dict],
    bring_scores: list[float],
    lead_scores: list[float],
) -> None:
    """Display team preview recommendations."""
    table = Table(title="Team Preview", show_header=True)
    table.add_column("Pokemon")
    table.add_column("Bring", justify="right")
    table.add_column("Lead", justify="right")

    # Sort by bring score descending
    indexed = [(i, own_team[i], bring_scores[i], lead_scores[i])
               for i in range(min(len(own_team), 6))]
    indexed.sort(key=lambda x: x[2], reverse=True)

    for i, poke, bring, lead in indexed:
        bring_pct = bring * 100
        lead_pct = lead * 100
        style = "bold green" if bring_pct >= 70 else "green" if bring_pct >= 50 else "dim"
        table.add_row(
            f"[{style}]{poke.get('species', '?')}[/{style}]",
            f"{bring_pct:.0f}%",
            f"{lead_pct:.0f}%",
        )

    console.print(table)


# ── Server ───────────────────────────────────────────────────────────────────

class LiveAssistServer:
    def __init__(
        self,
        encoder: LiveEncoder,
        battle_session: ort.InferenceSession,
        preview_session: ort.InferenceSession,
    ):
        self.encoder = encoder
        self.battle_session = battle_session
        self.preview_session = preview_session

    async def handler(self, websocket):
        state = LiveBattleState()
        console.print("[bold green]Client connected[/bold green]")

        try:
            async for raw in websocket:
                msg = json.loads(raw)
                msg_type = msg.get("type")

                if msg_type == "battle":
                    lines = msg["data"].split("\n")
                    state.update(lines)

                elif msg_type == "request":
                    request = msg["data"]
                    state.update_request(request)

                    if state.phase == "teampreview":
                        await self._handle_team_preview(websocket, state)
                    elif state.phase == "battle":
                        await self._handle_battle(websocket, state, request)

        except websockets.ConnectionClosed:
            console.print("[yellow]Client disconnected[/yellow]")

    async def _handle_team_preview(self, websocket, state: LiveBattleState) -> None:
        """Run team preview inference and send recommendations."""
        try:
            inputs = self.encoder.encode_team_preview(state)
            outputs = self.preview_session.run(None, inputs)

            # TeamPreviewNet outputs: bring_scores[6], lead_scores[6]
            bring_raw = outputs[0][0]  # [6]
            lead_raw = outputs[1][0]   # [6]

            # Sigmoid to get probabilities
            bring_scores = 1.0 / (1.0 + np.exp(-bring_raw))
            lead_scores = 1.0 / (1.0 + np.exp(-lead_raw))

            # Display in terminal
            own_team = [{"species": p.species} for p in state.own_team]
            display_team_preview(own_team, bring_scores.tolist(), lead_scores.tolist())

            # Send to overlay
            pokemon = []
            for i, poke in enumerate(state.own_team[:6]):
                pokemon.append({
                    "species": poke.species,
                    "bringScore": float(bring_scores[i]) if i < len(bring_scores) else 0,
                    "leadScore": float(lead_scores[i]) if i < len(lead_scores) else 0,
                })

            await websocket.send(json.dumps({
                "type": "team_preview",
                "pokemon": pokemon,
            }))

        except Exception as e:
            console.print(f"[red]Team preview error: {e}[/red]")

    async def _handle_battle(self, websocket, state: LiveBattleState, request: dict) -> None:
        """Run battle inference and send recommendations."""
        try:
            inputs = self.encoder.encode_battle(state)
            mask_a, mask_b = self.encoder.build_action_mask(request)

            outputs = self.battle_session.run(None, inputs)

            value = float(outputs[0][0])
            policy_a_logits = outputs[1][0]
            policy_b_logits = outputs[2][0]

            probs_a = masked_softmax(policy_a_logits, mask_a)
            probs_b = masked_softmax(policy_b_logits, mask_b)

            slot_a_actions = self.encoder.decode_actions(probs_a)
            slot_b_actions = self.encoder.decode_actions(probs_b)

            # Display in terminal
            display_state(state)
            display_recommendations(value, slot_a_actions, slot_b_actions)

            # Send to overlay
            await websocket.send(json.dumps({
                "type": "recommendation",
                "value": value,
                "slotA": [{"action": a["action"], "prob": a["prob"]} for a in slot_a_actions],
                "slotB": [{"action": a["action"], "prob": a["prob"]} for a in slot_b_actions],
            }))

        except Exception as e:
            console.print(f"[red]Battle inference error: {e}[/red]")


def resolve_paths(format_id: str) -> tuple[Path, Path, Path]:
    """Resolve vocab, battle model, and preview model paths from a format ID.

    Looks in Tools/DLModel/ relative to the repo root (two levels up from this file).
    """
    repo_root = Path(__file__).resolve().parent.parent.parent
    dl_dir = repo_root / "Tools" / "DLModel"

    vocab_path = dl_dir / "battle_model_vocab.json"
    battle_path = dl_dir / "battle_model.onnx"
    preview_path = dl_dir / "team_preview_model.onnx"

    for name, path in [("Vocab", vocab_path), ("Battle model", battle_path), ("Preview model", preview_path)]:
        if not path.exists():
            console.print(f"[red]{name} not found: {path}[/red]")
            sys.exit(1)

    return vocab_path, battle_path, preview_path


async def main():
    parser = argparse.ArgumentParser(description="Live Assist WebSocket Server")
    parser.add_argument("format", help="Showdown format ID (e.g. gen9vgc2025regi)")
    parser.add_argument("--host", default="localhost")
    parser.add_argument("--port", type=int, default=9876)
    args = parser.parse_args()

    vocab_path, battle_path, preview_path = resolve_paths(args.format)

    console.print(Panel.fit(
        f"[bold]Apogee VGC Live Assist[/bold]\n"
        f"Format: {args.format}\n"
        f"Listening on ws://{args.host}:{args.port}",
        border_style="blue",
    ))

    console.print("Loading models...")
    encoder = LiveEncoder(str(vocab_path))

    session_opts = ort.SessionOptions()
    session_opts.graph_optimization_level = ort.GraphOptimizationLevel.ORT_ENABLE_ALL

    battle_session = ort.InferenceSession(str(battle_path), session_opts)
    preview_session = ort.InferenceSession(str(preview_path), session_opts)
    console.print("[green]Models loaded[/green]")

    server = LiveAssistServer(encoder, battle_session, preview_session)

    async with websockets.serve(server.handler, args.host, args.port):
        console.print(f"[bold green]Server ready — waiting for connection...[/bold green]")
        await asyncio.Future()  # run forever


if __name__ == "__main__":
    asyncio.run(main())
