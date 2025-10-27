# Debugging Checklist - ApogeeVGC Battle Hang Issue

## Issue: Battle hangs after turn 1 without sending requests

This checklist will help you systematically debug why the battle stops progressing.

## Step 1: Verify Battle Loop is Running

### Check: Is the main battle loop continuing after turn 1?

**File:** `ApogeeVGC/Sim/BattleClasses/BattleAsync.Core.cs`

**Look for:**
- Main battle loop method (likely `RunBattleAsync` or similar)
- Turn increment logic
- Check if loop exits early or hangs

**Add debug output:**
```csharp
Console.Error.WriteLine($"[DEBUG] Turn {CurrentTurn} starting");
Console.Error.WriteLine($"[DEBUG] Battle state: {State}");
Console.Error.WriteLine($"[DEBUG] Waiting for choices: {WaitingForChoices}");
```

### ✅ Pass Criteria:
- [ ] Debug output shows "Turn 1 starting"
- [ ] Loop continues past turn 1 initialization
- [ ] No exceptions in battle loop

### ❌ Fail: Loop stops after turn 1
**Cause:** Battle loop exits prematurely or hits an exception
**Fix:** Check turn progression logic and exception handling

---

## Step 2: Verify Request Generation

### Check: Are choice requests being created?

**File:** `ApogeeVGC/Sim/BattleClasses/BattleAsync.Requests.cs`

**Look for:**
- Method that creates `MoveRequest` or `ChoiceRequest` objects
- Turn start logic that triggers request creation
- Conditions that might prevent request creation

**Add debug output:**
```csharp
Console.Error.WriteLine($"[DEBUG] Creating request for player {player.PlayerId}");
Console.Error.WriteLine($"[DEBUG] Request type: {request.GetType().Name}");
Console.Error.WriteLine($"[DEBUG] Request data: {JsonSerializer.Serialize(request)}");
```

### ✅ Pass Criteria:
- [ ] Debug output shows "Creating request for player p1"
- [ ] Debug output shows "Creating request for player p2"
- [ ] Request objects are created with valid move data

### ❌ Fail: No request creation debug output
**Cause:** Request generation code never called
**Fix:** Check turn start logic, verify request generation is triggered

### ❌ Fail: Requests created but empty/invalid
**Cause:** Request data not populated correctly
**Fix:** Check active Pokémon state, move data availability

---

## Step 3: Verify Request Distribution

### Check: Are requests being sent to player streams?

**File:** `ApogeeVGC/Sim/BattleClasses/BattleAsync.Requests.cs` or `BattleStream.cs`

**Look for:**
- Code that sends requests to player streams
- `WriteAsync` or similar calls to player channels
- Message formatting for `sideupdate` type

**Add debug output:**
```csharp
Console.Error.WriteLine($"[DEBUG] Sending request to {playerId}");
Console.Error.WriteLine($"[DEBUG] Request message: {message}");
Console.Error.WriteLine($"[DEBUG] Stream writable: {stream.CanWrite}");
```

### ✅ Pass Criteria:
- [ ] Debug output shows "Sending request to p1"
- [ ] Debug output shows "Sending request to p2"
- [ ] Request messages are properly formatted

### ❌ Fail: Requests not sent to streams
**Cause:** Missing code to distribute requests to player streams
**Fix:** Add code to send requests after generation

### ❌ Fail: Streams not writable
**Cause:** Player streams closed or not initialized
**Fix:** Check stream lifecycle and initialization

---

## Step 4: Verify AI Request Reception

### Check: Do AI players receive the requests?

**File:** `ApogeeVGC/Sim/Player/RandomPlayerAi.cs`

**Look for:**
- Request reading loop (likely `await foreach` or `ReadAsync`)
- Request parsing logic
- Response generation code

**Add debug output:**
```csharp
Console.Error.WriteLine($"[DEBUG] AI received message: {message}");
Console.Error.WriteLine($"[DEBUG] Message type: {messageType}");
Console.Error.WriteLine($"[DEBUG] Generating response...");
```

### ✅ Pass Criteria:
- [ ] Debug output shows "AI received message" for both players
- [ ] Messages are parsed as requests
- [ ] AI attempts to generate responses

### ❌ Fail: AI never receives messages
**Cause:** Messages not written to AI input streams
**Fix:** Check stream connections between battle and AI

### ❌ Fail: AI receives messages but doesn't parse as requests
**Cause:** Message format incorrect or parsing logic broken
**Fix:** Verify message format matches expected `sideupdate` structure

---

## Step 5: Verify AI Response Sending

### Check: Do AI players send their choices back?

**File:** `ApogeeVGC/Sim/Player/RandomPlayerAi.cs`

**Look for:**
- Choice generation logic
- `WriteAsync` or similar to send choices
- Choice format (`>p1 move 1`, etc.)

**Add debug output:**
```csharp
Console.Error.WriteLine($"[DEBUG] AI generated choice: {choice}");
Console.Error.WriteLine($"[DEBUG] Sending choice to battle...");
Console.Error.WriteLine($"[DEBUG] Choice sent successfully");
```

### ✅ Pass Criteria:
- [ ] Debug output shows "AI generated choice" for both players
- [ ] Choices are sent back to battle stream
- [ ] Choice format is correct (`>p1 move X` or `>p1 switch Y`)

### ❌ Fail: AI doesn't generate choices
**Cause:** Request parsing failed or choice generation logic broken
**Fix:** Check request data structure and choice generation code

### ❌ Fail: Choices generated but not sent
**Cause:** Stream writing issue or formatting problem
**Fix:** Verify stream connections and choice message format

---

## Step 6: Verify Battle Receives Choices

### Check: Does the battle receive and process player choices?

**File:** `ApogeeVGC/Sim/BattleClasses/BattleAsync.Core.cs` or `BattleStream.cs`

**Look for:**
- Choice parsing from input stream
- Choice validation logic
- Turn execution after both choices received

**Add debug output:**
```csharp
Console.Error.WriteLine($"[DEBUG] Battle received choice from {playerId}: {choice}");
Console.Error.WriteLine($"[DEBUG] Choices received: {choicesReceived}/{totalPlayers}");
Console.Error.WriteLine($"[DEBUG] Executing turn...");
```

### ✅ Pass Criteria:
- [ ] Debug output shows choices received from both players
- [ ] Battle executes turn after receiving both choices
- [ ] Turn 2 begins after turn 1 execution

### ❌ Fail: Battle doesn't receive choices
**Cause:** Stream connection broken or choice routing incorrect
**Fix:** Verify stream plumbing between AI and battle

### ❌ Fail: Battle receives choices but doesn't execute turn
**Cause:** Turn execution logic issue or validation failure
**Fix:** Check turn execution triggers and choice validation

---

## Step 7: Check for Deadlocks

### Check: Are there any async deadlocks?

**Common causes:**
- Using `.Wait()` or `.Result` on async operations
- Circular waiting (A waits for B, B waits for A)
- Not awaiting async operations properly

**Look for:**
```csharp
// BAD - can cause deadlocks
someTask.Wait();
var result = someTask.Result;

// GOOD - properly await
await someTask;
var result = await someTask;
```

**Add debug output before any blocking operations:**
```csharp
Console.Error.WriteLine($"[DEBUG] About to wait for: {operation}");
// blocking operation here
Console.Error.WriteLine($"[DEBUG] Completed waiting for: {operation}");
```

### ✅ Pass Criteria:
- [ ] No `.Wait()` or `.Result` calls in async code paths
- [ ] All async operations properly awaited
- [ ] No circular dependencies in task waiting

### ❌ Fail: Deadlock detected
**Cause:** Improper async/await usage
**Fix:** Convert all sync-over-async to proper async/await

---

## Step 8: Check Stream Lifecycle

### Check: Are streams properly initialized and not closed prematurely?

**Files:**
- `ApogeeVGC/Sim/BattleClasses/BattleStream.cs`
- `ApogeeVGC/Sim/Player/PlayerStreams.cs`

**Look for:**
- Stream initialization in `GetPlayerStreams`
- Stream disposal or closing
- Channel completion

**Add debug output:**
```csharp
Console.Error.WriteLine($"[DEBUG] Streams initialized: P1={p1Stream != null}, P2={p2Stream != null}");
Console.Error.WriteLine($"[DEBUG] Channel open: {channel.Reader.Completion.IsCompleted}");
```

### ✅ Pass Criteria:
- [ ] All player streams initialized before battle starts
- [ ] Streams remain open during battle
- [ ] Channels not completed prematurely

### ❌ Fail: Streams closed or null
**Cause:** Premature disposal or initialization issue
**Fix:** Check stream lifecycle management and disposal timing

---

## Quick Diagnostic Script

Add this to your `Driver.cs` for debugging:

```csharp
private async Task StartTestWithDebug()
{
    Console.Error.WriteLine("[DEBUG] ===== BATTLE DEBUG MODE =====");
    
 PlayerStreams streams = BattleStreamExtensions.GetPlayerStreams(new BattleStream(Library));
    Console.Error.WriteLine("[DEBUG] Streams created");

  // ... existing setup code ...

    Console.Error.WriteLine("[DEBUG] Starting AI players");
    Task p1Task = p1.StartAsync();
    Task p2Task = p2.StartAsync();

    Console.Error.WriteLine("[DEBUG] Starting stream consumer");
    Task streamConsumerTask = Task.Run(async () =>
{
        try
        {
 await foreach (string chunk in streams.Omniscient.ReadAllAsync())
     {
       Console.WriteLine(chunk);
          Console.Error.WriteLine($"[DEBUG] Received output chunk of length {chunk.Length}");
            }
  Console.Error.WriteLine("[DEBUG] Stream consumer finished normally");
        }
     catch (Exception ex)
        {
    Console.Error.WriteLine($"[DEBUG] Stream consumer error: {ex.Message}");
        }
    });

    Console.Error.WriteLine("[DEBUG] Writing start commands");
    await streams.Omniscient.WriteAsync(startCommand);
    Console.Error.WriteLine("[DEBUG] Start commands written");

    // Wait with status updates
    for (int i = 0; i < 30; i++)
 {
        await Task.Delay(1000);
        Console.Error.WriteLine($"[DEBUG] Battle running... {i}s elapsed");
        
        if (Task.WhenAll(p1Task, p2Task, streamConsumerTask).IsCompleted)
        {
   Console.Error.WriteLine("[DEBUG] All tasks completed!");
       break;
        }
    }
    
    Console.Error.WriteLine("[DEBUG] ===== END DEBUG MODE =====");
}
```

## Expected Debug Output Sequence

If everything works correctly, you should see:

```
[DEBUG] ===== BATTLE DEBUG MODE =====
[DEBUG] Streams created
[DEBUG] Starting AI players
[DEBUG] Starting stream consumer
[DEBUG] Writing start commands
[DEBUG] Start commands written
[DEBUG] Received output chunk of length 496
[DEBUG] Battle running... 0s elapsed
[DEBUG] AI received message
[DEBUG] AI generated choice: move 1
[DEBUG] Battle received choice from p1
[DEBUG] AI received message
[DEBUG] AI generated choice: move 2
[DEBUG] Battle received choice from p2
[DEBUG] Executing turn...
[DEBUG] Turn 2 starting
[DEBUG] Received output chunk of length 350
[DEBUG] Battle running... 1s elapsed
... continues until battle ends ...
[DEBUG] All tasks completed!
[DEBUG] ===== END DEBUG MODE =====
```

## Common Issues and Solutions

### Issue: Output stops after turn 1

**Likely Causes:**
1. Requests not generated → Check Step 2
2. Requests not sent → Check Step 3
3. AI not responding → Check Steps 4-5
4. Deadlock → Check Step 7

**Quick Fix Attempt:**
- Add debug output at each step
- Find where output stops
- Focus debugging on that step

### Issue: AI receives requests but doesn't respond

**Likely Causes:**
1. Request format incorrect → Verify JSON structure
2. AI parsing broken → Check request parsing code
3. AI stream closed → Check stream lifecycle

**Quick Fix Attempt:**
- Log raw request message
- Compare with expected format from SIM-PROTOCOL.md
- Verify AI can parse and extract move options

### Issue: Battle receives choices but doesn't execute

**Likely Causes:**
1. Choice validation failing → Check validation logic
2. Turn execution not triggered → Check turn trigger code
3. Exception in turn execution → Check error handling

**Quick Fix Attempt:**
- Log choice validation results
- Verify both players' choices received
- Check for silent exceptions

---

## When to Ask for Help

If you've completed all steps and still stuck:

1. **Collect debug output** from all steps
2. **Identify last successful step**
3. **Share relevant code** from the failing step
4. **Include error messages** if any

## Success Markers

You've fixed it when:
- ✅ Debug output shows requests being sent every turn
- ✅ AI players respond to every request
- ✅ Battle continues past turn 1
- ✅ Turn counter increments (|turn|2, |turn|3, etc.)
- ✅ Move messages appear in output (|move|..., |-damage|..., etc.)
- ✅ Battle eventually ends with |win| or |tie|

---

Good luck debugging! Start with Step 1 and work through systematically. Most likely the issue is in Steps 2-3 (request generation/distribution).
