using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSetWeather event.
/// Triggered when weather is set.
/// Signature: (Battle battle, Pokemon target, Pokemon source, Condition weather) => BoolVoidUnion
/// </summary>
public sealed record OnSetWeatherEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSetWeather event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
 /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
  [Obsolete("Use Create factory method instead.")]
  public OnSetWeatherEventInfo(
        Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetWeather;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
   typeof(Battle),
 typeof(Pokemon),
     typeof(Pokemon),
            typeof(Condition),
        ];
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
    public OnSetWeatherEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetWeather;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSetWeatherEventInfo Create(
        Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSetWeatherEventInfo(
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
