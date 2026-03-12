# Fractional Speed Integer Division Truncation

## Commit
`d1658217` — Fix fractional speed integer division truncating to zero in ResolvePriority

## Symptom
Switch-in ability ordering (e.g., Intimidate from two same-speed Pokemon) was wrong — abilities fired in the wrong order, and extra PRNG calls from spurious speed-tie shuffles desynchronized the RNG stream.

## Root Cause
In `Battle.Sorting.cs`'s `ResolvePriority`, the fractional speed adjustment for switch-in events was:
```csharp
speed -= SpeedOrder.IndexOf(fieldPositionValue) / (ActivePerHalf * 2);
```

`speed` was `int` (from `EventListener.Speed` and `IPriorityComparison.Speed`), and the division was integer division. For doubles with `ActivePerHalf * 2 = 4`:
- Index 0: `0 / 4 = 0`
- Index 1: `1 / 4 = 0`
- Index 2: `2 / 4 = 0`
- Index 3: `3 / 4 = 0`

All fractional adjustments truncated to zero. In Showdown, `handler.speed` is a JavaScript `number` (float64), correctly producing 0, 0.25, 0.5, 0.75.

When two handlers had the same integer speed, they appeared tied, causing `SpeedSort` to shuffle them (consuming a PRNG call). In Showdown, the fractional speed broke the tie, so no shuffle occurred.

## Fix
Changed `IPriorityComparison.Speed` from `int` to `double` across the interface and all 13 implementing classes. Cast the division in `ResolvePriority` to floating-point: `(double)SpeedOrder.IndexOf(fieldPositionValue) / (ActivePerHalf * 2)`.

## Pattern
**Integer division truncation** — when porting JavaScript code that uses implicit floating-point arithmetic, C# integer types silently truncate. Always verify division operand types when the result should be fractional.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/IPriorityComparison.cs`
- `ApogeeVGC/Sim/Events/EventListener.cs`
- `ApogeeVGC/Sim/BattleClasses/Battle.Sorting.cs`
- `ApogeeVGC/Sim/Actions/*.cs` (all action types)
- `ApogeeVGC/Sim/Choices/ChosenAction.cs`
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Core.cs`
