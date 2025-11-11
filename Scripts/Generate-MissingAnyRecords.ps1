#!/usr/bin/env pwsh
# Script to generate missing Any prefix EventHandlerInfo records

$OutputDirectory = "ApogeeVGC\Sim\Events\Handlers\EventMethods"

Write-Host "Generating missing Any prefix EventHandlerInfo records..." -ForegroundColor Cyan
Write-Host ""

# Define all missing Any prefix records (49 total)
$MissingRecords = @{
  "OnAnyAfterUseItem" = @{
    Delegate = "Action<Battle, Item, Pokemon>"
     Return = "void"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
   BaseEvent = "AfterUseItem"
        Prefix = "Any"
    }
    "OnAnyAfterFaint" = @{
Delegate = "Action<Battle, int, Pokemon, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "AfterFaint"
   Prefix = "Any"
    }
"OnAnyAfterMoveSecondarySelf" = @{
     Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
  Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   BaseEvent = "AfterMoveSecondarySelf"
 Prefix = "Any"
    }
    "OnAnyAccuracy" = @{
   Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>"
        Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   BaseEvent = "Accuracy"
        Prefix = "Any"
    }
"OnAnyBeforeFaint" = @{
        Delegate = "Action<Battle, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(IEffect)")
   BaseEvent = "BeforeFaint"
   Prefix = "Any"
    }
    "OnAnyBeforeSwitchOut" = @{
   Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
BaseEvent = "BeforeSwitchOut"
        Prefix = "Any"
    }
    "OnAnyTryBoost" = @{
   Delegate = "Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>"
        Return = "void"
     Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
   BaseEvent = "TryBoost"
   Prefix = "Any"
    }
    "OnAnyDamage" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>"
    Return = "IntBoolVoidUnion"
   Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
    BaseEvent = "Damage"
        Prefix = "Any"
    }
    "OnAnyDragOut" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon?, ActiveMove?>"
Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
 BaseEvent = "DragOut"
        Prefix = "Any"
    }
    "OnAnyEatItem" = @{
  Delegate = "Action<Battle, Item, Pokemon>"
     Return = "void"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
        BaseEvent = "EatItem"
   Prefix = "Any"
    }
    "OnAnyEffectiveness" = @{
        Delegate = "Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>"
      Return = "IntVoidUnion"
  Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(PokemonType)", "typeof(ActiveMove)")
    BaseEvent = "Effectiveness"
        Prefix = "Any"
    }
    "OnAnyMaybeTrapPokemon" = @{
  Delegate = "Action<Battle, Pokemon, Pokemon?>"
 Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)")
        BaseEvent = "MaybeTrapPokemon"
   Prefix = "Any"
    }
    "OnAnyModifyAtk" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
  Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   BaseEvent = "ModifyAtk"
        Prefix = "Any"
    }
    "OnAnyModifyBoost" = @{
        Delegate = "Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>"
        Return = "SparseBoostsTableVoidUnion"
   Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)")
        BaseEvent = "ModifyBoost"
  Prefix = "Any"
    }
    "OnAnyModifyDamage" = @{
  Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   BaseEvent = "ModifyDamage"
   Prefix = "Any"
    }
    "OnAnyModifyMove" = @{
   Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon?>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
      BaseEvent = "ModifyMove"
        Prefix = "Any"
    }
    "OnAnyModifySecondaries" = @{
      Delegate = "Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
  Params = @("typeof(Battle)", "typeof(List<SecondaryEffect>)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifySecondaries"
        Prefix = "Any"
 }
    "OnAnyModifySpA" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifySpA"
     Prefix = "Any"
    }
    "OnAnyModifySpD" = @{
 Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
 Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifySpD"
        Prefix = "Any"
    }
    "OnAnyModifyType" = @{
        Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
     BaseEvent = "ModifyType"
        Prefix = "Any"
    }
    "OnAnyModifyTarget" = @{
   Delegate = "Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>"
      Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
BaseEvent = "ModifyTarget"
        Prefix = "Any"
}
    "OnAnyNegateImmunity" = @{
  Delegate = "Func<Battle, Pokemon, PokemonType?, BoolVoidUnion>"
   Return = "BoolVoidUnion"
    Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(PokemonType)")
        BaseEvent = "NegateImmunity"
      Prefix = "Any"
    }
    "OnAnyOverrideAction" = @{
   Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>"
    Return = "DelegateVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
  BaseEvent = "OverrideAction"
        Prefix = "Any"
    }
    "OnAnyPseudoWeatherChange" = @{
  Delegate = "Action<Battle, Pokemon, Pokemon, Condition>"
   Return = "void"
    Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Condition)")
     BaseEvent = "PseudoWeatherChange"
        Prefix = "Any"
    }
    "OnAnyRedirectTarget" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>"
Return = "PokemonVoidUnion"
 Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)", "typeof(ActiveMove)")
        BaseEvent = "RedirectTarget"
        Prefix = "Any"
    }
    "OnAnySetAbility" = @{
        Delegate = "Func<Battle, Ability, Pokemon, Pokemon, IEffect, bool?>"
Return = "bool?"
        Params = @("typeof(Battle)", "typeof(Ability)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
BaseEvent = "SetAbility"
   Prefix = "Any"
    }
    "OnAnySetStatus" = @{
        Delegate = "Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion>"
        Return = "BoolVoidUnion"
   Params = @("typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "SetStatus"
        Prefix = "Any"
  }
    "OnAnySetWeather" = @{
      Delegate = "Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>"
  Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Condition)")
        BaseEvent = "SetWeather"
        Prefix = "Any"
    }
    "OnAnyTakeItem" = @{
      Delegate = "Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
   Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)", "typeof(Pokemon)")
  BaseEvent = "TakeItem"
        Prefix = "Any"
    }
    "OnAnyTerrain" = @{
  Delegate = "Action<Battle, Pokemon>"
   Return = "void"
     Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "Terrain"
        Prefix = "Any"
    }
    "OnAnyTrapPokemon" = @{
      Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "TrapPokemon"
      Prefix = "Any"
    }
    "OnAnyTryAddVolatile" = @{
 Delegate = "Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>"
  Return = "BoolVoidUnion"
     Params = @("typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "TryAddVolatile"
        Prefix = "Any"
    }
    "OnAnyTryEatItem" = @{
        Delegate = "Func<Battle, Item, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
      Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
BaseEvent = "TryEatItem"
        Prefix = "Any"
    }
    "OnAnyTryHeal" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>"
Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "TryHeal"
        Prefix = "Any"
    }
"OnAnyTryHitSide" = @{
 Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
 Return = "BoolEmptyVoidUnion"
      Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "TryHitSide"
        Prefix = "Any"
 }
    "OnAnyTryPrimaryHit" = @{
 Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>"
        Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     BaseEvent = "TryPrimaryHit"
        Prefix = "Any"
    }
    "OnAnyType" = @{
        Delegate = "Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>"
        Return = "TypesVoidUnion"
  Params = @("typeof(Battle)", "typeof(PokemonType[])", "typeof(Pokemon)")
     BaseEvent = "Type"
   Prefix = "Any"
    }
    "OnAnyModifyDamagePhase1" = @{
  Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
     Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   BaseEvent = "ModifyDamagePhase1"
     Prefix = "Any"
    }
"OnAnyModifyDamagePhase2" = @{
   Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
 Return = "DoubleVoidUnion"
    Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyDamagePhase2"
Prefix = "Any"
    }
    "OnAnyAfterSubDamage" = @{
        Delegate = "Action<Battle, int, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
BaseEvent = "AfterSubDamage"
        Prefix = "Any"
    }
    "OnAnyAfterSwitchInSelf" = @{
    Delegate = "Action<Battle, Pokemon>"
        Return = "void"
     Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "AfterSwitchInSelf"
        Prefix = "Any"
    }
    "OnAnyAfterEachBoost" = @{
   Delegate = "Action<Battle, SparseBoostsTable, Pokemon, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)")
        BaseEvent = "AfterEachBoost"
   Prefix = "Any"
    }
    "OnAnyAfterMoveSecondary" = @{
      Delegate = "Action<Battle, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "AfterMoveSecondary"
   Prefix = "Any"
    }
    "OnAnyAfterMove" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
    Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "AfterMove"
   Prefix = "Any"
    }
    "OnAnyAfterMoveSelf" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "AfterMoveSelf"
        Prefix = "Any"
    }
    "OnAnyAttract" = @{
Delegate = "Action<Battle, Pokemon, Pokemon>"
Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)")
 BaseEvent = "Attract"
        Prefix = "Any"
    }
    "OnAnyBeforeSwitchIn" = @{
   Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "BeforeSwitchIn"
        Prefix = "Any"
    }
    "OnAnyChargeMove" = @{
    Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
  Return = "BoolVoidUnion"
   Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
  BaseEvent = "ChargeMove"
        Prefix = "Any"
    }
    "OnAnyCriticalHit" = @{
    Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
Return = "BoolVoidUnion"
Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
  BaseEvent = "CriticalHit"
        Prefix = "Any"
    }
    "OnAnyDeductPp" = @{
    Delegate = "Func<Battle, Pokemon, Pokemon, IntVoidUnion>"
        Return = "IntVoidUnion"
   Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)")
    BaseEvent = "DeductPp"
        Prefix = "Any"
    }
    "OnAnyDisableMove" = @{
Delegate = "Action<Battle, Pokemon>"
Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "DisableMove"
        Prefix = "Any"
  }
    "OnAnyFlinch" = @{
   Delegate = "Func<Battle, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
     Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "Flinch"
        Prefix = "Any"
    }
    "OnAnyImmunity" = @{
   Delegate = "Action<Battle, PokemonType, Pokemon>"
    Return = "void"
        Params = @("typeof(Battle)", "typeof(PokemonType)", "typeof(Pokemon)")
        BaseEvent = "Immunity"
        Prefix = "Any"
    }
    "OnAnyLockMove" = @{
        Delegate = "Func<Battle, Pokemon, ActiveMove?>"
  Return = "ActiveMove"
      Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "LockMove"
 Prefix = "Any"
    }
    "OnAnyModifyCritRatio" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "ModifyCritRatio"
        Prefix = "Any"
    }
    "OnAnyModifyDef" = @{
      Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
 Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
 BaseEvent = "ModifyDef"
        Prefix = "Any"
    }
    "OnAnyModifyPriority" = @{
      Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
    Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     BaseEvent = "ModifyPriority"
        Prefix = "Any"
    }
    "OnAnyModifySpe" = @{
 Delegate = "Func<Battle, int, Pokemon, IntVoidUnion>"
        Return = "IntVoidUnion"
   Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)")
     BaseEvent = "ModifySpe"
        Prefix = "Any"
    }
    "OnAnyModifyStab" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   BaseEvent = "ModifyStab"
    Prefix = "Any"
    }
    "OnAnyModifyWeight" = @{
        Delegate = "Func<Battle, int, Pokemon, IntVoidUnion>"
        Return = "IntVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)")
        BaseEvent = "ModifyWeight"
 Prefix = "Any"
    }
    "OnAnyMoveAborted" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
 BaseEvent = "MoveAborted"
     Prefix = "Any"
    }
    "OnAnyResidual" = @{
      Delegate = "Action<Battle, PokemonSideUnion, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(PokemonSideUnion)", "typeof(Pokemon)", "typeof(IEffect)")
        BaseEvent = "Residual"
      Prefix = "Any"
    }
    "OnAnyStallMove" = @{
        Delegate = "Func<Battle, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        BaseEvent = "StallMove"
        Prefix = "Any"
    }
 "OnAnyTryHitField" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        BaseEvent = "TryHitField"
    Prefix = "Any"
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
    if ($Delegate -match "PokemonSideUnion") { $usings += "using ApogeeVGC.Sim.SideClasses;" }
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
    Write-Host ""
    Write-Host "?? Final step:" -ForegroundColor Cyan
    Write-Host "   Run: dotnet build" -ForegroundColor White
    Write-Host "   to verify IEventMethodsV2.cs compiles!" -ForegroundColor White
} else {
    Write-Host "??  All records already exist!" -ForegroundColor Blue
}
