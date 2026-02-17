using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeSetStatus event.
/// Triggered when attempting to set a status on a foe Pokemon.
/// Signature: (Battle battle, Condition status, Pokemon target, Pokemon source, IEffect effect) => BoolVoidUnion?
/// </summary>
public sealed record OnFoeSetStatusEventInfo : EventHandlerInfo
{
    /// <summary>
 /// Creates a new OnFoeSetStatus event handler.
    /// </summary>
  /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnFoeSetStatusEventInfo(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
  int? priority = null,
        bool usesSpeed = true)
  {
    Id = EventId.SetStatus;
  Prefix = EventPrefix.Foe;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
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
    /// </summary>
    public OnFoeSetStatusEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetStatus;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeSetStatusEventInfo Create(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeSetStatusEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetEffectParam<Condition>(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
                );
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
