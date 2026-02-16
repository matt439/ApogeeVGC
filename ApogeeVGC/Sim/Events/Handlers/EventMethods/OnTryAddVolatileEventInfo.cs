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
    /// <summary>
    /// Creates a new OnTryAddVolatile event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTryAddVolatileEventInfo(
Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
   int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.TryAddVolatile;
Handler = handler;
        Priority = priority;
   UsesSpeed = usesSpeed;
    ExpectedParameterTypes =
        [
   typeof(Battle),
   typeof(Condition),
   typeof(Pokemon),
   typeof(Pokemon),
   typeof(IEffect),
 ];
  ExpectedReturnType = typeof(BoolVoidUnion);
   
    // Nullability: All parameters non-nullable by default (adjust as needed)
      ParameterNullability = [false, false, false, false, false];
  ReturnTypeNullable = false;
    
// Validate configuration
     ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, Condition (via SourceEffect), TargetPokemon, SourcePokemon, SourceEffect
    /// </summary>
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
     context.GetRelayVarEffect<Condition>(),
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
