using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyPseudoWeatherChange event.
/// Signature: Action<Battle, Pokemon, Pokemon, Condition>
/// </summary>
public sealed record OnAnyPseudoWeatherChangeEventInfo : EventHandlerInfo
{
    public OnAnyPseudoWeatherChangeEventInfo(
        Action<Battle, Pokemon, Pokemon, Condition> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.PseudoWeatherChange;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(Condition)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyPseudoWeatherChangeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.PseudoWeatherChange;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyPseudoWeatherChangeEventInfo Create(
        Action<Battle, Pokemon, Pokemon, Condition> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyPseudoWeatherChangeEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetSourceEffect<Condition>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}