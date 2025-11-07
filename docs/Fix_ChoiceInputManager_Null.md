# Fix: _choiceInputManager Null Reference Exception

## Problem

`_choiceInputManager` was null when `RequestChoiceAsync()` was called, causing a runtime exception.

## Root Cause

**MonoGame Lifecycle Timing Issue**:

```
Timeline of Events (BROKEN):
????????????????????????????????????????????????????????????
1. new BattleGame()       ? Constructor
2. game.StartBattle(...)      ? Battle starts on background thread
3. game.Run()         ? MonoGame starts
4.   Initialize()           
5.   LoadContent() ? _choiceInputManager initialized HERE
6.   Battle requests choice   ? TOO LATE! Already tried to use null _choiceInputManager
```

The battle simulation thread tried to call `RequestChoiceAsync()` **before** `LoadContent()` initialized `_choiceInputManager`.

## Solution

**Deferred Battle Start Pattern**:

If `StartBattle()` is called before content is loaded, queue the battle parameters and start it automatically after `LoadContent()` completes.

```csharp
Timeline of Events (FIXED):
????????????????????????????????????????????????????????????
1. new BattleGame()? Constructor
2. game.StartBattle(...) ? Queues battle parameters (_shouldStartBattle = true)
3. game.Run()     ? MonoGame starts
4.   Initialize()
5.   LoadContent()       ? _choiceInputManager initialized
6.     StartBattleInternal()  ? Battle actually starts NOW (safe!)
7.   Battle requests choice   ? SUCCESS! _choiceInputManager is ready
```

## Implementation

### New Fields
```csharp
// Pending battle start data
private Library? _pendingLibrary;
private BattleOptions? _pendingBattleOptions;
private IPlayerController? _pendingPlayerController;
private bool _shouldStartBattle;
```

### Modified `StartBattle()`
```csharp
public void StartBattle(Library library, BattleOptions battleOptions, IPlayerController playerController)
{
    if (_battleRunner != null && _battleRunner.IsRunning)
    {
        Console.WriteLine("Battle is already running");
  return;
    }

    // If content is loaded, start immediately
    if (_choiceInputManager != null)
    {
   StartBattleInternal(library, battleOptions, playerController);
    }
    else
    {
        // Queue the battle to start after LoadContent()
        _pendingLibrary = library;
        _pendingBattleOptions = battleOptions;
        _pendingPlayerController = playerController;
        _shouldStartBattle = true;
    }
}
```

### Modified `LoadContent()`
```csharp
protected override void LoadContent()
{
    // ... existing initialization ...
    
    _choiceInputManager = new ChoiceInputManager(_spriteBatch, _defaultFont, GraphicsDevice);
    
    // If there's a pending battle start, start it now that content is loaded
    if (_shouldStartBattle && _pendingLibrary != null && 
        _pendingBattleOptions != null && _pendingPlayerController != null)
    {
     StartBattleInternal(_pendingLibrary, _pendingBattleOptions, _pendingPlayerController);
        _shouldStartBattle = false;
        _pendingLibrary = null;
      _pendingBattleOptions = null;
        _pendingPlayerController = null;
    }
}
```

## Usage (No Changes Required)

The existing usage pattern continues to work correctly:

```csharp
var game = new BattleGame();
game.StartBattle(library, battleOptions, simulator); // Safe to call before Run()
game.Run();
```

## Alternative Solutions Considered

### Option 1: Document the Requirement
? Require users to call `StartBattle()` only after `LoadContent()` has run.
- **Problem**: Users can't know when `LoadContent()` completes
- **Problem**: Violates principle of least surprise

### Option 2: Initialize in Constructor
? Create `_choiceInputManager` in the constructor.
- **Problem**: Requires `GraphicsDevice` which isn't available yet
- **Problem**: Requires `Content` manager which isn't available yet

### Option 3: Lazy Initialization (Chosen Alternative)
? Check and initialize on-demand in `RequestChoiceAsync()`.
```csharp
public Task<Choice> RequestChoiceAsync(...)
{
    if (_choiceInputManager == null)
    {
     // Wait for LoadContent() to complete
        SpinWait.SpinUntil(() => _choiceInputManager != null, TimeSpan.FromSeconds(5));
     
        if (_choiceInputManager == null)
     throw new InvalidOperationException("LoadContent() never completed");
    }
  
    return _choiceInputManager.RequestChoiceAsync(...);
}
```
- **Problem**: Blocking wait on battle thread
- **Problem**: Arbitrary timeout value

### Option 4: Deferred Start (IMPLEMENTED) ?
Queue battle parameters and start after `LoadContent()` completes.
- **Advantage**: No blocking waits
- **Advantage**: Works with existing usage pattern
- **Advantage**: Clear separation of concerns
- **Advantage**: No race conditions

## Testing

Before fix:
```
game.StartBattle(library, options, controller);
game.Run();

// Result: InvalidOperationException: "Choice input manager not initialized"
```

After fix:
```
game.StartBattle(library, options, controller);
game.Run();

// Result: Battle starts successfully after LoadContent() completes
```

## Related Files
- `ApogeeVGC/Gui/BattleGame.cs` - Fixed
- `ApogeeVGC/Examples/GuiBattleExample.cs` - No changes needed (still works)
