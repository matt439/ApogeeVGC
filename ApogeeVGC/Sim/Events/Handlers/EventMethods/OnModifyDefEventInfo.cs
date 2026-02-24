using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyDef event.
/// Modifies the Defense stat.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => DoubleVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifyDefEventInfo : EventHandlerInfo
{
    public OnModifyDefEventInfo(
EventHandlerDelegate contextHandler,
        int? priority = null,
bool usesSpeed = true)
    {
      Id = EventId.ModifyDef;
  ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifyDefEventInfo Create(
  Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
      int? priority = null,
      bool usesSpeed = true)
    {
        return new OnModifyDefEventInfo(
     context =>
{
   var result = handler(
       context.Battle,
     context.GetIntRelayVar(),
    context.GetTargetOrSourcePokemon(),
       context.GetSourceOrTargetPokemon(),
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
