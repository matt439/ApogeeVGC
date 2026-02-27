using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnStallMove event.
/// Triggered to check if a stall move (Protect, Detect, etc.) succeeds.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon) => BoolVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnStallMoveEventInfo : EventHandlerInfo
{
    public OnStallMoveEventInfo(
        EventHandlerDelegate contextHandler,
 int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.StallMove;
        ContextHandler = contextHandler;
   Priority = priority;
  UsesSpeed = usesSpeed;
    }
    
  /// <summary>
    /// Creates strongly-typed context-based handler.
/// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnStallMoveEventInfo Create(
Func<Battle, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
  return new OnStallMoveEventInfo(
  context =>
  {
 var result = handler(
  context.Battle,
    context.GetTargetOrSourcePokemon()
     );
 return result switch
 {
   BoolBoolVoidUnion b => (b.Value ? BoolRelayVar.True : BoolRelayVar.False),
 VoidBoolVoidUnion => null,
     _ => null
 };
   },
   priority,
  usesSpeed
   );
    }
}
