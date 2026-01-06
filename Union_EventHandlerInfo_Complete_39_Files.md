# Complete List of Union EventHandlerInfo Files - ALL VARIANTS

## Summary

Found **8 union types** with multiple prefix variants each, totaling **39 EventHandlerInfo files** that need updating.

---

## Files by Union Type and Prefix

### 1. OnCriticalHit (5 files)
| Prefix | File Location | Folder |
|--------|---------------|--------|
| (none) | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnCriticalHitEventInfo.cs` | EventMethods |
| Ally | `ApogeeVGC\Sim\Events\Handlers\PokemonEventMethods\OnAllyCriticalHitEventInfo.cs` | PokemonEventMethods |
| Foe | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFoeCriticalHitEventInfo.cs` | EventMethods |
| Source | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSourceCriticalHitEventInfo.cs` | EventMethods |
| Any | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnAnyCriticalHitEventInfo.cs` | EventMethods |

---

### 2. OnFlinch (5 files)
| Prefix | File Location | Folder |
|--------|---------------|--------|
| (none) | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFlinchEventInfo.cs` | EventMethods |
| Ally | `ApogeeVGC\Sim\Events\Handlers\PokemonEventMethods\OnAllyFlinchEventInfo.cs` | PokemonEventMethods |
| Foe | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFoeFlinchEventInfo.cs` | EventMethods |
| Source | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSourceFlinchEventInfo.cs` | EventMethods |
| Any | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnAnyFlinchEventInfo.cs` | EventMethods |

---

### 3. OnFractionalPriority (4 files) ?? No Ally variant
| Prefix | File Location | Folder |
|--------|---------------|--------|
| (none) | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFractionalPriorityEventInfo.cs` | EventMethods |
| ~~Ally~~ | ? Not found | - |
| Foe | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFoeFractionalPriorityEventInfo.cs` | EventMethods |
| Source | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSourceFractionalPriorityEventInfo.cs` | EventMethods |
| Any | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnAnyFractionalPriorityEventInfo.cs` | EventMethods |

**Note:** OnFractionalPriority has no Ally variant (probably doesn't make sense semantically).

---

### 4. OnNegateImmunity (5 files)
| Prefix | File Location | Folder |
|--------|---------------|--------|
| (none) | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnNegateImmunityEventInfo.cs` | EventMethods |
| Ally | `ApogeeVGC\Sim\Events\Handlers\PokemonEventMethods\OnAllyNegateImmunityEventInfo.cs` | PokemonEventMethods |
| Foe | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFoeNegateImmunityEventInfo.cs` | EventMethods |
| Source | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSourceNegateImmunityEventInfo.cs` | EventMethods |
| Any | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnAnyNegateImmunityEventInfo.cs` | EventMethods |

---

### 5. OnTryEatItem (5 files)
| Prefix | File Location | Folder |
|--------|---------------|--------|
| (none) | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTryEatItemEventInfo.cs` | EventMethods |
| Ally | `ApogeeVGC\Sim\Events\Handlers\PokemonEventMethods\OnAllyTryEatItemEventInfo.cs` | PokemonEventMethods |
| Foe | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFoeTryEatItemEventInfo.cs` | EventMethods |
| Source | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSourceTryEatItemEventInfo.cs` | EventMethods |
| Any | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnAnyTryEatItemEventInfo.cs` | EventMethods |

---

### 6. OnTryHeal (5 files) ?? Multiple signatures!
| Prefix | File Location | Folder |
|--------|---------------|--------|
| (none) | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTryHealEventInfo.cs` | EventMethods |
| Ally | `ApogeeVGC\Sim\Events\Handlers\PokemonEventMethods\OnAllyTryHealEventInfo.cs` | PokemonEventMethods |
| Foe | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFoeTryHealEventInfo.cs` | EventMethods |
| Source | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSourceTryHealEventInfo.cs` | EventMethods |
| Any | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnAnyTryHealEventInfo.cs` | EventMethods |

**Special:** Has 2 different delegate signatures + bool constant!

---

### 7. OnTakeItem (5 files)
| Prefix | File Location | Folder |
|--------|---------------|--------|
| (none) | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTakeItemEventInfo.cs` | EventMethods |
| Ally | `ApogeeVGC\Sim\Events\Handlers\PokemonEventMethods\OnAllyTakeItemEventInfo.cs` | PokemonEventMethods |
| Foe | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFoeTakeItemEventInfo.cs` | EventMethods |
| Source | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSourceTakeItemEventInfo.cs` | EventMethods |
| Any | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnAnyTakeItemEventInfo.cs` | EventMethods |

---

### 8. OnLockMove (5 files) ?? Returns MoveId!
| Prefix | File Location | Folder |
|--------|---------------|--------|
| (none) | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnLockMoveEventInfo.cs` | EventMethods |
| Ally | `ApogeeVGC\Sim\Events\Handlers\PokemonEventMethods\OnAllyLockMoveEventInfo.cs` | PokemonEventMethods |
| Foe | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFoeLockMoveEventInfo.cs` | EventMethods |
| Source | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSourceLockMoveEventInfo.cs` | EventMethods |
| Any | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnAnyLockMoveEventInfo.cs` | EventMethods |

**Special:** Returns `MoveId` constant instead of `bool`!

---

## Total File Count

| Union Type | Base | Ally | Foe | Source | Any | Total |
|------------|------|------|-----|--------|-----|-------|
| OnCriticalHit | ? | ? | ? | ? | ? | 5 |
| OnFlinch | ? | ? | ? | ? | ? | 5 |
| OnFractionalPriority | ? | ? | ? | ? | ? | 4 |
| OnNegateImmunity | ? | ? | ? | ? | ? | 5 |
| OnTryEatItem | ? | ? | ? | ? | ? | 5 |
| OnTryHeal | ? | ? | ? | ? | ? | 5 |
| OnTakeItem | ? | ? | ? | ? | ? | 5 |
| OnLockMove | ? | ? | ? | ? | ? | 5 |
| **TOTAL** | **8** | **7** | **8** | **8** | **8** | **39** |

---

## Files by Folder

### EventMethods Folder (32 files)
**Base versions (8):**
1. `OnCriticalHitEventInfo.cs`
2. `OnFlinchEventInfo.cs`
3. `OnFractionalPriorityEventInfo.cs`
4. `OnNegateImmunityEventInfo.cs`
5. `OnTryEatItemEventInfo.cs`
6. `OnTryHealEventInfo.cs`
7. `OnTakeItemEventInfo.cs`
8. `OnLockMoveEventInfo.cs`

**Foe prefixed (8):**
9. `OnFoeCriticalHitEventInfo.cs`
10. `OnFoeFlinchEventInfo.cs`
11. `OnFoeFractionalPriorityEventInfo.cs`
12. `OnFoeNegateImmunityEventInfo.cs`
13. `OnFoeTryEatItemEventInfo.cs`
14. `OnFoeTryHealEventInfo.cs`
15. `OnFoeTakeItemEventInfo.cs`
16. `OnFoeLockMoveEventInfo.cs`

**Source prefixed (8):**
17. `OnSourceCriticalHitEventInfo.cs`
18. `OnSourceFlinchEventInfo.cs`
19. `OnSourceFractionalPriorityEventInfo.cs`
20. `OnSourceNegateImmunityEventInfo.cs`
21. `OnSourceTryEatItemEventInfo.cs`
22. `OnSourceTryHealEventInfo.cs`
23. `OnSourceTakeItemEventInfo.cs`
24. `OnSourceLockMoveEventInfo.cs`

**Any prefixed (8):**
25. `OnAnyCriticalHitEventInfo.cs`
26. `OnAnyFlinchEventInfo.cs`
27. `OnAnyFractionalPriorityEventInfo.cs`
28. `OnAnyNegateImmunityEventInfo.cs`
29. `OnAnyTryEatItemEventInfo.cs`
30. `OnAnyTryHealEventInfo.cs`
31. `OnAnyTakeItemEventInfo.cs`
32. `OnAnyLockMoveEventInfo.cs`

### PokemonEventMethods Folder (7 files)
**Ally prefixed (7):**
33. `OnAllyCriticalHitEventInfo.cs`
34. `OnAllyFlinchEventInfo.cs`
35. `OnAllyNegateImmunityEventInfo.cs`
36. `OnAllyTryEatItemEventInfo.cs`
37. `OnAllyTryHealEventInfo.cs`
38. `OnAllyTakeItemEventInfo.cs`
39. `OnAllyLockMoveEventInfo.cs`

---

## Pattern for Updating

All 39 files need the same transformation:

### Before (Current):
```csharp
public sealed record OnCriticalHitEventInfo : EventHandlerInfo
{
    public OnCriticalHitEventInfo(
 Func<Battle, Pokemon, object?, Move, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
{
        Id = EventId.CriticalHit;
 Handler = handler; // ? Raw delegate
      Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(object), typeof(Move)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
```

### After (Updated):
```csharp
public sealed record OnCriticalHitEventInfo : UnionEventHandlerInfo<OnCriticalHit>
{
    public OnCriticalHitEventInfo(
  OnCriticalHit unionValue, // ? Union type
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
        UnionValue = unionValue; // ? Set union value
   Handler = ExtractDelegate(); // ? Extract delegate
        Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(object), typeof(Move)];
 ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
```

**Key Changes:**
1. Base class: `EventHandlerInfo` ? `UnionEventHandlerInfo<TUnion>`
2. Constructor parameter: Raw delegate ? Union type
3. Property: `Handler = handler` ? `UnionValue = unionValue; Handler = ExtractDelegate()`

---

## Special Cases

### 1. OnTryHeal (5 files) - Multiple Signatures

All 5 OnTryHeal variants need custom validation due to 2 delegate signatures:

```csharp
public new void Validate()
{
    if (IsConstantValue()) return;

  var del = ExtractDelegate();
    if (del == null) return;

    var paramCount = del.Method.GetParameters().Length;

 if (paramCount == 5)
  {
    ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
     ExpectedReturnType = typeof(IntBoolUnion);
    }
    else if (paramCount == 2)
    {
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(bool);
    }
    else
    {
        throw new InvalidOperationException($"OnTryHeal: Invalid parameter count {paramCount}.");
    }

    base.Validate();
}
```

### 2. OnLockMove (5 files) - Returns MoveId

All 5 OnLockMove variants return `MoveIdVoidUnion` instead of `BoolVoidUnion`:

```csharp
ExpectedReturnType = typeof(MoveIdVoidUnion); // ?? Not bool!
```

### 3. OnFractionalPriority (4 files) - Returns decimal

All 4 OnFractionalPriority variants (no Ally) return `int` (ModifierSourceMoveHandler):

```csharp
ExpectedReturnType = typeof(int); // ?? Returns int, not bool!
```

---

## Prefix-Specific Notes

### Ally Variants (PokemonEventMethods folder)
- 7 files total (no OnFractionalPriority)
- Same union types, just with `Ally` prefix in EventId
- Example: `Id = EventId.CriticalHit` becomes `Id = EventId.AllyCriticalHit` (wait, check this!)

Actually, let me check what the EventId should be for prefixed variants...

Most likely:
- Base: `EventId.CriticalHit`
- Ally: Still `EventId.CriticalHit` with `Prefix = EventPrefix.Ally`
- Foe: Still `EventId.CriticalHit` with `Prefix = EventPrefix.Foe`
- etc.

The `Prefix` property in `EventHandlerInfo` is what distinguishes them!

---

## Update Strategy

### Phase 1: Base Versions (8 files)
Update the base versions first to establish the pattern:
1. OnCriticalHitEventInfo
2. OnFlinchEventInfo
3. OnFractionalPriorityEventInfo
4. OnNegateImmunityEventInfo
5. OnTryEatItemEventInfo
6. OnTryHealEventInfo (special - multiple signatures)
7. OnTakeItemEventInfo
8. OnLockMoveEventInfo (special - returns MoveId)

### Phase 2: Prefixed Versions (31 files)
Apply the same pattern to all prefixed variants:
- 7 Ally variants (PokemonEventMethods folder)
- 8 Foe variants (EventMethods folder)
- 8 Source variants (EventMethods folder)
- 8 Any variants (EventMethods folder)

---

## Automation Opportunity

Since all 39 files follow the same pattern, this could be automated:

```python
# Pseudocode
for union_type in ["OnCriticalHit", "OnFlinch", ...]:
for prefix in [None, "Ally", "Foe", "Source", "Any"]:
   if should_skip(union_type, prefix):# e.g., OnFractionalPriority + Ally
            continue
        
        update_file(
   file=f"{prefix}{union_type}EventInfo.cs",
      union_type=union_type,
   prefix=prefix
      )
```

---

## Summary

? **Found all 8 union types**  
? **Found all 39 EventHandlerInfo files** (8 base + 31 prefixed)  
? **Identified 3 special cases** (OnTryHeal, OnLockMove, OnFractionalPriority)  
? **Confirmed Ally variants are in PokemonEventMethods folder**  
? **All other variants are in EventMethods folder**  

**Total files to update: 39**

---

## Next Steps

Would you like me to:
1. Start with the 8 base files?
2. Create a script to automate the updates?
3. Update specific files you're most concerned about?

Let me know how you'd like to proceed!
