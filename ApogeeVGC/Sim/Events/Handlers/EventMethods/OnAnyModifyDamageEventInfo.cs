using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyModifyDamage event.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => DoubleVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnAnyModifyDamageEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, RelayVar (int damage), SourcePokemon, TargetPokemon, Move
    /// </summary>
    public OnAnyModifyDamageEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyDamage;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnAnyModifyDamageEventInfo Create(
      Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
 bool usesSpeed = true)
  {
   return new OnAnyModifyDamageEventInfo(
context =>
     {
   var result = handler(
      context.Battle,
     context.GetIntRelayVar(),
     context.GetSourceOrTargetPokemon(),
       context.GetTargetOrSourcePokemon(),
   context.GetMove()
   );
     return result switch
  {
        DoubleDoubleVoidUnion => null,
 VoidDoubleVoidUnion => null,
       _ => null
  };
  },
  priority,
 usesSpeed
   );
    }
}
