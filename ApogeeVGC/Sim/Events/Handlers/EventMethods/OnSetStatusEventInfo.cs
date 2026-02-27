using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSetStatus event.
/// Triggered when attempting to set a status condition, can prevent it.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Condition, Pokemon, Pokemon, IEffect) => BoolVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnSetStatusEventInfo : EventHandlerInfo
{
    public OnSetStatusEventInfo(
  EventHandlerDelegate contextHandler,
        int? priority = null,
   bool usesSpeed = true)
    {
  Id = EventId.SetStatus;
        ContextHandler = contextHandler;
Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnSetStatusEventInfo Create(
    Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
  return new OnSetStatusEventInfo(
     context =>
{
     var result = handler(
  context.Battle,
 context.GetEffectParam<Condition>(),
   context.GetTargetOrSourcePokemon(),
   context.GetSourceOrTargetPokemon(),
context.GetSourceEffect<IEffect>()
      );
     // null from handler = TS null = "silent failure" (block the status)
     // This must be falsy so RunEvent propagates it as a blocking result
     if (result == null) return BoolRelayVar.False;
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
