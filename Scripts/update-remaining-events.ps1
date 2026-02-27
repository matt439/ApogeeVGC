# Comprehensive Update Script for All 33 Remaining Event Handler Classes
# Run this from the solution root directory

$updates = @(
    # Format: FileName, ClassName, Parameters (type name pairs), ReturnType
    @{
        File = "ApogeeVGC\Sim\Events\Handlers\SideEventMethods\OnSideResidualEventInfo.cs"
        Class = "OnSideResidualEventInfo"
        EventId = "SideResidual"
        Params = @("Battle battle", "Side side", "Pokemon pokemon", "IEffect effect")
        ContextParams = @("context.Battle", "context.GetTargetSide()", "context.GetTargetPokemon()", "context.GetSourceEffect<IEffect>()")
        Return = "void"
     OptionalParams = @("int? priority = null", "int? order = null", "int? subOrder = null", "bool usesSpeed = true")
    },
    @{
        File = "ApogeeVGC\Sim\Events\Handlers\ConditionSpecific\OnEndEventInfo.cs"
        Class = "OnEndEventInfo"
        EventId = "End"
    Params = @("Battle battle", "Pokemon target")
        ContextParams = @("context.Battle", "context.GetTargetPokemon()")
        Return = "void"
        OptionalParams = @("int? priority = null")
    },
    @{
   File = "ApogeeVGC\Sim\Events\Handlers\EventMethods\OnModifyMoveEventInfo.cs"
       Class = "OnModifyMoveEventInfo"
        EventId = "ModifyMove"
        Params = @("Battle battle", "ActiveMove move", "Pokemon pokemon", "Pokemon target")
        ContextParams = @("context.Battle", "context.GetMove()", "context.GetSourcePokemon()", "context.GetTargetPokemon()")
        Return = "void"
        OptionalParams = @("int? priority = null")
    }
    # Add remaining 30 classes here...
)

Write-Host "This script template shows the pattern for updating all event handler classes."
Write-Host "The actual update process requires manual completion for accuracy."
Write-Host ""
Write-Host "Files to update: 33"
Write-Host "Pattern established in: OnBeforeMoveEventInfo, OnStartEventInfo, OnResidualEventInfo, DurationCallbackEventInfo"
Write-Host ""
Write-Host "Recommended: Use GitHub Copilot with this prompt for each file:"
Write-Host ""
Write-Host @"
Add context constructor and Create method following this exact pattern:

1. Context Constructor:
    public ClassName(
        EventHandlerDelegate contextHandler,
  <copy optional params from existing constructor>)
    {
      Id = EventId.XXX;
        ContextHandler = contextHandler;
     <copy optional param assignments>
    }

2. Create Method:
    public static ClassName Create(
      <same signature as existing constructor>,
        <optional params>)
    {
        return new ClassName(
    context => {
     <call existing handler with context.GetXxx() accessors>
         return <appropriate RelayVar or null>;
            },
            <forward optional params>
        );
    }

Reference: OnBeforeMoveEventInfo.cs
"@
