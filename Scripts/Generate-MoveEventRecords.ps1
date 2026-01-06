#!/usr/bin/env pwsh
# Script to generate remaining move-specific EventHandlerInfo records

$OutputDirectory = "ApogeeVGC\Sim\Events\Handlers\MoveEventMethods"

# Define all remaining move event records
$MoveEvents = @{
    "OnAfterHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
    Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "AfterHit"
 Description = "Triggered after a move hits"
    }
    "OnAfterSubDamage" = @{
        Delegate = "Action<Battle, int, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
 Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "AfterSubDamage"
  Description = "Triggered after substitute damage"
    }
    "OnAfterMoveSecondarySelf" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
      EventId = "AfterMoveSecondarySelf"
        Description = "Triggered after move secondary effects on self"
    }
    "OnAfterMoveSecondary" = @{
    Delegate = "Action<Battle, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
    Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "AfterMoveSecondary"
Description = "Triggered after move secondary effects"
    }
    "OnAfterMove" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "AfterMove"
        Description = "Triggered after a move completes"
    }
    "OnDamage" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>"
        Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
    EventId = "Damage"
        Description = "Triggered to modify damage"
        NeedsIEffect = $true
    }
    "OnBasePower" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "BasePower"
      Description = "Triggered to modify base power"
    }
    "OnEffectiveness" = @{
        Delegate = "Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>"
        Return = "IntVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(PokemonType)", "typeof(ActiveMove)")
        EventId = "Effectiveness"
  Description = "Triggered to modify type effectiveness"
    }
    "OnHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "Hit"
   Description = "Triggered when a move hits"
    }
    "OnHitField" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
      Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "HitField"
        Description = "Triggered when a move hits the field"
    }
    "OnHitSide" = @{
        Delegate = "Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
    Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Side)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "HitSide"
        Description = "Triggered when a move hits a side"
        NeedsSide = $true
    }
    "OnModifyMove" = @{
        Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon?>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
        EventId = "ModifyMove"
        Description = "Triggered to modify a move"
    }
    "OnModifyPriority" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifyPriority"
        Description = "Triggered to modify move priority"
    }
    "OnMoveFail" = @{
   Delegate = "Action<Battle, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    EventId = "MoveFail"
        Description = "Triggered when a move fails"
    }
    "OnModifyType" = @{
     Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
        EventId = "ModifyType"
        Description = "Triggered to modify move type"
    }
    "OnModifyTarget" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
      Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
  EventId = "ModifyTarget"
        Description = "Triggered to modify move target"
}
    "OnPrepareHit" = @{
 Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     EventId = "PrepareHit"
        Description = "Triggered to prepare a hit"
    }
    "OnTry" = @{
   Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
     Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
 EventId = "Try"
        Description = "Triggered to try executing a move"
    }
    "OnTryHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>"
 Return = "BoolIntEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "TryHit"
        Description = "Triggered to try hitting with a move"
    }
    "OnTryHitField" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "TryHitField"
        Description = "Triggered to try hitting the field"
    }
    "OnTryHitSide" = @{
        Delegate = "Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Side)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "TryHitSide"
        Description = "Triggered to try hitting a side"
        NeedsSide = $true
    }
    "OnTryImmunity" = @{
 Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "TryImmunity"
  Description = "Triggered to check immunity"
    }
    "OnTryMove" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
    Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    EventId = "TryMove"
      Description = "Triggered to try using a move"
    }
    "OnUseMoveMessage" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
    Return = "BoolVoidUnion"
 Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "UseMoveMessage"
        Description = "Triggered to display move use message"
    }
}

function Get-RequiredUsings {
    param($EventDef)
    
    $usings = @(
 "using ApogeeVGC.Sim.BattleClasses;",
     "using ApogeeVGC.Sim.Moves;",
        "using ApogeeVGC.Sim.PokemonClasses;"
    )
    
    if ($EventDef.NeedsSide) { $usings += "using ApogeeVGC.Sim.SideClasses;" }
    if ($EventDef.NeedsIEffect) { $usings += "using ApogeeVGC.Sim.Effects;" }
    if ($EventDef.Delegate -match "Union" -or $EventDef.Return -match "Union") { 
        $usings += "using ApogeeVGC.Sim.Utils.Unions;" 
    }
    
    return $usings | Sort-Object -Unique
}

function Generate-EventHandlerInfo {
    param($EventName, $EventDef)
    
    $className = "${EventName}EventInfo"
    $usings = Get-RequiredUsings $EventDef
    $paramsStr = $EventDef.Params -join ", "
    
    $content = @"
$($usings -join "`n")

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for $EventName event (move-specific).
/// $($EventDef.Description).
/// Signature: $($EventDef.Delegate)
/// </summary>
public sealed record ${className} : EventHandlerInfo
{
public ${className}(
        $($EventDef.Delegate) handler,
        int? priority = null,
   bool usesSpeed = true)
    {
    Id = EventId.$($EventDef.EventId);
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [$paramsStr];
        ExpectedReturnType = typeof($($EventDef.Return));
    }
}
"@
  
    return $content
}

Write-Host "Generating move-specific EventHandlerInfo records..." -ForegroundColor Cyan
Write-Host ""

$created = 0
$skipped = 0

foreach ($entry in $MoveEvents.GetEnumerator()) {
    $eventName = $entry.Key
    $eventDef = $entry.Value
    $fileName = "${eventName}EventInfo.cs"
    $filePath = Join-Path $OutputDirectory $fileName
    
    if (Test-Path $filePath) {
        Write-Host "??  Skipping $fileName (already exists)" -ForegroundColor Gray
        $skipped++
        continue
    }
    
    try {
        $content = Generate-EventHandlerInfo $eventName $eventDef
        [System.IO.File]::WriteAllText($filePath, $content)
        Write-Host "? Created $fileName" -ForegroundColor Green
        $created++
    }
    catch {
 Write-Host "? Error creating ${fileName}: $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Summary:" -ForegroundColor Cyan
Write-Host "   Created: $created" -ForegroundColor Green
Write-Host "   Skipped: $skipped" -ForegroundColor Yellow
Write-Host "   Total:   $($created + $skipped)" -ForegroundColor White
Write-Host ""

if ($created -gt 0) {
    Write-Host "? Generation complete!" -ForegroundColor Green
}
