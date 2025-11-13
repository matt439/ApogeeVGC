using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAfterMoveSecondarySelf event.
/// Triggered after a move's secondary effect on the user.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnAfterMoveSecondarySelfEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAfterMoveSecondarySelf event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAfterMoveSecondarySelfEventInfo(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterMoveSecondarySelf;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
        [
      typeof(Battle),
            typeof(Pokemon),
          typeof(Pokemon),
         typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
