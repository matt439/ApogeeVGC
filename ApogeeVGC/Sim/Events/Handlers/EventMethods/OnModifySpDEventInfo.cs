using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySpD event.
/// Modifies the Special Defense stat.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => DoubleVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifySpDEventInfo : EventHandlerInfo
{
    public OnModifySpDEventInfo(
   EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
     Id = EventId.ModifySpD;
        ContextHandler = contextHandler;
     Priority = priority;
     UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifySpDEventInfo Create(
 Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
    int? priority = null,
bool usesSpeed = true)
    {
 return new OnModifySpDEventInfo(
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
