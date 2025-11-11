# EventHandlerInfo Generator Scripts

This directory contains scripts to automatically generate the remaining EventHandlerInfo records for all prefix variants (Foe, Source, Ally, Any).

## Quick Start

### Option 1: Simple PowerShell Script (Recommended)

No additional tools required! Just run:

```powershell
.\Scripts\GenerateEventHandlerInfoRecords-Simple.ps1
```

This will generate all missing prefix variants for the events defined in the script.

### Option 2: C# Script with dotnet-script

For the full-featured C# implementation:

```powershell
.\Scripts\GenerateEventHandlerInfoRecords.ps1
```

This will:
1. Install `dotnet-script` if needed
2. Run the C# generator script
3. Create all missing EventHandlerInfo records

## What Gets Generated

The scripts will create EventHandlerInfo records for all combinations of:

**Prefixes:** Foe, Source, Ally, Any

**Events:** ~50 base events including:
- EmergencyExit, BeforeSwitchIn, BeforeTurn, Update
- Attract, ChargeMove
- ModifyDef, ModifyCritRatio, ModifyPriority, ModifyStab
- ModifySpe, ModifyWeight
- MoveAborted, DisableMove, DeductPp, EntryHazard
- ChangeBoost, AfterMove, AfterMoveSelf, AfterMoveSecondary
- AfterEachBoost, AfterSubDamage, AfterSwitchInSelf
- Immunity, CriticalHit, Flinch, LockMove
- FractionalPriority, TryHitField
- SwitchIn, Swap, WeatherModifyDamage, StallMove
- AfterTerastallization, Residual
- And more...

**Total:** ~120-140 additional records (depending on what already exists)

## Generated File Format

Each generated file follows this pattern:

```csharp
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
// ... other required usings

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for On[Prefix][Event] event.
/// Signature: [DelegateType]
/// </summary>
public sealed record On[Prefix][Event]EventInfo : EventHandlerInfo
{
    public On[Prefix][Event]EventInfo(
        [DelegateType] handler,
   int? priority = null,
        bool usesSpeed = true)
    {
    Id = EventId.[Event];
        Prefix = EventPrefix.[Prefix];
        Handler = handler;
     Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [...];
        ExpectedReturnType = typeof([ReturnType]);
    }
}
```

## After Generation

1. **Review** the generated files in `ApogeeVGC\Sim\Events\Handlers\EventMethods\`
2. **Build** the project: `dotnet build`
3. **Verify** all files compile successfully
4. **Commit** the new files to version control

## Customizing the Generator

### Adding More Events

Edit the `$Events` array in `GenerateEventHandlerInfoRecords-Simple.ps1`:

```powershell
$Events = @(
    @{Name="YourEvent"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    # ... more events
)
```

### Changing Prefixes

Modify the `$Prefixes` array:

```powershell
$Prefixes = @("Foe", "Source", "Ally", "Any", "YourPrefix")
```

## Expected Output

Running the script will produce output like:

```
EventHandlerInfo Generator
=========================

? Created OnFoeEmergencyExitEventInfo.cs
? Created OnFoeBeforeSwitchInEventInfo.cs
? Created OnFoeBeforeTurnEventInfo.cs
??  Skipping OnFoeUpdateEventInfo.cs (already exists)
...

?? Summary:
   Created: 98
   Skipped: 24
   Total:   122

? Generation complete!

?? Next steps:
   1. Review generated files in ApogeeVGC\Sim\Events\Handlers\EventMethods\
   2. Run: dotnet build
   3. Verify compilation
```

## Troubleshooting

### "File already exists" messages
This is normal! The script skips files that already exist to avoid overwriting your work.

### Build errors after generation
1. Check that all required `using` statements are included
2. Verify the EventId enum has entries for all events
3. Ensure delegate signatures match expectations

### Script execution policy error
Run PowerShell as Administrator and execute:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

## Benefits of Script Generation

? **Consistency** - All records follow the same pattern
? **Speed** - Generate 100+ files in seconds vs hours of manual work
? **Accuracy** - No typos or copy-paste errors
? **Maintainability** - Easy to regenerate if patterns change
? **Documentation** - Auto-generated XML comments

## Current Status

As of the last manual creation session:
- **162 records** exist
- **~120-140 more** can be generated with these scripts
- **Target: 280-300 total** for comprehensive coverage

Running these scripts will push coverage from ~44% to ~75-80%!
