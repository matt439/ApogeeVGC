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
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => BoolVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// 
/// Return values:
/// - false: Prevent the move from executing
/// - null: Prevent the move from executing (same as false)
/// - VoidReturn or void union: Allow the move to continue
/// </summary>
public sealed record OnBeforeMoveEventInfo : EventHandlerInfo
{
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
                context.GetTargetOrSourcePokemon(),
        context.GetSourceOrTargetPokemon(),
    context.GetMove()
 ),
       priority,
  usesSpeed
        );
    }
}
