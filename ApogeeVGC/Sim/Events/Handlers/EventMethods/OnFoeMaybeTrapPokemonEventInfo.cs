using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeMaybeTrapPokemon event.
/// Triggered to potentially trap a foe Pokemon.
/// Signature: (Battle battle, Pokemon pokemon, Pokemon? source) => void
/// </summary>
public sealed record OnFoeMaybeTrapPokemonEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates a new OnFoeMaybeTrapPokemon event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
  [Obsolete("Use Create factory method instead.")]
  public OnFoeMaybeTrapPokemonEventInfo(
   Action<Battle, Pokemon, Pokemon?> handler,
        int? priority = null,
     bool usesSpeed = true)
    {
        Id = EventId.MaybeTrapPokemon;
        Prefix = EventPrefix.Foe;
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
  ];
        ExpectedReturnType = typeof(void);
   
        // Nullability: Battle (non-null), Pokemon (non-null), Pokemon source (nullable)
  ParameterNullability = [false, false, true];
  ReturnTypeNullable = false; // void
        
   // Validate configuration
 ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeMaybeTrapPokemonEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.MaybeTrapPokemon;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeMaybeTrapPokemonEventInfo Create(
        Action<Battle, Pokemon, Pokemon?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeMaybeTrapPokemonEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.SourcePokemon
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
