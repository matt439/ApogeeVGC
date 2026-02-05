# Wind Rider Null SideCondition Fix

## Problem Summary
Wind Rider ability crashed with `NullReferenceException` when trying to access `sideCondition.Id` in its `OnSideConditionStart` handler at line 446 of `AbilitiesVWX.cs`. The error occurred during random battle testing when a Pokémon with Wind Rider was on the field and a side condition was added.

**Error Message:**
```
System.NullReferenceException: Object reference not set to an instance of an object.
  at ApogeeVGC.Data.Abilities.Abilities.<>c.<CreateAbilitiesVwx>b__15_24(Battle battle, Side _, Pokemon _, Condition sideCondition)
```

## Root Cause
The `OnSideConditionStart` event handler did not check if the `sideCondition` parameter was null before accessing its `Id` property. The parameter was marked as non-nullable in the `OnSideConditionStartEventInfo` signature (`ParameterNullability = [false, false, false, false]`), but could legitimately be null in certain contexts when side condition events are triggered without an associated condition.

### Why Can sideCondition Be Null?
When `EventHandlerAdapter.ResolveParameter` tries to resolve a `Condition` parameter (which implements `IEffect`), it returns `context.SourceEffect` at lines 230-232:

```csharp
if (typeof(IEffect).IsAssignableFrom(paramType))
{
    return context.SourceEffect;
}
```

The adapter returns `context.SourceEffect` even if it's null. The nullability check at lines 236-242 only throws an exception if:
1. No value is resolved at all (we return before the check), AND
2. The parameter is marked as non-nullable

Since we DO return a value (even though it's null), the nullability check doesn't trigger.

## Solution
Applied a two-part fix following the pattern from previous similar fixes (Ripen, Disguise, Adrenaline Orb, Berserk):

### 1. Added Null Guard in Wind Rider Handler
Added explicit null check before accessing `sideCondition.Id`:

```csharp
if (sideCondition != null && sideCondition.Id == ConditionId.Tailwind)
{
    pokemon.AddVolatile(ConditionId.Charge);
}
```

### 2. Updated Parameter Nullability
Modified `OnSideConditionStartEventInfo.cs` to mark the `sideCondition` parameter as nullable:

**Handler Signature:**
```csharp
// Before:
Action<Battle, Side, Pokemon, Condition> handler

// After:
Action<Battle, Side, Pokemon, Condition?> handler
```

**Nullability Array:**
```csharp
// Before:
ParameterNullability = [false, false, false, false];

// After (position 3 is now nullable):
ParameterNullability = [false, false, false, true];
```

**Documentation Comment:**
```csharp
// Nullability: sideCondition parameter can be null (position 3)
```

## TypeScript Reference
In TypeScript, the signature is:
```typescript
onSideConditionStart(side, source, sideCondition) {
    const pokemon = this.effectState.target;
    if (sideCondition.id === 'tailwind') {
        this.boost({ atk: 1 }, pokemon, pokemon);
    }
}
```

TypeScript doesn't throw when accessing properties on `undefined`, so the code runs without explicit null checks. C# requires defensive null checking.

## Impact
When `sideCondition` is null, Wind Rider no longer crashes. The handler simply doesn't activate the Charge volatile status, which is correct behavior since there's no side condition to check against.

This prevents crashes while maintaining correct game mechanics where Wind Rider only activates when Tailwind specifically starts.

## Files Modified
1. **ApogeeVGC\Data\Abilities\AbilitiesVWX.cs**
   - Added `sideCondition != null` check before accessing `.Id` property

2. **ApogeeVGC\Sim\Events\Handlers\EventMethods\OnSideConditionStartEventInfo.cs**
   - Changed handler signature to accept `Condition?` (nullable)
   - Updated `ParameterNullability` array: `[false, false, false, true]`
   - Updated documentation comment

## Testing
After the fix, random battles with Wind Rider should no longer crash when side conditions are added. The ability correctly activates only when Tailwind is started and the Pokemon is on the field.

## Related Bug Fixes
This follows the same pattern as several previous fixes documented in INDEX.md:
- **Ripen Ability Null Effect Fix**: Added null guard for `effect` parameter in `OnTryHeal`
- **Disguise Ability Null Effect Fix**: Added null guard for `effect` parameter in `OnDamage`
- **Adrenaline Orb Null Effect Fix**: Added null guard for `effect` parameter in `OnAfterBoost`
- **Berserk Ability Null Effect Fix**: Added null guard for `effect` parameter in `OnDamage`

All of these share the same pattern: event parameters that implement `IEffect` (or its derived types like `Condition`) can be null when events are triggered from sources that don't have an associated effect.

## Prevention Guidelines
When creating new event handlers with `IEffect`, `Condition`, `Move`, `Ability`, or `Item` parameters:

1. **Consider nullable contexts**: Can this event be triggered without an associated effect?
2. **Mark as nullable**: If yes, mark the parameter as nullable in both the handler signature AND the `ParameterNullability` array
3. **Add defensive checks**: Always check for null before accessing properties on effect parameters
4. **Follow TypeScript semantics**: If TypeScript doesn't crash without null checks, C# needs explicit guards

## Keywords
`Wind Rider`, `ability`, `OnSideConditionStart`, `null reference`, `sideCondition parameter`, `Tailwind`, `side condition`, `null check`, `IEffect`, `EventHandlerAdapter`, `parameter nullability`, `Condition`, `defensive programming`
