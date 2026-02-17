using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryAddVolatile event.
/// Triggered when attempting to add a volatile condition.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Condition, Pokemon, Pokemon, IEffect) => BoolVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnTryAddVolatileEventInfo : EventHandlerInfo
{
 public OnTryAddVolatileEventInfo(
   EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
{
  Id = EventId.TryAddVolatile;
        ContextHandler = contextHandler;
   Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnTryAddVolatileEventInfo Create(
   Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
  int? priority = null,
   bool usesSpeed = true)
 {
return new OnTryAddVolatileEventInfo(
     context =>
{
 var result = handler(
   context.Battle,
     context.GetEffectParam<Condition>(),
   context.GetTargetOrSourcePokemon(),
   context.GetSourceOrTargetPokemon(),
     context.GetSourceEffect<IEffect>()
    );
  if (result == null) return null;
     return result switch
   {
      BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
      VoidBoolVoidUnion => null,
 _ => null
      };
 },
   priority,
 usesSpeed
        );
    }
}
