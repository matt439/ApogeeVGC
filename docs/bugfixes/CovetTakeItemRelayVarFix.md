# Covet TakeItem Event RelayVar Fix

**Date**: 2025-01-XX  
**Severity**: Medium  
**Systems Affected**: Item-stealing moves (Covet), TakeItem event handlers

## Problem

When the **Covet** move attempted to steal an item (such as Mind Plate) from an opponent, it triggered the `TakeItem` event incorrectly, causing an `InvalidOperationException`:

```
System.InvalidOperationException: Event TakeItem adapted handler failed on effect Mind Plate (Item)
Inner exception: ArgumentException: Object of type 'ApogeeVGC.Sim.Moves.ActiveMove' cannot be converted to type 'ApogeeVGC.Sim.Items.Item'.
```

### Root Cause

The Covet move's `OnAfterHit` handler invoked the TakeItem event without passing the item as the `relayVar` parameter:

```csharp
// BEFORE (incorrect)
RelayVar? takeItemResult = battle.SingleEvent(EventId.TakeItem, yourItem,
    target.ItemState, source, target, move);
```

The `TakeItem` event handler signature expects:
```csharp
Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>
```

When resolving parameters, the `EventHandlerAdapter` uses the `relayVar` to provide type-specific values. Without the item passed as `relayVar`, the adapter could not correctly resolve the `Item` parameter for handlers like Mind Plate's `OnTakeItem`, leading to type mismatch errors.

### TypeScript Reference

In Pokemon Showdown's TypeScript source, the TakeItem event is called with the item passed as both `effect` and `relayVar`:

```typescript
// Covet move (moves.ts:3225)
!this.singleEvent('TakeItem', yourItem, target.itemState, source, target, move, yourItem)
//                             ^^^^^^^^                                              ^^^^^^^^
//                             effect param                                          relayVar param
```

This pattern is used by all item-stealing/swapping moves (Covet, Thief, Trick, Switcheroo, etc.).

## Solution

Added the item as the `relayVar` parameter when invoking the TakeItem event:

```csharp
// AFTER (correct)
RelayVar? takeItemResult = battle.SingleEvent(EventId.TakeItem, yourItem,
    target.ItemState, source, target, move, yourItem);
//                                          ^^^^  ^^^^^^^^
//                                          sourceEffect  relayVar
```

### How RelayVar Resolution Works

With the item passed as `relayVar`, the `EventHandlerAdapter` can resolve the `Item` parameter using the unwrapping logic at lines 118-122:

```csharp
// EventHandlerAdapter.cs:118-122
if (context.RelayVar is EffectRelayVar effectRelayVar && 
    paramType.IsAssignableFrom(effectRelayVar.Effect.GetType()))
{
    return effectRelayVar.Effect;  // Returns the unwrapped Item
}
```

Since `Item` has an implicit conversion to `RelayVar` (specifically to `EffectRelayVar`), passing `yourItem` as the `relayVar` allows it to be correctly unwrapped and passed to the handler's `Item` parameter.

## Files Changed

- **ApogeeVGC/Data/Moves/MovesABC.cs** (line 2479)
  - Added `yourItem` as the `relayVar` parameter to the `SingleEvent` call in Covet's `OnAfterHit` handler

## Verification

Other item-related moves were checked and already had the correct pattern:
- ? Fling (MovesDEF.cs:2492)
- ? Knock Off (MovesJKL.cs:144)
- ? Switcheroo (MovesSTU.cs:3194, 3198)
- ? Thief (MovesSTU.cs:3912)
- ? Trick (MovesSTU.cs:4602, 4606)

Only Covet was missing the `relayVar` parameter.

## Testing

The fix was verified by:
1. Successful compilation after the change
2. Resolution of the exception that occurred during incremental testing with Imprison move (the bug was triggered during random battles where Covet was used against items like Mind Plate)

## Keywords

`TakeItem event`, `item stealing`, `Covet`, `Mind Plate`, `relayVar`, `EventHandlerAdapter`, `parameter resolution`, `type mismatch`

## Related Issues

- This follows the same pattern as other item-related events that need the item passed as both `effect` and `relayVar`
- Similar to how `EffectRelayVar` unwrapping was used in previous fixes for `TryAddVolatile` events

## Prevention

When implementing moves that:
- Steal, swap, or remove items
- Trigger item-related events (TakeItem, End, etc.)

Always check the TypeScript reference to ensure all required parameters (including `relayVar`) are passed to `SingleEvent` calls.
