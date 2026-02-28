using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for BasePowerCallback event (move-specific).
/// Callback for calculating base power dynamically.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => IntFalseUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record BasePowerCallbackEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, SourcePokemon, TargetPokemon, Move
    /// </summary>
    public BasePowerCallbackEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BasePowerCallback;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static BasePowerCallbackEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
return new BasePowerCallbackEventInfo(
 context =>
 {
     var result = handler(
    context.Battle,
   context.GetSourceOrTargetPokemon(),
        context.GetTargetOrSourcePokemon(),
      context.GetMove()
    );
  if (result == null) return null;
   return result switch
  {
    IntIntFalseUnion i => IntRelayVar.Get(i.Value),
     FalseIntFalseUnion => BoolRelayVar.False,
    _ => null
       };
    },
     priority,
   usesSpeed
        );
    }
}
