# SideResidual Pokemon Parameter Nullability Fix

## Problem Summary
When Tailwind (or any side condition with a `SideResidual` handler) was active and the `SideResidual` event triggered at turn end, the battle crashed with:

```
System.InvalidOperationException: Event SideResidual adapted handler failed on effect Tailwind (Condition)
  Inner exception: Parameter 2 (Pokemon _) is non-nullable but no Pokemon found in context (TargetPokemon=False, SourcePokemon=False)
```

## Root Cause
The `OnSideResidualEventInfo` handler signature declared the `Pokemon` parameter (parameter index 2) as non-nullable in its `ParameterNullability` array:

```csharp
ParameterNullability = new[] { false, false, false, false };
//                                     Battle  Side   Pokemon IEffect
```

However, side-level residual events (like Tailwind countdown, Reflect duration tracking, Light Screen, etc.) don't have a Pokemon context when invoked - they operate on the entire side, not individual Pokemon. When `FieldEvent` called `InvokeEventHandlerInfo` with a `SideSingleEventTarget`, no Pokemon was provided in the context, causing the parameter resolution to fail.

## Solution
Mark the `Pokemon` parameter (index 2) as nullable in `OnSideResidualEventInfo.ParameterNullability` array:

```csharp
// Pokemon parameter (index 2) can be null for side-level residual events like Tailwind
ParameterNullability = new[] { false, false, true, false };
//                                     Battle  Side   Pokemon IEffect
```

## Files Modified

### ApogeeVGC\Sim\Events\Handlers\SideEventMethods\OnSideResidualEventInfo.cs
- **Line 35**: Updated `ParameterNullability` array to mark Pokemon parameter as nullable
- **Added comment**: Explaining that Pokemon can be null for side-level residual events

## Verification
All existing side condition `SideResidual` handlers use discard parameters (`_`) for the Pokemon argument, so they correctly handle null Pokemon. Examples:

**Tailwind** (line 1072 of ConditionsSTU.cs):
```csharp
OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
{
    Order = 26,
    SubOrder = 5,
},
```

**Safeguard** (line 97 of ConditionsSTU.cs):
```csharp
OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
{
    Order = 26,
    SubOrder = 3,
},
```

## Pattern Recognition
This is the **third instance** of this exact pattern:
1. **FieldResidual Pokemon Parameter Nullability Fix** - Field-level residual events (weather)
2. **SwitchIn Null Pokemon Parameter Fix** - Event parameter resolution returning null
3. **SideResidual Pokemon Parameter Nullability Fix** (this fix) - Side-level residual events

## General Rule
When an event can be triggered at different scopes:
- **Field-level events** (weather, terrain) ? Pokemon parameter should be nullable
- **Side-level events** (side conditions like Tailwind, Reflect) ? Pokemon parameter should be nullable  
- **Pokemon-level events** ? Pokemon parameter is non-nullable

The same event ID may have different nullability requirements depending on the invocation scope.

## Impact
- Fixes crashes for all side conditions with `OnSideResidual` handlers (Tailwind, Reflect, Light Screen, Safeguard, etc.)
- Enables proper turn-end processing for side conditions
- Allows battles to progress normally when side conditions are active

## Testing
Verified that:
1. Build succeeds with the change
2. All existing handlers using discard parameters continue to work
3. Pattern matches previous FieldResidual fix

## Keywords
`SideResidual`, `side condition`, `Tailwind`, `Pokemon parameter`, `nullable`, `EventHandlerAdapter`, `parameter nullability`, `OnSideResidualEventInfo`, `side event`, `residual`, `turn end`, `Reflect`, `Light Screen`, `Safeguard`, `ParameterNullability`

## Related Bug Fixes
- [FieldResidual Pokemon Parameter Nullability Fix](FieldResidualPokemonParameterNullabilityFix.md) - Same issue for field-level events
- [SwitchIn Null Pokemon Parameter Fix](SwitchInNullPokemonFix.md) - Related null parameter handling
- [Wind Rider Null SideCondition Fix](WindRiderNullSideConditionFix.md) - Related nullability pattern

## Date
2025-01-20
