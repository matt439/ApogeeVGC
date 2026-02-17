using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeSetWeather event.
/// Signature: Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>
/// </summary>
public sealed record OnFoeSetWeatherEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnFoeSetWeatherEventInfo(
        Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetWeather;
        Prefix = EventPrefix.Foe;
   #pragma warning disable CS0618
   Handler = handler;
   #pragma warning restore CS0618
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(Condition)];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeSetWeatherEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetWeather;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeSetWeatherEventInfo Create(
        Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeSetWeatherEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<Condition>()
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
