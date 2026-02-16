using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;

/// <summary>
/// Event handler info for OnStart event (Condition-specific variant).
/// Triggered when a condition starts on a Pokemon.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, IEffect) => BoolVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnStartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using legacy strongly-typed pattern.
    /// </summary>
    public OnStartEventInfo(
        Func<Battle, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Start;
        Prefix = EventPrefix.None;
        Handler = handler;
      Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
          [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
        // Nullability: All parameters non-nullable by default (adjust as needed)
 ParameterNullability = new[] { false, false, false, false };
     ReturnTypeNullable = false;
    
   // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, SourceEffect
    /// </summary>
    public OnStartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Start;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
     Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnStartEventInfo Create(
        Func<Battle, Pokemon, Pokemon, IEffect, RelayVar?> handler,
        int? priority = null,
 bool usesSpeed = true)
    {
    return new OnStartEventInfo(
            context => handler(
      context.Battle,
                context.GetTargetOrSourcePokemon(),
         context.GetSourceOrTargetPokemon(),
      context.GetSourceEffect<IEffect>()
  ),
        priority,
   usesSpeed
        );
    }
}