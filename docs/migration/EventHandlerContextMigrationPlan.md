# Event Handler Context Migration Plan

## Goal

Convert **all 1,496 legacy event handler instantiations** across 36 data files from the reflection-based legacy pattern to the direct-invocation `Create` factory pattern. This eliminates `DynamicInvoke` (reflection) from the hot path, which is critical for simulation performance.

## Current State

| Metric | Value |
|--------|-------|
| Total `EventHandlerInfo` classes | 462 |
| Classes already migrated (have `Create` + context ctor) | 37 (8%) |
| Classes needing migration | 425 (92%) |
| Total handler instantiations in `Data/` | 1,496 |
| Instantiations using legacy `new` pattern | 1,496 (100%) |
| Instantiations using `Create` factory | 0 (0%) |
| Unique delegate signatures across all classes | ~50 |
| Data files containing handlers | 36 |

### Handler Classes by Directory

| Directory | Total Classes | Already Have `Create` | Need Migration |
|-----------|--------------|----------------------|----------------|
| `EventMethods/` | 357 | 20 | 337 |
| `PokemonEventMethods/` | 78 | 0 | 78 |
| `MoveEventMethods/` | 30 | 7 | 23 |
| `ConditionSpecific/` | 5 | 4 | 1 |
| `FieldEventMethods/` | 4 | 3 | 1 |
| `SideEventMethods/` | 4 | 3 | 1 |
| `AbilityEventMethods/` | 3 | 0 | 3 |
| `ItemSpecific/` | 4 | 0 | 4 |

---

## Architecture Summary

### Legacy Pattern (Current — Uses Reflection)

```csharp
// In Data file — creates a Handler (Delegate) property
OnBasePower = new OnBasePowerEventInfo((battle, basePower, source, target, move) =>
{
    // handler body
    return new VoidReturn();
}, priority: 23)
```

At runtime, `InvokeEventHandlerInfo` detects `Handler != null && ContextHandler == null`, then calls `EventHandlerAdapter.AdaptLegacyHandler()` which uses `DynamicInvoke` with reflection-based parameter name/type matching.

### Target Pattern (New — Direct Invocation)

```csharp
// In Data file — uses Create factory which sets ContextHandler
OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, source, target, move) =>
{
    // handler body — SAME code, just returns RelayVar? instead of union type
    return null; // VoidReturn equivalent
}, priority: 23)
```

At runtime, `InvokeEventHandlerInfo` detects `ContextHandler != null` and calls it directly. No reflection, no `DynamicInvoke`, no parameter name matching.

### What Each EventHandlerInfo Class Needs

Each class needs two additions beyond its existing legacy constructor:

1. **Context constructor** — accepts `EventHandlerDelegate contextHandler` and sets `ContextHandler` instead of `Handler`
2. **`Create` static factory** — accepts the same strongly-typed delegate as the legacy constructor but wraps it in a `ContextHandler` lambda that extracts parameters from `EventContext`

Reference the already-migrated `OnDamagingHitEventInfo.cs` or `OnBeforeMoveEventInfo.cs` as templates.

### Return Type Mapping (Legacy → Create)

When converting Data files from `new` to `Create`, the handler return type changes:

| Legacy Return | Create Return (`RelayVar?`) |
|---------------|----------------------------|
| `void` (Action) | `return null;` (handled by Create wrapper) |
| `VoidReturn` / `new VoidReturn()` | `return null;` |
| `bool false` | `return new BoolRelayVar(false);` |
| `bool true` | `return new BoolRelayVar(true);` |
| `BoolVoidUnion` returning bool | `return new BoolRelayVar(value);` |
| `BoolVoidUnion` returning void | `return null;` |
| `int` value | `return new IntRelayVar(value);` |
| `double` / `decimal` value | `return new DecimalRelayVar(value);` |
| `DoubleVoidUnion` returning double | `return new DecimalRelayVar(value);` |
| `IntVoidUnion` returning int | `return new IntRelayVar(value);` |
| `BoolIntEmptyVoidUnion` | Use `BoolRelayVar`, `IntRelayVar`, or `null` as appropriate |
| `BoolEmptyVoidUnion` | `BoolRelayVar` or `null` |
| Keep as-is if return type already `RelayVar?` | No change needed |

**Important**: The `Create` factory method signature should use `RelayVar?` as its return type (or `void` via `Action` when the handler never returns a meaningful value). The factory wrapper handles the conversion internally.

### Parameter Mapping (EventContext → Strongly-Typed)

When writing the `Create` factory body, map `EventContext` to parameters:

| Parameter Position/Type | EventContext Accessor |
|------------------------|----------------------|
| `Battle battle` (always position 0) | `context.Battle` |
| `Pokemon target` / `Pokemon pokemon` | `context.GetTargetPokemon()` |
| `Pokemon source` | `context.GetSourcePokemon()` |
| `ActiveMove move` | `context.GetMove()` |
| `IEffect effect` / `IEffect sourceEffect` | `context.GetSourceEffect<IEffect>()` |
| `Side side` | `context.GetTargetSide()` |
| `Field field` | `context.Battle.Field` (or `context.TargetField`) |
| `int relayVar` (damage, basePower, etc.) | `context.GetRelayVar<IntRelayVar>().Value` |
| `int? relayVar` (nullable int) | `context.TryGetRelayVar<IntRelayVar>()?.Value` |
| `decimal relayVar` | `context.GetRelayVar<DecimalRelayVar>().Value` |
| `Condition condition` | `context.GetSourceEffect<Condition>()` (or cast from RelayVar) |
| `Ability ability` | Typically from `context.SourceEffect` or `context.Effect` |
| `Item item` | Typically from `context.SourceEffect` or `context.Effect` |
| `SparseBoostsTable boosts` | `context.GetRelayVar<BoostTableRelayVar>()` (check actual type) |
| `PokemonType type` | `context.SourceType` or unwrap from relay var |
| `PokemonType[] types` | Unwrap from relay var |
| `SecondaryEffect[] secondaries` | Unwrap from relay var |

---

## Phases

### Phase 0: Preparation & Validation

**Scope**: Set up tooling and verify the migration approach.

**Tasks**:
1. Verify the project builds clean before starting: `dotnet build`
2. Run existing tests to establish a green baseline
3. Review `EventContext.cs` accessors to confirm all parameter types can be resolved
4. Review `RelayVar` subclasses to confirm all return types have relay var equivalents
5. Verify that existing `Create`-migrated classes (37 total) work correctly by running tests

**Deliverable**: Green build + green tests = safe to proceed.

---

### Phase 1: Migrate High-Frequency EventHandlerInfo Classes (EventMethods)

**Scope**: Add `Create` factory + context constructor to the **top 20 most-used** EventHandlerInfo classes in `EventMethods/` that don't already have them. These account for the majority of handler instantiations.

**Priority order** (by usage count in Data files, excluding already-migrated):

| # | Class | Usages | Signature |
|---|-------|--------|-----------|
| 1 | `OnStartEventInfo` (EventMethods) | 142 | `Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion?>` |
| 2 | `OnHitEventInfo` (EventMethods) | 88 | mixed — check file |
| 3 | `OnTryHitEventInfo` (EventMethods) | 58 | already has Create |
| 4 | `OnModifyMoveEventInfo` | 55 | already has Create |
| 5 | `OnEndEventInfo` (EventMethods) | 49 | multiple signatures |
| 6 | `OnUpdateEventInfo` | 43 | `Action<Battle, Pokemon>` |
| 7 | `OnTryEventInfo` (MoveEventMethods) | 36 | already has Create |
| 8 | `OnEatEventInfo` (ItemSpecific) | 35 | check signature |
| 9 | `OnTakeItemEventInfo` | 28 | check signature |
| 10 | `OnTryMoveEventInfo` | 20 | check signature |
| 11 | `OnModifyTypeEventInfo` | 15 | check signature |
| 12 | `OnTryBoostEventInfo` | 16 | check signature |
| 13 | `OnDamageEventInfo` (EventMethods) | 14 | check signature |
| 14 | `OnFieldStartEventInfo` | 17 | already has Create |
| 15 | `OnFieldResidualEventInfo` | 14 | check signature |
| 16 | `OnFieldEndEventInfo` | 15 | already has Create |
| 17 | `OnSideStartEventInfo` | 15 | already has Create |
| 18 | `OnRestartEventInfo` | 11 | already has Create |
| 19 | `OnImmunityEventInfo` | 10 | check signature |
| 20 | `OnModifyDamageEventInfo` | 10 | check signature |

**How to migrate each class**:
1. Open the `.cs` file for the EventHandlerInfo class
2. Read the existing legacy constructor to identify the delegate signature and parameter names
3. Add a **context constructor** that takes `EventHandlerDelegate contextHandler` and copies all other params (priority, order, subOrder, usesSpeed). Set `ContextHandler = contextHandler` instead of `Handler = handler`. Do NOT set `ExpectedParameterTypes` or call `ValidateConfiguration()`
4. Add a **`Create` static factory** that takes the same delegate type as the legacy constructor but with `RelayVar?` return (or `Action` for void handlers). The factory body creates a `new` instance using the context constructor, wrapping the delegate in a lambda that extracts parameters from `EventContext`
5. Use already-migrated classes as reference:
   - `Action` handler: `OnResidualEventInfo.cs`, `OnDamagingHitEventInfo.cs`
   - `Func` returning union: `OnBeforeMoveEventInfo.cs`, `OnBasePowerEventInfo.cs`
6. Build to verify no compile errors

**Deliverable**: All top-20 classes have `Create` + context constructor. Build passes.

---

### Phase 2: Migrate Remaining EventMethods Classes

**Scope**: Add `Create` factory + context constructor to the remaining **~317 EventMethods** classes not covered in Phase 1.

**Strategy**: Group by delegate signature since classes sharing the same signature need identical `Create` bodies. There are ~50 unique signatures. Process one signature group at a time.

**Top signature groups (by class count)**:

| Delegate Signature | Class Count |
|--------------------|-------------|
| `Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion?>` | 67 |
| `Action<Battle, Pokemon, Pokemon, ActiveMove>` | 12 |
| `Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>` | 10 |
| `Action<Battle, Pokemon, Pokemon, IEffect>` | 9 |
| `Func<Battle, int, Pokemon, IntVoidUnion>` | 9 |
| `Action<Battle, Item, Pokemon>` | 9 |
| `Action<Battle, int, Pokemon, Pokemon, ActiveMove>` | 9 |
| `Action<Battle, Pokemon, Pokemon>` | 8 |
| `Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>` | 8 |
| All others (41 signatures) | ~150 |

**How to batch**: For each unique signature group:
1. Write the `Create` factory template once (parameter extraction + return wrapping)
2. Apply it to every class in that group, adjusting only `EventId`, `Prefix`, and constructor param names

**Deliverable**: All 357 EventMethods classes have `Create`. Build passes.

---

### Phase 3: Migrate PokemonEventMethods Classes (Prefixed Handlers)

**Scope**: Add `Create` factory + context constructor to all **78 PokemonEventMethods** classes (`OnAlly*`).

**Key insight**: These share identical delegate signatures with their base `EventMethods` counterparts. The only differences are:
- Different `EventPrefix` (Ally, Foe, Source, Any)
- Same parameter extraction logic

**Strategy**: For each `OnAlly*` class, copy the `Create` pattern from the corresponding `EventMethods` class and only change the `Prefix` value.

**Deliverable**: All 78 PokemonEventMethods classes have `Create`. Build passes.

---

### Phase 4: Migrate Remaining Handler Directories

**Scope**: Add `Create` factory + context constructor to remaining classes in:

| Directory | Classes Remaining |
|-----------|------------------|
| `MoveEventMethods/` | 23 |
| `AbilityEventMethods/` | 3 |
| `ItemSpecific/` | 4 |
| `ConditionSpecific/` | 1 (`OnCopyEventInfo`) |
| `FieldEventMethods/` | 1 (`OnFieldResidualEventInfo`) |
| `SideEventMethods/` | 1 (`OnSideRestartEventInfo`) |

**Total**: ~33 classes.

**Deliverable**: All 462 EventHandlerInfo classes have `Create` + context constructor. Build passes.

---

### Phase 5: Convert Data Files — Abilities

**Scope**: Convert all handler instantiations in `Data/Abilities/*.cs` from `new OnXxxEventInfo(...)` to `OnXxxEventInfo.Create(...)`.

**Files and handler counts**:

| File | Handlers |
|------|----------|
| `AbilitiesSTU.cs` | 124 |
| `AbilitiesABC.cs` | 67 |
| `AbilitiesGHI.cs` | 66 |
| `AbilitiesPQR.cs` | 65 |
| `AbilitiesDEF.cs` | 60 |
| `AbilitiesMNO.cs` | 54 |
| `AbilitiesVWX.cs` | 29 |
| `AbilitiesJKL.cs` | 16 |
| `AbilitiesYZ.cs` | 4 |
| **Total** | **485** |

**Conversion rules for each handler**:
1. Change `new OnXxxEventInfo((params) => { body }, priority: N)` to `OnXxxEventInfo.Create((params) => { body }, priority: N)`
2. Update `using` statements if needed (add the handler's namespace for the `Create` call which is a static method)
3. Update return values per the return type mapping table above:
   - `return new VoidReturn();` → `return null;`
   - `return false;` / `return true;` when legacy return type was `BoolVoidUnion` → `return new BoolRelayVar(false);` / `return new BoolRelayVar(true);`
   - `return someInt;` when legacy return type was `IntVoidUnion` → `return new IntRelayVar(someInt);`
   - `return someDouble;` when legacy return type was `DoubleVoidUnion` → `return new DecimalRelayVar(someDouble);`
   - Any union type returning the "void" variant → `return null;`
4. **Important**: If the `Create` factory uses `Action` (void handlers), no return value changes are needed — the existing code can stay as-is since there's no return statement to convert
5. Build after each file to catch errors early

**Deliverable**: All ability data files use `Create`. Build passes.

---

### Phase 6: Convert Data Files — Conditions

**Scope**: Convert all handler instantiations in `Data/Conditions/*.cs`.

**Files and handler counts**:

| File | Handlers |
|------|----------|
| `ConditionsSTU.cs` | 92 |
| `ConditionsDEF.cs` | 76 |
| `ConditionsPQR.cs` | 68 |
| `ConditionsABC.cs` | 49 |
| `ConditionsGHI.cs` | 37 |
| `ConditionsMNO.cs` | 33 |
| `ConditionsJKL.cs` | 16 |
| `ConditionsVWX.cs` | 16 |
| `ConditionsYZ.cs` | 5 |
| **Total** | **392** |

**Same conversion rules as Phase 5.**

**Deliverable**: All condition data files use `Create`. Build passes.

---

### Phase 7: Convert Data Files — Moves

**Scope**: Convert all handler instantiations in `Data/Moves/*.cs`.

**Files and handler counts**:

| File | Handlers |
|------|----------|
| `MovesSTU.cs` | 75 |
| `MovesDEF.cs` | 46 |
| `MovesABC.cs` | 44 |
| `MovesPQR.cs` | 41 |
| `MovesGHI.cs` | 25 |
| `MovesMNO.cs` | 17 |
| `MovesJKL.cs` | 12 |
| `MovesVWX.cs` | 11 |
| `MovesYZ.cs` | 1 |
| **Total** | **272** |

**Same conversion rules as Phase 5.**

**Deliverable**: All move data files use `Create`. Build passes.

---

### Phase 8: Convert Data Files — Items

**Scope**: Convert all handler instantiations in `Data/Items/*.cs`.

**Files and handler counts**:

| File | Handlers |
|------|----------|
| `ItemsABC.cs` | 59 |
| `ItemsSTU.cs` | 40 |
| `ItemsPQR.cs` | 38 |
| `ItemsMNO.cs` | 37 |
| `ItemsDEF.cs` | 36 |
| `ItemsJKL.cs` | 26 |
| `ItemsGHI.cs` | 23 |
| `ItemsVWX.cs` | 17 |
| `ItemsYZ.cs` | 4 |
| **Total** | **280** |

**Same conversion rules as Phase 5.**

**Deliverable**: All item data files use `Create`. Build passes.

---

### Phase 9: Convert Remaining Data Files

**Scope**: Convert any handler instantiations in other Data files (e.g., `SpeciesData/`, `Library.cs`, `Format/`).

**Check**: `Select-String -Path "ApogeeVGC\Data\**\*.cs" -Pattern "new On\w+EventInfo\(" -Exclude "*Abilities*","*Conditions*","*Moves*","*Items*"`

**Deliverable**: Zero remaining `new On*EventInfo(` instantiations in any Data file. Build passes.

---

### Phase 10: Cleanup & Deprecation

**Scope**: Mark legacy paths as obsolete and clean up adapter code.

**Tasks**:
1. Add `[Obsolete("Use Create factory method instead")]` to all legacy constructors (the ones that set `Handler` property) in every EventHandlerInfo class
2. Verify no code anywhere uses the legacy `Handler` property directly (outside of EventHandlerAdapter)
3. Mark `EventHandlerAdapter.AdaptLegacyHandler()` as `[Obsolete]`
4. Mark the `Handler` property on `EventHandlerInfo` as `[Obsolete]`
5. Remove the debug logging in `EventHandlerAdapter.ResolveParameter` (lines with `context.Battle.Debug`)
6. Consider removing `ExpectedParameterTypes`, `ParameterNullability`, `ReturnTypeNullable`, and `ValidateConfiguration()` from migrated classes since context handlers don't need them
7. Run full test suite
8. Clean up migration documentation files in repo root (`MIGRATION_TEMPLATE.md`, `QUICK_START.md`, `IMPLEMENTATION_COMPLETE.md`, `EVENTHANDLERINFO_MIGRATION_COMPLETE.md`, etc.)

**Deliverable**: Legacy path marked obsolete. All tests pass. Clean build with no warnings from new `[Obsolete]` usage.

---

## Execution Rules for Implementing Threads

### Per-Phase Protocol

1. **Start**: Read this document and identify which phase to execute
2. **Build first**: Always `dotnet build` at the start to confirm green baseline
3. **One file at a time**: Migrate one `.cs` file, then build to verify
4. **No behavior changes**: The `Create` factory must produce identical runtime behavior to the legacy constructor. The only difference is the invocation path (direct vs reflection)
5. **Build after**: Always build after completing each file
6. **Test after each phase**: Run tests after completing an entire phase

### Template: Adding Create to an EventHandlerInfo Class

Given a class like:

```csharp
public sealed record OnFooEventInfo : EventHandlerInfo
{
    public OnFooEventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, SomeUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Foo;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [...];
        ExpectedReturnType = typeof(SomeUnion);
        ParameterNullability = [...];
        ReturnTypeNullable = false;
        ValidateConfiguration();
    }
}
```

Add:

```csharp
    // Context constructor
    public OnFooEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Foo;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    // Create factory
    public static OnFooEventInfo Create(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, RelayVar?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFooEventInfo(
            context => handler(
                context.Battle,
                context.GetRelayVar<IntRelayVar>().Value,
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetMove()
            ),
            priority,
            usesSpeed
        );
    }
```

For `Action` (void) handlers, the `Create` factory wraps in a lambda that returns `null`:

```csharp
    public static OnFooEventInfo Create(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFooEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetTargetPokemon(),
                    context.GetSourcePokemon(),
                    context.GetMove()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
```

### Template: Converting Data File Handlers

**Before** (legacy):
```csharp
OnBasePower = new OnBasePowerEventInfo((battle, basePower, source, target, move) =>
{
    if (move.TypeChangerBoosted == battle.Effect)
    {
        battle.ChainModify([4915, 4096]);
        return battle.FinalModify(basePower);
    }
    return new VoidReturn();
}, priority: 23),
```

**After** (Create):
```csharp
OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, source, target, move) =>
{
    if (move.TypeChangerBoosted == battle.Effect)
    {
        battle.ChainModify([4915, 4096]);
        return new DecimalRelayVar(battle.FinalModify(basePower));
    }
    return null;
}, priority: 23),
```

Key changes:
1. `new OnBasePowerEventInfo(` → `OnBasePowerEventInfo.Create(`
2. `return new VoidReturn();` → `return null;`
3. Return values that were union type members become explicit `RelayVar` subclass instances

### Important Notes

- **Preserve all constructor parameters** (priority, order, subOrder, usesSpeed) — these must be passed through to the context constructor identically
- **Prefixed classes** (`OnAlly*`, `OnFoe*`, `OnSource*`, `OnAny*`) must set `Prefix = EventPrefix.Xxx` in both legacy and context constructors
- **Some classes have `Suffix`** — preserve that too
- **`IUnionEventHandler` implementations** — some EventHandlerInfo classes implement `IUnionEventHandler` for constant value fast-paths. The `Create` factory bypasses this, which is fine — the constant path is only used by the legacy `InvokeEventHandlerInfo` flow
- **Do not modify `EventHandlerAdapter.cs`** — it stays as the fallback for any legacy handlers not yet converted
- **Do not modify `Battle.Events.cs`** — the invocation logic already handles both paths correctly

---

## Summary Timeline

| Phase | Scope | Classes/Files | Estimated Handler Count |
|-------|-------|---------------|------------------------|
| 0 | Preparation | 0 | 0 |
| 1 | Top-20 EventHandlerInfo classes | ~15 classes | 0 (class changes only) |
| 2 | Remaining EventMethods classes | ~317 classes | 0 (class changes only) |
| 3 | PokemonEventMethods classes | 78 classes | 0 (class changes only) |
| 4 | Other handler directories | ~33 classes | 0 (class changes only) |
| 5 | Data/Abilities | 9 files | 485 handlers |
| 6 | Data/Conditions | 9 files | 392 handlers |
| 7 | Data/Moves | 9 files | 272 handlers |
| 8 | Data/Items | 9 files | 280 handlers |
| 9 | Other Data files | varies | ~67 handlers |
| 10 | Cleanup & deprecation | varies | 0 |

**Phases 1–4** (class infrastructure) should be completed before **Phases 5–9** (data file conversion), since the data files need the `Create` factories to exist.

**Each phase is independently buildable and testable.** A thread can complete any single phase and leave the project in a working state.
