using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

public sealed record OnAnyAfterMegaEventInfo : EventHandlerInfo
{
    public OnAnyAfterMegaEventInfo(
        Action<Battle, Pokemon> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterMega;
   Prefix = EventPrefix.Any;
        Handler = handler;
  Priority = priority;
  UsesSpeed = usesSpeed;
  ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
