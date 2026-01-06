# Union EventHandlerInfo Update Progress

## Status: 39/39 Files Completed ? ??

## Progress Summary

| Prefix | Files | Status |
|--------|-------|--------|
| Base (none) | 8/8 | ? COMPLETE |
| Foe | 8/8 | ? COMPLETE |
| Source | 8/8 | ? COMPLETE |
| Any | 8/8 | ? COMPLETE |
| Ally | 7/7 | ? COMPLETE |
| **TOTAL** | **39/39** | **100% Complete** ? |

---

## All Files Completed (39) ?

### Base Files (8) ?
1. ? `OnCriticalHitEventInfo.cs`
2. ? `OnFlinchEventInfo.cs`
3. ? `OnFractionalPriorityEventInfo.cs`
4. ? `OnNegateImmunityEventInfo.cs`
5. ? `OnTryEatItemEventInfo.cs`
6. ? `OnTryHealEventInfo.cs` (with custom validation - FIXED)
7. ? `OnTakeItemEventInfo.cs`
8. ? `OnLockMoveEventInfo.cs`

### Foe Files (8) ?
9. ? `OnFoeCriticalHitEventInfo.cs`
10. ? `OnFoeFlinchEventInfo.cs`
11. ? `OnFoeFractionalPriorityEventInfo.cs`
12. ? `OnFoeNegateImmunityEventInfo.cs`
13. ? `OnFoeTryEatItemEventInfo.cs`
14. ? `OnFoeTryHealEventInfo.cs` (with custom validation - FIXED)
15. ? `OnFoeTakeItemEventInfo.cs`
16. ? `OnFoeLockMoveEventInfo.cs`

### Source Files (8) ?
17. ? `OnSourceCriticalHitEventInfo.cs`
18. ? `OnSourceFlinchEventInfo.cs`
19. ? `OnSourceFractionalPriorityEventInfo.cs`
20. ? `OnSourceNegateImmunityEventInfo.cs`
21. ? `OnSourceTryEatItemEventInfo.cs`
22. ? `OnSourceTryHealEventInfo.cs` (with custom validation - FIXED)
23. ? `OnSourceTakeItemEventInfo.cs`
24. ? `OnSourceLockMoveEventInfo.cs`

### Any Files (8) ?
25. ? `OnAnyCriticalHitEventInfo.cs`
26. ? `OnAnyFlinchEventInfo.cs`
27. ? `OnAnyFractionalPriorityEventInfo.cs`
28. ? `OnAnyNegateImmunityEventInfo.cs`
29. ? `OnAnyTryEatItemEventInfo.cs`
30. ? `OnAnyTryHealEventInfo.cs` (with custom validation - FIXED)
31. ? `OnAnyTakeItemEventInfo.cs`
32. ? `OnAnyLockMoveEventInfo.cs`

### Ally Files (7) ? - PokemonEventMethods folder
33. ? `OnAllyCriticalHitEventInfo.cs`
34. ? `OnAllyFlinchEventInfo.cs`
35. ? `OnAllyNegateImmunityEventInfo.cs`
36. ? `OnAllyTryEatItemEventInfo.cs`
37. ? `OnAllyTryHealEventInfo.cs` (with custom validation - FIXED)
38. ? `OnAllyTakeItemEventInfo.cs`
39. ? `OnAllyLockMoveEventInfo.cs`

**Note:** No `OnAllyFractionalPriorityEventInfo` (doesn't exist semantically)

---

## Update Pattern Applied

All files were successfully transformed from the old `EventHandlerInfo` pattern to the new `UnionEventHandlerInfo<TUnion>` pattern.

### Standard Transformation

```csharp
// BEFORE
public sealed record OnCriticalHitEventInfo : EventHandlerInfo
{
    public OnCriticalHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
        Handler = handler;
        Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [...];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}

// AFTER
public sealed record OnCriticalHitEventInfo : UnionEventHandlerInfo<OnCriticalHit>
{
    public OnCriticalHitEventInfo(
OnCriticalHit unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
      UnionValue = unionValue;
Handler = ExtractDelegate();
        Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes = [...];
   ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
```

### Key Changes Applied:
1. **Base class:** `EventHandlerInfo` ? `UnionEventHandlerInfo<TUnion>`
2. **Constructor parameter:** `Func<...> handler` ? `TUnion unionValue`
3. **Added line:** `UnionValue = unionValue;`
4. **Changed line:** `Handler = handler;` ? `Handler = ExtractDelegate();`
5. **Added:** `using ApogeeVGC.Sim.Utils.Unions;` (if missing)

---

## Special Validation Fix for OnTryHeal Variants

### Problem Discovered

During implementation, discovered that the custom `Validate()` method in OnTryHeal variants attempted to set `init-only` properties `ExpectedParameterTypes` and `ExpectedReturnType`, which caused compilation errors.

### Solution Applied

Modified all 5 OnTryHeal variants to use **local variables** for validation instead of attempting to modify `init-only` properties:

```csharp
public new void Validate()
{
    if (UnionValue == null) return;
    if (IsConstantValue()) return;

    var extractedDelegate = ExtractDelegate();
    if (extractedDelegate == null) return;

  MethodInfo method = extractedDelegate.Method;
 var actualParams = method.GetParameters();
    int paramCount = actualParams.Length;

    // Determine expected types based on parameter count (LOCAL VARIABLES)
    Type[]? expectedParamTypes;
    Type? expectedRetType;

    if (paramCount == 5)
    {
    expectedParamTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
 expectedRetType = typeof(IntBoolUnion);
    }
    else if (paramCount == 2)
    {
expectedParamTypes = [typeof(Battle), typeof(Pokemon)];
        expectedRetType = typeof(bool);
    }
    else
    {
        throw new InvalidOperationException($"...");
    }

    // Perform validation using local variables
  ValidateParameters(method, actualParams, expectedParamTypes);
    ValidateReturnType(method, expectedRetType);
}

private void ValidateParameters(MethodInfo method, ParameterInfo[] actualParams, Type[] expectedParamTypes)
{
    // Validation logic using local variables
}

private void ValidateReturnType(MethodInfo method, Type expectedRetType)
{
    // Validation logic using local variables
}
```

### Files with Fixed Validation (5):
- ? `OnTryHealEventInfo.cs`
- ? `OnFoeTryHealEventInfo.cs`
- ? `OnSourceTryHealEventInfo.cs`
- ? `OnAnyTryHealEventInfo.cs`
- ? `OnAllyTryHealEventInfo.cs`

---

## Union Type Mapping

| EventInfo File Pattern | Union Type | Return Type |
|------------------------|------------|-------------|
| `*CriticalHitEventInfo` | `OnCriticalHit` | `BoolVoidUnion` |
| `*FlinchEventInfo` | `OnFlinch` | `BoolVoidUnion` |
| `*FractionalPriorityEventInfo` | `OnFractionalPriority` | `double` |
| `*NegateImmunityEventInfo` | `OnNegateImmunity` | `BoolVoidUnion` |
| `*TryEatItemEventInfo` | `OnTryEatItem` | `BoolVoidUnion` |
| `*TryHealEventInfo` | `OnTryHeal` | Multi-signature ?? |
| `*TakeItemEventInfo` | `OnTakeItem` | `PokemonVoidUnion` |
| `*LockMoveEventInfo` | `OnLockMove` | `MoveIdVoidUnion` |

---

## Build Validation ?

### Files Updated: 39
### Compilation Errors in Updated Files: 0 ?

All 39 EventHandlerInfo files compile successfully with no errors.

**Note:** Build shows errors in other parts of the codebase (Battle.Events.cs, Battle.Sorting.cs, etc.), but these are **pre-existing issues** unrelated to the Union EventHandlerInfo updates.

---

## Benefits of Union Pattern

### ? Support for Constants
Can now use constant values directly:
```csharp
new OnCriticalHitEventInfo(OnCriticalHit.FromBool(true))  // Always crits
new OnFlinchEventInfo(OnFlinch.FromBool(false))         // Never flinches
new OnFractionalPriorityEventInfo(OnFractionalPriority.FromDouble(0.1))  // +0.1 priority
```

### ? Support for Delegates
Original delegate functionality preserved:
```csharp
new OnCriticalHitEventInfo((battle, target, source, move) => 
    BoolVoidUnion.FromBool(source.HasAbility(AbilityId.SuperLuck)))
```

### ? Type Safety
Union types provide compile-time type checking:
```csharp
OnCriticalHit unionValue = OnCriticalHit.FromBool(true);  // ? Valid
OnCriticalHit unionValue = OnCriticalHit.FromDelegate(...);  // ? Valid
OnCriticalHit unionValue = 42;  // ? Compile error
```

### ? Cleaner Code
Reduces boilerplate for simple cases:
```csharp
// BEFORE: Verbose lambda for simple constant
new OnCriticalHitEventInfo((b, t, s, m) => BoolVoidUnion.FromBool(true))

// AFTER: Clean constant
new OnCriticalHitEventInfo(OnCriticalHit.FromBool(true))
```

---

## Summary

**Completed:** 39/39 files (100%) ?  
**Pattern:** Consistent across all files  
**Special Cases:** 5 OnTryHeal variants with fixed validation  
**Build Status:** ? All updated files compile successfully  
**Compilation Errors:** 0 (in updated files)

---

**Status:** ? COMPLETE  
**Last Updated:** All 39 files successfully migrated  
**Date Completed:** [Current Session]

## ?? Union EventHandlerInfo Migration - COMPLETE!

All 39 EventHandlerInfo files have been successfully updated to use the UnionEventHandlerInfo pattern, enabling both delegate and constant value support across the entire event system
