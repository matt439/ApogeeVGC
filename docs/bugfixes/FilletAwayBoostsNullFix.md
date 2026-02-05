# Fillet Away Boosts Null Fix

## Problem Summary

When Fillet Away (or any move with `Boosts` defined) was used, the battle crashed with `ArgumentNullException: Value cannot be null. (Parameter 'Boosts')` in the move's `OnTryHit` handler.

## Error Details

```
System.ArgumentNullException
  Message=Value cannot be null. (Parameter 'Boosts')
  Source=ApogeeVGC
  StackTrace:
   at ApogeeVGC.Data.Moves.Moves.<>c.<CreateMovesDef>b__9_38(Battle battle, Pokemon _, Pokemon _, ActiveMove move) in MovesDEF.cs:line 2000
```

The error occurred when the `OnTryHit` handler tried to access `move.Boosts` to apply stat boosts to the user.

## Root Cause

The `Boosts` property (inherited from `HitEffect`) wasn't being explicitly copied from `Move` to `ActiveMove` in the `ToActiveMove()` method.

**How It Failed**:
1. Fillet Away is defined with `Boosts = new SparseBoostsTable { Atk = 2, SpA = 2, Spe = 2 }`
2. When `ToActiveMove()` creates an `ActiveMove` using object initializer syntax:
   ```csharp
   return new ActiveMove
   {
       Id = Id,
       Name = Name,
       // ... many other properties ...
       // Boosts = Boosts,  // ? MISSING!
   };
   ```
3. Since `Boosts` isn't explicitly initialized, it defaults to `null` instead of copying the value from the source `Move`
4. When `OnTryHit` is called during `SpreadMoveHit`, `move.Boosts` is null
5. The handler checks `if (move.Boosts is null) throw new ArgumentNullException(...)` and crashes

**Type System Context**:
- `HitEffect` (base class) defines `public SparseBoostsTable? Boosts { get; set; }`
- `Move` inherits from `HitEffect`
- `ActiveMove` inherits from `Move`
- Record inheritance provides automatic property copying ONLY for constructor parameters
- Properties set via object initializer must be explicitly listed

## Solution

Added `Boosts = Boosts,` to the object initializer in `Move.ToActiveMove()`:

```csharp
return new ActiveMove
{
    // ... other properties ...
    Self = Self,
    HasSheerForce = HasSheerForce,
    AlwaysHit = AlwaysHit,
    BaseMoveType = BaseMoveType,
    Boosts = Boosts,  // ? ADDED
    BasePowerModifier = BasePowerModifier,
    // ... rest of properties ...
};
```

## Files Modified

- **`ApogeeVGC/Sim/Moves/Move.Core.cs`**: Added `Boosts = Boosts,` to the `ToActiveMove()` object initializer (line ~218)

## Affected Moves

Any move with `Boosts` defined in its move data:
- **Fillet Away**: `{ Atk = 2, SpA = 2, Spe = 2 }` (self-stat boost move that triggered this bug)
- Other self-boost moves if they use the `Boosts` property (though many use `SelfBoost` instead)

## TypeScript Comparison

In TypeScript, object spreading automatically copies all properties:
```typescript
const activeMove = {
    ...move,  // Automatically copies all properties including boosts
    // Additional ActiveMove-specific properties
};
```

In C#, object initializers require explicit property listing, and missing properties default to their type's default value (null for nullable reference types).

## Testing

1. Run a battle with a Pokémon using Fillet Away
2. Verify the move executes successfully
3. Confirm stat boosts are applied correctly (+2 Atk, +2 SpA, +2 Spe)
4. Confirm HP is reduced by 50%
5. No `ArgumentNullException` should be thrown

## Prevention Guidelines

When adding new properties to `HitEffect`, `Move`, or `ActiveMove`:
1. Check if the property needs to be copied in `ToActiveMove()`
2. If it's a move-data property (not runtime state), add it to the object initializer
3. Test moves that use the new property to ensure it's copied correctly

## Related Issues

- **Similar Pattern**: `Secondary` property had a similar issue where it needed to be wrapped in a `Secondaries` array (see Spirit Break fix)
- **Property Inheritance**: This demonstrates that record inheritance doesn't automatically copy properties set via object initializers

---

**Keywords**: `Fillet Away`, `Boosts`, `ArgumentNullException`, `ToActiveMove`, `Move`, `ActiveMove`, `HitEffect`, `property copying`, `object initializer`, `record inheritance`, `stat boost`, `self-boost`
