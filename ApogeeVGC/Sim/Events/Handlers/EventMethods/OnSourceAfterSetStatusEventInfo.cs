using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceAfterSetStatus event.
/// Triggered after a status condition is applied when this Pokemon is the source.
/// Signature: (Battle battle, Condition status, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnSourceAfterSetStatusEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSourceAfterSetStatus event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
[Obsolete("Use Create factory method instead.")]
public OnSourceAfterSetStatusEventInfo(
    Action<Battle, Condition, Pokemon, Pokemon, IEffect> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.AfterSetStatus;
  Prefix = EventPrefix.Source;
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
ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceAfterSetStatusEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSetStatus;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceAfterSetStatusEventInfo Create(
        Action<Battle, Condition, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceAfterSetStatusEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetEffectParam<Condition>(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
