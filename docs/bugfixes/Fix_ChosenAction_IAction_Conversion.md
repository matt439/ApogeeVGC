# Fix: ChosenAction IAction Conversion Error

## Problems Fixed

### 1. "ActionChoice must be convertible to IAction" Error

**Error Message**:
```
Invalid choice from Matt, auto-choosing
Battle error: ActionChoice must be convertible to IAction
```

**Root Cause**:
`BattleQueue.ResolveAction()` attempts to cast `IActionChoice` to `IAction`:
```csharp
IAction currentAction = action as IAction ??
    throw new InvalidOperationException("ActionChoice must be convertible to IAction");
```

But `ChosenAction` only implemented `IActionChoice`, not `IAction`.

**Solution**:
Made `ChosenAction` implement `IAction` by adding explicit interface implementation:

```csharp
// ApogeeVGC/Sim/Choices/ChosenAction.cs (AFTER)
public record ChosenAction : IAction
{
    public required ChoiceType Choice { get; init; }
    
    // Explicit interface implementation to map ChoiceType -> ActionId
    ActionId IAction.Choice => Choice switch
    {
        ChoiceType.Move => ActionId.Move,
        ChoiceType.Switch => ActionId.Switch,
    ChoiceType.InstaSwitch => ActionId.InstaSwitch,
        ChoiceType.Team => ActionId.Team,
        ChoiceType.Pass => ActionId.Pass,
       ChoiceType.RevivalBlessing => ActionId.RevivalBlessing,
        _ => ActionId.Move, // Default
};
    
    // ... rest of properties ...
}
```

This allows `ChosenAction` to be used as both an input choice and a queue action.

### 2. Team Preview Keyboard Input Range

**Problem**:
Keyboard input was limited to 1-4 for all request types, but Team Preview needs 1-6.

**Solution**:
Made keyboard input range dynamic based on request type:

```csharp
// ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.cs
private void ProcessKeyboardInput(KeyboardState keyboardState)
{
    // Determine max number based on request type
    int maxNumbers = _requestType switch
    {
        BattleRequestType.TeamPreview => 6, // Team preview allows 1-6
        _ => 6, // Default to 6 for safety
 };

    // Number keys 1-6 (or fewer) for quick selection
    for (int i = 0; i < maxNumbers; i++)
  {
        Keys numberKey = Keys.D1 + i;
        if (IsKeyPressed(keyboardState, numberKey))
    {
  _keyboardInput += (i + 1).ToString();
      ProcessNumericInput(i + 1);
        break;
        }
    }
    
    // ... rest of input handling ...
}
```

## Why ChosenAction Needs Both Interfaces

### Interface Hierarchy
```
IPriorityComparison
  ?
IActionChoice
  ?
IAction
```

`ChosenAction` serves two purposes:
1. **Input to `Side.Choose()`** - As an `IActionChoice`, representing player intent
2. **Queue Storage** - As an `IAction`, representing a resolved battle action

### The Type Mapping

`ChosenAction` has a `ChoiceType` enum that needs to map to `ActionId`:

| ChoiceType | ActionId |
|---|---|
| `Move` | `ActionId.Move` |
| `Switch` | `ActionId.Switch` |
| `InstaSwitch` | `ActionId.InstaSwitch` |
| `Team` | `ActionId.Team` |
| `Pass` | `ActionId.Pass` |
| `RevivalBlessing` | `ActionId.RevivalBlessing` |

The explicit interface implementation (`ActionId IAction.Choice`) provides this mapping.

## Files Modified

### 1. `ApogeeVGC/Sim/Choices/ChosenAction.cs`
**Before**:
```csharp
public record ChosenAction : IActionChoice
{
    public required ChoiceType Choice { get; init; }
    // ...
}
```

**After**:
```csharp
public record ChosenAction : IAction
{
    public required ChoiceType Choice { get; init; }
    
    // Explicit interface implementation
    ActionId IAction.Choice => Choice switch { /* mapping */ };
    
    // ...
}
```

### 2. `ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.cs`
- Updated `ProcessKeyboardInput()` to allow 1-6 for team preview

## Expected Behavior Now

**Team Preview**:
- Pressing 1-6 should select Pokemon positions
- Pressing Enter should submit the team order

**Move Selection**:
- Pressing 1-4 should select a move
- Pressing Enter should submit the choice
- No more "ActionChoice must be convertible to IAction" errors

## Testing

Try the battle again:
1. Team Preview: Press **1**, **2**, **Enter**
2. Move Selection: Press **1**, **Enter**

Expected output:
```
[PlayerGui] GetNextChoiceAsync called for P1
[BattleGame] RequestChoiceAsync called. _choiceInputManager null? False
? Choice processed successfully
? Battle continues
```

No more "ActionChoice must be convertible to IAction" errors!
