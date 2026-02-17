using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyAtk event.
/// Modifies the Attack stat.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => DoubleVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifyAtkEventInfo : EventHandlerInfo
{
    public OnModifyAtkEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
bool usesSpeed = true)
    {
      Id = EventId.ModifyAtk;
        ContextHandler = contextHandler;
 Priority = priority;
     UsesSpeed = usesSpeed;
    }
  
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifyAtkEventInfo Create(
 Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
int? priority = null,
        bool usesSpeed = true)
  {
 return new OnModifyAtkEventInfo(
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
         DoubleDoubleVoidUnion d => new DecimalRelayVar((decimal)d.Value),
    VoidDoubleVoidUnion => null,
     _ => null
      };
 },
priority,
   usesSpeed
      );
    }
}
