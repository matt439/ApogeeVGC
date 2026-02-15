using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterSetStatus event.
/// Triggered after a status condition is applied to a foe Pokemon.
/// Signature: (Battle battle, Condition status, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnFoeAfterSetStatusEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAfterSetStatus event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeAfterSetStatusEventInfo(
Action<Battle, Condition, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
 Id = EventId.AfterSetStatus;
        Prefix = EventPrefix.Foe;
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
    public OnFoeAfterSetStatusEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSetStatus;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeAfterSetStatusEventInfo Create(
        Action<Battle, Condition, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeAfterSetStatusEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetSourceEffect<Condition>(),
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
