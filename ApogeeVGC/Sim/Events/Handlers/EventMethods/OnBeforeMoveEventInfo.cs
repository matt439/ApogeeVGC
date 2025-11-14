using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnBeforeMove event.
/// Triggered before a Pokemon uses a move, can prevent the move.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => BoolVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnBeforeMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnBeforeMove event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate with explicit parameters</param>
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
  
    /// <summary>
    /// Creates a new OnBeforeMove event handler using the context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move
    /// </summary>
    /// <param name="contextHandler">The context-based handler</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnBeforeMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeMove;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
   
      // No need for ExpectedParameterTypes with context handlers
        // The EventContext provides everything
    }
    
    /// <summary>
    /// Creates a strongly-typed context-based handler.
    /// This gives you the best of both worlds: context pattern with compile-time type safety.
    /// </summary>
    /// <param name="handler">Strongly-typed handler: (battle, target, source, move) => RelayVar?</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public static OnBeforeMoveEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, RelayVar?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnBeforeMoveEventInfo(
       context => handler(
         context.Battle,
                context.GetTargetPokemon(),
        context.GetSourcePokemon(),
    context.GetMove()
 ),
       priority,
  usesSpeed
        );
    }
}
