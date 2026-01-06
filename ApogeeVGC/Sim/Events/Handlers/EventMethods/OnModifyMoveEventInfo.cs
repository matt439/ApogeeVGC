using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyMove event.
/// Modifies properties of a move before it executes.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, ActiveMove, Pokemon, Pokemon?) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifyMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyMove event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyMoveEventInfo(
  Action<Battle, ActiveMove, Pokemon, Pokemon?> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
     Id = EventId.ModifyMove;
        Handler = handler;
        Priority = priority;
   UsesSpeed = usesSpeed;
      ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(ActiveMove),
            typeof(Pokemon),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(void);
        
        // Nullability: All parameters non-nullable by default (adjust as needed)
    ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, Move, SourcePokemon, TargetPokemon
    /// </summary>
    public OnModifyMoveEventInfo(
        EventHandlerDelegate contextHandler,
      int? priority = null,
     bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
  
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifyMoveEventInfo Create(
        Action<Battle, ActiveMove, Pokemon, Pokemon?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
    return new OnModifyMoveEventInfo(
     context =>
   {
          handler(
    context.Battle,
    context.GetMove(),
         context.GetSourcePokemon(),
         context.TargetPokemon  // Direct access, can be null
     );
  return null;
  },
   priority,
      usesSpeed
        );
 }
}
