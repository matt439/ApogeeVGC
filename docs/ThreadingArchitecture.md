# Threading Architecture for GUI + Battle Simulation

## The Problem

The battle simulation uses **async/await** patterns:
```csharp
await Battle.StartAsync();
await RequestAndWaitForChoicesAsync();
```

MonoGame uses a **synchronous game loop**:
```csharp
protected override void Update(GameTime gameTime) { }
protected override void Draw(GameTime gameTime) { }
```

If the battle simulation's `await RequestChoiceAsync()` runs on the same thread as the MonoGame loop, **the entire GUI would freeze** while waiting for user input - defeating the purpose of having a GUI!

## The Solution

### **Battle runs on background thread, GUI runs on main thread**

```
???????????????????????????????????????????????????????????????
?                 Main Thread?
?  ????????????????????????????????????????????????????????  ?
?  ?           MonoGame Game Loop (60 FPS)      ?  ?
?  ?  - Update()  ? Process input, update UI        ??
?  ?  - Draw()    ? Render battle state      ?  ?
?  ????????????????????????????????????????????????????????  ?
???????????????????????????????????????????????????????????????
          ? (Thread-safe communication)
???????????????????????????????????????????????????????????????
?       Background Thread          ?
?  ????????????????????????????????????????????????????????  ?
?  ?  Battle Simulation (Async)              ?  ?
?  ?  - await Battle.StartAsync()  ?  ?
?  ?  - await RequestChoiceAsync() ? BLOCKS HERE ?  ?
?  ?  - Continue when choice received     ?  ?
?  ????????????????????????????????????????????????????????  ?
???????????????????????????????????????????????????????????????
```

### **Communication Flow**

1. **Battle Thread**: Calls `await RequestChoiceAsync(...)` and blocks
2. **GUI Thread**: Continues updating and drawing
3. **User**: Clicks button or presses key on GUI thread
4. **GUI Thread**: Calls `TaskCompletionSource.SetResult(choice)`
5. **Battle Thread**: Unblocks and continues simulation

## Key Classes

### **BattleRunner** (`ApogeeVGC/Sim/Core/BattleRunner.cs`)
- Runs the battle on a background thread using `Task.Run()`
- Starts the battle asynchronously
- Provides status checking (`IsRunning`, `IsCompleted`)

### **ChoiceInputManager** (`ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.cs`)
- **Thread-safe** choice input handler
- Uses `TaskCompletionSource<Choice>` to bridge async battle ? sync GUI
- `RequestChoiceAsync()` called from battle thread, returns immediately
- `Update()` called from GUI thread, processes input
- `SubmitChoice()` completes the `TaskCompletionSource`, unblocking battle thread

### **BattleGame** (`ApogeeVGC/Gui/BattleGame.cs`)
- MonoGame window running on main thread
- Holds reference to `BattleRunner`
- Provides `RequestChoiceAsync()` that delegates to `ChoiceInputManager`
- Thread-safe perspective updates via `lock`

## Thread Safety

### **Volatile Flag**
```csharp
private volatile bool _isRequestActive;
```
- Prevents compiler optimizations that could break cross-thread visibility
- Ensures battle thread sees when GUI thread sets this flag

### **TaskCompletionSource**
```csharp
private TaskCompletionSource<Choice>? _choiceCompletionSource;
```
- Thread-safe way to signal completion from one thread to another
- Battle thread awaits `.Task`
- GUI thread calls `.SetResult(choice)` to complete it

### **Lock for State**
```csharp
private readonly object _stateLock = new();
lock (_stateLock) { _currentBattlePerspective = perspective; }
```
- Protects shared state (battle perspective) from concurrent access

## Usage Example

```csharp
// Create the MonoGame window (main thread)
using var game = new BattleGame();

// Create battle options with GUI player
var battleOptions = new BattleOptions
{
    Player1Options = new PlayerOptions
    {
        Type = PlayerType.Gui,
        GuiWindow = game,  // Pass the GUI window
        // ...
    },
    // ...
};

// Create simulator/controller
var simulator = new Simulator();

// Start battle on background thread
game.StartBattle(library, battleOptions, simulator);

// Run MonoGame loop on main thread (blocks until window closes)
game.Run();
```

## Answering Your Original Question

> Could `availableChoices` just be a single `IChoiceRequest` instead of a list?

**Yes!** In standard 1v1/2v2 battles, each side gets exactly one request at a time. The current code passes `[side.ActiveRequest!]` - a single-element list.

**Recommendation**: Simplify the interface to accept a single `IChoiceRequest`:

```csharp
// Instead of:
Task<Choice> RequestChoiceAsync(SideId sideId, List<IChoiceRequest> availableChoices, ...);

// Use:
Task<Choice> RequestChoiceAsync(SideId sideId, IChoiceRequest request, ...);
```

This is clearer and matches the actual usage pattern. The list structure might have been for future multi-battle support, but it's unnecessary complexity for your current implementation.

## Why This Works

1. **No Blocking**: MonoGame loop never awaits anything - it just calls synchronous Update()/Draw()
2. **Responsive GUI**: User can see animations, move cursor, etc. while battle waits for input
3. **Clean Separation**: Battle logic doesn't know about MonoGame threading model
4. **Thread-Safe**: All cross-thread communication uses proper synchronization primitives

## Common Pitfalls to Avoid

? **Don't** call `await` in `Update()` or `Draw()` - they must be synchronous  
? **Don't** access GUI state directly from battle thread without locking  
? **Don't** assume battle completes instantly - it runs asynchronously  

? **Do** use `TaskCompletionSource` to bridge async ? sync worlds  
? **Do** use `volatile` or `lock` for shared state  
? **Do** run battle on background thread via `Task.Run()`
