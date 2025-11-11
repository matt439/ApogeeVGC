#!/usr/bin/env pwsh
# Script to generate missing Source prefix EventHandlerInfo records

$OutputDirectory = "ApogeeVGC\Sim\Events\Handlers\EventMethods"

Write-Host "Generating missing Source prefix EventHandlerInfo records..." -ForegroundColor Cyan
Write-Host ""

# Define all missing Source prefix records (39 total)
$MissingRecords = @{
    "OnSourceAfterUseItem" = @{
    Delegate = "Action<Battle, Item, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
        BaseEvent = "AfterUseItem"
 Prefix = "Source"
 }
    "OnSourceAfterBoost" = @{
        Delegate = "Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "AfterBoost"
        Prefix = "Source"
    }
    "OnSourceAfterFaint" = @{
   Delegate = "Action<Battle, int, Pokemon, Pokemon, IEffect>"
 Return = "void"
Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "AfterFaint"
     Prefix = "Source"
    }
    "OnSourceAfterMoveSecondarySelf" = @{
   Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
      Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     BaseEvent = "AfterMoveSecondarySelf"
        Prefix = "Source"
    }
    "OnSourceBeforeFaint" = @{
        Delegate = "Action<Battle, Pokemon, IEffect>"
        Return = "void"
   Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "BeforeFaint"
    Prefix = "Source"
    }
    "OnSourceBeforeSwitchOut" = @{
    Delegate = "Action<Battle, Pokemon>"
     Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
      BaseEvent = "BeforeSwitchOut"
        Prefix = "Source"
 }
    "OnSourceTryBoost" = @{
        Delegate = "Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "TryBoost"
        Prefix = "Source"
  }
  "OnSourceDragOut" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon?, ActiveMove?>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "DragOut"
        Prefix = "Source"
    }
    "OnSourceEatItem" = @{
        Delegate = "Action<Battle, Item, Pokemon>"
 Return = "void"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
     BaseEvent = "EatItem"
        Prefix = "Source"
    }
    "OnSourceEffectiveness" = @{
        Delegate = "Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>"
        Return = "IntVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(PokemonType)", "typeof(ActiveMove)")
        BaseEvent = "Effectiveness"
        Prefix = "Source"
    }
    "OnSourceFaint" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, IEffect>"
        Return = "void"
  Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "Faint"
        Prefix = "Source"
  }
    "OnSourceHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
    Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
  BaseEvent = "Hit"
      Prefix = "Source"
    }
    "OnSourceMaybeTrapPokemon" = @{
  Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "MaybeTrapPokemon"
        Prefix = "Source"
    }
    "OnSourceModifyBoost" = @{
        Delegate = "Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>"
    Return = "SparseBoostsTableVoidUnion"
        Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)")
        BaseEvent = "ModifyBoost"
   Prefix = "Source"
    }
    "OnSourceModifyMove" = @{
        Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon?>"
  Return = "void"
        Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
        BaseEvent = "ModifyMove"
      Prefix = "Source"
    }
    "OnSourceModifySecondaries" = @{
        Delegate = "Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>"
     Return = "void"
        Params = @("typeof(Battle)", "typeof(List<SecondaryEffect>)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    BaseEvent = "ModifySecondaries"
        Prefix = "Source"
    }
    "OnSourceModifySpD" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
 Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     BaseEvent = "ModifySpD"
        Prefix = "Source"
    }
    "OnSourceModifyType" = @{
     Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon>"
     Return = "void"
        Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
        BaseEvent = "ModifyType"
        Prefix = "Source"
    }
    "OnSourceModifyTarget" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyTarget"
      Prefix = "Source"
    }
    "OnSourceNegateImmunity" = @{
 Delegate = "Func<Battle, Pokemon, PokemonType?, BoolVoidUnion>"
        Return = "BoolVoidUnion"
    Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(PokemonType)")
    BaseEvent = "NegateImmunity"
        Prefix = "Source"
}
  "OnSourceOverrideAction" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>"
        Return = "DelegateVoidUnion"
      Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "OverrideAction"
     Prefix = "Source"
    }
"OnSourceRedirectTarget" = @{
 Delegate = "Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>"
 Return = "PokemonVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)", "typeof(ActiveMove)")
        BaseEvent = "RedirectTarget"
        Prefix = "Source"
    }
    "OnSourceSetAbility" = @{
        Delegate = "Func<Battle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>"
        Return = "BoolVoidUnion"
   Params = @("typeof(Battle)", "typeof(Ability)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
   BaseEvent = "SetAbility"
        Prefix = "Source"
    }
    "OnSourceSetStatus" = @{
        Delegate = "Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>"
        Return = "BoolVoidUnion"
Params = @("typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
BaseEvent = "SetStatus"
        Prefix = "Source"
    }
    "OnSourceSetWeather" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Condition)")
        BaseEvent = "SetWeather"
        Prefix = "Source"
 }
    "OnSourceSwitchOut" = @{
    Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "SwitchOut"
   Prefix = "Source"
    }
    "OnSourceTakeItem" = @{
        Delegate = "Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)", "typeof(Pokemon)")
        BaseEvent = "TakeItem"
        Prefix = "Source"
    }
    "OnSourceTerrain" = @{
   Delegate = "Action<Battle, Pokemon>"
        Return = "void"
   Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "Terrain"
        Prefix = "Source"
    }
    "OnSourceTrapPokemon" = @{
        Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
     BaseEvent = "TrapPokemon"
   Prefix = "Source"
    }
    "OnSourceTryAddVolatile" = @{
        Delegate = "Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "TryAddVolatile"
     Prefix = "Source"
    }
    "OnSourceTryEatItem" = @{
        Delegate = "Func<Battle, Item, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
  BaseEvent = "TryEatItem"
        Prefix = "Source"
    }
    "OnSourceTryHeal" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>"
 Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "TryHeal"
        Prefix = "Source"
}
    "OnSourceTryHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>"
    Return = "BoolIntEmptyVoidUnion"
     Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
BaseEvent = "TryHit"
  Prefix = "Source"
    }
    "OnSourceTryHitSide" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "TryHitSide"
        Prefix = "Source"
    }
    "OnSourceTryMove" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
      Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "TryMove"
    Prefix = "Source"
    }
    "OnSourceTryPrimaryHit" = @{
    Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>"
        Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     BaseEvent = "TryPrimaryHit"
        Prefix = "Source"
    }
    "OnSourceType" = @{
        Delegate = "Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>"
        Return = "TypesVoidUnion"
        Params = @("typeof(Battle)", "typeof(PokemonType[])", "typeof(Pokemon)")
        BaseEvent = "Type"
        Prefix = "Source"
    }
    "OnSourceModifyDamagePhase1" = @{
    Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
 BaseEvent = "ModifyDamagePhase1"
   Prefix = "Source"
    }
  "OnSourceModifyDamagePhase2" = @{
Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyDamagePhase2"
        Prefix = "Source"
    }
}

function Get-RequiredUsings {
    param($Delegate, $Return)
    
    $usings = @(
        "using ApogeeVGC.Sim.BattleClasses;",
     "using ApogeeVGC.Sim.PokemonClasses;"
    )
    
    if ($Delegate -match "ActiveMove|Move|SecondaryEffect") { $usings += "using ApogeeVGC.Sim.Moves;" }
    if ($Delegate -match "IEffect|Effect") { $usings += "using ApogeeVGC.Sim.Effects;" }
    if ($Delegate -match "Item") { $usings += "using ApogeeVGC.Sim.Items;" }
    if ($Delegate -match "Condition") { $usings += "using ApogeeVGC.Sim.Conditions;" }
  if ($Delegate -match "Ability") { $usings += "using ApogeeVGC.Sim.Abilities;" }
    if ($Delegate -match "SparseBoostsTable") { $usings += "using ApogeeVGC.Sim.Stats;" }
    if ($Delegate -match "Union" -or $Return -match "Union") { $usings += "using ApogeeVGC.Sim.Utils.Unions;" }
    
    return $usings | Sort-Object -Unique
}

function Generate-EventHandlerInfo {
    param($EventName, $EventDef)
    
    $className = "${EventName}EventInfo"
    $usings = Get-RequiredUsings $EventDef.Delegate $EventDef.Return
    $paramsStr = $EventDef.Params -join ", "
    
    $content = @"
$($usings -join "`n")

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for $EventName event.
/// Signature: $($EventDef.Delegate)
/// </summary>
public sealed record ${className} : EventHandlerInfo
{
    public ${className}(
        $($EventDef.Delegate) handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.$($EventDef.BaseEvent);
        Prefix = EventPrefix.$($EventDef.Prefix);
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

$created = 0
$skipped = 0

foreach ($entry in $MissingRecords.GetEnumerator()) {
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
} else {
    Write-Host "??  All records already exist!" -ForegroundColor Blue
}
