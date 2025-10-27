# Battle Flow Diagram - Expected vs Actual

## Expected Flow (Pokémon Showdown TypeScript)

```
┌─────────────────────────────────────────────────────────────────┐
│ INITIALIZATION PHASE           │
└─────────────────────────────────────────────────────────────────┘

Driver → BattleStream: >start {"formatid":"gen9customgame"}
Driver → BattleStream: >player p1 {...team...}
Driver → BattleStream: >player p2 {...team...}

Battle Created
  ↓
Initialize Players
  ↓
Send Initial Updates:
  |player|p1|Bot 1||
  |player|p2|Bot 2||
  |gen|9
  |tier|[Gen 9] OU
  |
  |teamsize|p1|6
  |teamsize|p2|6
  |start
  ↓
Send Switch Messages (ONE per Pokémon):
  |switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|205/205
  |switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|205/205
↓
Announce Abilities (BEFORE turn marker):
  |-ability|p1a: calyrex-ice|As One
  |-ability|p1a: calyrex-ice|Unnerve
  |-ability|p2a: calyrex-ice|As One
  |-ability|p2a: calyrex-ice|Unnerve
  ↓
|turn|1

┌─────────────────────────────────────────────────────────────────┐
│ TURN LOOP PHASE            │
└─────────────────────────────────────────────────────────────────┘

For each turn:

  1. Generate Requests
     ├─→ Battle → P1 Stream: sideupdate\np1\n|request|{...}
     └─→ Battle → P2 Stream: sideupdate\np2\n|request|{...}

  2. Wait for Choices
     ├─→ P1 AI: Reads request from stream
│       Parses request JSON
     │      Generates choice (e.g., "move 1")
     │          Writes: >p1 move 1
     │
     └─→ P2 AI: Reads request from stream
        Parses request JSON
      Generates choice (e.g., "move 2")
         Writes: >p2 move 2

  3. Process Choices
     Battle receives: >p1 move 1
     Battle receives: >p2 move 2
     ↓
     Validate choices
 ↓
     Execute turn actions

  4. Send Turn Updates
     |move|p1a: calyrex-ice|Glacial Lance|p2a: calyrex-ice
     |-damage|p2a: calyrex-ice|150/205
     |move|p2a: calyrex-ice|Glacial Lance|p1a: calyrex-ice
     |-damage|p1a: calyrex-ice|160/205
     ↓
   |turn|2

  5. Check Win Conditions
     If battle over:
       |win|Bot 1  (or |tie|)
       END
     Else:
       Goto step 1 (next turn)

┌─────────────────────────────────────────────────────────────────┐
│ END PHASE  │
└─────────────────────────────────────────────────────────────────┘

Battle ends
  ↓
Close streams
  ↓
Tasks complete
```

---

## Actual Flow (ApogeeVGC C# - Current State)

```
┌─────────────────────────────────────────────────────────────────┐
│ INITIALIZATION PHASE        │
└─────────────────────────────────────────────────────────────────┘

Driver → BattleStream: >start {"formatid":"gen9customgame"}
Driver → BattleStream: >player p1 {...team...}
Driver → BattleStream: >player p2 {...team...}

Battle Created ✅
  ↓
Initialize Players ✅
  ↓
Send Initial Updates: ⚠️ DUPLICATED
  |player|p1|Bot 1||
  |player|p2|Bot 2||
  |gen|9
  |tier|[Gen 9] OU
  |
  |teamsize|p1|6
  |player|p1|Bot 1||          ← DUPLICATE
  |gen|9           ← DUPLICATE
  |player|p2|Bot 2||      ← DUPLICATE
  |teamsize|p1|6            
  |teamsize|p2|6
  |
  |tier|[Gen 9] OU       ← DUPLICATE
  |start
  ↓
Send Switch Messages: ❌ MULTIPLE TIMES
  |split|p1
  |switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|100/100    ← Switch #1
  |split|p2
  |switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|205/205    ← Switch #2 (same Pokémon!)
  |switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|205/205
  ↓
Announce Abilities: ⚠️ WRONG TIMING
  |-ability|p1a: calyrex-ice|As One
  |-ability|p2a: calyrex-ice|As One
  |-ability|p2a: calyrex-ice|Unnerve
  ↓
|turn|1
  ↓
|-ability|p1a: calyrex-ice|Unnerve    ← AFTER turn marker (wrong!)
|switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|100/100    ← Extra switch (wrong!)

┌─────────────────────────────────────────────────────────────────┐
│ TURN LOOP PHASE   │
└─────────────────────────────────────────────────────────────────┘

For each turn:

  1. Generate Requests ❌ NOT HAPPENING
     ├─→ Battle → P1 Stream: [NOTHING SENT]
     └─→ Battle → P2 Stream: [NOTHING SENT]

  2. Wait for Choices ⏳ WAITING FOREVER
     ├─→ P1 AI: Waiting for request...
     │   [NEVER RECEIVES ANYTHING]
     │          [STUCK FOREVER]
     │
     └─→ P2 AI: Waiting for request...
     [NEVER RECEIVES ANYTHING]
       [STUCK FOREVER]

  3. Process Choices ❌ NEVER REACHED
     [BATTLE NEVER GETS HERE]

  4. Send Turn Updates ❌ NEVER REACHED
     [NO MOVES EXECUTED]

  5. Check Win Conditions ❌ NEVER REACHED
     [BATTLE STUCK IN LIMBO]

┌─────────────────────────────────────────────────────────────────┐
│ HUNG STATE - BATTLE NEVER ENDS                │
└─────────────────────────────────────────────────────────────────┘

Battle loop: ??? (unknown state)
  ↑
  └─── STUCK HERE FOREVER
       - No requests generated
       - AI players waiting
     - No timeout handling
       - Must be killed externally
```

---

## Side-by-Side Comparison: Request Flow

### Expected (TypeScript)

```
[After |turn|1]

Battle Thread:
  1. Determine active Pokémon
  2. For each active Pokémon:
     - Get available moves
     - Check PP, disabled moves
     - Get possible switch options
  3. Build request JSON object
  4. Send to player stream:
     "sideupdate\np1\n|request|{JSON}"

Player Stream → AI Input:
  "sideupdate\np1\n|request|{JSON}"

AI Thread:
  1. Read from input stream
  2. Parse message type: "sideupdate"
  3. Extract request JSON
  4. Generate choice based on request
  5. Write to output stream:
     ">p1 move 1"

AI Output → Battle Input Stream:
  ">p1 move 1"

Battle Thread:
  1. Read choice from stream
  2. Parse choice
  3. Store for this player
4. Wait for other player(s)
  5. When all choices received:
     → Execute turn
```

### Actual (C# - Broken)

```
[After |turn|1]

Battle Thread:
  ??? 
  [UNKNOWN - NEEDS INVESTIGATION]
  
  Either:
  A) Request generation code not called
  B) Requests generated but not sent
  C) Requests sent to wrong streams
  D) Battle loop exited/stuck

Player Stream → AI Input:
  [NOTHING - EMPTY STREAM]

AI Thread:
  1. Read from input stream
  2. ⏳ WAITING...
  3. ⏳ WAITING...
  4. ⏳ WAITING...
  5. [FOREVER - NO MESSAGE EVER ARRIVES]

AI Output → Battle Input Stream:
  [NOTHING - AI NEVER RESPONDS]

Battle Thread:
  [UNKNOWN STATE]
  - Never progresses to turn 2
  - Never sends updates
  - Never ends
  - Must be killed
```

---

## Data Flow Diagram

### Expected Stream Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│          BattleStream    │
│            │
│  ┌─────────────┐   ┌─────────────┐      ┌─────────────┐      │
│  │   P1 Input  │      │   P2 Input  │    │  Omniscient │      │
│  │   Channel   │   │   Channel   │      │   Channel   │      │
│  │             │      │ │      │             │      │
│  │  Write: AI  │      │  Write: AI  │      │ Write:Battle│  │
│  │  Read:Battle│      │  Read:Battle│      │ Read: Driver│      │
│  └─────────────┘      └─────────────┘      └─────────────┘      │
│         ↑ ↑           ↑        │
│ │       │         │             │
└─────────┼────────────────────┼─────────────────────┼─────────────┘
          │            │  │
     │      │             │
┌─────────┴──────────┐ ┌───────┴──────────┐  ┌──────┴──────────┐
│  P1 Output Channel │ │ P2 Output Channel│  │   (no input)    │
│     │ │   │  │ │
│  Write: Battle     │ │  Write: Battle   │  │          │
│  Read: AI          │ │  Read: AI        │  │  │
└────────────────────┘ └───────────────────┘  └─────────────────┘
  ↓        ↓
       │  │
    ┌────┴────┐         ┌────┴────┐
  │ P1 AI   │ │ P2 AI   │
    │         │ │       │
    │ Reads   │           │ Reads   │
    │ requests│        │ requests│
    │         │    │         │
    │ Writes  │           │ Writes  │
    │ choices │     │ choices │
    └─────────┘           └─────────┘
```

### Critical Flow for Turn Execution

```
Turn N starts:

  Battle:
    ├─> Generate request for P1
    │   ├─> Create request object
    │   ├─> Convert to JSON
    │   └─> Write to P1 Output Channel
    │
    └─> Generate request for P2
        ├─> Create request object
        ├─> Convert to JSON
        └─> Write to P2 Output Channel

  ⬇️  [Messages flow through channels] ⬇️

  P1 AI:
    ├─> Read from P1 Input (receives request)
    ├─> Parse request
    ├─> Generate choice
└─> Write choice to P1 Output: ">p1 move X"

  P2 AI:
    ├─> Read from P2 Input (receives request)
    ├─> Parse request
 ├─> Generate choice
    └─> Write choice to P2 Output: ">p2 move Y"

  ⬆️  [Choices flow back through channels] ⬆️

  Battle:
    ├─> Read from P1 Input Channel (receives choice)
    ├─> Read from P2 Input Channel (receives choice)
    ├─> Execute turn with both choices
    ├─> Send updates to Omniscient Channel
    └─> Loop to next turn
```

---

## Where is the Break?

Based on the hang, the break is at one of these points:

### 🔴 Most Likely: Request Generation

```
Turn N starts:

  Battle:
 ❌ Generate request for P1 [NOT HAPPENING]
    ❌ Generate request for P2 [NOT HAPPENING]
```

**Why:** No debug output suggests requests being generated.
**Check:** `BattleAsync.Requests.cs` - Is request generation code called?

---

### 🟡 Possible: Request Sending

```
Turn N starts:

  Battle:
    ✅ Generate request for P1 [HAPPENS]
    ✅ Generate request for P2 [HAPPENS]
    ❌ Write to P1 Output Channel [NOT HAPPENING]
    ❌ Write to P2 Output Channel [NOT HAPPENING]
```

**Why:** Requests might be created but not sent to channels.
**Check:** `BattleStream.cs` - Is write code called after generation?

---

### 🟢 Unlikely: Channel Communication

```
Turn N starts:

  Battle:
    ✅ Generate request for P1
    ✅ Write to P1 Output Channel
    ❌ P1 AI never receives it [CHANNEL BROKEN]
```

**Why:** If this were the issue, we'd see debug output for generation.
**Check:** Channel initialization and lifecycle.

---

## Fix Priority

1. **FIRST:** Add debug output to find where flow breaks
2. **SECOND:** Fix the broken step
3. **THIRD:** Clean up message duplication
4. **FOURTH:** Fix switch message issues
5. **FIFTH:** Remove debug output from protocol stream

---

## How to Use This Document

1. **Understand the expected flow** - Top section
2. **Compare with actual flow** - Middle section
3. **Identify the break point** - Bottom section
4. **Add debug output** at suspected break point
5. **Use DEBUGGING_CHECKLIST.md** for systematic debugging
6. **Fix identified issues**
7. **Re-run test** with `run_battle_test.ps1`
8. **Repeat** until battle completes

---

## Success Indicators

You'll know you fixed it when the actual flow matches the expected flow:

✅ Initialization completes (currently works)
✅ Requests generated every turn
✅ AI receives and responds to requests
✅ Battle executes turns
✅ Turn counter increments
✅ Battle ends naturally with |win| or |tie|

Currently at step 1/6 (initialization only).
Need to reach 6/6 (full battle completion).
