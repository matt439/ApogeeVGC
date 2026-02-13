# Wind Power OnSideConditionStart Parameter Resolution Fix

## Summary
**Severity**: High  
**Systems Affected**: EventHandlerAdapter parameter resolution for any handler with a `Condition`-typed parameter named `sideCondition` (or any name containing "side" but not typed as `Side`)

## Problem
When Kilowattrel's ally used Tailwind, the Wind Power ability's `OnSideConditionStart` handler crashed with a `NullReferenceException` at line 446 of `AbilitiesVWX.cs`:

```
System.NullReferenceException: Object reference not set to an instance of an object.
   at ApogeeVGC.Data.Abilities.Abilities.<>c.<CreateAbilitiesVwx>b__15_24(
       Battle battle, Side _, Pokemon _, Condition sideCondition)
```

The `sideCondition` parameter was `null` when it should have been the Tailwind `Condition` object.

## Root Cause
In `EventHandlerAdapter.ResolveParameter`, the name-based parameter matching checked parameter names in order. The parameter `sideCondition` (type `Condition`) contained the substring `"side"`, which matched the name-based check for Side parameters:

```csharp
if (paramName.Contains("side"))
{
    return context.TargetSide;  // returns null — wrong type anyway
}
```

This check fired before the type-based `Condition` check could match. Since `context.TargetSide` was `null` (the event target was a `Side` but `RunEvent` only populates `Event.Target` for `PokemonRunEventTarget`), the handler received `null` for the `sideCondition` parameter.

### Event flow
1. `AddSideCondition` calls `Battle.RunEvent(EventId.SideConditionStart, side, source, tailwindCondition)`
2. `tailwindCondition` is passed as `sourceEffect` (4th parameter)
3. Wind Power's `OnSideConditionStart` handler is found and invoked
4. `ResolveParameter` for position 3 (`Condition sideCondition`):
   - Name `"sidecondition"` contains `"side"` → returns `context.TargetSide` (null) ← **BUG**
   - Should have reached the Condition type check or IEffect fallback which would correctly return `context.SourceEffect` (the Tailwind Condition)

## Fix

### 1. Add type guard to name-based "side" check
The name-based "side" check now also verifies the parameter type is actually `Side`:

```csharp
// Before (broken)
if (paramName.Contains("side"))
{
    return context.TargetSide;
}

// After (fixed)
if (paramName.Contains("side") &&
    (paramType == typeof(Side) || typeof(Side).IsAssignableFrom(paramType)))
{
    return context.TargetSide;
}
```

### 2. Extend Condition type check to also check SourceEffect
The Condition type check previously only checked `context.Effect`. Since in this case `context.Effect` is the Wind Power ability (not a Condition), we also check `context.SourceEffect`:

```csharp
if (paramType == typeof(Condition) || typeof(Condition).IsAssignableFrom(paramType))
{
    if (context.Effect is Condition condition)
    {
        return condition;
    }
    if (context.SourceEffect is Condition sourceCondition)
    {
        return sourceCondition;
    }
}
```

## Files Changed
- `ApogeeVGC/Sim/Events/EventHandlerAdapter.cs` — `ResolveParameter` method

## Reproduction Seeds
- Team1: 56172, Team2: 69742, P1: 14198, P2: 3672, Battle: 11731

## Keywords
`Wind Power`, `Tailwind`, `OnSideConditionStart`, `ResolveParameter`, `name-based matching`, `NullReferenceException`, `parameter resolution`, `sideCondition`
