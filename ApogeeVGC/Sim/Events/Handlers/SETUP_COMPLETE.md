# Specialized Event Handler Classes - Setup Complete

## What Was Created

### 1. Abstract Base Class
**File:** `ApogeeVGC/Sim/Events/EventHandlerInfo.cs`
- Changed from concrete record to `abstract record`
- All specialized classes inherit from this
- Provides validation and signature checking

### 2. New Directory Structure
```
ApogeeVGC/Sim/Events/Handlers/
```
This directory contains all specialized event handler classes.

### 3. Example Implementations (8 classes)

| Class | Event | Signature | Has Priority | Has Order |
|-------|-------|-----------|--------------|-----------|
| `OnDamagingHitEventInfo` | DamagingHit | Action<Battle, int, Pokemon, Pokemon, ActiveMove> | No | Yes |
| `OnBasePowerEventInfo` | BasePower | ModifierSourceMoveHandler | Yes | No |
| `OnResidualEventInfo` | Residual | Action<Battle, Pokemon, Pokemon, IEffect> | Yes | Yes (+ SubOrder) |
| `OnBeforeMoveEventInfo` | BeforeMove | VoidSourceMoveHandler | Yes | No |
| `OnAfterSetStatusEventInfo` | AfterSetStatus | Action<Battle, Condition, Pokemon, Pokemon, IEffect> | Yes | No |
| `OnSetStatusEventInfo` | SetStatus | Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> | Yes | No |
| `OnDamageEventInfo` | Damage | Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> | Yes | No |
| `OnEmergencyExitEventInfo` | EmergencyExit | Action<Battle, Pokemon> | No | No |

### 4. Documentation
- **README.md** - Complete guide for creating new classes
- **CHECKLIST.md** - All 148 remaining events to create

### 5. Removed Obsolete File
- Deleted `EventHandlerInfoBuilder.cs` (replaced by specialized classes)

## Type Safety Guarantees

With this architecture, you get **compile-time enforcement** of:

1. **EventId matches property name**
   ```csharp
   // ? Correct - types match
   OnDamagingHitInfo = new OnDamagingHitEventInfo(...)
   
   // ? Compile error - type mismatch
   OnDamagingHitInfo = new OnBasePowerEventInfo(...)
   ```

2. **Signature is correct**
   ```csharp
   // ? Correct signature
   new OnDamagingHitEventInfo(
       (Battle b, int dmg, Pokemon t, Pokemon s, ActiveMove m) => { }
   )
   
   // ? Compile error - wrong parameters
   new OnDamagingHitEventInfo(
       (Battle b, Pokemon t) => { }  // Missing parameters!
   )
   ```

3. **Metadata is consistent**
   - All `OnDamagingHitEventInfo` instances have `Id = EventId.DamagingHit`
   - All have identical `ExpectedParameterTypes`
   - Impossible to create mismatched metadata

## How to Create Remaining Classes

### Quick Process:

1. **Pick an EventId** from `CHECKLIST.md`

2. **Find signature** in `IEventMethods.cs`:
   ```csharp
   // Example: looking for OnStart
   Action<Battle, Pokemon>? OnStart { get; }
   ```

3. **Copy template** from `README.md`

4. **Fill in details**:
   - Replace `{EventId}` with `Start`
   - Replace `{HandlerType}` with `Action<Battle, Pokemon>`
   - Add parameter types to `ExpectedParameterTypes`
   - Set `ExpectedReturnType` to `typeof(void)`

5. **Check for priority/order** in `IEventMethods.cs`:
   ```csharp
int? OnStartPriority { get; }  // Add priority parameter
   ```

6. **Save** to `Handlers/On{EventId}EventInfo.cs`

### Example Workflow:

**Goal:** Create `OnStartEventInfo`

**Step 1:** Find in `IEventMethods.cs`:
```csharp
Action<Battle, Pokemon>? OnStart { get; }
```

**Step 2:** Check for priority properties:
```csharp
// Not found in IEventMethods, so no priority parameter needed
```

**Step 3:** Create file `Handlers/OnStartEventInfo.cs`:
```csharp
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers;

/// <summary>
/// Event handler info for OnStart event.
/// Triggered when an effect starts (ability activated, item used, etc.)
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnStartEventInfo : EventHandlerInfo
{
    public OnStartEventInfo(
      Action<Battle, Pokemon> handler,
     bool usesSpeed = false)
    {
        Id = EventId.Start;
        Handler = handler;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = new[] 
        { 
            typeof(Battle), 
      typeof(Pokemon) 
     };
        ExpectedReturnType = typeof(void);
    }
}
```

## Next Steps for Integration

### Phase 1: Create All Classes
- [ ] Create remaining 148 event classes
- [ ] Prioritize most commonly used events first (see CHECKLIST.md)

### Phase 2: Update IEventMethodsV2
```csharp
public interface IEventMethodsV2
{
    // Change from methods to properties:
    
    // OLD:
    // EventHandlerInfo? GetOnDamagingHitInfo();
  
    // NEW:
    EventHandlerInfo? OnDamagingHitInfo { get; }
}
```

### Phase 3: Update Ability.cs
```csharp
public record Ability : IEffect, IEventMethodsV2
{
    // Strongly-typed properties
    public OnDamagingHitEventInfo? OnDamagingHitInfo { get; init; }
    public OnBasePowerEventInfo? OnBasePowerInfo { get; init; }
    
    // Implement interface (returns base type)
    EventHandlerInfo? IEventMethodsV2.OnDamagingHitInfo => OnDamagingHitInfo;
    EventHandlerInfo? IEventMethodsV2.OnBasePowerInfo => OnBasePowerInfo;
}
```

### Phase 4: Update Abilities.cs (Data)
```csharp
[AbilityId.FlameBody] = new()
{
    Id = AbilityId.FlameBody,
    Name = "Flame Body",
    
    // Use specialized class - compiler enforces everything!
    OnDamagingHitInfo = new OnDamagingHitEventInfo(
        handler: (battle, damage, target, source, move) =>
        {
            if (!battle.CheckMoveMakesContact(move, source, target)) return;
        if (battle.RandomChance(3, 10))
  {
     source.TrySetStatus(ConditionId.Burn, target);
 }
        },
        order: 2,
        usesSpeed: true
    ),
},
```

### Phase 5: Apply to Other Effect Types
- Update `Item.cs`, `Condition.cs`, `Move.cs` same as `Ability.cs`
- Update data files (`Items.cs`, `Conditions.cs`, `Moves.cs`)

### Phase 6: Update Battle Code
- Replace `GetDelegate()` calls with `GetEventHandlerInfo()`
- Single lookup gets delegate + all metadata

## Benefits Summary

? **Compile-time type safety** - Wrong EventId = compile error
? **Signature enforcement** - Wrong signature = compile error
? **Consistent metadata** - Impossible to have mismatched data
? **IDE support** - IntelliSense shows correct parameters
? **Refactoring safe** - Rename/change signature updates everywhere
? **Self-documenting** - Class name = EventId = intent
? **Validation built-in** - Can call `Validate()` at startup

## Files Created

```
ApogeeVGC/Sim/Events/
??? EventHandlerInfo.cs (updated - now abstract)
??? Handlers/
    ??? README.md (creation guide)
    ??? CHECKLIST.md (all events to create)
    ??? SETUP_COMPLETE.md (this file)
    ??? OnDamagingHitEventInfo.cs
    ??? OnBasePowerEventInfo.cs
    ??? OnResidualEventInfo.cs
    ??? OnBeforeMoveEventInfo.cs
    ??? OnAfterSetStatusEventInfo.cs
    ??? OnSetStatusEventInfo.cs
    ??? OnDamageEventInfo.cs
    ??? OnEmergencyExitEventInfo.cs
```

## Build Status

? **All new files compile successfully**
? **No breaking changes to existing code**
? **Base architecture complete**
? **Ready for manual creation of remaining classes**

---

**You now have everything you need to manually create the remaining ~148 event handler classes!**

Each class follows the same pattern - just copy the template, fill in the signature, and save to the `Handlers/` directory.
