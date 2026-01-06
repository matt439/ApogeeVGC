using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for BeforeTurnCallback event (move-specific).
/// Callback executed before a turn begins.
/// Signature: Action&lt;Battle, Pokemon, Pokemon, ActiveMove&gt;
/// </summary>
public sealed record BeforeTurnCallbackEventInfo : EventHandlerInfo
{
    public BeforeTurnCallbackEventInfo(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeTurnCallback;
        Prefix = EventPrefix.None;
      Handler = handler;
        Priority = priority;
      UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
