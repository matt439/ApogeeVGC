# Message-Perspective Synchronization Fix

## Problem Statement

### Current Issue
The battle engine has a fundamental timing issue where messages are added to the log, but the battle state is updated immediately before the GUI processes those messages. This causes perspectives to be captured **after** state changes have occurred, leading to visual bugs where:

- Replacement Pokémon (e.g., Ursaluna) appear before the fainted Pokémon's animation completes
- The GUI receives messages referencing a game state that no longer exists
- Messages and perspectives are desynchronized

### Root Cause
```csharp
// Current flow (BROKEN):
1. Add("faint", miraidon)              // Message added to log
2. miraidon.Fainted = true             // State updated immediately
3. side.Active[1] = ursaluna           // Replacement happens
4. FlushEvents()                       // Perspective captured HERE
   ??> GetPerspectiveForSide()         // Reads CURRENT state (Ursaluna already there!)
5. GUI receives faint message          // But perspective shows Ursaluna!
```

The `Add()` method just appends to a log, while `FlushEvents()` captures perspective much later when the battle state has already changed.

## Solution: Capture Perspective Per Message (Option 1)

### Approach
Modify the `Add()` method to capture the battle perspective **immediately** when each message is created, rather than waiting until `FlushEvents()` is called.

### Key Changes

#### 1. Modify `Battle.Add()` Method
**File:** `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs`

Change the `Add()` method to:
1. Format the message as usual
2. Add to the log (existing behavior)
3. **NEW:** Parse the message immediately
4. **NEW:** Capture perspective at this exact moment
5. **NEW:** Add to `PendingEvents` with the captured perspective

```csharp
public void Add(params PartFuncUnion[] parts)
{
    // Full observability mode: no secret/shared split needed
    var formattedParts = new List<string>();
    
    foreach (PartFuncUnion part in parts)
    {
        if (part is FuncPartFuncUnion funcPart)
        {
            SideSecretSharedResult result = funcPart.Func();
            formattedParts.Add(result.Secret.ToString());
        }
        else if (part is PartPartFuncUnion directPart)
        {
            formattedParts.Add(FormatPart(directPart.Part));
        }
    }
    
    // Add single message to log
    string message = $"|{string.Join("|", formattedParts)}";
    Log.Add(message);
    
    // NEW: Capture perspective immediately when message is added
    if (DisplayUi)
    {
        BattleMessage? parsed = ParseSingleLogEntry(message);
        if (parsed != null)
        {
            BattlePerspectiveType perspectiveType = RequestState == RequestState.TeamPreview
                ? BattlePerspectiveType.TeamPreview
                : BattlePerspectiveType.InBattle;
            
            BattlePerspective perspective = GetPerspectiveForSide(SideId.P1, perspectiveType);
            
            PendingEvents.Add(new BattleEvent
            {
                Message = parsed,
                Perspective = perspective  // Captured NOW, not later!
            });
        }
    }
}
```

#### 2. Create `ParseSingleLogEntry()` Method
**File:** `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs`

Extract a single message parser from the existing `ParseLogToMessages()` method:

```csharp
/// <summary>
/// Parses a single log entry into a BattleMessage.
/// Returns null if the message type is not recognized or doesn't need to be sent to GUI.
/// </summary>
private BattleMessage? ParseSingleLogEntry(string logEntry)
{
    if (string.IsNullOrEmpty(logEntry)) return null;

    string[] parts = logEntry.Split('|');
    if (parts.Length < 2) return null;

    string command = parts[1]; // parts[0] is empty due to leading |

    try
    {
        return command switch
        {
            "turn" when parts.Length > 2 && int.TryParse(parts[2], out int turnNum) =>
                new TurnStartMessage { TurnNumber = turnNum },

            "move" when parts.Length > 3 =>
                new MoveUsedMessage
                {
                    PokemonName = ExtractPokemonName(parts[2]),
                    SideId = ExtractSideId(parts[2]),
                    MoveName = parts[3]
                },

            "switch" or "drag" when parts.Length > 3 =>
                new SwitchMessage
                {
                    TrainerName = ExtractTrainerName(parts[2]),
                    PokemonName = ExtractPokemonName(parts[2])
                },

            "faint" when parts.Length > 2 =>
                new FaintMessage
                {
                    PokemonName = ExtractPokemonName(parts[2]),
                    SideId = ExtractSideId(parts[2])
                },

            "-damage" when parts.Length > 3 =>
                ParseDamageMessage(parts),

            "-heal" when parts.Length > 3 =>
                ParseHealMessage(parts),

            "-status" when parts.Length > 3 =>
                new StatusMessage
                {
                    PokemonName = ExtractPokemonName(parts[2]),
                    SideId = ExtractSideId(parts[2]),
                    StatusName = parts[3]
                },

            "-supereffective" =>
                new EffectivenessMessage
                {
                    Effectiveness = EffectivenessMessage.EffectivenessType.SuperEffective
                },

            "-resisted" =>
                new EffectivenessMessage
                {
                    Effectiveness = EffectivenessMessage.EffectivenessType.NotVeryEffective
                },

            "-immune" =>
                new EffectivenessMessage
                {
                    Effectiveness = EffectivenessMessage.EffectivenessType.NoEffect
                },

            "-crit" =>
                new CriticalHitMessage(),

            "-miss" when parts.Length > 2 =>
                new MissMessage
                {
                    PokemonName = ExtractPokemonName(parts[2]),
                    SideId = ExtractSideId(parts[2])
                },

            "-fail" when parts.Length > 2 =>
                new MoveFailMessage
                {
                    Reason = parts.Length > 4 ? parts[4] : (parts.Length > 3 ? parts[3] : "Unknown"),
                    TargetPokemonName = parts.Length > 3 ? ExtractPokemonName(parts[3]) : null,
                    TargetSideId = parts.Length > 3 ? ExtractSideId(parts[3]) : null
                },

            "-boost" or "-unboost" when parts.Length > 4 =>
                ParseStatChangeMessage(parts, command == "-boost"),

            "-weather" when parts.Length > 2 =>
                new WeatherMessage
                {
                    WeatherName = parts[2],
                    IsEnding = parts.Length > 3 && parts[3] == "[upkeep]"
                },

            "-ability" when parts.Length > 3 =>
                new AbilityMessage
                {
                    PokemonName = ExtractPokemonName(parts[2]),
                    SideId = ExtractSideId(parts[2]),
                    AbilityName = parts[3],
                    AdditionalInfo = parts.Length > 4 ? parts[4] : null
                },

            "-item" when parts.Length > 3 =>
                new ItemMessage
                {
                    PokemonName = ExtractPokemonName(parts[2]),
                    SideId = ExtractSideId(parts[2]),
                    ItemName = parts[3]
                },

            "-sidestart" when parts.Length > 3 =>
                new GenericMessage
                {
                    Text = $"{parts[3]} raised {GetSideName(parts[2])}'s team's {GetStatNameForCondition(parts[3])}!"
                },

            "-sideend" when parts.Length > 3 =>
                new GenericMessage
                {
                    Text = $"{GetSideName(parts[2])}'s {parts[3]} wore off!"
                },

            "cant" when parts.Length > 3 =>
                new CantMessage
                {
                    PokemonName = ExtractPokemonName(parts[2]),
                    SideId = ExtractSideId(parts[2]),
                    Reason = parts[3]
                },

            _ => null // Unknown or non-displayable message type
        };
    }
    catch (Exception ex)
    {
        Debug($"Error parsing log entry '{logEntry}': {ex.Message}");
        return null;
    }
}
```

#### 3. Simplify `FlushEvents()` Method
**File:** `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs`

Since events are now created immediately in `Add()`, `FlushEvents()` only needs to send accumulated events:

```csharp
/// <summary>
/// Flush all pending events to players with GUI interfaces.
/// Events are already created with correct perspectives in Add().
/// This method just sends them.
/// </summary>
public void FlushEvents()
{
    if (!DisplayUi) return;

    // If we have pending events, send them
    if (PendingEvents.Count > 0)
    {
        // Send events to each side's player via UpdateRequested event
        foreach (Side side in Sides)
        {
            EmitUpdate(side.Id, new List<BattleEvent>(PendingEvents));
        }

        PendingEvents.Clear();
    }
    else
    {
        // No pending events - only send perspective if explicitly needed (team preview)
        // This handles the case where no messages were added but state changed
        BattlePerspectiveType perspectiveType = RequestState == RequestState.TeamPreview
            ? BattlePerspectiveType.TeamPreview
            : BattlePerspectiveType.InBattle;

        foreach (Side side in Sides)
        {
            BattlePerspective perspective = GetPerspectiveForSide(side.Id, perspectiveType);
            
            var events = new List<BattleEvent>
            {
                new BattleEvent
                {
                    Message = null,
                    Perspective = perspective
                }
            };
            
            EmitUpdate(side.Id, events);
        }
    }
}
```

#### 4. Remove Redundant Log Parsing
**File:** `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs`

The existing `ParseLogToMessages()` method can be kept for backward compatibility or debugging, but is no longer used in the main event flow. Consider marking it obsolete or removing it.

## Benefits

### 1. **Correct Timing**
Perspectives are captured at the exact moment each message is created, ensuring perfect synchronization between messages and game state.

### 2. **Minimal Changes**
Only modifies the message creation pipeline - no changes needed to combat logic, switch handling, or GUI rendering.

### 3. **Performance Acceptable**
Modern hardware can easily generate perspectives for each message (typically 10-50 per turn). Each perspective generation is ~1ms.

### 4. **Maintains Architecture**
Works within the existing event-based system without requiring a complete rewrite.

## Testing Checklist

- [ ] Team preview displays correctly with all 6 Pokémon
- [ ] Miraidon faints and **stays visible** until animation completes
- [ ] Ursaluna appears **after** Miraidon faint animation
- [ ] HP bars animate correctly before perspective changes
- [ ] Damage messages show correct HP values
- [ ] Switch messages display in correct order
- [ ] Multiple simultaneous faints handled correctly
- [ ] Turn counter increments at the right time
- [ ] Status condition messages show before state changes
- [ ] Multi-hit moves display all hits before fainting

## Performance Considerations

### Before (Current System)
- Messages: ~50 per turn
- Perspectives generated: ~3-5 per turn (only when flushing)
- Total perspective calls: ~150-250 per battle

### After (This Fix)
- Messages: ~50 per turn (unchanged)
- Perspectives generated: ~50 per turn (one per message)
- Total perspective calls: ~2500 per battle

**Impact:** ~10x more perspective generations, but still <1ms each = ~2.5ms overhead per battle (negligible).

## Alternative Approaches Considered

### Option 2: State Snapshots
Capture full battle state before each change. **Rejected** due to complexity and memory overhead.

### Option 3: Deferred State Updates
Queue all state changes to apply after messages sent. **Rejected** because it makes reasoning about state very difficult.

### Option 4: Event Sourcing
Complete rewrite using event sourcing pattern. **Rejected** as too invasive for current timeline.

### Option 5: Synchronous Barriers
Block simulation until GUI acknowledges each message. **Rejected** due to architectural complexity.

## Implementation Order

1. ? Create `ParseSingleLogEntry()` method
2. ? Modify `Add()` to capture perspective immediately
3. ? Simplify `FlushEvents()` to just send events
4. ? Test with faint scenario (Miraidon ? Ursaluna)
5. ? Test team preview
6. ? Test multi-hit moves and multiple faints
7. ? Performance profiling
8. ? Mark `ParseLogToMessages()` obsolete or remove

## Related Files

- `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs` - Core changes
- `ApogeeVGC/Sim/BattleClasses/BattleEvent.cs` - Event structure (no changes needed)
- `ApogeeVGC/Sim/BattleClasses/BattlePerspective.cs` - Perspective structure (no changes needed)
- `ApogeeVGC/Sim/BattleClasses/Battle.Fainting.cs` - FlushEvents() caller
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Switch.cs` - FlushEvents() caller
- `ApogeeVGC/Gui/BattleGame.cs` - Event consumer (no changes needed)
- `ApogeeVGC/Gui/Animations/AnimationCoordinator.cs` - Animation processing (no changes needed)

## Success Criteria

The fix is successful when:
1. Fainted Pokémon remain visible until their disappear animation completes
2. Replacement Pokémon only appear **after** the fainted Pokémon's animation
3. All damage values and HP bars correspond to the correct perspective
4. No visual "jumps" or state inconsistencies in the GUI
5. Performance remains within acceptable bounds (<5ms overhead per battle)

## Notes

- This fix addresses the **root cause** rather than patching symptoms
- The synchronization issue has been present since the event system was implemented
- This change makes the event system work as originally intended (perspective per message)
- Future optimizations could batch perspective generation if performance becomes an issue
