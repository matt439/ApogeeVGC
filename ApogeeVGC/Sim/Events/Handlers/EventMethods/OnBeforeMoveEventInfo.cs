using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnBeforeMove event.
/// Triggered before a Pokemon uses a move, can prevent the move.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => BoolVoidUnion
/// </summary>
public sealed record OnBeforeMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnBeforeMove event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate (VoidSourceMoveHandler)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnBeforeMoveEventInfo(
        VoidSourceMoveHandler handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeMove;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle), 
    typeof(Pokemon), // target
         typeof(Pokemon), // source
 typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
        // Nullability: Battle (non-null), target (non-null), source (non-null), move (non-null)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false; // BoolVoidUnion is a struct, never null
        
     // Validate configuration
        ValidateConfiguration();
    }
}
