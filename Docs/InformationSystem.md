# Information System

## Overview

The information system controls what the AI knows about the opponent during battle. In competitive Pokemon, players have limited information — they can't see the opponent's full moveset, item, ability, or EV spread until those are revealed through gameplay actions. The AI must respect these constraints to play realistically.

Two formats are supported:

- **Open Team Sheets (OTS)**: Both players see each other's full team at team preview, including moves, abilities, items, and tera types. Used in best-of-3 tournament play.
- **Closed Team Sheets (CTS)**: Only species are visible at team preview. Moves, abilities, items, and tera types are hidden until revealed during battle. Used in single-game ladder play.

## Architecture

```
Battle Events (moves used, abilities triggered, items consumed, etc.)
     │
     ▼
BattleInfoTracker
     │  Maintains per-opponent-Pokemon knowledge:
     │  - RevealedMoves (HashSet<MoveId>)
     │  - RevealedAbility (AbilityId?)
     │  - RevealedItem (ItemId?)
     │  - RevealedTeraType (MoveType?)
     │  - HasBeenActive (bool)
     │
     ├──→ HeuristicEval.Evaluate(battle, sideId, tracker)
     │      Only considers revealed opponent moves when computing
     │      type matchup advantage. Own moves always fully known.
     │
     ├──→ EnsembleEvaluator.ScoreEdges(battle, sideId, edges, oppPred, tracker)
     │      Passes tracker to all 8 mini-models.
     │      Mini-models filter opponent move access through tracker.
     │
     └──→ BattleInfoTracker.FilterPerspective(perspective)
            Creates a filtered BattlePerspective where unrevealed
            opponent info is masked (for DL model input encoding).
```

## Key Design Decisions

### Simulation vs Evaluation

MCTS simulation needs full information to advance the game state — `oppSide.AutoChoose()` must know the opponent's actual moves to generate legal game states. The information restriction applies only to **evaluation** (heuristic scoring and mini-model priors), not to the simulation itself.

This means MCTS explores what the opponent *could* do (based on their real moves), but evaluates positions based only on what we *know* they can do. This is a reasonable approximation — the proper solution (Information Set MCTS with determinization) is planned as future work.

### Backward Compatibility

All tracker parameters are nullable with default `null`. When no tracker is provided, all code paths use full observability — identical to the pre-tracker behavior. This means:

- Bot-vs-bot evaluation continues to work unchanged (`tracker=null`)
- Only Showdown/ladder play needs to create and maintain a tracker
- The tracker can be enabled/disabled per-battle for A/B testing

### Move Filtering

The core filtering mechanism is `BattleInfoTracker.FilterMovesForEval()`:

```csharp
public static IEnumerable<MoveSlot> FilterMovesForEval(
    Pokemon pokemon, bool isOpponent, BattleInfoTracker? tracker)
```

- Own Pokemon (`isOpponent=false`): always returns all moves
- Opponent with `tracker=null`: returns all moves (full observability)
- Opponent with tracker: returns only moves in `RevealedMoves`
- Opponent with tracker but no revealed moves: returns empty (conservative)

This is used by `HeuristicEval.BestMoveScore()` and can be used by mini-models for opponent threat assessment.

## Components

### BattleInfoTracker (`ApogeeVGC/Mcts/BattleInfoTracker.cs`)

The central tracker class. One instance per battle, owned by the player.

**Initialization:**
- Auto-initializes from the first `BattlePerspective` received
- Records all opponent species from team preview
- Builds reverse lookup tables (move name → MoveId, etc.) from the Library

**Event Processing:**
```csharp
public void ProcessEvents(IEnumerable<BattleEvent> events)
```

Called from the player's `UpdateEvents()` method. Processes these message types:

| Message Type | What's Revealed |
|---|---|
| `MoveUsedMessage` | Opponent move added to `RevealedMoves` |
| `AbilityMessage` | Opponent ability recorded |
| `ItemMessage` | Opponent item recorded |
| `EndItemMessage` | Opponent item recorded + marked as consumed |
| `TerastallizeMessage` | Opponent tera type recorded |
| `SwitchMessage` | Opponent Pokemon marked as `HasBeenActive` |

**Perspective Filtering:**
```csharp
public BattlePerspective FilterPerspective(BattlePerspective perspective)
```

Creates a copy of the perspective with unrevealed opponent info masked:
- Unrevealed moves → `MoveId.None`
- Unrevealed ability → `AbilityId.None`
- Unrevealed item → `ItemId.None`
- Unrevealed tera → `MoveType.Unknown`
- Opponent stats → zeroed (never directly observable)

### RevealedPokemonInfo (`ApogeeVGC/Mcts/BattleInfoTracker.cs`)

Per-Pokemon tracking record:

```csharp
public sealed class RevealedPokemonInfo(SpecieId species)
{
    public SpecieId Species { get; }
    public HashSet<MoveId> RevealedMoves { get; }
    public AbilityId? RevealedAbility { get; set; }
    public ItemId? RevealedItem { get; set; }
    public bool ItemConsumed { get; set; }
    public MoveType? RevealedTeraType { get; set; }
    public bool HasBeenActive { get; set; }
}
```

### Integration Points

#### HeuristicEval

```csharp
public static float Evaluate(Battle battle, SideId sideId, BattleInfoTracker? tracker = null)
```

When a tracker is provided, `BestMoveScore()` only iterates revealed opponent moves. If no opponent moves are known, the matchup advantage term defaults to 0 (neutral assumption).

#### Mini-Models

All 8 mini-models accept `BattleInfoTracker? tracker = null` in their `Evaluate()` method. Models that assess opponent threats (DamageMin, KOAvoidance, KOSeeking) should filter through `BattleInfoTracker.FilterMovesForEval()` when evaluating what the opponent can do to us.

#### MCTS Search Classes

Both `MctsSearchStandalone` and `MctsSearchEnsemble` have a `Tracker` property:

```csharp
public BattleInfoTracker? Tracker { get; set; }
```

Set by the player before each search. Passed to `HeuristicEval.Evaluate()` for leaf evaluation and to `EnsembleEvaluator.ScoreEdges()` for root priors.

## What's Known Under Each Format

### OTS (Open Team Sheets)

| Information | When Known |
|---|---|
| Species (all 6) | Team preview |
| Moves (all 4 per Pokemon) | Team preview |
| Ability | Team preview |
| Item | Team preview |
| Tera type | Team preview |
| EV spread / Nature | Never (hidden) |
| Which 4 brought | After team preview |
| HP % | Always visible in battle |
| Status / Boosts / Volatiles | Always visible in battle |

### CTS (Closed Team Sheets)

| Information | When Known |
|---|---|
| Species (all 6) | Team preview (names visible) |
| Moves | As used during battle |
| Ability | When triggered during battle |
| Item | When activated/consumed/revealed |
| Tera type | When terastallized |
| EV spread / Nature | Never (hidden) |
| Which 4 brought | As they enter battle |
| HP % | Always visible in battle |
| Status / Boosts / Volatiles | Always visible in battle |

## Usage

### Bot-vs-Bot Evaluation (Full Observability)

No tracker needed. All code defaults to full observability:

```csharp
// Existing evaluation code — unchanged
HeuristicEval.Evaluate(battle, sideId);  // tracker=null → full info
```

### Showdown Ladder Play (CTS)

```csharp
// Create tracker at battle start
var tracker = new BattleInfoTracker(mySideId, library);

// Process events each turn
public void UpdateEvents(IEnumerable<BattleEvent> events)
{
    tracker.ProcessEvents(events);
}

// Set tracker on search before each decision
search.Tracker = tracker;
var (actionA, actionB) = search.Search(battle, sideId, legalActions);
```

### Showdown Tournament Play (OTS)

```csharp
// Create tracker and initialize from team preview
var tracker = new BattleInfoTracker(mySideId, library);
// After team preview, all info is already visible in the perspective
// The tracker will auto-initialize from ProcessEvents
```

## Future Work

### Information Set MCTS (ISMCTS)

The current approach restricts evaluation but simulates with full information. A more rigorous approach would use **determinization**: sample plausible opponent states consistent with revealed information, then run MCTS on each determinization independently.

For example, if the opponent has only revealed 2 of 4 moves, we would:
1. Sample possible remaining 2 moves from the metagame move pool
2. Create K determinized battles with different sampled movesets
3. Run MCTS on each determinization
4. Aggregate visit counts across all K trees

This is described in `Docs/AI_System.md` under "Imperfect Information (Future)".

### Logical Deduction Engine

Certain game events allow deducing hidden information with certainty:
- Pokemon outspeeds when it shouldn't → Choice Scarf
- 100% accuracy move misses without evasion → Bright Powder
- Two different moves used → not Choice-locked

These deductions are documented in `Docs/information_model.md` and could be integrated as additional signals to the tracker.
