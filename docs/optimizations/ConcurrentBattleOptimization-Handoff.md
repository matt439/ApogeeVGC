# Concurrent Battle Optimization — Handoff Document

## Context

Target method: `Driver.RunRndVsRndVgcRegIEvaluation()` in `Sim/Core/Driver.Evaluation.cs`
Runs 100,000 VGC Reg I battles via `Parallel.For` with 32 threads.

Benchmark artifact: `BattleSimulationBenchmark.RunSingleBattle` in `BenchmarkSuite1/BattleSimulationBenchmark.cs`

---

## Completed Work

### 1. Cached `Enum.GetValues<T>()` in `EventHandlerInfoMapper`

**File**: `Sim/Events/EventHandlerInfoMapper.cs` (lines 14–17)

**What**: Replaced 6 `Enum.GetValues<EventId>()`, `Enum.GetValues<EventPrefix>()`, `Enum.GetValues<EventSuffix>()` calls inside the triple-nested loops of `BuildHandlerCache` and `BuildMoveHandlerCache` with 3 static cached arrays (`CachedEventIds`, `CachedEventPrefixes`, `CachedEventSuffixes`).

**Why**: Each `Enum.GetValues<T>()` allocates a new array via reflection on every call. The triple-nested loops caused ~841 allocations per cache build × every effect/move × every battle.

**Result**: Single-battle benchmark improved from **15.85 ms → 6.346 ms (2.50x speedup)**.

### 2. Per-effect `FrozenDictionary` handler caches (pre-existing)

The `GetHandlerInfo-CachingPlan.md` steps 1–8 were already implemented before this optimization pass:
- `BuildHandlerCache(IEffect)` and `BuildMoveHandlerCache(IMoveEventMethods)` exist
- Each effect type (`Ability`, `Item`, `Condition`, `Format`, `ActiveMove`) has a lazy `_handlerCache` field
- `HasAnyEventHandlers` is implemented on all `IEffect` types
- `Species.HasAnyEventHandlers` returns `false` (early skip)
- SwitchIn→OnStart fallback is pre-computed in `BuildHandlerCache`

---

## Remaining Bottlenecks (Post-Optimization Profile)

Profiled via `BattleSimulationBenchmark.RunSingleBattle` at 6.346 ms/battle:

| Rank | Function | Total CPU | Self CPU | Category |
|------|----------|-----------|----------|----------|
| 1 | `EventHandlerInfoMapper.BuildMoveHandlerCache` | 46.22% | 38.7% | Cache rebuild per ActiveMove |
| 2 | `Battle.FindEventHandlers` | 22.0% | 2.76% | Event dispatch (orchestration) |
| 3 | `Battle.FindPokemonEventHandlers` | 17.84% | 13.3% | Per-Pokemon handler discovery |
| 4 | `Battle.TurnLoop` | 73.07% | 0.09% | Orchestrator (not a bottleneck itself) |

---

## Optimization Opportunities (Ordered by Impact)

### OP-1: Lift `ActiveMove` handler cache to base `Move` (HIGH — ~46% CPU)

**Problem**: `ActiveMove._handlerCache` is built per-instance via lazy init. `ActiveMove` is created per move usage per battle (via `Move.ToActiveMove()` in `BattleActions.Moves.cs:69`). Each `BuildMoveHandlerCache` call iterates 140 EventIds × 5 Prefixes × 3 Suffixes = 2,100 combinations and builds a `FrozenDictionary`.

**Key insight**: `ActiveMove` inherits all event handler properties from `Move` via `init`-only properties. The `ToActiveMove()` method copies every handler (`OnHit`, `OnTryHit`, `OnModifyMove`, etc.) directly from the base `Move`. Since `Move` instances are immutable library singletons, the handler cache is identical for every `ActiveMove` created from the same `Move`.

**Complication**: `Move.OnHit` has `{ get; set; }` (mutable), and `ActiveMove` could have its `OnHit` changed after creation. Check all call sites that mutate `ActiveMove.OnHit` to determine if invalidation is needed.

**Proposed approach**:
1. Add a lazy `_moveHandlerCache` field to `Move` (same pattern as Ability/Item/Condition)
2. In `ActiveMove`, initialize `_handlerCache` from the base `Move`'s cache in `ToActiveMove()`
3. If `ActiveMove.OnHit` (or any handler) is mutated post-creation, invalidate the cache (set `_handlerCache = null` so it rebuilds lazily)
4. Alternative: if handler mutation is rare, track it with a `bool _handlersDirty` flag

**Files to modify**:
- `Sim/Moves/Move.MoveEventMethods.cs` — add `_moveHandlerCache` lazy field
- `Sim/Moves/Move.Core.cs` — wire cache in `ToActiveMove()`, share with `ActiveMove`
- `Sim/Moves/ActiveMove.cs` — accept pre-built cache, add invalidation on setter if needed
- `Sim/Events/EventHandlerInfoMapper.cs` — potentially add `BuildMoveHandlerCache(Move)` overload

**Expected impact**: Eliminates ~46% of per-battle CPU. Cache is built once per unique `Move` in the Library (~900 moves) instead of once per move usage per battle.

---

### OP-2: Reduce `FindPokemonEventHandlers` allocation pressure (MEDIUM — ~17.8% CPU)

**Problem**: `FindPokemonEventHandlers` creates `EventListenerWithoutPriority` record instances with closure-based `End` delegates on every call. Each status/volatile/ability/item/slotCondition check allocates:
- A `new EventListenerWithoutPriority { ... }` record
- Lambda closures for `End` delegates: `new Action<bool>(_ => pokemon.ClearStatus())`, `(Func<Condition, bool>)pokemon.RemoveVolatile`, etc.
- `List<object>` for `EndCallArgs`

In doubles, a single `RunEvent` call triggers `FindPokemonEventHandlers` ~7+ times (self + allies × {Ally,Any} + foes × {Foe,Any} + source). Each call iterates status + N volatiles + ability + item + species + M slot conditions.

**Possible approaches**:
- **Pool `EventListener` objects** using `ObjectPool<T>` or a thread-local pool, resetting and returning after each `RunEvent` cycle
- **Cache `End` delegates** on the Pokemon or effect type instead of creating closures per-call (e.g., `pokemon.ClearStatusDelegate` as a cached `EffectDelegate`)
- **Use `stackalloc` or `ArrayPool`** for the `handlers` list in `FindEventHandlers` since its lifetime is bounded to the `RunEvent` call
- **Skip null-handler checks early**: if `handlerInfo == null && getKey == null`, skip the allocation entirely (the current code already partially does this but still enters branches)

**Files to modify**:
- `Sim/BattleClasses/Battle.EventHandlers.cs` — `FindPokemonEventHandlers`, `FindEventHandlers`
- `Sim/Events/EventListenerWithoutPriority.cs` — potentially make it a `class` instead of `record` for pooling
- `Sim/PokemonClasses/Pokemon.cs` or related — cache `End` delegates

---

### OP-3: Reduce `FindEventHandlers` dispatch overhead (MEDIUM — ~22% CPU, 2.76% self)

**Problem**: `FindEventHandlers` calls `FindPokemonEventHandlers` multiple times per event for prefix variants. In doubles with a Pokemon target:
- 1× self (no prefix)
- 1× per ally with Ally prefix + 1× per ally with Any prefix
- 1× per foe with Foe prefix + 1× per foe with Any prefix
- 1× source with Source prefix
- Plus side conditions, field, and battle handlers

Total: ~7–11 calls to `FindPokemonEventHandlers` for a single event.

**Possible approaches**:
- **Batch prefix lookups**: Modify `FindPokemonEventHandlers` to accept multiple prefixes and check them in a single pass over the Pokemon's effects (one iteration instead of 7)
- **Pre-filter events**: Most events don't have Ally/Foe/Any/Source handlers on most effects. Add per-effect bitflags (`HasFoeHandlers`, `HasAnyHandlers`, etc.) to skip prefix variants when no effect on the field implements them
- **Event-specific fast paths**: For common events that never have prefixed handlers (like `BeforeTurn`, `Update`), the `prefixedHandlers` flag already exists but could be extended

**Files to modify**:
- `Sim/BattleClasses/Battle.EventHandlers.cs` — `FindEventHandlers`, `FindPokemonEventHandlers`
- `Sim/Effects/IEffect.cs` — potentially add prefix-aware bitflags

---

### OP-4: `ToFrozenDictionary()` cost in cache building (LOW-MEDIUM)

**Problem**: Part of the 38.7% self-time in `BuildMoveHandlerCache` comes from `ToFrozenDictionary()`, which sorts and optimizes the dictionary for read performance. If OP-1 is implemented (cache shared from `Move`), this cost is amortized across all battles and becomes negligible.

**Note**: Only relevant if OP-1 is NOT pursued. If OP-1 is done, this is automatically resolved.

---

### OP-5: Concurrent data structure overhead in `RunRndVsRndVgcRegIEvaluation` (LOW)

**Problem**: `ConcurrentBag<SimulatorResult>`, `ConcurrentBag<int>`, and 4× `ConcurrentDictionary` for coverage tracking. These involve lock-free synchronization overhead.

**Assessment**: Profiling showed these are NOT significant bottlenecks. The battle simulation itself dominates. However, if all other optimizations reduce per-battle time significantly, these could become visible.

**Possible future approaches**:
- Replace `ConcurrentBag` with thread-local lists + merge after `Parallel.For` completes
- Replace `ConcurrentDictionary` coverage counters with thread-local dictionaries + merge
- Use `Parallel.ForEach` with `localInit`/`localFinally` pattern for aggregation

---

## Recommended Priority Order

1. **OP-1** (Lift ActiveMove cache to Move) — highest impact, eliminates ~46% CPU
2. **OP-2** (Reduce FindPokemonEventHandlers allocations) — medium impact, reduces GC pressure
3. **OP-3** (Batch prefix lookups) — medium impact, reduces dispatch overhead
4. OP-4 is auto-resolved by OP-1
5. OP-5 is low priority

---

## Existing Reference Material

- `docs/optimizations/GetHandlerInfo-CachingPlan.md` — detailed analysis of the handler cache architecture (steps 1–8 already implemented)

## Key Files Map

| File | Role |
|------|------|
| `Sim/Core/Driver.Evaluation.cs` | `RunRndVsRndVgcRegIEvaluation` — the parallel evaluation method |
| `Sim/Core/Driver.Helpers.cs` | `RunBattleWithPrebuiltTeamsDirect` — per-battle entry point |
| `Sim/Core/SyncSimulator.cs` | Battle simulation loop |
| `Sim/Events/EventHandlerInfoMapper.cs` | `BuildHandlerCache`, `BuildMoveHandlerCache`, static enum maps |
| `Sim/BattleClasses/Battle.EventHandlers.cs` | `GetHandlerInfo`, `FindEventHandlers`, `FindPokemonEventHandlers` |
| `Sim/Moves/Move.Core.cs` | `Move` record, `ToActiveMove()` |
| `Sim/Moves/Move.MoveEventMethods.cs` | Event handler properties on `Move` (`OnHit` is `{ get; set; }`) |
| `Sim/Moves/ActiveMove.cs` | `ActiveMove` record, per-instance `_handlerCache` |
| `Sim/Abilities/Ability.Core.cs` | Lazy `_handlerCache` pattern (reference impl) |
| `Sim/Items/Item.Core.cs` | Lazy `_handlerCache` pattern (reference impl) |
| `Sim/Conditions/Condition.Core.cs` | Lazy `_handlerCache` pattern (reference impl) |
| `Sim/Effects/IEffect.cs` | `IEffect` interface with `HasAnyEventHandlers`, `GetEventHandlerInfo` |
| `Sim/Events/EventListenerWithoutPriority.cs` | Per-event allocation (target for OP-2) |
| `BenchmarkSuite1/BattleSimulationBenchmark.cs` | Existing benchmark for single battle |

## Benchmark Baseline

```
| Method          | Mean     | Error     | StdDev    |
|---------------- |---------:|----------:|----------:|
| RunSingleBattle | 6.346 ms | 0.0320 ms | 0.0284 ms |
```

Run with: `run_benchmark(["BattleSimulationBenchmark.RunSingleBattle"], "CPU")`
