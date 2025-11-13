using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceDeductPp event.
/// Signature: Func<Battle, Pokemon, Pokemon, int>
/// </summary>
public sealed record OnSourceDeductPpEventInfo : EventHandlerInfo
{
 public OnSourceDeductPpEventInfo(
      Func<Battle, Pokemon, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.DeductPp;
   Prefix = EventPrefix.Source;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(int);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}