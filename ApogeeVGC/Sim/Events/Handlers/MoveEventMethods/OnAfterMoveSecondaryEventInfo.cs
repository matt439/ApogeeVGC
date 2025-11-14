using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnAfterMoveSecondary event (move-specific).
/// Triggered after move secondary effects.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnAfterMoveSecondaryEventInfo : EventHandlerInfo
{
    public OnAfterMoveSecondaryEventInfo(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
   int? priority = null,
bool usesSpeed = true)
    {
Id = EventId.AfterMoveSecondary;
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
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move
    /// </summary>
    public OnAfterMoveSecondaryEventInfo(
  EventHandlerDelegate contextHandler,
  int? priority = null,
 bool usesSpeed = true)
    {
 Id = EventId.AfterMoveSecondary;
   Prefix = EventPrefix.None;
 ContextHandler = contextHandler;
  Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnAfterMoveSecondaryEventInfo Create(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
  int? priority = null,
        bool usesSpeed = true)
{
return new OnAfterMoveSecondaryEventInfo(
context =>
   {
handler(
    context.Battle,
   context.GetTargetPokemon(),
   context.GetSourcePokemon(),
  context.GetMove()
       );
    return null;
   },
 priority,
   usesSpeed
  );
    }
}