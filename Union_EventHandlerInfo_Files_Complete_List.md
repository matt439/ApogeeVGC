# Union Event Handler Info Files - Complete List

## Summary

Found **8 union types** that need their EventHandlerInfo records updated to inherit from `IUnionEventHandler`.

---

## The 8 Union Types and Their EventHandlerInfo Files

| # | Union Type | Union Definition Location | EventHandlerInfo File |
|---|------------|---------------------------|----------------------|
| 1 | `OnCriticalHit` | `SpecificUnion.cs:1398-1694` | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnCriticalHitEventInfo.cs` |
| 2 | `OnFlinch` | `SpecificUnion.cs:2885-3148` | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFlinchEventInfo.cs` |
| 3 | `OnFractionalPriority` | `SpecificUnion.cs:573-1081` | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFractionalPriorityEventInfo.cs` |
| 4 | `OnNegateImmunity` | `SpecificUnion.cs:4681-4982` | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnNegateImmunityEventInfo.cs` |
| 5 | `OnTryEatItem` | `SpecificUnion.cs:21614-21888` | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTryEatItemEventInfo.cs` |
| 6 | `OnTryHeal` | `SpecificUnion.cs:2095-2502` | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTryHealEventInfo.cs` |
| 7 | `OnTakeItem` | `SpecificUnion.cs:5318-5601` | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTakeItemEventInfo.cs` |
| 8 | `OnLockMove` | `SpecificUnion.cs:5546-5970` | `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnLockMoveEventInfo.cs` |

---

## Union Type Signatures

### 1. OnCriticalHit
```csharp
/// Func<Battle, Pokemon, object?, Move, BoolVoidUnion> | bool
public abstract record OnCriticalHit : IUnionEventHandler
```
**EventHandlerInfo:** `OnCriticalHitEventInfo`

---

### 2. OnFlinch
```csharp
/// Func<Battle, Pokemon, object?, Move, BoolVoidUnion> | bool
public abstract record OnFlinch : IUnionEventHandler
```
**EventHandlerInfo:** `OnFlinchEventInfo`

---

### 3. OnFractionalPriority
```csharp
/// ModifierSourceMoveHandler | -0.1
public abstract record OnFractionalPriority : IUnionEventHandler
```
**EventHandlerInfo:** `OnFractionalPriorityEventInfo`

---

### 4. OnNegateImmunity
```csharp
/// Func<Battle, Pokemon, PokemonType, BoolVoidUnion> | bool
public abstract record OnNegateImmunity : IUnionEventHandler
```
**EventHandlerInfo:** `OnNegateImmunityEventInfo`

---

### 5. OnTryEatItem
```csharp
/// Func<Battle, Item, Pokemon, BoolVoidUnion> | bool
public abstract record OnTryEatItem : IUnionEventHandler
```
**EventHandlerInfo:** `OnTryEatItemEventInfo`

---

### 6. OnTryHeal
```csharp
/// Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> 
/// | Func<Battle, Pokemon, bool?> 
/// | bool
public abstract record OnTryHeal : IUnionEventHandler
```
**EventHandlerInfo:** `OnTryHealEventInfo`
**Note:** This is the most complex - has **2 different delegate signatures + bool**!

---

### 7. OnTakeItem
```csharp
/// Func<Battle, Item, Pokemon, Pokemon, Move?, PokemonVoidUnion> | bool
public abstract record OnTakeItem : IUnionEventHandler
```
**EventHandlerInfo:** `OnTakeItemEventInfo`

---

### 8. OnLockMove ? (8th type!)
```csharp
/// Func<Battle, Pokemon, MoveIdVoidUnion> | MoveId
public abstract record OnLockMove : IUnionEventHandler
```
**EventHandlerInfo:** `OnLockMoveEventInfo`
**Note:** Returns `MoveId` as constant instead of `bool`!

---

## Files to Modify

### Phase 1: Union Type Implementations (SpecificUnion.cs)

All 8 union types already implement `IUnionEventHandler` (confirmed for `OnLockMove`). ?

**Status:** ? Already implemented in `SpecificUnion.cs`

---

### Phase 2: EventHandlerInfo Records (8 files)

Each EventHandlerInfo needs to:
1. Change base class from `EventHandlerInfo` to `UnionEventHandlerInfo<TUnion>`
2. Accept union type in constructor instead of raw delegate
3. Populate `UnionValue` instead of `Handler`
4. Call `ExtractDelegate()` to populate `Handler`

#### Files to Update:

1. ? `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnCriticalHitEventInfo.cs`
2. ? `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFlinchEventInfo.cs`
3. ? `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFractionalPriorityEventInfo.cs`
4. ? `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnNegateImmunityEventInfo.cs`
5. ? `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTryEatItemEventInfo.cs`
6. ? `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTryHealEventInfo.cs`
7. ? `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnTakeItemEventInfo.cs`
8. ? `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnLockMoveEventInfo.cs` ? **8th file!**

---

## Current Status: OnLockMove

### Confirmed: OnLockMove Already Implements IUnionEventHandler

```csharp
public abstract record OnLockMove : IUnionEventHandler
{
    public static implicit operator OnLockMove(MoveId moveId) => new OnLockMoveMoveId(moveId);
    public static implicit operator OnLockMove(Func<Battle, Pokemon, MoveIdVoidUnion> func) =>
        new OnLockMoveFunc(func);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}
```

? **Already has the interface!** Just need to update `OnLockMoveEventInfo.cs`.

---

## Pattern for Updating EventHandlerInfo Records

### Before (Current - Wrong):
```csharp
public sealed record OnCriticalHitEventInfo : EventHandlerInfo
{
    public OnCriticalHitEventInfo(
        Func<Battle, Pokemon, object?, Move, BoolVoidUnion> handler,
 int? priority = null,
        bool usesSpeed = true)
    {
     Id = EventId.CriticalHit;
        Handler = handler; // ? Takes raw delegate
     Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(object), typeof(Move)];
      ExpectedReturnType = typeof(BoolVoidUnion);
}
}
```

### After (New - Correct):
```csharp
public sealed record OnCriticalHitEventInfo : UnionEventHandlerInfo<OnCriticalHit>
{
    public OnCriticalHitEventInfo(
        OnCriticalHit unionValue, // ? Takes union type
        int? priority = null,
        bool usesSpeed = true)
    {
     Id = EventId.CriticalHit;
        UnionValue = unionValue; // ? Set union value
        Handler = ExtractDelegate(); // ? Extract delegate from union
        Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(object), typeof(Move)];
      ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
```

---

## Special Cases

### 1. OnTryHeal - Multiple Delegate Signatures

```csharp
public sealed record OnTryHealEventInfo : UnionEventHandlerInfo<OnTryHeal>
{
  public OnTryHealEventInfo(
        OnTryHeal unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
      Id = EventId.TryHeal;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
      Priority = priority;
        UsesSpeed = usesSpeed;
  
        // Don't set ExpectedParameterTypes/ExpectedReturnType
        // because OnTryHeal has 2 different signatures:
        // 1. Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>
      // 2. Func<Battle, Pokemon, bool?>
      ExpectedParameterTypes = null;
        ExpectedReturnType = null;
    }

    // Override Validate to handle multiple signatures
    public new void Validate()
    {
        if (IsConstantValue()) return;

        var del = ExtractDelegate();
     if (del == null) return;

        var method = del.Method;
   var paramCount = method.GetParameters().Length;

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
    throw new InvalidOperationException(
                $"OnTryHeal: Invalid parameter count {paramCount}. Expected 5 or 2.");
        }

        base.Validate();
    }
}
```

### 2. OnLockMove - Returns MoveId Instead of Bool

```csharp
public sealed record OnLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    public OnLockMoveEventInfo(
 OnLockMove unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.LockMove;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
   ExpectedReturnType = typeof(MoveIdVoidUnion); // ? Returns MoveId, not bool!
    }
}
```

### 3. OnFractionalPriority - Returns Decimal Constant

```csharp
public sealed record OnFractionalPriorityEventInfo : UnionEventHandlerInfo<OnFractionalPriority>
{
    public OnFractionalPriorityEventInfo(
        OnFractionalPriority unionValue,
        int? priority = null,
   bool usesSpeed = true)
    {
 Id = EventId.FractionalPriority;
        UnionValue = unionValue;
      Handler = ExtractDelegate();
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
     ExpectedReturnType = typeof(int); // ModifierSourceMoveHandler returns int
    }
}
```

---

## Where These Events Are Used

### IEventMethods Properties (Base Events)
- `OnCriticalHit? OnCriticalHit { get; }`
- `OnFlinch? OnFlinch { get; }`
- `OnFractionalPriority? OnFractionalPriority { get; }`
- `OnNegateImmunity? OnNegateImmunity { get; }`
- `OnTryEatItem? OnTryEatItem { get; }`
- `OnTryHeal? OnTryHeal { get; }`
- `OnTakeItem? OnTakeItem { get; }`
- `OnLockMove? OnLockMove { get; }`

### With Prefixes (Foe, Source, Any, Ally)
Each union type appears in multiple variants:
- **OnCriticalHit**: `OnCriticalHit`, `OnFoeCriticalHit`, `OnSourceCriticalHit`, `OnAnyCriticalHit`, `OnAllyCriticalHit`
- **OnFlinch**: `OnFlinch`, `OnFoeFlinch`, `OnSourceFlinch`, `OnAnyFlinch`, `OnAllyFlinch`
- **OnLockMove**: `OnLockMove`, `OnFoeLockMove`, `OnSourceLockMove`, `OnAnyLockMove`, `OnAllyLockMove`
- etc.

**Total EventHandlerInfo files to update:** 8 base files × (1 + 4 prefixes) = **~40 files** ??

Wait, let me check if prefixed versions exist...

---

## Verification: Do Prefixed Versions Exist?

Let me check if there are Foe/Source/Any/Ally variants of these union types...

Looking at the search results, I only found **8 files** in `EventMethods` folder:
1. `OnCriticalHitEventInfo.cs`
2. `OnFlinchEventInfo.cs`
3. `OnFractionalPriorityEventInfo.cs`
4. `OnNegateImmunityEventInfo.cs`
5. `OnTryEatItemEventInfo.cs`
6. `OnTryHealEventInfo.cs`
7. `OnTakeItemEventInfo.cs`
8. `OnLockMoveEventInfo.cs`

These are likely the **base (no prefix) versions** only. The prefixed versions (`OnFoe...`, `OnSource...`, `OnAny...`, `OnAlly...`) probably use the same union types but may be in different folders or don't have separate EventHandlerInfo records.

**Actual files to update:** **8 files** (just the base versions)

---

## Action Plan

### Step 1: Update EventHandlerInfo Records (8 files)
- Change base class to `UnionEventHandlerInfo<TUnion>`
- Accept union type in constructor
- Set `UnionValue` and extract `Handler`

### Step 2: Test Each Union Type
- Test with delegate: `new OnCriticalHitEventInfo((b, p, s, m) => true)`
- Test with constant: `new OnCriticalHitEventInfo(OnCriticalHit.FromBool(true))`

### Step 3: Search for Prefixed Variants
- Check if `OnAllyCriticalHitEventInfo`, `OnFoeCriticalHitEventInfo`, etc. exist
- If they do, apply the same pattern

---

## Summary

? **Found all 8 union types**  
? **Found all 8 EventHandlerInfo files**  
? **OnLockMove already implements IUnionEventHandler**  
?? **Ready to update the 8 EventHandlerInfo files**

---

## Next Steps

1. Update `OnCriticalHitEventInfo.cs`
2. Update `OnFlinchEventInfo.cs`
3. Update `OnFractionalPriorityEventInfo.cs`
4. Update `OnNegateImmunityEventInfo.cs`
5. Update `OnTryEatItemEventInfo.cs`
6. Update `OnTryHealEventInfo.cs` (special case - multiple signatures)
7. Update `OnTakeItemEventInfo.cs`
8. Update `OnLockMoveEventInfo.cs` (special case - returns MoveId)

Would you like me to start updating these files?
