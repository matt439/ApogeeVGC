# OnFlinch Event Parameter Mismatch Fix

## Problem Summary

When a Pokémon with the Steadfast ability flinched, the battle crashed with:
```
InvalidOperationException: EventHandlerInfo validation failed for event Flinch on effect Steadfast (Ability): 
Event Flinch: Expected 2 parameters, got 4. Expected: (Battle, Pokemon), Got: (Battle, Pokemon, Object, Move)
```

## Root Cause

There was a mismatch between:
1. `OnFlinchEventInfo.ExpectedParameterTypes` - defined as `[Battle, Pokemon]` (2 parameters)
2. `OnFlinch` union type delegate - defined as `Func<Battle, Pokemon, object?, Move, BoolVoidUnion>` (4 parameters)
3. Steadfast ability's `OnFlinch` handler - using the 4-parameter signature

The confusion arose from the TypeScript type signature in a comment:
```typescript
((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) => boolean | void) | boolean
```

However, in TypeScript:
- `this: Battle` is the context binding, not a real parameter
- When `runEvent('Flinch', pokemon)` is called (in conditions.ts line 204), only `pokemon` is passed
- `source` and `move` are always `undefined`/`null` in practice

## Solution

Simplified all signatures to match the actual TypeScript behavior:

### 1. OnFlinch Union Type (OnFlinch.cs)
Changed from 4-parameter to 2-parameter delegate:
```csharp
// Before
Func<Battle, Pokemon, object?, Move, BoolVoidUnion>

// After
Func<Battle, Pokemon, BoolVoidUnion>
```

### 2. OnFlinchEventInfo (OnFlinchEventInfo.cs)
Already had correct 2-parameter types, kept as-is:
```csharp
ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
```

### 3. Steadfast Ability (AbilitiesSTU.cs)
Updated handler to use 2-parameter signature:
```csharp
// Before
(Func<Battle, Pokemon, object?, Move, BoolVoidUnion>)((battle, _, _, _) => ...)

// After
(Func<Battle, Pokemon, BoolVoidUnion>)((battle, _) => ...)
```

## Files Modified

- `ApogeeVGC\Sim\Utils\Unions\OnFlinch.cs` - Changed delegate signature to 2 parameters
- `ApogeeVGC\Sim\Events\Handlers\EventMethods\OnFlinchEventInfo.cs` - Added clarifying comment (signature was already correct)
- `ApogeeVGC\Data\Abilities\AbilitiesSTU.cs` - Updated Steadfast handler signature

## Key Insight

When porting from TypeScript to C#:
- `this: T` in TypeScript type signatures represents the calling context, not a parameter
- Always check what arguments are **actually passed** to `runEvent()`, not just the type signature
- The `runEvent` call in conditions.ts only passes `pokemon`, so handlers should only expect `(Battle, Pokemon)`

## TypeScript Reference

```typescript
// conditions.ts line 204
this.runEvent('Flinch', pokemon);

// abilities.ts - Steadfast
onFlinch(pokemon) {
    this.boost({ spe: 1 });
}
```

## Related Documentation

See `UnionTypeHandlingGuide.md` for comprehensive guidance on Union type and EventHandlerInfo signature alignment.

---

**Keywords**: `OnFlinch`, `Steadfast`, `event parameter`, `signature mismatch`, `EventHandlerInfo`, `validation failed`, `union type`, `delegate signature`, `TypeScript this binding`
