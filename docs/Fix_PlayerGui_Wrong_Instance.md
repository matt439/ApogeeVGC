# Fix: PlayerGui Using Wrong BattleGame Instance

## Problem

`_choiceInputManager` was still null in `BattleGame.RequestChoiceAsync()` even after the deferred start fix, confirmed with breakpoint.

## Root Cause

**Wrong Instance Problem**: `PlayerGui` was creating a **new instance** of `BattleGame` instead of using the one passed in `PlayerOptions`.

### The Bug

```csharp
// PlayerGui.cs (BEFORE - BROKEN)
public class PlayerGui(SideId sideId, PlayerOptions options, IBattleController battleController) : IPlayer
{
  // ...
    public BattleGame GuiWindow { get; set; } = new(); // ? Always creates NEW instance
}
```

```csharp
// Simulator.cs (BEFORE)
private PlayerGui CreateGuiPlayer(SideId sideId, PlayerOptions options)
{
    var playerGui = new PlayerGui(sideId, options, this);
    
    // This tries to set GuiWindow AFTER construction
    if (options.GuiWindow is { } battleGame)
    {
        playerGui.GuiWindow = battleGame; // ? TOO LATE! Default already set
    }
    
    return playerGui;
}
```

### What Was Happening

```
User Code:
??????????????????????????????????????????????????
var game = new BattleGame();  // Instance #1 (the REAL game window)

var options = new PlayerOptions
{
  GuiWindow = game,  // Pass Instance #1
    // ...
};

game.StartBattle(library, options, simulator);
game.Run();  // Runs Instance #1


Simulator.CreateGuiPlayer():
??????????????????????????????????????????????????
1. new PlayerGui(...) is called
   ? PlayerGui constructor runs
   ? GuiWindow = new()  // Creates Instance #2 (wrong!)
   
2. Try to set options.GuiWindow
   ? playerGui.GuiWindow = game  // Instance #1
   ? BUT this is a property setter, not constructor
   ? The assignment fails to replace the default

Result:
??????????????????????????????????????????????????
PlayerGui.GuiWindow = Instance #2 (new BattleGame with no initialization)
User's game variable = Instance #1 (the real game running in game.Run())

When battle requests choice:
  PlayerGui calls Instance #2.RequestChoiceAsync()
    Instance #2 never had LoadContent() called
    Instance #2._choiceInputManager is NULL
    ? CRASH!
```

## Solution

**Use Constructor Parameter**: Initialize `GuiWindow` from `options.GuiWindow` in the `PlayerGui` constructor.

### Fixed Code

```csharp
// PlayerGui.cs (AFTER - FIXED)
public class PlayerGui : IPlayer
{
    public BattleGame GuiWindow { get; set; }

  public PlayerGui(SideId sideId, PlayerOptions options, IBattleController battleController)
    {
        SideId = sideId;
        Options = options;
        BattleController = battleController;
        
        // ? Use the GuiWindow from options if provided
        GuiWindow = options.GuiWindow ?? new BattleGame();
    }
    
    // ...
}
```

```csharp
// Simulator.cs (AFTER - SIMPLIFIED)
private PlayerGui CreateGuiPlayer(SideId sideId, PlayerOptions options)
{
    // ? PlayerGui constructor handles GuiWindow from options
return new PlayerGui(sideId, options, this);
}
```

### Now It Works Correctly

```
User Code:
??????????????????????????????????????????????????
var game = new BattleGame();  // Instance #1

var options = new PlayerOptions
{
    GuiWindow = game,  // Pass Instance #1
};

game.StartBattle(library, options, simulator);
game.Run();  // Runs Instance #1


Simulator.CreateGuiPlayer():
??????????????????????????????????????????????????
1. new PlayerGui(sideId, options, this) is called
   ? PlayerGui constructor runs
   ? GuiWindow = options.GuiWindow ?? new BattleGame()
   ? GuiWindow = game (Instance #1) ?

Result:
??????????????????????????????????????????????????
PlayerGui.GuiWindow = Instance #1 (the SAME instance running game.Run())
User's game variable = Instance #1

When battle requests choice:
  PlayerGui calls Instance #1.RequestChoiceAsync()
    Instance #1 had LoadContent() called
    Instance #1._choiceInputManager is initialized ?
    ? SUCCESS!
```

## Why The Property Initializer Didn't Work

C# primary constructors execute **before** property initializers, but the issue here was different:

```csharp
// This DOESN'T work as expected:
public class PlayerGui(...) : IPlayer
{
    public BattleGame GuiWindow { get; set; } = new(); // ? Runs DURING construction
}

// Later trying to set it:
playerGui.GuiWindow = options.GuiWindow; // ? This is just a property assignment
```

The property initializer `= new()` sets the **default value** during construction. The subsequent property assignment `playerGui.GuiWindow = ...` is just a normal property setter call, which **does work**, but it happens **after** the object is already constructed with the default value.

The real problem was in the **Simulator** code trying to set it post-construction instead of passing it to the constructor.

## Files Changed

### `ApogeeVGC/Player/PlayerGui.cs`
**Before**:
- Used primary constructor with property initializer
- Always created new `BattleGame()` instance

**After**:
- Explicit constructor with parameters
- Uses `options.GuiWindow ?? new BattleGame()`
- Correctly uses the provided instance

### `ApogeeVGC/Sim/Core/Simulator.cs`
**Before**:
- Tried to set `GuiWindow` after construction
- Redundant null-coalescing in `CreateGuiPlayer()`

**After**:
- Simplified to just `new PlayerGui(sideId, options, this)`
- Constructor handles the logic

## Testing

**Before Fix**:
```csharp
var game = new BattleGame();
var options = new PlayerOptions { GuiWindow = game, ... };
game.StartBattle(library, options, simulator);
game.Run();

// Result: InvalidOperationException in RequestChoiceAsync()
// _choiceInputManager is null because wrong BattleGame instance
```

**After Fix**:
```csharp
var game = new BattleGame();
var options = new PlayerOptions { GuiWindow = game, ... };
game.StartBattle(library, options, simulator);
game.Run();

// Result: ? Works correctly!
// _choiceInputManager is initialized because correct instance is used
```

## Key Lessons

1. **Be careful with property initializers** - they set default values, not constructor parameters
2. **Pass dependencies through constructors** - don't try to set them post-construction
3. **Primary constructors are concise** but explicit constructors are sometimes clearer for complex initialization
4. **Always use the same instance** when sharing state between components

## Related Issues

This fix also resolves:
- Battle using uninitialized `BattleGame` instance
- GUI not rendering because it's the wrong instance
- Choice requests going to orphaned `BattleGame` objects
