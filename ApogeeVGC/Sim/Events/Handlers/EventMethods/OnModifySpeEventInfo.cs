using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySpe event.
/// Modifies the Speed stat.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon) => IntVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifySpeEventInfo : EventHandlerInfo
{
    public OnModifySpeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
 bool usesSpeed = true)
    {
        Id = EventId.ModifySpe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifySpeEventInfo Create(
Func<Battle, int, Pokemon, IntVoidUnion> handler,
      int? priority = null,
     bool usesSpeed = true)
    {
    return new OnModifySpeEventInfo(
context =>
    {
       var result = handler(
     context.Battle,
         context.GetIntRelayVar(),
   context.GetTargetOrSourcePokemon()
    );
    return result switch
     {
          IntIntVoidUnion i => IntRelayVar.Get(i.Value),
     VoidIntVoidUnion => null,
     _ => null
       };
  },
            priority,
     usesSpeed
    );
    }
}
