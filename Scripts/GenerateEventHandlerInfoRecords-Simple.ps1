#!/usr/bin/env pwsh
# Simple PowerShell script to generate EventHandlerInfo records directly
# No external tools required!

$OutputDirectory = "ApogeeVGC\Sim\Events\Handlers\EventMethods"

# Event definitions with their signatures
$Events = @(
    @{Name="EmergencyExit"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="BeforeSwitchIn"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="BeforeTurn"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
 @{Name="Update"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="Attract"; Delegate="Action<Battle, Pokemon, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)")},
    @{Name="ChargeMove"; Delegate="Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"; Return="BoolVoidUnion"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="ModifyDef"; Delegate="Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"; Return="DoubleVoidUnion"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="ModifyCritRatio"; Delegate="Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"; Return="DoubleVoidUnion"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="ModifyPriority"; Delegate="Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"; Return="DoubleVoidUnion"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="ModifyStab"; Delegate="Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"; Return="DoubleVoidUnion"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="ModifySpe"; Delegate="Func<Battle, int, Pokemon, IntVoidUnion>"; Return="IntVoidUnion"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)")},
    @{Name="ModifyWeight"; Delegate="Func<Battle, int, Pokemon, IntVoidUnion>"; Return="IntVoidUnion"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)")},
    @{Name="MoveAborted"; Delegate="Action<Battle, Pokemon, Pokemon, ActiveMove>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="DisableMove"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="DeductPp"; Delegate="Func<Battle, Pokemon, Pokemon, int>"; Return="int"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)")},
    @{Name="EntryHazard"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="ChangeBoost"; Delegate="Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>"; Return="void"; Params=@("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")},
    @{Name="AfterMove"; Delegate="Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"; Return="BoolVoidUnion"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="AfterMoveSelf"; Delegate="Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"; Return="BoolVoidUnion"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="AfterMoveSecondary"; Delegate="Action<Battle, Pokemon, Pokemon, ActiveMove>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="AfterEachBoost"; Delegate="Action<Battle, SparseBoostsTable, Pokemon, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)")},
    @{Name="AfterSubDamage"; Delegate="Action<Battle, int, Pokemon, Pokemon, ActiveMove>"; Return="void"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="AfterSwitchInSelf"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="Immunity"; Delegate="Action<Battle, PokemonType, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(PokemonType)", "typeof(Pokemon)")},
    @{Name="CriticalHit"; Delegate="Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"; Return="BoolVoidUnion"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="Flinch"; Delegate="Func<Battle, Pokemon, BoolVoidUnion>"; Return="BoolVoidUnion"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
 @{Name="LockMove"; Delegate="Func<Battle, Pokemon, ActiveMove?>"; Return="ActiveMove"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="FractionalPriority"; Delegate="Func<Battle, int, Pokemon, ActiveMove, double>"; Return="double"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="TryHitField"; Delegate="Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"; Return="BoolEmptyVoidUnion"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="SwitchIn"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="Swap"; Delegate="Action<Battle, Pokemon, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)")},
    @{Name="WeatherModifyDamage"; Delegate="Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"; Return="DoubleVoidUnion"; Params=@("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")},
    @{Name="StallMove"; Delegate="Func<Battle, Pokemon, BoolVoidUnion>"; Return="BoolVoidUnion"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
 @{Name="AfterTerastallization"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")},
    @{Name="Residual"; Delegate="Action<Battle, Pokemon>"; Return="void"; Params=@("typeof(Battle)", "typeof(Pokemon)")}
)

$Prefixes = @("Foe", "Source", "Ally", "Any")

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
    if ($Delegate -match "Side") { $usings += "using ApogeeVGC.Sim.SideClasses;" }
    if ($Delegate -match "SparseBoostsTable") { $usings += "using ApogeeVGC.Sim.Stats;" }
    if ($Delegate -match "Union" -or $Return -match "Union") { $usings += "using ApogeeVGC.Sim.Utils.Unions;" }
    
    return $usings | Sort-Object
}

function Generate-EventHandlerInfo {
    param($Prefix, $Event)
    
    $className = "On${Prefix}$($Event.Name)EventInfo"
    $usings = Get-RequiredUsings $Event.Delegate $Event.Return
    $paramsStr = $Event.Params -join ", "
    
    $content = @"
$($usings -join "`n")

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for On${Prefix}$($Event.Name) event.
/// Signature: $($Event.Delegate)
/// </summary>
public sealed record ${className} : EventHandlerInfo
{
 public ${className}(
      $($Event.Delegate) handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.$($Event.Name);
   Prefix = EventPrefix.${Prefix};
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [$paramsStr];
        ExpectedReturnType = typeof($($Event.Return));
    }
}
"@
    
    return $content
}

Write-Host "EventHandlerInfo Generator" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan
Write-Host ""

$created = 0
$skipped = 0

foreach ($prefix in $Prefixes) {
    foreach ($event in $Events) {
        $fileName = "On${prefix}$($event.Name)EventInfo.cs"
        $filePath = Join-Path $OutputDirectory $fileName
        
        if (Test-Path $filePath) {
   Write-Host "??  Skipping $fileName (already exists)" -ForegroundColor Gray
            $skipped++
            continue
}
     
     try {
   $content = Generate-EventHandlerInfo $prefix $event
   [System.IO.File]::WriteAllText($filePath, $content)
            Write-Host "? Created $fileName" -ForegroundColor Green
            $created++
        }
        catch {
     Write-Host "? Error creating ${fileName}: $_" -ForegroundColor Red
    }
    }
}

Write-Host ""
Write-Host "?? Summary:" -ForegroundColor Cyan
Write-Host "   Created: $created" -ForegroundColor Green
Write-Host "   Skipped: $skipped" -ForegroundColor Yellow
Write-Host "   Total:   $($created + $skipped)" -ForegroundColor White
Write-Host ""
Write-Host "? Generation complete!" -ForegroundColor Green
Write-Host ""
Write-Host "?? Next steps:" -ForegroundColor Cyan
Write-Host "   1. Review generated files in $OutputDirectory" -ForegroundColor White
Write-Host "   2. Run: dotnet build" -ForegroundColor White
Write-Host "   3. Verify compilation" -ForegroundColor White
