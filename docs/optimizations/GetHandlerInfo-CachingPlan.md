# Optimize GetHandlerInfo Event Handler Resolution (26.61% CPU)

## Problem

The event handler resolution chain `GetHandlerInfo → GetEventHandlerInfo → EventHandlerInfoMapper.GetEventHandlerInfo → FrozenDictionary.TryGetValue` is the single largest CPU hotspot at **26.61%**.

## Root Cause

### Extreme call volume

`FindPokemonEventHandlers` (`Battle.EventHandlers.cs:271`) is called **per effect, per Pokemon, per prefix variant**. A single `FindEventHandlers` call for one Pokemon target triggers:

- **Self**: status + N volatiles + ability + item + species + M slot conditions = **5+N+M** `GetHandlerInfo` calls
- **Per ally** (×Ally prefix, ×Any prefix): same breakdown × 2 prefixes × number of allies
- **Per foe** (×Foe prefix, ×Any prefix): same breakdown × 2 prefixes × number of foes
- **Source Pokemon** (×Source prefix): same breakdown
- **Plus** Side conditions, Field (pseudo-weather + weather + terrain), Battle (format + events)

**In a doubles battle: easily 100+ `GetHandlerInfo` invocations per single `RunEvent` call.**

### 3-step indirection per lookup

Each `GetHandlerInfo` call does:

1. `FrozenDictionary<EventId, Func<...>>.TryGetValue` — hash + compare on `EventId`
2. Lambda/delegate invocation — property getter through `Func<IEventMethods, EventHandlerInfo?>`
3. `MatchesPrefixAndSuffix` validation — redundant check given map partitioning

### ~90%+ of lookups return null

Most effects implement only 1–10 of ~140 events, so the vast majority of lookups produce no useful result.

### SwitchIn special case multiplies lookups

For ability/item effects during `SwitchIn`, up to **3 lookups** are needed in `GetHandlerInfo` (lines 26–67):
1. Main handler lookup
2. `AnySwitchIn` existence check (to decide fallback)
3. `Start` handler fallback

## Key Insight: Effects Are Immutable Library Singletons

All effect types flowing through `FindEventHandlers` are **immutable `record` types** created once at Library initialization:

| Type | Source | Mutates? |
|------|--------|----------|
| `Ability` | `Library.Abilities[id]` | No (`record` with `init` props) |
| `Item` | `Library.Items[id]` | No (`record` with `init` props) |
| `Condition` | `Library.Conditions[id]` | No (`record` with `init` props) |
| `Format` | `Library.Format` | No (`record` with `init` props) |
| `Species` | `Library.Species[id]` | No (always returns null) |
| `ActiveMove` | Per-battle, but event handlers are from `init` props | Handlers don't change |

Since their event handler registrations **never change after construction**, per-effect caching is perfectly safe.

## Architecture Overview

### Current call chain (hot path)

```
Battle.RunEvent(eventId, target, source, ...)
  → FindEventHandlers(target, eventId, source)
    → FindPokemonEventHandlers(handlers, pokemon, eventId, prefix)
      → GetHandlerInfo(pokemon, effect, eventId, prefix, suffix)      // ×5+N+M per Pokemon
        → effect.GetEventHandlerInfo(eventId, prefix?, suffix?)       // virtual dispatch
          → EventHandlerInfoMapper.GetEventHandlerInfo(effect, id, prefix, suffix)
            → FrozenDictionary.TryGetValue(id, out accessor)          // hash + compare
            → accessor(eventMethods)                                   // lambda invocation
            → MatchesPrefixAndSuffix(info, prefix, suffix)            // redundant check
```

### Key files

| File | Role |
|------|------|
| `Sim/BattleClasses/Battle.EventHandlers.cs` | `GetHandlerInfo`, `FindEventHandlers`, `FindPokemonEventHandlers` |
| `Sim/Events/EventHandlerInfoMapper.cs` | Static mapper with 6 `FrozenDictionary` maps (base, Foe, Source, Any, Ally, Ability, Move) |
| `Sim/Effects/IEffect.cs` | `IEffect` interface with `GetEventHandlerInfo` method |
| `Sim/Abilities/Ability.Core.cs` | `Ability` record, delegates to `EventHandlerInfoMapper` |
| `Sim/Items/Item.Core.cs` | `Item` record, delegates to `EventHandlerInfoMapper` |
| `Sim/Conditions/Condition.Core.cs` | `Condition` record, delegates to `EventHandlerInfoMapper` |
| `Sim/FormatClasses/Format.Core.cs` | `Format` record, delegates to `EventHandlerInfoMapper` |
| `Sim/Moves/ActiveMove.cs` | `ActiveMove`, delegates to `EventHandlerInfoMapper` |
| `Sim/SpeciesClasses/Species.cs` | Always returns null (no event handlers) |
| `Data/Library.cs` | Creates and stores all effect singletons |

### Relevant enums

- `EventId` — ~140 values (events like `SwitchIn`, `BasePower`, `ModifyDamage`, etc.)
- `EventPrefix` — 5 values: `None`, `Ally`, `Foe`, `Source`, `Any`
- `EventSuffix` — 3 values: `None`, `SwitchIn`, `RedirectTarget`

### EventHandlerInfoMapper structure

The mapper has 6 static `FrozenDictionary` maps partitioned by prefix:
- `EventMethodsMap` — base events (no prefix), keyed by `EventId`
- `FoeEventMethodsMap` — Foe-prefixed events
- `SourceEventMethodsMap` — Source-prefixed events
- `AnyEventMethodsMap` — Any-prefixed events
- `AllyEventMethodsMap` — Ally-prefixed events (only for `IPokemonEventMethods`)
- `AbilityEventMethodsMap` — Ability-specific events (`Start`, `End`, `CheckShow`)
- `MoveEventMethodsMap` — Move-specific events

Each maps `EventId → Func<IEventMethods, EventHandlerInfo?>` (a lambda that reads a property).

## Optimization Strategy

**Replace the 3-step lookup chain with a single pre-computed dictionary lookup per effect.**

For each immutable effect instance, pre-build a `FrozenDictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo>` containing **only non-null entries**. Then `GetEventHandlerInfo` becomes one `TryGetValue` call.

### Cache key design

```csharp
// Use non-nullable enums with None as canonical "no prefix/suffix"
// This avoids Nullable<T> boxing overhead in dictionary keys
(EventId id, EventPrefix prefix, EventSuffix suffix)
```

Callers already use `EventPrefix.None` / `EventSuffix.None` — the conversion to `null` in `GetHandlerInfo` line 30–31 becomes unnecessary.

### Expected cache size

- Per effect: **1–10 entries** (only non-null handlers stored)
- Total across Library: ~300 effects × ~5 entries avg = ~1,500 dictionary entries
- Memory: negligible (< 100KB total)

---

## Implementation Steps

### Step 1: Add cache infrastructure to EventHandlerInfoMapper

**File**: `Sim/Events/EventHandlerInfoMapper.cs`

Add a static method that builds the per-effect cache by iterating all `EventId × EventPrefix × EventSuffix` combinations and calling the existing `GetEventHandlerInfo` to find non-null results:

```csharp
public static FrozenDictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo>
    BuildHandlerCache(IEffect effect)
{
    var cache = new Dictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo>();

    foreach (EventId id in Enum.GetValues<EventId>())
    {
        foreach (EventPrefix prefix in Enum.GetValues<EventPrefix>())
        {
            foreach (EventSuffix suffix in Enum.GetValues<EventSuffix>())
            {
                EventPrefix? p = prefix == EventPrefix.None ? null : prefix;
                EventSuffix? s = suffix == EventSuffix.None ? null : suffix;

                EventHandlerInfo? info = GetEventHandlerInfo(effect, id, p, s);
                if (info != null)
                    cache[(id, prefix, suffix)] = info;
            }
        }
    }

    return cache.ToFrozenDictionary();
}
```

### Step 2: Add cache field + HasAnyEventHandlers to IEffect

**File**: `Sim/Effects/IEffect.cs`

```csharp
// Option A: Add to interface (cleanest, but requires all implementations)
bool HasAnyEventHandlers { get; }

// Option B: Convention-based — each implementation adds a private cache field
```

### Step 3: Wire cache into each effect type

**Files**: `Ability.Core.cs`, `Item.Core.cs`, `Condition.Core.cs`, `Format.Core.cs`, `ActiveMove.cs`, `Species.cs`

Each effect gets:
1. A `FrozenDictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo>` field (lazy or eager)
2. Updated `GetEventHandlerInfo` that does `cache.TryGetValue(...)` instead of delegating to `EventHandlerInfoMapper`
3. `HasAnyEventHandlers => cache.Count > 0` (or `=> false` for Species)

**Note on Species**: `Species.GetEventHandlerInfo` already returns `null` unconditionally. Add `HasAnyEventHandlers => false` and skip `GetHandlerInfo` calls for it in `FindPokemonEventHandlers`.

**Note on ActiveMove**: `ActiveMove` is created per battle action but its event handler properties are `init`-only from the move definition. Cache can be built lazily on first access or at construction time. Alternatively, since `ActiveMove` copies handlers from the base `Move` data, the cache could live on the `Move` definition and be shared.

### Step 4: Initialize caches at Library build time

**File**: `Data/Library.cs`

After all effects are created, iterate through all Abilities, Items, Conditions, and the Format to call `BuildHandlerCache` and store the result on each effect.

Approach depends on Step 2/3 design:
- If cache is a mutable field set post-construction: call `effect.InitializeHandlerCache()` after Library init
- If cache is lazy: no explicit init needed, built on first access

### Step 5: Pre-compute SwitchIn→OnStart fallback

**File**: `Sim/BattleClasses/Battle.EventHandlers.cs` (lines 26–67)

The Gen 5+ special case in `GetHandlerInfo`:
```
if no SwitchIn handler AND target is Pokemon AND Gen >= 5
    AND no AnySwitchIn handler AND effect is Ability/Item:
        use OnStart handler with Id = SwitchIn
```

**Two options:**

**Option A (preferred)**: Pre-compute at cache build time. In `BuildHandlerCache`, after the main scan, for each Ability/Item effect: if it has `(Start, None, None)` but not `(SwitchIn, None, None)` and not `(AnySwitchIn, None, None)`, add a synthetic entry:
```csharp
cache[(EventId.SwitchIn, EventPrefix.None, EventSuffix.None)] =
    startHandler with { Id = EventId.SwitchIn };
```
This eliminates the runtime conditional entirely.

**Caveat**: The original check also requires `target is PokemonRunEventTarget` and `Gen >= 5`. Since Gen is always 9 in this codebase, the Gen check is always true. The target-type check is always true when called from `FindPokemonEventHandlers`. So this is safe.

**Option B**: Keep the runtime check but make it faster (single cache lookup instead of 3).

### Step 6: Simplify GetHandlerInfo

**File**: `Sim/BattleClasses/Battle.EventHandlers.cs`

After Steps 1–5, `GetHandlerInfo` becomes:

```csharp
private EventHandlerInfo? GetHandlerInfo(RunEventTarget target, IEffect effect,
    EventId callbackName, EventPrefix prefix = EventPrefix.None,
    EventSuffix suffix = EventSuffix.None)
{
    // Fast path: skip effects with no handlers at all
    if (!effect.HasAnyEventHandlers)
        return null;

    // Single cache lookup replaces the entire chain
    return effect.GetEventHandlerInfo(callbackName, prefix, suffix);
    // Note: SwitchIn→OnStart fallback is now pre-computed in the cache
}
```

The `GetEventHandlerInfo` signature may change to accept non-nullable `EventPrefix`/`EventSuffix` directly, avoiding the `None → null` conversion.

### Step 7: Optimize FindPokemonEventHandlers skip patterns

**File**: `Sim/BattleClasses/Battle.EventHandlers.cs` (lines 271–407)

Add early skips:
```csharp
// Skip species check entirely (never has handlers)
// Line 367: handlerInfo = GetHandlerInfo(pokemon, species, callbackName, prefix);
// → Remove or guard with HasAnyEventHandlers (Species returns false)

// Skip "none" status/item effects
if (status.HasAnyEventHandlers)
{
    // ... existing status handler logic
}
```

### Step 8: Remove MatchesPrefixAndSuffix

**File**: `Sim/Events/EventHandlerInfoMapper.cs`

With the cache keyed by `(EventId, EventPrefix, EventSuffix)`, the prefix/suffix is already validated by the cache key. The `MatchesPrefixAndSuffix` method and all its call sites become dead code.

### Step 9: Build and test

- Run full build to verify compilation
- Run existing test suite for behavioral equivalence
- Verify no regressions in battle simulation output

### Step 10: Benchmark

- Profile the same workload to measure CPU reduction in the `GetHandlerInfo` chain
- Verify cache memory overhead is acceptable
- Compare single-threaded and parallel throughput

---

## Risk Assessment

| Risk | Mitigation |
|------|------------|
| ActiveMove handler cache invalidation | ActiveMove handlers are `init`-only; safe to cache at construction |
| SwitchIn pre-computation correctness | Gen is always 9; target is always Pokemon in FindPokemonEventHandlers |
| Memory overhead | ~1,500 cache entries total across all Library effects; negligible |
| Breaking existing behavior | Cache is built using existing `GetEventHandlerInfo`; same results by construction |

## Expected Outcome

- **Single `FrozenDictionary.TryGetValue`** replaces 3-step indirection chain
- **Null results short-circuited** via `HasAnyEventHandlers` check
- **SwitchIn special case eliminated** from hot path entirely
- Estimated **60–80% reduction** in this hotspot's CPU contribution
- Direct throughput multiplier under parallelism
