# Fix: Players Not Initialized in BattleRunner

## Problem

The battle was stuck in a loop with the error:
```
Battle error: Player 1 is not initialized
Battle complete! Winner:
Battle complete! Winner:
... (repeating indefinitely)
```

## Root Cause

**`BattleRunner` was creating `Battle` directly without creating players:**

```csharp
// BEFORE (BROKEN)
public class BattleRunner
{
    public void StartBattle()
    {
        Battle = new Battle(_battleOptions, _library, _playerController);
        
  _battleTask = Task.Run(async () =>
        {
       await Battle.StartAsync(_cancellationTokenSource.Token);
    // ...
  });
    }
}
```

**The Problem**:
- `Battle` needs players (`Player1` and `Player2`) to be created
- `Simulator.Run()` is responsible for creating the players via `CreatePlayer()`
- `BattleRunner` was bypassing this and creating `Battle` directly
- Result: Battle started with `Player1 = null` and `Player2 = null`

**Why It Looped**:
The `BattleGame.Update()` method checks `_battleRunner.IsCompleted` and prints "Battle complete!" repeatedly:
```csharp
if (_battleRunner is { IsCompleted: true })
{
  Console.WriteLine($"Battle complete! Winner: {_battleRunner.Result}");
}
```

Since the battle task completed immediately (due to error), this condition was true on every frame of the game loop, causing the infinite spam.

## The Fix

**Use `Simulator.Run()` which properly creates players:**

```csharp
// AFTER (FIXED)
public class BattleRunner
{
    private readonly Simulator _simulator;
    
    public BattleRunner(Library library, BattleOptions battleOptions, Simulator simulator)
    {
        _library = library;
        _battleOptions = battleOptions;
        _simulator = simulator; // ? Store simulator instance
    }
    
    public void StartBattle()
    {
        _battleTask = Task.Run(async () =>
     {
     // ? Use Simulator.Run() which creates players and battle
       SimulatorResult result = await _simulator.Run(_library, _battleOptions, printDebug: true);
  Result = result;
            return result;
  });
    }
}
```

### What `Simulator.Run()` Does

```csharp
public async Task<SimulatorResult> Run(Library library, BattleOptions battleOptions, bool printDebug = true)
{
    // ? Creates the battle
 Battle = new Battle(battleOptions, library, this);
  
    // ? Creates player 1
    Player1 = CreatePlayer(SideId.P1, battleOptions.Player1Options);
    
  // ? Creates player 2
    Player2 = CreatePlayer(SideId.P2, battleOptions.Player2Options);
    
    // ? Starts the battle
    await Battle.StartAsync(cancellationTokenSource.Token);
    
    // ? Determines winner
    return DetermineWinner();
}
```

## Files Modified

### 1. `ApogeeVGC/Sim/Core/BattleRunner.cs`

**Before**:
```csharp
private readonly IPlayerController _playerController;
public Battle? Battle { get; private set; }

public BattleRunner(Library library, BattleOptions battleOptions, IPlayerController playerController)
{
    _playerController = playerController;
}

public void StartBattle()
{
  Battle = new Battle(_battleOptions, _library, _playerController);
    _battleTask = Task.Run(async () =>
    {
        await Battle.StartAsync(_cancellationTokenSource.Token);
        SimulatorResult result = DetermineWinner(Battle);
        // ...
    });
}
```

**After**:
```csharp
private readonly Simulator _simulator;
public Battle? Battle => _simulator.Battle;

public BattleRunner(Library library, BattleOptions battleOptions, Simulator simulator)
{
    _simulator = simulator;
}

public void StartBattle()
{
    _battleTask = Task.Run(async () =>
    {
   SimulatorResult result = await _simulator.Run(_library, _battleOptions, printDebug: true);
        Result = result;
        return result;
    });
}
```

### 2. `ApogeeVGC/Gui/BattleGame.cs`

**Added type check**:
```csharp
private void StartBattleInternal(Library library, BattleOptions battleOptions, IPlayerController playerController)
{
    // Ensure we have a Simulator instance
    if (playerController is not Simulator simulator)
  {
        throw new InvalidOperationException("PlayerController must be a Simulator instance for GUI battles");
    }
    
    _battleRunner = new BattleRunner(library, battleOptions, simulator);
    _battleRunner.StartBattle();
}
```

## Expected Console Output After Fix

```
[Driver] BattleGame created, instance: 30015890
[Driver] Simulator created
[BattleGame] StartBattle called. _choiceInputManager null? True
[BattleGame] Content not loaded yet, queueing battle start
[Driver] Battle queued, calling battleGame.Run()
[BattleGame] LoadContent() called
[BattleGame] ChoiceInputManager initialized successfully
[BattleGame] Starting pending battle...
[BattleGame] StartBattleInternal called
[BattleGame] Battle runner started
Starting battle simulation...
Battle constructor complete.
[PlayerGui] Constructor called for P1
[PlayerGui] GuiWindow from options: 30015890
[PlayerGui] GuiWindow assigned: 30015890
[PlayerRandom] Constructor called for P2  ? Player 2 created!
... (battle proceeds normally)
```

## Why This Architecture

The `Simulator` class has several responsibilities:
1. **Create players** based on `PlayerOptions.Type`
2. **Create the battle** with the correct player controller
3. **Manage the battle lifecycle**
4. **Implement `IPlayerController`** to route choice requests to the correct player

By using `Simulator.Run()`, we ensure all these responsibilities are handled correctly.

## Bonus Fix: Prevent Infinite Loop in Update()

Consider adding a flag to prevent repeated "Battle complete!" messages:

```csharp
private bool _battleCompleteShown = false;

protected override void Update(GameTime gameTime)
{
    // ... existing code ...
    
    // Check if battle is complete
    if (_battleRunner is { IsCompleted: true })
    {
        if (!_battleCompleteShown)
        {
Console.WriteLine($"Battle complete! Winner: {_battleRunner.Result}");
   _battleCompleteShown = true;
        }
  }
    
    base.Update(gameTime);
}
```

This ensures the message only prints once instead of every frame.

## Result

? Players are properly created  
? Battle starts with initialized players
? No more "Player 1 is not initialized" error  
? No infinite loop  
? Battle proceeds normally
