#!/usr/bin/env pwsh
# Script to generate ONLY the missing EventHandlerInfo records needed for IEventMethodsV2.cs

$OutputDirectory = "ApogeeVGC\Sim\Events\Handlers\EventMethods"

# Get list of existing records
$existingFiles = Get-ChildItem $OutputDirectory -Filter "*EventInfo.cs" | 
    Select-Object -ExpandProperty BaseName

Write-Host "Found $($existingFiles.Count) existing EventHandlerInfo records" -ForegroundColor Cyan
Write-Host ""

# Define the missing records based on compilation errors from IEventMethodsV2.cs
$MissingRecords = @{
    # Foe Prefix
    "OnFoeBeforeSwitchOut" = @{
        Delegate = "Action<Battle, Pokemon>"
  Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "BeforeSwitchOut"
        Prefix = "Foe"
    }
    "OnFoeDragOut" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon?, ActiveMove?>"
        Return = "void"
   Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "DragOut"
 Prefix = "Foe"
    }
    "OnFoeEatItem" = @{
      Delegate = "Action<Battle, Item, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
        BaseEvent = "EatItem"
        Prefix = "Foe"
    }
"OnFoeEffectiveness" = @{
      Delegate = "Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>"
    Return = "IntVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(PokemonType)", "typeof(ActiveMove)")
      BaseEvent = "Effectiveness"
        Prefix = "Foe"
    }
    "OnFoeFaint" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
 BaseEvent = "Faint"
    Prefix = "Foe"
    }
    "OnFoeHit" = @{
  Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
 BaseEvent = "Hit"
   Prefix = "Foe"
    }
    "OnFoeModifyAccuracy" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
   Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyAccuracy"
        Prefix = "Foe"
    }
    "OnFoeModifyAtk" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
      Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    BaseEvent = "ModifyAtk"
        Prefix = "Foe"
    }
    "OnFoeModifyBoost" = @{
    Delegate = "Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>"
      Return = "SparseBoostsTableVoidUnion"
        Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)")
        BaseEvent = "ModifyBoost"
        Prefix = "Foe"
    }
 "OnFoeModifyDamage" = @{
   Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyDamage"
        Prefix = "Foe"
    }
    "OnFoeModifyMove" = @{
        Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon?>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
        BaseEvent = "ModifyMove"
        Prefix = "Foe"
    }
    "OnFoeModifySecondaries" = @{
        Delegate = "Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(List<SecondaryEffect>)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   BaseEvent = "ModifySecondaries"
     Prefix = "Foe"
    }
    "OnFoeModifySpA" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    BaseEvent = "ModifySpA"
        Prefix = "Foe"
    }
    "OnFoeModifyType" = @{
    Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon>"
      Return = "void"
        Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
        BaseEvent = "ModifyType"
  Prefix = "Foe"
    }
    "OnFoeModifyTarget" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
    Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyTarget"
   Prefix = "Foe"
    }
    "OnFoeNegateImmunity" = @{
        Delegate = "Func<Battle, Pokemon, PokemonType?, BoolVoidUnion>"
      Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(PokemonType)")
     BaseEvent = "NegateImmunity"
        Prefix = "Foe"
}
    "OnFoeOverrideAction" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>"
    Return = "DelegateVoidUnion"
   Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "OverrideAction"
        Prefix = "Foe"
    }
    "OnFoePrepareHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "PrepareHit"
        Prefix = "Foe"
  }
    "OnFoeSetAbility" = @{
      Delegate = "Func<Battle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Ability)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "SetAbility"
        Prefix = "Foe"
    }
    "OnFoeSetWeather" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>"
    Return = "BoolVoidUnion"
     Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Condition)")
  BaseEvent = "SetWeather"
        Prefix = "Foe"
    }
    "OnFoeTakeItem" = @{
        Delegate = "Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)", "typeof(Pokemon)")
        BaseEvent = "TakeItem"
Prefix = "Foe"
    }
    "OnFoeTerrain" = @{
        Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
      BaseEvent = "Terrain"
 Prefix = "Foe"
    }
    "OnFoeTryAddVolatile" = @{
        Delegate = "Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
BaseEvent = "TryAddVolatile"
        Prefix = "Foe"
    }
    "OnFoeTryEatItem" = @{
        Delegate = "Func<Battle, Item, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
        BaseEvent = "TryEatItem"
        Prefix = "Foe"
    }
    "OnFoeTryHeal" = @{
   Delegate = "Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>"
        Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
     BaseEvent = "TryHeal"
     Prefix = "Foe"
    }
    "OnFoeTryHit" = @{
  Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>"
        Return = "BoolIntEmptyVoidUnion"
     Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     BaseEvent = "TryHit"
        Prefix = "Foe"
    }
    "OnFoeTryHitSide" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
  Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "TryHitSide"
        Prefix = "Foe"
    }
    "OnFoeInvulnerability" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>"
   Return = "BoolIntEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "Invulnerability"
    Prefix = "Foe"
    }
    "OnFoeTryMove" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "TryMove"
        Prefix = "Foe"
    }
    "OnFoeTryPrimaryHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>"
        Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
  BaseEvent = "TryPrimaryHit"
        Prefix = "Foe"
    }
    "OnFoeType" = @{
        Delegate = "Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>"
      Return = "TypesVoidUnion"
        Params = @("typeof(Battle)", "typeof(PokemonType[])", "typeof(Pokemon)")
 BaseEvent = "Type"
        Prefix = "Foe"
    }
    "OnFoeModifyDamagePhase1" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyDamagePhase1"
     Prefix = "Foe"
    }
    "OnFoeModifyDamagePhase2" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
      Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyDamagePhase2"
        Prefix = "Foe"
    }
}

function Get-RequiredUsings {
    param($Delegate, $Return)
    
    $usings = @(
        "using ApogeeVGC.Sim.BattleClasses;",
     "using ApogeeVGC.Sim.PokemonClasses;"
    )
    
    if ($Delegate -match "ActiveMove|Move") { $usings += "using ApogeeVGC.Sim.Moves;" }
    if ($Delegate -match "IEffect|Effect") { $usings += "using ApogeeVGC.Sim.Effects;" }
  if ($Delegate -match "Item") { $usings += "using ApogeeVGC.Sim.Items;" }
    if ($Delegate -match "Condition") { $usings += "using ApogeeVGC.Sim.Conditions;" }
    if ($Delegate -match "Ability") { $usings += "using ApogeeVGC.Sim.Abilities;" }
    if ($Delegate -match "SparseBoostsTable") { $usings += "using ApogeeVGC.Sim.Stats;" }
    if ($Delegate -match "SecondaryEffect") { $usings += "using ApogeeVGC.Sim.Moves;" }
    if ($Delegate -match "PokemonType") { $usings += "using ApogeeVGC.Sim.PokemonClasses;" }
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

Write-Host "Generating missing EventHandlerInfo records..." -ForegroundColor Cyan
Write-Host ""

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
    Write-Host ""
    Write-Host "?? Next steps:" -ForegroundColor Cyan
    Write-Host "   1. Run: dotnet build" -ForegroundColor White
    Write-Host "   2. Verify IEventMethodsV2.cs compiles" -ForegroundColor White
} else {
    Write-Host "??  All records already exist!" -ForegroundColor Blue
}
