# Async Simulator Stack Overflow Fix

## Problem
The async `Simulator` was experiencing stack overflow exceptions when running battles with Random vs Random players. The synchronous `SyncSimulator` worked fine, indicating an issue specific to the async coordination pattern.

## Root Cause Analysis

### The Infinite Recursion Loop
The stack overflow was caused by nested `Battle.Choose()` calls within the same call stack:

1. **Initial Request**: `Battle.Start()` ? `RunPickTeam()` ? sets `RequestState.TeamPreview`
2. **Request Emission**: `Simulator.RunBattleLoop()` ? `Battle.RequestPlayerChoices()` ? fires `ChoiceRequested` events
3. **Async Choice Handling**: `OnChoiceRequested` ? starts async task ? gets choice from player
4. **Choice Submission**: Async task ? `_choiceResponseChannel.WriteAsync()`
5. **Choice Processing**: `ProcessChoiceResponsesAsync` ? `Battle.Choose(P1)` ? not all choices done ? returns
6. **Race Condition**: Almost immediately, `ProcessChoiceResponsesAsync` ? `Battle.Choose(P2)` ? ALL choices done ? `CommitChoices()` ? `TurnLoop()` ? `MakeRequest()` ? sets new `RequestState`
7. **Immediate Re-emission**: Still in the call stack from step 6, `ProcessChoiceResponsesAsync` checks `RequestState` ? finds new request ? calls `Battle.RequestPlayerChoices()`
8. **Nested Call**: `RequestPlayerChoices()` ? fires `ChoiceRequested` ? async tasks call `Battle.Choose()` **while still deep in the stack from the previous `Battle.Choose()` call**
9. **Stack Overflow**: This pattern repeats, creating deeply nested call stacks that eventually overflow

### Why SyncSimulator Works
`SyncSimulator` avoids this by using an explicit main loop:
```csharp
while (!Battle.Ended)
{
    if (Battle.RequestState != RequestState.None)
    {
        Battle.RequestPlayerChoices();
     // Battle.Choose() is called synchronously
        // Control returns here after each choice cycle completes
    }
}
```

The key difference: **Control returns to the main loop** after each `Battle.Choose()` call completes, so new requests are emitted from a fresh call stack.

## The Fix

### Solution: Batch-Aware Request Emission
Modified `ProcessChoiceResponsesAsync` to only emit new requests after **all choices in the current batch** have been processed, avoiding nested calls:

```csharp
private async Task ProcessChoiceResponsesAsync(CancellationToken cancellationToken)
{
    await foreach (ChoiceResponse response in _choiceResponseChannel!.Reader.ReadAllAsync(cancellationToken))
    {
        // Submit the choice (may trigger CommitChoices if all choices done)
        Battle!.Choose(response.SideId, response.Choice);
        
        // Clean up completed task
        lock (_pendingChoiceTasks)
        {
   _pendingChoiceTasks.Remove(response.SideId);
        }

     // Check if any choice tasks are still running
        bool hasPendingTasks;
        lock (_pendingChoiceTasks)
 {
     hasPendingTasks = _pendingChoiceTasks.Count > 0;
        }

        // Only emit new requests if:
        // 1. No pending choice tasks (all choices in batch processed)
        // 2. New request is pending
        // 3. Battle hasn't ended
     if (!hasPendingTasks && Battle.RequestState != RequestState.None && !Battle.Ended)
        {
     // Small delay to ensure call stack has unwound
    await Task.Yield();
       
         // Emit new requests from a fresh call stack
      Battle.RequestPlayerChoices();
        }
    }
}
```

### Key Changes
1. **Track Pending Tasks**: Use `_pendingChoiceTasks` dictionary to know when all choices in the current batch are processed
2. **Batch Completion Check**: Only check for new requests when `_pendingChoiceTasks.Count == 0`
3. **Call Stack Unwinding**: `await Task.Yield()` ensures the previous `Battle.Choose()` call has fully returned before emitting new requests
4. **Fresh Call Stack**: New requests are emitted from the async loop, not from within a nested `Battle.Choose()` call

### Additional Fixes
1. **Removed Direct Calls**: Removed `Battle.RequestPlayerChoices()` calls from `Battle.RunPickTeam()` - now only sets `RequestState` and returns
2. **Simulator Responsibility**: `Simulator.RunBattleLoop()` checks for pending requests after `Battle.Start()` returns and emits them
3. **Separation of Concerns**: Battle manages game state and requests; Simulator handles async coordination and emission

## Testing
- Created `RunAsyncRandomVsRandomSinglesTest()` to test async Simulator with Random players
- Added `AsyncRandomVsRandomSingles` to `DriverMode` enum  
- Battle completes successfully without stack overflow
- Winner determined correctly: "Player2Win"

## Benefits
1. **No Stack Overflow**: Prevents deeply nested call stacks
2. **Clean Separation**: Battle and Simulator have clear responsibilities
3. **Thread Safe**: Each batch of choices completes before next request
4. **Maintainable**: Easy to understand the async flow
5. **Consistent Pattern**: Similar to SyncSimulator's explicit loop pattern, but async-friendly

## Files Modified
- `ApogeeVGC/Sim/Core/Simulator.cs` - Fixed `ProcessChoiceResponsesAsync` and `RunBattleLoop`
- `ApogeeVGC/Sim/BattleClasses/Battle.Validation.cs` - Removed direct `RequestPlayerChoices()` calls
- `ApogeeVGC/Sim/Core/Driver.cs` - Added `RunAsyncRandomVsRandomSinglesTest` method
- `ApogeeVGC/Sim/Core/CoreEnums.cs` - Added `AsyncRandomVsRandomSingles` enum value
- `ApogeeVGC/Program.cs` - Changed test mode to use async test
