using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyPriority event.
/// Modifies move priority.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => DoubleVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifyPriorityEventInfo : EventHandlerInfo
{
    public OnModifyPriorityEventInfo(
  EventHandlerDelegate contextHandler,
  int? priority = null,
        bool usesSpeed = true)
    {
      Id = EventId.ModifyPriority;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
 
    /// <summary>
/// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifyPriorityEventInfo Create(
      Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
int? priority = null,
      bool usesSpeed = true)
    {
  return new OnModifyPriorityEventInfo(
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
