# Intuitive Delegate Method Names - Quick Reference

## Overview

Instead of using generic method names like `GetVoidSourceMoveDelegate()`, you can now use intuitive, self-documenting method names that clearly indicate their purpose in the battle engine.

## Method Mapping

### VoidSourceMoveHandler (Multiple Events ? Single Handler)

| Intuitive Method Name | Purpose | Parameters |
|----------------------|---------|------------|
| `GetOnAfterHitDelegate()` | After move hits target | `(battleContext, source, target, move)` |
| `GetOnAfterMoveDelegate()` | After move execution completes | `(battleContext, source, target, move)` |
| `GetOnAfterMoveSecondarySelfDelegate()` | After secondary effects on user | `(battleContext, source, target, move)` |
| `GetOnUseMoveMessageDelegate()` | Display move usage message | `(battleContext, source, target, move)` |

### VoidMoveHandler

| Intuitive Method Name | Purpose | Parameters |
|----------------------|---------|------------|
| `GetOnAfterMoveSecondaryDelegate()` | After secondary effects on target | `(battleContext, target, source, move)` |
| `GetOnMoveFailDelegate()` | When move fails to execute | `(battleContext, target, source, move)` |

### ModifierSourceMoveHandler (Multiple Events ? Single Handler)

| Intuitive Method Name | Purpose | Returns | Parameters |
|----------------------|---------|---------|------------|
| `GetOnBasePowerDelegate()` | Modify move's base power | `int?` | `(battleContext, basePower, source, target, move)` |
| `GetOnModifyPriorityDelegate()` | Modify move's priority | `int?` | `(battleContext, priority, source, target, move)` |

### ResultMoveHandler (Multiple Events ? Single Handler)

| Intuitive Method Name | Purpose | Returns | Parameters |
|----------------------|---------|---------|------------|
| `GetOnHitDelegate()` | Check if move hits | `bool?` | `(battleContext, target, source, move)` |
| `GetOnHitFieldDelegate()` | Check if move affects field | `bool?` | `(battleContext, target, source, move)` |
| `GetOnPrepareHitDelegate()` | Prepare move to hit | `bool?` | `(battleContext, target, source, move)` |
| `GetOnTryImmunityDelegate()` | Check target immunity | `bool?` | `(battleContext, target, source, move)` |

### ResultSourceMoveHandler

| Intuitive Method Name | Purpose | Returns | Parameters |
|----------------------|---------|---------|------------|
| `GetOnTryDelegate()` | Check if move can be used | `bool?` | `(battleContext, source, target, move)` |
| `GetOnTryMoveDelegate()` | Attempt to use move | `bool?` | `(battleContext, source, target, move)` |

### ExtResultSourceMoveHandler

| Intuitive Method Name | Purpose | Returns | Parameters |
|----------------------|---------|---------|------------|
| `GetOnTryHitDelegate()` | Try to hit with extended result | `IntBoolUnion?` | `(battleContext, source, target, move)` |

### Move-Specific Delegates (Unique Signatures)

| Intuitive Method Name | Purpose | Returns | Parameters |
|----------------------|---------|---------|------------|
| `GetBasePowerCallbackDelegate()` | Calculate base power | `IntFalseUnion?` | `(battleContext, pokemon, target, move)` |
| `GetBeforeMoveCallbackDelegate()` | Before move execution | `bool?` | `(battleContext, pokemon, target?, move)` |
| `GetDamageCallbackDelegate()` | Calculate damage | `IntFalseUnion?` | `(battleContext, pokemon, target, move)` |
| `GetOnEffectivenessDelegate()` | Modify effectiveness | `int?` | `(battleContext, typeMod, target?, type, move)` |
| `GetOnModifyMoveDelegate()` | Modify move properties | `void` | `(battleContext, move, pokemon, target?)` |
| `GetOnAfterSubDamageDelegate()` | After substitute damage | `void` | `(battleContext, damage, target, source, move)` |
| `GetOnDamageDelegate()` | Handle damage events | `IntBoolUnion?` | `(battleContext, damage, target, source, effect)` |

## Usage Examples

### Before (Generic Names)
```csharp
// Unclear what this handler does
var handler = move.GetVoidSourceMoveDelegate(library);
handler?.Invoke(battleContext, source, target, move);

// Have to remember which CommonHandler type to use
var modHandler = move.GetModifierSourceMoveDelegate(library);
var power = modHandler?.Invoke(battleContext, move.BasePower, source, target, move);
```

### After (Intuitive Names)
```csharp
// Crystal clear what each handler does
var afterHitHandler = move.GetOnAfterHitDelegate(library);
afterHitHandler?.Invoke(battleContext, source, target, move);

var basePowerHandler = move.GetOnBasePowerDelegate(library);
var power = basePowerHandler?.Invoke(battleContext, move.BasePower, source, target, move);

var hitCheckHandler = move.GetOnHitDelegate(library);
bool? canHit = hitCheckHandler?.Invoke(battleContext, target, source, move);
```

## Battle Engine Integration

```csharp
public void ExecuteMove(Move move, Pokemon source, Pokemon target, BattleContext context)
{
    var library = context.Library;
    
    // 1. Pre-execution checks
    var beforeMove = move.GetBeforeMoveCallbackDelegate(library);
    if (beforeMove?.Invoke(context, source, target, move) == true)
        return; // Move blocked
    
    // 2. Try to use move
    var tryMove = move.GetOnTryDelegate(library);
    if (tryMove?.Invoke(context, source, target, move) == false)
    {
        move.GetOnMoveFailDelegate(library)?.Invoke(context, target, source, move);
        return;
    }
    
    // 3. Calculate base power
    int basePower = move.BasePower;
    var basePowerMod = move.GetOnBasePowerDelegate(library);
    basePower = basePowerMod?.Invoke(context, basePower, source, target, move) ?? basePower;
    
    // 4. Try to hit
    var tryHit = move.GetOnTryHitDelegate(library);
    var hitResult = tryHit?.Invoke(context, source, target, move);
    
    // 5. Execute damage/effects
    ExecuteDamage(basePower, source, target, move, context);
    
    // 6. After-effects
    move.GetOnAfterHitDelegate(library)?.Invoke(context, source, target, move);
    move.GetOnAfterMoveDelegate(library)?.Invoke(context, source, target, move);
    move.GetOnUseMoveMessageDelegate(library)?.Invoke(context, source, target, move);
}
```

## Key Benefits

1. **Self-Documenting Code**: Method names clearly indicate their purpose
2. **Better IntelliSense**: Easy to find the right method for your use case
3. **Reduced Cognitive Load**: No need to remember which CommonHandler type maps to which event
4. **Backward Compatible**: Generic methods still available for advanced usage
5. **Type Safety**: Same strong typing as before, just with better names

## Registration Stays the Same

```csharp
// Registration still uses the same EventId system
library.RegisterMoveDelegate<VoidSourceMoveHandler>(
    EventId.VoidSourceMoveHandler,
    MoveId.Tackle,
    (battleContext, source, target, move) => {
        Console.WriteLine($"{source.Name} made contact!");
    });

// But now you can access it with intuitive names
var afterHit = move.GetOnAfterHitDelegate(library);
var afterMove = move.GetOnAfterMoveDelegate(library);
var message = move.GetOnUseMoveMessageDelegate(library);

// All three return the same handler instance!
Console.WriteLine(ReferenceEquals(afterHit, afterMove)); // True
```

This system gives you the best of both worlds: the efficiency of shared CommonHandlers delegates with the clarity of purpose-specific method names!