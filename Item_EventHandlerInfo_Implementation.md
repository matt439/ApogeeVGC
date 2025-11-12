# Item EventHandlerInfo Implementation

## Summary

Successfully updated 4 Item-specific properties to use the new EventHandlerInfo pattern:

### 1. **OnEatEventInfo** (Union Type)
- **File**: `ApogeeVGC\Sim\Events\Handlers\ItemSpecific\OnEatEventInfo.cs`
- **Union Type**: `OnItemEatUse` = `Action<Battle, Pokemon>` | `false`
- **Event ID**: `EventId.Eat`
- **Signature**: `(Battle, Pokemon) => void`
- **Special Features**:
  - Handles union type with `UnionValue` property
  - `IsConstantValue()` checks if value is `false`
  - `GetConstantValue()` returns `false` when constant
  - `Invoke()` method handles both delegate and constant cases

### 2. **OnUseEventInfo** (Union Type)
- **File**: `ApogeeVGC\Sim\Events\Handlers\ItemSpecific\OnUseEventInfo.cs`
- **Union Type**: `OnItemEatUse` = `Action<Battle, Pokemon>` | `false`
- **Event ID**: `EventId.Use`
- **Signature**: `(Battle, Pokemon) => void`
- **Special Features**: Same as OnEatEventInfo

### 3. **OnStartEventInfo** (Simple Delegate)
- **File**: `ApogeeVGC\Sim\Events\Handlers\ItemSpecific\OnStartEventInfo.cs`
- **Type**: `Action<Battle, Pokemon>`
- **Event ID**: `EventId.Start`
- **Signature**: `(Battle, Pokemon) => void`

### 4. **OnEndEventInfo** (Simple Delegate)
- **File**: `ApogeeVGC\Sim\Events\Handlers\ItemSpecific\OnEndEventInfo.cs`
- **Type**: `Action<Battle, Pokemon>`
- **Event ID**: `EventId.End`
- **Signature**: `(Battle, Pokemon) => void`

## Changes Made

### Item.Core.cs
**Before:**
```csharp
public OnItemEatUse? OnEat { get; init; }
public OnItemEatUse? OnUse { get; init; }
public Action<Battle, Pokemon>? OnStart { get; init; }
public Action<Battle, Pokemon>? OnEnd { get; init; }
```

**After:**
```csharp
public OnEatEventInfo? OnEat { get; init; }
public OnUseEventInfo? OnUse { get; init; }
public OnStartEventInfo? OnStart { get; init; }
public OnEndEventInfo? OnEnd { get; init; }
```

### Data\Items.cs
**Before:**
```csharp
OnStart = (battle, pokemon) => { /* handler code */ }
```

**After:**
```csharp
OnStart = new OnStartEventInfo
{
    Handler = (Action<Battle, Pokemon>)((battle, pokemon) => { /* handler code */ })
}
```

## Implementation Notes

### Union Type Handling
Since `OnItemEatUse` doesn't implement `IUnionEventHandler`, we couldn't use `UnionEventHandlerInfo<TUnion>` as a base class. Instead:

1. Inherit directly from `EventHandlerInfo`
2. Add `UnionValue` property of type `OnItemEatUse?`
3. Override `Handler` property to extract delegate from union
4. Implement helper methods:
   - `IsConstantValue()` - checks if union contains `false`
   - `GetConstantValue()` - returns `false` when constant
   - `Invoke(params object[] args)` - handles both cases

### Pattern Matching
Uses C# pattern matching to distinguish union cases:
- `OnItemEatUseFunc` contains the delegate
- `OnItemEatUseFalse` represents the `false` constant

### Type Safety
All EventHandlerInfo types include:
- `ExpectedParameterTypes` for compile-time validation
- `ExpectedReturnType` for return type checking
- Proper XML documentation

## Usage Example

```csharp
// Creating an item with OnStart handler
var item = new Item
{
    Id = ItemId.ChoiceSpecs,
    Name = "Choice Specs",
    OnStart = new OnStartEventInfo
    {
        Handler = (Action<Battle, Pokemon>)((battle, pokemon) =>
   {
            // Handler implementation
        })
    }
};

// Accessing the handler
if (item.OnStart?.Handler is Action<Battle, Pokemon> handler)
{
    handler(battle, pokemon);
}

// Using union types
var berryItem = new Item
{
    OnEat = new OnEatEventInfo
    {
        UnionValue = new OnItemEatUseFunc((battle, pokemon) => 
        {
  // Eat handler
        })
  }
};

// Or with false constant
var nonEdibleItem = new Item
{
    OnEat = new OnEatEventInfo
    {
        UnionValue = new OnItemEatUseFalse()
    }
};
```

## Compilation Status
? All Item-specific EventHandlerInfo types compile successfully
? Item.Core.cs updated and compiling
? Data\Items.cs updated with proper using directives and type casts
